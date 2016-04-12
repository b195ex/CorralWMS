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
    }
}