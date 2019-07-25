using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emanate.Web.Model
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }


    public class DisplayConfiguration : Entity
    {
        [ForeignKey(nameof(DisplayDevice))]
        public Guid DisplayDeviceId { get; set; }
        public DisplayDevice DisplayDevice { get; set; }

        public List<SourceGroup> SourceGroups { get; set; }
    }

    public class SourceGroup : Entity
    {
        [ForeignKey(nameof(SourceDevice))]
        public Guid SourceDeviceId { get; set; }
        public SourceDevice SourceDevice { get; set; }

        public List<SourceConfiguration> SourceConfiguration { get; set; }
    }

    public class SourceConfiguration : Entity
    {
        public string Builds { get; set; }
    }
}
