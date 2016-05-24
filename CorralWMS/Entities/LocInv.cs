using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class LocInv
    {
        [Key]
        [Column(Order=1)]
        public int WhsinvId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int BinAbs { get; set; }
        public string BinCode { get; set; }
        public int? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual WhsInv WhsInv { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}