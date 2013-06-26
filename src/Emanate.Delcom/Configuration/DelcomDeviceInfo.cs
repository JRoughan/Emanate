using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
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
                    Thread.Sleep(100);
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