using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Models.Entity
{
    public class AdminPartialController : Controllers.BaseController
    {
        // GET: AdminPartial
        navigaEntities db = new navigaEntities();
        [HttpGet]
        public ActionResult UserAdd()
        {
            List<SelectListItem> UserRoleList = (from i in db.UserRoleTables.ToList()
                                             select new SelectListItem
                                             {
                                                 Text = i.rolename,
                                                 Value = i.id.ToString()

                                             }).ToList();
            ViewBag.roles = UserRoleList;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserAdd(string username, string usersurname, string userpassword, string useremail, string userphoto, int userroleid, string userfacebook, string userlinkedin, string usertwitter, string useryoutube)
        {
            var add = new UserTable
            {
                username = username,
                usersurname = usersurname,
                userpassword = userpassword,
                useremail = useremail,
                userroleid = userroleid,
                userfacebook = userfacebook,
                userlinkedin = userlinkedin,
                usertwitter = usertwitter,
                useryoutube = useryoutube,
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

            var addlog = new ManualLog
            {
                logexp = "Yeni Kullanıcı Eklendi, Eklenen kullanıcı :"+username+" "+usersurname,
                whodidit = "Sessionİsmi",
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.UserTables.Add(add);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        [HttpGet]
        public ActionResult CategoryAdd()
        {
            List<SelectListItem> MenuList = (from i in db.MenuTables.ToList()
                                                 select new SelectListItem
                                                 {
                                                     Text = i.menuname,
                                                     Value = i.id.ToString()

                                                 }).ToList();
            ViewBag.menus = MenuList;
            return PartialView();
        }
        [HttpPost]
        public ActionResult CategoryAdd(string categoryname, int menuid)
        {
            var add = new CategoryTable
            {
                categoryname = categoryname,
                menuid = menuid,
            };
            var addlog = new ManualLog
            {
                logexp = "Yeni Kategori Eklendi, Eklenen Kategori :" + categoryname,
                whodidit = "Sessionİsmi",
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.CategoryTables.Add(add);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult Categories()
        {
            var catlist = (from s in db.CategoryTables select s).ToList();
            return PartialView(catlist);
        }
        public ActionResult CategoryDelete(int id)
        {
            var cat = db.CategoryTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Kategori Silindi, Silinen Kategori : " + cat.categoryname ,
                whodidit = "Sessionİsmi",
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.CategoryTables.Remove(cat);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

        }
        public ActionResult VideoAdd()
        {
            
            return PartialView();
        }
        [HttpPost]
        public ActionResult VideoAdd(string videotitle, string videodescription, string releasetime, string videolink, string showroom)
        {
            string rtime = releasetime;
            //DateTime rtime = DateTime.ParseExact(releasetime, "yyyy/MM/dd", null);
            var add = new videoTable
            {
                videotitle = videotitle,
                videodescription = videodescription,
                isaktif = 1,
                videolink = videolink,
                createtime = DateTime.Now,
                updatetime = DateTime.Now,
                releasetime = Convert.ToDateTime(rtime),
                showroom = showroom,
            };
            var addlog = new ManualLog
            {
                logexp = "Yeni Video Eklendi, Eklenen Video :" + videotitle,
                whodidit = "Sessionİsmi",
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.videoTables.Add(add);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult AdAdd()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AdAdd(string advertisingname, string advertisinglink, string advertisingphoto, string advertisingformat, string advertisingstart, string advertisingend, string fixedad)
        {
            string stime = advertisingstart;
            string etime = advertisingend;
            var add = new AdvertisingTable
            {
                advertisingname = advertisingname,
                advertisinglink = advertisinglink,
                fixedad = fixedad,
                createtime = DateTime.Now,
                updatetime = DateTime.Now,
                numberofclicks = 1,
                isaktif = 1,
                numberofview = 1,
                advertisingstart = Convert.ToDateTime(stime),
                advertisingend = Convert.ToDateTime(etime),
            };
            if(advertisingformat == "260x230")
            {
               
                var control = (from s in db.AdvertisingTables
                               where s.advertising260x230 == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if(control >= 2 && fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    add.advertising260x230 = "true";
                }
                
            }
            else if(advertisingformat == "320x270")
            {
                var control1 = (from s in db.AdvertisingTables
                               where s.advertising320x270 == "true" &&
                               s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                               && s.fixedad != "true"
                               select s).Count();
                if (control1 >= 2 && fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    add.advertising320x270 = "true";
                }
                
            }
            else if(advertisingformat == "728x90")
            {
                var control2 = (from s in db.AdvertisingTables
                                where s.advertising728x90 == "true" &&
                                s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                                && s.fixedad != "true"
                                select s).Count();
                if (control2 >= 2 && fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    add.advertising728x90 = "true";
                }
            }
            else if (advertisingformat == "728x90banner")
            {
                var control2 = (from s in db.AdvertisingTables
                                where s.advertising728x90banner == "true" &&
                                s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                                && s.fixedad != "true"
                                select s).Count();
                if (control2 >= 2 && fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    add.advertising728x90banner = "true";
                }
            }
            else if (advertisingformat == "320x540")
            {
                var control3 = (from s in db.AdvertisingTables
                                where s.advertising320x540 == "true" &&
                                s.advertisingstart < DateTime.Now && s.advertisingend > DateTime.Now
                                && s.fixedad != "true"
                                select s).Count();
                if (control3 >= 2 && fixedad != "true")
                {
                    TempData["ads"] = string.Format("no");
                    return RedirectToAction("Index", "Advertisement");
                }
                else
                {
                    add.advertising320x540 = "true";
                }
                
            }
            if (advertisingphoto != null)
            {
                var addphoto = Request.Files[0];
                var fileInfo = new FileInfo(addphoto.FileName);
                var pic = "AddPhoto" + DateTime.Now.Ticks + fileInfo.Extension;
                var filePath = "/Photos/AdPhotos/" + pic;
                var tempFilePath = Server.MapPath("~\\Photos\\AdPhotos\\" + pic);
                addphoto.SaveAs(tempFilePath);
                add.advertisingphoto = filePath;
            }
            var addlog = new ManualLog
            {
                logexp = "Yeni Reklam Eklendi, Eklenen Reklam :" + advertisingname,
                whodidit = "Sessionİsmi",
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.AdvertisingTables.Add(add);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult FutureAd()
        {
            var adlist = (from s in db.AdvertisingTables
                          where s.advertisingstart > DateTime.Now && s.advertisingend > DateTime.Now
                          orderby s.createtime descending
                          select s).ToList();

            return PartialView(adlist);
        }

        public ActionResult ExpiredAd()
        {
            var adlist = (from s in db.AdvertisingTables
                          where s.advertisingstart < DateTime.Now && s.advertisingend < DateTime.Now
                          orderby s.createtime descending
                          select s).ToList();

            return PartialView(adlist);
        }

        public ActionResult Ebultenlist()
        {
            var ebulten = (from s in db.EbultenTables
                           orderby s.registertime descending
                           select s).Take(10);
            return PartialView(ebulten);
        }
        public ActionResult Contentlist()
        {
            var contentlist = (from s in db.ContentTables
                               where s.approved == "onhold"
                           orderby s.createtime descending
                           select s).Take(10);
            return PartialView(contentlist);
        }

        public ActionResult Userlist()
        {
            var userlist = (from s in db.UserTables
                           where s.approved == "onhold"
                           orderby s.creationtime descending 
                           select s).Take(10);
            return PartialView(userlist);
        }
        public ActionResult Commentlist()
        {
            var comment = (from s in db.CommentTables
                            orderby s.commenttime descending
                            select s).Take(10);
            return PartialView(comment);
        }

        public ActionResult QuestionList()
        {
            var question = (from s in db.questionTables
                           orderby s.questiontime descending
                           where s.isaktif == 2
                           select s).Take(10);
            return PartialView(question);
        }
        public ActionResult Uploaded()
        {

            var uploaded = (from s in db.UploadTables 
                            orderby s.id descending
                            select s).ToList();
            return PartialView(uploaded);
            
        }
    }
    
}