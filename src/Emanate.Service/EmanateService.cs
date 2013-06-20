using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public partial class EmanateService : ServiceBase
    {
        private readonly IComponentContext componentContext;
        private readonly Dictionary<string, IBuildMonitor> buildMonitors = new Dictionary<string, IBuildMonitor>();
        private readonly Dictionary<string, IOutput> outputs = new Dictionary<string, IOutput>();

        public EmanateService(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            InitializeComponent();
        }

        public void Initialize(GlobalConfig config)
        {
            foreach (var outputDevice in config.OutputDevices)
            {
                IOutput output;
                if (!outputs.TryGetValue(outputDevice.Key, out output))
                {
                    //var device = componentContext.ResolveKeyed<IOutputDevice>(outputDevice.Key);
                    output = componentContext.ResolveKeyed<IOutput>(outputDevice.Key);
                    outputs.Add(outputDevice.Key, output);
                }

                foreach (var inputGroup in outputDevice.Inputs.GroupBy(i => i.Source))
                {
                    IBuildMonitor monitor;
                    if (!buildMonitors.TryGetValue(inputGroup.Key, out monitor))
                    {
                        monitor = componentContext.ResolveKeyed<IBuildMonitor>(inputGroup.Key);
                        buildMonitors.Add(inputGroup.Key, monitor);
                    }
                    monitor.StatusChanged += output.UpdateStatus;
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            foreach (var buildMonitor in buildMonitors.Values)
                buildMonitor.BeginMonitoring();
        }

        protected override void OnStop()
        {
            foreach (var buildMonitor in buildMonitors.Values)
            {
                buildMonitor.EndMonitoring();
                foreach (var output in outputs.Values)
                    buildMonitor.StatusChanged -= output.UpdateStatus;
            }
        }
    }
}
