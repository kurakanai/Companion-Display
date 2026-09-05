using CompanionDisplayWinUI.ClassImplementations;
using DiscordRPC;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using Windows.Media.Control;

namespace CompanionDisplayWinUI.API
{
    public static class DiscordAPI
    {
        public readonly static DiscordRpcClient discordRpcClient;
        static DiscordAPI()
        {
            try
            {
                discordRpcClient = new DiscordRpcClient(Globals.DiscordID);
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
            var now = DateTime.UtcNow;
            var currentSong = MusicAPI.currentSong;
            string details = Truncate(currentSong?.title, 128);
            string state = MusicAPI.currentLyric != null ? Truncate(MusicAPI.currentLyric, 128)  : MusicAPI.BuildDetails();
            return new RichPresence
            {
                Details = details,
                State = state,
                Timestamps = new Timestamps
                {
                    Start = now - songElapsed,
                    End = now - songElapsed + songEnd
                },
                Assets = new Assets
                {
                    LargeImageKey = currentSong?.albumCoverUrl,
                    LargeImageText = MusicAPI.BuildDetails(),
                    SmallImageText = currentSong?.album,
                    SmallImageKey = "mini_logo"
                },
                Buttons = Globals.showPromo ? [
                    new Button
            {
                Label = "Get Companion Display",
                Url = "https://github.com/kurakanai/Companion-Display/releases"
            }
                ] : null,
                Type = ActivityType.Listening
            };
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLength ? value : string.Concat(value.AsSpan(0, maxLength - 3), "...");
        }
        private static RichPresence comparisonPresence = new();
        public static void PushPresenceDiscord(RichPresence presence)
        {
            if (discordRpcClient == null || presence == null)
                return;

            bool isPlaying = MusicAPI.playbackInfo?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;

            if (isPlaying)
            {
                bool hasChanged = presence.State != comparisonPresence?.State || presence.Details != comparisonPresence?.Details;
                if (hasChanged)
                {
                    comparisonPresence = new RichPresence
                    {
                        State = presence.State,
                        Details = presence.Details
                    };
                    discordRpcClient.SetPresence(presence);
                }
            }
            else
            {
                discordRpcClient.ClearPresence();
            }
        }
    }
}
