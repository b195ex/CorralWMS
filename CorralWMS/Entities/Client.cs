using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Client
    {
        [Key]
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int? RouteID { get; set; }
        public virtual Route Route { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}