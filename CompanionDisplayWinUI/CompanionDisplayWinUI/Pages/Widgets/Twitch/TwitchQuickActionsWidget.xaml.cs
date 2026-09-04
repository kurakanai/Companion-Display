namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchQuickActionsWidget : SharedTwitch
    {
        public TwitchQuickActionsWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/quick-actions";
            pageWebView = Player;
        }
    }
}
