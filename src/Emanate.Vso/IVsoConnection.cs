using System;
using System.Threading.Tasks;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        Task<ProjectCollection> GetProjects();
        Task<BuildDefinitionCollection> GetBuildDefinitions(Guid projectId);

        Task<Build> GetBuild(Guid projectId, int buildId);
    }
}