namespace Emanate.Core.Input.TeamCity
{
    public interface ITeamCityConnection
    {
        string GetProjects();
        string GetProject(string projectId);
        string GetRunningBuilds();
        string GetBuild(string buildId);
    }
}