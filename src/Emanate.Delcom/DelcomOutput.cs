using System;
using System.Management;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Delcom
{
    public class DelcomOutput : IOutput
    {
        private Device device;
        private BuildState lastCompletedState;
        private DateTimeOffset lastUpdateTime;
        private const int minutesTillFullDim = 24 * 60; // 1 full day

        public DelcomOutput()
        {
            this.device = new Device();
            ListenForDeviceConnectionChanges();
        }

        private void ListenForDeviceConnectionChanges()
        {
            var attachedDeviceQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType=2");
            var watcher = new ManagementEventWatcher(attachedDeviceQuery);
            watcher.EventArrived += ReAttachDeviceIfRequired;
            watcher.Start();
        }

        void ReAttachDeviceIfRequired(object sender, EventArrivedEventArgs e)
        {
            lock (device)
            {
                if (device == null)
                    device = new Device();
                else
                {
                    var tempDevice = new Device();
                    if (tempDevice.Name != device.Name)
                    {
                        device.Dispose();
                        device = tempDevice;
                    }
                }
            }
            UpdateStatus(lastCompletedState, lastCompletedState, lastUpdateTime);
        }

        public void UpdateStatus(BuildState oldState, BuildState newState, DateTimeOffset timeStamp)
        {
            lock (device)
            {
                if (!device.IsOpen)
                    device.Open();
            }

            switch (newState)
            {
                case BuildState.Unknown:
                    device.TurnOn(Color.Red);
                    device.TurnOn(Color.Green);
                    device.TurnOff(Color.Yellow);
                    break;
                case BuildState.Succeeded:
                    device.TurnOff(Color.Red);
                    device.TurnOff(Color.Yellow);
                    TurnOnColorWithCustomPowerLevel(Color.Green, timeStamp);
                    lastCompletedState = newState;
                    break;
                case BuildState.Error:
                case BuildState.Failed:
                    device.TurnOff(Color.Green);
                    device.TurnOff(Color.Yellow);
                    TurnOnColorWithCustomPowerLevel(Color.Red, timeStamp);
                    if (lastCompletedState != BuildState.Failed && lastCompletedState != BuildState.Error)
                        device.StartBuzzer(100, 2, 20, 20);
                    lastCompletedState = newState;
                    break;
                case BuildState.Running:
                    device.TurnOff(Color.Red);
                    device.TurnOff(Color.Green);
                    device.Flash(Color.Yellow);
                    break;
                default:
                    device.Flash(Color.Red);
                    break;
            }
            lastCompletedState = newState != BuildState.Running ? newState : lastCompletedState;
            lastUpdateTime = timeStamp;
        }

        private void TurnOnColorWithCustomPowerLevel(Color color, DateTimeOffset timeStamp)
        {
            var minutesSinceLastBuild = (DateTimeOffset.Now - timeStamp).TotalMinutes;
            if (minutesSinceLastBuild > minutesTillFullDim)
            {
                device.TurnOn(color, color.MinPower);
            }
            else
            {
                var power = color.MaxPower - (minutesSinceLastBuild / minutesTillFullDim) * (color.MaxPower - color.MinPower);
                device.TurnOn(color, (byte)power);
            }
        }
    }
}