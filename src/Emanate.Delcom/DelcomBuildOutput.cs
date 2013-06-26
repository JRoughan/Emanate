//using System;
//using System.Management;
//using Emanate.Core.Input;
//using Emanate.Core.Output;

//namespace Emanate.Delcom
//{
//    public class DelcomBuildOutput : IBuildOutput
//    {
//        private PhysicalDevice physicalDevice;


//        public DelcomBuildOutput(PhysicalDevice physicalDevice)
//        {
//            this.physicalDevice = physicalDevice;
//            ListenForDeviceConnectionChanges();
//        }

//        private void ListenForDeviceConnectionChanges()
//        {
//            var attachedDeviceQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType=2");
//            var watcher = new ManagementEventWatcher(attachedDeviceQuery);
//            watcher.EventArrived += ReAttachDeviceIfRequired;
//            watcher.Start();
//        }

//        void ReAttachDeviceIfRequired(object sender, EventArrivedEventArgs e)
//        {
//            lock (physicalDevice)
//            {
//                // TODO
//                //if (device == null)
//                //    device = new Device();
//                //else
//                //{
//                //    var tempDevice = new Device();
//                //    if (tempDevice.Name != device.Name)
//                //    {
//                //        device.Dispose();
//                //        device = tempDevice;
//                //    }
//                //}
//            }
//            UpdateStatus(lastCompletedState, lastUpdateTime);
//        }

//        public void UpdateStatus(BuildState state, DateTimeOffset timeStamp)
//        {
//            lock (physicalDevice)
//            {
//                if (!physicalDevice.IsOpen)
//                    physicalDevice.Open();
//            }

//            switch (state)
//            {
//                case BuildState.Unknown:
//                    physicalDevice.TurnOn(Color.Red);
//                    physicalDevice.TurnOn(Color.Green);
//                    physicalDevice.TurnOff(Color.Yellow);
//                    break;
//                case BuildState.Succeeded:
//                    physicalDevice.TurnOff(Color.Red);
//                    physicalDevice.TurnOff(Color.Yellow);
//                    TurnOnColorWithCustomPowerLevel(Color.Green, timeStamp);
//                    lastCompletedState = state;
//                    break;
//                case BuildState.Error:
//                case BuildState.Failed:
//                    physicalDevice.TurnOff(Color.Green);
//                    physicalDevice.TurnOff(Color.Yellow);
//                    TurnOnColorWithCustomPowerLevel(Color.Red, timeStamp);
//                    if (lastCompletedState != BuildState.Failed && lastCompletedState != BuildState.Error)
//                        physicalDevice.StartBuzzer(100, 2, 20, 20);
//                    lastCompletedState = state;
//                    break;
//                case BuildState.Running:
//                    physicalDevice.TurnOff(Color.Red);
//                    physicalDevice.TurnOff(Color.Green);
//                    physicalDevice.Flash(Color.Yellow);
//                    break;
//                default:
//                    physicalDevice.Flash(Color.Red);
//                    break;
//            }
//            lastCompletedState = state != BuildState.Running ? state : lastCompletedState;
//            lastUpdateTime = timeStamp;
//        }

//        private void TurnOnColorWithCustomPowerLevel(Color color, DateTimeOffset timeStamp)
//        {
//            var minutesSinceLastBuild = (DateTimeOffset.Now - timeStamp).TotalMinutes;
//            if (minutesSinceLastBuild > minutesTillFullDim)
//            {
//                physicalDevice.TurnOn(color, color.MinPower);
//            }
//            else
//            {
//                var power = color.MaxPower - (minutesSinceLastBuild / minutesTillFullDim) * (color.MaxPower - color.MinPower);
//                physicalDevice.TurnOn(color, (byte)power);
//            }
//        }
//    }
//}