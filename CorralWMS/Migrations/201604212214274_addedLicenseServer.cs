namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLicenseServer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SapSettings", "LicenseServer", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SapSettings", "LicenseServer");
        }
    }
}
