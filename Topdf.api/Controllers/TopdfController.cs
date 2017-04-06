using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spire.Pdf;
using Spire.Pdf.AutomaticFields;
using Spire.Pdf.Graphics;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using Topdf.api.DTO;
using Topdf.api.Models;

namespace Topdf.api.Controllers
{
    [EnableCors(origins: "http://localhost:1105", headers: "*", methods: "*")]

    public class TopdfController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Sample()
        {
            return SendHttpResponse("Success  - " + DateTime.Now.ToString(), HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage CreateUser([FromBody]User value)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    var user = new User();
                    user.AddressLine1 = value.AddressLine1;
                    user.AddressLine2 = value.AddressLine2;
                    user.City = value.City;
                    user.CompanyName = value.CompanyName;
                    user.CompanyWebsite = value.CompanyWebsite;
                    user.Country = value.Country;
                    user.Email = value.Email;
                    user.FirstName = value.FirstName;
                    user.LastName = value.LastName;
                    user.Password = value.Password;
                    user.Phone = value.Phone;
                    user.PostCode = value.PostCode;
                    user.State = value.State;
                    user.Avatar = value.Avatar;
                    user.CreatedDate = DateTime.Now;

                    context.Users.Add(user);
                    context.SaveChanges();

                    return SendHttpResponse("Success", HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return SendHttpResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Login([FromBody]dynamic value)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    string e = value.Email;
                    string p = value.Password;
                    var user = context.Users.FirstOrDefault(c => c.Email == e && c.Password == p);
                    if (user == null) return SendHttpResponse("Not Authenticated", HttpStatusCode.InternalServerError);

                    return SendHttpResponse(user.UserId, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateTemplate([FromBody]Models.PdfTemplate template)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var pdfTemplate = new Models.PdfTemplate();
                            pdfTemplate.ApiKey = GetApiKey();
                            pdfTemplate.ContactEmail = template.ContactEmail;
                            pdfTemplate.CreatedDate = DateTime.UtcNow;
                            pdfTemplate.DeliveryModeId = template.DeliveryModeId;
                            pdfTemplate.Desc = template.Desc;
                            pdfTemplate.EmailErrorTo = template.EmailErrorTo;
                            pdfTemplate.InputType = template.InputType;
                            pdfTemplate.IsActive = true;
                            pdfTemplate.IsDeleted = false;
                            pdfTemplate.PdfTemplateName = template.PdfTemplateName;
                            pdfTemplate.SampleData = template.SampleData;
                            pdfTemplate.UserId = template.UserId;
                            pdfTemplate.TemplateSections = GetTemplateSections(template.TemplateSections);

                            context.PdfTemplates.Add(pdfTemplate);
                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            throw;
                        }
                    }
                    return SendHttpResponse("Success", HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        private ICollection<TemplateSection> GetTemplateSections(ICollection<TemplateSection> templateSections)
        {
            List<TemplateSection> temSections = new List<TemplateSection>();

            foreach (var t in templateSections)
            {
                var tempSection = new TemplateSection();
                tempSection.ContentMarkup = t.ContentMarkup;
                tempSection.CreatedDate = DateTime.UtcNow;
                tempSection.SectionType = t.SectionType;
                temSections.Add(tempSection);
            }
            return temSections;
        }

        private string GetApiKey()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public HttpResponseMessage Dashboard(int userId)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    var user = context.Users.FirstOrDefault(c => c.UserId == userId);
                    if (user != null)
                    {
                        return SendHttpResponse(GetLoginResult(user), HttpStatusCode.OK);
                    }
                }
                return SendHttpResponse("User not found", HttpStatusCode.InternalServerError);
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage UserAvatar(int userId)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    var user = context.Users.FirstOrDefault(c => c.UserId == userId);
                    if (user == null) return SendHttpResponse("User not found", HttpStatusCode.InternalServerError);

                    return SendHttpResponse(user.Avatar, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage Preview()
        {
            try
            {
                string fileName = string.Format("{0}.pdf", Guid.NewGuid());
                string data = Request.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(data)) throw new Exception("Html not recieved");
                string defaultFont = "Callibri"; float defaultSize = 11f;
                byte[] bytes = GetPdfFile(data, "", "", fileName, defaultFont, defaultSize);

                if (bytes != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(bytes, 0, bytes.Length);

                        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                        httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                        httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                        httpResponseMessage.StatusCode = HttpStatusCode.OK;
                        return httpResponseMessage;
                    }
                }
                return this.Request.CreateResponse(HttpStatusCode.NotFound, "Cannot create preview file");
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        private byte[] GetPdfFile(string header, string body, string footer, string fileName, string defaultFont, float defaultSize)
        {
            //create new PDF document
            PdfDocument doc = new PdfDocument();
            //set margin   
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(2.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(4.17f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;
            //apply template in PDF document
            SetDocumentTemplate(doc, PdfPageSize.A4, margin, header,defaultFont, defaultSize);

            int pageNum = 1;
            PdfPageBase page = doc.Pages.Add();
            doc.Pages.Add();

            AddFooterToPage(doc, pageNum.ToString(), ref page, PdfPageSize.A4.Width, "Callibri");

            //Save pdf file.
            var root = HttpContext.Current.Server.MapPath(fileName);
            doc.SaveToFile(root);
            doc.Close();
            return File.ReadAllBytes(root);
        }

        private void SetDocumentTemplate(PdfDocument doc, SizeF pageSize, PdfMargins margin, string header,  string defaultFont, float defaultSize)
        {
            if (string.IsNullOrEmpty(header)) return;
            //set a top page template
            PdfPageTemplateElement topSpace = new PdfPageTemplateElement(pageSize.Width, margin.Top);
            topSpace.Foreground = true;
            doc.Template.Top = topSpace;
            //draw text header in template
            float y = 0;
            float x = 0;

            var lines = GetLines(header,  defaultFont,  defaultSize);
            foreach (var line in lines)
            {
                topSpace.Graphics.DrawString(line.Text, line.Font, line.Brush, x, y, line.Format);
                y = y + line.Size.Height;

                if (line.IsLine)
                {
                    PdfPen pen1 = new PdfPen(Color.Black, 2f);
                    topSpace.Graphics.DrawLine(pen1, 0, y, pageSize.Width, y);
                }
            }
        }

        private List<Line> GetLines(string header, string defaultFont, float defaultSize)
        {
            HtmlDocument headerHtml = new HtmlDocument();
            headerHtml.LoadHtml(header);

            string s = "h1,h2,h3,h4,h5,h6,p";

            var lines = new List<Line>();
            foreach (var node in headerHtml.DocumentNode.ChildNodes)
            {
                var line = new Line();
                #region h & p;
                if (s.IndexOf(node.Name) > 0)
                {
                    line.Text = node.InnerText;
                    var isBold = node.InnerHtml.Contains("<strong>");
                    var isItalic = node.InnerHtml.Contains("<em>");
                    var isUl = node.InnerHtml.Contains("<u>");
                    var isStrike = node.InnerHtml.Contains("<strike>");

                    var name = node.Name.ToLower();
                    float f = name == "h1" ? 32f : name == "h2" ? 24f : name == "h3" ? 18f : name == "h4" ? 15f : name == "h5" ? 13f : name == "h6" ? 10f : defaultSize;
                    FontStyle f1 = 0;
                    if (isBold)
                        f1 |= FontStyle.Bold;
                    if (isItalic)
                        f1 |= FontStyle.Italic;
                    if (isUl)
                        f1 |= FontStyle.Underline;
                    if (isStrike)
                        f1 |= FontStyle.Strikeout;

                    line.Font = new PdfTrueTypeFont(new Font(defaultFont, f, f1));
                    line.Brush = new PdfSolidBrush(Color.Black);
                    line.Format =new PdfStringFormat(PdfTextAlignment.Left);
                }
                #endregion
                else if (node.Name == "br")
                {
                    line.IsLineBreak = true;
                    line.Text = " ";
                }
                else if (node.Name == "hr")
                {
                    line.IsLine = true;
                }
                else
                {
                    line.Text = "Invalid Value";
                }
                lines.Add(line);
            }
            return lines;
        }

        [HttpPost]
        public HttpResponseMessage FormatData()
        {
            string retData = "";
            string data = Request.Content.ReadAsStringAsync().Result;
            List<Fields> fList = new List<Fields>();
            JToken node = null;
            try
            {
                node = JToken.Parse(data);
                Fields f = new Fields();
                f.Children = new List<Fields>();
                f.FieldName = "Root";
                if (node.Type == JTokenType.Array)
                {
                    foreach (JToken j in node.Children())
                    {
                        if (j.Type == JTokenType.Object)
                        {
                            handleObj((JObject)j, f);
                        }
                        if (j.Type == JTokenType.Property)
                        {
                            f.FieldValue = ((JProperty)j).Value.ToString();
                            f.FieldPath = j.Path;
                        }
                    }
                }
                if (node.Type == JTokenType.Object)
                {
                    handleObj((JObject)node, f);
                }
                fList.Add(f);
                return SendHttpResponse(fList, HttpStatusCode.OK);
            }
            catch (JsonReaderException)
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(data);

                    Fields f = new Fields();
                    f.Children = new List<Fields>();
                    f.FieldName = xDoc.DocumentElement.Name;
                    f.FieldPath = "/" + f.FieldName;
                    fList.Add(f);
                    foreach (XmlNode x in xDoc.DocumentElement.ChildNodes)
                    {
                        handleXml(x, f);
                    }
                    return SendHttpResponse(fList, HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return SendHttpResponse(ex.Message + ex.StackTrace, HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message + e.StackTrace, HttpStatusCode.InternalServerError);
            }
        }

        public float MARGIN_FROM_LEFT = 30f;
        public float MARGIN_FROM_RIGHT = 30f;
        private void AddFooterToPage(PdfDocument doc, string pageNum, ref PdfPageBase page1, float pageWidth, string fontName)
        {
            PdfUnitConvertor unitCvtr = new PdfUnitConvertor();
            PdfMargins margin = new PdfMargins();
            margin.Top = unitCvtr.ConvertUnits(2.54f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Bottom = margin.Top;
            margin.Left = unitCvtr.ConvertUnits(3.17f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            margin.Right = margin.Left;

            var pageSize = PdfPageSize.A4;

            PdfPageTemplateElement bottomSpace
                = new PdfPageTemplateElement(pageSize.Width, margin.Bottom - 10);
            bottomSpace.Foreground = true;
            doc.Template.Bottom = bottomSpace;

            PdfTrueTypeFont pageNormalFont = new PdfTrueTypeFont(new Font(fontName, 6f, FontStyle.Regular));
            PdfPen linePen1 = new PdfPen(Color.Black, 0.5f);
            //draw footer label
            float y = pageNormalFont.Height + 1;
            bottomSpace.Graphics.DrawLine(linePen1, 0, y, pageSize.Width - 1, y);
            y = y + 1;


            PdfPageNumberField pageNumber = new PdfPageNumberField();
            PdfPageCountField pageCount = new PdfPageCountField();
            PdfCompositeField pageNumberLabel = new PdfCompositeField();
            pageNumberLabel.AutomaticFields
                = new PdfAutomaticField[] { pageNumber, pageCount };
            pageNumberLabel.Brush = PdfBrushes.Black;
            pageNumberLabel.Font = pageNormalFont;
            pageNumberLabel.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            pageNumberLabel.Text = "Page {0}";
            pageNumberLabel.Draw(bottomSpace.Graphics, pageSize.Width - MARGIN_FROM_RIGHT, y);

            //footerY = footerY + 6;
            var brush2 = new PdfSolidBrush(Color.Black);
            var font2 = new PdfTrueTypeFont(new Font(fontName, 6f, FontStyle.Regular));
            var format2 = new PdfStringFormat(PdfTextAlignment.Left);
            string text = "Proof Of Delivery - Generated by Linfox Australia Pty Ltd.";
            bottomSpace.Graphics.DrawString(text, font2, brush2, 0, y, format2);
            text = "Page " + pageNumberLabel.Text;
            //bottomSpace.Graphics.DrawString(text, font2, brush2, pageWidth - 30, y, format2);

            string tz = "";
            text = "Generated On: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss ") + tz;
            bottomSpace.Graphics.DrawString(text, font2, brush2, 0, y + 10, format2);
        }
        private static void handleXml(XmlNode xmlNode, Fields field)
        {
            Fields f = new Fields();
            f.Children = new List<Fields>();
            f.FieldName = xmlNode.Name;
            f.FieldValue = xmlNode.InnerText;
            f.FieldPath = field.FieldPath + "/" + f.FieldName;
            field.Children.Add(f);

            foreach (XmlNode x in xmlNode.ChildNodes)
            {
                if (x.Value == null)
                {
                    handleXml(x, f);
                }
            }
        }

        private UserAccountDto GetLoginResult(User user)
        {
            string token = Encrypt(user.Email + "|" + DateTime.Now.ToString("yyyyMMddHHmmssff"));
            var r = new UserAccountDto();
            r.User = new UserDto() { AuthToken = token, UserId = user.UserId, UserName = user.FirstName + " " + user.LastName, Email = user.Email };
            r.SuccessCount = 0;
            r.ErrorCount = 0;
            using (var context = new ToPDFDBContext())
            {
                r.Messages = null;
                r.PdfTemplates = context.PdfTemplates.Where(c => c.UserId == user.UserId && !c.IsDeleted).Take(20).Select
                    (s => new PdfTemplateDto()
                    {
                        ApiKey = s.ApiKey,
                        ContactEmail = s.ContactEmail,
                        CreatedDate = s.CreatedDate,
                        DeliveryMode = s.DeliveryMode.DeliveryModeName,
                        Desc = s.Desc,
                        EmailErrorTo = s.EmailErrorTo,
                        IsActive = s.IsActive,
                        PdfTemplateDesc = s.Desc,
                        PdfTemplateName = s.PdfTemplateName,
                        SuccessRate = 0
                    }).ToList();
                r.Subscription = context.UserSubscriptions.Where(c => c.UserId == user.UserId).Select(
                    c => new SubscriptionDto() { SubscriptionName = c.Subscription.SubscriptionName, SubscriptionEndDate = c.EndDate, SubscriptionStatus = c.Status }).ToList();
            }
            return r;
        }

        [HttpGet]
        public HttpResponseMessage EmailExists(string email)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    bool val = context.Users.Any(c => c.Email == email);
                    return SendHttpResponse(val, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public HttpResponseMessage Subscriptions()
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    var val = context.Subscriptions.Select(c => new { SubscriptionName = c.SubscriptionName, Price = c.Price }).ToList();
                    return SendHttpResponse(val, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage VerifyEmail(string email)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    var user = context.Users.FirstOrDefault(c => c.Email == email);
                    if (user != null)
                    {
                        user.EmailVerifiedDate = DateTime.Now;
                        context.SaveChanges();
                        return SendHttpResponse("Success", HttpStatusCode.OK);
                    }
                    else
                    {
                        return SendHttpResponse("Email Not Found", HttpStatusCode.InternalServerError);
                    }
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public HttpResponseMessage ResetPwdAndEmail(string email)
        {
            try
            {
                using (var context = new ToPDFDBContext())
                {
                    var user = context.Users.FirstOrDefault(c => c.Email == email);
                    if (user != null)
                    {
                        string p = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
                        user.Password = Convert.ToBase64String(Encoding.ASCII.GetBytes(p));
                        context.SaveChanges();
                        SendEmail(p, email);
                    }
                    return SendHttpResponse("Success", HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }
        #region Private Methods

        private static void handleObj(JObject currObj, Fields f1)
        {
            var l1 = ((JObject)currObj).Properties().Select(p => p.Name).ToList();
            foreach (var s1 in l1)
            {
                var f2 = new Fields();
                f2.FieldName = s1;
                f2.Children = new List<Fields>();
                if (currObj[s1].Type == JTokenType.String)
                {
                    f2.FieldValue = ((JValue)currObj[s1]).Value.ToString();
                }
                if (currObj[s1].Type == JTokenType.Object)
                {
                    handleObj((JObject)currObj[s1], f2);
                }
                if (currObj[s1].Type == JTokenType.Property)
                {
                    f2.FieldValue = ((JProperty)currObj[s1]).Value.ToString();
                }
                f1.Children.Add(f2);
            }
        }

        private void SendEmail(string pwd, string email)
        {

        }
        private string Encrypt(string input)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = Convert.FromBase64String("YC3t4aGXsyX1kw/MFlVcSMezIpyxbcUL");
            tripleDES.IV = ASCIIEncoding.ASCII.GetBytes("ToPDF_K");
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private HttpResponseMessage SendHttpResponse(object content, HttpStatusCode status)
        {
            var result = new HttpResponseMessage(status);
            var serializer = new JavaScriptSerializer();
            var contentStr = serializer.Serialize(content);
            result.Content = new StringContent(contentStr);
            return result;
        }

        #endregion
    }
    public class LoginRes
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public byte[] Avatar { get; set; }
    }
    public class Fields
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldPath { get; set; }
        public List<Fields> Children { get; set; }
    }
    public class Line
    {
        public bool IsLineBreak { get; set; }
        public bool IsLine { get; set; }
        public string Text { get; set; }
        public PdfFontBase Font { get; set; }
        public PdfBrush Brush { get; set; }
        public PdfStringFormat Format { get; set; }
        public SizeF Size
        {
            get
            {
                return this.Font.MeasureString(this.Text, this.Format);
            }
        }
    }
}
