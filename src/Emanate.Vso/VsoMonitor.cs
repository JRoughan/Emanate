using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class VsoMonitorFactory : IBuildMonitorFactory
    {
        private readonly Func<IDevice, VsoMonitor> monitorFactory;

        public VsoMonitorFactory(Func<IDevice, VsoMonitor> monitorFactory)
        {
            this.monitorFactory = monitorFactory;
        }

        public IBuildMonitor Create(IDevice device)
        {
            return monitorFactory(device);
        }
    }

    public class VsoMonitor : IBuildMonitor
    {
        private readonly object pollingLock = new object();
        private readonly TimeSpan lockingInterval;
        private readonly IVsoConnection vsoConnection;
        private readonly Timer timer;
        private readonly Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>>();

        private static readonly string InProgressStatus = "inProgress";
        private static readonly string SucceededStatus = "succeeded";

        public VsoMonitor(IDevice device)
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


        public void AddBuilds(IOutputDevice outputDevice, IEnumerable<string> inputs)
        {
            Log.Information("=> VsoMonitor.AddBuilds");
            var buildIds = inputs.Select(i =>
            {
                var parts = i.Split(':');
                return new BuildKey(Guid.Parse(parts[0]), int.Parse(parts[1]));
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

        private void PollStatus(object sender, ElapsedEventArgs e)
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

        private async void UpdateBuildStates()
        {
            Log.Information("=> VsoMonitor.UpdateBuildStates");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (!outputDevice.IsAvailable)
                {
                    Log.Warning($"Output device '{outputDevice.Name}' unavailable - skipping update");
                    continue;
                }

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

        private async Task<IEnumerable<BuildInfo>> GetNewBuildStates(IEnumerable<BuildKey> buildKeys)
        {
            Log.Information("=> VsoMonitor.GetNewBuildStates");
            var buildInfos = new List<BuildInfo>();
            foreach (var buildKey in buildKeys)
            {
                try
                {
                    var build = await vsoConnection.GetBuild(buildKey.ProjectId, buildKey.BuildId);
                    var startTime = build.StartTime;
                    var state = ConvertState(build);

                    Log.Information($"Adding build state for {buildKey.ProjectId}:{buildKey.BuildId}");
                    buildInfos.Add(new BuildInfo { BuildKey = buildKey, State = state, TimeStamp = startTime });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to get build state for {buildKey.ProjectId}:{buildKey.BuildId}");
                    buildInfos.Add(new BuildInfo { BuildKey = buildKey, State = BuildState.Error, TimeStamp = DateTime.Now });
                }
            }
            return buildInfos;
        }

        private BuildState ConvertState(Build build)
        {
            string result = build.Result;
            if (string.IsNullOrWhiteSpace(result) && build.Status == InProgressStatus)
                return BuildState.Running;

            return !string.IsNullOrWhiteSpace(result) && result == SucceededStatus ? BuildState.Succeeded : BuildState.Failed;
        }

        [DebuggerDisplay("{BuildKey} - {State}")]
        private class BuildInfo
        {
            public BuildKey BuildKey { get; set; }
            public BuildState State { get; set; }
            public DateTime? TimeStamp { get; set; }
        }
    }
}
