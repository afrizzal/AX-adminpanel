using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using HRM.Models;

namespace HRM.Controllers
{
    public class HomeController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
        private string sSql = "";

        public class notifapp
        {
            public int notifqty { get; set; }
            public string notiftype { get; set; }
            public string notiftitle { get; set; }
            public string notifmsg { get; set; }
            public string notifaction { get; set; }
            public string notifctrl { get; set; }
            public string notifcss { get; set; }
            public string notiflink { get; set; }
        }

        [HttpPost]
        public ActionResult GetNotification(string userID)
        {
            List<notifapp> dtnotifapp = null;
            JsonResult js = null;

            try
            {
                sSql = "SELECT COUNT(DISTINCT a.oid) notifqty, 'Approval' notiftype, apppersonnote notiftitle, 'are waiting for your approval' notifmsg, 'Index' notifaction, 'WaitingAction' notifctrl, 'fa fa-envelope-o' notifcss, ('/' + a.tablename) notiflink FROM QL_approval a INNER JOIN QL_approvalperson ap ON a.tablename=ap.tablename WHERE a.approvaluser='" + userID + "' AND a.statusrequest='New' AND a.event='In Approval' GROUP BY apppersonnote, a.tablename ORDER BY apppersonnote";
                dtnotifapp = db.Database.SqlQuery<notifapp>(sSql).ToList();

                for (int i = 0; i < dtnotifapp.Count; i++)
                {
                    dtnotifapp[i].notiflink = Url.Action(dtnotifapp[i].notifaction + dtnotifapp[i].notiflink.ToLower(), dtnotifapp[i].notifctrl);
                    dtnotifapp[i].notifmsg = dtnotifapp[i].notifqty.ToString() + " " + dtnotifapp[i].notiftitle + " " + dtnotifapp[i].notifmsg;
                }
            }
            catch { }

            js = Json(dtnotifapp, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }

        [HttpPost]
        public ActionResult GetNotifPRNonPO()
        {
            var result = "";
            JsonResult js = null;
            List<string> tblcols = new List<string>();
            List<Dictionary<string, object>> tblrows = new List<Dictionary<string, object>>();

            try
            {
                List<RoleDetail> dtrole = (List<RoleDetail>)Session["Role"];
                List<RoleDetail> dtrolesel = dtrole.Where(r => r.formaddress.ToUpper() == "PRREPORT" && r.formmenu.ToUpper().StartsWith("REPORTSTATUS/")).ToList();
                if (dtrolesel.Count > 0)
                {
                    ReportModels.FullFormType formtype;
                    var totaldata = 1;
                    var UserID = Session["UserID"].ToString();
                    sSql = "SELECT * FROM (";
                    foreach (var item in dtrolesel)
                    {
                        formtype = new ReportModels.FullFormType(item.formmenu.ToUpper().Replace("REPORTSTATUS/", ""));
                        sSql += "SELECT ('ReportStatus/" + formtype.formtype + "/' + pr.cmpcode + '?proid=' + CAST(pr.pr" + formtype.reftype + "mstoid AS VARCHAR(10))) [Go], ISNULL((SELECT divname FROM QL_mstdivision di WHERE di.cmpcode=pr.cmpcode), '') [Business Unit], pr" + formtype.reftype + "no [PR No.], CONVERT(VARCHAR(10), pr" + formtype.reftype + "date, 101) [PR Date], ISNULL((SELECT deptname FROM QL_mstdept de WHERE de.cmpcode=pr.cmpcode AND de.deptoid=pr.deptoid), '') [Department], pr" + formtype.reftype + "mstnote [Note], ISNULL((SELECT COUNT(pr" + formtype.reftype + "dtloid) FROM QL_pr" + formtype.reftype + "dtl pd WHERE pr.cmpcode=pd.cmpcode AND pr.pr" + formtype.reftype + "mstoid=pd.pr" + formtype.reftype + "mstoid AND pr" + formtype.reftype + "dtlstatus<>'COMPLETE'), 0) [num_Outstanding] FROM QL_pr" + formtype.reftype + "mst pr WHERE pr.pr" + formtype.reftype + "mststatus='Approved' AND ";
                        if (Session["CompnyCode"].ToString() != CompnyCode)
                            sSql += "pr.cmpcode IN ('" + Session["CompnyCode"].ToString() + "')";
                        else
                            sSql += "pr.cmpcode LIKE '%'";
                        if (totaldata < dtrolesel.Count)
                        {
                            sSql += " UNION ALL ";
                            totaldata++;
                        }
                    }
                    sSql += ") tblPRNonPO ORDER BY [Business Unit], [PR No.]";
                    DataTable tbl = new ClassConnection().GetDataTable(sSql, "tblPRNonPO");
                    if (tbl.Rows.Count > 0)
                    {
                        Dictionary<string, object> row;
                        foreach (DataRow dr in tbl.Rows)
                        {
                            row = new Dictionary<string, object>();
                            foreach (DataColumn col in tbl.Columns)
                            {
                                var item = dr[col].ToString();
                                if (col.ColumnName == "Go")
                                {
                                    var tmp = item.Split('?');
                                    if (tmp.Length > 1)
                                        item = "<a href='" + Url.Action(tmp[0], "PRReport") + "?" + tmp[1] + "'>Go</a>";
                                    else
                                        item = "<a href='" + Url.Action(item, "PRReport") + "'>Go</a>";
                                }
                                row.Add(col.ColumnName, item);
                                if (!tblcols.Contains(col.ColumnName))
                                    tblcols.Add(col.ColumnName);
                            }
                            tblrows.Add(row);
                        }
                    }
                    else
                        result = "Data Not Found.";
                }
                else
                    result = "No Notif";
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            js = Json(new { result, tblcols, tblrows }, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "profile");
            return View();
        }
    }
}