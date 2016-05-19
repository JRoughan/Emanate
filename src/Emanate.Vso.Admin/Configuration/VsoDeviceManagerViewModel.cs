﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Extensibility;
using Emanate.Vso.Configuration;

namespace Emanate.Vso.Admin.Configuration
{
    public class VsoDeviceManagerViewModel : ViewModel
    {
        private readonly VsoConfiguration vsoConfiguration;

        public VsoDeviceManagerViewModel(VsoConfiguration vsoConfiguration)
        {
            this.vsoConfiguration = vsoConfiguration;

            var deviceVms = vsoConfiguration.Devices.Select(d => new VsoDeviceViewModel(d));
            Devices = new ObservableCollection<VsoDeviceViewModel>(deviceVms);

            AddDeviceCommand = new DelegateCommand(AddDevice);
            RemoveDeviceCommand = new DelegateCommand<VsoDeviceViewModel>(RemoveDevice, CanRemoveDevice);
        }

        public override async Task<InitializationResult> Initialize()
        {
            return await Task.Run(() => InitializationResult.Succeeded);
        }

        public ObservableCollection<VsoDeviceViewModel> Devices { get; }

        public ICommand AddDeviceCommand { get; private set; }

        private void AddDevice()
        {
            var deviceInfo = new VsoDevice
            {
                Id = Guid.NewGuid(),
                Name = "New"
            };
            vsoConfiguration.AddDevice(deviceInfo);
            Devices.Add(new VsoDeviceViewModel(deviceInfo));
        }

        public ICommand RemoveDeviceCommand { get; private set; }
        private bool CanRemoveDevice(VsoDeviceViewModel deviceInfo)
        {
            return deviceInfo != null;
        }
        private void RemoveDevice(VsoDeviceViewModel deviceVm)
        {
            vsoConfiguration.RemoveDevice(deviceVm.Device);
            Devices.Remove(deviceVm);
        }
    }
}