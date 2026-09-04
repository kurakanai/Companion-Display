namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class FanSensor : SharedSensorPage
    {
        public FanSensor()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsRPM;
            sensorText = LoadPercent;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
