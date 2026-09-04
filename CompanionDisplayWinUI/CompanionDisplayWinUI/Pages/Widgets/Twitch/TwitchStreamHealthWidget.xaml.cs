namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchStreamHealthWidget : SharedTwitch
    {
        public TwitchStreamHealthWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/stream-health";
            pageWebView = Player;
        }
    }
}
