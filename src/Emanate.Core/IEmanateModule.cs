using Autofac;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void LoadServiceComponents(ContainerBuilder builder);
    }
}
