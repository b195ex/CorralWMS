namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedWhsInv_User : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WhsInvs", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.WhsInvs", "UserId");
            AddForeignKey("dbo.WhsInvs", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WhsInvs", "UserId", "dbo.Users");
            DropIndex("dbo.WhsInvs", new[] { "UserId" });
            DropColumn("dbo.WhsInvs", "UserId");
        }
    }
}
