using System;
using System.Threading.Tasks;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        Task<ProjectCollection> GetProjects(bool forceRefresh = false);
        Task<BuildDefinitionCollection> GetBuildDefinitions(Guid projectId, bool forceRefresh = false);

        Task<Build> GetBuild(Guid projectId, int buildId);
    }
}