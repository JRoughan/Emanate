using System.Collections.Generic;
using System.Linq;
using Autofac.Features.Indexed;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
//using Nancy.Hosting.Self;
using Serilog;

namespace Emanate.Service
{
    public class EmanateService
    {
        private readonly GlobalConfig config;
        private readonly IIndex<string, IBuildMonitorFactory> buildMonitorFactories;

        private readonly Dictionary<IDevice, IBuildMonitor> activeBuildMonitors = new Dictionary<IDevice, IBuildMonitor>();
        //private NancyHost nancyHost;

        public EmanateService(GlobalConfig config, IIndex<string, IBuildMonitorFactory> buildMonitorFactories)
        {
            this.config = config;
            this.buildMonitorFactories = buildMonitorFactories;
        }

        public void Start()
        {
            Log.Information("=> EmanateService.Start");
            Initialize();
            foreach (var buildMonitor in activeBuildMonitors.Values)
            {
                Log.Information("Starting build monitor '{0}'", buildMonitor.GetType().Name);
                buildMonitor.BeginMonitoring();
            }
            //var apiUrl = new Uri($"http://localhost:{Settings.Port}/api/");
            //nancyHost = new NancyHost(apiUrl);
            //nancyHost.Start();
            //Log.Information($"Nancy now listening - {apiUrl}.");
        }

        public void Pause()
        {
            Log.Information("=> EmanateService.Pause");
        }

        public void Continue()
        {
            Log.Information("=> EmanateService.Continue");
        }

        public void Stop()
        {
            Log.Information("=> EmanateService.Stop");
            //nancyHost.Stop();
            foreach (var buildMonitor in activeBuildMonitors.Values)
            {
                Log.Information("Ending build monitor '{0}'", buildMonitor.GetType().Name);
                buildMonitor.EndMonitoring();
            }
        }

        private void Initialize()
        {
            Log.Information("=> EmanateService.Initialize");
            foreach (var mapping in config.Mappings)
            {
                Log.Information("Finding output device '{0}'", mapping.OutputDeviceId);
                var outputDevice = config.OutputDevices.Single(d => d.Id == mapping.OutputDeviceId);
                foreach (var inputGroup in mapping.InputGroups)
                {
                    Log.Information("Processing input group for device '{0}'", inputGroup.InputDeviceId);
                    var inputDevice = config.InputDevices.Single(d => d.Id == inputGroup.InputDeviceId);
                    IBuildMonitor monitor;
                    if (!activeBuildMonitors.TryGetValue(inputDevice, out monitor))
                    {
                        var factory = buildMonitorFactories[inputDevice.Key];
                        monitor = factory.Create(inputDevice);
                        activeBuildMonitors.Add(inputDevice, monitor);
                        Log.Information("Monitor '{0}' added", monitor.GetType());
                    }
                    monitor.AddBuilds(outputDevice, inputGroup.Inputs);
                }
            }
        }
    }
}
