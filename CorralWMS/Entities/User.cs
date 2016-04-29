using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Se requiere Un Nombre de Usuario.")]
        [StringLength(64)]
        [Display(Name = "Usuario")]
        [Index(IsUnique = true)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Se requiere una Contraseña.")]
        [StringLength(64)]
        [MinLength(8)]
        public string Password { get; set; }

        [StringLength(128)]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [StringLength(128)]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Ingrese Un Correo válido")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        [NotMapped]
        [Display(Name = "Nombre Completo")]
        public string Name { get { return FirstName + " " + LastName; } }

        [Display(Name = "Activo")]
        public bool Active { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<MailingList> MailingLists { get; set; }
    }
}