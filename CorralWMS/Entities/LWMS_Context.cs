using System.Data.Entity;

namespace CorralWMS.Entities
{
    public class LWMS_Context : DbContext
    {
        public LWMS_Context()
            : base("name=LWMS_Context")
        {

        }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<SapSetting> SapSettings { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<TransReq> TransReqs { get; set; }
        public virtual DbSet<FromLocation> FromLocations { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }
        public virtual DbSet<ToLocation> ToLocations { get; set; }
        public virtual DbSet<Box> Boxes { get; set; }
        public virtual DbSet<MailSetting> MailSettings { get; set; }
        public virtual DbSet<MailingList> MailingLists { get; set; }
        public virtual DbSet<ProdEntry> ProdEntries { get; set; }
        public virtual DbSet<EntryLocation> EntryLocations { get; set; }
        public virtual DbSet<CriticLocation> CriticLocations { get; set; }
        public virtual DbSet<CycleCount> CycleCounts { get; set; }
        public virtual DbSet<BinAudit> BinAudits { get; set; }
        public virtual DbSet<WhsInv> WhsInvs { get; set; }
        public virtual DbSet<LocInv> LocInvs { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrdrDtl> OrdrDtls { get; set; }
    }
}