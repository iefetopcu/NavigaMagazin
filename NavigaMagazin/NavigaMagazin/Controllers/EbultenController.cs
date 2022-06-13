using NavigaMagazin.Models.Entity;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace NavigaMagazin.Controllers
{
    public class EbultenController : BaseController
    {
        // GET: Ebulten
        navigaEntities db = new navigaEntities();
        public ActionResult Index(int page = 1)
        {
            var ebulten = (from s in db.EbultenTables
                         orderby s.registertime descending
                         select s).ToList().ToPagedList(page, 20);
            return View(ebulten);
        }

        public ActionResult EbultenDelete(int id)
        {
            UserTable currentUser = (UserTable)Session["MySessionUser"];
            var ebulten = db.EbultenTables.Find(id);
            var addlog = new ManualLog
            {
                logexp = "Ebulten üyesi Silindi, Silinen Üye : " + ebulten.ebultenmail,
                whodidit = currentUser.username + " " + currentUser.usersurname,
                processingtime = DateTime.Now,
            };
            db.ManualLogs.Add(addlog);
            db.EbultenTables.Remove(ebulten);
            db.SaveChanges();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());

        }

        //public void ExcelExport()
        //{
        //    UserTable currentUser = (UserTable)Session["MySessionUser"];
        //    var ebultenlist = db.EbultenTables.ToList();

        //    ExcelPackage.LicenseContext = LicenseContext.Commercial;
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    ExcelPackage pck = new ExcelPackage();
        //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            
        //    ws.Cells["A1"].Value = "E-Mail";
        //    ws.Cells["B1"].Value = "Kayıt Tarihi";
        //    ws.Cells["C1"].Value = "Oluşturan Kullanıcı";
        //    ws.Cells["C2"].Value = currentUser.username + " " + currentUser.usersurname;
        //    int rowStart = 2;
        //    foreach (var item in ebultenlist)
        //    {
        //        ws.Cells[String.Format("A{0}", rowStart)].Value = item.ebultenmail;
        //        ws.Cells[String.Format("B{0}", rowStart)].Value = item.registertime.ToString();          
        //        rowStart++;
        //    }

        //    ws.Cells["A:AZ"].AutoFitColumns();
        //    Response.Clear();
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment; filename=" + "E-Bülten_Listesi" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ".xlsx");
        //    Response.BinaryWrite(pck.GetAsByteArray());
        //    Response.End();
        //}
    }
}