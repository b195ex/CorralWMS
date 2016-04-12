namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUsers_Roles_Permissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppSettings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Description, unique: true);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 128),
                        Permission_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permissions", t => t.Permission_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.RoleName, unique: true)
                .Index(t => t.Permission_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 64),
                        Password = c.String(nullable: false, maxLength: 64),
                        FirstName = c.String(maxLength: 128),
                        LastName = c.String(maxLength: 128),
                        Email = c.String(),
                        Active = c.Boolean(nullable: false, defaultValue: true),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Roles", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Roles", "Permission_Id", "dbo.Permissions");
            DropIndex("dbo.Users", new[] { "UserName" });
            DropIndex("dbo.Roles", new[] { "User_Id" });
            DropIndex("dbo.Roles", new[] { "Permission_Id" });
            DropIndex("dbo.Roles", new[] { "RoleName" });
            DropIndex("dbo.Permissions", new[] { "Description" });
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.Permissions");
            DropTable("dbo.AppSettings");
        }
    }
}
