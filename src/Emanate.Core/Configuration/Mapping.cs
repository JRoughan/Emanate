using System;
using System.Collections.Generic;
using System.Linq;
using Emanate.Model;

namespace Emanate.Core.Configuration
{
    public class Mapping
    {
        public List<SourceGroup> InputGroups { get; } = new List<SourceGroup>();
        public Guid OutputDeviceId { get; set; }

        public SourceGroup GetOrAddInputGroup(Guid deviceId)
        {
            var inputGroup = InputGroups.SingleOrDefault(ig => ig.SourceDeviceId == deviceId);
            if (inputGroup == null)
            {
                InputGroups.Add(new SourceGroup { SourceDeviceId = deviceId });
            }
            return inputGroup;
        }
    }
}