namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SmallDataSensor : SharedSensorPage
    {
        public SmallDataSensor()
        {
            this.InitializeComponent();
            sensorSuffix = AppStrings.sensorsSmallData;
            sensorText = LoadPercent;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
