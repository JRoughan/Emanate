using Autofac;

namespace Emanate.Core
{
    public interface IEmanateAdminModule
    {
        void LoadAdminComponents(ContainerBuilder builder);
    }
}