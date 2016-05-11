using System;

namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        dynamic GetProjects();
        dynamic GetBuild(Guid projectId, int buildId);
        dynamic GetBuildDefinitions(Guid projectId);
    }
}