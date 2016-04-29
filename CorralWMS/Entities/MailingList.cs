using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;

namespace CorralWMS.Entities
{
    public class MailingList
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [NotMapped]
        public string List
        {
            get
            {
                var bldr = new StringBuilder();
                foreach (var user in Recipients)
                {
                    bldr.Append(user.Email).Append(",");
                }
                return bldr.ToString();
            }
        }
        public virtual ICollection<User> Recipients { get; set; }
    }
}