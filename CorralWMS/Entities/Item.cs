using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Item
    {
        [Key]
        [MaxLength(20)]
        public string ItemCode { get; set; }

        public string ItemName { get; set; }
        
        public Double Duration { get; set; }
    }
}