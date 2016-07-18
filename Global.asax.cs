using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebAPI_FormsAuth.Infrastructure;

namespace WebAPI_FormsAuth
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var basicAuthMessageHandler = new WebAPI_FormsAuth.Helper.BasicAuthMessageHandler();
            //basicAuthMessageHandler.PrincipalProvider = new WebAPI_FormsAuth.Helper.adminPrincipalProvider();
            //start message handler
            //GlobalConfiguration.Configuration.MessageHandlers.Add(basicAuthMessageHandler);

            Database.SetInitializer<DataContext>(null);
        }
    }
}
