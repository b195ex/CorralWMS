namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedStockTransferObjects : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RolePermissions", newName: "PermissionRoles");
            RenameTable(name: "dbo.UserRoles", newName: "RoleUsers");
            DropPrimaryKey("dbo.PermissionRoles");
            DropPrimaryKey("dbo.RoleUsers");
            CreateTable(
                "dbo.Boxes",
                c => new
                    {
                        Batch = c.String(nullable: false, maxLength: 36),
                        Id = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 20),
                        ManufDate = c.DateTime(nullable: false),
                        Weight = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.Batch, t.Id })
                .ForeignKey("dbo.Items", t => t.ItemCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "dbo.FromLocations",
                c => new
                    {
                        AbsEntry = c.Int(nullable: false),
                        TransReqId = c.Int(nullable: false),
                        BinCode = c.String(nullable: false, maxLength: 228),
                        WhsCode = c.String(nullable: false, maxLength: 8),
                    })
                .PrimaryKey(t => new { t.AbsEntry, t.TransReqId })
                .ForeignKey("dbo.TransReqs", t => t.TransReqId, cascadeDelete: true)
                .Index(t => t.TransReqId);
            
            CreateTable(
                "dbo.TransReqs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocEntry = c.Int(),
                        DocNum = c.Int(),
                        UserId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        FromWhs = c.String(nullable: false, maxLength: 8),
                        ToWhs = c.String(nullable: false, maxLength: 8),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Transfers",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        DocEntry = c.Int(),
                        DocNum = c.Int(),
                        UserId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransReqs", t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemCode = c.String(nullable: false, maxLength: 20),
                        Duration = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ItemCode);
            
            CreateTable(
                "dbo.ToLocations",
                c => new
                    {
                        AbsEntry = c.Int(nullable: false),
                        TransferId = c.Int(nullable: false),
                        BinCode = c.String(nullable: false, maxLength: 228),
                        WhsCode = c.String(nullable: false, maxLength: 8),
                    })
                .PrimaryKey(t => new { t.AbsEntry, t.TransferId })
                .ForeignKey("dbo.Transfers", t => t.TransferId, cascadeDelete: true)
                .Index(t => t.TransferId);
            
            CreateTable(
                "dbo.FromLocationBoxes",
                c => new
                    {
                        FromLocation_AbsEntry = c.Int(nullable: false),
                        FromLocation_TransReqId = c.Int(nullable: false),
                        Box_Batch = c.String(nullable: false, maxLength: 36),
                        Box_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FromLocation_AbsEntry, t.FromLocation_TransReqId, t.Box_Batch, t.Box_Id })
                .ForeignKey("dbo.FromLocations", t => new { t.FromLocation_AbsEntry, t.FromLocation_TransReqId }, cascadeDelete: true)
                .ForeignKey("dbo.Boxes", t => new { t.Box_Batch, t.Box_Id }, cascadeDelete: true)
                .Index(t => new { t.FromLocation_AbsEntry, t.FromLocation_TransReqId })
                .Index(t => new { t.Box_Batch, t.Box_Id });
            
            CreateTable(
                "dbo.ToLocationBoxes",
                c => new
                    {
                        ToLocation_AbsEntry = c.Int(nullable: false),
                        ToLocation_TransferId = c.Int(nullable: false),
                        Box_Batch = c.String(nullable: false, maxLength: 36),
                        Box_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ToLocation_AbsEntry, t.ToLocation_TransferId, t.Box_Batch, t.Box_Id })
                .ForeignKey("dbo.ToLocations", t => new { t.ToLocation_AbsEntry, t.ToLocation_TransferId }, cascadeDelete: true)
                .ForeignKey("dbo.Boxes", t => new { t.Box_Batch, t.Box_Id }, cascadeDelete: true)
                .Index(t => new { t.ToLocation_AbsEntry, t.ToLocation_TransferId })
                .Index(t => new { t.Box_Batch, t.Box_Id });
            
            AddPrimaryKey("dbo.PermissionRoles", new[] { "Permission_Id", "Role_Id" });
            AddPrimaryKey("dbo.RoleUsers", new[] { "Role_Id", "User_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ToLocations", "TransferId", "dbo.Transfers");
            DropForeignKey("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id" }, "dbo.Boxes");
            DropForeignKey("dbo.ToLocationBoxes", new[] { "ToLocation_AbsEntry", "ToLocation_TransferId" }, "dbo.ToLocations");
            DropForeignKey("dbo.Boxes", "ItemCode", "dbo.Items");
            DropForeignKey("dbo.FromLocations", "TransReqId", "dbo.TransReqs");
            DropForeignKey("dbo.TransReqs", "UserId", "dbo.Users");
            DropForeignKey("dbo.Transfers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Transfers", "Id", "dbo.TransReqs");
            DropForeignKey("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id" }, "dbo.Boxes");
            DropForeignKey("dbo.FromLocationBoxes", new[] { "FromLocation_AbsEntry", "FromLocation_TransReqId" }, "dbo.FromLocations");
            DropIndex("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id" });
            DropIndex("dbo.ToLocationBoxes", new[] { "ToLocation_AbsEntry", "ToLocation_TransferId" });
            DropIndex("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id" });
            DropIndex("dbo.FromLocationBoxes", new[] { "FromLocation_AbsEntry", "FromLocation_TransReqId" });
            DropIndex("dbo.ToLocations", new[] { "TransferId" });
            DropIndex("dbo.Transfers", new[] { "UserId" });
            DropIndex("dbo.Transfers", new[] { "Id" });
            DropIndex("dbo.TransReqs", new[] { "UserId" });
            DropIndex("dbo.FromLocations", new[] { "TransReqId" });
            DropIndex("dbo.Boxes", new[] { "ItemCode" });
            DropPrimaryKey("dbo.RoleUsers");
            DropPrimaryKey("dbo.PermissionRoles");
            DropTable("dbo.ToLocationBoxes");
            DropTable("dbo.FromLocationBoxes");
            DropTable("dbo.ToLocations");
            DropTable("dbo.Items");
            DropTable("dbo.Transfers");
            DropTable("dbo.TransReqs");
            DropTable("dbo.FromLocations");
            DropTable("dbo.Boxes");
            AddPrimaryKey("dbo.RoleUsers", new[] { "User_Id", "Role_Id" });
            AddPrimaryKey("dbo.PermissionRoles", new[] { "Role_Id", "Permission_Id" });
            RenameTable(name: "dbo.RoleUsers", newName: "UserRoles");
            RenameTable(name: "dbo.PermissionRoles", newName: "RolePermissions");
        }
    }
}
