using InspurOA.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspurOA.Controllers
{
    public class BaseController : Controller
    {
        public new InspurPrincipal User { get; internal set; }
    }
}