using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Windows.Media.Control;

namespace CompanionDisplayWinUI.API
{
    static class MusicAPI
    {
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs CallInfoUpdate, CallTimingUpdate, CallLyricUpdate, CallCoverUpdate;
        private static GlobalSystemMediaTransportControlsSessionManager sessionManager;
        public static GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo;
        public static GlobalSystemMediaTransportControlsSession currentSession;
        public static GlobalSystemMediaTransportControlsSessionMediaProperties sessionMediaProperties;
        public static GlobalSystemMediaTransportControlsSessionTimelineProperties timelineProperties;
        public static SongObject currentSong = new();
        private static TimeSpan songElapsed, songEnd;
        public static string currentLyric;
        private static bool enableConstantDetection = false;
        private static bool runningConstantDetection = false;
        private static double songEndMs;
        public static string songElapsedFormatted, songEndFormatted;
        public static double songProgress;
        public static int currentLyricIndex;
        private static HttpClient httpWrapper;
        private static readonly SemaphoreSlim semaphore = new(1);
        static MusicAPI()
        {
            // Pass onto second thread
            Thread thread = new(StartThread);
            thread.Start();
        }
        private static void StartThread()
        {
            httpWrapper = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false });
            httpWrapper.DefaultRequestHeaders.Add("User-Agent", "Companion Display " + Globals.Version + " (https://github.com/kurakanai/Companion-Display)");
            initializeLocalMedia();
        }
        private async static void initializeLocalMedia()
        {
            progressSmoother.Start();
            callerTimer.Elapsed += callTimeUpdate;
            callerTimer.Start();
            sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            sessionManager.SessionsChanged += UpdateSessions;
            UpdateSessions(sessionManager, null);
        }

        private static void callTimeUpdate(object sender, ElapsedEventArgs e)
        {
            actuallyUpdateTiming();
        }

        private static async void UpdateSessions(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            try
            {
                currentSession = sender.GetCurrentSession();
                currentSession.MediaPropertiesChanged -= UpdateInfo;
                currentSession.MediaPropertiesChanged += UpdateInfo;
                currentSession.TimelinePropertiesChanged -= UpdateTiming;
                currentSession.TimelinePropertiesChanged += UpdateTiming;
                currentSession.PlaybackInfoChanged -= checkStatus;
                currentSession.PlaybackInfoChanged += checkStatus;
                playbackInfo = currentSession.GetPlaybackInfo();
                currentSong = new();
            }
            catch { }
            Thread thread = new Thread(async () =>
            {
                UpdateInfo(currentSession, null);
                UpdateTiming(currentSession, null);
            });
            thread.Start();
        }
        private static Stopwatch progressSmoother = new();
        private static double syncOffset = 0;
        public static double songElapsedMs()
        {
            return progressSmoother.Elapsed.TotalMilliseconds + syncOffset;
        }
        private static async void UpdateInfo(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            await semaphore.WaitAsync();
            try
            {
                sessionMediaProperties = await sender.TryGetMediaPropertiesAsync();
                    string newTitle = sessionMediaProperties.Title;
                    string newAlbum = sessionMediaProperties.AlbumTitle;
                    if (currentSong == null || !currentSong.checkIfSameSimple(newTitle, newAlbum))
                    {
                        currentSong = new SongObject();
                        UpdateTiming(sender, null);
                        actuallyUpdateTiming();
                    }
                    RequestSongID();
                    SetLyrics(-1, "");
                    GetLyrics();
            }
            catch{ }
            SongChanged();
            semaphore.Release();
        }
        private static void UpdateTiming(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args)
        {
            timelineProperties = sender.GetTimelineProperties();
            syncOffset = timelineProperties.Position.TotalMilliseconds - progressSmoother.ElapsedMilliseconds;
            songEndMs = timelineProperties.EndTime.TotalMilliseconds;
            songEnd = TimeSpan.FromMilliseconds(songEndMs);
        }
        private static void actuallyUpdateTiming()
        {
            try
            {
                songElapsed = TimeSpan.FromMilliseconds(songElapsedMs());
                SetTime(songElapsed.ToString(@"m\:ss"), songEnd.ToString(@"m\:ss"), timelineProperties.Position.TotalMilliseconds / timelineProperties.EndTime.TotalMilliseconds * 100.0);
                GetExactLyric();
                DiscordAPI.PushPresenceDiscord(DiscordAPI.PresenceBuilder(songElapsed, songEnd));
            }
            catch
            {

            }
        }
        private static System.Timers.Timer callerTimer = new()
        {
            AutoReset = true,
            Interval = 1000,
        };
        public static void SetTime(string currentTime, string duration, double progress)
        {
            songElapsedFormatted = currentTime;
            songEndFormatted = duration;
            songProgress = progress;
            SongTimingChanged();
        }
        public static void SetLyrics(int index, string lyrics)
        {
            currentLyricIndex = index;
            currentLyric = lyrics;
            SongLyricChanged();
        }
        private static void RequestSongID()
        {
            try
            {
                string queryArtistName = currentSong.artist;
                string querySongName = currentSong.title;
                string queryAlbumName = currentSong.album;
                if (queryAlbumName != "")
                {
                    queryAlbumName = "+AND+release:" + HttpUtility.UrlEncode(queryAlbumName);
                }
                string queryURL = "https://musicbrainz.org/ws/2/recording/?query=" + HttpUtility.UrlEncode(Regex.Replace(querySongName, @"[^\w\s]", "")) + queryAlbumName + "+AND+artist:" + HttpUtility.UrlEncode(queryArtistName.Replace(" - Topic", "")) +"+AND+status:official" + "&release-group-type=album,single,ep,lp&fmt=json";
                var queryResponse = httpWrapper.GetStringAsync(queryURL);
                StringReader readerlyric0 = new(queryResponse.Result);
                JToken array = JToken.Parse(queryResponse.Result)["recordings"];
                int count = array.Count();
                int i = 0;
                double testa = TimeSpan.ParseExact(songEndFormatted, @"m\:ss", null).TotalMilliseconds;
                while (array[i]["length"] == null || Math.Abs((int)array[i]["length"] - testa) > 3000 || (array[i]["releases"][0]["artist-credit"][0]["name"].ToString() == "Various Artists"))
                {
                    i++;
                }
                currentSong.artist = array[i]["artist-credit"][0]["name"].ToString();
                currentSong.setReleaseDate(array[i]["first-release-date"].ToString());
                currentSong.album = array[i]["releases"][0]["title"].ToString();
                currentSong.setAlbumCover("https://coverartarchive.org/release/" + array[i]["releases"][0]["id"] + "/front");
            }
            catch { }
        }
        private static void GetLyrics()
        {
            try
            {
                if (currentSong.title != "")
                {
                    string url2 = "https://lrclib.net/api/search?q=" + HttpUtility.UrlEncode(currentSong.title + " " + currentSong.artist.Replace(" - Topic", "") + " " + currentSong.album);
                    var response2 = httpWrapper.GetStringAsync(url2);
                    StringReader readerlyric0 = new(response2.Result);
                    JToken array = JArray.Parse(response2.Result);
                    int count = array.Count();
                    int i = 0;
                    double testa = TimeSpan.ParseExact(songEndFormatted, @"m\:ss", null).TotalMilliseconds;
                    while (JArray.Parse(response2.Result)[i]["syncedLyrics"].ToString() == "" || Math.Abs(double.Parse(array[i]["duration"].ToString()) * 1000 - testa) > 3000)
                    {
                        int ia = Math.Abs(int.Parse(TimeSpan.ParseExact(songEndFormatted, @"m\:ss", null).TotalMilliseconds.ToString()) - (int)(double.Parse(JArray.Parse(response2.Result)[i]["duration"].ToString()) * 1000));
                        if (JArray.Parse(response2.Result)[i]["syncedLyrics"].ToString() == "" && Math.Abs(double.Parse(array[i]["duration"].ToString()) * 1000 - testa) <= 3000)
                        {
                            currentSong.nonTimedLyrics = JArray.Parse(response2.Result)[i]["plainLyrics"].ToString();
                        }
                        i++;
                    }
                    int isd = Math.Abs(int.Parse(TimeSpan.ParseExact(songEndFormatted, @"m\:ss", null).TotalMilliseconds.ToString()) - (int)(double.Parse(JArray.Parse(response2.Result)[i]["duration"].ToString()) * 1000));
                    string lyrics = JArray.Parse(response2.Result)[i]["syncedLyrics"].ToString();
                    List<double> timestamps = [];
                    List<string> lyricsList = [];
                    foreach (string line in lyrics.Split("\n"))
                    {
                        Match match = Regex.Match(line, @"^\[(\d{2}:\d{2}\.\d{2})\](.*)?");

                        if (match.Success)
                        {
                            double time = TimeSpan.ParseExact(match.Groups[1].Value.Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture).TotalMilliseconds;
                            timestamps.Add(time);
                            string Lyric = match.Groups[2].Value.Trim();
                            if (Lyric == "")
                            {
                                Lyric = "♪‎‎ ";
                            }
                            lyricsList.Add(Lyric);
                        }
                    }
                    currentSong.timedLyricsTimestamps = [.. timestamps];
                    currentSong.timedLyricsText = [.. lyricsList];
                }
                else
                {
                    throw new Exception();
                }
                currentSong.setLyricsType();
            }
            catch
            {
                currentLyricIndex = 0;
            }
        }
        private static void GetExactLyric()
        {
            try
            {
                int i = 0;
                double songElapsed = songElapsedMs();
                while (songElapsed > currentSong.timedLyricsTimestamps[i])
                {
                    i++;
                }
                currentLyric = currentSong.timedLyricsText[i - 1];
                currentLyricIndex = i - 1;
                SongLyricChanged();
            }
            catch { }
        }
        public static string buildDetails()
        {
            string details = currentSong.artist; 
            if(currentSong.hasReleaseDate)
            {
                details += " · " + currentSong.releaseDate;
            }
            return details;
        }
        private static void checkStatus(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
        {
            playbackInfo = currentSession.GetPlaybackInfo();
        }
        // Triggers
        public static void SongChanged()
        {
            CallInfoUpdate?.Invoke();
        }
        public static void SongTimingChanged()
        {
            CallTimingUpdate?.Invoke();
        }
        public static void SongLyricChanged()
        {
            CallLyricUpdate?.Invoke();
        }
        public static void SongCoverChanged()
        {
            CallCoverUpdate?.Invoke();
        }
    }
}
