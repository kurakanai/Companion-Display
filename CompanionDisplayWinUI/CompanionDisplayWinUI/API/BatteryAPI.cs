namespace CompanionDisplayWinUI.API
{
    public static class BatteryAPI
    {
        public static string GetBatteryIcon(int battery)
        {
            return battery switch
            {
                >= 0 and < 10 => ("\ue850"),
                >= 10 and < 20 => ("\ue851"),
                >= 20 and < 30 => ("\ue852"),
                >= 30 and < 40 => ("\ue853"),
                >= 40 and < 50 => ("\ue854"),
                >= 50 and < 60 => ("\ue855"),
                >= 60 and < 70 => ("\ue856"),
                >= 70 and < 80 => ("\ue857"),
                >= 80 and < 90 => ("\ue858"),
                >= 90 and < 100 => ("\ue859"),
                100 => ("\ue83f"),
                _ => ("\ue996"),
            };
        }
    }
}
