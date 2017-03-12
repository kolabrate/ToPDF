using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Topdf.api.DTO
{
    public class UserAccountDto
    {
        public List<SubscriptionDto> Subscription { get; set; }
        public List<PdfTemplateDto> PdfTemplates { get; set; }
        public MessageDto Messages { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AuthToken { get; set; }
    }

    public class MessageDto
    {
        public int NewMessageCount { get; set; }
        public List<KeyValuePair<int, string>> MessagePreview { get; set; }
        public List<string> Alerts { get; set; }
    }

    public class PdfTemplateDto
    {
        public string PdfTemplateName { get; set; }
        public string PdfTemplateDesc { get; set; }
        public bool IsActive { get; set; }
        public string ApiKey { get; set; }
        public string Desc { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmailErrorTo { get; set; }
        public string ContactEmail { get; set; }
        public string DeliveryMode { get; set; }
        public decimal SuccessRate { get; set; }
    }

    public class SubscriptionDto
    {
        public string SubscriptionName { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
    }
}