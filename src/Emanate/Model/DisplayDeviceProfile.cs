using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emanate.Model
{
    public class DisplayDeviceProfile : Entity
    {
        [Required]
        public string Name { get; set; }

        public Guid TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public DisplayDeviceType Type { get; set; }
    }
}
