﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Extensibility;

namespace Emanate.TeamCity2017.Admin.Devices
{
    public class TeamCityDeviceManagerViewModel : ViewModel
    {
        private readonly TeamCity2017Configuration teamCityConfiguration;

        public TeamCityDeviceManagerViewModel(TeamCity2017Configuration teamCityConfiguration)
        {
            this.teamCityConfiguration = teamCityConfiguration;

            var deviceVms = teamCityConfiguration.Devices.Select(d => new TeamCityDeviceViewModel((TeamCity2017Device)d));
            Devices = new ObservableCollection<TeamCityDeviceViewModel>(deviceVms);

            AddDeviceCommand = new DelegateCommand(AddDevice);
            RemoveDeviceCommand = new DelegateCommand<TeamCityDeviceViewModel>(RemoveDevice, CanRemoveDevice);
        }

        public override async Task<InitializationResult> Initialize()
        {
            return await Task.Run(() => InitializationResult.Succeeded);
        }

        public ObservableCollection<TeamCityDeviceViewModel> Devices { get; }

        public ICommand AddDeviceCommand { get; private set; }

        private void AddDevice()
        {
            var deviceInfo = new TeamCity2017Device
            {
                Id = Guid.NewGuid(),
                Name = "New"
            };
            teamCityConfiguration.AddDevice(deviceInfo);
            Devices.Add(new TeamCityDeviceViewModel(deviceInfo));
        }

        public ICommand RemoveDeviceCommand { get; private set; }
        private bool CanRemoveDevice(TeamCityDeviceViewModel deviceInfo)
        {
            return deviceInfo != null;
        }
        private void RemoveDevice(TeamCityDeviceViewModel deviceVm)
        {
            teamCityConfiguration.RemoveDevice(deviceVm.Device);
            Devices.Remove(deviceVm);
        }
    }
}