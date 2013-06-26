using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Core.Output;
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

    public class DelcomDeviceInfo : ViewModel
    {
        private readonly IOutputDevice configuredDevice;

        public DelcomDeviceInfo(DelcomDevice delcomDevice, IOutputDevice configuredDevice)
        {
            IndicateCommand = new DelegateCommand(Indicate);

            Device = delcomDevice;
            this.configuredDevice = configuredDevice;
            Name = delcomDevice.PhysicalDevice.Name;
            
            if (configuredDevice != null)
            {
                Profile = configuredDevice.Profile.Key;
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        private string profile;
        public string Profile
        {
            get { return profile; }
            set { profile = value; OnPropertyChanged("Profile"); }
        }

        private DelcomDevice device;
        public DelcomDevice Device
        {
            get { return device; }
            set { device = value; OnPropertyChanged("Device"); }
        }

        public ICommand IndicateCommand { get; private set; }
        private void Indicate()
        {
            Task.Factory.StartNew(() =>
                {
                    Device.PhysicalDevice.TurnOn(Color.Red);
                    Device.PhysicalDevice.TurnOn(Color.Yellow);
                    Device.PhysicalDevice.TurnOn(Color.Green);
                    Thread.Sleep(200);
                })
                .ContinueWith(t =>
                    {
                        Device.PhysicalDevice.TurnOff(Color.Red);
                        Device.PhysicalDevice.TurnOff(Color.Yellow);
                        Device.PhysicalDevice.TurnOff(Color.Green);
                    });
        }
    }
}