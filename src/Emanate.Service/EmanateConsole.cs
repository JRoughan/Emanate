using System.Threading;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service
{
    public class EmanateConsole : EmanateService
    {
        public EmanateConsole(IBuildMonitor monitor, IOutput output)
            : base(monitor, output) { }

        public void Start()
        {
            base.OnStart(null);
            SpinWait.SpinUntil(() => false);
        }
    }
}