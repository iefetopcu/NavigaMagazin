using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NavigaMagazin.Models.Entity;
using PagedList;
using System.Data.Entity;

namespace NavigaMagazin.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        navigaEntities db = new navigaEntities();
        public ActionResult Index(int page = 1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if(currentUser.UserRoleTable.rolename != "Admin")
            {
                return RedirectToAction("Index","Admin");
            }
            else
            {
                var userlist = (from s in db.UserTables select s).ToList().ToPagedList(page, 20);
                return View(userlist);
            }
            
        }
        public ActionResult UserDelete(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var user = db.UserTables.Find(id);         
            var addlog = new ManualLog
            {
                logexp = "Kullanıcı Silindi, Silinen kullanıcı : "+user.username+" "+user.usersurname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.UserTables.Remove(user);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

        }
        [HttpGet]
        public ActionResult UserUpdate(int id)
        {
            List<SelectListItem> UserRoleList = (from i in db.UserRoleTables.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.rolename,
                                                     Value = i.id.ToString()

                                                 }).ToList();
            ViewBag.roles = UserRoleList;
            var user = db.UserTables.Find(id);
            return View("UserUpdate", user);
        }
        [HttpPost]
        public ActionResult UserUpdate(UserTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var user = db.UserTables.Find(p1.id);
            user.username = p1.username;
            user.usersurname = p1.usersurname;
            user.useremail = p1.useremail;
            user.userpassword = p1.userpassword;
            user.userlinkedin = p1.userlinkedin;
            user.userfacebook = p1.userfacebook;
            user.usertwitter = p1.usertwitter;
            user.useryoutube = p1.useryoutube;
            user.userroleid = p1.userroleid;
            user.approved = p1.approved;
            user.updatetime = DateTime.Now;
            if(p1.userphoto != null)
            {
                var userphotofile = Request.Files[0];
                var fileInfo = new FileInfo(userphotofile.FileName);
                var pic = "User_Profil_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/UserPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\UserPhotos\\" + pic);
                userphotofile.SaveAs(tempFilePath);
                user.userphoto = filePath;
            }
            var addlog = new ManualLog
            {
                logexp = "Kullanıcı Güncellendi , Güncellenen kullanıcı :" + p1.username + " " + p1.usersurname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UserSearch(string searchkey)
        {
            var search = from x in db.UserTables
                         select x;
            if (!String.IsNullOrEmpty(searchkey))
            {
                search = search.Where(s => s.username.Contains(searchkey)
                || s.usersurname.Contains(searchkey) || s.useremail.Contains(searchkey)
                );
            }
            ViewBag.Words = searchkey;
            return View(search.ToList());
        }
        public ActionResult ApprovalRequest(int page = 1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if (currentUser.UserRoleTable.rolename != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var userlist = (from s in db.UserTables where s.approved == "onhold" select s).ToList().ToPagedList(page, 20);
                return View(userlist);
            }
        }
        public ActionResult Confirm(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var user = db.UserTables.Find(id);
            user.approved = "true";
            var addlog = new ManualLog
            {
                logexp = "Kullanıcı onaylandı, onaylanan kullanıcı : " + user.username + " " + user.usersurname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();          
            return RedirectToAction("ApprovalRequest","User");
        }
        public ActionResult Reject(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var user = db.UserTables.Find(id);
            user.approved = "false";
            var addlog = new ManualLog
            {
                logexp = "Kullanıcı reddedildi, reddedilen : " + user.username + " " + user.usersurname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();           
            return RedirectToAction("ApprovalRequest", "User");
        }
        public ActionResult ViewUser(int id)
        {
            ViewBag.usercomment = (from s in db.CommentTables
                                   where s.userid == id
                                   select s).Count();
            var user = db.UserTables.Include(a => a.CommentTables).FirstOrDefault(a => a.id == id);                   
            return View(user);
        }
    }
}