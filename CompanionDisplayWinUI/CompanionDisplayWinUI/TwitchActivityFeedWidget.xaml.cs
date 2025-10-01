namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchActivityFeedWidget : SharedTwitch
    {
        public TwitchActivityFeedWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/activity-feed";
            pageWebView = Player;
        }
    }
}
