using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class ToLocation
    {
        [Key]
        [Column(Order = 1)]
        public int AbsEntry { get; set; }
        [Key]
        [Column(Order = 2)]
        public int TransferId { get; set; }

        [Required]
        [MaxLength(228)]
        public string BinCode { get; set; }

        [Required]
        [MaxLength(8)]
        public string WhsCode { get; set; }

        public virtual Transfer Transfer { get; set; }

        public virtual ICollection<Box> Boxes { get; set; }
    }
}