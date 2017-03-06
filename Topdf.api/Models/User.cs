using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
using System.Data.Entity;


namespace Topdf.api.Models
{
    [Table("Users")]
    public class User
    {

      
            [Key]
            [Required]
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CompanyName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
            public string CompanyWebsite { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string PostCode { get; set; }
            public byte[] Avatar { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public Nullable<System.DateTime> EmailVerifiedDate { get; set; }

    }


    public class ToPDFDBContext : DbContext
    {

        public ToPDFDBContext() : base("TopdfConn")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ToPDFDBContext, Migrations.Configuration>("TopdfConn"));

        }

        public DbSet<User> users { get; set; }


    }
}