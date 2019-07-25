﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Emanate.Model
{
    public class SourceDevice : Entity
    {
        [Required]
        public string Name { get; set; }

        public Guid? TypeId { get; set; }
        [ForeignKey(nameof(TypeId))]
        public SourceDeviceType Type { get; set; }

        public Guid? ProfileId { get; set; }
        [ForeignKey(nameof(ProfileId))]
        public SourceDeviceProfile Profile { get; set; }
    }
}
