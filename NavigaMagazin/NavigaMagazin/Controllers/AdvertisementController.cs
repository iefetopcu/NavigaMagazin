using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Controllers
{
    public class AdvertisementController : BaseController
    {
        // GET: Advertisement
        navigaEntities db = new navigaEntities();
        public ActionResult Index()
        {
            ViewBag.ads = TempData["ads"];
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if (currentUser.UserRoleTable.rolename != "Admin")
            {
                return RedirectToAction("Index", "Admin");

            }
            else
            {
                var adlist = (from s in db.AdvertisingTables
                              where s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                              orderby s.createtime descending
                              select s).ToList();
                return View(adlist);
            }
            
        }

        public ActionResult DeleteAd(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var ad = db.AdvertisingTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Reklam Silindi, Silinen Reklam : " + ad.advertisingname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.AdvertisingTables.Remove(ad);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult UpdateAd(int id)
        {

            var ad = db.AdvertisingTables.Find(id);
            ViewBag.ads = TempData["ads"];
            return View("UpdateAd", ad);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateAd(AdvertisingTable p1, string advertisingformat)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var ad = db.AdvertisingTables.Find(p1.id);
            var stime = p1.advertisingstart;
            var etime = p1.advertisingend;
            if(p1.advertisingphoto != null)
            {
                var addphoto = Request.Files[0];
                var fileInfo = new FileInfo(addphoto.FileName);
                var pic = "AddPhoto" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/AdPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\AdPhotos\\" + pic);
                addphoto.SaveAs(tempFilePath);
                ad.advertisingphoto = filePath;
            }

            ad.advertisingname = p1.advertisingname;
            ad.advertisinglink = p1.advertisinglink;
            ad.advertisingstart = Convert.ToDateTime(stime);
            ad.advertisingend = Convert.ToDateTime(etime);
            ad.updatetime = DateTime.Now;
            ad.fixedad = p1.fixedad;
            ad.isaktif = p1.isaktif;

            if (advertisingformat == "260x230")
            {
                var control = (from s in db.AdvertisingTables
                               where s.advertising260x230 == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if (control >= 2 && p1.fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    ad.advertising260x230 = "true";
                    ad.advertising320x270 = null;
                    ad.advertising728x90 = null;
                    ad.advertising320x540 = null;
                    ad.advertising728x90banner = null;
                }
               
            }
            else if (advertisingformat == "320x270")
            {
                var control = (from s in db.AdvertisingTables
                               where s.advertising320x270 == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if (control >= 2 && p1.fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    ad.advertising260x230 = null;
                    ad.advertising320x270 = "true";
                    ad.advertising728x90 = null;
                    ad.advertising320x540 = null;
                    ad.advertising728x90banner = null;

                }
               
            }
            else if (advertisingformat == "728x90")
            {
                var control = (from s in db.AdvertisingTables
                               where s.advertising728x90 == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if (control >= 2 && p1.fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    ad.advertising260x230 = null;
                    ad.advertising320x270 = null;
                    ad.advertising728x90 = "true";
                    ad.advertising320x540 = null;
                    ad.advertising728x90banner = null;
                }
                
            }
            else if (advertisingformat == "728x90banner")
            {
                var control = (from s in db.AdvertisingTables
                               where s.advertising728x90banner == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if (control >= 2 && p1.fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    ad.advertising260x230 = null;
                    ad.advertising320x270 = null;
                    ad.advertising728x90 = null;
                    ad.advertising728x90banner = "true";
                    ad.advertising320x540 = null;
                }
                
            }
            else if (advertisingformat == "320x540")
            {
                var control = (from s in db.AdvertisingTables
                               where s.advertising320x540 == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if (control >= 2 && p1.fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else {
                    ad.advertising260x230 = null;
                    ad.advertising320x270 = null;
                    ad.advertising728x90 = null;
                    ad.advertising728x90banner = null;
                    ad.advertising320x540 = "true";
                }

            }

            var addlog = new ManualLog
            {
                logexp = "Reklam Güncellendi , Güncellenen Reklam : " + p1.advertisingname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };

            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult SearchAd(string searchkey)
        {
            var search = from x in db.AdvertisingTables
                         select x;
            if (!String.IsNullOrEmpty(searchkey))
            {
                search = search.Where(s => s.advertisingname.Contains(searchkey)          
                );
            }
            ViewBag.Words = searchkey;
            return View(search.ToList());
        }

    }
}