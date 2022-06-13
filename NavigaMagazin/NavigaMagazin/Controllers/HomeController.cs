using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using NavigaMagazin.Helpers;
using PagedList;

namespace NavigaMagazin.Controllers
{
    public class HomeController : Controller
    {
        navigaEntities db = new navigaEntities();
        public ActionResult Index()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var modal = (from s in db.modalTables
                         where s.modalactive == "true"
                         select s).ToList();
            if(modal.Count > 0 )
            {
                ViewBag.Modal = "ok";
            }
            
           
            return View();
        }
        public ActionResult Oku(string url)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var post = db.ContentTables.Include(a => a.CommentTables).FirstOrDefault(a => a.url == url);              
            if (post == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            post.postview += 1;
            db.SaveChanges();
            return View("Oku", post);
        }
        [HttpPost]
        public ActionResult Comment(string commentcontent, string commenttitle, int postid)
        {
            
            UserTable currentUser = (UserTable)Session["MySessionUser"];

            var add = new CommentTable
            {
                postid = postid,
                commenttitle = commenttitle,
                userid = currentUser.id,
                commentcontent = commentcontent,
                commenttime = DateTime.Now,
            };
            if(currentUser != null)
            {
                if(currentUser.approved != "true")
                {
                    add.approved = "false";
                }
                else
                {
                    add.approved = "true";
                }
            }
            db.CommentTables.Add(add);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult Kullanici()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if(currentUser == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            var user = db.UserTables.Find(currentUser.id);
            ViewBag.approval = TempData["approvalrequest"];
            return View(user);
        }
        public ActionResult CommentDelete(int id)
        {

            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var comment = db.CommentTables.Find(id);
            if(currentUser.UserRoleTable.rolename == "Admin" || currentUser.UserRoleTable.rolename == "Editor")
            {
                db.CommentTables.Remove(comment);
                var addlog = new ManualLog
                {
                    logexp = "Yorum Silindi, Silinen Yorum : " + comment.commenttitle,
                    whodidit = currentUser.username +" " + currentUser.usersurname,
                    processingtime = DateTime.Now,
                };
                db.ManualLogs.Add(addlog);
                db.SaveChanges();
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

            }
            else
            {
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
           
            
        }
        public ActionResult ShowUser(int id)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var user = db.UserTables.Include(a => a.ContentTables).FirstOrDefault(a => a.id == id);          
            if(user == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            return View("ShowUser", user);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserAdd(string username, string usersurname, string userpassword, string useremail, string userphoto)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var userx = db.UserTables.FirstOrDefault(x => x.useremail == useremail);

            if(userx != null)
            {
                TempData["useradd"] = string.Format("no");
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            else
            {
                var add = new UserTable
                {
                    username = username,
                    usersurname = usersurname,
                    userpassword = userpassword,
                    useremail = useremail,
                    userroleid = 3,
                    isaktif = 1,
                    creationtime = DateTime.Now,
                    updatetime = DateTime.Now,
                };

                if (userphoto != null)
                {
                    var userphotofile = Request.Files[0];
                    var fileInfo = new FileInfo(userphotofile.FileName);
                    var pic = "User_Profil_" + DateTime.Now.Ticks + fileInfo.Extension;
                    var filePath = "/Photos/UserPhotos/" + pic;
                    var tempFilePath = Server.MapPath("~\\Photos\\UserPhotos\\" + pic);
                    userphotofile.SaveAs(tempFilePath);
                    add.userphoto = filePath;
                }
                db.UserTables.Add(add);
                TempData["useradd"] = string.Format("ok");
                db.SaveChanges();
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            
        }
        public ActionResult Ebulten(string ebultenmail)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var mail = db.EbultenTables.FirstOrDefault(x => x.ebultenmail == ebultenmail);
            if (mail != null)
            {
                TempData["ebulten"] = string.Format("no");
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }

            var add = new EbultenTable
            {
                ebultenmail = ebultenmail,
                registertime = DateTime.Now,

            };
            db.EbultenTables.Add(add);
            TempData["ebulten"] = string.Format("ok");
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpPost]
        public ActionResult Login(UserTable model)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var NavigaUser = db.UserTables.FirstOrDefault(x => x.useremail == model.useremail && x.userpassword == model.userpassword);
            if (NavigaUser != null)
            {
                Session["MySessionUser"] = NavigaUser;               
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            else
            {
                TempData["login"] = string.Format("no");
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
        }
        public ActionResult Logout()
        {
            Session.Remove("MySessionUser");
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index");
        }
        public void Admeter(int id)
        {
            var ad = db.AdvertisingTables.Find(id);
            ad.numberofclicks += 1;
            db.SaveChanges();

            string link = ad.advertisinglink;
            Response.Redirect(link, true);
        }
        public ActionResult Iletisim()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            ViewBag.contact = TempData["contact"];
            return View();
        }
        [HttpPost]
        public ActionResult Iletisim(string messagefullname, string messageemail, string messagesubject, string messagedescription)
        {
            var add = new MessageTable
            {
                messagefullname = messagefullname,
                messagedescription = messagedescription,
                messageemail = messageemail,
                messagesubject = messagesubject,
                messagetime = DateTime.Now,
                messagestatu = 1,
            };
            db.MessageTables.Add(add);
            db.SaveChanges();

            TempData["contact"] = string.Format("ok");
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpPost]
        public ActionResult Kullanici(UserTable p1)
        {        
            var user = db.UserTables.Find(p1.id);
            user.username = p1.username;
            user.usersurname = p1.usersurname;
            user.useremail = p1.useremail;
            user.userpassword = p1.userpassword;
            user.userlinkedin = p1.userlinkedin;
            user.userfacebook = p1.userfacebook;
            user.usertwitter = p1.usertwitter;
            user.useryoutube = p1.useryoutube;          
            user.updatetime = DateTime.Now;
            if (p1.userphoto != null)
            {
                var userphotofile = Request.Files[0];
                var fileInfo = new FileInfo(userphotofile.FileName);
                var pic = "User_Profil_" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/UserPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\UserPhotos\\" + pic);
                userphotofile.SaveAs(tempFilePath);
                user.userphoto = filePath;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult LikePost(int id) {
            var post = db.ContentTables.Find(id);
            post.postlike += 1;
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

        }
        public ActionResult DislikePost(int id)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var post = db.ContentTables.Find(id);
            post.postdislike += 1;
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult Bagis()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            return View();
        }
        public ActionResult Hakkimizda()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            return View();
        }
        public ActionResult Gizlilik()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            return View();
        }
        public ActionResult Arsiv()
        {
            return View();
        }
        public ActionResult Menu(int id)
        {
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.categoryid == id
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToList();           
            var menname = db.CategoryTables.Where(x => x.id == id).Select(x => x.categoryname).SingleOrDefault();
            var catname = db.CategoryTables.Where(x => x.id == id).Select(x => x.MenuTable.menuname).SingleOrDefault();          
            ViewBag.categoryid = id;
            ViewBag.categoryname = catname.ToString();
            ViewBag.menuname = menname.ToString();           
            return View(postlist);
        }
        public ActionResult Motoryat(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Motoryat" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Yelkenli(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Yelkenli" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToList().ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Teknik(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Teknik" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Urunler(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Urunler" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Haberler(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Haberler" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Kuzine(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Kuzine" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Rota(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Rota" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Rib(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.ContentTables
                            where s.isaktif == 1 && s.CategoryTable.MenuTable.menuname == "Rib" && s.releasetime < DateTime.Now
                            && s.releasetime < DateTime.Now
                            orderby s.releasetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
        public ActionResult Yazarlar()
        {

            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var userlist = (from s in db.UserTables
                            where s.isaktif == 1 && s.UserRoleTable.rolename == "Yazar"
                            orderby s.id ascending
                            select s).ToList();

            return View(userlist);
        }
        public ActionResult Ara(string searchkey)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var search = from x in db.ContentTables
                         select x;
            if (!String.IsNullOrEmpty(searchkey))
            {
                search = search.Where(s => s.posttitle.Contains(searchkey)
                || s.postcontent.Contains(searchkey) && s.isaktif == 1 && s.releasetime < DateTime.Now
                );
            }
            ViewBag.Words = searchkey;
            return View(search.ToList());
        }
        public ActionResult Hata()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            return View();
        }
        public ActionResult ApprovalRequest(int id)
        {

            var user = db.UserTables.Find(id);
            user.approved = "onhold";
            db.SaveChanges();
            TempData["approvalrequest"] = string.Format("ok");
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult ApprovalContent()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            if(currentUser != null)
            {
                List<SelectListItem> categorylist = (from i in db.CategoryTables.ToList()
                                                     select new SelectListItem
                                                     {
                                                         Text = i.categoryname + " - " + i.MenuTable.menuname,
                                                         Value = i.id.ToString()

                                                     }).ToList();
                ViewBag.categories = categorylist;
            }
            else
            {
                return RedirectToAction("Hata", "Home");
            }
            
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ApprovalContent(string posttitle, string postspottext, string postcontent, int categoryid, string postphoto)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var seourl = string.IsNullOrEmpty(posttitle.Trim()) ? Utility.UrlSeo(posttitle.Trim()) : Utility.UrlSeo(posttitle.Trim());        
            var add = new ContentTable
            {
                categoryid = categoryid,
                userid = currentUser.id,
                posttitle = posttitle,
                postspottext = postspottext,
                postcontent = postcontent,
                isaktif = 1,                
                postdislike = 0,
                postlike = 0,
                postview = 0,
                createtime = DateTime.Now,
                updatetime = DateTime.Now,
                url = seourl,
                approved = "onhold",
            };
            //Burada gelen dosya kontrolu yapılacak
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
            db.ContentTables.Add(add);
            db.SaveChanges();
            TempData["approvalrequest"] = string.Format("ok");
            return RedirectToAction("/Index");
        }
        public ActionResult SoruSor()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            ViewBag.question = TempData["question"];
            return View();
        }
        [HttpPost]
        public ActionResult SoruSor(string questiontitle , string questioncontent)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var addx = new questionTable
            {
                userid = currentUser.id,
                questioncontent = questioncontent,
                questiontitle = questiontitle,
                questiontime = DateTime.Now,
                isaktif = 2,            
            };
            db.questionTables.Add(addx);
            db.SaveChanges();
            TempData["question"] = string.Format("ok");
            return RedirectToAction("/SoruSor");
        }
        public ActionResult TeknikKurul()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var questionlist = (from s in db.questionTables
                            where s.isaktif == 1
                            orderby s.id descending
                            select s).ToList();

            return View(questionlist);
        }
        public ActionResult NavigaTV()
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var videolist = (from s in db.videoTables
                                where s.isaktif == 1 && s.releasetime < DateTime.Now
                                orderby s.releasetime descending
                                select s).ToList();
            
            return View(videolist);
        }
        public ActionResult AylikDergi(string url)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var magazine = db.magazineTables.FirstOrDefault(a => a.url == url);
            if (magazine == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            
            db.SaveChanges();
            return View("AylikDergi", magazine);
        }
        public ActionResult Etkinlik(string url)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var etkinlik = db.calendarTables.FirstOrDefault(a => a.url == url);
            if (etkinlik == null)
            {
                return RedirectToAction("Hata", "Home");
            }
            
            return View("Etkinlik", etkinlik);
        }
        public ActionResult Dergi(int page = 1)
        {
            ViewBag.user = TempData["useradd"];
            ViewBag.ebulten = TempData["ebulten"];
            ViewBag.login = TempData["login"];
            ViewBag.approvalrequest = TempData["approvalrequest"];
            var postlist = (from s in db.magazineTables                            
                            orderby s.magazinetime descending
                            select s).ToPagedList(page, 20);
            return View(postlist);
        }
    }

}