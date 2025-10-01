namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class ClockSensor : SharedSensorPage
    {
        public ClockSensor()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsFreq;
            sensorText = LoadPercent;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
