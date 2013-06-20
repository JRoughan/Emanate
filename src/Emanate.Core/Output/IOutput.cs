using System;
using Emanate.Core.Input;

namespace Emanate.Core.Output
{
    public interface IOutput
    {
        void UpdateStatus(BuildState oldState, BuildState newState, DateTimeOffset timeStamp);
    }
}