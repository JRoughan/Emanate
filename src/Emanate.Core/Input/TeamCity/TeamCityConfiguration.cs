namespace Emanate.Core.Input.TeamCity
{
    [Configuration("TeamCity")]
    public class TeamCityConfiguration
    {
        [Key("TeamCityUri")]
        public string TeamCityUri { get; set; }

        [Key("TeamCityPollingInterval")]
        public int PollingInterval { get; set; }

        [Key("TeamCityUser")]
        public string UserName { get; set; }

        [Key("TeamCityPassword")]
        public string Password { get; set; }

        [Key("TeamCityGuestAuthentication")]
        public bool IsUsingGuestAuthentication { get; set; }

        [Key("TeamCityBuilds")]
        public string BuildsToMonitor { get; set; }
    }
}