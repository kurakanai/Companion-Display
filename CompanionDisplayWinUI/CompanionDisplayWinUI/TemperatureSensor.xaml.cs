namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TemperatureSensor : SharedSensorPage
    {
        public TemperatureSensor()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsTemperature;
            sensorText = LoadPercent;
            sensorRing = TempProgress;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
