namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ProdEntry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EntryLocations",
                c => new
                    {
                        ProdEntryID = c.Int(nullable: false),
                        AbsEntry = c.Int(nullable: false),
                        BinCode = c.String(nullable: false, maxLength: 228),
                    })
                .PrimaryKey(t => new { t.ProdEntryID, t.AbsEntry })
                .ForeignKey("dbo.ProdEntries", t => t.ProdEntryID, cascadeDelete: true)
                .Index(t => t.ProdEntryID);
            
            CreateTable(
                "dbo.ProdEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocEntry = c.Int(),
                        BaseEntry = c.Int(),
                        ItemCode = c.String(maxLength: 20),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Boxes", "ProdEntryID", c => c.Int());
            AddColumn("dbo.Boxes", "AbsEntry", c => c.Int());
            CreateIndex("dbo.Boxes", new[] { "ProdEntryID", "AbsEntry" });
            AddForeignKey("dbo.Boxes", new[] { "ProdEntryID", "AbsEntry" }, "dbo.EntryLocations", new[] { "ProdEntryID", "AbsEntry" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Boxes", new[] { "ProdEntryID", "AbsEntry" }, "dbo.EntryLocations");
            DropForeignKey("dbo.ProdEntries", "UserId", "dbo.Users");
            DropForeignKey("dbo.EntryLocations", "ProdEntryID", "dbo.ProdEntries");
            DropIndex("dbo.ProdEntries", new[] { "UserId" });
            DropIndex("dbo.EntryLocations", new[] { "ProdEntryID" });
            DropIndex("dbo.Boxes", new[] { "ProdEntryID", "AbsEntry" });
            DropColumn("dbo.Boxes", "AbsEntry");
            DropColumn("dbo.Boxes", "ProdEntryID");
            DropTable("dbo.ProdEntries");
            DropTable("dbo.EntryLocations");
        }
    }
}
