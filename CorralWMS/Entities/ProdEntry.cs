using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class ProdEntry
    {
        public int Id { get; set; }
        public int? DocEntry { get; set; }
        public int? BaseEntry { get; set; }
        [MaxLength(20)]
        public string ItemCode { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<EntryLocation> EntryLocations { get; set; }
    }
}