using CompanionDisplayWinUI.ClassImplementations;
using LibreHardwareMonitor.Hardware;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;

namespace CompanionDisplayWinUI.API
{
    internal class HardwareMonitorAPI
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
        public Computer computer = new()
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsNetworkEnabled = true,
            IsStorageEnabled = true
        };
        public void Init()
        {
            try
            {
                computer.Open();
                computer.Accept(new UpdateVisitor());
                Thread thread = new(UpdateSensor);
                thread.Start();
            }
            catch { }
        }
        public static event CommonlyAccessedInstances.HandleEventsWithNoArgs UpdateSensorValueEvent;
        static void CallSensorUpdate()
        {
            UpdateSensorValueEvent?.Invoke();
        }
        public void UpdateSensor() 
        {
            if(Globals.CurrentHW != null)
            {
                UpdateVisitor update = new();
                update.VisitHardware(Globals.CurrentHW);
            }
            CallSensorUpdate();
            Thread.Sleep(5000);
            Thread thread = new(UpdateSensor);
            thread.Start();
        }
        public static void UpdateSensorValue(ISensor sensor1, double lastValue, TextBlock textBlock, ProgressRing ring, string termination, DispatcherQueue dispatcher, bool extraPrecision)
        {
            try
            {
                int mult = 1;
                if (extraPrecision)
                {
                    mult = 100;
                }
                double value = (Math.Round((float)(sensor1.Value) * mult)) / mult;
                if (value != lastValue && (Globals.CurrentHW == sensor1.Hardware || Globals.CurrentHW == sensor1.Hardware.Parent) && lastValue != -2 && Math.Round((float)sensor1.Value) != lastValue)
                {
                    dispatcher.TryEnqueue(() =>
                    {
                        textBlock.Text = value + termination;
                        if (ring != null)
                        {
                            ring.Value = value;
                            if (ring.Maximum != 100)
                            {
                                ring.Maximum = sensor1.Max.Value;
                            }
                        }
                    });
                }
            }
            catch { }
        }

    }
}
