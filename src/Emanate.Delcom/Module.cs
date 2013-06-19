using Autofac;
using Emanate.Core;
using Emanate.Core.Output;
using Emanate.Core.Output.DelcomVdi;

namespace Emanate.TeamCity
{
    public class Module : IModule
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DelcomOutput>().As<IOutput>();
        }
    }
}
