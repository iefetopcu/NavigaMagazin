using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NavigaMagazin.Models.Entity;

namespace NavigaMagazin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {          
            var session = (UserTable)filterContext.HttpContext.Session["MySessionUser"];
            if(session == null)
            {
                filterContext.Result = new RedirectResult("/");
                base.OnActionExecuting(filterContext);
            }
            else
            {
                if (session.UserRoleTable.rolename == "Admin" || session.UserRoleTable.rolename == "Editor")
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
                else
                {
                    filterContext.Result = new RedirectResult("/");
                    base.OnActionExecuting(filterContext);
                }
            }                       
        }
    }
}