using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("DeliveryModes")]
    public class DeliveryMode
    {
        [Key]
        public int DeliveryModeId { get; set; }
        [Required]
        [StringLength(50)]
        public string DeliveryModeName { get; set; }
    }
}