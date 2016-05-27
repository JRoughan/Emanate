using System;
using System.Threading.Tasks;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        Task<ProjectCollection> GetProjects();
        Task<Build> GetBuild(Guid projectId, int buildId);
        Task<BuildDefinitionCollection> GetBuildDefinitions(Guid projectId);
    }
}