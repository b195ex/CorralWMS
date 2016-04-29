using System.Data.Entity;

namespace CorralWMS.Entities
{
    public class LWMS_Context : DbContext
    {
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
    }
}