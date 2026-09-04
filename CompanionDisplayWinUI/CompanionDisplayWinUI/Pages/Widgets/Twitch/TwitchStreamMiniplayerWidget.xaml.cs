namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitichStreamMiniplayerWidget : SharedTwitch
    {
        public TwitichStreamMiniplayerWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/stream-preview";
            pageWebView = Player;
        }
    }
}
