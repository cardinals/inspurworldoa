using InspurOA.DAL;
using InspurOA.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InspurOA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ////如果模型变化则重新创建数据库(会丢失所有数据)
            //Database.SetInitializer(new InspurDropCreateDatabaseIfModelChanges());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
