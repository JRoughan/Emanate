using System.Management;
using Emanate.Core.Input;

namespace Emanate.Core.Output.DelcomVdi
{
    public class DelcomOutput : IOutput
    {
        private Device device;
        private BuildState lastState;

        public DelcomOutput()
        {
            device = new Device();
            ListenForDeviceConnectionChanges();
        }

        private void ListenForDeviceConnectionChanges()
        {
            var watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType=2");
            watcher.EventArrived += ReAttachDeviceIfRequired;
            watcher.Query = query;
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
            UpdateStatus(lastState, true);
        }

        public void UpdateStatus(BuildState state)
        {
            UpdateStatus(state, false);
        }

        public void UpdateStatus(BuildState state, bool force)
        {
            lock (device)
            {
                if (!device.IsOpen)
                    device.Open();
            }

            if (!force && state == lastState)
                return;

            switch (state)
            {
                case BuildState.Succeeded:
                    device.TurnOff(Color.Red);
                    device.TurnOn(Color.Green);
                    device.TurnOff(Color.Yellow);
                    break;
                case BuildState.Failed:
                    device.TurnOn(Color.Red);
                    device.TurnOff(Color.Green);
                    device.TurnOff(Color.Yellow);
                    device.StartBuzzer(100, 2, 20, 20);
                    break;
                case BuildState.Running:
                    device.TurnOff(Color.Red);
                    device.TurnOff(Color.Green);
                    device.Flash(Color.Yellow);
                    break;
            }
            lastState = state;
        }
    }
}