using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Emanate.Web.Model
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
