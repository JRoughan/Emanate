using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceProcess;

namespace Emanate.Service.Admin
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly PluginConfigurationLoader pluginConfigurationLoader;
        private readonly ServiceController service;

        public MainWindowViewModel()
        {
            startCommand = new DelegateCommand(StartService, CanStartService);
            stopCommand = new DelegateCommand(StopService, CanStopService);
            restartCommand = new DelegateCommand(RestartService, CanStopService);

            // TODO: Dynamically determine service name
            service = new ServiceController("MonitoringService");
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
            return service.Status == ServiceControllerStatus.Stopped;
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
            return service.CanStop && service.Status == ServiceControllerStatus.Running;
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
