namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSapSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SapSettings",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        CompanyDB = c.String(nullable: false),
                        DbPassword = c.String(nullable: false),
                        DbServerType = c.Int(nullable: false),
                        DbUserName = c.String(nullable: false),
                        language = c.Int(nullable: false),
                        Password = c.String(nullable: false),
                        Server = c.String(nullable: false),
                        UserName = c.String(nullable: false),
                        UseTrusted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SapSettings");
        }
    }
}
