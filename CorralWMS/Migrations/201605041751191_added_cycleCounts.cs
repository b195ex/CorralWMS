namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_cycleCounts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CycleCounts",
                c => new
                    {
                        BinEntry = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        BinCode = c.String(maxLength: 228),
                        UserId = c.Int(nullable: false),
                        Closed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.BinEntry, t.Date })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.CriticLocations",
                c => new
                    {
                        BinEntry = c.Int(nullable: false),
                        BinCode = c.String(maxLength: 228),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BinEntry)
                .Index(t => t.Priority, unique: true);
            
            CreateTable(
                "dbo.CycleCountBoxes",
                c => new
                    {
                        CycleCount_BinEntry = c.Int(nullable: false),
                        CycleCount_Date = c.DateTime(nullable: false),
                        Box_Batch = c.String(nullable: false, maxLength: 36),
                        Box_Id = c.Int(nullable: false),
                        Box_ItemCode = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => new { t.CycleCount_BinEntry, t.CycleCount_Date, t.Box_Batch, t.Box_Id, t.Box_ItemCode })
                .ForeignKey("dbo.CycleCounts", t => new { t.CycleCount_BinEntry, t.CycleCount_Date }, cascadeDelete: true)
                .ForeignKey("dbo.Boxes", t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode }, cascadeDelete: true)
                .Index(t => new { t.CycleCount_BinEntry, t.CycleCount_Date })
                .Index(t => new { t.Box_Batch, t.Box_Id, t.Box_ItemCode });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CycleCounts", "UserId", "dbo.Users");
            DropForeignKey("dbo.CycleCountBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes");
            DropForeignKey("dbo.CycleCountBoxes", new[] { "CycleCount_BinEntry", "CycleCount_Date" }, "dbo.CycleCounts");
            DropIndex("dbo.CycleCountBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            DropIndex("dbo.CycleCountBoxes", new[] { "CycleCount_BinEntry", "CycleCount_Date" });
            DropIndex("dbo.CriticLocations", new[] { "Priority" });
            DropIndex("dbo.CycleCounts", new[] { "UserId" });
            DropTable("dbo.CycleCountBoxes");
            DropTable("dbo.CriticLocations");
            DropTable("dbo.CycleCounts");
        }
    }
}
