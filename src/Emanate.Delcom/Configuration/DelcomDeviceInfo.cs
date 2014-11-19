using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class DelcomDeviceInfo : ViewModel
    {
        private const string defaultName = "Delcom Device";

        public DelcomDeviceInfo(DelcomDevice delcomDevice, IOutputDevice configuredDevice, IModuleConfiguration delcomConfiguration)
        {
            IndicateCommand = new DelegateCommand(Indicate);

            Device = delcomDevice;

            if (configuredDevice != null)
            {
                Name = configuredDevice.Name;
                Profile = configuredDevice.Profile.Key;
            }
            else
            {
                for (int i = 0;; i++)
                {
                    var currentname = defaultName + " #" + i;
                    if (delcomConfiguration.OutputDevices.All(d => d.Name != currentname))
                    {
                        Name = currentname;
                        break;
                    }
                }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private string profile;
        public string Profile
        {
            get { return profile; }
            set { profile = value; OnPropertyChanged(); }
        }

        private DelcomDevice device;
        public DelcomDevice Device
        {
            get { return device; }
            set { device = value; OnPropertyChanged(); }
        }

        public bool IsMissingPhysicalDevice { get { return device == null || device.PhysicalDevice == null; } }

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