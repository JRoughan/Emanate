using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emanate.Model
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
