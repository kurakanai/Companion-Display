using CompanionDisplayWinUI.ClassImplementations;
using DiscordRPC;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using Windows.Media.Protection.PlayReady;

namespace CompanionDisplayWinUI.API
{
    public static class DiscordAPI
    {
        public static DiscordRpcClient discordRpcClient;
        static DiscordAPI()
        {
            try
            {
                discordRpcClient = new DiscordRpcClient(Globals.DiscordID);
                discordRpcClient.OnReady += delegate (object sender, ReadyMessage e)
                {
                    Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };
                discordRpcClient.OnPresenceUpdate += delegate (object sender, PresenceMessage e)
                {
                    Console.WriteLine("Received Update! {0}", e.Presence);
                };
                if (!Globals.disableDiscord)
                {
                    discordRpcClient.Initialize();
                }
            }
            catch
            {
            }
        }
        public static RichPresence PresenceBuilder(TimeSpan songElapsed, TimeSpan songEnd)
        {
            DateTime dt = DateTime.Now.ToUniversalTime().Add(-songElapsed);
            DateTime dt2 = DateTime.Now.ToUniversalTime().Add(-songElapsed + songEnd);
            string details = MusicAPI.currentSong.title;
            if (details.Length > 128)
            {
                details = details[..125] + "...";
            }
            RichPresence presence = new()
            {
                Details = details,
                State = MusicAPI.buildDetails(),
                Timestamps = new Timestamps
                {
                    Start = dt,
                    End = dt2
                },
                Assets = new Assets
                {
                    LargeImageKey = MusicAPI.currentSong.albumCoverUrl,
                    LargeImageText = MusicAPI.buildDetails(),
                    SmallImageText = MusicAPI.currentSong.album,
                    SmallImageKey = "mini_logo"
                },
            };
            if (MusicAPI.currentLyric != null)
            {
                if (details.Length >= 128)
                {
                    details = details[..124] + "...";
                }
                if (MusicAPI.currentSong.title != null && MusicAPI.currentSong.title.Length >= 128)
                {
                    MusicAPI.currentSong.title = MusicAPI.currentSong.title[..124] + "...";
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
                    presence.State = MusicAPI.currentLyric;
                }
                catch
                {
                    if (details != null && details.Length >= 50)
                    {
                        presence.State = MusicAPI.currentLyric.Remove(50, details.Length - 50) + "...";
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
                    Url = "https://github.com/kurakanai/Companion-Display/releases"
                });
            }

            presence.Buttons = buttons.ToArray();
            presence.Type = ActivityType.Listening;
            return presence;
        }
        private static RichPresence comparisonPresence = new();
        public static void PushPresenceDiscord(RichPresence presence)
        {
            try
            {
                bool checkPlaybackInfo = (MusicAPI.playbackInfo != null && (MusicAPI.playbackInfo.PlaybackStatus == (GlobalSystemMediaTransportControlsSessionPlaybackStatus)4));
                if (discordRpcClient != null)
                {
                    if (presence.State != comparisonPresence.State || presence.Details != comparisonPresence.Details)
                    {
                        comparisonPresence.State = presence.State;
                        comparisonPresence.Details = presence.Details;
                        discordRpcClient.SetPresence(presence);
                    }
                }
                else
                {
                    discordRpcClient?.ClearPresence();
                }
            }
            catch { }
        }
    }
}
