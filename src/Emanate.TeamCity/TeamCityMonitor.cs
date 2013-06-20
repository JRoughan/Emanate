using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.TeamCity.Configuration;
using Timer = System.Timers.Timer;

namespace Emanate.TeamCity
{
    public class TeamCityMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private const string m_teamCityDateFormat = "yyyyMMdd'T'HHmmsszzz";
        private readonly TimeSpan lockingInterval;
        private readonly ITeamCityConnection teamCityConnection;
        private bool isInitialized;
        private readonly Timer timer;
        private readonly Dictionary<string, BuildState> buildStates = new Dictionary<string, BuildState>();
        private readonly Dictionary<string, BuildState> stateMap = new Dictionary<string, BuildState>
                                                              {
                                                                  { "UNKNOWN", BuildState.Unknown },
                                                                  { "RUNNING", BuildState.Running },
                                                                  { "ERROR", BuildState.Error },
                                                                  { "FAILURE", BuildState.Failed },
                                                                  { "SUCCESS", BuildState.Succeeded }
                                                              };


        public TeamCityMonitor(ITeamCityConnection teamCityConnection, TeamCityConfiguration configuration)
        {
            this.teamCityConnection = teamCityConnection;

            var pollingInterval = configuration.PollingInterval * 1000;
            if (pollingInterval < 1)
                pollingInterval = 30000; // default to 30 seconds

            lockingInterval = TimeSpan.FromSeconds(pollingInterval / 2.0);

            timer = new Timer(pollingInterval);
            timer.Elapsed += PollTeamCityStatus;
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public BuildState CurrentState { get; private set; }

        public void AddBuilds(IEnumerable<InputInfo> inputs)
        {
            foreach (var info in inputs)
                AddBuild(info);
        }

        public void AddBuild(InputInfo input)
        {
            buildStates.Add(input.Id, BuildState.Unknown);
        }

        public void BeginMonitoring()
        {
            UpdateBuildStates();
            timer.Start();
        }

        public void EndMonitoring()
        {
            timer.Stop();
        }

        void PollTeamCityStatus(object sender, ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(pollingLock, lockingInterval))
                return;

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
            var newStates = GetNewBuildStates().ToList();

            var newState = BuildState.Unknown;
            var timeStamp = DateTimeOffset.Now;

            if (newStates.Any())
            {
                foreach (var buildState in newStates)
                    buildStates[buildState.BuildId] = buildState.State;

                newState = (BuildState)newStates.Max(s => (int)s.State);

                timeStamp = newStates.Max(s => s.TimeStamp);
            }

            var oldState = CurrentState;
            CurrentState = newState;
            OnStatusChanged(oldState, newState, timeStamp);
        }

        private void OnStatusChanged(BuildState oldState, BuildState newState, DateTimeOffset timeStamp)
        {
            var handler = StatusChanged;
            if (handler != null)
                handler(this, new StatusChangedEventArgs(oldState, newState, timeStamp));
        }

        private IEnumerable<BuildInfo> GetNewBuildStates()
        {
            foreach (var buildId in buildStates.Keys)
            {
                var resultXml = teamCityConnection.GetBuild(buildId);

                var resultRoot = XElement.Parse(resultXml);
                var buildXml = resultRoot.Elements("build").SingleOrDefault(); // Need to check for null in case build no longer exists
                if (buildXml != null)
                {
                    var build = new
                                    {
                                        IsRunning = buildXml.Attribute("running") != null,
                                        Status = buildXml.Attribute("status").Value,
                                        TimeStamp = DateTimeOffset.ParseExact(buildXml.Attribute("startDate").Value, m_teamCityDateFormat, CultureInfo.InvariantCulture)
                                    };

                    var state = ConvertState(build.Status);

                    if (build.IsRunning && state == BuildState.Succeeded)
                        state = BuildState.Running;

                    yield return new BuildInfo { BuildId = buildId, State = state, TimeStamp = build.TimeStamp};
                }
            }
        }

        private BuildState ConvertState(string state)
        {
            BuildState convertedState;
            if (stateMap.TryGetValue(state, out convertedState))
                return convertedState;

            throw new NotSupportedException(string.Format("State '{0}' is not supported.", state));
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
