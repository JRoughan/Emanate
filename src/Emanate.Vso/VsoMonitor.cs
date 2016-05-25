﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Emanate.Core;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Serilog;
using Timer = System.Timers.Timer;

namespace Emanate.Vso
{
    public class VsoMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private TimeSpan lockingInterval;
        private IVsoConnection vsoConnection;
        private Timer timer;
        private readonly Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>>();

        private static readonly string InProgressStatus = "inProgress";
        private static readonly string SucceededStatus = "succeeded";

        public void SetDevice(IDevice device)
        {
            var vsoDevice = (VsoDevice)device;
            vsoConnection = new VsoConnection(vsoDevice);

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
            Log.Information("=> VsoMonitor.AddBuilds");
            var buildIds = inputs.Select(i =>
            {
                var parts = i.Split(':');
                return new BuildKey(new Guid(parts[0]), parts[1]);
            });
            buildStates.Add(outputDevice, buildIds.ToDictionary(b => b, b => BuildState.Unknown));
        }

        public void BeginMonitoring()
        {
            Log.Information("=> VsoMonitor.BeginMonitoring");
            UpdateBuildStates();
            Log.Information("Starting polling timer");
            timer.Start();
        }

        public void EndMonitoring()
        {
            Log.Information("=> VsoMonitor.EndMonitoring");
            timer.Stop();
        }

        void PollStatus(object sender, ElapsedEventArgs e)
        {
            Log.Information("=> VsoMonitor.PollStatus");
            if (!Monitor.TryEnter(pollingLock, lockingInterval))
            {
                Log.Warning("Could not acquire polling lock - skipping attempt");
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

        private async void UpdateBuildStates()
        {
            Log.Information("=> VsoMonitor.UpdateBuildStates");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (!outputDevice.IsAvailable)
                    continue;

                var buildInfos = await GetNewBuildStates(output.Value.Keys);
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

                var asoldState = CurrentState;
                CurrentState = newState;
                outputDevice.UpdateStatus(newState, timeStamp);
            }
        }

        private async Task<IEnumerable<BuildInfo>> GetNewBuildStates(IEnumerable<BuildKey> buildKeys)
        {
            Log.Information("=> VsoMonitor.GetNewBuildStates");
            var buildInfos = new List<BuildInfo>();
            foreach (var buildKey in buildKeys)
            {
                var rawBuild = await vsoConnection.GetBuild(buildKey.ProjectId, int.Parse(buildKey.BuildId));
                var tfsBuild = rawBuild["value"][0];
                if (tfsBuild != null)
                {
                    var rawDate = ((string)tfsBuild["startTime"]).Trim('{', '}');
                    var startTime = DateTime.ParseExact(rawDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    var state = ConvertState(tfsBuild);

                    buildInfos.Add(new BuildInfo { BuildKey = buildKey, State = state, TimeStamp = startTime });
                }
                else
                    Log.Warning("Build '{0}' invalid", buildKey);
            }
            return buildInfos;
        }

        private BuildState ConvertState(dynamic build)
        {
            string result = build["result"]?.Value;
            if (string.IsNullOrWhiteSpace(result) && build["status"]?.Value == InProgressStatus)
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
