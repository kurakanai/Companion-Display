namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PowerSensor : SharedSensorPage
    {
        public PowerSensor()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsPower;
            sensorText = LoadPercent;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
