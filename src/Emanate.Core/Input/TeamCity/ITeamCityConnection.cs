namespace Emanate.Core.Input.TeamCity
{
    public interface ITeamCityConnection
    {
        string GetProjects();
        string GetProject(string projectId);
        string GetBuild(string buildId);
        string GetBuilds(string buildId, int count);
    }
}