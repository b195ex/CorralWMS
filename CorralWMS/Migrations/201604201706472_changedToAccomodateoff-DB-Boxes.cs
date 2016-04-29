namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedToAccomodateoffDBBoxes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Boxes", "ManufDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Boxes", "ManufDate", c => c.DateTime(nullable: false));
        }
    }
}
