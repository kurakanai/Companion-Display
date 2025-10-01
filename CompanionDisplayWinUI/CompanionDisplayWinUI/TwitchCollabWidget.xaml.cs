namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchCollabWidget : SharedTwitch
    {
        public TwitchCollabWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/guest-star-stream-together";
            pageWebView = Player;
        }
    }
}
