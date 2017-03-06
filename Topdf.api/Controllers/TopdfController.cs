using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Topdf.api.Controllers
{
    public class TopdfController : ApiController
    {

        public List<string> NameList = new List<string>() {"AnandA","Maran","Priya","Arjun"};

        [HttpGet]
        public IEnumerable<string> GetName()
        {
            return NameList.AsEnumerable();
        }

    }
}
