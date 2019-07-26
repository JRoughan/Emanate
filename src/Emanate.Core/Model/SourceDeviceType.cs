using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emanate.Model
{
    public class SourceDeviceType : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Icon { get; set; }

        public ICollection<SourceDeviceProfile> Profiles { get; set; }
    }
}
