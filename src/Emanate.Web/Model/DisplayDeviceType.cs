using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Emanate.Web.Model
{
    public class DisplayDeviceType
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Icon { get; set; }

        public ICollection<DisplayDeviceProfile> Profiles { get; set; }
    }
}
