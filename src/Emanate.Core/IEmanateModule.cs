using Autofac;
using Emanate.Core.Configuration;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void LoadServiceComponents(ContainerBuilder builder);
    }

    public interface IEmanateAdminModule
    {
        void LoadAdminComponents(ContainerBuilder builder);
    }

    public interface IModule
    {
        string Key { get; }
        Direction Direction { get; }

        IConfiguration GenerateDefaultConfig();
    }

    public enum Direction
    {
        Unknown = 0,
        Input,
        Output
    }
}
