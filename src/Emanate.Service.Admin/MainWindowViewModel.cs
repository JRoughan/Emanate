using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceProcess;

namespace Emanate.Service.Admin
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PluginConfigurationStorer pluginConfigurationStorer;
        private readonly ServiceController service;
        private readonly bool serviceIsInstalled;

        public MainWindowViewModel()
        {
            startCommand = new DelegateCommand(StartService, CanStartService);
            stopCommand = new DelegateCommand(StopService, CanStopService);
            restartCommand = new DelegateCommand(RestartService, CanStopService);
            saveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            applyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);

            // TODO: Dynamically determine service name
            service = new ServiceController("MonitoringService");
            try
            {
                Status = service.DisplayName + " service is installed";
                serviceIsInstalled = true;
            }
            catch (Exception)
            {
                Status = "Service is not installed";
                serviceIsInstalled = false;
            }
 
            pluginConfigurationStorer = new PluginConfigurationStorer();
            ConfigurationInfos = new ObservableCollection<ConfigurationInfo>();

        }

        public event EventHandler CloseRequested;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Initialize()
        {
            if (!serviceIsInstalled)
                return;

            foreach (var plugin in pluginConfigurationStorer.Load())
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

        private readonly DelegateCommand saveCommand;
        public DelegateCommand SaveCommand { get { return saveCommand; } }

        private void SaveAndExit()
        {
            SaveConfiguration();
            OnCloseRequested();
        }

        private readonly DelegateCommand applyCommand;
        public DelegateCommand ApplyCommand { get { return applyCommand; } }

        private void SaveConfiguration()
        {
            pluginConfigurationStorer.Save(configurationInfos);
        }

        private bool CanFindServiceConfiguration()
        {
            return serviceIsInstalled;
        }

        private readonly DelegateCommand cancelCommand;
        public DelegateCommand CancelCommand { get { return cancelCommand; } }

        private void OnCloseRequested()
        {
            var handler = CloseRequested;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
