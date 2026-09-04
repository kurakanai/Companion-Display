namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VoltageSensor : SharedSensorPage
    {
        public VoltageSensor()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsVoltage;
            sensorText = LoadPercent;
            sensorName = SensorName;
            needsExtraPrecision = true;
        }
    }
}
