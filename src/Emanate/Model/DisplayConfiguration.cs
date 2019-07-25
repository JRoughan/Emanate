using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emanate.Model
{
    public class DisplayConfiguration : Entity
    {
        [ForeignKey(nameof(DisplayDevice))]
        public Guid DisplayDeviceId { get; set; }
        public DisplayDevice DisplayDevice { get; set; }

        public List<SourceGroup> SourceGroups { get; set; }
    }
}