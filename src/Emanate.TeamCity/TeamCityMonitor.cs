using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Serilog;
using Timer = System.Timers.Timer;

namespace Emanate.TeamCity
{
    public class TeamCityMonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IDevice, TeamCityMonitor> monitorFactory;

        public TeamCityMonitorFactory(Func<IDevice, TeamCityMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IDevice device)
        {
            return monitorFactory(device);
        }
    }

    public class TeamCityMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private const string teamCityDateFormat = "yyyyMMdd'T'HHmmsszzz";
        private readonly TimeSpan lockingInterval;
        private readonly ITeamCityConnection teamCityConnection;
        private readonly Timer timer;
        private readonly Dictionary<IOutputDevice, Dictionary<string, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<string, BuildState>>();
        private readonly Dictionary<string, BuildState> stateMap = new Dictionary<string, BuildState>
                                                              {
                                                                  { "UNKNOWN", BuildState.Unknown },
                                                                  { "RUNNING", BuildState.Running },
                                                                  { "ERROR", BuildState.Error },
                                                                  { "FAILURE", BuildState.Failed },
                                                                  { "SUCCESS", BuildState.Succeeded }
                                                              };

        public TeamCityMonitor(IDevice device)
        {
            var vsoDevice = (TeamCityDevice)device;
            teamCityConnection = new TeamCityConnection(vsoDevice);

            var pollingInterval = vsoDevice.PollingInterval * 1000;
            if (pollingInterval < 1)
                pollingInterval = 30000; // default to 30 seconds

            lockingInterval = TimeSpan.FromSeconds(pollingInterval / 2.0);

            timer = new Timer(pollingInterval);
            timer.Elapsed += PollStatus;
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
            UpdateBuildStates();
            Log.Information("Starting polling timer");
            timer.Start();
        }

        public void EndMonitoring()
        {
            Log.Information("=> TeamCityModule.EndMonitoring");
            timer.Stop();
        }

        private void PollStatus(object sender, ElapsedEventArgs e)
        {
            Log.Information("=> TeamCityMonitor.PollStatus");
            if (!Monitor.TryEnter(pollingLock, lockingInterval))
            {
                Log.Warning("Could not acquire polling lock - skipping attempt");
                return;
            }

            try
            {
                UpdateBuildStates();
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
            finally
            {
                Monitor.Exit(pollingLock);
            }
        }

        private void UpdateBuildStates()
        {
            Log.Information("=> TeamCityModule.UpdateBuildStates");
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
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (outputDevice.IsAvailable)
                    outputDevice.UpdateStatus(BuildState.Error, DateTime.Now);
            }
        }

        private IEnumerable<BuildInfo> GetNewBuildStates(IEnumerable<string> buildIds)
        {
            Log.Information("=> TeamCityModule.GetNewBuildStates");
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
