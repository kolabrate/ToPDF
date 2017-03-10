namespace Topdf.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Priya1232 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false, maxLength: 200),
                        Body = c.String(nullable: false),
                        TemplateId = c.Int(nullable: false),
                        SentDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        From = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "UserId", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "UserId" });
            DropTable("dbo.Messages");
        }
    }
}
