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
            context.SapSettings.AddOrUpdate(p => p.id, 
                new SapSetting()
                {
                    CompanyDB = "SBODEMOCR",
                    DbPassword = "On3S0lut10ns",
                    DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012,
                    DbUserName = "sa",
                    id = 1,
                    language = SAPbobsCOM.BoSuppLangs.ln_English,
                    Password = "1234",
                    Server = "HNAPP01",
                    UserName = "manager",
                    UseTrusted = false,
                    LicenseServer = "HNAPP01"
                }
            );
            context.Users.AddOrUpdate(u => u.Id, 
                new User()
                {
                    Active = true,
                    Email = "mail@example.com",
                    FirstName = "Erick",
                    Id = 1,
                    LastName = "Viera",
                    Password = "lonewolf",
                    UserName = "b195ex"
                }
            );
            context.MailSettings.AddOrUpdate(m => m.Id, 
                new MailSetting()
                {
                    FromAddress = "payrolltc@gmail.com",
                    FromPass = "payroll.123",
                    Id = 1,
                    MailHost = "smtp.gmail.com",
                    MailPort = 587
                }
            );
            context.MailingLists.AddOrUpdate(ml => ml.Id,
                new MailingList() { Id = 1, Name = "Facturaci�n" },
                new MailingList() { Id = 2, Name = "Traslado" }
            );
            context.Permissions.AddOrUpdate(p => p.Id,
                new Permission() { Description = "Acceso a ManageCriticLocations", Id = 25 },
                new Permission() { Description = "Acceso a ManageMailingLists", Id = 20 },
                new Permission() { Description = "Acceso a ManagePermissions", Id = 7 },
                new Permission() { Description = "Acceso a ManageRoles", Id = 9 },
                new Permission() { Description = "Acceso a ManageSAP", Id = 14 },
                new Permission() { Description = "Acceso a ManageSettings", Id = 10 },
                new Permission() { Description = "Acceso a ManageSysMail", Id = 19 },
                new Permission() { Description = "Acceso a ManageUsers", Id = 8 },
                new Permission() { Description = "Acceso a ver Inventarios de Almac�n", Id = 32 },
                new Permission() { Description = "Cierre de Inventarios de almac�n", Id = 31 },
                new Permission() { Description = "Creaci�n de inventarios de Almac�", Id = 30 },
                new Permission() { Description = "Creaci�n de Permisos", Id = 2 },
                new Permission() { Description = "Creaci�n de Roles", Id = 3 },
                new Permission() { Description = "Creaci�n de Settings", Id = 11 },
                new Permission() { Description = "Creaci�n de Usuarios", Id = 1 },
                new Permission() { Description = "Crear Lista de Correos", Id = 21 },
                new Permission() { Description = "Crear nueva cuenta de correo del sistema", Id = 17 },
                new Permission() { Description = "Crear transferencia", Id = 15 },
                new Permission() { Description = "Crear Ubicaci�n Cr�tica", Id = 26 },
                new Permission() { Description = "Edici�n de Par�metros de Conexi�n con SAP", Id = 13 },
                new Permission() { Description = "Edici�n de Permisos", Id = 6 },
                new Permission() { Description = "Edici�n de Roles", Id = 5 },
                new Permission() { Description = "Edici�n de Settings", Id = 12 },
                new Permission() { Description = "Edici�n de Usuarios", Id = 4 },
                new Permission() { Description = "Editar cuentas de correo del sistema", Id = 18 },
                new Permission() { Description = "Editar Lista de Correos", Id = 22 },
                new Permission() { Description = "Editar Ubicaci�n Cr�tica", Id = 27 },
                new Permission() { Description = "Eliminar Ubicaci�n Cr�tica", Id = 28 },
                new Permission() { Description = "Ingresar Producto Terminado", Id = 23 },
                new Permission() { Description = "Realizar Auditoria de Ubicaci�n", Id = 29 },
                new Permission() { Description = "Realizar Inventario C�clico", Id = 24 },
                new Permission() { Description = "Realizar Inventario de Ubicaci�n", Id = 33 },
                new Permission() { Description = "Recibir transferencia", Id = 16 }
            );
        }
    }
}
