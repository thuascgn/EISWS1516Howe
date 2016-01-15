using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskService.Models;
using GlobalClassLibrary;
using TaskService.Providers;

namespace TaskService.Controllers
{
    public class TasksController : ApiController
    {
        HttpResponseMessage response = new HttpResponseMessage();
        HttpContent content;
        TaskProvider taskProvider = new TaskProvider();
        RuleTask ruleTask;

        // GET: api/task
        //  public IEnumerable<string> Get()
        [HttpGet]
        [Route("api/tasks")]
        public HttpResponseMessage Get()
        {
            //response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            content = new StringContent("Task Service Api v 0.1, Status:" + response.StatusCode.ToString());
            response.Content = content;

            return response;
        }

        // GET: 
        // api/task/channel/
        [HttpGet]
        [Route("api/tasks/{department}")]
        public HttpResponseMessage GetNext(string department)
        {
            //response = new HttpResponseMessage();
            taskProvider = new TaskProvider();
            ruleTask = taskProvider.Read(department);
            
            if (ruleTask != null)
            {
                response.StatusCode = HttpStatusCode.OK;
                content = new StringContent(ruleTask.ToJson());
            }
            else {
                response.StatusCode = HttpStatusCode.NotFound;
                content = new StringContent("RuleTask für Abteilung: " + department + " nicht gefunden." );
            }
            response.Content = content;
            return response;
        }


        // POST: api/task
        [HttpPost]
        [Route("api/tasks")]
        public HttpResponseMessage Post([FromBody]RuleTask task)
        {
            long insertedId = -1;
            taskProvider = new TaskProvider();

            if (String.IsNullOrEmpty(task.Department) || String.IsNullOrWhiteSpace(task.Department) ) {
                task.Department = "Verwaltung";
            }

            
            insertedId = taskProvider.Create(task);

            if (insertedId > 0)
            {
                response.StatusCode = HttpStatusCode.Created;
                response.Content = new StringContent("RuleTask created. Id: " + insertedId);
            } else {
                response.StatusCode = HttpStatusCode.Conflict;
                response.Content = new StringContent("RuleTask NOT created. " + insertedId);
            }
            return response;
        }

        // PUT: api/task/5
        //change to void after test
        [HttpPut]
        [Route("api/tasks/{id}")]
        public HttpResponseMessage Put(int id, [FromBody]RuleTask task)
        {
            TaskProvider tp = new TaskProvider();
            if (tp.Update(task))
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent(task.ToJson());
            } else {
                response.StatusCode = HttpStatusCode.NotModified;
                response.Content = new StringContent("RuleTask Not modified: " + task.ToString());
            }
            return response;
        }

        // DELETE: api/tasks/5
        //change to void after test
        [HttpDelete]
        [Route("api/tasks")]
        public HttpResponseMessage Delete(int id)
        {
            TaskProvider tp = new TaskProvider();
            if (tp.Delete(id)) {
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("RuleTask mit Id" + id + "gelöscht");
            } else {
                response.StatusCode = HttpStatusCode.NotModified;
                response.Content = new StringContent("RuleTask mit Id: " + id + "nicht gelöscht");
            }
            return response;
        }

        // OPtions
        [HttpOptions]
        [Route("api/tasks")]
        public HttpResponseMessage Options() {
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("GET/POST/PUT/DELETE...");
            return response;                
        }
    }
}

