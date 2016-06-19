namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRoutes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrdrDtls",
                c => new
                    {
                        DocEntry = c.Int(nullable: false),
                        LineNum = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 20),
                        Quantity = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.DocEntry, t.LineNum })
                .ForeignKey("dbo.Items", t => t.ItemCode)
                .ForeignKey("dbo.Orders", t => t.DocEntry, cascadeDelete: true)
                .Index(t => t.DocEntry)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        DocEntry = c.Int(nullable: false),
                        DocNum = c.Int(nullable: false),
                        CardCode = c.String(maxLength: 128),
                        TargetEntry = c.Int(),
                        TargetRef = c.Int(),
                        Comment = c.String(),
                        DocDate = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.DocEntry)
                .ForeignKey("dbo.Clients", t => t.CardCode)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.CardCode)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        CardCode = c.String(nullable: false, maxLength: 128),
                        CardName = c.String(),
                        RouteID = c.Int(),
                    })
                .PrimaryKey(t => t.CardCode)
                .ForeignKey("dbo.Routes", t => t.RouteID)
                .Index(t => t.RouteID);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RouteName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Boxes", "OrdrDtl_DocEntry", c => c.Int());
            AddColumn("dbo.Boxes", "OrdrDtl_LineNum", c => c.Int());
            CreateIndex("dbo.Boxes", new[] { "OrdrDtl_DocEntry", "OrdrDtl_LineNum" });
            AddForeignKey("dbo.Boxes", new[] { "OrdrDtl_DocEntry", "OrdrDtl_LineNum" }, "dbo.OrdrDtls", new[] { "DocEntry", "LineNum" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrdrDtls", "DocEntry", "dbo.Orders");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "CardCode", "dbo.Clients");
            DropForeignKey("dbo.Clients", "RouteID", "dbo.Routes");
            DropForeignKey("dbo.OrdrDtls", "ItemCode", "dbo.Items");
            DropForeignKey("dbo.Boxes", new[] { "OrdrDtl_DocEntry", "OrdrDtl_LineNum" }, "dbo.OrdrDtls");
            DropIndex("dbo.Clients", new[] { "RouteID" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "CardCode" });
            DropIndex("dbo.OrdrDtls", new[] { "ItemCode" });
            DropIndex("dbo.OrdrDtls", new[] { "DocEntry" });
            DropIndex("dbo.Boxes", new[] { "OrdrDtl_DocEntry", "OrdrDtl_LineNum" });
            DropColumn("dbo.Boxes", "OrdrDtl_LineNum");
            DropColumn("dbo.Boxes", "OrdrDtl_DocEntry");
            DropTable("dbo.Routes");
            DropTable("dbo.Clients");
            DropTable("dbo.Orders");
            DropTable("dbo.OrdrDtls");
        }
    }
}
