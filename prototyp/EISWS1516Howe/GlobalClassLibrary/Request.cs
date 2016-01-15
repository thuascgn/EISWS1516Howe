using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GlobalClassLibrary
{
    public class Request
    {
        public Request() { }

        public HttpResponseMessage PostSync(string baseUri, string resource, string content)
        {
            HttpResponseMessage httpResponse;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    httpResponse = client.PostAsync(resource, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return httpResponse;
        }
    }
}
