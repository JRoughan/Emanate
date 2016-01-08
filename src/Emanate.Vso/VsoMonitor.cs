using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.Vso.Configuration;
using Timer = System.Timers.Timer;

namespace Emanate.Vso
{
    public class VsoMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private const string vsoDateFormat = "yyyyMMdd'T'HHmmsszzz";
        private readonly TimeSpan lockingInterval;
        private readonly IVsoConnection vsoConnection;
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


        public VsoMonitor(IVsoConnection vsoConnection, VsoConfiguration configuration)
        {
            this.vsoConnection = vsoConnection;

            var pollingInterval = configuration.PollingInterval * 1000;
            if (pollingInterval < 1)
                pollingInterval = 30000; // default to 30 seconds

            lockingInterval = TimeSpan.FromSeconds(pollingInterval / 2.0);

            timer = new Timer(pollingInterval);
            timer.Elapsed += PollStatus;
        }

        public BuildState CurrentState { get; private set; }

        public void AddBuilds(IOutputDevice outputDevice, IEnumerable<string> buildIds)
        {
            Trace.TraceInformation("=> VsoMonitor.AddBuilds");
            buildStates.Add(outputDevice, buildIds.ToDictionary(b => b, b => BuildState.Unknown));
        }

        public void BeginMonitoring()
        {
            Trace.TraceInformation("=> VsoMonitor.BeginMonitoring");
            UpdateBuildStates();
            Trace.TraceInformation("Starting polling timer");
            timer.Start();
        }

        public void EndMonitoring()
        {
            Trace.TraceInformation("=> TeamCityModule.EndMonitoring");
            timer.Stop();
        }

        void PollStatus(object sender, ElapsedEventArgs e)
        {
            Trace.TraceInformation("=> VsoMonitor.PollStatus");
            if (!Monitor.TryEnter(pollingLock, lockingInterval))
            {
                Trace.TraceWarning("Could not acquire polling lock - skipping attempt");
                return;
            }

            try
            {
                UpdateBuildStates();
            }
            finally
            {
                Monitor.Exit(pollingLock);
            }
        }

        private void UpdateBuildStates()
        {
            Trace.TraceInformation("=> TeamCityModule.UpdateBuildStates");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (!outputDevice.IsAvailable)
                    continue;

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

        private IEnumerable<BuildInfo> GetNewBuildStates(IEnumerable<string> buildIds)
        {
            Trace.TraceInformation("=> TeamCityModule.GetNewBuildStates");
            foreach (var buildId in buildIds)
            {
                var resultXml = vsoConnection.GetBuild(buildId);

                var resultRoot = XElement.Parse(resultXml);
                var buildXml = resultRoot.Elements("build").SingleOrDefault(); // Need to check for null in case build no longer exists
                if (buildXml != null)
                {
                    var build = new
                                    {
                                        IsRunning = buildXml.Attribute("running") != null,
                                        Status = buildXml.GetAttributeString("status"),
                                        TimeStamp = buildXml.GetAttributeDateTime("startDate", vsoDateFormat)
                                    };

                    var state = ConvertState(build.Status);

                    if (build.IsRunning && state == BuildState.Succeeded)
                        state = BuildState.Running;

                    yield return new BuildInfo { BuildId = buildId, State = state, TimeStamp = build.TimeStamp};
                }
                else
                    Trace.TraceWarning("Build '{0}' invalid", buildId);
            }
        }

        private BuildState ConvertState(string state)
        {
            BuildState convertedState;
            if (stateMap.TryGetValue(state, out convertedState))
                return convertedState;

            Trace.TraceInformation("State '{0}' is not supported.", state);
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
