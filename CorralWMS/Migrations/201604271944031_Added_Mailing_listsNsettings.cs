namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Mailing_listsNsettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MailingLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MailSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromAddress = c.String(nullable: false),
                        FromPass = c.String(nullable: false),
                        MailHost = c.String(nullable: false),
                        MailPort = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MailingListUsers",
                c => new
                    {
                        MailingList_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.MailingList_Id, t.User_Id })
                .ForeignKey("dbo.MailingLists", t => t.MailingList_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.MailingList_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MailingListUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.MailingListUsers", "MailingList_Id", "dbo.MailingLists");
            DropIndex("dbo.MailingListUsers", new[] { "User_Id" });
            DropIndex("dbo.MailingListUsers", new[] { "MailingList_Id" });
            DropTable("dbo.MailingListUsers");
            DropTable("dbo.MailSettings");
            DropTable("dbo.MailingLists");
        }
    }
}
