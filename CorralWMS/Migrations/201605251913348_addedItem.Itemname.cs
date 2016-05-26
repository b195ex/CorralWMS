namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedItemItemname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Items", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Items", "ItemName");
        }
    }
}
