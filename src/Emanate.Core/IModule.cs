using Autofac;

namespace Emanate.Core
{
    public interface IModule
    {
        void Load(ContainerBuilder builder);
    }
}
