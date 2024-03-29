﻿using System;
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
    public class HighlightStoreController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string sSql = "";


        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            return View(db.HighlightStores.ToList());
        }

        [HttpPost]
        public ActionResult GetDataList()
        {
            var msg = "";

            sSql = "SELECT ID, ID [NO], TITLE [TITLE], SUBTITLE [SUBTITLE], EMAIL [EMAIL], WORK_HOUR [WORK HOUR], INSTAGRAM [INSTAGRAM], FACEBOOK [FACEBOOK], YOUTUBE [YOUTUBE], WEBSITE [WEBSITE], TOKOPEDIA [TOKOPEDIA], DEKORUMA [DEKORUMA] FROM HighlightStore";

            if (msg == "")
            {
                DataTable tbl = new ClassConnection().GetDataTable(sSql, "HighlightStore");

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
                                item = "<a class='text-primary' href='" + Url.Action("Form/" + dr["ID"].ToString(), "HighlightStore") + "'>" + item + "</a>";
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
            HighlightStore qL_mstcurr = db.HighlightStores.Find(id);
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

            HighlightStore HighlightStore;
            string action = "New Data";
            if (id == null)
            {

                HighlightStore = new HighlightStore();
                HighlightStore.CREATED_AT = ClassFunction.GetServerTime();

            }
            else
            {
                action = "Update Data";
                HighlightStore = db.HighlightStores.Find(id);

              
            }

            if (HighlightStore == null)
            {
                return HttpNotFound();
            }

           
            ViewBag.action = action;
            return View(HighlightStore);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Form(HighlightStore HighlightStore, string action)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            HighlightStore.CREATED_AT = ClassFunction.GetServerTime();
            sSql = "select top 1 max(ID) + 1 from HighlightStore";
            var mstoid = db.Database.SqlQuery<Decimal>(sSql).FirstOrDefault();
            if (ModelState.IsValid)
            {
              
                using (var objTrans = db.Database.BeginTransaction())
                {
                    
                    try
                    {
                       
                      
                        if (action == "New Data")
                        {
                            sSql = "INSERT INTO HighlightStore (TITLE, SUBTITLE, CREATED_AT, UPDATED_AT) VALUES('" + HighlightStore.TITLE + "', '" + HighlightStore.SUBTITLE + "', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                            db.Database.ExecuteSqlCommand(sSql);
                            db.SaveChanges();
                        }
                        else
                        {
                            sSql = "UPDATE HighlightStore SET TITLE='" + HighlightStore.TITLE + "', SUBTITLE='" + HighlightStore.SUBTITLE + "', CREATED_AT=CURRENT_TIMESTAMP, UPDATED_AT=CURRENT_TIMESTAMP WHERE ID=" + HighlightStore.ID + "";
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
            return View(HighlightStore);
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
            HighlightStore list = db.HighlightStores.Find(id);
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
                        sSql = "DELETE FROM HighlightStore WHERE ID=" + list.ID;
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
        
        public bool IsInputValid(HighlightStore HighlightStore)
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
