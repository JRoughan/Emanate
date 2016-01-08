using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Emanate.Vso.Configuration;
using Microsoft.TeamFoundation.Build.WebApi;
using Timer = System.Timers.Timer;

namespace Emanate.Vso
{
    public class VsoMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private readonly TimeSpan lockingInterval;
        private readonly IVsoConnection vsoConnection;
        private readonly Timer timer;
        private readonly Dictionary<IOutputDevice, Dictionary<int, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<int, BuildState>>();


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
            buildStates.Add(outputDevice, buildIds.ToDictionary(int.Parse, b => BuildState.Unknown));
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

                var newStates = GetNewBuildStates(output.Value.Keys).ToList();

                var newState = BuildState.Unknown;
                var timeStamp = DateTime.Now;


                if (newStates.Any())
                {
                    var states = buildStates[outputDevice];
                    foreach (var buildState in newStates)
                        states[buildState.BuildId] = buildState.State;

                    newState = (BuildState)newStates.Max(s => (int)s.State);

                    timeStamp = newStates.Where(s => s.TimeStamp.HasValue).Max(s => s.TimeStamp.Value);
                }

                var oldState = CurrentState;
                CurrentState = newState;
                outputDevice.UpdateStatus(newState, timeStamp);
            }
        }

        private IEnumerable<BuildInfo> GetNewBuildStates(IEnumerable<int> buildIds)
        {
            Trace.TraceInformation("=> VsoMonitor.GetNewBuildStates");
            foreach (var buildId in buildIds)
            {
                var tfsBuild = vsoConnection.GetBuild(buildId).Result;
                if (tfsBuild != null)
                {
                    var build = new
                                    {
                                        IsRunning = tfsBuild.Status == BuildStatus.InProgress,
                                        Status = tfsBuild.Status,
                                        TimeStamp = tfsBuild.StartTime
                                    };

                    var state = ConvertState(tfsBuild);

                    if (build.IsRunning && state == BuildState.Succeeded)
                        state = BuildState.Running;

                    yield return new BuildInfo { BuildId = buildId, State = state, TimeStamp = build.TimeStamp};
                }
                else
                    Trace.TraceWarning("Build '{0}' invalid", buildId);
            }
        }

        private BuildState ConvertState(Build build)
        {
            if (build.Status == BuildStatus.InProgress)
                return BuildState.Running;

            return build.Result.HasValue && build.Result.Value == BuildResult.Succeeded ? BuildState.Succeeded : BuildState.Failed;
        }

        [DebuggerDisplay("{BuildId} - {State}")]
        class BuildInfo
        {
            public int BuildId { get; set; }
            public BuildState State { get; set; }
            public DateTime? TimeStamp { get; set; }
        }
    }
}
