namespace CorralWMS.Migrations
{
    using CorralWMS.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CorralWMS.Entities.LWMS_Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CorralWMS.Entities.LWMS_Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.SapSettings.AddOrUpdate(p => p.id, new SapSetting(){
                CompanyDB = "SBO_DEMOCR",
                DbPassword = "On3S0lu10ns",
                DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012,
                DbUserName = "sa",
                id = 1,
                language = SAPbobsCOM.BoSuppLangs.ln_English,
                Password = "manager",
                Server = "HNAPP01",
                UserName = "manager",
                UseTrusted = false
            });
            context.Users.AddOrUpdate(u => u.Id, new User()
            {
                Active = true,
                Email = "mail@example.com",
                FirstName = "Erick",
                Id = 1,
                LastName = "Viera",
                Password = "lonewolf",
                UserName = "b195ex"
            });
        }
    }
}
