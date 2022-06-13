using NavigaMagazin.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NavigaMagazin.Controllers
{
    public class PartialController : Controller
    {
        // GET: Partial
        navigaEntities db = new navigaEntities();
        public ActionResult menu()
        {
            var menu = (from s in db.CategoryTables
                        select s
                            ).ToList();
            return PartialView(menu);
        }
        public ActionResult Slider()
        {
            var slider = (from s in db.ContentTables
                          where s.isaktif == 1 && s.releasetime < DateTime.Now && s.approved == "true"
                          orderby s.releasetime descending
                          select s).Take(10);

            return PartialView(slider);
        }
        public ActionResult Showroom()
        {
            var showroom = (from s in db.ContentTables
                          where s.isaktif == 1 && s.releasetime < DateTime.Now && s.approved == "true" && s.postshowroom == "true"
                          orderby s.releasetime descending
                          select s).Take(9);

            return PartialView(showroom);
        }
        public ActionResult Sliderfirstpost()
        {
            var vitrin1 = db.ContentTables.Where(x => x.releasetime < DateTime.Now && x.isaktif == 1 && x.approved == "true").OrderBy(x => Guid.NewGuid()).Take(1);
            return PartialView(vitrin1);

        }
        public ActionResult Slidersecondpost()
        {
            var vitrin2 = db.ContentTables.Where(x => x.releasetime < DateTime.Now && x.isaktif == 1 && x.approved == "true").OrderBy(x => Guid.NewGuid()).Take(1);
            return PartialView(vitrin2);

        }
        public ActionResult Sidecontent()
        {
            return PartialView();
        }
        public ActionResult NavigaTV()
        {
            var navigatv = db.videoTables.Where(x => x.releasetime < DateTime.Now && x.isaktif == 1 && x.showroom == "true" ).OrderBy(x => Guid.NewGuid()).Take(8);
            return PartialView(navigatv);

        }
        public ActionResult PopularNews()
        {
            var populer = (from s in db.ContentTables
                          where s.isaktif == 1 && s.releasetime < DateTime.Now && s.approved == "true"
                           orderby s.postview descending
                          select s).Take(6);

            return PartialView(populer);
        }
        public ActionResult MotoryatNews()
        {
            var motoryat = (from s in db.ContentTables
                           where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Motoryat" && s.approved == "true"
                            orderby s.releasetime descending
                           select s).Take(4);

            return PartialView(motoryat);
        }
        public ActionResult YelkenliNews()
        {
            var yelkenli = (from s in db.ContentTables
                            where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Yelkenli" && s.approved == "true"
                            orderby s.releasetime descending
                            select s).Take(4);

            return PartialView(yelkenli);
        }
        public ActionResult TeknikNews()
        {
            var teknik = (from s in db.ContentTables
                            where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Teknik" && s.approved == "true"
                          orderby s.releasetime descending
                            select s).Take(4);

            return PartialView(teknik);
        }
        public ActionResult RibNews()
        {
            var rib = (from s in db.ContentTables
                          where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Rib" && s.approved == "true"
                          orderby s.releasetime descending
                          select s).Take(4);

            return PartialView(rib);
        }
        public ActionResult RotaNews()
        {
            var rota = (from s in db.ContentTables
                       where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Rota" && s.approved == "true"
                       orderby s.releasetime descending
                       select s).Take(4);

            return PartialView(rota);
        }      
        public ActionResult UrunlerNews()
        {
            var urunler = (from s in db.ContentTables
                           where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Urunler" && s.approved == "true"
                           orderby s.releasetime descending
                           select s).Take(4);

            return PartialView(urunler);
        }
        public ActionResult HaberlerNews()
        {
            var haberler = (from s in db.ContentTables
                           where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Haberler" && s.approved == "true"
                            orderby s.releasetime descending
                           select s).Take(4);

            return PartialView(haberler);
        }
        public ActionResult KuzineNews()
        {
            var kuzine = (from s in db.ContentTables
                            where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Kuzine" && s.approved == "true"
                          orderby s.releasetime descending
                            select s).Take(4);

            return PartialView(kuzine);
        }
        public ActionResult SidebarPopuler()
        {
            var siderbarpopuler = (from s in db.ContentTables
                          where s.isaktif == 1 && s.releasetime < DateTime.Now && s.approved == "true"
                                   orderby s.postview descending
                          select s).Take(5);

            return PartialView(siderbarpopuler);
        }
        public ActionResult SidebarLatest()
        {
            var sidebarlatest = (from s in db.ContentTables
                                   where s.isaktif == 1 && s.releasetime < DateTime.Now && s.approved == "true"
                                 orderby s.id descending
                                   select s).Take(5);

            return PartialView(sidebarlatest);
        }
        public ActionResult SiderbarCategories()
        {
            var categories = (from s in db.CategoryTables
                                   
                                   orderby s.id descending
                                   select s).ToList();

            return PartialView(categories);
        }
        public ActionResult Sliderfirstad()
        {
            var ad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertisingstart < DateTime.Now && x.advertisingend > DateTime.Now && x.advertising260x230 == "true" && x.fixedad != "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            
            if(ad != null)
            {
                ad.numberofview += 1;
                db.SaveChanges();
                return PartialView(ad);
            }
            else
            {
                var fixedad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertising260x230 == "true" && x.fixedad == "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();              
                if(fixedad != null)
                {
                    fixedad.numberofview += 1;
                }
                
                db.SaveChanges();
                return PartialView(fixedad);
            }
        }
        public ActionResult Slidersecondad()
        {
            var ad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertisingstart < DateTime.Now && x.advertisingend > DateTime.Now && x.advertising260x230 == "true" && x.fixedad != "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (ad != null)
            {
                ad.numberofview += 1;
                db.SaveChanges();
                return PartialView(ad);
            }
            else
            {
                var fixedad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertising260x230 == "true" && x.fixedad == "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (fixedad != null)
                {
                    fixedad.numberofview += 1;
                }

                db.SaveChanges();
                return PartialView(fixedad);
            }
        }
        public ActionResult ad728x90()
        {
            var ad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertisingstart < DateTime.Now && x.advertisingend > DateTime.Now && x.advertising728x90 == "true" && x.fixedad != "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (ad != null)
            {
                ad.numberofview += 1;
                db.SaveChanges();
                return PartialView(ad);
            }
            else
            {
                var fixedad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertising728x90 == "true" && x.fixedad == "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (fixedad != null)
                {
                    fixedad.numberofview += 1;
                }
                db.SaveChanges();
                return PartialView(fixedad);
            }
        }

        public ActionResult ad728x90banner()
        {
            var ad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertisingstart < DateTime.Now && x.advertisingend > DateTime.Now && x.advertising728x90banner == "true" && x.fixedad != "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (ad != null)
            {
                ad.numberofview += 1;
                db.SaveChanges();
                return PartialView(ad);
            }
            else
            {
                var fixedad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertising728x90banner == "true" && x.fixedad == "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (fixedad != null)
                {
                    fixedad.numberofview += 1;
                }
                db.SaveChanges();
                return PartialView(fixedad);
            }
        }
        public ActionResult ad320x270()
        {
            var ad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertisingstart < DateTime.Now && x.advertisingend > DateTime.Now && x.advertising320x270 == "true" && x.fixedad != "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (ad != null)
            {
                ad.numberofview += 1;
                db.SaveChanges();
                return PartialView(ad);
            }
            else
            {
                var fixedad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertising320x270 == "true" && x.fixedad == "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (fixedad != null)
                {
                    fixedad.numberofview += 1;
                }
                
                db.SaveChanges();
                return PartialView(fixedad);
            }
        }
        public ActionResult ad320x540()
        {
            var ad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertisingstart < DateTime.Now && x.advertisingend > DateTime.Now && x.advertising320x540 == "true" && x.fixedad != "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            if (ad != null)
            {
                ad.numberofview += 1;
                db.SaveChanges();
                return PartialView(ad);
            }
            else
            {
                var fixedad = db.AdvertisingTables.Where(x => x.isaktif == 1 && x.advertising320x540 == "true" && x.fixedad == "true").OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                if (fixedad != null)
                {
                    fixedad.numberofview += 1;
                }
               
                db.SaveChanges();
                return PartialView(fixedad);
            }
        }
        public ActionResult Trending()
        {
            var trend = (from s in db.ContentTables
                            where s.isaktif == 1 && s.releasetime < DateTime.Now && s.CategoryTable.MenuTable.menuname == "Haberler" && s.approved == "true"
                         orderby s.releasetime descending
                            select s).Take(5);

            return PartialView(trend);
        }
        public ActionResult TeknikKurul()
        {
            var userlist = (from s in db.UserTables
                            where s.isaktif == 1 && s.UserRoleTable.rolename == "Uzman"
                            orderby s.id ascending
                            select s).ToList();

            return PartialView(userlist);
        }
        public ActionResult StormCalender()
        {
            var storm = (from s in db.stormTables
                            where s.releasetime > DateTime.Now
                            orderby s.releasetime descending
                            select s).Take(5);
            return PartialView(storm);
        }
        public ActionResult EventCalendar()
        {
            var calender = (from s in db.calendarTables
                            where s.calendestartrdate > DateTime.Now
                            orderby s.calendestartrdate ascending
                            select s).Take(5);
            return PartialView(calender);
        }
        public ActionResult lastmagazine()
        {
            var magazine = (from s in db.magazineTables                        
                         select s).ToList();
            var lastmagazine = Enumerable.Reverse(magazine).Take(1).Reverse().ToList();
            return PartialView(lastmagazine);
        }
        public ActionResult ModalPage()
        {
            var modal = (from s in db.modalTables
                            where s.modalactive == "true"
                            select s).ToList();
            
            return PartialView(modal);
        }

        public ActionResult MoonCalendar()
        {
            var moon = (from s in db.moonCalendars
                         where s.moonTime > DateTime.Now
                         orderby s.moonTime descending
                         select s).Take(1);

            return PartialView(moon);
        }

        
    }
}