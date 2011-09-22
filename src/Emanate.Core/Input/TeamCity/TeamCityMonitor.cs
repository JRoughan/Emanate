using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Xml.Linq;

namespace Emanate.Core.Input.TeamCity
{
    public class TeamCityMonitor : IBuildMonitor
    {
        private readonly TeamCityConfiguration config;
        private bool isInitialized;
        private readonly Timer timer;
        private TeamCityConnection connection;
        private Dictionary<string, BuildState> monitoredBuilds;
        private readonly Dictionary<string, BuildState> stateMap = new Dictionary<string, BuildState>
                                                              {
                                                                  { "RUNNING", BuildState.Running },
                                                                  { "FAILURE", BuildState.Failed },
                                                                  { "SUCCESS", BuildState.Succeeded },
                                                              };

        

        public TeamCityMonitor(IConfigurationGenerator configuration)
        {
            config = configuration.Generate<TeamCityConfiguration>();

            var pollingInterval = config.PollingInterval * 1000;
            timer = new Timer(pollingInterval);
            timer.Elapsed += PollTeamCityStatus;
        }

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public BuildState CurrentState { get; private set; }

        // TODO: Allow more than one build per project (i.e. duplicate keys)
        private IEnumerable<string> GetBuildIds(string builds)
        {
            var uri = connection.CreateUri("/httpAuth/app/rest/projects");
            var projectXml = connection.Request(uri);
            var projectRoot = XElement.Parse(projectXml);

            var configParts = builds.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var projectValues = configParts.Select(p =>
                                                      {
                                                          var parts = p.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                                          return new { Name = parts[0], Id = parts[1] };
                                                      });

            var projectNames = projectValues.Select(pv => pv.Name).Distinct();

            var projectElements =
                from projectElement in projectRoot.Elements("project")
                where projectNames.Contains(projectElement.Attribute("name").Value)
                select projectElement;

            // TODO: Optimize to only issue builds request once per project
            foreach (var projectElement in projectElements)
            {
                var projectName = projectElement.Attribute("name").Value;
                var projectId = projectElement.Attribute("id").Value;

                var buildNames = projectValues.Where(pv => pv.Name == projectName).Select(pv => pv.Id);

                var buildUri = connection.CreateUri(string.Format("/httpAuth/app/rest/projects/id:{0}", projectId));
                var buildXml = connection.Request(buildUri);
                var builtRoot = XElement.Parse(buildXml);

                var buildElements = from buildTypesElement in builtRoot.Elements("buildTypes")
                                    from buildElement in buildTypesElement.Elements("buildType")
                                    where buildNames.Contains(buildElement.Attribute("name").Value)
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
                connection = new TeamCityConnection(config);
                monitoredBuilds = GetBuildIds(config.BuildsToMonitor).ToDictionary(x => x, x => BuildState.Unknown);
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
            UpdateBuildStates();
        }

        private void UpdateBuildStates()
        {
            var newStates = GetNewBuildStates();

            var newState = BuildState.Unknown;
            foreach (var buildState in newStates.ToList())
            {
                monitoredBuilds[buildState.BuildId] = buildState.State;
                if ((int)buildState.State > (int)newState)
                    newState = buildState.State;
            }

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
            var runningBuilds = GetRunningBuildIds().ToList();

            foreach (var buildId in monitoredBuilds.Keys)
            {
                if (runningBuilds.Contains(buildId))
                    yield return new BuildInfo { BuildId = buildId, State = BuildState.Running };

                var resultUri = connection.CreateUri(string.Format("httpAuth/app/rest/buildTypes/id:{0}/builds", buildId));
                var resultXml = connection.Request(resultUri);

                var resultRoot = XElement.Parse(resultXml);
                var states = from resultElement in resultRoot.Elements("build")
                             select
                                 new
                                     {
                                         Id = resultElement.Attribute("id").Value,
                                         Status = resultElement.Attribute("status").Value
                                     };

                var state = states.OrderByDescending(s => s.Id).Select(s => s.Status).First();
                yield return new BuildInfo { BuildId = buildId, State = stateMap[state] };
            }
        }

        private IEnumerable<string> GetRunningBuildIds()
        {
            var runningUri = connection.CreateUri("httpAuth/app/rest/builds?locator=running:true");
            var runningXml = connection.Request(runningUri);

            var runningRoot = XElement.Parse(runningXml);

            return //from buildTypesElement in runningRoot.Elements("builds")
                   from buildElement in runningRoot.Elements("build")
                   select buildElement.Attribute("buildTypeId").Value;
        }

        class BuildInfo
        {
            public string BuildId { get; set; }
            public BuildState State { get; set; }
        }
    }
}
