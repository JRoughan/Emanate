using System;
using System.Collections.Generic;
using System.Linq;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;

namespace Emanate.Delcom
{
    public class DelcomDevice : IOutputDevice
    {
        private const string key = "delcom";
        private const string defaultName = "Delcom";
        private BuildState lastCompletedState;
        private DateTimeOffset lastUpdateTime; // TODO: This should be used fo device reconnection to discover decay level
        //private const int minutesTillFullDim = 24 * 60; // 1 full day

        string IOutputDevice.Key { get { return key; } }

        private string name;

        public string Name
        {
            get { return name ?? defaultName; }
            set { name = value; }
        }

        public DelcomDevice()
        {
            Inputs = new List<InputInfo>();
        }

        public string Id { get; set; }

        public string Type { get { return key; } }

        public List<InputInfo> Inputs { get; private set; }

        private MonitoringProfile profile;
        public IOutputProfile Profile
        {
            get { return profile; }
            set { profile = value as MonitoringProfile; }
        }

        public PhysicalDevice PhysicalDevice { get; set; }

        public void UpdateStatus(BuildState state, DateTimeOffset timeStamp)
        {
            lock (PhysicalDevice)
            {
                if (!PhysicalDevice.IsOpen)
                    PhysicalDevice.Open();
            }

            var profileState = profile.States.SingleOrDefault(p => p.BuildState == state);
            if (profileState == null)
            {
                PhysicalDevice.Flash(Color.Red);
                return;
            }

            switch (state)
            {
                case BuildState.Unknown:
                    SetColorBasedOnProfile(profileState, Color.Green, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Yellow, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Red, timeStamp);
                    break;
                case BuildState.Succeeded:
                    SetColorBasedOnProfile(profileState, Color.Green, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Yellow, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Red, timeStamp);

                    lastCompletedState = state;
                    break;
                case BuildState.Error:
                case BuildState.Failed:
                    SetColorBasedOnProfile(profileState, Color.Green, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Yellow, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Red, timeStamp);

                    // TODO: Move buzzer flag to profile
                    if (lastCompletedState != BuildState.Failed && lastCompletedState != BuildState.Error)
                        PhysicalDevice.StartBuzzer(100, 2, 20, 20);

                    lastCompletedState = state;
                    break;
                case BuildState.Running:
                    SetColorBasedOnProfile(profileState, Color.Green, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Yellow, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Red, timeStamp);
                    break;
            }
            lastCompletedState = state != BuildState.Running ? state : lastCompletedState;
            lastUpdateTime = timeStamp;
        }

        private void SetColorBasedOnProfile(ProfileState profileState, Color color, DateTimeOffset timeStamp)
        {
            var isColorTurnedOn = color == Color.Green && profileState.Green ||
                                  color == Color.Yellow && profileState.Yellow ||
                                  color == Color.Red && profileState.Red;

            if (isColorTurnedOn)
            {
                if (profile.Decay > 0)
                    TurnOnColorWithCustomPowerLevel(color, timeStamp, profileState.Flash);
                else
                {
                    if (profileState.Flash)
                        PhysicalDevice.Flash(color);
                    else
                        PhysicalDevice.TurnOn(color);
                }
            }
            else
            {
                PhysicalDevice.TurnOff(color);
            }
        }

        private void TurnOnColorWithCustomPowerLevel(Color color, DateTimeOffset timeStamp, bool flash)
        {
            var minutesSinceLastBuild = (DateTimeOffset.Now - timeStamp).TotalMinutes;
            if (minutesSinceLastBuild > profile.Decay * 60)
            {
                if (flash)
                    PhysicalDevice.Flash(color, color.MinPower);
                else
                    PhysicalDevice.TurnOn(color, color.MinPower);
            }
            else
            {
                var power = color.MaxPower - (minutesSinceLastBuild / profile.Decay * 60) * (color.MaxPower - color.MinPower);
                if (flash)
                    PhysicalDevice.Flash(color, (byte)power);
                else
                    PhysicalDevice.TurnOn(color, (byte)power);
            }
        }

    }
}