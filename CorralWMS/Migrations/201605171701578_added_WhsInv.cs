namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_WhsInv : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocInvs",
                c => new
                    {
                        WhsinvId = c.Int(nullable: false),
                        BinAbs = c.Int(nullable: false),
                        BinCode = c.String(),
                        UserId = c.Int(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.WhsinvId, t.BinAbs })
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.WhsInvs", t => t.WhsinvId, cascadeDelete: true)
                .Index(t => t.WhsinvId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.WhsInvs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WhsCode = c.String(nullable: false),
                        WhsName = c.String(),
                        DocEntry = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocInvBoxes",
                c => new
                    {
                        LocInv_WhsinvId = c.Int(nullable: false),
                        LocInv_BinAbs = c.Int(nullable: false),
                        Box_Batch = c.String(nullable: false, maxLength: 36),
                        Box_Id = c.Int(nullable: false),
                        Box_ItemCode = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => new { t.LocInv_WhsinvId, t.LocInv_BinAbs, t.Box_Batch, t.Box_Id, t.Box_ItemCode })
                .ForeignKey("dbo.LocInvs", t => new { t.LocInv_WhsinvId, t.LocInv_BinAbs }, cascadeDelete: true)
                .ForeignKey("dbo.Boxes", t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode }, cascadeDelete: true)
                .Index(t => new { t.LocInv_WhsinvId, t.LocInv_BinAbs })
                .Index(t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocInvs", "WhsinvId", "dbo.WhsInvs");
            DropForeignKey("dbo.LocInvs", "UserId", "dbo.Users");
            DropForeignKey("dbo.LocInvBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes");
            DropForeignKey("dbo.LocInvBoxes", new[] { "LocInv_WhsinvId", "LocInv_BinAbs" }, "dbo.LocInvs");
            DropIndex("dbo.LocInvBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            DropIndex("dbo.LocInvBoxes", new[] { "LocInv_WhsinvId", "LocInv_BinAbs" });
            DropIndex("dbo.LocInvs", new[] { "UserId" });
            DropIndex("dbo.LocInvs", new[] { "WhsinvId" });
            DropTable("dbo.LocInvBoxes");
            DropTable("dbo.WhsInvs");
            DropTable("dbo.LocInvs");
        }
    }
}
