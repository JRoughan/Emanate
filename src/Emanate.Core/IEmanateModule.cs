using Autofac;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void LoadAdminComponents(ContainerBuilder builder);
        void LoadServiceComponents(ContainerBuilder builder);
    }
}
