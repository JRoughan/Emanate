using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Xml.Linq;
using Timer = System.Timers.Timer;

namespace Emanate.Core.Input.TeamCity
{
    public class TeamCityMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private const string m_teamCityDateFormat = "yyyyMMdd'T'HHmmsszzz";
        private readonly TimeSpan lockingInterval;
        private readonly ITeamCityConnection teamCityConnection;
        private readonly TeamCityConfiguration configuration;
        private bool isInitialized;
        private readonly Timer timer;
        private Dictionary<string, BuildState> buildStates;
        private Dictionary<string, string> lastRunningBuildIds = new Dictionary<string,string>();
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
            this.configuration = configuration;

            var pollingInterval = configuration.PollingInterval * 1000;
            if (pollingInterval < 1)
                pollingInterval = 30000; // default to 30 seconds

            lockingInterval = TimeSpan.FromSeconds(pollingInterval / 2.0);

            timer = new Timer(pollingInterval);
            timer.Elapsed += PollTeamCityStatus;
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public IEnumerable<string> MonitoredBuilds
        {
            get { return buildStates.Keys; }
        }

        public BuildState CurrentState { get; private set; }


        private static bool IsWildcardMatch(string project, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(project, regexPattern);
        }

        private IEnumerable<string> GetBuildIds(string builds)
        {
            var projectXml = teamCityConnection.GetProjects();
            var projectRoot = XElement.Parse(projectXml);

            var configParts = builds.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var projectValues = configParts.Select(p =>
                                                      {
                                                          var parts = p.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                                          return new { Project = parts[0].Trim(), Build = parts[1].Trim() };
                                                      });

            var projectNames = projectValues.Select(pv => pv.Project).Distinct();

            var projectElements =
                from projectElement in projectRoot.Elements("project")
                let p = projectNames.FirstOrDefault(p => IsWildcardMatch(projectElement.Attribute("name").Value, p))
                where p != null
                select new { Name = p, Id = projectElement.Attribute("id").Value };

            foreach (var projectElement in projectElements)
            {
                var projectName = projectElement.Name;
                var projectId = projectElement.Id;

                var buildNames = projectValues.Where(pv => pv.Project == projectName).Select(pv => pv.Build);

                var buildXml = teamCityConnection.GetProject(projectId);
                var builtRoot = XElement.Parse(buildXml);

                var buildElements = from buildTypesElement in builtRoot.Elements("buildTypes")
                                    from buildElement in buildTypesElement.Elements("buildType")
                                    let b = buildNames.FirstOrDefault(b => IsWildcardMatch(buildElement.Attribute("name").Value, b))
                                    where b != null
                                    select buildElement;


                foreach (var buildElement in buildElements)
                {
                    yield return buildElement.Attribute("id").Value;
                }
            }
        }

        public void BeginMonitoring()
        {
            if (!isInitialized)
            {
                var monitoredBuilds = GetBuildIds(configuration.BuildsToMonitor);
                buildStates = monitoredBuilds.ToDictionary(x => x, x => BuildState.Unknown);
                isInitialized = true;
            }

            UpdateBuildStates();
            // Run twice on startup, because if any builds are running we need two passes to get last and current state
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
            foreach (var buildConfigId in buildStates.Keys)
            {
                var resultXml = teamCityConnection.GetBuilds(buildConfigId, 2);

                var resultRoot = XElement.Parse(resultXml);
                var builds = resultRoot.Elements("build");
                var buildXml = builds.FirstOrDefault(); // Need to check for null in case build no longer exists
                if (buildXml != null)
                {
                    if (buildXml.Attribute("running") != null)
                    {
                        string currentRunningBuildId = buildXml.Attribute("id").Value;
                        if (!lastRunningBuildIds.ContainsKey(buildConfigId) 
                            || (!String.IsNullOrEmpty(lastRunningBuildIds[buildConfigId]) && lastRunningBuildIds[buildConfigId] != currentRunningBuildId))
                        {
                            // We missed the point where the last build ended, so find that build if possible
                            var list = builds.ToList();
                            if (list.Count > 1)
                            {
                                var previousBuildXML = list[1];
                                if (previousBuildXML != null)
                                    buildXml = previousBuildXML;
                            }
                            lastRunningBuildIds[buildConfigId] = "";
                        }
                        else
                        {
                            lastRunningBuildIds[buildConfigId] = currentRunningBuildId;
                        }
                    }
                    else
                    {
                        lastRunningBuildIds[buildConfigId] = "";
                    }
                    
                    var build = new
                                    {
                                        IsRunning = buildXml.Attribute("running") != null,
                                        Status = buildXml.Attribute("status").Value,
                                        TimeStamp = DateTimeOffset.ParseExact(buildXml.Attribute("startDate").Value, m_teamCityDateFormat, CultureInfo.InvariantCulture)
                                    };

                    var state = ConvertState(build.Status);

                    if (build.IsRunning && state == BuildState.Succeeded)
                        state = BuildState.Running;

                    yield return new BuildInfo { BuildId = buildConfigId, State = state, TimeStamp = build.TimeStamp};
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
