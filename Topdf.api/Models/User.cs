using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;


namespace Topdf.api.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
        [Required]
        [StringLength(10)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string CompanyWebsite { get; set; }
        [Required]
        [StringLength(100)]
        public string AddressLine1 { get; set; }
        [StringLength(100)]
        public string AddressLine2 { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [StringLength(100)]
        public string State { get; set; }
        [Required]
        [StringLength(100)]
        public string Country { get; set; }
        [Required]
        [StringLength(100)]
        public string PostCode { get; set; }
        public byte[] Avatar { get; set; }
        [Required]
        public System.DateTime CreatedDate { get; set; }        
        public System.DateTime LastModifiedDate { get; set; }
        public Nullable<System.DateTime> EmailVerifiedDate { get; set; }

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }

    }


    public class ToPDFDBContext : DbContext
    {

        public ToPDFDBContext() : base("TopdfConn")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ToPDFDBContext, Migrations.Configuration>("TopdfConn"));

        }

        public DbSet<User> Users { get; set; }
        public DbSet<DeliveryMode> DeliveryModes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PdfTemplate> PdfTemplates { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public DbSet<TemplateSection> TemplateSections { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
    }
}