using Emanate.Core;
using Emanate.Core.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Emanate.TeamCity
{
    public class TeamCityModule : IEmanateModule, IModule
    {
        public string Key { get; } = "teamcity";
        public string Name { get; } = "TeamCity";
        public Direction Direction { get; } = Direction.Input;

        public void LoadServiceComponents(IServiceCollection services)
        {
            Log.Information("=> TeamCityModule.LoadServiceComponents");
            services.AddTransient<ITeamCityConnection, TeamCityConnection>();
            services.AddSingleton<IBuildMonitorFactory, TeamCityMonitorFactory>();
            services.AddTransient<TeamCityMonitor>();
        }
    }
}
