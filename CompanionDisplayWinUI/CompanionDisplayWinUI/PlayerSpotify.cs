using DiscordRPC.Message;
using DiscordRPC;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Control;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Globalization;
using CompanionDisplayWinUI.ClassImplementations;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CompanionDisplayWinUI
{
    class PlayerSpotify
    {
        private string Toki, TokiA, TokiO, TitleSongOffline, AlbumCoverBase2, SongID, SongIDCache, timestamp, timestamp2;
        bool changed = false;
        public TimeSpan ts, ts2;
        private int currenttimestamp = 0, endtimestamp = 0;
        public const byte VK_MEDIA_PLAY_PAUSE = 179, VK_MEDIA_NEXT = 176, VK_MEDIA_PREV = 177;
        public DiscordRpcClient client;
        private readonly Uri BaseUri = new("http://localhost:5543/callback");
        private readonly PlayerCurrentlyPlayingRequest request2 = new();
        // Media rework effort
        // Start Logic
        public void Page_Loaded()
        {
            Thread thread = new(StartThread);
            thread.Start();
        }
        private void StartThread()
        {
            LoadConfigs();
            PreStart();
        }
        public void PreStart()
        {
            try
            {
                Initialize();
                LoadDiscordRPC();
            }
            catch { }
            try
            {
                InitSessionManager();
            }
            catch { }
        }
        // End Of Start Logic
        // Start Music Info
        public void PerformLyricShit()
        {
            while (Globals.StartedPlayer)
            {
                ActuallyDoShit();
                Thread.Sleep(1000);
            }
        }
        private void ActuallyDoShit()
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
            ClientSpotifyLyrics();
            if (!Globals.IsSpotify)
            {
                if (!changed)
                {
                    GetExactLyric();
                }
                else
                {
                    changed = false;
                    GetLyrics();
                }
            }
            PushPresenceDiscord(PresenceBuilder(Globals.IsSpotify));
        }
        // End Music Info
        // Spotify Token Logic
        private static void GetLyrics()
        {
            try
            {
                if (Media.SongName != "")
                {
                    using HttpClient client2 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false });
                    Media.nonTimedLyrics = null;
                    string url2 = "https://lrclib.net/api/search?q=" + HttpUtility.UrlEncode(Media.SongName + " " + Media.ArtistName.Replace(" - Topic", "") + " " + Media.AlbumName);
                    client2.DefaultRequestHeaders.Clear();
                    client2.DefaultRequestHeaders.Add("User-Agent", "Companion Display " + Globals.Version + " (https://github.com/yagdev/Companion-Display)");
                    var response2 = client2.GetStringAsync(url2);
                    StringReader readerlyric0 = new(response2.Result);
                    JToken array = JArray.Parse(response2.Result);
                    int count = array.Count();
                    int i = 0;
                    double testa = TimeSpan.ParseExact(Media.SongEnd, @"m\:ss", null).TotalMilliseconds;
                    while (JArray.Parse(response2.Result)[i]["syncedLyrics"].ToString() == "" || Math.Abs(double.Parse(array[i]["duration"].ToString()) * 1000 - testa) > 3000)
                    {
                        int ia = Math.Abs(int.Parse(TimeSpan.ParseExact(Media.SongEnd, @"m\:ss", null).TotalMilliseconds.ToString()) - (int)(double.Parse(JArray.Parse(response2.Result)[i]["duration"].ToString()) * 1000));
                        if (JArray.Parse(response2.Result)[i]["syncedLyrics"].ToString() == "" && Math.Abs(double.Parse(array[i]["duration"].ToString()) * 1000 - testa) <= 3000)
                        {
                            Media.nonTimedLyrics = JArray.Parse(response2.Result)[i]["plainLyrics"].ToString();
                        }
                        i++;
                    }
                    int isd = Math.Abs(int.Parse(TimeSpan.ParseExact(Media.SongEnd, @"m\:ss", null).TotalMilliseconds.ToString()) - (int)(double.Parse(JArray.Parse(response2.Result)[i]["duration"].ToString()) * 1000));
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
                            lyricsList.Add(match.Groups[2].Value.Trim());
                        }
                    }
                    Media.LyricTimings = [.. timestamps];
                    Media.Lyrics = [.. lyricsList];
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                Media.currentLyric = 0;
                Media.Lyrics = null;
                Media.LyricTimings = null;
            }
        }
        private void GetExactLyric()
        {
            try
            {
                int i = 0;
                while (currenttimestamp > Media.LyricTimings[i])
                {
                    i++;
                }
                Media.SongLyrics = Media.Lyrics[i - 1];
                Media.currentLyric = i - 1;
                Media.SongLyricChanged();
            }
            catch { }
        }
        public void Initialize()
        {
            RefreshToken();
            Thread thread = new(PerformLyricShit);
            thread.Start();
        }
        public async void RefreshToken()
        {
            SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();
            EmbedIOAuthServer server = new(new Uri("http://localhost:5543/callback"), 5543);
            try
            {
                if (Globals.RefreshToken != "")
                {
                    TokiO = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, Globals.RefreshToken))).AccessToken;
                    Toki = TokiO;
                    RefreshToken2();
                    await server.Stop();
                }
                else
                {
                    server.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                    {
                        AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId, Globals._secretId, response.Code, BaseUri));
                        await server.Stop();
                        TokiO = tokenResponse.AccessToken;
                        Globals.RefreshToken = tokenResponse.RefreshToken;
                        File.WriteAllText(Globals.RefreshTokenPath, tokenResponse.RefreshToken);
                        RefreshToken2();
                    };
                    await server.Start();
                    LoginRequest loginRequest = new(server.BaseUri, Globals._clientId, LoginRequest.ResponseType.Code)
                    {
                        Scope = ["user-read-currently-playing", "user-read-playback-state", "user-read-recently-played"]
                    };
                    BrowserUtil.Open(loginRequest.ToUri());
                }
            }
            catch
            {
                await server.Stop();
                RefreshToken2();
            }

        }
        private async void RefreshToken2()
        {
            SpotifyClientConfig config1 = SpotifyClientConfig.CreateDefault();
            EmbedIOAuthServer server1 = new(new Uri("http://localhost:5543/callback"), 5543);
            try
            {
                if (Globals.RefreshToken2 != "")
                {
                    TokiA = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId2, Globals._secretId2, Globals.RefreshToken2))).AccessToken;
                }
                else
                {
                    server1.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                    {
                        AuthorizationCodeTokenResponse tokenResponse1 = await new OAuthClient(config1).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId2, Globals._secretId2, response.Code, BaseUri));
                        await server1.Stop();
                        TokiA = tokenResponse1.AccessToken;
                        Globals.RefreshToken2 = tokenResponse1.RefreshToken;
                        File.WriteAllText(Globals.RefreshToken2Path, tokenResponse1.RefreshToken);
                    };
                    await server1.Start();
                    LoginRequest loginRequest1 = new(server1.BaseUri, Globals._clientId2, LoginRequest.ResponseType.Code)
                    {
                        Scope = ["user-read-currently-playing", "user-read-playback-state", "user-read-recently-played"]
                    };
                    BrowserUtil.Open(loginRequest1.ToUri());
                }
            }
            catch
            {
                using var _ = server1.Stop();
            }
        }
        // Spotify Token Logic End
        private void ClientSpotifyLyrics()
        {
            using HttpClient client0 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false });
            bool cache = Globals.IsSpotify;
            try
            {
                string url = "https://api.spotify.com/v1/me/player/currently-playing";
                client0.DefaultRequestHeaders.Add("Authorization", "Bearer " + Toki);
                using Task<string> response = client0.GetStringAsync(url);
                if ((string)JObject.Parse(response.Result)["is_playing"] == "True")
                {
                    Globals.IsSpotify = true;
                    responsedecode = JObject.Parse(response.Result.ToString());
                    SongID = responsedecode["item"]["id"].ToString();
                    Media.ArtistName = responsedecode["item"]["artists"][0]["name"].ToString();
                    Media.SongName = responsedecode["item"]["name"].ToString();
                    timestamp = responsedecode["progress_ms"].ToString();
                    timestamp2 = responsedecode["item"]["duration_ms"].ToString();
                    Globals.SongID = responsedecode["item"]["external_ids"]["isrc"].ToString();
                    Media.AlbumName = responsedecode["item"]["album"]["name"].ToString();
                    Media.SongBackground = responsedecode["item"]["album"]["images"][0]["url"].ToString();
                    Media.SongBackgroundDiscord = responsedecode["item"]["album"]["images"][0]["url"].ToString();
                    Media.SongDetails = responsedecode["item"]["artists"][0]["name"].ToString() + " · " + responsedecode["item"]["album"]["release_date"].ToString();
                    currenttimestamp = int.Parse(timestamp);
                    Globals.currenttimestamp = int.Parse(timestamp);
                    Media.SetTime(TimeSpan.FromMilliseconds(double.Parse(responsedecode["progress_ms"].ToString())).ToString(@"m\:ss"), TimeSpan.FromMilliseconds(double.Parse(responsedecode["item"]["duration_ms"].ToString())).ToString(@"m\:ss"), (double.Parse(timestamp) / double.Parse(timestamp2)) * 100);
                    ts = TimeSpan.FromMilliseconds(currenttimestamp);
                    endtimestamp = int.Parse(timestamp2);
                    ts2 = TimeSpan.FromMilliseconds(endtimestamp);
                    if (SongID != SongIDCache)
                    {
                        Media.SetLyrics(0, "");
                        SongIDCache = SongID;
                        GetLyrics();
                        SongIDCache = SongID;
                        Media.SongChanged();
                    }
                    GetExactLyric();
                }
                else
                {
                    Globals.IsSpotify = false;
                }
            }
            catch (Exception ex)
            {
                Globals.IsSpotify = false;
                if (ex.InnerException != null && ex.InnerException.Message.Contains("401"))
                {
                    RefreshToken();
                }
                if (Toki == TokiO && TokiA != null)
                {
                    Toki = TokiA;
                }
                else
                {
                    Toki = TokiO;
                }
            }
            if (cache && !Globals.IsSpotify)
            {
                SongIDCache = "";
                Media.SetLyrics(0, "");
            }
        }
        // Spotify Check End
        // Spotify DB
        private void RequestSongID()
        {
            try
            {
                using HttpClient client2 = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false });
                string name = Media.ArtistName;
                string url2 = "https://musicbrainz.org/ws/2/recording/?query=" + HttpUtility.UrlEncode(Regex.Replace(Media.SongName, @"[^\w\s]", "")) + "+AND+artist:" + HttpUtility.UrlEncode(Media.ArtistName.Replace(" - Topic", "")) + "&release-group-type=album,single,ep,lp&fmt=json";
                client2.DefaultRequestHeaders.Clear();
                client2.DefaultRequestHeaders.Add("User-Agent", "Companion Display " + Globals.Version + " (https://github.com/yagdev/Companion-Display)");
                var response2 = client2.GetStringAsync(url2);
                StringReader readerlyric0 = new(response2.Result);
                JToken array = JToken.Parse(response2.Result)["recordings"];
                int count = array.Count();
                int i = 0;
                double testa = TimeSpan.ParseExact(Media.SongEnd, @"m\:ss", null).TotalMilliseconds;
                while (array[i]["length"] == null || Math.Abs((int)array[i]["length"] - testa) > 3000)
                {
                    i++;
                }
                Media.SongDetails = array[i]["artist-credit"][0]["name"].ToString() + " · " + array[i]["first-release-date"].ToString();
                Media.AlbumName = array[i]["releases"][0]["title"].ToString();
                Media.SongBackgroundDiscord = "https://coverartarchive.org/release-group/" + array[i]["releases"][0]["release-group"]["id"] + "/front";
            }
            catch { }
            Media.SongChanged();
        }
        // End Spotify DB
        public void LoadDiscordRPC()
        {
            if(client != null)
            {
                client?.Dispose();
            }
            try
            {
                client = new DiscordRpcClient(Globals.DiscordID);
                client.OnReady += delegate (object sender, ReadyMessage e)
                {
                    Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };
                client.OnPresenceUpdate += delegate (object sender, PresenceMessage e)
                {
                    Console.WriteLine("Received Update! {0}", e.Presence);
                };
                client.Initialize();
            }
            catch
            {
            }
        }
        private static void LoadConfigs()
        {
            if (File.Exists(Globals.MediaConfigFile))
            {
                string config = File.ReadAllText(Globals.MediaConfigFile);
                using StringReader readerconfig = new(config);
                {
                    Globals._clientId = readerconfig.ReadLine();
                    Globals._secretId = readerconfig.ReadLine();
                    Globals._clientId2 = readerconfig.ReadLine();
                    Globals._secretId2 = readerconfig.ReadLine();
                }
            }
            if (File.Exists(Globals.RefreshTokenPath))
            {
                Globals.RefreshToken = File.ReadAllText(Globals.RefreshTokenPath);
            }
            if (File.Exists(Globals.RefreshToken2Path))
            {
                Globals.RefreshToken2 = File.ReadAllText(Globals.RefreshToken2Path);
            }
        }
        // End Start Logic
        // Discord Presence Interactions
        private RichPresence PresenceBuilder(bool isSpotify)
        {
            DateTime dt = DateTime.Now.ToUniversalTime().Add(-ts);
            DateTime dt2 = DateTime.Now.ToUniversalTime().Add(-ts + ts2);
            string details = Media.SongName + " · " + Media.SongDetails;
            if (details.Length > 128)
            {
                details = details[..125] + "...";
            }
            RichPresence presence = new()
            {
                Details = Media.SongName,
                State = Media.SongDetails,
                Timestamps = new Timestamps
                {
                    Start = dt,
                    End = dt2
                },
                Assets = new Assets
                {
                    LargeImageKey = Media.SongBackgroundDiscord,
                    LargeImageText = Media.AlbumName,
                    SmallImageKey = "mini_logo"
                },
            };
            if (Media.SongLyrics != null)
            {
                if (details.Length >= 128)
                {
                    details = details[..124] + "...";
                }
                if (Media.SongName != null && Media.SongName.Length >= 128)
                {
                    Media.SongName = Media.SongName[..124] + "...";
                }
                try
                {
                    presence.Details = details + "";
                }
                catch
                {
                    try
                    {
                        presence.Details = details[..50] + "...";
                    }
                    catch { }
                }
                try
                {
                    presence.State = Media.SongLyrics;
                }
                catch
                {
                    if (details != null && details.Length >= 50)
                    {
                        presence.State = Media.SongLyrics.Remove(50, details.Length - 50) + "...";
                    }
                    else
                    {
                        presence.State = "";
                    }
                }
            }
            var buttons = new List<Button>();

            if (Globals.showPromo)
            {
                buttons.Add(new Button
                {
                    Label = "Get Companion Display",
                    Url = "https://github.com/yagdev/Companion-Display/releases"
                });
            }

            if (isSpotify)
            {
                buttons.Add(new Button
                {
                    Label = "View on Spotify",
                    Url = "https://open.spotify.com/track/" + SongID
                });
            }
            presence.Buttons = buttons.ToArray();
            return presence;
        }
        private RichPresence comparisonPresence = new();
        private void PushPresenceDiscord(RichPresence presence)
        {
            try
            {
                if ((client != null && Globals.playbackInfo != null && Globals.playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing) || Globals.IsSpotify)
                {
                    if (presence.State != comparisonPresence.State || presence.Details != comparisonPresence.Details)
                    {
                        comparisonPresence.State = presence.State;
                        comparisonPresence.Details = presence.Details;
                        client.SetPresence(presence);
                    }
                }
                else
                {
                    client?.ClearPresence();
                }
            }
            catch { }
        }
        // Discord Presence Interactions End
        // Offline Player Interactions
        private async void InitSessionManager()
        {
            Globals.sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            Globals.sessionManager.SessionsChanged += UpdateSessions;
            UpdateSessions(Globals.sessionManager, null);
        }
        private void UpdateSessions(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            try
            {
                Globals.currentSession = sender.GetCurrentSession();
                Globals.currentSession.MediaPropertiesChanged -= UpdateInfo;
                Globals.currentSession.MediaPropertiesChanged += UpdateInfo;
                Globals.currentSession.TimelinePropertiesChanged += UpdateTiming;
                Globals.playbackInfo = Globals.currentSession.GetPlaybackInfo();
                UpdateInfo(Globals.currentSession, null);
                UpdateTiming(Globals.currentSession, null);
            }
            catch { }
        }
        private void UpdateTiming(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args)
        {
            Media.SongTimingChanged();
            try
            {
                Globals.timelineInfo = sender.GetTimelineProperties();
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(Globals.timelineInfo.Position.TotalMilliseconds);
                ts = timeSpan;
                currenttimestamp = (int)timeSpan.TotalMilliseconds;
                TimeSpan timeSpan2 = TimeSpan.FromMilliseconds(Globals.timelineInfo.EndTime.TotalMilliseconds);
                ts2 = timeSpan2;
                Media.SetTime(timeSpan.ToString(@"m\:ss"), timeSpan2.ToString(@"m\:ss"), Globals.timelineInfo.Position.TotalMilliseconds / Globals.timelineInfo.EndTime.TotalMilliseconds * 100.0);
                GetExactLyric();
            }
            catch { }
        }
        private async void UpdateInfo(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            if (sender != null && !Globals.IsSpotify)
            {
                try
                {
                    GlobalSystemMediaTransportControlsSessionMediaProperties s = await sender.TryGetMediaPropertiesAsync();
                    Globals.songInfo = await sender.TryGetMediaPropertiesAsync();
                    if (TitleSongOffline != Globals.songInfo.Title)
                    {
                        TitleSongOffline = Globals.songInfo.Title;
                    }
                    Media.SongName = Globals.songInfo.Title;
                    if (Globals.songInfo.Artist != "")
                    {
                        Media.SongDetails = Globals.songInfo.Artist;
                        Media.ArtistName = Globals.songInfo.Artist;
                    }
                    else
                    {
                        Media.SongDetails = Globals.songInfo.AlbumArtist;
                    }
                    Media.AlbumName = Globals.songInfo.AlbumTitle;
                    GetLyrics();
                    RequestSongID();
                    Media.SetLyrics(-1, "");
                    Media.SongChanged();
                }
                catch { }
                changed = true;
            }
        }
        // Offline Player Interactions End
        // Media rework effort end
        private JObject responsedecode;
    }
}