using System;

namespace Emanate.Core.Output
{
    public class InputInfo
    {
        public InputInfo(string source, Guid inputDeviceId, string inputId)
        {
            Source = source;
            InputDeviceId = inputDeviceId;
            Id = inputId;
        }

        public string Source { get; private set; }
        public Guid InputDeviceId { get; private set; }
        public string Id { get; private set; }
    }
}