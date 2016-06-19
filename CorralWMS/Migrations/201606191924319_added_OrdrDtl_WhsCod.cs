namespace CorralWMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_OrdrDtl_WhsCod : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrdrDtls", "WhsCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrdrDtls", "WhsCode");
        }
    }
}
