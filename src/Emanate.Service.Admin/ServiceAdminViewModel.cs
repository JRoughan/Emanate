using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Service.Admin
{
    class ServiceAdminViewModel : ViewModel
    {
        private ServiceController service;
        private readonly BackgroundWorker statusUpdateWorker = new BackgroundWorker();

        public ServiceAdminViewModel()
        {
            startCommand = new DelegateCommand(StartService, CanStartService);
            stopCommand = new DelegateCommand(StopService, CanStopService);
            restartCommand = new DelegateCommand(RestartService, CanStopService);

            statusUpdateWorker.DoWork += UpdateServiceStatus;
            statusUpdateWorker.RunWorkerCompleted += DisplayNewStatus;
        }

        public override async Task<InitializationResult> Initialize()
        {
            try
            {
                await Task.Run(() =>
                {
                    // TODO: Dynamically determine service name
                    service = new ServiceController("EmanateService");
                    UpdateStatus();
                });
            }
            catch (Exception)
            {
                Log.Warning("Emanate service missing");
                IsInstalled = false;
                return InitializationResult.Failed;
            }
            return InitializationResult.Succeeded;
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                OnPropertyChanged();
            }
        }

        private bool isStopped;
        public bool IsStopped
        {
            get { return isStopped; }
            set
            {
                isStopped = value;
                OnPropertyChanged();
            }
        }

        private bool isInstalled;
        public bool IsInstalled
        {
            get { return isInstalled; }
            set
            {
                isInstalled = value;
                OnPropertyChanged();
            }
        }

        private readonly DelegateCommand startCommand;
        public DelegateCommand StartCommand { get { return startCommand; } }

        private bool CanStartService()
        {
            return isInstalled && !statusUpdateWorker.IsBusy && service.Status == ServiceControllerStatus.Stopped;
        }

        private void StartService()
        {
            Log.Information("Starting Emanate service");
            var args = new StatusUpdateArgs(s => s.Start(), ServiceControllerStatus.Running);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private readonly DelegateCommand stopCommand;
        public DelegateCommand StopCommand { get { return stopCommand; } }

        private bool CanStopService()
        {
            return isInstalled && !statusUpdateWorker.IsBusy && service.CanStop && service.Status == ServiceControllerStatus.Running;
        }

        private void StopService()
        {
            Log.Information("Stopping Emanate service");
            var args = new StatusUpdateArgs(s => s.Stop(), ServiceControllerStatus.Stopped);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private readonly DelegateCommand restartCommand;
        public DelegateCommand RestartCommand { get { return restartCommand; } }

        private void RestartService()
        {
            Log.Information("Restarting Emanate service");
            var args = new StatusUpdateArgs(s => { s.Stop(); s.WaitForStatus(ServiceControllerStatus.Stopped); s.Start(); }, ServiceControllerStatus.Running);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private void DisplayNewStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                IsInstalled = false;
                var errorMessage = "Could not update service: " + e.Error.Message;
                Log.Information(errorMessage);
                MessageBox.Show(errorMessage);
                return;
            }
            UpdateStatus();
        }

        void UpdateStatus()
        {
            IsInstalled = true;
            IsRunning = service.Status == ServiceControllerStatus.Running;
            IsStopped = service.Status == ServiceControllerStatus.Stopped;
        }

        void UpdateServiceStatus(object sender, DoWorkEventArgs e)
        {
            var args = (StatusUpdateArgs)e.Argument;
            args.Method(service);
            Log.Information("Waiting for Emanate service status");
            service.WaitForStatus(args.FinalStatus, TimeSpan.FromSeconds(30));
            var resultStatus = service.Status;
            if (resultStatus == args.FinalStatus)
                Log.Information("Emanate service status: " + resultStatus);
            else
                Log.Warning("Unexpected Emanate service status: " + resultStatus);

            e.Result = resultStatus;
        }
    }

    class StatusUpdateArgs
    {
        public StatusUpdateArgs(Action<ServiceController> method, ServiceControllerStatus finalStatus)
        {
            Method = method;
            FinalStatus = finalStatus;
        }

        public ServiceControllerStatus FinalStatus { get; }
        public Action<ServiceController> Method { get; }
    }
}
