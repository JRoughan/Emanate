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
        //private readonly Dictionary<string, IOutputDevice> outputDevices = new Dictionary<string, IOutputDevice>();

        public EmanateService(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            InitializeComponent();
        }

        public void Initialize(GlobalConfig config)
        {
            foreach (var outputDevice in config.OutputDevices)
            {
                //outputDevices.Add(outputDevice.Key, outputDevice);

                foreach (var inputGroup in outputDevice.Inputs.GroupBy(i => i.Source))
                {
                    IBuildMonitor monitor;
                    if (!buildMonitors.TryGetValue(inputGroup.Key, out monitor))
                    {
                        monitor = componentContext.ResolveKeyed<IBuildMonitor>(inputGroup.Key);
                        buildMonitors.Add(inputGroup.Key, monitor);
                    }
                    monitor.AddBuilds(outputDevice, inputGroup.Select(i => i.Id));
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
                buildMonitor.EndMonitoring();
        }
    }
}
