using System;
using System.Collections.Generic;
using System.Linq;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomDevice : IOutputDevice
    {
        private const string key = "delcom";
        private const string defaultName = "Delcom";
        private BuildState lastCompletedState;
        private DateTimeOffset lastUpdateTime; // TODO: This should be used for device reconnection to discover decay level
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

        public List<InputInfo> Inputs { get; }

        private MonitoringProfile profile;
        public IOutputProfile Profile
        {
            get { return profile; }
            set { profile = value as MonitoringProfile; }
        }

        public PhysicalDevice PhysicalDevice { get; set; }

        public bool IsAvailable { get { return PhysicalDevice != null; } }

        public void UpdateStatus(BuildState state, DateTimeOffset timeStamp)
        {
            Log.Information("=> DelcomDevice.UpdateStatus");
            Log.Information("New state is '{0}'", state);
            if (profile.HasRestrictedHours)
            {
                var currentTime = DateTime.Now;
                if (currentTime.Hour < profile.StartTime || currentTime.Hour > profile.EndTime)
                {
                    Log.Information("Outside restricted hours");
                    lock (PhysicalDevice)
                    {
                        if (PhysicalDevice.IsOpen)
                        {
                            Log.Information("Turning off device");
                            PhysicalDevice.Close();
                        }
                        else
                            Log.Information("Ignoring update");

                        return;
                    }
                }
            }

            lock (PhysicalDevice)
            {
                if (!PhysicalDevice.IsOpen)
                {
                    Log.Information("Turning on device");
                    PhysicalDevice.Open();
                }
            }

            Log.Warning("Finding profile for state '{0}'", state);
            var profileState = profile.States.SingleOrDefault(p => p.BuildState == state);
            if (profileState == null)
            {
                Log.Warning("No profile found for state '{0}'", state);
                PhysicalDevice.Flash(Color.Red);
                return;
            }

            if (profileState.Buzzer && lastCompletedState != state)
            {
                // TODO: Make actual buzzer sound configurable
                Log.Information("Sounding buzzer");
                PhysicalDevice.StartBuzzer(100, 2, 20, 20);
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

                    Log.Information("Setting last completed state to '{0}'", state);
                    lastCompletedState = state;
                    break;
                case BuildState.Error:
                case BuildState.Failed:
                    SetColorBasedOnProfile(profileState, Color.Green, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Yellow, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Red, timeStamp);

                    Log.Information("Setting last completed state to '{0}'", state);
                    lastCompletedState = state;
                    break;
                case BuildState.Running:
                    SetColorBasedOnProfile(profileState, Color.Green, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Yellow, timeStamp);
                    SetColorBasedOnProfile(profileState, Color.Red, timeStamp);
                    break;
            }
            lastCompletedState = state != BuildState.Running ? state : lastCompletedState;
            Log.Information("Setting last update time to '{0}'", timeStamp);
            lastUpdateTime = timeStamp;
        }

        private void SetColorBasedOnProfile(ProfileState profileState, Color color, DateTimeOffset timeStamp)
        {
            Log.Information("=> DelcomDevice.SetColorBasedOnProfile");
            var isColorTurnedOn = color == Color.Green && profileState.Green ||
                                  color == Color.Yellow && profileState.Yellow ||
                                  color == Color.Red && profileState.Red;

            if (isColorTurnedOn)
            {
                var intensity = GetColorIntensity(color, timeStamp);
                if (profileState.Flash)
                {
                    Log.Information("Flashing '{0}' at intensity '{1}'", color.Name, intensity);
                    PhysicalDevice.Flash(color, intensity);
                }
                else
                {
                    Log.Information("Turning on '{0}' at intensity '{1}'", color.Name, intensity);
                    PhysicalDevice.TurnOn(color, intensity);
                }
            }
            else
            {
                Log.Information("Turning off '{0}'", color.Name);
                PhysicalDevice.TurnOff(color);
            }
        }

        private byte GetColorIntensity(Color color, DateTimeOffset timeStamp)
        {
            Log.Information("=> DelcomDevice.GetColorIntensity");

            if (profile.Decay <= 0)
                return color.MaxPower;

            var minutesSinceLastBuild = (DateTimeOffset.Now - timeStamp).TotalMinutes;
            if (minutesSinceLastBuild > profile.Decay * 60)
                return color.MinPower;

            var power = color.MaxPower - (minutesSinceLastBuild / profile.Decay * 60) * (color.MaxPower - color.MinPower);
            return (byte)power;
        }
    }
}