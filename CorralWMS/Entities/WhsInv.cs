using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class WhsInv
    {
        public int Id { get; set; }
        [Required]
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public int DocEntry { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<LocInv> Locations { get; set; }
    }
}