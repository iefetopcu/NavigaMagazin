using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using NavigaMagazin.Helpers;
using System.IO;

namespace NavigaMagazin.Controllers
{
    public class SettingsController : BaseController
    {
        // GET: Settings
        navigaEntities db = new navigaEntities();
        public ActionResult Index()
        {

            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if (currentUser.UserRoleTable.rolename != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return View();
            }         
        }
        public ActionResult Calender(int page = 1)
        {
            var calender = (from s in db.calendarTables
                         orderby s.creationtime descending                       
                         select s).ToList().ToPagedList(page, 20);
            return View(calender);
        }
        public ActionResult CalenderAdd()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CalenderAdd(string calendartitle, string calendercontent, string calenderlocation , string calenderstartdate, string calenderenddate)
        {
            string stime = calenderstartdate;
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var seourl = string.IsNullOrEmpty(calendartitle.Trim()) ? Utility.UrlSeo(calendartitle.Trim()) : Utility.UrlSeo(calendartitle.Trim());
            var add = new calendarTable
            {
                calendartitle = calendartitle,
                calendercontent = calendercontent,
                calendestartrdate = Convert.ToDateTime(calenderstartdate),
                calenderenddate = Convert.ToDateTime(calenderenddate),
                calenderlocation = calenderlocation,
                creationtime = DateTime.Now,
                url = seourl,
            };
            var addlog = new ManualLog
            {
                logexp = "Yeni Etkinlik Eklendi , Eklenen Etkinlik : " + calendartitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.calendarTables.Add(add);
            db.SaveChanges();
            return RedirectToAction("Calender","Settings");
        }
        public ActionResult DeleteCalender(int id)
        {        
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var calender = db.calendarTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Etkinlik Silindi, Silinen Etkinlik : " + calender.calendartitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.calendarTables.Remove(calender);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult UpdateCalender(int id)
        {
            var calender = db.calendarTables.Find(id);
            return View("UpdateCalender", calender);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateCalender(calendarTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var calender = db.calendarTables.Find(p1.id);
            var url = string.IsNullOrEmpty(p1.calendartitle.Trim()) ? Utility.UrlSeo(p1.calendartitle.Trim()) : Utility.UrlSeo(p1.calendartitle.Trim());
            calender.calendartitle = p1.calendartitle;
            calender.calendercontent = p1.calendercontent;
            calender.calendestartrdate = Convert.ToDateTime(p1.calendestartrdate);
            calender.calenderenddate = Convert.ToDateTime(p1.calenderenddate);
            calender.calenderlocation = p1.calenderlocation;
            var addlog = new ManualLog
            {
                logexp = "Etkinlik Güncellendi , Güncellenen Etkinlik : " + p1.calendartitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("Calender","Settings");
        }
        public ActionResult StormCalender()
        {
            var calenderstorm = (from s in db.stormTables
                            orderby s.releasetime descending
                            select s).ToList();
            return View(calenderstorm);
        }
        [HttpPost]
        public ActionResult StormCalenderAdd(string stormname, string releasetime)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            string rtime = releasetime;
            var add = new stormTable
            {
                stormname = stormname,
                releasetime = Convert.ToDateTime(rtime)
            };
            db.stormTables.Add(add);
            var addlog = new ManualLog
            {
                logexp = "Fırtına Takvimi Eklendi, Eklenen : " + stormname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult DeleteStorm(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var storm = db.stormTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Fırtına Takvimi Silindi, Silinen Takvim : " + storm.stormname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.stormTables.Remove(storm);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult Modal()
        {
            ViewBag.modal = TempData["modal"];
            var modallist = (from s in db.modalTables
                                 orderby s.id ascending
                                 select s).ToList();
            return View(modallist);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ModalAdd(string modallink, string modalphoto,string modalactive)
        {

            var modal = (from s in db.modalTables
                         where s.modalactive == "true"
                         select s).FirstOrDefault();
            if ( modal != null)
            {
                TempData["modal"] = string.Format("no");
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            else
            {
                var add = new modalTable
                {
                    modallink = modallink,
                    modalactive = modalactive,
                };
                if (modalphoto != null)
                {
                    var image = Request.Files[0];
                    if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                        throw new Exception("hata mesaaji");
                    var fileInfo = new FileInfo(image.FileName);
                    var pic = "ModalPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                    var filePath = "/Photos/Modals/" + pic;
                    var tempFilePath = Server.MapPath("~\\Photos\\Modals\\" + pic);
                    image.SaveAs(tempFilePath);
                    add.modalphoto = filePath;
                }
                db.modalTables.Add(add);
                db.SaveChanges();
                return RedirectToAction("Modal", "Settings");
            }
            
        }
        public ActionResult ModalUpdate(int id)
        {

            var modal = (from s in db.modalTables
                         where s.id == id
                         select s).FirstOrDefault();
            return View(modal);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ModalUpdate(modalTable p1)
        {
            var modal = db.modalTables.Find(p1.id);
            modal.modalactive = p1.modalactive;
            modal.modallink = p1.modallink;
            if (p1.modalphoto != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "ModalPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/Modals/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\Modals\\" + pic);
                image.SaveAs(tempFilePath);
                modal.modalphoto = filePath;
            }
            db.SaveChanges();
            return RedirectToAction("Modal", "Settings");
        }
        public ActionResult ModalDelete(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var modal = db.modalTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Modal Silindi, Silinen Modal : " + modal.modallink,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.modalTables.Remove(modal);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult Magazine(int page = 1)
        {
            var magazine = (from s in db.magazineTables
                            orderby s.magazinetime descending
                            select s).ToList().ToPagedList(page, 20);
            return View(magazine);
        }
        [HttpGet]
        public ActionResult MagazineAdd()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MagazineAdd(string coverphoto, string magazinetitle, string magazinecontent, string magazinetime)
        {
            string rtime = magazinetime;
            var seourl = string.IsNullOrEmpty(magazinetitle.Trim()) ? Utility.UrlSeo(magazinetitle.Trim()) : Utility.UrlSeo(magazinetitle.Trim());
            UserTable currentUser = (UserTable)Session["MySessionUser"];

            var add = new magazineTable
            {
                magazinetitle = magazinetitle,
                magazinecontent = magazinecontent,
                magazinetime = Convert.ToDateTime(rtime),
                creationtime = DateTime.Now,
                url = seourl,
            };
            if (coverphoto != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "MagazinePic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/Magazine/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\Magazine\\" + pic);
                image.SaveAs(tempFilePath);
                add.coverphoto = filePath;
            }
            var addlog = new ManualLog
            {
                logexp = "Yeni İçerik Eklendi , Eklenen İçerik : " + magazinetitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.magazineTables.Add(add);
            db.SaveChanges();

            return RedirectToAction("Magazine","Settings");
        }

        public ActionResult UpdateMagazine(int id)
        {
            var magazine = db.magazineTables.Find(id);

            return View(magazine);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateMagazine(magazineTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var magazine = db.magazineTables.Find(p1.id);
            var zaman = p1.magazinetime;
            var url = string.IsNullOrEmpty(p1.magazinetitle.Trim()) ? Utility.UrlSeo(p1.magazinetitle.Trim()) : Utility.UrlSeo(p1.magazinetitle.Trim());
            if (p1.coverphoto != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "MagazinePic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/Magazine/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\Magazine\\" + pic);
                image.SaveAs(tempFilePath);
                magazine.coverphoto = filePath;
            }
            magazine.magazinetitle = p1.magazinetitle;
            magazine.magazinecontent = p1.magazinetitle;
            magazine.magazinetime = Convert.ToDateTime(zaman);
            magazine.url = url;
            var addlog = new ManualLog
            {
                logexp = "Aylık Dergi Güncellendi , Güncellenen Dergi : " + p1.magazinetitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("Magazine","Settings");
        }

        public ActionResult DeleteMagazine(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var magazine = db.magazineTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Dergi Silindi, Silinen Dergi : " + magazine.magazinetitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.magazineTables.Remove(magazine);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

    }
}