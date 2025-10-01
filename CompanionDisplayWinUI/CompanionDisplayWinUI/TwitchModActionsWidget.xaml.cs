namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class TwitchModActionsWidget : SharedTwitch
    {
        public TwitchModActionsWidget()
        {
            this.InitializeComponent();
            destinationUrl = "https://dashboard.twitch.tv/popout/stream-manager/moderation-actions";
            pageWebView = Player;
        }
    }
}
