using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskService;
using TaskService.Controllers;
using System.Net.Http;

namespace TaskService.Tests.Controllers
{
    [TestClass]
    class TaskControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Anordnen
            TasksController controller = new TasksController();

            // Aktion ausführen
            //ViewResult result = controller.Index() as ViewResult;

            HttpResponseMessage result = controller.Get();

            // Bestätigen
            Assert.IsNotNull(result);
            Console.WriteLine("content: " + result.Content.ToString() );
            Assert.AreEqual("task:{test: ok}", result.Content.ToString() );
            
        }
    }
}
