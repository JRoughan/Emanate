using System;
using System.ComponentModel.DataAnnotations;

namespace Emanate.Web.Model
{
    public class DisplayDevice
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
