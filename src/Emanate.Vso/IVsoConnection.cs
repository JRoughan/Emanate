namespace Emanate.Vso
{
    public interface IVsoConnection
    {
        string GetProjects();
        string GetProject(string projectId);
        string GetBuild(string buildId);
    }
}