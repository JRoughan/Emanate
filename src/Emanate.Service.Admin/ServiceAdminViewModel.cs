using System;
using System.Linq;
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

        public ServiceAdminViewModel()
        {
            StartCommand = new DelegateCommand(StartService, CanStartService);
            StopCommand = new DelegateCommand(StopService, CanStopService);
            RestartCommand = new DelegateCommand(RestartService, CanStopService);
        }

        public override async Task<InitializationResult> Initialize()
        {
            try
            {
                service = await GetEmanateService();
                if (service != null)
                {
                    Log.Information("Found Emanate service");
                    IsInstalled = true;
                    UpdateStatus(service.Status);
                }
                else
                {
                    Log.Warning("Emanate service missing");
                    IsInstalled = false;
                    return InitializationResult.Failed;
                }
            }
            catch (Exception)
            {
                Log.Warning("Emanate service missing");
                IsInstalled = false;
                return InitializationResult.Failed;
            }
            return InitializationResult.Succeeded;
        }

        private async Task<ServiceController> GetEmanateService()
        {
            return await Task.Run(() =>
            {
                var services = ServiceController.GetServices();
                var installedService = services.FirstOrDefault(s => s.ServiceName == "EmanateService"); // TODO: Dynamically determine service name
                return installedService;
            });
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

        public DelegateCommand StartCommand { get; }

        private bool CanStartService()
        {
            return isInstalled && service.Status == ServiceControllerStatus.Stopped;
        }

        private async void StartService()
        {
            Log.Information("Starting Emanate service");
            await UpdateService(s => s.Start(), ServiceControllerStatus.Running);
        }

        public DelegateCommand StopCommand { get; }

        private bool CanStopService()
        {
            return isInstalled && service.CanStop && service.Status == ServiceControllerStatus.Running;
        }

        private async void StopService()
        {
            Log.Information("Stopping Emanate service");
            await UpdateService(s => s.Stop(), ServiceControllerStatus.Stopped);
        }

        public DelegateCommand RestartCommand { get; }

        private async void RestartService()
        {
            Log.Information("Restarting Emanate service");
            await UpdateService(s => { s.Stop(); s.WaitForStatus(ServiceControllerStatus.Stopped); s.Start(); }, ServiceControllerStatus.Running);
        }

        void UpdateStatus(ServiceControllerStatus status)
        {
            IsRunning = status == ServiceControllerStatus.Running;
            IsStopped = status == ServiceControllerStatus.Stopped;
        }

        private async Task UpdateService(Action<ServiceController> updateAction, ServiceControllerStatus expectedStatus, int timeoutSeconds = 30)
        {
            try
            {
                var status = await Task.Run(() =>
                {
                    Log.Information("Updating service");
                    updateAction(service);
                    Log.Information("Waiting for Emanate service status");
                    service.WaitForStatus(expectedStatus, TimeSpan.FromSeconds(timeoutSeconds));
                    var resultStatus = service.Status;
                    if (resultStatus == expectedStatus)
                        Log.Information("Emanate service status: " + resultStatus);
                    else
                        Log.Warning("Unexpected Emanate service status: " + resultStatus);

                    return resultStatus;
                });
                UpdateStatus(status);
            }
            catch (Exception e)
            {
                Log.Warning(e, "Could not update service");
                MessageBox.Show("Could not update service: " + e.Message);
            }
        }
    }
}
