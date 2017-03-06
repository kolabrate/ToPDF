namespace Topdf.api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        CompanyName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        Phone = c.String(),
                        CompanyWebsite = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PostCode = c.String(),
                        Avatar = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        EmailVerifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
