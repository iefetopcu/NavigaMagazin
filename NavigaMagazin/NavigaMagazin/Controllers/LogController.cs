using NavigaMagazin.Models.Entity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Controllers
{
    public class LogController : BaseController
    {
        // GET: Log
        navigaEntities db = new navigaEntities();
        public ActionResult Index(int page = 1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if (currentUser.UserRoleTable.rolename != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var logs = (from s in db.ManualLogs orderby s.processingtime descending select s).ToList().ToPagedList(page, 50);
                return View(logs);
            }            
        }
        public ActionResult LogSearch(string searchkey)
        {
            var search = from x in db.ManualLogs                       
                         select x;
            if (!String.IsNullOrEmpty(searchkey))
            {
                search = search.Where(s => s.logexp.Contains(searchkey)
                || s.whodidit.Contains(searchkey)
                );
            }
            ViewBag.Words = searchkey;       
            return View(search.ToList());
        }
    }
}