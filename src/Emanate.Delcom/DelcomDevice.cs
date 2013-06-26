using System;
using System.Collections.Generic;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Delcom
{
    public class DelcomDevice : IOutputDevice
    {
        private const string key = "delcom";
        private const string defaultName = "Delcom";
        private BuildState lastCompletedState;
        private DateTimeOffset lastUpdateTime;
        private const int minutesTillFullDim = 24 * 60; // 1 full day

        string IOutputDevice.Key { get { return key; } }

        private string name;

        public string Name
        {
            get { return name ?? defaultName; }
            set { name = value; }
        }

        public DelcomDevice()
        {
            Inputs = new List<InputInfo>();
        }

        public string Id { get; set; }

        public string Type { get { return key; } }

        public List<InputInfo> Inputs { get; private set; }

        public IOutputProfile Profile { get; set; }

        public PhysicalDevice PhysicalDevice { get; set; }

        public void UpdateStatus(BuildState state, DateTimeOffset timeStamp)
        {
            lock (PhysicalDevice)
            {
                if (!PhysicalDevice.IsOpen)
                    PhysicalDevice.Open();
            }

            switch (state)
            {
                case BuildState.Unknown:
                    PhysicalDevice.TurnOn(Color.Red);
                    PhysicalDevice.TurnOn(Color.Green);
                    PhysicalDevice.TurnOff(Color.Yellow);
                    break;
                case BuildState.Succeeded:
                    PhysicalDevice.TurnOff(Color.Red);
                    PhysicalDevice.TurnOff(Color.Yellow);
                    TurnOnColorWithCustomPowerLevel(Color.Green, timeStamp);
                    lastCompletedState = state;
                    break;
                case BuildState.Error:
                case BuildState.Failed:
                    PhysicalDevice.TurnOff(Color.Green);
                    PhysicalDevice.TurnOff(Color.Yellow);
                    TurnOnColorWithCustomPowerLevel(Color.Red, timeStamp);
                    if (lastCompletedState != BuildState.Failed && lastCompletedState != BuildState.Error)
                        PhysicalDevice.StartBuzzer(100, 2, 20, 20);
                    lastCompletedState = state;
                    break;
                case BuildState.Running:
                    PhysicalDevice.TurnOff(Color.Red);
                    PhysicalDevice.TurnOff(Color.Green);
                    PhysicalDevice.Flash(Color.Yellow);
                    break;
                default:
                    PhysicalDevice.Flash(Color.Red);
                    break;
            }
            lastCompletedState = state != BuildState.Running ? state : lastCompletedState;
            lastUpdateTime = timeStamp;
        }

        private void TurnOnColorWithCustomPowerLevel(Color color, DateTimeOffset timeStamp)
        {
            var minutesSinceLastBuild = (DateTimeOffset.Now - timeStamp).TotalMinutes;
            if (minutesSinceLastBuild > minutesTillFullDim)
            {
                PhysicalDevice.TurnOn(color, color.MinPower);
            }
            else
            {
                var power = color.MaxPower - (minutesSinceLastBuild / minutesTillFullDim) * (color.MaxPower - color.MinPower);
                PhysicalDevice.TurnOn(color, (byte)power);
            }
        }

    }
}