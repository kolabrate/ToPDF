using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [StringLength(200)]
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public int TemplateId { get; set; }
        [Required]
        public DateTime SentDate { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int From { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual User User { get; set; }
    }
}