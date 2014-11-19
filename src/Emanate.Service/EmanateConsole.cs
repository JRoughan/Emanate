using System.Diagnostics;
using System.Threading;
using Autofac;

namespace Emanate.Service
{
    public class EmanateConsole : EmanateService
    {
        public EmanateConsole(IComponentContext componentContext)
            : base(componentContext) { }

        public void Start()
        {
            Trace.TraceInformation("=> EmanateConsole.Start");
            base.OnStart(null);
            Trace.TraceInformation("Spinning to simulate service loop");
            SpinWait.SpinUntil(() => false);
        }
    }
}