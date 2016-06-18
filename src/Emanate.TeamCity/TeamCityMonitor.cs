using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Serilog;

namespace Emanate.TeamCity
{
    public class TeamCityMonitor : IBuildMonitor
    {
        private const string teamCityDateFormat = "yyyyMMdd'T'HHmmsszzz";
        private readonly TimeSpan delayInterval;
        private readonly ITeamCityConnection teamCityConnection;
        private readonly Dictionary<IOutputDevice, Dictionary<string, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<string, BuildState>>();
        private readonly Dictionary<string, BuildState> stateMap = new Dictionary<string, BuildState>
                                                              {
                                                                  { "UNKNOWN", BuildState.Unknown },
                                                                  { "RUNNING", BuildState.Running },
                                                                  { "ERROR", BuildState.Error },
                                                                  { "FAILURE", BuildState.Failed },
                                                                  { "SUCCESS", BuildState.Succeeded }
                                                              };

        private bool isMonitoring;

        public TeamCityMonitor(IDevice device)
        {
            var vsoDevice = (TeamCityDevice)device;
            teamCityConnection = new TeamCityConnection(vsoDevice);

            var pollingInterval = vsoDevice.PollingInterval > 0 ? vsoDevice.PollingInterval : 30; // default to 30 seconds
            delayInterval = TimeSpan.FromSeconds(pollingInterval);
        }

        public BuildState CurrentState { get; private set; }

        public void AddBuilds(IOutputDevice outputDevice, IEnumerable<string> inputs)
        {
            Log.Information("=> TeamCityMonitor.AddBuilds");
            buildStates.Add(outputDevice, inputs.ToDictionary(b => b, b => BuildState.Unknown));
        }

        public void BeginMonitoring()
        {
            Log.Information("=> TeamCityMonitor.BeginMonitoring");
            isMonitoring = true;
            Task.Run(() => { UpdateLoop(); });
        }

        public void EndMonitoring()
        {
            Log.Information("=> TeamCityMonitor.EndMonitoring");
            isMonitoring = false;
        }

        private async void UpdateLoop()
        {
            while (isMonitoring)
            {
                try
                {
                    await UpdateBuildStates();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Could not update build states");
                    try
                    {
                        DisplayErrorOnAllOutputs();
                    }
                    catch (Exception ex1)
                    {
                        Log.Error(ex1, "Could not display error on output devices");
                        throw;
                    }
                }
                Log.Information($"Waiting for {delayInterval} seconds");
                await Task.Delay(delayInterval);
            }
        }

        private async Task UpdateBuildStates()
        {
            Log.Information("=> TeamCityMonitor.UpdateBuildStates");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (!outputDevice.IsAvailable)
                {
                    Log.Warning($"Output device '{outputDevice.Name}' unavailable - skipping update");
                    continue;
                }

                var newStates = GetNewBuildStates(output.Value.Keys).ToList();

                var newState = BuildState.Unknown;
                var timeStamp = DateTimeOffset.Now;


                if (newStates.Any())
                {
                    var states = buildStates[outputDevice];
                    foreach (var buildState in newStates)
                        states[buildState.BuildId] = buildState.State;

                    newState = (BuildState)newStates.Max(s => (int)s.State);

                    timeStamp = newStates.Max(s => s.TimeStamp);
                }

                var oldState = CurrentState;
                CurrentState = newState;
                outputDevice.UpdateStatus(newState, timeStamp);
            }
        }

        private void DisplayErrorOnAllOutputs()
        {
            Log.Information("=> TeamCityMonitor.DisplayErrorOnAllOutputs");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (outputDevice.IsAvailable)
                    outputDevice.UpdateStatus(BuildState.Error, DateTime.Now);
            }
        }

        private IEnumerable<BuildInfo> GetNewBuildStates(IEnumerable<string> buildIds)
        {
            Log.Information("=> TeamCityMonitor.GetNewBuildStates");
            foreach (var buildId in buildIds)
            {
                var resultXml = teamCityConnection.GetBuild(buildId);

                var resultRoot = XElement.Parse(resultXml);
                var buildXml = resultRoot.Elements("build").SingleOrDefault(); // Need to check for null in case build no longer exists
                if (buildXml != null)
                {
                    var build = new
                                    {
                                        IsRunning = buildXml.Attribute("running") != null,
                                        Status = buildXml.GetAttributeString("status"),
                                        TimeStamp = buildXml.GetAttributeDateTime("startDate", teamCityDateFormat)
                                    };

                    var state = ConvertState(build.Status);

                    if (build.IsRunning && state == BuildState.Succeeded)
                        state = BuildState.Running;

                    yield return new BuildInfo { BuildId = buildId, State = state, TimeStamp = build.TimeStamp};
                }
                else
                    Log.Warning("Build '{0}' invalid", buildId);
            }
        }

        private BuildState ConvertState(string state)
        {
            BuildState convertedState;
            if (stateMap.TryGetValue(state, out convertedState))
                return convertedState;

            Log.Information("State '{0}' is not supported.", state);
            return BuildState.Unknown;
        }

        [DebuggerDisplay("{BuildId} - {State}")]
        class BuildInfo
        {
            public string BuildId { get; set; }
            public BuildState State { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }
    }
}
