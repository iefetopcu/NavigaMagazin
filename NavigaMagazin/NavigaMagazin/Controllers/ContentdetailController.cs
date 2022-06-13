using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NavigaMagazin.Helpers;
using PagedList;

namespace NavigaMagazin.Controllers
{
    public class ContentdetailController : BaseController
    {
        // GET: Content
        navigaEntities db = new navigaEntities();
        public ActionResult Index(int page = 1)
        {
            var posts = (from s in db.ContentTables
                         orderby s.releasetime descending where s.approved == "true"
                         select s).ToList().ToPagedList(page, 20);         
            return View(posts);
        }
        [HttpGet]
        public ActionResult ContentAdd()
        {
            List<SelectListItem> userlist = (from i in db.UserTables.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.username + i.usersurname,
                                                 Value = i.id.ToString()

                                             }).ToList();
            List<SelectListItem> categorylist = (from i in db.CategoryTables.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.categoryname +" - " + i.MenuTable.menuname,
                                                     Value = i.id.ToString()

                                                 }).ToList();
            ViewBag.users = userlist;
            ViewBag.categories = categorylist;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ContentAdd(string posttitle, string postspottext, string postcontent, int userid, int categoryid, string postphoto, string releasetime, int isaktif, string postshowroom) {
            string rtime = releasetime;
            var seourl = string.IsNullOrEmpty(posttitle.Trim()) ? Utility.UrlSeo(posttitle.Trim()) : Utility.UrlSeo(posttitle.Trim());
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var add = new ContentTable
            {
                categoryid = categoryid,
                userid = userid,
                posttitle = posttitle,
                postspottext = postspottext,
                postcontent = postcontent,
                isaktif = isaktif,
                releasetime = Convert.ToDateTime(rtime),
                postdislike = 0,
                postlike = 0,
                postview = 0,
                createtime = DateTime.Now,
                updatetime = DateTime.Now,
                postshowroom = postshowroom,
                url = seourl,
                approved = "true",
            };
            if (postphoto != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "ContentPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/ContentPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\ContentPhotos\\" + pic);
                image.SaveAs(tempFilePath);
                add.postphoto = filePath;
            }
            var addlog = new ManualLog
            {
                logexp = "Yeni İçerik Eklendi , Eklenen İçerik : " + posttitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.ContentTables.Add(add);
            db.SaveChanges();

            return RedirectToAction("/Index");
        }
        public ActionResult DeleteContent(int id)
        {
            //var comments = db.comenttables.Where(c => c.postid == id);
            //db.comenttables.RemoveRange(comments);
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var contentdelete = db.ContentTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "İçerik Silindi, Silinen İçerik : " + contentdelete.posttitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.ContentTables.Remove(contentdelete);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult UpdateContent(int id)
        {
            List<SelectListItem> userlist = (from i in db.UserTables.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.username + i.usersurname,
                                                 Value = i.id.ToString()
                                             }).ToList();
            List<SelectListItem> categorylist = (from i in db.CategoryTables.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.categoryname + " - " + i.MenuTable.menuname,
                                                     Value = i.id.ToString()
                                                 }).ToList();
            ViewBag.users = userlist;
            ViewBag.categories = categorylist;
            var post = db.ContentTables.Find(id);
            return View("UpdateContent", post);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateContent(ContentTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var post = db.ContentTables.Find(p1.id);
            var zaman = p1.releasetime;
            var url = string.IsNullOrEmpty(p1.posttitle.Trim()) ? Utility.UrlSeo(p1.posttitle.Trim()) : Utility.UrlSeo(p1.posttitle.Trim());
            if(p1.postphoto != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "ContentPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/ContentPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\ContentPhotos\\" + pic);
                image.SaveAs(tempFilePath);
                post.postphoto = filePath;
            }
            post.postshowroom = p1.postshowroom;
            post.isaktif = p1.isaktif;
            post.categoryid = p1.categoryid;
            post.userid = p1.userid;
            post.posttitle = p1.posttitle;
            post.postspottext = p1.postspottext;
            post.postcontent = p1.postcontent;
            post.releasetime = Convert.ToDateTime(zaman);
            post.updatetime = DateTime.Now;
            post.url = url;
            var addlog = new ManualLog
            {
                logexp = "İçerik Güncellendi , Güncellenen İçerik : " + p1.posttitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SearchContent(string searchkey)
        {
            var search = from x in db.ContentTables
                         select x;
            if (!String.IsNullOrEmpty(searchkey))
            {
                search = search.Where(s => s.posttitle.Contains(searchkey)
                || s.postcontent.Contains(searchkey)
                );
            }
            ViewBag.Words = searchkey;
            return View(search.ToList());
        }
        [HttpGet]
        public ActionResult UpdateCat(int id)
        {
            List<SelectListItem> MenuList = (from i in db.MenuTables.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.menuname,
                                                 Value = i.id.ToString()

                                             }).ToList();
            ViewBag.menus = MenuList;
            var categorie = db.CategoryTables.Find(id);
            return View("UpdateCat", categorie);
        }
        [HttpPost]
        public ActionResult UpdateCat(CategoryTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var cat = db.CategoryTables.Find(p1.id);
            cat.categoryname = p1.categoryname;
            cat.menuid = p1.menuid;
            var addlog = new ManualLog
            {
                logexp = "Kategori Güncellendi , Güncellenene Kategori : " + p1.categoryname,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();

            return RedirectToAction("/Index");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddUpload(string uploadname, string uploadurl)
        {
            var add = new UploadTable
            {
                uploadname = uploadname,
            };

            if (uploadurl != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "ContentPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var copylink = "Photos/ContentPhotos/" + pic;
                var filePath = "/Photos/ContentPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\ContentPhotos\\" + pic);
                image.SaveAs(tempFilePath);
                add.uploadurl = filePath;
                add.uploadlink = copylink;
            }

            db.UploadTables.Add(add);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult UploadDelete(int id)
        {
            var upload = db.UploadTables.Find(id);
            db.UploadTables.Remove(upload);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult ApprovalRequest(int page = 1)
        {
            var posts = (from s in db.ContentTables
                         orderby s.releasetime descending
                         where s.approved == "onhold"
                         select s).ToList().ToPagedList(page, 20);
            return View(posts);
        }
        [HttpGet]
        public ActionResult ToBeApproved(int id)
        {
            List<SelectListItem> categorylist = (from i in db.CategoryTables.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.categoryname,
                                                     Value = i.id.ToString()
                                                 }).ToList();           
            ViewBag.categories = categorylist;
            var post = db.ContentTables.Find(id);
            return View("ToBeApproved", post);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ToBeApproved(ContentTable p1)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var post = db.ContentTables.Find(p1.id);
            var zaman = p1.releasetime;
            var url = string.IsNullOrEmpty(p1.posttitle.Trim()) ? Utility.UrlSeo(p1.posttitle.Trim()) : Utility.UrlSeo(p1.posttitle.Trim());
            if (p1.postphoto != null)
            {
                var image = Request.Files[0];
                if ((5 * 1024 * 1024) < image.ContentLength) //5MB
                    throw new Exception("hata mesaaji");
                var fileInfo = new FileInfo(image.FileName);
                var pic = "ContentPic_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/ContentPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\ContentPhotos\\" + pic);
                image.SaveAs(tempFilePath);
                post.postphoto = filePath;
            }
            post.isaktif = p1.isaktif;
            post.categoryid = p1.categoryid;
            post.posttitle = p1.posttitle;
            post.postspottext = p1.postspottext;
            post.postcontent = p1.postcontent;
            post.releasetime = Convert.ToDateTime(zaman);
            post.updatetime = DateTime.Now;
            post.url = url;
            post.approved = "true";
            var addlog = new ManualLog
            {
                logexp = "İçerik Onaylandı , Onaylandı İçerik : " + p1.posttitle,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.SaveChanges();
            return RedirectToAction("ApprovalRequest","Contentdetail");
        }
    }
}