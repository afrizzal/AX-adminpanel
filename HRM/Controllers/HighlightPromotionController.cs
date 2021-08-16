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
    public class HighlightPromotionController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
        private string BranchTrnCode = System.Configuration.ConfigurationManager.AppSettings["BranchTrnCode"];
        private string CompnyName = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
        private string sColSynchFlag = System.Configuration.ConfigurationManager.AppSettings["Synchflag"];
        private string sSql = "";


        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            return View(db.HighlightPromotion.ToList());
        }

        [HttpPost]
        public ActionResult GetDataList()
        {
            var msg = "";

            sSql = "SELECT ID, ID [NO], TITLE [TITLE], SUBTITLE [SUBTITLE], CONTEN [CONTENT] FROM HighlightPromotion";
            if (msg == "")
            {
                DataTable tbl = new ClassConnection().GetDataTable(sSql, "HighlightPromotion");

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
                                item = "<a class='text-primary' href='" + Url.Action("Form/" + dr["ID"].ToString(), "HighlightPromotion") + "'>" + item + "</a>";
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
            HighlightPromotion qL_mstcurr = db.HighlightPromotion.Find(id);
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
            HighlightPromotion HighlightPromotion;
            string action = "New Data";
            if (id == null)
            {

                HighlightPromotion = new HighlightPromotion();
                HighlightPromotion.CREATED_AT = ClassFunction.GetServerTime();

            }
            else
            {
                action = "Update Data";
                HighlightPromotion = db.HighlightPromotion.Find(id);

              
            }

            if (HighlightPromotion == null)
            {
                return HttpNotFound();
            }

           
            ViewBag.action = action;
            return View(HighlightPromotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Form(HighlightPromotion HighlightPromotion, string action)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            HighlightPromotion.CREATED_AT = ClassFunction.GetServerTime();
            sSql = "select top 1 max(ID) + 1 from HighlightPromotion";
            var mstoid = db.Database.SqlQuery<Decimal>(sSql).FirstOrDefault();
            if (ModelState.IsValid)
            {
              
                using (var objTrans = db.Database.BeginTransaction())
                {
                    
                    try
                    {
                       
                      
                        if (action == "New Data")
                        {
                            if (db.HighlightPromotion.Find(HighlightPromotion.ID) != null)
                                HighlightPromotion.ID = mstoid;
                            sSql = "INSERT INTO HighlightPromotion (TITLE, SUBTITLE, CREATED_AT, UPDATED_AT, CONTEN) VALUES('" + HighlightPromotion.TITLE + "', '" + HighlightPromotion.SUBTITLE + "', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, '" + HighlightPromotion.CONTEN + "')";
                            db.Database.ExecuteSqlCommand(sSql);
                            db.SaveChanges();
                        }
                        else
                        {
                            sSql = "UPDATE HighlightPromotion SET TITLE='" + HighlightPromotion.TITLE + "', SUBTITLE='" + HighlightPromotion.SUBTITLE + "', CREATED_AT=CURRENT_TIMESTAMP, UPDATED_AT=CURRENT_TIMESTAMP, CONTEN='" + HighlightPromotion.CONTEN + "' WHERE ID=" + HighlightPromotion.ID + "";
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
            return View(HighlightPromotion);
        }

        public class oidusage
        {
            public string tblusage { get; set; }
            [AllowHtml]
            public string colusage { get; set; }

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");

            HighlightPromotion list = db.HighlightPromotion.Find(id);
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
                        sSql = "DELETE FROM HighlightPromotion WHERE ID=" + list.ID;
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
        
        public bool IsInputValid(HighlightPromotion HighlightPromotion)
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
