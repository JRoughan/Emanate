using System.ServiceProcess;

namespace Emanate.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[] { new MonitoringService() };
            ServiceBase.Run(servicesToRun);
        }
    }
}
