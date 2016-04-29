namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_itemcode_to_boxPrimaryKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id" }, "dbo.Boxes");
            DropForeignKey("dbo.Boxes", "ItemCode", "dbo.Items");
            DropForeignKey("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id" }, "dbo.Boxes");
            DropIndex("dbo.Boxes", new[] { "ItemCode" });
            DropIndex("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id" });
            DropIndex("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id" });
            DropPrimaryKey("dbo.Boxes");
            DropPrimaryKey("dbo.FromLocationBoxes");
            DropPrimaryKey("dbo.ToLocationBoxes");
            AddColumn("dbo.FromLocationBoxes", "Box_ItemCode", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.ToLocationBoxes", "Box_ItemCode", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Boxes", "ItemCode", c => c.String(nullable: false, maxLength: 20));
            AddPrimaryKey("dbo.Boxes", new[] { "Batch", "Id", "ItemCode" });
            AddPrimaryKey("dbo.FromLocationBoxes", new[] { "FromLocation_AbsEntry", "FromLocation_TransReqId", "Box_Batch", "Box_Id", "Box_ItemCode" });
            AddPrimaryKey("dbo.ToLocationBoxes", new[] { "ToLocation_AbsEntry", "ToLocation_TransferId", "Box_Batch", "Box_Id", "Box_ItemCode" });
            CreateIndex("dbo.Boxes", "ItemCode");
            CreateIndex("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            CreateIndex("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            AddForeignKey("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes", new[] { "Batch", "Id", "ItemCode" }, cascadeDelete: true);
            AddForeignKey("dbo.Boxes", "ItemCode", "dbo.Items", "ItemCode", cascadeDelete: true);
            AddForeignKey("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes", new[] { "Batch", "Id", "ItemCode" }, cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes");
            DropForeignKey("dbo.Boxes", "ItemCode", "dbo.Items");
            DropForeignKey("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" }, "dbo.Boxes");
            DropIndex("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            DropIndex("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id", "Box_ItemCode" });
            DropIndex("dbo.Boxes", new[] { "ItemCode" });
            DropPrimaryKey("dbo.ToLocationBoxes");
            DropPrimaryKey("dbo.FromLocationBoxes");
            DropPrimaryKey("dbo.Boxes");
            AlterColumn("dbo.Boxes", "ItemCode", c => c.String(maxLength: 20));
            DropColumn("dbo.ToLocationBoxes", "Box_ItemCode");
            DropColumn("dbo.FromLocationBoxes", "Box_ItemCode");
            AddPrimaryKey("dbo.ToLocationBoxes", new[] { "ToLocation_AbsEntry", "ToLocation_TransferId", "Box_Batch", "Box_Id" });
            AddPrimaryKey("dbo.FromLocationBoxes", new[] { "FromLocation_AbsEntry", "FromLocation_TransReqId", "Box_Batch", "Box_Id" });
            AddPrimaryKey("dbo.Boxes", new[] { "Batch", "Id" });
            CreateIndex("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id" });
            CreateIndex("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id" });
            CreateIndex("dbo.Boxes", "ItemCode");
            AddForeignKey("dbo.ToLocationBoxes", new[] { "Box_Batch", "Box_Id" }, "dbo.Boxes", new[] { "Batch", "Id" }, cascadeDelete: true);
            AddForeignKey("dbo.Boxes", "ItemCode", "dbo.Items", "ItemCode");
            AddForeignKey("dbo.FromLocationBoxes", new[] { "Box_Batch", "Box_Id" }, "dbo.Boxes", new[] { "Batch", "Id" }, cascadeDelete: true);
        }
    }
}
