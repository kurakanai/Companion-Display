namespace CompanionDisplayWinUI.ClassImplementations.SharedPages
{
    public sealed partial class FactorSensor : SharedSensorPage
    {
        public FactorSensor()
        {
            this.InitializeComponent();
            sensorText = LoadPercent;
            sensorName = SensorName;
            needsExtraPrecision = false;
        }
    }
}
