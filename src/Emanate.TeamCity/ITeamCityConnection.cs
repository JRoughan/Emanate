using System.Threading.Tasks;

namespace Emanate.TeamCity
{
    public interface ITeamCityConnection
    {
        Task<string> GetProjects();
        Task<string> GetProject(string projectId);
        Task<string> GetBuild(string buildId);
    }
}