using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("PdfTemplates")]
    public class PdfTemplate
    {
        [Key]
        public int PdfTemplateId { get; set; }
        [Required]
        [StringLength(100)]
        public string PdfTemplateName { get; set; }
        [StringLength(100)]
        public string Desc { get; set; }
        [Required]
        [StringLength(500)]
        public string ApiKey { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }
        [Required]
        [EmailAddress]
        public string EmailErrorTo { get; set; }
        [Required]
        public int DeliveryModeId { get; set; }
        [Required]
        [StringLength(10)]
        public string InputType { get; set; }
        [Required]
        public string SampleData { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual ICollection<TemplateSection> TemplateSections { get; set; }
        public virtual User User { get; set; }
        public virtual DeliveryMode DeliveryMode { get; set; }
    }
}