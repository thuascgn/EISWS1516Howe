using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MessageService.Models;

namespace MessageService.Controllers
{
    public class StatusController : ApiController
    {
        HttpResponseMessage response = new HttpResponseMessage();
        HttpContent content;

        // GET: api/status
        [HttpGet]
        [Route("api/status")]
        public string Get()
        {
            Status stat = new Status();
            stat.Read();
            return stat.ToString();
        }

        // GET: api/Status/department
        [HttpGet]
        [Route("api/status/{department}")]
        public HttpResponseMessage Get(string department)
        {
            Console.WriteLine("GET: api/status/{department} " + department);

            Status status = new Status();
            Int64 amount = status.CheckAmount(department);
            if (amount >= 0)
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Es stehen " + amount.ToString() + " Rechungen für Sie bereit.");
            }
            else {
                response.StatusCode = HttpStatusCode.NotFound;
                response.Content = new StringContent("Der Dokumentenstatus konnte für " + department + " nicht ermittelt werden.");
            }
            return response;
        }

        // POST: api/Status
        [HttpPost]
        [ActionName("priorize")]
        public string Post([FromBody]List<StatusUpdate> statusUpdateList)
        {
            Status status = new Status();
            status.Update(statusUpdateList);
            return "priorize success  ::  " + status.ToString();
        }

        // PUT: api/Status/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Status/5
        public void Delete(int id)
        {
        }
    }
}
