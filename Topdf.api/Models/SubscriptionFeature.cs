using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("SubscriptionFeatures")]
    public class SubscriptionFeature
    {
       [Key]
        public int SubscriptionFeatureId { get; set; }
        [Required]
        [StringLength(100)]
        public string SubscriptionFeatureName { get; set; }
        [Required]
        public int SubscriptionId { get; set; }
        [Required]        
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual Subscription Subscription { get; set; }
    }
}