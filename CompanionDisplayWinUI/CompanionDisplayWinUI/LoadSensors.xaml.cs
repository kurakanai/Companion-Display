namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadSensors : SharedSensorPage
    {
        public LoadSensors()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsLoad;
            sensorText = LoadPercent;
            sensorRing = TempProgress;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
