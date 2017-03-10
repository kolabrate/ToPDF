using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Topdf.api.Models
{
    [Table("TemplateSections")]
    public class TemplateSection
    {
        [Key]
            
        public int TemplateSectionId { get; set; }
        [Required]
        public int PdfTemplateId { get; set; }
        [Required]
        [StringLength(10)]
        public string SectionType { get; set; }
        [Required]
        public string ContentMarkup { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual PdfTemplate PdfTemplate { get; set; }
    }
}