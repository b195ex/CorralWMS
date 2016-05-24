using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class CycleCount
    {
        [Key]
        [Column(Order=1)]
        public int BinEntry { get; set; }
        [Key]
        [Column(Order=2)]
        public DateTime Date { get; set; }
        [MaxLength(228)]
        public string BinCode { get; set; }
        public int UserId { get; set; }
        public bool Closed { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}