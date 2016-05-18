using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Extensibility;

namespace Emanate.Vso.Configuration
{
    public class VsoDeviceManagerViewModel : ViewModel
    {
        private readonly VsoConfiguration vsoConfiguration;

        public VsoDeviceManagerViewModel(VsoConfiguration vsoConfiguration)
        {
            this.vsoConfiguration = vsoConfiguration;

            Devices = new ObservableCollection<VsoDeviceInfo>(vsoConfiguration.Devices);

            AddDeviceCommand = new DelegateCommand(AddDevice);
            RemoveDeviceCommand = new DelegateCommand<VsoDeviceInfo>(RemoveDevice, CanRemoveDevice);
        }

        public override async Task<InitializationResult> Initialize()
        {
            return await Task.Run(() => InitializationResult.Succeeded);
        }

        public ObservableCollection<VsoDeviceInfo> Devices { get; }

        public ICommand AddDeviceCommand { get; private set; }

        private void AddDevice()
        {
            var deviceInfo = new VsoDeviceInfo
            {
                Name = "New"
            };
            //deviceInfo.Id = device.PhysicalDevice.Name;
            //deviceInfo.Name = deviceInfo.Name;
            //device.Profile = vsoConfiguration.Profiles.FirstOrDefault() ?? vsoConfiguration.AddDefaultProfile("Default");
            //deviceInfo.Profile = deviceInfo.Device.Profile.Key; // TODO: Binding should deal with this
            vsoConfiguration.AddDevice(deviceInfo);
            Devices.Add(deviceInfo);
        }

        public ICommand RemoveDeviceCommand { get; private set; }
        private bool CanRemoveDevice(VsoDeviceInfo deviceInfo)
        {
            return deviceInfo != null;
        }
        private void RemoveDevice(VsoDeviceInfo deviceInfo)
        {
            //deviceInfo.Device.Profile = null;
            //deviceInfo.Profile = null; // TODO: Binding should deal with this

            vsoConfiguration.RemoveDevice(deviceInfo);
            Devices.Remove(deviceInfo);
        }
    }
}