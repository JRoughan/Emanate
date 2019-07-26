using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Emanate.Core.Input;
using Emanate.Core.Output;
using Serilog;

namespace Emanate.Vso
{
    public class VsoMonitor : IBuildMonitor
    { 
        private readonly TimeSpan delayInterval;
        private readonly IVsoConnection vsoConnection;

        private readonly Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>> buildStates = new Dictionary<IOutputDevice, Dictionary<BuildKey, BuildState>>();

        private const string inProgressStatus = "inProgress";
        private const string succeededStatus = "succeeded";

        private bool isMonitoring;
        private readonly string name;

        public VsoMonitor(VsoDevice device, VsoConnection.Factory connectionFactory)
        {
            name = device.Name;
            vsoConnection = connectionFactory(device);

            var pollingInterval = device.PollingInterval > 0 ? device.PollingInterval : 30; // default to 30 seconds
            delayInterval = TimeSpan.FromSeconds(pollingInterval);
        }

        public void AddBuilds(IOutputDevice outputDevice, IEnumerable<string> inputs)
        {
            Log.Information($"=> VsoMonitor[{name}] - AddBuilds({outputDevice.Name}, {inputs})");
            var buildIds = inputs.Select(i =>
            {
                var parts = i.Split(':');
                return new BuildKey(Guid.Parse(parts[0]), int.Parse(parts[1]));
            });
            buildStates.Add(outputDevice, buildIds.ToDictionary(b => b, b => BuildState.Unknown));
        }

        public Task BeginMonitoring()
        {
            Log.Information($"=> VsoMonitor[{name}] - BeginMonitoring()");
            isMonitoring = true;
            return Task.Run(() => { UpdateLoop(); });
        }

        public void EndMonitoring()
        {
            Log.Information($"=> VsoMonitor[{name}] - EndMonitoring()");
            isMonitoring = false;
        }

        private async void UpdateLoop()
        {
            Log.Information($"=> VsoMonitor[{name}] - UpdateLoop()");
            while (isMonitoring)
            {
                try
                {
                    await UpdateBuildStates();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"VsoMonitor[{name}] - Could not update build states");
                    try
                    {
                        DisplayErrorOnAllOutputs();
                    }
                    catch (Exception ex1)
                    {
                        Log.Error(ex1, $"VsoMonitor[{name}] - Could not display error on output devices");
                        throw;
                    }
                }
                Log.Information($"VsoMonitor[{name}] - Waiting for {delayInterval} seconds");
                await Task.Delay(delayInterval);
            }
        }

        private async Task UpdateBuildStates()
        {
            Log.Information($"=> VsoMonitor[{name}] - UpdateBuildStates()");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (!outputDevice.IsAvailable)
                {
                    Log.Warning($"=> VsoMonitor[{name}] - Output device '{outputDevice.Name}' unavailable - skipping update");
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
            Log.Information($"=> VsoMonitor[{name}] - DisplayErrorOnAllOutputs()");
            foreach (var output in buildStates)
            {
                var outputDevice = output.Key;
                if (outputDevice.IsAvailable)
                    outputDevice.UpdateStatus(BuildState.Error, DateTime.Now);
            }
        }

        private async Task<IEnumerable<BuildInfo>> GetNewBuildStates(IEnumerable<BuildKey> buildKeys)
        {
            Log.Information($"=> VsoMonitor[{name}] - GetNewBuildStates()");
            var buildInfos = new List<BuildInfo>();
            foreach (var buildKey in buildKeys)
            {
                try
                {
                    var build = await vsoConnection.GetBuild(buildKey.ProjectId, buildKey.BuildId);
                    var startTime = build.StartTime;
                    var state = ConvertState(build);

                    Log.Information($"VsoMonitor[{name}] - Adding build state for {buildKey.ProjectId}:{buildKey.BuildId}");
                    buildInfos.Add(new BuildInfo { BuildKey = buildKey, State = state, TimeStamp = startTime });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"VsoMonitor[{name}] - Failed to get build state for {buildKey.ProjectId}:{buildKey.BuildId}");
                    buildInfos.Add(new BuildInfo { BuildKey = buildKey, State = BuildState.Error, TimeStamp = DateTime.Now });
                }
            }
            return buildInfos;
        }

        private BuildState ConvertState(Build build)
        {
            Log.Information($"=> VsoMonitor[{name}] - ConvertState({build})");
            string result = build.Result;
            if (string.IsNullOrWhiteSpace(result) && build.Status == inProgressStatus)
                return BuildState.Running;

            return !string.IsNullOrWhiteSpace(result) && result == succeededStatus ? BuildState.Succeeded : BuildState.Failed;
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
