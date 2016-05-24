namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_BinAudits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BinAudits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinEntry = c.Int(nullable: false),
                        BinCode = c.String(nullable: false),
                        WhsCode = c.String(nullable: false),
                        UserId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.BoxBinAudits",
                c => new
                    {
                        Box_Batch = c.String(nullable: false, maxLength: 36),
                        Box_Id = c.Int(nullable: false),
                        Box_ItemCode = c.String(nullable: false, maxLength: 20),
                        BinAudit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode, t.BinAudit_Id })
                .ForeignKey("dbo.Boxes", t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode }, cascadeDelete: true)
                .ForeignKey("dbo.BinAudits", t => t.BinAudit_Id, cascadeDelete: true)
                .Index(t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode })
                .Index(t => t.BinAudit_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BinAudits", "UserId", "dbo.Users");
            DropForeignKey("dbo.BoxBinAudits", "BinAudit_Id", "dbo.BinAudits");
            DropForeignKey("dbo.BoxBinAudits", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes");
            DropIndex("dbo.BoxBinAudits", new[] { "BinAudit_Id" });
            DropIndex("dbo.BoxBinAudits", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            DropIndex("dbo.BinAudits", new[] { "UserId" });
            DropTable("dbo.BoxBinAudits");
            DropTable("dbo.BinAudits");
        }
    }
}
