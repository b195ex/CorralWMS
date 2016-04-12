namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_role_Description : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roles", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "Description");
        }
    }
}
