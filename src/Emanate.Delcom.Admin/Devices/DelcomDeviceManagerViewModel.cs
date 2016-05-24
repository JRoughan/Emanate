using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Extensibility;

namespace Emanate.Delcom.Admin.Devices
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

        public override async Task<InitializationResult> Initialize()
        {
            return await Task.Run(() =>
            {
                for (uint i = 1; ; i++)
                {
                    var delcom = new DelcomHid();
                    if (delcom.OpenNthDevice(i) != 0)
                        break;

                    var physicalDevice = new PhysicalDevice(delcom);
                    var delcomDevice = new DelcomDevice { PhysicalDevice = physicalDevice };

                    var physicalDeviceId = physicalDevice.Id;
                    var configuredDevice = delcomConfiguration.OutputDevices.OfType<DelcomDevice>().SingleOrDefault(d => d.PhysicalDeviceId == physicalDeviceId);

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

                foreach (var missingDevice in delcomConfiguration.OutputDevices.Where(od => ConfiguredDevices.All(cd => cd.Name != od.Name)))
                    ConfiguredDevices.Add(new DelcomDeviceInfo(null, missingDevice, delcomConfiguration));

                return InitializationResult.Succeeded;
            });
        }

        public ObservableCollection<DelcomDeviceInfo> ConfiguredDevices { get; }
        public ObservableCollection<DelcomDeviceInfo> AvailableDevices { get; }

        public ICommand AddDeviceCommand { get; private set; }
        private bool CanAddDevice(DelcomDeviceInfo deviceInfo)
        {
            return deviceInfo != null;
        }
        private void AddDevice(DelcomDeviceInfo deviceInfo)
        {
            var device = deviceInfo.Device;
            device.PhysicalDeviceId = device.PhysicalDevice.Id;
            device.Name = deviceInfo.Name;
            device.Profile = delcomConfiguration.Profiles.FirstOrDefault() ?? delcomConfiguration.AddDefaultProfile("Default");
            deviceInfo.Profile = deviceInfo.Device.Profile.Id.ToString(); // TODO: Binding should deal with this
            delcomConfiguration.AddOutputDevice(deviceInfo.Device);
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

            delcomConfiguration.RemoveOutputDevice(deviceInfo.Device);
            ConfiguredDevices.Remove(deviceInfo);
            AvailableDevices.Add(deviceInfo);
        }
    }
}