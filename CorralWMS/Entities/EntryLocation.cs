using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class EntryLocation
    {
        [Key]
        [Column(Order = 1)]
        public int ProdEntryID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int AbsEntry { get; set; }
        [Required]
        [MaxLength(228)]
        public string BinCode { get; set; }
        public virtual ProdEntry ProdEntry { get; set; }
        public virtual ICollection<Box> Boxes { get; set; }
    }
}