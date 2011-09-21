using System;

namespace Emanate.Core.Input
{
    public interface IBuildMonitor
    {
        void BeginMonitoring();
        void EndMonitoring();

        event EventHandler<StatusChangedEventArgs> StatusChanged;
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public BuildState OldState { get; private set; }
        public BuildState NewState { get; private set; }

        public StatusChangedEventArgs(BuildState oldState, BuildState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}