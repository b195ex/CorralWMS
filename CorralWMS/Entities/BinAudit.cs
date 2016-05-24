using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class BinAudit
    {
        public int Id { get; set; }
        public int BinEntry { get; set; }
        [Required]
        public string BinCode { get; set; }
        [Required]
        public string WhsCode { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}