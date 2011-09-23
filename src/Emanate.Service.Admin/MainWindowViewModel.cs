using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceProcess;

namespace Emanate.Service.Admin
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PluginConfigurationLoader pluginConfigurationLoader;
        private readonly ServiceController service;
        private bool serviceIsInstalled;

        public MainWindowViewModel()
        {
            startCommand = new DelegateCommand(StartService, CanStartService);
            stopCommand = new DelegateCommand(StopService, CanStopService);
            restartCommand = new DelegateCommand(RestartService, CanStopService);

            // TODO: Dynamically determine service name
            service = new ServiceController("MonitoringService");
            try
            {
                Status = service.DisplayName + " is running";
                serviceIsInstalled = true;
            }
            catch (Exception)
            {
                Status = service.DisplayName + " is not installed";
                serviceIsInstalled = false;
            }
 
            pluginConfigurationLoader = new PluginConfigurationLoader();
            ConfigurationInfos = new ObservableCollection<ConfigurationInfo>();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Initialize()
        {
            foreach (var plugin in pluginConfigurationLoader.Load())
            {
                ConfigurationInfos.Add(plugin);
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private ObservableCollection<ConfigurationInfo> configurationInfos;
        public ObservableCollection<ConfigurationInfo> ConfigurationInfos
        {
            get { return configurationInfos; }
            private set
            {
                configurationInfos = value;
                OnPropertyChanged("ConfigurationInfos");
            }
        }

        private readonly DelegateCommand startCommand;
        public DelegateCommand StartCommand { get { return startCommand; } }

        private bool CanStartService()
        {
            return serviceIsInstalled && service.Status == ServiceControllerStatus.Stopped;
        }

        private void StartService()
        {
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
        }

        private readonly DelegateCommand stopCommand;
        public DelegateCommand StopCommand { get { return stopCommand; } }

        private bool CanStopService()
        {
            return serviceIsInstalled && service.CanStop && service.Status == ServiceControllerStatus.Running;
        }

        private void StopService()
        {
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        private readonly DelegateCommand restartCommand;
        public DelegateCommand RestartCommand { get { return restartCommand; } }

        private void RestartService()
        {
            StopService();
            StartService();
        }
    }
}
