namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_transferendDateToNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transfers", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transfers", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
