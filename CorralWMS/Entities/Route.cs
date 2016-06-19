using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public string RouteName { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
    }
}