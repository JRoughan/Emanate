using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        IEnumerable<TeamProjectReference> GetProjects();
        TeamProject GetProject(Guid projectId);
        Build GetBuild(Guid projectId, int buildId);
        IEnumerable<DefinitionReference> GetBuildDefinitions(Guid projectId);
    }
}