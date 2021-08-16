using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRM.Models;

namespace HRM.Controllers
{
    public class DashboardController : Controller
    {
        private HRMEntities db = new HRMEntities();
        private string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
        private string sSql = "";
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Login", "Profile");

            ViewBag.StartPeriod = ClassFunction.GetServerTime().ToString("MM/01/yyyy");
            ViewBag.EndPeriod = ClassFunction.GetServerTime().ToString("MM/dd/yyyy");
            List<SelectListItem> DDLYear = new List<SelectListItem>();
            int start = DateTime.Today.Year;
            int end = 2013;
            for (int i = start; i >= end; i--)
            {
                var item = new SelectListItem();
                item.Text = i.ToString().ToUpper();
                item.Value = i.ToString();
                DDLYear.Add(item);
            }
            ViewBag.FilterYearAvgInventRM = DDLYear;
            ViewBag.FilterYearAvgInventKY = DDLYear;
            return View();
        }

        public class dbshipmentmodel
        {
            public string divname { get; set; }
            public decimal totalshipment { get; set; }
        }

        public ActionResult GetDataDashboardShipment()
        {
            var startperiod = ClassFunction.GetServerTime().ToString("01/01/yyyy") + " 00:00:00";
            var endperiod = ClassFunction.GetServerTime().ToString("MM/dd/yyyy") + " 23:59:59";

            sSql = "SELECT divname, SUM(totalshipment) totalshipment FROM (";
            sSql += "SELECT arm.cmpcode, (SELECT divname FROM QL_mstdivision div WHERE div.cmpcode=arm.cmpcode) divname, (SELECT groupcode + ' - ' + groupdesc FROM QL_mstdeptgroup dgm WHERE dgm.cmpcode=arm.cmpcode AND dgm.groupoid=som.groupoid) groupdesc, (SELECT currcode FROM QL_mstcurr c WHERE c.curroid=arm.curroid) currcode, SUM(aritemdtlnetto) totalshipment FROM QL_trnaritemmst arm INNER JOIN QL_trnaritemdtl ard ON ard.cmpcode=arm.cmpcode AND ard.aritemmstoid=arm.aritemmstoid INNER JOIN QL_trnshipmentitemdtl shd ON shd.cmpcode=ard.cmpcode AND shd.shipmentitemdtloid=ard.shipmentitemdtloid INNER JOIN QL_trndoitemdtl dod ON dod.cmpcode=shd.cmpcode AND dod.doitemdtloid=shd.doitemdtloid INNER JOIN QL_trnsoitemmst som ON som.cmpcode=dod.cmpcode AND som.soitemmstoid=dod.soitemmstoid WHERE aritemmststatus IN ('Approved', 'Closed') AND aritemdate <= CAST('" + endperiod + "' AS DATETIME) GROUP BY arm.cmpcode, som.groupoid, arm.curroid";
            sSql += ") QL_totalshipment GROUP BY divname ORDER BY divname";

            List<dbshipmentmodel> dataDtl = db.Database.SqlQuery<dbshipmentmodel>(sSql).ToList();
            return Json(dataDtl, JsonRequestBehavior.AllowGet);
        }

        public class dbprodtotalkikmodel
        {
            public string statuskik { get; set; }
            public int totalkik { get; set; }
        }

        public class dbproddurationmodel
        {
            public string dayrange { get; set; }
            public int totalkik { get; set; }
        }

        public class dbprodprogressmodel
        {
            public int unclosedkik { get; set; }
            public int closedkik { get; set; }
        }

        public ActionResult GetDataDashboardProduction(string StartPeriod, string EndPeriod, string BusinessUnit)
        {
            sSql = "SELECT womststatus statuskik, COUNT(womstoid) totalkik FROM QL_trnwomst WHERE cmpcode='" + BusinessUnit + "' AND wodate>=CAST('" + StartPeriod + " 00:00:00' AS DATETIME) AND wodate<=CAST('" + EndPeriod + " 23:59:59' AS DATETIME) GROUP BY womststatus ORDER BY womststatus";
            List<dbprodtotalkikmodel> datatotalkik = db.Database.SqlQuery<dbprodtotalkikmodel>(sSql).ToList();

            sSql = "SELECT * FROM QL_trnwomst WHERE cmpcode='" + BusinessUnit + "' AND wodate>=CAST('" + StartPeriod + " 00:00:00' AS DATETIME) AND wodate<=CAST('" + EndPeriod + " 23:59:59' AS DATETIME)";
            List<dbproddurationmodel> dataduration = new List<dbproddurationmodel>();
            var totalkik1 = 0;
            var totalkik2 = 0;
            var totalkik3 = 0;

            dataduration.Add(new dbproddurationmodel() { dayrange = "<15", totalkik = totalkik1 });
            dataduration.Add(new dbproddurationmodel() { dayrange = "16 to 30", totalkik = totalkik2 });
            dataduration.Add(new dbproddurationmodel() { dayrange = "> 30", totalkik = totalkik3 });

            sSql = "SELECT SUM(unclosedkik) unclosedkik, SUM(closedkik) closedkik FROM (SELECT COUNT(womstoid) unclosedkik, 0 closedkik FROM QL_trnwomst WHERE cmpcode='" + BusinessUnit + "' AND wodate>=CAST('" + StartPeriod + " 00:00:00' AS DATETIME) AND wodate<=CAST('" + EndPeriod + " 23:59:59' AS DATETIME) AND womststatus IN ('In Process', 'Post') UNION ALL SELECT 0 unclosedkik, COUNT(womstoid) closedkik FROM QL_trnwomst WHERE cmpcode='" + BusinessUnit + "' AND wodate>=CAST('" + StartPeriod + " 00:00:00' AS DATETIME) AND wodate<=CAST('" + EndPeriod + " 23:59:59' AS DATETIME) AND womststatus NOT IN ('In Process', 'Post')) wo";
            List<dbprodprogressmodel> dataprogress = db.Database.SqlQuery<dbprodprogressmodel>(sSql).ToList();

            return Json(new { datatotalkik, dataduration, dataprogress }, JsonRequestBehavior.AllowGet);
        }

        public class dbavginventorymodel
        {
            public string periodacctg { get; set; }
            public string periodmonth { get; set; }
            public decimal amount { get; set; }
            public decimal amount_dead { get; set; }
            public decimal amount_active { get; set; }
            public decimal amount_usg { get; set; }
            public decimal ratio { get; set; }
            public string ratiotarget { get; set; }
        }

        public ActionResult GetDataDashboardAvgInventory_RM(string FilterBusUnit, string FilterYear)
        {
            sSql = "SELECT periodacctg, periodmonth, amount, amount_dead, (amount - amount_dead) amount_active, amount_usg, ((amount - amount_dead) / amount_usg) ratio, ISNULL((SELECT TOP 1 gendesc FROM QL_mstgen WHERE activeflag='ACTIVE' AND gengroup='RATIO TARGET AVG INVENT RM' AND ISNULL(genother1, '')='11'), '0') ratiotarget FROM (SELECT periodacctg, (SUBSTRING('JAN FEB MAR APR MAY JUN JUL AUG SEP OCT NOV DEC ', (CAST(RIGHT(periodacctg, 2) AS INT) * 4) - 3, 3)) periodmonth, ((SUM(amount) OVER (ORDER BY periodacctg)) / 1000000) amount, (amount_dead / 1000000) amount_dead, (amount_usg / 30000000) amount_usg FROM (SELECT periodacctg, SUM(amount) amount, SUM(amount_dead) amount_dead, SUM(amount_usg) amount_usg FROM (SELECT '201901' periodacctg, ISNULL((SUM((qtyin - qtyout) * valueidr)), 0.0) amount, 0.0 amount_dead, 0.0 amount_usg FROM View_ConMat WHERE cmpcode='11' AND periodacctg < '201901' AND refname='RAW MATERIAL' UNION ALL SELECT periodacctg, ISNULL((SUM((qtyin - qtyout) * valueidr)), 0.0) amount, 0.0 amount_dead, 0.0 amount_usg FROM View_ConMat WHERE cmpcode='11' AND periodacctg LIKE '2019%' AND refname='RAW MATERIAL' GROUP BY periodacctg UNION ALL SELECT periodacctg, 0.0 amount, 0.0 amount_dead, ISNULL((SUM((qtyout - qtyin) * valueidr)), 0.0) amount_usg FROM View_ConMat WHERE cmpcode='11' AND periodacctg LIKE '2019%' AND refname='RAW MATERIAL' AND formaction IN (SELECT tblname FROM QL_dashboardfilter) GROUP BY periodacctg UNION ALL SELECT periodacctg, 0.0 amount, ISNULL((SELECT SUM((conx.qtyin - conx.qtyout) * conx.valueidr) amount FROM View_ConMat conx WHERE conx.cmpcode='11' AND conx.refname='RAW MATERIAL' AND conx.periodacctg < con.periodacctg AND conx.refoid NOT IN (SELECT conxx.refoid FROM View_ConMat conxx WHERE conxx.cmpcode='11' AND conxx.refname='RAW MATERIAL' AND conxx.periodacctg=con.periodacctg)), 0.0) amount_dead, 0.0 amount_usg FROM View_ConMat con WHERE cmpcode='11' AND periodacctg LIKE '2019%' AND refname='RAW MATERIAL' GROUP BY periodacctg) con GROUP BY periodacctg) con) con ORDER BY periodacctg";
            sSql = sSql.Replace("11", "" + FilterBusUnit + "").Replace("2019", FilterYear);
            List<dbavginventorymodel> dataDtl = db.Database.SqlQuery<dbavginventorymodel>(sSql).ToList();

            JsonResult js = Json(dataDtl, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }

        public ActionResult GetDataDashboardAvgInventory_KY(string FilterBusUnit, string FilterYear)
        {
            sSql = "SELECT periodacctg, periodmonth, amount, amount_dead, (amount - amount_dead) amount_active, amount_usg, ((amount - amount_dead) / amount_usg) ratio, ISNULL((SELECT TOP 1 gendesc FROM QL_mstgen WHERE activeflag='ACTIVE' AND gengroup='RATIO TARGET AVG INVENT KY' AND ISNULL(genother1, '')='11'), '0') ratiotarget FROM (SELECT periodacctg, (SUBSTRING('JAN FEB MAR APR MAY JUN JUL AUG SEP OCT NOV DEC ', (CAST(RIGHT(periodacctg, 2) AS INT) * 4) - 3, 3)) periodmonth, ((SUM(amount) OVER (ORDER BY periodacctg)) / 1000000) amount, (amount_dead / 1000000) amount_dead, (amount_usg / 30000000) amount_usg FROM (SELECT periodacctg, SUM(amount) amount, SUM(amount_dead) amount_dead, SUM(amount_usg) amount_usg FROM (SELECT '201901' periodacctg, ISNULL((SUM((qtyin - qtyout) * valueidr)), 0.0) amount, 0.0 amount_dead, 0.0 amount_usg FROM View_ConMat WHERE cmpcode='11' AND periodacctg < '201901' AND refname IN ('LOG', 'PALLET') UNION ALL SELECT periodacctg, ISNULL((SUM((qtyin - qtyout) * valueidr)), 0.0) amount, 0.0 amount_dead, 0.0 amount_usg FROM View_ConMat WHERE cmpcode='11' AND periodacctg LIKE '2019%' AND refname IN ('LOG', 'PALLET') GROUP BY periodacctg UNION ALL SELECT periodacctg, 0.0 amount, 0.0 amount_dead, ISNULL((SUM((qtyout - qtyin) * valueidr)), 0.0) amount_usg FROM View_ConMat WHERE cmpcode='11' AND periodacctg LIKE '2019%' AND refname IN ('LOG', 'PALLET') AND formaction IN (SELECT tblname FROM QL_dashboardfilter) GROUP BY periodacctg UNION ALL SELECT periodacctg, 0.0 amount, ISNULL((SELECT SUM((conx.qtyin - conx.qtyout) * conx.valueidr) amount FROM View_ConMat conx WHERE conx.cmpcode='11' AND conx.refname IN ('LOG', 'PALLET') AND conx.periodacctg < con.periodacctg AND (conx.refname + '-' + CAST(conx.refoid AS VARCHAR(20))) NOT IN (SELECT (conxx.refname + '-' + CAST(conxx.refoid AS VARCHAR(20))) FROM View_ConMat conxx WHERE conxx.cmpcode='11' AND conxx.refname IN ('LOG', 'PALLET') AND conxx.periodacctg=con.periodacctg)), 0.0) amount_dead, 0.0 amount_usg FROM View_ConMat con WHERE cmpcode='11' AND periodacctg LIKE '2019%' AND refname IN ('LOG', 'PALLET') GROUP BY periodacctg) con GROUP BY periodacctg) con) con ORDER BY periodacctg";
            sSql = sSql.Replace("11", "" + FilterBusUnit + "").Replace("2019", FilterYear);
            List<dbavginventorymodel> dataDtl = db.Database.SqlQuery<dbavginventorymodel>(sSql).ToList();

            JsonResult js = Json(dataDtl, JsonRequestBehavior.AllowGet);
            js.MaxJsonLength = Int32.MaxValue;
            return js;
        }
    }
}