using System;
using System.Collections.Generic;

namespace Emanate.Core.Output
{
    public class InputGroup
    {
        public Guid InputDeviceId { get; set; }
        public List<string> Inputs { get; } = new List<string>();
    }
}