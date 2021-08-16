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
using System.Text.RegularExpressions;

namespace HRM.Controllers
{
    public class ProfileController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
        private string BranchTrnCode = System.Configuration.ConfigurationManager.AppSettings["BranchTrnCode"];
        private string CompnyName = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
        private string sColSynchflag = System.Configuration.ConfigurationManager.AppSettings["Synchflag"];
        public string Title { get; set; }
        private string sSql = "";

        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.ReturnUrl = "Home";
            this.Title = CompnyName + " - Login";
            return View(new DataLogin());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(DataLogin clog, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                string userpassword = "Integra12345"/*db.QL_mstprof.Where(o => o.profoid.ToLower().Equals(clog.userid) && o.profstatus == "ACTIVE").Any() ? db.QL_mstprof.Where(o => o.profoid.ToLower().Equals(clog.userid) && o.profstatus == "ACTIVE").FirstOrDefault().profpass : ""*/;

                if (string.IsNullOrEmpty(userpassword))
                    ModelState.AddModelError("", "User ID doesn't exist, please contact administrator");
                else
                {
                    if (clog.userpwd.ToLower().Equals(userpassword.ToLower()))
                    {
                        SetUserSession(clog.userid);
                        //if (!string.IsNullOrEmpty(ReturnUrl))
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Incorrect Password");
                    }
                }
            }
            return View(clog);
        }

        private void SetUserSession(string userid)
        {
            //var tbl = db.QL_mstprof.Where(o => o.profoid.ToLower().Equals(userid.ToLower())).FirstOrDefault();
            Session["UserID"] = "admin";
            Session["UserName"] = "administrator";
            Session["ApprovalLimit"] = 0;
            string sSql = "";
            //if (/*tbl.proftype*/tbl.profname == "Admin")
            //    sSql = "SELECT menutype formtype, menuname formname, menucontroller formaddress, menumodule formmodule, menuorder formnumber, menuview formmenu, '' formimage FROM QL_formmenu WHERE menuflag='ACTIVE' ORDER BY menumodule, menutype, menuorder";
            //else
            //    sSql = "SELECT DISTINCT menutype formtype, menuname formname, menucontroller formaddress, menumodule formmodule, menuorder formnumber, menuview formmenu, '' formimage FROM QL_formuserrole ur INNER JOIN QL_formroledtl rd ON rd.roleoid=ur.roleoid INNER JOIN QL_formmenu m ON m.menuoid=rd.menuoid WHERE menuflag='ACTIVE' AND ur.profoid='" + tbl.profoid + "' ORDER BY menumodule, menutype, menuorder";
            sSql = "SELECT 'HIGHLIGHT' formtype, 'Promotion' formname, 'HighlightPromotion' AS formaddress, 'SETUP' AS formmodule, 0 AS formnumber, 'Index' AS formmenu, '' AS formimage UNION all SELECT 'REGULAR' formtype, 'Promo' formname, 'Promo' AS formaddress, 'SETUP' AS formmodule, 0 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'HIGHLIGHT' formtype, 'News' formname, 'HighlightNews' AS formaddress, 'SETUP' AS formmodule, 1 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'REGULAR' formtype, 'News' formname, 'News' AS formaddress, 'SETUP' AS formmodule, 1 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'HIGHLIGHT' formtype, 'Store' formname, 'HighlightStore' AS formaddress, 'SETUP' AS formmodule, 2 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'REGULAR' formtype, 'Store' formname, 'Store' AS formaddress, 'SETUP' AS formmodule, 2 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'HIGHLIGHT' formtype, 'Katalogues' formname, 'HighlightKatalogues' AS formaddress, 'SETUP' AS formmodule, 3 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'REGULAR' formtype, 'Katalogues' formname, 'Katalogues' AS formaddress, 'SETUP' AS formmodule, 3 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'REGULAR' formtype, 'FAQ' formname, 'FAQ' AS formaddress, 'SETUP' AS formmodule, 4 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'REGULAR' formtype, 'Benefit' formname, 'Benefit' AS formaddress, 'SETUP' AS formmodule, 5 AS formnumber, 'Index' AS formmenu, '' AS formimage union ALL SELECT 'REGULAR' formtype, 'Media' formname, 'Media' AS formaddress, 'SETUP' AS formmodule, 6 AS formnumber, 'Index' AS formmenu, '' AS formimage";
            Session["Role"] = db.Database.SqlQuery<RoleDetail>(sSql).ToList();
            List<RoleDetail> dtRoleTmp = db.Database.SqlQuery<RoleDetail>(sSql).ToList();
            List<RoleDetail> dtRole = new List<RoleDetail>();
            for (int i = 0; i < dtRoleTmp.Count; i++)
            {
                var pathcheck = dtRoleTmp[i].formaddress;
                if (dtRoleTmp[i].formtype.ToUpper() == "REPORT" || dtRoleTmp[i].formtype.ToUpper() == "ANALYST")
                    pathcheck = "ReportForm/" + dtRoleTmp[i].formaddress;
                var sfilepath = Server.MapPath("~/Controllers/" + pathcheck + "Controller.cs");
                if (System.IO.File.Exists(sfilepath))
                {
                    var item = new RoleDetail();
                    item.formtype = dtRoleTmp[i].formtype;
                    item.formname = dtRoleTmp[i].formname;
                    item.formaddress = dtRoleTmp[i].formaddress;
                    item.formmodule = dtRoleTmp[i].formmodule;
                    item.formnumber = dtRoleTmp[i].formnumber;
                    item.formmenu = dtRoleTmp[i].formmenu;
                    item.formimage = dtRoleTmp[i].formimage;
                    dtRole.Add(item);
                }
            }
            Session["Role"] = dtRole;
        }

        [HttpPost]
        public ActionResult LoginAgain(string userid, string password)
        {
            var result = "";
            JsonResult js = null;

            string userpassword = "Integra12345"/*db.QL_mstprof.Where(o => o.profoid.ToLower().Equals(userid) && o.profstatus == "ACTIVE").Any() ? db.QL_mstprof.Where(o => o.profoid.ToLower().Equals(userid) && o.profstatus == "ACTIVE").FirstOrDefault().profpass : ""*/;

            if (string.IsNullOrEmpty(userpassword))
                result = "User ID doesn't exist, please contact administrator";
            else
            {
                if (password.ToLower().Equals(userpassword.ToLower()))
                    SetUserSession(userid);
                else
                    result = "Incorrect Password";
            }

            js = Json(new { result }, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }
        
        public ActionResult IsSessionExpired()
        {
            return Json(Session["UserID"] == null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Profile");
        }

        public ActionResult Keepalive()
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotAuthorize()
        {
            return View();
        }
        
        public ActionResult Presensi()
        {
            return View();
        }


        public class mstuserrole
        {
            public int seq { get; set; }
            public int roleoid { get; set; }
            public string rolename { get; set; }
            public string roledesc { get; set; }
            public string special { get; set; }
        }

        private string GetQueryDDLRoleDetail(string stype)
        {
            return "SELECT genoid ifield, genother2 sfield FROM QL_mstgen WHERE cmpcode='" + CompnyCode + "' AND gengroup='MOBILE ROLE DETAIL' AND activeflag='ACTIVE' AND genother1=" + stype + " ORDER BY sfield";
        }

        private string GetQueryDDLDetail(string cmp, string stype, string stype_tx, string stypedtl_tx)
        {
            var sResult = "SELECT 0 ifield, '' sfield";
            int itype = int.Parse(stype);
            return sResult;
        }

        [HttpPost]
        public ActionResult InitDDLRoleDetail(string stype)
        {
            var result = "";
            JsonResult js = null;
            List<ReportModels.DDLDoubleField> tbl = new List<ReportModels.DDLDoubleField>();

            try
            {
                tbl = db.Database.SqlQuery<ReportModels.DDLDoubleField>(GetQueryDDLRoleDetail(stype)).ToList();
                if (tbl.Count == 0)
                    result = "Data Not Found.";
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            js = Json(new { result, tbl }, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }

        [HttpPost]
        public ActionResult InitDDLDetail(string cmp, string stype, string stype_tx, string stypedtl_tx)
        {
            var result = "";
            JsonResult js = null;
            List<ReportModels.DDLDoubleField> tbl = new List<ReportModels.DDLDoubleField>();

            try
            {
                tbl = db.Database.SqlQuery<ReportModels.DDLDoubleField>(GetQueryDDLDetail(cmp, stype, stype_tx, stypedtl_tx)).ToList();
                if (tbl.Count == 0)
                    result = "Data Not Found.";
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            js = Json(new { result, tbl }, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }
        

        [HttpPost]
        public ActionResult GetPersonData(string cmp, string id)
        {
            var result = "";
            JsonResult js = null;
            var cols = db.Database.SqlQuery<string>("SELECT (STUFF((SELECT DISTINCT ',' + 'p.' + name FROM sys.syscolumns WHERE id=OBJECT_ID('QL_mstperson') AND name<>'cardno' FOR XML PATH('')), 1, 1, '')) cols").FirstOrDefault();

            try
            {
                sSql = "SELECT " + cols + ", deptname cardno FROM QL_mstperson p INNER JOIN QL_mstdept d ON d.deptoid=p.deptoid WHERE p.cmpcode='" + cmp + "' AND p.personoid NOT IN (SELECT personoid FROM QL_mstprof WHERE profoid<>'" + id + "')";
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }

        [HttpPost]
        public ActionResult GetDataDetails(string id)
        {
            var result = "";
            JsonResult js = null;
            List<mstuserrole> tbl = new List<mstuserrole>();

            try
            {
                sSql = "SELECT 0 seq, roleoid, rolename, roledesc, 'No' special FROM QL_mstrole WHERE cmpcode='" + CompnyCode + "' AND roleactiveflag='ACTIVE' AND roleoid NOT IN (SELECT roleoid FROM QL_mstuserrole WHERE profoid='" + id + "' and deleteflag = '')";
                tbl = db.Database.SqlQuery<mstuserrole>(sSql).ToList();
                for (int i = 0; i < tbl.Count; i++)
                {
                    tbl[i].seq = i + 1;
                }
                if (tbl.Count == 0)
                    result = "Data Not Found.";
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            js = Json(new { result, tbl }, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }
        

        [HttpPost]
        public JsonResult SetDataDetails(List<mstuserrole> tbl)
        {
            Session["QL_mstuserrole"] = tbl;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult FillDetailData()
        {
            if (Session["QL_mstuserrole"] == null)
            {
                Session["QL_mstuserrole"] = new List<mstuserrole>();
            }

            List<mstuserrole> tbl = (List<mstuserrole>)Session["QL_mstuserrole"];
            return Json(tbl, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult GetDataList()
        {
            var msg = "";

            sSql = "SELECT p.cmpcode, profoid [User ID], profname [User Name], profstatus [Status], divname [Business Unit] FROM QL_mstprof p INNER JOIN QL_mstdivision div ON div.divcode=p.cmpcode WHERE 1=1 and p.deleteflag = ''";

            if (!ClassFunction.isSpecialAccess(this.ControllerContext.RouteData.Values["controller"].ToString(), (List<RoleSpecial>)Session["SpecialAccess"]))
                sSql += " AND createuser='" + Session["UserID"].ToString() + "'";

            sSql += " ORDER BY profoid, profstatus";

            if (msg == "")
            {
                DataTable tbl = new ClassConnection().GetDataTable(sSql, "QL_mstprof");

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
                            if (col.ColumnName == "User ID")
                            {
                                item = "<a class='text-primary' href='" + Url.Action("Form/" + dr["User ID"].ToString() + "/" + dr["cmpcode"].ToString(), "Profile") + "'>" + item + "</a>";
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
    }
}