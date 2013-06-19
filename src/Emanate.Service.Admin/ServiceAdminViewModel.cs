using System;
using System.ComponentModel;
using System.ServiceProcess;
using Emanate.Core;

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

            IsNotInstalled = true;
        }

        public override void Initialize()
        {
            try
            {
                // TODO: Dynamically determine service name
                service = new ServiceController("EmanateService");
                UpdateStatus();
            }
            catch (Exception)
            {
                IsNotInstalled = true;
            }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        private bool isStopped;
        public bool IsStopped
        {
            get { return isStopped; }
            set
            {
                isStopped = value;
                OnPropertyChanged("IsStopped");
            }
        }

        // TODO: Add an imverse bool->vis converter and get rid of this double negative
        private bool isNotInstalled;
        public bool IsNotInstalled
        {
            get { return isNotInstalled; }
            set
            {
                isNotInstalled = value;
                OnPropertyChanged("IsNotInstalled");
            }
        }

        private readonly DelegateCommand startCommand;
        public DelegateCommand StartCommand { get { return startCommand; } }

        private bool CanStartService()
        {
            return !isNotInstalled && !statusUpdateWorker.IsBusy && service.Status == ServiceControllerStatus.Stopped;
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
            return !isNotInstalled && !statusUpdateWorker.IsBusy && service.CanStop && service.Status == ServiceControllerStatus.Running;
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

        private void DisplayNewStatus(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                IsNotInstalled = true;
                return;
            }
            UpdateStatus();
        }

        void UpdateStatus()
        {
            IsNotInstalled = false;
            IsRunning = service.Status == ServiceControllerStatus.Running;
            IsStopped = service.Status == ServiceControllerStatus.Stopped;
        }

        void UpdateServiceStatus(object sender, DoWorkEventArgs e)
        {
            var args = (StatusUpdateArgs)e.Argument;
            args.Method(service);
            service.WaitForStatus(args.FinalStatus, TimeSpan.FromSeconds(30));
            e.Result = service.Status;
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
