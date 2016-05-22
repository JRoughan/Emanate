using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Nancy.Hosting.Self;
using Serilog;

namespace Emanate.Service
{
    public class EmanateService
    {
        private readonly IComponentContext componentContext;
        private readonly Dictionary<string, IBuildMonitor> buildMonitors = new Dictionary<string, IBuildMonitor>();
        //private NancyHost nancyHost;

        public EmanateService(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public void Start()
        {
            Log.Information("=> EmanateService.Start");
            Initialize();
            foreach (var buildMonitor in buildMonitors.Values)
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
            foreach (var buildMonitor in buildMonitors.Values)
            {
                Log.Information("Ending build monitor '{0}'", buildMonitor.GetType().Name);
                buildMonitor.EndMonitoring();
            }
        }

        private void Initialize()
        {
            Log.Information("=> EmanateService.Initialize");
            var config = componentContext.Resolve<GlobalConfig>();
            foreach (var mapping in config.Mappings)
            {
                Log.Information("Finding output device '{0}'", mapping.OutputId);
                var outputDevice = config.OutputDevices.Single(d => d.Id == mapping.OutputId);
                foreach (var inputGroup in mapping.Inputs.GroupBy(i => i.Source))
                {
                    Log.Information("Processing input group '{0}'", inputGroup.Key);
                    IBuildMonitor monitor;
                    if (!buildMonitors.TryGetValue(inputGroup.Key, out monitor))
                    {
                        monitor = componentContext.ResolveKeyed<IBuildMonitor>(inputGroup.Key);
                        buildMonitors.Add(inputGroup.Key, monitor);
                        Log.Information("Monitor '{0}' added", monitor.GetType());
                    }
                    foreach (var inputInfo in inputGroup)
                    {
                        var inputDevice = config.InputDevices.Single(d => d.Id == inputInfo.InputDeviceId);
                        monitor.AddMapping(inputDevice, outputDevice, inputInfo);
                    }
                }
            }
        }
    }
}
