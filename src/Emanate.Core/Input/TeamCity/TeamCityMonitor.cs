namespace Emanate.Core.Input.TeamCity
{
    public class TeamCityMonitor : IBuildMonitor
    {
        private readonly TeamCityConnection connection;

        public TeamCityMonitor(IConfiguration configuration)
        {
            connection = new TeamCityConnection(configuration);
        }

        public string GetProjects()
        {
            var uri = connection.CreateUri("/httpAuth/app/rest/projects");

            return connection.Request(uri);
        }
    }
}
