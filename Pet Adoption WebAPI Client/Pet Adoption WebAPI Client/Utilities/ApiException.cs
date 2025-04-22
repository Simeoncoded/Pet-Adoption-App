using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Utilities
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }
        public ApiException(HttpResponseMessage response)
        {
            this.Response = response;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return this.Response.StatusCode;
            }
        }


        public IEnumerable<string> Errors
        {
            get
            {
                return this.Data.Values.Cast<string>().ToList();
            }
        }
    }
}
