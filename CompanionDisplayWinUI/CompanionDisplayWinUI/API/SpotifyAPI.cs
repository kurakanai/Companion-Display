using CompanionDisplayWinUI.ClassImplementations;
using CompanionDisplayWinUI.Objects;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CompanionDisplayWinUI.API
{
    static class SpotifyAPI
    {
        private static HttpClient httpWrapper;
        private static string tokenSpotify;
        public static string spotifyID;
        private static Uri BaseUri = new("http://127.0.0.1:5543/callback");
        private static JObject responsedecode;
        public static bool isReady = false, isErr = false;
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs SpotifyEvent;
        static SpotifyAPI()
        {
            ConfigAPI.LoadPlayerSettings();
            httpWrapper = new(new SocketsHttpHandler() { ConnectTimeout = TimeSpan.FromSeconds(2.0), KeepAlivePingTimeout = TimeSpan.FromSeconds(5.0), EnableMultipleHttp2Connections = false });
            LoadConfigs();
            grabToken();
        }
        private static void LoadConfigs()
        {
            if (File.Exists(Globals.RefreshTokenPath))
            {
                Globals.RefreshToken = File.ReadAllText(Globals.RefreshTokenPath);
            }
            if (File.Exists(Globals.RefreshToken2Path))
            {
                Globals.RefreshToken2 = File.ReadAllText(Globals.RefreshToken2Path);
            }
        }
        private async static void grabToken()
        {
            SpotifyClientConfig config = SpotifyClientConfig.CreateDefault();
            EmbedIOAuthServer server = new(new Uri("http://127.0.0.1:5543/callback"), 5543);
            try
            {
                if (Globals.RefreshToken != "")
                {
                    tokenSpotify = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, Globals.RefreshToken))).AccessToken;
                    httpWrapper.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenSpotify);
                    await server.Stop();
                }
                else
                {
                    server.AuthorizationCodeReceived += async delegate (object sender, AuthorizationCodeResponse response)
                    {
                        AuthorizationCodeTokenResponse tokenResponse = await new OAuthClient(config).RequestToken(new AuthorizationCodeTokenRequest(Globals._clientId, Globals._secretId, response.Code, BaseUri));
                        await server.Stop();
                        tokenSpotify = tokenResponse.AccessToken;
                        Globals.RefreshToken = tokenResponse.RefreshToken;
                        File.WriteAllText(Globals.RefreshTokenPath, tokenResponse.RefreshToken);
                        httpWrapper.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenSpotify);
                        DispatcherTimer timer = new()
                        {
                            Interval = TimeSpan.FromSeconds(3590)
                        };
                        timer.Tick += async (s, e) =>
                        {
                            getToken();
                        };
                        timer.Start();
                    };
                    await server.Start();
                    LoginRequest loginRequest = new(server.BaseUri, Globals._clientId, LoginRequest.ResponseType.Code)
                    {
                        Scope = ["user-read-currently-playing", "user-read-playback-state", "user-read-recently-played"]
                    };
                    BrowserUtil.Open(loginRequest.ToUri());
                }
                isReady = true;
                APIInitialized();
            }
            catch
            {
                isErr = true;
                await server.Stop();
            }
        }
        private async static void getToken()
        {
            tokenSpotify = (await new OAuthClient().RequestToken(new AuthorizationCodeRefreshRequest(Globals._clientId, Globals._secretId, Globals.RefreshToken))).AccessToken;
            httpWrapper.DefaultRequestHeaders.Remove("Authorization");
            httpWrapper.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenSpotify);
        }
        public static object[] getInfoSpotify(SongObject songObjectOriginal, double songElapsedOriginal, double songDurationOriginal)
        {
            object[] updatedInfo = new object[3];
            try
            {
                string url = "https://api.spotify.com/v1/me/player/currently-playing";
                using Task<string> response = httpWrapper.GetStringAsync(url);
                if ((string)JObject.Parse(response.Result)["is_playing"] == "True")
                {
                    responsedecode = JObject.Parse(response.Result.ToString());
                    songObjectOriginal.spotifyID = responsedecode["item"]["id"].ToString();
                    songObjectOriginal.artist = responsedecode["item"]["artists"][0]["name"].ToString();
                    songObjectOriginal.title = responsedecode["item"]["name"].ToString();
                    songObjectOriginal.internationalID = responsedecode["item"]["external_ids"]["isrc"].ToString();
                    songObjectOriginal.album = responsedecode["item"]["album"]["name"].ToString();
                    songObjectOriginal.setAlbumCover(responsedecode["item"]["album"]["images"][0]["url"].ToString());
                    songObjectOriginal.setReleaseDate(responsedecode["item"]["album"]["release_date"].ToString());
                    songElapsedOriginal = Double.Parse(responsedecode["progress_ms"].ToString());
                    songDurationOriginal = Double.Parse(responsedecode["item"]["duration_ms"].ToString());
                }
                updatedInfo[0] = songObjectOriginal;
                updatedInfo[1] = songElapsedOriginal;
                updatedInfo[2] = songDurationOriginal;
            }
            catch (Exception e)
            {
                if (e.Message.Contains("401"))
                {
                    getToken();
                }
            }
            return updatedInfo;

        }
        public static void APIInitialized()
        {
            SpotifyEvent?.Invoke();
        }
    }
}
