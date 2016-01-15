using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RuleService.Providers;
using GlobalClassLibrary;

namespace RuleService.Controllers
{
    public class DocumentsController : ApiController
    {

        HttpResponseMessage response = new HttpResponseMessage();
        HttpContent content;
        DocumentProvider documentProvider = new DocumentProvider();
        RuleProvider ruleProvider = new RuleProvider();
        Document document;
        Request req;

        // GET api/rules
        [HttpGet]
        [Route("api/documents")]
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
        [Route("api/documents/{id}")]
        public HttpResponseMessage Get(int id)
        {
            document = documentProvider.Read(id);

            if (document != null)
            {
                response.StatusCode = HttpStatusCode.OK;
                content = new StringContent(document.ToJson());
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
                content = new StringContent("Dokument für Id = " + id + " nicht gefunden.");
            }
            response.Content = content;
            return response;
        }

        #region Check
        /*
        // GET api/rules/check
        // [ActionName("check")]
        [HttpGet]
        [Route("api/rules/check")]
        public HttpResponseMessage Check([FromBody] Document document)
        {
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
        */
        #endregion

        // POST api/rules
        [HttpPost]
        [Route("api/documents")]
        public HttpResponseMessage Post([FromBody]Document document)
        {
            Rule rule = ruleProvider.Check(document);

            if (rule != null)
            {
                if (documentProvider.Export(document, rule))
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent("Document expotiert, " + document.Number);
                }
                else {
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.Content = new StringContent("Document nicht expotiert, " + document.Number);
                }

            }
            else
            {
                long insertedId = -1;
                //ruleProvider = new RuleProvider();
                insertedId = documentProvider.Create(document);

                if (insertedId > 0)
                {
                    document.Id = (int)insertedId;
                    Request req = new Request();
                    RuleTask ruleTask = new RuleTask(document);
                    HttpResponseMessage res = req.PostSync("http://localhost:55121", "/api/tasks", ruleTask.ToJson());

                    response.StatusCode = res.StatusCode;
                    response.Content = res.Content;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.Content = new StringContent("Rule NOT created. " + insertedId);
                }
            }
            return response;
        }

        // PUT api/values/5
        [HttpPut]
        [Route("api/documents/{id}")]
        public HttpResponseMessage Put(int id, [FromBody]Document document)
        {
            if (documentProvider.Update(document))
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent(document.ToJson());
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotModified;
                response.Content = new StringContent("RuleTask Not modified: " + document.ToString());
            }
            return response;
        }

        // DELETE api/values/5
        [HttpDelete]
        [Route("api/documents/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            if (documentProvider.Delete(id))
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
        [Route("api/documents")]
        public HttpResponseMessage Options()
        {
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("GET/POST/PUT/DELETE...");
            return response;
        }

    }
}
