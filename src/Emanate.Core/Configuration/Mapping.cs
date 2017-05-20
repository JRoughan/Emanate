using System;
using System.Collections.Generic;
using System.Linq;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class Mapping
    {
        public List<InputGroup> InputGroups { get; } = new List<InputGroup>();
        public Guid OutputDeviceId { get; set; }

        public InputGroup GetOrAddInputGroup(Guid deviceId)
        {
            var inputGroup = InputGroups.SingleOrDefault(ig => ig.InputDeviceId == deviceId);
            if (inputGroup == null)
            {
                InputGroups.Add(new InputGroup { InputDeviceId = deviceId });
            }
            return inputGroup;
        }
    }
}