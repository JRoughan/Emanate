using System;
using System.Diagnostics;
using System.Windows.Input;
using Emanate.Service.Admin;

namespace Emanate.Vso.Configuration
{
    public class VsoConfigurationViewModel : ViewModel
    {
        private readonly VsoConfiguration vsoConfiguration;

        public VsoConfigurationViewModel(VsoConfiguration vsoConfiguration)
        {
            this.vsoConfiguration = vsoConfiguration;
            IsEditable = vsoConfiguration != null;

            TestConnectionCommand = new DelegateCommand(TestConnection, CanTestConnection);
        }

        public string Uri
        {
            get { return vsoConfiguration.Uri; }
            set { vsoConfiguration.Uri = value; OnPropertyChanged(); }
        }

        public int PollingInterval
        {
            get { return vsoConfiguration.PollingInterval; }
            set { vsoConfiguration.PollingInterval = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get { return vsoConfiguration.UserName; }
            set { vsoConfiguration.UserName = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return vsoConfiguration.Password; }
            set { vsoConfiguration.Password = value; OnPropertyChanged(); }
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
            Trace.TraceInformation("=> VsoConfigurationViewModel.TestConnection");
            isTesting = true;
            IsEditable = false;
            IsTestSuccessful = null;
            var connection = new VsoConnection(vsoConfiguration);
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
                Trace.TraceInformation("VSO connection test " + (IsTestSuccessful.HasValue && IsTestSuccessful.Value ? "succeeded" : "failed"));
            }
        }
    }
}