namespace Emanate.TeamCity
{
    public interface ITeamCityConnection
    {
        string GetProjects();
        string GetProject(string projectId);
        string GetBuild(string buildId);
    }
}