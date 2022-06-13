using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Controllers
{
    public class MessagesController : BaseController
    {
        // GET: Messages
        navigaEntities db = new navigaEntities();
        public ActionResult Index()
        {
            var messages = (from s in db.MessageTables
                         orderby s.messagetime descending
                         select s).ToList();
            return View(messages);
            
        }

        public ActionResult DeleteMessage(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var message = db.MessageTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Mesaj Silindi, Silinen Mesaj : " + message.messagesubject,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.MessageTables.Remove(message);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult ReadMessage(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var message = db.MessageTables.Find(id);
            message.messagestatu = 2;
            var addlog = new ManualLog
            {
                logexp = "Mesaj Okundu, Okunan Mesaj : " + message.messagesubject,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult UnreadMessage(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var message = db.MessageTables.Find(id);
            message.messagestatu = 1;
            var addlog = new ManualLog
            {
                logexp = "Mesaj Okunmadı olarak işaretlendi, İşaretlenen Mesaj : " + message.messagesubject,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
    }
}