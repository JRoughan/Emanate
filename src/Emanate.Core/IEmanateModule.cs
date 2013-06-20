using Autofac;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void Load(ContainerBuilder builder);
    }
}
