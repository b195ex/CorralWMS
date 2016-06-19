using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class OrdrDtl
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Order")]
        public int DocEntry { get; set; }
        [Key]
        [Column(Order = 2)]
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public string WhsCode { get; set; }
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}