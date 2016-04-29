using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Transfer
    {
        [ForeignKey("TransReq")]
        public int Id { get; set; }
        
        public int? DocEntry { get; set; }

        public int? DocNum { get; set; }

        public int UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual User User { get; set; }
        public virtual TransReq TransReq { get; set; }

        public virtual ICollection<ToLocation> ToLocations { get; set; }
    }
}