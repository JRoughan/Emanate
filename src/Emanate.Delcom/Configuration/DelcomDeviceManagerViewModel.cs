using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Emanate.Core.Output;
using Emanate.Service.Admin;

namespace Emanate.Delcom.Configuration
{
    public class DelcomDeviceManagerViewModel : ViewModel
    {
        private readonly ObservableCollection<IOutputDevice> existingDevices;

        public DelcomDeviceManagerViewModel(ObservableCollection<IOutputDevice> existingDevices)
        {
            this.existingDevices = existingDevices;
            AvailableDevices = new ObservableCollection<IOutputProfile>();

            SaveDeviceCommand = new DelegateCommand<DelcomDevice>(SaveDevice, CanSaveDevice);
        }

        public IEnumerable<IOutputProfile> AvailableDevices { get; private set; }

        public ICommand SaveDeviceCommand { get; private set; }
        private bool CanSaveDevice(DelcomDevice device)
        {
            return device != null;
        }
        private void SaveDevice(DelcomDevice device)
        {
            existingDevices.Add(device);
        }
    }
}