using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class TransReq
    {
        public int Id { get; set; }
        
        public int? DocEntry { get; set; }
        
        public int? DocNum { get; set; }
        
        public int UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        
        [Required]
        [MaxLength(8)]
        public string FromWhs { get; set; }
        
        [Required]
        [MaxLength(8)]
        public string ToWhs { get; set; }
        
        public virtual User User { get; set; }
        public virtual Transfer Transfer { get; set; }

        public virtual ICollection<FromLocation> FromLocations { get; set; }
    }
}