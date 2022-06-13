using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        navigaEntities db = new navigaEntities();
        public ActionResult Index()
        {

            ViewBag.user = (from s in db.UserTables                                     
                                       select s).Count();

            ViewBag.useractive = (from s in db.UserTables
                            where s.isaktif == 1
                            select s).Count();

            ViewBag.userpassive = (from s in db.UserTables
                            where s.isaktif == 2
                            select s).Count();

            ViewBag.content = (from s in db.ContentTables
                                   
                                   select s).Count();

            ViewBag.contentpublished = (from s in db.ContentTables
                                        where s.releasetime < DateTime.Now
                               select s).Count();

            ViewBag.contentwillbe = (from s in db.ContentTables
                                     where s.releasetime > DateTime.Now
                                     select s).Count();

            ViewBag.video = (from s in db.videoTables

                               select s).Count();

            ViewBag.videopublished = (from s in db.videoTables
                                        where s.releasetime < DateTime.Now
                                        select s).Count();

            ViewBag.videowillbe = (from s in db.videoTables
                                     where s.releasetime > DateTime.Now
                                     select s).Count();

            ViewBag.ebulten = (from s in db.EbultenTables
                                   
                                   select s).Count();
            ViewBag.adwillbe = (from s in db.AdvertisingTables
                                where s.advertisingstart > DateTime.Now && s.advertisingend > DateTime.Now
                                select s).Count();

            ViewBag.adactive = (from s in db.AdvertisingTables
                                where s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                                select s).Count();
            ViewBag.adtotal = (from s in db.AdvertisingTables
                                
                                select s).Count();

            return View();
        }
        public ActionResult Yorumlar()
        {
            var comment = (from s in db.CommentTables
                           orderby s.commenttime descending
                           where s.approved != "true"
                           select s).ToList();

            return PartialView(comment);
        }
        public ActionResult Sorular()
        {
            var sorular = (from s in db.questionTables
                           orderby s.questiontime descending          
                           select s).ToList();

            return View(sorular);
        }
        [HttpGet]
        public ActionResult SoruGir()
        {
            List<SelectListItem> expertlist = (from i in db.UserTables
                                             where i.UserRoleTable.rolename == "Uzman"
                                             select new SelectListItem
                                             {
                                                 Text = i.username + i.usersurname,
                                                 Value = i.id.ToString()
                                             }).ToList();

            List<SelectListItem> userlist = (from i in db.UserTables
                                             
                                             select new SelectListItem
                                             {
                                                 Text = i.username + i.usersurname,
                                                 Value = i.id.ToString()
                                             }).ToList();
            ViewBag.users = userlist;
            ViewBag.experts = expertlist;
            return View();
        }
        [HttpPost]
        public ActionResult SoruGir(int userid, int expertid, string questiontitle, string questioncontent, string questionanswer)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var addx = new questionTable
            {
                userid = userid,
                questioncontent = questioncontent,
                questiontitle = questiontitle,
                questiontime = DateTime.Now,
                expertid = expertid,
                questionanswer = questionanswer,
                isaktif = 1,
            };
            db.questionTables.Add(addx);
            var addlog = new ManualLog
            {
                logexp = "Soru Girildi , Girilen ,Soru : " + questiontitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();           
            return RedirectToAction("SoruGir","Admin");
        }
        public ActionResult DeleteComment(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var comment = db.CommentTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Yorum Silindi, Silinen yorum : " +  comment.commenttitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.CommentTables.Remove(comment);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult Approved(int id)
        {      
            var comment = db.CommentTables.Find(id);
            comment.approved = "true";
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult AnswerQuestion(int id)
        {
            List<SelectListItem> userlist = (from i in db.UserTables where i.UserRoleTable.rolename == "Uzman"                                                                                        
                                             select new SelectListItem
                                             {
                                                 Text = i.username + i.usersurname,
                                                 Value = i.id.ToString()
                                             }).ToList();         
            ViewBag.users = userlist;           
            var question = db.questionTables.Find(id);
            return View("AnswerQuestion", question);
        }
        [HttpPost]
        public ActionResult AnswerQuestion(questionTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var question = db.questionTables.Find(p1.id);         
            question.expertid = p1.expertid;
            question.questionanswer = p1.questionanswer;
            question.isaktif = 1;
            var addlog = new ManualLog
            {
                logexp = "Soru Güncellendi , Güncellenen Soru : " + question.questiontitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("Sorular","Admin");
        }
        public ActionResult DeleteQuestion(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var question = db.questionTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Soru Silindi , Silinen ,Soru : " + question.questiontitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.questionTables.Remove(question);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

        }

        public ActionResult MoonCalendar()
        {
            var moon = (from s in db.moonCalendars
                        orderby s.moonTime descending
                        select s).ToList();
            return View(moon);
        }
        [HttpPost]
        public ActionResult AddMoon(string moonPhotoUrl, string moonTime)
        {
            string rtime = moonTime;
            var add = new moonCalendar
            {
                moonTime = Convert.ToDateTime(rtime),
            };
            if (moonPhotoUrl != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "ContentPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/MoonPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\MoonPhotos\\" + pic);
                image.SaveAs(tempFilePath);
                add.moonPhotoUrl = filePath;
            }
            db.moonCalendars.Add(add);
            db.SaveChanges();
            return RedirectToAction("MoonCalendar","Admin");
        }
        public ActionResult DeleteMoon(int id)
        {
            var moon = db.moonCalendars.FirstOrDefault(a => a.id == id);
            db.moonCalendars.Remove(moon);
            db.SaveChanges();
            return RedirectToAction("MoonCalendar", "Admin");
        }
    }
}