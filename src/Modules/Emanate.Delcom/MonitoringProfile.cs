using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;

namespace Emanate.Delcom
{
    public class MonitoringProfile : IProfile
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ProfileState> States { get; } = new List<ProfileState>();

        public uint Decay { get; set; }

        public bool HasRestrictedHours { get; set; }

        public uint StartTime { get; set; }

        public uint EndTime { get; set; }

        public XElement CreateMemento()
        {
            var profileElement = new XElement("profile");
            profileElement.Add(new XAttribute("id", Id));
            profileElement.Add(new XAttribute("name", Name));
            profileElement.Add(new XAttribute("decay", Decay));
            profileElement.Add(new XAttribute("restrictedhours", HasRestrictedHours));
            profileElement.Add(new XAttribute("starttime", StartTime));
            profileElement.Add(new XAttribute("endtime", EndTime));

            foreach (var state in States)
            {
                var stateElement = new XElement("state");
                stateElement.Add(new XAttribute("name", Enum.GetName(typeof(BuildState), state.BuildState)));
                stateElement.Add(new XAttribute("green", state.Green));
                stateElement.Add(new XAttribute("yellow", state.Yellow));
                stateElement.Add(new XAttribute("red", state.Red));
                stateElement.Add(new XAttribute("flash", state.Flash));
                stateElement.Add(new XAttribute("buzzer", state.Buzzer));
                profileElement.Add(stateElement);
            }
            return profileElement;
        }

        public void SetMemento(XElement profileElement)
        {
            Id = Guid.Parse(profileElement.GetAttributeString("id"));
            Name = profileElement.GetAttributeString("name");
            Decay = profileElement.GetAttributeUint("decay");
            HasRestrictedHours = profileElement.GetAttributeBoolean("restrictedhours");
            StartTime = profileElement.GetAttributeUint("starttime");
            EndTime = profileElement.GetAttributeUint("endtime");

            foreach (var stateElement in profileElement.Elements("state"))
            {
                var state = new ProfileState
                {
                    BuildState = stateElement.GetAttributeEnum("name", BuildState.Unknown),
                    Green = stateElement.GetAttributeBoolean("green"),
                    Yellow = stateElement.GetAttributeBoolean("yellow"),
                    Red = stateElement.GetAttributeBoolean("red"),
                    Flash = stateElement.GetAttributeBoolean("flash"),
                    Buzzer = stateElement.GetAttributeBoolean("buzzer")
                };
                States.Add(state);
            }
        }
    }
}