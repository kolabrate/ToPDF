namespace Topdf.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Priya1231 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TemplateSections", newName: "UserSubscriptions");
            CreateTable(
                "dbo.DeliveryModes",
                c => new
                    {
                        DeliveryModeId = c.Int(nullable: false, identity: true),
                        DeliveryModeName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.DeliveryModeId);
            
            CreateTable(
                "dbo.PdfTemplates",
                c => new
                    {
                        PdfTemplateId = c.Int(nullable: false, identity: true),
                        PdfTemplateName = c.String(nullable: false, maxLength: 100),
                        Desc = c.String(maxLength: 100),
                        ApiKey = c.String(nullable: false, maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        ContactEmail = c.String(nullable: false),
                        EmailErrorTo = c.String(nullable: false),
                        DeliveryModeId = c.Int(nullable: false),
                        InputType = c.String(nullable: false, maxLength: 10),
                        SampleXml = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UserId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PdfTemplateId)
                .ForeignKey("dbo.DeliveryModes", t => t.DeliveryModeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.DeliveryModeId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TemplateSections",
                c => new
                    {
                        TemplateSectionId = c.Int(nullable: false, identity: true),
                        PdfTemplateId = c.Int(nullable: false),
                        SectionType = c.String(nullable: false, maxLength: 10),
                        ContentMarkup = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TemplateSectionId)
                .ForeignKey("dbo.PdfTemplates", t => t.PdfTemplateId, cascadeDelete: true)
                .Index(t => t.PdfTemplateId);
            
            CreateTable(
                "dbo.SubscriptionFeatures",
                c => new
                    {
                        SubscriptionFeatureId = c.Int(nullable: false, identity: true),
                        SubscriptionFeatureName = c.String(nullable: false, maxLength: 100),
                        SubscriptionId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionFeatureId)
                .ForeignKey("dbo.Subscriptions", t => t.SubscriptionId, cascadeDelete: true)
                .Index(t => t.SubscriptionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionFeatures", "SubscriptionId", "dbo.Subscriptions");
            DropForeignKey("dbo.PdfTemplates", "UserId", "dbo.Users");
            DropForeignKey("dbo.TemplateSections", "PdfTemplateId", "dbo.PdfTemplates");
            DropForeignKey("dbo.PdfTemplates", "DeliveryModeId", "dbo.DeliveryModes");
            DropIndex("dbo.SubscriptionFeatures", new[] { "SubscriptionId" });
            DropIndex("dbo.TemplateSections", new[] { "PdfTemplateId" });
            DropIndex("dbo.PdfTemplates", new[] { "UserId" });
            DropIndex("dbo.PdfTemplates", new[] { "DeliveryModeId" });
            DropTable("dbo.SubscriptionFeatures");
            DropTable("dbo.TemplateSections");
            DropTable("dbo.PdfTemplates");
            DropTable("dbo.DeliveryModes");
            RenameTable(name: "dbo.UserSubscriptions", newName: "TemplateSections");
        }
    }
}
