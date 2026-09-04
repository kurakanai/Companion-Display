namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchActiveModsWidget : SharedTwitch
    {
        public TwitchActiveModsWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/active-mods";
            pageWebView = Player;
        }
    }
}
