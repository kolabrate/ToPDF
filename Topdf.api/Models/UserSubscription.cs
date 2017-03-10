using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("UserSubscriptions")]
    public class UserSubscription
    {
        [Key]
        public int UserSubscriptionId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int SubscriptionId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual User User { get; set; }
        public virtual Subscription Subscription { get; set; }

    }
}