using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        Task<IEnumerable<TeamProjectReference>> GetProjects();
        Task<TeamProject> GetProject(Guid projectId);
        Task<Build> GetBuild(Guid projectId, int buildId);
        Task<IEnumerable<DefinitionReference>> GetBuildDefinitions(Guid projectId);
    }
}