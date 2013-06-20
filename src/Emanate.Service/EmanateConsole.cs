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
            base.OnStart(null);
            SpinWait.SpinUntil(() => false);
        }
    }
}