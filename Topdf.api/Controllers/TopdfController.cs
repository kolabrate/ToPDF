using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using Topdf.api.Models;

namespace Topdf.api.Controllers
{
    public class TopdfController : ApiController
    {

        public List<string> NameList = new List<string>() { "AnandA", "Maran", "Priya", "Arjun" };

        //[HttpGet]
        //public IEnumerable<string> GetName()
        //{
        //    return NameList.AsEnumerable();
        //}

        //    #region Service Methods
        //    [HttpPost]
        //    public HttpResponseMessage PostPdf()
        //    {

        //        //Sample
        //        try
        //        {

        //            HttpContent requestContent = Request.Content;
        //            string xmlContent = requestContent.ReadAsStringAsync().Result;
        //            string dtStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        //            string returnStr = "" ;//CreatePDFFromXML(xmlContent);

        //            //Return Pdf file as stream content in the response
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //            result.Content = new StringContent(returnStr);
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        //            result.Content.Headers.ContentLength = returnStr.Length;
        //            return result;
        //        }
        //        catch (Exception e)
        //        {
        //            //Return Pdf file as stream content in the response
        //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //            result.Content = new StringContent(e.ToString());
        //            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/text");
        //            result.Content.Headers.ContentLength = e.ToString().Length;
        //            return result;
        //        }
        //    }

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
                return SendHttpResponse(ex.Message, HttpStatusCode.BadRequest);
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
                    if (user == null) return SendHttpResponse("Not Authenticated", HttpStatusCode.BadRequest);

                    string token = Encrypt(e + "|" + DateTime.Now.ToString("yyyyMMddHHmmssff"));
                    var lresult = new LoginRes() { UserId = user.UserId, Token = token, Avatar = user.Avatar };
                    return SendHttpResponse(lresult, HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.BadRequest);
            }
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
                return SendHttpResponse(e.Message, HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        public HttpResponseMessage EmailVerified(string email)
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
                        return SendHttpResponse("Email Not Found", HttpStatusCode.BadRequest);
                    }
                }
            }
            catch (Exception e)
            {
                return SendHttpResponse(e.Message, HttpStatusCode.BadRequest);
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
                return SendHttpResponse(e.Message, HttpStatusCode.BadRequest);
            }
        }

        //    #endregion

        #region Private Methods

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
}
