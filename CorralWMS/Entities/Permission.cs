using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Permission
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese una Descripción")]
        [Display(Name = "Descripción")]
        [Index(IsUnique = true)]
        [StringLength(128)]
        public string Description { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}