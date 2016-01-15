using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace TaskService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web-API-Konfiguration und -Dienste
            // Web-API für die ausschließliche Verwendung von Trägertokenauthentifizierung konfigurieren.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web-API-Routen
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "GetTaskById",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "GetNextMessageByChannel",
            //    routeTemplate: "api/{controller}/{channel}/{action}",
            //    defaults: new { channel = RouteParameter.Optional  }
            //);
            //config.Routes.MapHttpRoute(
            //    name: "Priorize",
            //    routeTemplate: "api/{controller}/{action}"
            //, defaults: new { channel = RouteParameter.Optional }
            //);
        }
    }
}
