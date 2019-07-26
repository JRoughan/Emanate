using Emanate.Model;

namespace Emanate.TeamCity
{
    public class TeamCityDevice : SourceDevice
    {
        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RequiresAuthentication { get; set; }
    }
}