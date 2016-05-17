using System;
using System.Threading.Tasks;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        Task<dynamic> GetProjects();
        Task<dynamic> GetBuild(Guid projectId, int buildId);
        Task<dynamic> GetBuildDefinitions(Guid projectId);
    }
}