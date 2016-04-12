using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ingrese un Descripción")]
        [Display(Name = "Nombre")]
        [StringLength(128)]
        [Index(IsUnique = true)]
        public string RoleName { get; set; }
        
        [Display(Name="Descripción")]
        public string Description { get; set; }

        public ICollection<Permission> Permissions { get; set; }

        public ICollection<User> Users { get; set; }
    }
}