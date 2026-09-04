namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchStreamInfoWidget : SharedTwitch
    {
        public TwitchStreamInfoWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/edit-stream-info";
            pageWebView = Player;
        }
    }
}
