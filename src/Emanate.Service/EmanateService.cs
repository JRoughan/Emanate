using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Input;

namespace Emanate.Service
{
    public partial class EmanateService : ServiceBase
    {
        private readonly IComponentContext componentContext;
        private readonly Dictionary<string, IBuildMonitor> buildMonitors = new Dictionary<string, IBuildMonitor>();

        public EmanateService(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            InitializeComponent();
        }

        public void Initialize(GlobalConfig config)
        {
            Trace.TraceInformation("=> EmanateService.Initialize");
            foreach (var outputDevice in config.OutputDevices)
            {
                Trace.TraceInformation("Processing output device '{0}'", outputDevice.Name);
                foreach (var inputGroup in outputDevice.Inputs.GroupBy(i => i.Source))
                {
                    Trace.TraceInformation("Processing input group '{0}'", inputGroup.Key);
                    IBuildMonitor monitor;
                    if (!buildMonitors.TryGetValue(inputGroup.Key, out monitor))
                    {
                        monitor = componentContext.ResolveKeyed<IBuildMonitor>(inputGroup.Key);
                        buildMonitors.Add(inputGroup.Key, monitor);
                        Trace.TraceInformation("Monitor '{0}' added", monitor.GetType());
                    }
                    monitor.AddBuilds(outputDevice, inputGroup);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            Trace.TraceInformation("=> EmanateService.OnStart");
            foreach (var buildMonitor in buildMonitors.Values)
            {
                Trace.TraceInformation("Starting build monitor '{0}'", buildMonitor.GetType().Name);
                buildMonitor.BeginMonitoring();
            }
        }

        protected override void OnStop()
        {
            Trace.TraceInformation("=> EmanateService.OnStop");
            foreach (var buildMonitor in buildMonitors.Values)
            {
                Trace.TraceInformation("Ending build monitor '{0}'", buildMonitor.GetType().Name);
                buildMonitor.EndMonitoring();
            }
        }
    }
}
