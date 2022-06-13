using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Controllers
{
    public class VideoController : BaseController
    {
        // GET: Video
        navigaEntities db = new navigaEntities();
        public ActionResult Index()
        {
            var videos = (from s in db.videoTables
                         orderby s.releasetime descending
                         select s).ToList();
            return View(videos);
        }

        public ActionResult VideoDelete(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var video = db.videoTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Video Silindi, Silinen Video : " + video.videotitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.videoTables.Remove(video);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult VideoUpdate(int id)
        {
            var video = db.videoTables.Find(id);
            return View("VideoUpdate", video);
        }
        [HttpPost]
        public ActionResult VideoUpdate(videoTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var video = db.videoTables.Find(p1.id);
            var zaman = p1.releasetime;
            video.videotitle = p1.videotitle;
            video.videodescription = p1.videodescription;
            video.videolink = p1.videolink;
            video.showroom = p1.showroom;
            video.updatetime = DateTime.Now;
            video.releasetime = Convert.ToDateTime(zaman);

            var addlog = new ManualLog
            {
                logexp = "Video Güncellendi, Güncellenen Video : " + video.videotitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);

            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult SearchVideo(string searchkey)
        {
            var search = from x in db.videoTables
                         select x;
            if (!String.IsNullOrEmpty(searchkey))
            {
                search = search.Where(s => s.videotitle.Contains(searchkey)
                || s.videodescription.Contains(searchkey)
                );
            }
            ViewBag.Words = searchkey;
            return View(search.ToList());
        }
    }
}