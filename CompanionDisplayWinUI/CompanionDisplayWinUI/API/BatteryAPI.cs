namespace CompanionDisplayWinUI.API
{
    public static class BatteryAPI
    {
        public static string GetBatteryIcon(int battery)
        {
            return battery switch
            {
                >= 0 and < 100 => ((char)('\ue850' + (battery / 10))).ToString(),
                100 => "\ue83f",
                _ => "\ue996"
            };
        }
    }
}
