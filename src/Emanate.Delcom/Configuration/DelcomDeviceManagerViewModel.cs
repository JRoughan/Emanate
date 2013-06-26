using System.Collections.ObjectModel;
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

            SaveDeviceCommand = new DelegateCommand<DelcomDevice>(SaveDevice, CanSaveDevice);
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

                var delcomDeviceInfo = new DelcomDeviceInfo(delcomDevice, configuredDevice);

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

        public ICommand SaveDeviceCommand { get; private set; }
        private bool CanSaveDevice(DelcomDevice device)
        {
            return device != null;
        }
        private void SaveDevice(DelcomDevice device)
        {
            delcomConfiguration.OutputDevices.Add(device);
        }

        
    }
}