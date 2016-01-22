using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;
using Emanate.Extensibility;

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

        public override Task<bool> Initialize()
        {
            try
            {
                // TODO: Dynamically determine service name
                service = new ServiceController("EmanateService");
                UpdateStatus();
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                Trace.TraceWarning("Emanate service missing");
                IsInstalled = false;
                return Task.FromResult(false);
            }
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
            Trace.TraceInformation("Starting Emanate service");
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
            Trace.TraceInformation("Stopping Emanate service");
            var args = new StatusUpdateArgs(s => s.Stop(), ServiceControllerStatus.Stopped);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private readonly DelegateCommand restartCommand;
        public DelegateCommand RestartCommand { get { return restartCommand; } }

        private void RestartService()
        {
            Trace.TraceInformation("Restarting Emanate service");
            var args = new StatusUpdateArgs(s => { s.Stop(); s.WaitForStatus(ServiceControllerStatus.Stopped); s.Start(); }, ServiceControllerStatus.Running);
            statusUpdateWorker.RunWorkerAsync(args);
        }

        private void DisplayNewStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                IsInstalled = false;
                var errorMessage = "Could not update service: " + e.Error.Message;
                Trace.TraceInformation(errorMessage);
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
            Trace.TraceInformation("Waiting for Emanate service status");
            service.WaitForStatus(args.FinalStatus, TimeSpan.FromSeconds(30));
            var resultStatus = service.Status;
            if (resultStatus == args.FinalStatus)
                Trace.TraceInformation("Emanate service status: " + resultStatus);
            else
                Trace.TraceWarning("Unexpected Emanate service status: " + resultStatus);

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

        public ServiceControllerStatus FinalStatus { get; private set; }
        public Action<ServiceController> Method { get; private set; }
    }
}
