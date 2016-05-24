using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class CriticLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BinEntry { get; set; }
        [MaxLength(228)]
        public string BinCode { get; set; }
        [Index(IsUnique = true)]
        public int Priority { get; set; }
    }
}