using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.Vso.Configuration;
using Timer = System.Timers.Timer;

namespace Emanate.Vso
{
    public class VsoMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private readonly TimeSpan lockingInterval;
        private readonly IVsoConnection vsoConnection;
        private readonly Timer timer;
        private readonly Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>>();

        private static readonly string InProgressStatus = "inProgress";
        private static readonly string SucceededStatus = "succeeded";
        
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

        public void AddBuilds(IOutputDevice outputDevice, IEnumerable<InputInfo> inputs)
        {
            Trace.TraceInformation("=> VsoMonitor.AddBuilds");
            var buildIds = inputs.Select(i => new BuildKey(i.ProjectId, i.Id)).ToList();
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
            Trace.TraceInformation("=> VsoMonitor.EndMonitoring");
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
            Trace.TraceInformation("=> VsoMonitor.UpdateBuildStates");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (!outputDevice.IsAvailable)
                    continue;

                var buildInfos = GetNewBuildStates(output.Value.Keys);
                var newStates = buildInfos.ToList();

                var newState = BuildState.Unknown;
                var timeStamp = DateTime.Now;

                if (newStates.Any())
                {
                    var states = buildStates[outputDevice];
                    foreach (var buildState in newStates)
                        states[buildState.BuildKey] = buildState.State;

                    newState = (BuildState)newStates.Max(s => (int)s.State);

                    timeStamp = newStates.Where(s => s.TimeStamp.HasValue).Max(s => s.TimeStamp.Value);
                }

                var oldState = CurrentState;
                CurrentState = newState;
                outputDevice.UpdateStatus(newState, timeStamp);
            }
        }

        private IEnumerable<BuildInfo> GetNewBuildStates(IEnumerable<BuildKey> buildKeys)
        {
            Trace.TraceInformation("=> VsoMonitor.GetNewBuildStates");
            var buildInfos = new List<BuildInfo>();
            foreach (var buildKey in buildKeys)
            {
                var tfsBuild = vsoConnection.GetBuild(buildKey.ProjectId, int.Parse(buildKey.BuildId))["value"][0];
                if (tfsBuild != null)
                {
                    var startTime = DateTime.Parse((string) tfsBuild["startTime"]);
                    var state = ConvertState(tfsBuild);

                    buildInfos.Add(new BuildInfo { BuildKey = buildKey, State = state, TimeStamp = startTime });
                }
                else
                    Trace.TraceWarning("Build '{0}' invalid", buildKey);
            }
            return buildInfos;
        }

        private BuildState ConvertState(dynamic build)
        {
            string result = build["result"];
            if (string.IsNullOrWhiteSpace(result) && build["status"] == InProgressStatus)
                return BuildState.Running;

            return !string.IsNullOrWhiteSpace(result) && result == SucceededStatus ? BuildState.Succeeded : BuildState.Failed;
        }

        [DebuggerDisplay("{BuildKey} - {State}")]
        class BuildInfo
        {
            public BuildKey BuildKey { get; set; }
            public BuildState State { get; set; }
            public DateTime? TimeStamp { get; set; }
        }
    }

    [DebuggerDisplay("{ProjectId}:{BuildId}")]
    public class BuildKey : IEquatable<BuildKey>
    {
        public BuildKey(Guid projectId, string buildId)
        {
            ProjectId = projectId;
            BuildId = buildId;
        }

        public Guid ProjectId { get; }
        public string BuildId { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((BuildKey)obj);
        }

        public bool Equals(BuildKey other)
        {
            if (other == null)
                return false;

            return ProjectId.Equals(other.ProjectId) && BuildId.Equals(other.BuildId);
        }

        public override int GetHashCode()
        {
            return (ProjectId.ToString() + BuildId).GetHashCode();
        }
    }
}
