using System;
using System.ComponentModel.DataAnnotations;

namespace Emanate.Web.Model
{
    public class DisplayDeviceProfile : Entity
    {
        [Required]
        public string Name { get; set; }
    }
}
