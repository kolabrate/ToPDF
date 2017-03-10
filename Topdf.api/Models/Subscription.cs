using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("Subscriptions")]
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }
        [StringLength(100)]
        [Required]        
        public string SubscriptionName { get; set; }
        [Required]
        public int Price { get; set; }


    }
}