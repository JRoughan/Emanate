using System;
using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Input
{
    public interface IBuildMonitor
    {
        void BeginMonitoring();
        void EndMonitoring();

        event EventHandler<StatusChangedEventArgs> StatusChanged;

        void AddBuild(InputInfo input);
        void AddBuilds(IEnumerable<InputInfo> inputs);
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public BuildState OldState { get; private set; }
        public BuildState NewState { get; private set; }
        public DateTimeOffset TimeStamp { get; private set; }

        public StatusChangedEventArgs(BuildState oldState, BuildState newState, DateTimeOffset timeStamp)
        {
            OldState = oldState;
            NewState = newState;
            TimeStamp = timeStamp;
        }
    }
}