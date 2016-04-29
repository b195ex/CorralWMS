using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class FromLocation
    {
        [Key]
        [Column(Order = 1)]
        public int AbsEntry { get; set; }
        [Key]
        [Column(Order = 2)]
        public int TransReqId { get; set; }

        [Required]
        [MaxLength(228)]
        public string BinCode { get; set; }

        [Required]
        [MaxLength(8)]
        public string WhsCode { get; set; }
        
        public virtual TransReq TransReq { get; set; }

        public virtual ICollection<Box> Boxes { get; set; }

    }
}