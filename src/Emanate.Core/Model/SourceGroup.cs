using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emanate.Model
{
    public class SourceGroup : Entity
    {
        [ForeignKey(nameof(SourceDevice))]
        public Guid SourceDeviceId { get; set; }
        public SourceDevice SourceDevice { get; set; }

        public SourceConfiguration SourceConfiguration { get; set; }
    }
}