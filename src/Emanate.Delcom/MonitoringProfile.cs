using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Delcom
{
    public class MonitoringProfile : IOutputProfile
    {
        public string Key { get; set; }

        public List<ProfileState> States { get; } = new List<ProfileState>();

        public uint Decay { get; set; }

        public bool HasRestrictedHours { get; set; }

        public uint StartTime { get; set; }

        public uint EndTime { get; set; }
    }
}