using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HRM.Models;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using CrystalDecisions.CrystalReports.Engine;

namespace HRM.Controllers
{
    public class FAQController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string sSql = "";


        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
                return View(db.FAQ.ToList());
        }

        [HttpPost]
        public ActionResult GetDataList()
        {
            var msg = "";

            sSql = "SELECT ID, ID [NO], QUESTION [QUESTION], ANSWER [ANSWER] FROM FAQ";
            if (msg == "")
            {
                DataTable tbl = new ClassConnection().GetDataTable(sSql, "FAQ");

                if (tbl.Rows.Count > 0)
                {
                    List<string> colname = new List<string>();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in tbl.Columns)
                        {
                            var item = dr[col].ToString();
                            if (col.ColumnName == "NO")
                            {
                                item = "<a class='text-primary' href='" + Url.Action("Form/" + dr["ID"].ToString(), "FAQ") + "'>" + item + "</a>";
                            }
                            row.Add(col.ColumnName, item);
                            if (!colname.Contains(col.ColumnName))
                                colname.Add(col.ColumnName);
                        }
                        rows.Add(row);
                    }

                    JsonResult js = Json(new { msg, colname, rows }, JsonRequestBehavior.AllowGet);
                    js.MaxJsonLength = Int32.MaxValue;
                    return js;
                }
                else
                    msg = "Data Not Found";
            }

            return Json(new { msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FAQ qL_mstcurr = db.FAQ.Find(id);
            if (qL_mstcurr == null)
            {
                return HttpNotFound();
            }
            return View(qL_mstcurr);
        }

        public ActionResult Form(int? id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            FAQ FAQ;
            string action = "New Data";
            if (id == null)
            {

                FAQ = new FAQ();
                FAQ.CREATED_AT = ClassFunction.GetServerTime();
            }
            else
            {
                action = "Update Data";
                FAQ = db.FAQ.Find(id);

              
            }

            if (FAQ == null)
            {
                return HttpNotFound();
            }

           
            ViewBag.action = action;
            return View(FAQ);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(Katalogue FAQ, string action)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            FAQ.CREATED_AT = ClassFunction.GetServerTime();
            sSql = "select top 1 max(ID) + 1 from FAQ";
            var mstoid = db.Database.SqlQuery<Decimal>(sSql).FirstOrDefault();
            if (ModelState.IsValid)
            {
              
                using (var objTrans = db.Database.BeginTransaction())
                {
                    
                    try
                    {
                       
                      
                        if (action == "New Data")
                        {

                            sSql = "INSERT INTO FAQ (TITLE, SUBTITLE, CREATED_AT, UPDATED_AT) VALUES('" + FAQ.TITLE + "', '" + FAQ.SUBTITLE + "', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                            db.Database.ExecuteSqlCommand(sSql);
                            db.SaveChanges();
                        }
                        else
                        {
                            sSql = "UPDATE FAQ SET TITLE='" + FAQ.TITLE + "', SUBTITLE='" + FAQ.SUBTITLE + "', CREATED_AT=CURRENT_TIMESTAMP, UPDATED_AT=CURRENT_TIMESTAMP WHERE ID=" + FAQ.ID + "";
                            db.Database.ExecuteSqlCommand(sSql);

                            db.SaveChanges();
                        }

                        objTrans.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        objTrans.Rollback();
                        ModelState.AddModelError("", ex.ToString());
                    }
                }

            }
            ViewBag.action = action;
            return View(FAQ);
        }

        public class oidusage
        {
            public string tblusage { get; set; }
            public string colusage { get; set; }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            FAQ list = db.FAQ.Find(id);
            var servertime = ClassFunction.GetServerTime();

            string result = "success";
            string msg = "";
            if (list == null)
            {
                result = "failed";
                msg = "Data can't be found!";
            }

            if (result == "success")
            {
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        sSql = "DELETE FROM FAQ WHERE ID=" + list.ID;
                        db.Database.ExecuteSqlCommand(sSql);
                        db.SaveChanges();

                        objTrans.Commit();
                    }
                    catch (Exception ex)
                    {
                        objTrans.Rollback();
                        result = "failed";
                        msg = ex.ToString();
                    }
                }
            }
            return Json(new { result, msg }, JsonRequestBehavior.AllowGet);
        }
        
        public bool IsInputValid(Katalogue FAQ)
        {
            bool isValid = true;

            if (!ModelState.IsValid)
            {
                isValid = false;

            }
            return isValid;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
