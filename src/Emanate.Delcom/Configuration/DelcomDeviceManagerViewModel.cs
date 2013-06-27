﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class DelcomDeviceManagerViewModel : ViewModel
    {
        private readonly DelcomConfiguration delcomConfiguration;

        public DelcomDeviceManagerViewModel(DelcomConfiguration delcomConfiguration)
        {
            this.delcomConfiguration = delcomConfiguration;

            ConfiguredDevices = new ObservableCollection<DelcomDeviceInfo>();
            AvailableDevices = new ObservableCollection<DelcomDeviceInfo>();

            AddDeviceCommand = new DelegateCommand<DelcomDeviceInfo>(AddDevice, CanAddDevice);
            RemoveDeviceCommand = new DelegateCommand<DelcomDeviceInfo>(RemoveDevice, CanRemoveDevice);
        }

        public override void Initialize()
        {
            for (uint i = 1; ; i++)
            {
                var delcom = new DelcomHid();
                if (delcom.OpenNthDevice(i) != 0)
                    break;

                var physicalDevice = new PhysicalDevice(delcom);
                var delcomDevice = new DelcomDevice {PhysicalDevice = physicalDevice};

                var deviceId = physicalDevice.Name;
                var configuredDevice = delcomConfiguration.OutputDevices.SingleOrDefault(d => d.Id == deviceId);

                var delcomDeviceInfo = new DelcomDeviceInfo(delcomDevice, configuredDevice, delcomConfiguration);

                if (configuredDevice != null)
                {
                    ConfiguredDevices.Add(delcomDeviceInfo);
                }
                else
                {
                    AvailableDevices.Add(delcomDeviceInfo);
                }
            }
        }

        public ObservableCollection<DelcomDeviceInfo> ConfiguredDevices { get; private set; }
        public ObservableCollection<DelcomDeviceInfo> AvailableDevices { get; private set; }

        public ICommand AddDeviceCommand { get; private set; }
        private bool CanAddDevice(DelcomDeviceInfo deviceInfo)
        {
            return deviceInfo != null;
        }
        private void AddDevice(DelcomDeviceInfo deviceInfo)
        {
            var device = deviceInfo.Device;
            device.Id = device.PhysicalDevice.Name;
            device.Name = deviceInfo.Name;
            device.Profile = delcomConfiguration.Profiles.First();

            deviceInfo.Profile = deviceInfo.Device.Profile.Key; // TODO: Binding should deal with this
            delcomConfiguration.OutputDevices.Add(deviceInfo.Device);
            AvailableDevices.Remove(deviceInfo);
            ConfiguredDevices.Add(deviceInfo);
        }

        public ICommand RemoveDeviceCommand { get; private set; }
        private bool CanRemoveDevice(DelcomDeviceInfo deviceInfo)
        {
            return deviceInfo != null;
        }
        private void RemoveDevice(DelcomDeviceInfo deviceInfo)
        {
            deviceInfo.Device.Profile = null;
            deviceInfo.Profile = null; // TODO: Binding should deal with this

            delcomConfiguration.OutputDevices.Remove(deviceInfo.Device);
            ConfiguredDevices.Remove(deviceInfo);
            AvailableDevices.Add(deviceInfo);
        }
    }
}