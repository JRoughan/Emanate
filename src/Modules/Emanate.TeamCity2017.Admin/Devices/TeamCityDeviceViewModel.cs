﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.TeamCity2017.Admin.Devices
{
    public class TeamCityDeviceViewModel : ViewModel
    {
        private readonly TeamCity2017Device teamCityDevice;

        public TeamCityDeviceViewModel(TeamCity2017Device teamCityDevice)
        {
            this.teamCityDevice = teamCityDevice;
            IsEditable = teamCityDevice != null;

            TestConnectionCommand = new DelegateCommand(TestConnection, CanTestConnection);
        }

        public TeamCity2017Device Device => teamCityDevice;

        public string Uri
        {
            get { return teamCityDevice.Uri; }
            set { teamCityDevice.Uri = value; OnPropertyChanged(); }
        }

        public int PollingInterval
        {
            get { return teamCityDevice.PollingInterval; }
            set { teamCityDevice.PollingInterval = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get { return teamCityDevice.UserName; }
            set { teamCityDevice.UserName = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return teamCityDevice.Password; }
            set { teamCityDevice.Password = value; OnPropertyChanged(); }
        }

        public bool RequiresAuthentication
        {
            get { return teamCityDevice.RequiresAuthentication; }
            set { teamCityDevice.RequiresAuthentication = value; OnPropertyChanged(); }
        }

        private bool isEditable;
        public bool IsEditable
        {
            get { return isEditable; }
            set { isEditable = value; OnPropertyChanged(); }
        }

        private bool? isTestSuccessful;
        public bool? IsTestSuccessful
        {
            get { return isTestSuccessful; }
            set { isTestSuccessful = value; OnPropertyChanged(); }
        }

        public ICommand TestConnectionCommand { get; set; }

        private bool isTesting;
        private bool CanTestConnection()
        {
            return !isTesting;
        }

        private void TestConnection()
        {
            Log.Information("=> TeamCityDeviceViewModel.TestConnection");
            isTesting = true;
            IsEditable = false;
            IsTestSuccessful = null;
            var connection = new TeamCity2017Connection(teamCityDevice);
            try
            {
                IsTestSuccessful = connection.GetProjects() != null;
            }
            catch (Exception)
            {
                IsTestSuccessful = false;
            }
            finally
            {
                isTesting = false;
                IsEditable = true;
                Log.Information("TeamCity connection test " + (IsTestSuccessful.HasValue && IsTestSuccessful.Value ? "succeeded" : "failed"));
            }
        }
    }
}