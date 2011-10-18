using System;
using System.Collections.Generic;
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
        private readonly TimeSpan lockingInterval;
        private readonly ITeamCityConnection teamCityConnection;
        private readonly TeamCityConfiguration configuration;
        private bool isInitialized;
        private readonly Timer timer;
        private Dictionary<string, BuildState> buildStates;
        private readonly Dictionary<string, BuildState> stateMap = new Dictionary<string, BuildState>
                                                              {
                                                                  { "RUNNING", BuildState.Running },
                                                                  { "ERROR", BuildState.Error },
                                                                  { "FAILURE", BuildState.Failed },
                                                                  { "SUCCESS", BuildState.Succeeded },
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

        public IEnumerable<string> MonitoredProjects
        {
            get { return buildStates.Keys; }
        }

        public BuildState CurrentState { get; private set; }


        private static bool IsWildcardMatch(string project, string pattern)
        {
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return Regex.IsMatch(project, regexPattern);
        }

        // TODO: Allow more than one build per project (i.e. duplicate keys)
        private IEnumerable<string> GetBuildIds(string builds)
        {
            var projectXml = teamCityConnection.GetProjects();
            var projectRoot = XElement.Parse(projectXml);

            var configParts = builds.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var projectValues = configParts.Select(p =>
                                                      {
                                                          var parts = p.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                                          return new { Project = parts[0], Build = parts[1] };
                                                      });

            var projectNames = projectValues.Select(pv => pv.Project).Distinct();

            var projectElements =
                from projectElement in projectRoot.Elements("project")
                let p = projectNames.FirstOrDefault(p => IsWildcardMatch(projectElement.Attribute("name").Value, p))
                where p != null
                select new { Name = p, Id = projectElement.Attribute("id").Value };

            // TODO: Optimize to only issue builds request once per project
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

            foreach (var buildState in newStates)
                buildStates[buildState.BuildId] = buildState.State;

            var newState = (BuildState)newStates.Max(s => (int)s.State);

            var oldState = CurrentState;
            CurrentState = newState;
            OnStatusChanged(oldState, newState);
        }

        private void OnStatusChanged(BuildState oldState, BuildState newState)
        {
            var handler = StatusChanged;
            if (handler != null)
                handler(this, new StatusChangedEventArgs(oldState, newState));
        }

        private IEnumerable<BuildInfo> GetNewBuildStates()
        {
            foreach (var buildId in buildStates.Keys)
            {
                var resultXml = teamCityConnection.GetBuild(buildId);

                var resultRoot = XElement.Parse(resultXml);
                var buildXml = resultRoot.Elements("build").Single();
                var build = new
                {
                    IsRunning = buildXml.Attribute("running") != null,
                    Status = buildXml.Attribute("status").Value
                };
                
                var state = ConvertState(build.Status);

                if (build.IsRunning && state == BuildState.Succeeded)
                    state = BuildState.Running;

                yield return new BuildInfo { BuildId = buildId, State = state };
            }
        }

        private BuildState ConvertState(string state)
        {
            BuildState convertedState;
            if (stateMap.TryGetValue(state, out convertedState))
                return convertedState;

            throw new NotSupportedException(string.Format("State '{0}' is not supported.", state));
        }

        class BuildInfo
        {
            public string BuildId { get; set; }
            public BuildState State { get; set; }
        }
    }
}
