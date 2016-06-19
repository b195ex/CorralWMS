﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CorralWMS.Entities
{
    public class Box
    {
        [Key]
        [MaxLength(36)]
        [Column(Order = 1)]
        public String Batch { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Id { get; set; }
        [NotMapped]
        public string SAPBatch { get { return Batch + Id.ToString().PadLeft(8, '0'); } }
        [Key]
        [Column(Order = 3)]
        [MaxLength(20)]
        public string ItemCode { get; set; }
        public DateTime? ManufDate { get; set; }
        [NotMapped]
        public DateTime ExpDate { get { return ManufDate == null ? ManufDate.Value : ManufDate.Value.AddDays(Item.Duration); } }
        public Double Weight { get; set; }
        [ForeignKey("EntryLocation")]
        [Column(Order = 4)]
        public int? ProdEntryID { get; set; }
        [ForeignKey("EntryLocation")]
        [Column(Order = 5)]
        public int? AbsEntry { get; set; }
        public virtual ICollection<FromLocation> FromLocations { get; set; }
        public virtual ICollection<ToLocation> ToLocations { get; set; }
        public virtual Item Item { get; set; }
        public virtual EntryLocation EntryLocation { get; set; }
        public virtual ICollection<CycleCount> CycleCounts { get; set; }
        public virtual ICollection<BinAudit> BinAudits { get; set; }
        public virtual ICollection<LocInv> LocInvs { get; set; }
        public virtual OrdrDtl OrdrDtl { get; set; }
    }
}