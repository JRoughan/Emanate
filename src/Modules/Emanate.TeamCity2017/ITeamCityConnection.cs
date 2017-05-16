using System.Linq;
using System.Threading.Tasks;

namespace Emanate.TeamCity2017
{
    public interface ITeamCityConnection
    {
        Task<string> GetProjects();
        Task<string> GetProject(string projectId);
        Task<string> GetBuild(string buildId);
    }
}