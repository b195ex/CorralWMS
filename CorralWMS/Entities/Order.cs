using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        [ForeignKey("Client")]
        public String CardCode { get; set; }
        public int? TargetEntry { get; set; }
        public int? TargetRef { get; set; }
        public string Comment { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? UserId { get; set; }
        //public SAPbobsCOM.BoObjectTypes TargetType { get; set; }

        public virtual User Filler { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<OrdrDtl> OrdrDtls { get; set; }
    }
}