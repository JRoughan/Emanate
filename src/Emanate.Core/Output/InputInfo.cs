using System;
using System.Collections.Generic;

namespace Emanate.Core.Output
{
    public class InputInfo
    {
        public InputInfo(string inputId)
        {
            Id = inputId;
        }

        public string Id { get; private set; }
    }

    public class InputGroup
    {
        public Guid InputDeviceId { get; set; }
        public List<InputInfo> Inputs { get; } = new List<InputInfo>();
    }
}