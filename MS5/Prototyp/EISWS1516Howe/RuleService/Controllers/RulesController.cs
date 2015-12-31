using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using RuleService.Models;
using RuleService.Providers;
using GlobalClassLibrary;

namespace RuleService.Controllers
{
    public class RulesController : ApiController
    {
        HttpResponseMessage response = new HttpResponseMessage();
        HttpContent content;
        RuleProvider ruleProvider = new RuleProvider();
        Rule rule;

        // GET api/rules
        [HttpGet]
        [Route("api/rules")]
        public HttpResponseMessage Get()
        {
            //response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            content = new StringContent("RuleService Service Api v 0.1, Status:" + response.StatusCode.ToString());
            response.Content = content;

            return response;
        }

        // GET api/rules/5
        [HttpGet]
        [Route("api/rules/{id}")]
        public HttpResponseMessage Get(int id)
        {
            rule = ruleProvider.Read(id);

            if (rule != null)
            {
                response.StatusCode = HttpStatusCode.OK;
                content = new StringContent(rule.ToJson());
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
                content = new StringContent("RuleTask für Id = " + id + " nicht gefunden.");
            }
            response.Content = content;
            return response;
        }

        // GET api/rules/check
        // [ActionName("check")]
        [HttpGet]
        [Route("api/rules/check")]
        public HttpResponseMessage Check([FromBody] Document document) {
            rule = ruleProvider.Check(document);

            if (rule != null)
            {
                response.StatusCode = HttpStatusCode.OK;
                content = new StringContent(rule.ToJson());
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
                content = new StringContent("Rule für Dokumenht: = " + document.ToString() + " nicht gefunden.");
            }
            response.Content = content;
            return response;

        }

        // POST api/rules
        [HttpPost]
        [Route("api/rules")]
        public HttpResponseMessage Post([FromBody]Rule rule)
        {
            long insertedId = -1;
            //ruleProvider = new RuleProvider();
            insertedId = ruleProvider.Create(rule);

            if (insertedId > 0)
            {
                response.StatusCode = HttpStatusCode.Created;
                response.Content = new StringContent("Rule created. Id: " + insertedId);
            }
            else
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.Content = new StringContent("Rule NOT created. " + insertedId);
            }
            return response;
        }

        // PUT api/values/5
        [HttpPut]
        [Route("api/rules/{id}")]
        public HttpResponseMessage Put(int id, [FromBody]Rule rule)
        {
            if (ruleProvider.Update(rule))
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent(rule.ToJson());
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotModified;
                response.Content = new StringContent("RuleTask Not modified: " + rule.ToString());
            }
            return response;
        }

        // DELETE api/values/5
        [HttpDelete]
        [Route("api/rules/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            if (ruleProvider.Delete(id))
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("Rule mit Id" + id + "gelöscht");
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotModified;
                response.Content = new StringContent("Rule mit Id: " + id + "nicht gelöscht");
            }

            return response;
        }

        [HttpOptions]
        [Route("api/rules")]
        public HttpResponseMessage Options()
        {
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("GET/POST/PUT/DELETE...");
            return response;
        }
    }
}
