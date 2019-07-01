using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Emanate.Web.Model
{
    public class DisplayDevice
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Guid? TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public DisplayDeviceType Type { get; set; }

        public Guid? ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public DisplayDeviceProfile Profile { get; set; }
    }
}
