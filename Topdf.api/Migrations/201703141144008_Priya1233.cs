namespace Topdf.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Priya1233 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserSubscriptions", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserSubscriptions", "Status");
        }
    }
}
