namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchAutoModWidget : SharedTwitch
    {
        public TwitchAutoModWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/auto-mod-queue";
            pageWebView = Player;
        }
    }
}
