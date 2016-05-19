using System;
using System.Windows.Input;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Vso.Admin
{
    public class VsoDeviceViewModel : ViewModel
    {
        private readonly VsoDevice device;

        public VsoDeviceViewModel(VsoDevice device)
        {
            this.device = device;
            TestConnectionCommand = new DelegateCommand(TestConnection, CanTestConnection);
        }

        public Guid Id { get { return device.Id; } }

        public string Name
        {
            get { return device.Name; }
            set { device.Name = value; OnPropertyChanged(); }
        }

        public string Uri
        {
            get { return device.Uri; }
            set { device.Uri = value; OnPropertyChanged(); }
        }

        public int PollingInterval
        {
            get { return device.PollingInterval; }
            set { device.PollingInterval = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get { return device.UserName; }
            set { device.UserName = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return device.Password; }
            set { device.Password = value; OnPropertyChanged(); }
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

        public ICommand TestConnectionCommand { get; private set; }
        public VsoDevice Device => device;

        private bool isTesting;
        private bool CanTestConnection()
        {
            return !isTesting;
        }

        private async void TestConnection()
        {
            Log.Information("=> VsoConfigurationViewModel.TestConnection");
            isTesting = true;
            IsEditable = false;
            IsTestSuccessful = null;
            var connection = new VsoConnection(device);
            try
            {
                var projects = await connection.GetProjects();
                IsTestSuccessful = projects != null;
            }
            catch (Exception)
            {
                IsTestSuccessful = false;
            }
            finally
            {
                isTesting = false;
                IsEditable = true;
                Log.Information("VSO connection test " + (IsTestSuccessful.HasValue && IsTestSuccessful.Value ? "succeeded" : "failed"));
            }
        }
    }
}
