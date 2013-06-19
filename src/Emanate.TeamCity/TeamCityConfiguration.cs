using Emanate.Core.Configuration;

namespace Emanate.TeamCity
{
    [Configuration("TeamCity")]
    public class TeamCityConfiguration
    {
        [Key("TeamCityUri")]
        public string Uri { get; set; }

        [Key("TeamCityPollingInterval")]
        public int PollingInterval { get; set; }

        [Key("TeamCityUser")]
        public string UserName { get; set; }

        [Key("TeamCityPassword", IsPassword=true)]
        public string Password { get; set; }

        [Key("TeamCityGuestAuthentication")]
        public bool IsUsingGuestAuthentication { get; set; }

        [Key("TeamCityBuilds")]
        public string BuildsToMonitor { get; set; }
    }
}