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

namespace HRM.Controllers
{
    public class PromoController : Controller
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
            //if (ClassFunction.checkPagePermission(this.ControllerContext.RouteData.Values["controller"].ToString(), (List<RoleDetail>)Session["Role"]))
            //    return RedirectToAction("NotAuthorize", "Profile");
            return View(db.Promoes.ToList());
        }

        [HttpPost]
        public ActionResult GetDataList()
        {
            var msg = "";

            sSql = "SELECT ID, ID [NO], TITLE [TITLE], SUBTITLE [SUBTITLE], CONTEN [CONTENT] FROM Promo";
            //if (!ClassFunction.isSpecialAccess(this.ControllerContext.RouteData.Values["controller"].ToString(), (List<RoleSpecial>)Session["SpecialAccess"]))
            //    sSql += " AND createuser='" + Session["UserID"].ToString() + "'";

            //sSql += " ORDER BY ID ";

            if (msg == "")
            {
                DataTable tbl = new ClassConnection().GetDataTable(sSql, "Promo");

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
                                item = "<a class='text-primary' href='" + Url.Action("Form/" + dr["ID"].ToString(), "Promo") + "'>" + item + "</a>";
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

        // GET: Currency/Details/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promo qL_mstcurr = db.Promoes.Find(id);
            if (qL_mstcurr == null)
            {
                return HttpNotFound();
            }
            return View(qL_mstcurr);
        }

        // GET: Currency/Create
        public ActionResult Form(int? id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            //if (ClassFunction.checkPagePermission(this.ControllerContext.RouteData.Values["controller"].ToString(), (List<RoleDetail>)Session["Role"]))
            //    return RedirectToAction("NotAuthorize", "Profile");

            Promo Promo;
            string action = "New Data";
            if (id == null)
            {

                Promo = new Promo();
                Promo.CREATED_AT = ClassFunction.GetServerTime();
                //Promo.createtime = ClassFunction.GetServerTime();

            }
            else
            {
                action = "Update Data";
                Promo = db.Promoes.Find(id);

              
            }

            if (Promo == null)
            {
                return HttpNotFound();
            }

           
            ViewBag.action = action;
            return View(Promo);
        }

        // POST: Currency/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Form(Promo Promo, string action)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");
            //if (ClassFunction.checkPagePermission(this.ControllerContext.RouteData.Values["controller"].ToString(), (List<RoleDetail>)Session["Role"]))
            //    return RedirectToAction("NotAuthorize", "Profile");
            //qL_mstcurr.upduser = Session["UserID"].ToString();
            Promo.CREATED_AT = ClassFunction.GetServerTime();
            //if (qL_mstcurr.currencycode.Length > 10)
            //{
            //    ModelState.AddModelError("currencycode", "Currency Code can't be longer than 10 characters");
            //}
            //if (qL_mstcurr.currencydesc.Length > 100)
            //{
            //    ModelState.AddModelError("currencydesc", "Description can't be longer than 100 character");
            //}
            //if (!string.IsNullOrEmpty(qL_mstcurr.currencynote)) {
            //    if (qL_mstcurr.currencynote.Length > 100)
            //    {
            //        ModelState.AddModelError("currencynote", "Currency Note can't be longer than 100 character");
            //    }
            //}
            //int mstoid = db.QL_mstcurr.Any() ? db.QL_mstcurr.Max(o => o.currencyoid) + 1 : 1;
            sSql = "select top 1 max(ID) + 1 from Promo";
            var mstoid = db.Database.SqlQuery<Decimal>(sSql).FirstOrDefault();
            if (ModelState.IsValid)
            {
                using (var objTrans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (action == "New Data")
                        {

                            //qL_mstcurr.cmpcode = Session["CompnyCode"].ToString();
                            //qL_mstcurr.currencyoid = mstoid;
                            if (db.Promoes.Find(Promo.ID) != null)
                                Promo.ID = mstoid;
                            //qL_mstcurr.createuser = Session["UserID"].ToString();
                            //Promo.CREATED_AT = ClassFunction.GetServerTime();
                            //Promo.UPDATED_AT = Promo.CREATED_AT;
                            //db.QL_mstcurr.Add(qL_mstcurr);
                            sSql = "INSERT INTO Promo (TITLE, SUBTITLE, CREATED_AT, UPDATED_AT, CONTEN) VALUES('" + Promo.TITLE + "', '" + Promo.SUBTITLE + "', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, '" + Promo.CONTEN + "')";
                            db.Database.ExecuteSqlCommand(sSql);

                            //sSql = "UPDATE QL_mstoid SET lastoid=" + qL_mstcurr.currencyoid + " WHERE tablename='QL_MSTCURR' AND cmpcode='" + CompnyCode + "'";
                            //db.Database.ExecuteSqlCommand(sSql);
                            db.SaveChanges();
                        }
                        else
                        {
                            //db.Entry(qL_mstcurr).State = EntityState.Modified;
                            sSql = "UPDATE Promo SET TITLE='" + Promo.TITLE + "', SUBTITLE='" + Promo.SUBTITLE + "', CREATED_AT=CURRENT_TIMESTAMP, UPDATED_AT=CURRENT_TIMESTAMP, CONTEN='" + Promo.CONTEN + "' WHERE ID=" + Promo.ID + "";
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
            return View(Promo);
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
            Promo list = db.Promoes.Find(id);
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
                        sSql = "DELETE FROM Promo WHERE ID=" + list.ID;
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
        
        public bool IsInputValid(Promo Promo)
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
