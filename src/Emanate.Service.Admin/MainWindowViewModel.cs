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
        private readonly BackgroundWorker statusUpdateWorker = new BackgroundWorker();

        public MainWindowViewModel()
        {
            startCommand = new DelegateCommand(StartService, CanStartService);
            stopCommand = new DelegateCommand(StopService, CanStopService);
            restartCommand = new DelegateCommand(RestartService, CanStopService);
            saveCommand = new DelegateCommand(SaveAndExit, CanFindServiceConfiguration);
            applyCommand = new DelegateCommand(SaveConfiguration, CanFindServiceConfiguration);
            cancelCommand = new DelegateCommand(OnCloseRequested);

            statusUpdateWorker.DoWork += UpdateServiceStatus;
            statusUpdateWorker.RunWorkerCompleted += DisplayNewStatus;

            // TODO: Dynamically determine service name
            service = new ServiceController("EmanateService");
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

        void DisplayNewStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Status = "Could not update service";
                return;
            }

            Status = ((ServiceControllerStatus)e.Result).ToString();
        }

        void UpdateServiceStatus(object sender, DoWorkEventArgs e)
        {
            var args = (StatusUpdateArgs)e.Argument;
            args.Method(service);
            service.WaitForStatus(args.FinalStatus, TimeSpan.FromSeconds(30));
            e.Result = service.Status;
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
            return serviceIsInstalled && !statusUpdateWorker.IsBusy && service.Status == ServiceControllerStatus.Stopped;
        }

        class StatusUpdateArgs
        {
            public StatusUpdateArgs(Action<ServiceController> method, ServiceControllerStatus finalStatus)
            {
                Method = method;
                FinalStatus = finalStatus;
            }

            public ServiceControllerStatus FinalStatus { get; private set; }
            public Action<ServiceController> Method { get; private set; }
        }

        private void StartService()
        {
            var args = new StatusUpdateArgs(s => s.Start(), ServiceControllerStatus.Running);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private readonly DelegateCommand stopCommand;
        public DelegateCommand StopCommand { get { return stopCommand; } }

        private bool CanStopService()
        {
            return serviceIsInstalled && !statusUpdateWorker.IsBusy && service.CanStop && service.Status == ServiceControllerStatus.Running;
        }

        private void StopService()
        {
            var args = new StatusUpdateArgs(s => s.Stop(), ServiceControllerStatus.Stopped);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private readonly DelegateCommand restartCommand;
        public DelegateCommand RestartCommand { get { return restartCommand; } }

        private void RestartService()
        {
            var args = new StatusUpdateArgs(s => { s.Stop(); s.WaitForStatus(ServiceControllerStatus.Stopped); s.Start(); }, ServiceControllerStatus.Running);
            statusUpdateWorker.RunWorkerAsync(args);
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
