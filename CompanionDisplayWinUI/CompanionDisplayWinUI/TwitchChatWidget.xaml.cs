namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchChatWidget : SharedTwitch
    {
        public TwitchChatWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/chat";
            pageWebView = Player;
        }
    }
}
