using Autofac;
using Emanate.Core.Configuration;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        string Key { get; }
        void LoadAdminComponents(ContainerBuilder builder);
        void LoadServiceComponents(ContainerBuilder builder);
    }

    public interface IOutputModule : IEmanateModule
    {
        IOutputConfiguration GenerateDefaultConfig();
    }

    public interface IInputModule : IEmanateModule
    {
        IInputConfiguration GenerateDefaultConfig();
    }
}
