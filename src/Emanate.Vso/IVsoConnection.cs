using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        dynamic GetProjects();
        TeamProject GetProject(Guid projectId);
        Build GetBuild(Guid projectId, int buildId);
        dynamic GetBuildDefinitions(Guid projectId);
    }
}