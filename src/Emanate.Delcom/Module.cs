using Autofac;
using Emanate.Core;
using Emanate.Core.Output;

namespace Emanate.Delcom
{
    public class Module : IModule
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DelcomOutput>().As<IOutput>();
        }
    }
}
