using System.Timers;

namespace Emanate.Core.Input.TeamCity
{
    public class TeamCityMonitor : IBuildMonitor
    {
        private readonly Timer timer;
        private readonly IConfiguration configuration;
        private readonly TeamCityConnection connection;

        public TeamCityMonitor(IConfiguration configuration)
        {
            this.configuration = configuration;
            connection = new TeamCityConnection(configuration);

            var pollingInterval = configuration.GetInt("TeamCityPollingInterval") * 1000;
            timer = new Timer(pollingInterval);
            timer.Elapsed += PollTeamCityStatus;
        }

        public string GetProjects()
        {
            var uri = connection.CreateUri("/httpAuth/app/rest/projects");

            return connection.Request(uri);
        }

        public void BeginMonitoring()
        {
           
            timer.Start();
        }

        public void EndMonitoring()
        {
            timer.Stop();
        }

        void PollTeamCityStatus(object sender, ElapsedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
