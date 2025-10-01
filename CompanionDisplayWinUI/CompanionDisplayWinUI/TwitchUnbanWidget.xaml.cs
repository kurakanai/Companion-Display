namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchUnbanWidget : SharedTwitch
    {
        public TwitchUnbanWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/unban-requests";
            pageWebView = Player;
        }
    }
}
