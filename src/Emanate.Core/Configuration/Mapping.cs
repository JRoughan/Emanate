using System;
using System.Collections.Generic;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class Mapping
    {
        public List<InputGroup> InputGroups { get; } = new List<InputGroup>();
        public Guid OutputDeviceId { get; set; }
    }
}