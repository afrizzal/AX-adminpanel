using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HRM.Models;
using System.ComponentModel;
using System.Data;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HRM.Controllers
{
    public class ClassFunction
    {
        public class mstacctg
        {
            public int acctgoid { get; set; }
        }

        public class trngldtl
        {
            public int glseq { get; set; }
            public string acctgcode { get; set; }
            public string acctgdesc { get; set; }
            public decimal acctgdebet { get; set; }
            public decimal acctgcredit { get; set; }
        }

        public static bool checkPagePermission(string PagePath, List<RoleDetail> dtAccess)
        {
            if (HttpContext.Current.Session["UserID"].ToString().ToLower() == "admin")
            {
                return false;
            }
            List<RoleDetail> newrole = dtAccess.FindAll(o => o.formaddress.ToUpper() == PagePath.ToUpper());
            return newrole.Count() <= 0;
        }

        public static bool checkPagePermissionForReport(string PagePath, List<RoleDetail> dtAccess)
        {
            if (HttpContext.Current.Session["UserID"].ToString().ToLower() == "admin")
            {
                return false;
            }
            List<RoleDetail> newrole = dtAccess.FindAll(o => o.formaddress.ToUpper() + "/" + o.formmenu.ToUpper() == PagePath.ToUpper());
            return newrole.Count() <= 0;
        }

        public static bool isSpecialAccess(string PagePath, List<RoleSpecial> dtAccess)
        {
            if (HttpContext.Current.Session["UserID"].ToString().ToLower() == "admin")
            {
                return true;
            }
            List<RoleSpecial> newrole = dtAccess.FindAll(o => o.formaddress.ToUpper() == PagePath.ToUpper());
            return newrole.Count() > 0;
        }

        public static DateTime GetServerTime()
        {
            HRMEntities db = new HRMEntities();
            return db.Database.SqlQuery<DateTime>("SELECT GETDATE() AS tanggal").FirstOrDefault();
        }

        public static int GenerateID(string sTableName)
        {
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            HRMEntities db = new HRMEntities();
            int oid = 1;
            return oid;
        }

        public static string GetDateToPeriodAcctg(DateTime date)
        {
            return date.ToString("yyyyMM");
        }

        public static string Tchar(string svar)
        {
            return svar.Trim().Replace("'", "''");
        }

        public static String Left(string input, int length)
        {
            var result = "";
            if ((input.Length <= 0)) return result;
            if ((length > input.Length))
            {
                length = input.Length;
            }
            result = input.Substring(0, length);
            return result;
        }

        public static String Mid(string input, int start, int length)
        {
            var result = "";
            if (((input.Length <= 0) || (start >= input.Length))) return result;
            if ((start + length > input.Length))
            {
                length = (input.Length - start);
            }
            result = input.Substring(start, length);
            return result;
        }

        public static String Right(string input, int length)
        {
            var result = "";
            if ((input.Length <= 0)) return result;
            if ((length > input.Length))
            {
                length = input.Length;
            }
            result = input.Substring((input.Length - length), length);
            return result;
        }

        public static string GetLastPeriod(string speriod)
        {
            string periodbefore = "";
            if (speriod != "")
            {
                if (speriod.Length == 6)
                {
                    var iVal_1 = Convert.ToInt32(Left(speriod, 2));
                    if (iVal_1 == 1)
                    {
                        var sVal_2 = (Convert.ToInt32(Left(speriod, 4)) - 1).ToString("0000");
                        periodbefore = sVal_2 + "12";
                    }
                    else
                    {
                        var sVal_3 = Left(speriod, 4);
                        var sVal_4 = (Convert.ToInt32(Right(speriod, 2)) - 1).ToString("00");
                        periodbefore = sVal_3 + sVal_4;
                    }
                }
            }
            return periodbefore;
        }

        public static string GetUserLocation(string sUser)
        {
            HRMEntities db = new HRMEntities();
            return db.Database.SqlQuery<string>("SELECT ISNULL(CAST(whoid AS VARCHAR(10)), '') whoid FROM QL_mstperson p INNER JOIN QL_mstprof pr ON pr.cmpcode=p.cmpcode AND pr.personoid=p.personoid WHERE profoid='" + sUser + "'").FirstOrDefault();
        }

        public static Boolean IsStockAvailable(string cmp, string sPeriod, int iMatOid, int iWhoid, decimal dQty, string sType)
        {
            HRMEntities db = new HRMEntities();
            var sSql = "";
            sSql = "SELECT COUNT(refoid) FROM QL_crdmtr WHERE cmpcode='" + cmp + "' AND periodacctg IN ('" + sPeriod + "', '" + GetLastPeriod(sPeriod) + "') AND refoid=" + iMatOid + " AND refname LIKE '" + sType + "%' AND mtrwhoid=" + iWhoid + " AND closingdate='01/01/1900' GROUP BY refoid HAVING SUM(saldoakhir)>=" + dQty;
            var iStockQty = db.Database.SqlQuery<int>(sSql).FirstOrDefault();
            if (iStockQty > 0) { return true; } else { return false; }
        }

        public static string GenNumberString(int iNumber, int iDefaultCounter)
        {
            int iAdd = iNumber.ToString().Length - iDefaultCounter;
            if (iAdd > 0)
            {
                iDefaultCounter += iAdd;
            }
            string sFormat = "";
            for (int i = 1; i <= iDefaultCounter; i++)
            {
                sFormat += "0";
            }
            return iNumber.ToString(sFormat);
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static Boolean IsInterfaceExists(string sVar, string cmp)
        {
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            HRMEntities db = new HRMEntities();
            var sSql = ""; var flag = false;
            sSql = "SELECT COUNT(-1) FROM QL_mstinterface WHERE cmpcode='" + CompnyCode + "' AND activeflag='ACTIVE' AND interfacevar='" + sVar + "' AND interfaceres1='" + cmp + "'";
            var iCountVar = db.Database.SqlQuery<int>(sSql).FirstOrDefault();
            if (iCountVar > 0) {
                flag = true;
            } else {
                sSql = "SELECT COUNT(-1) FROM QL_mstinterface WHERE cmpcode='" + CompnyCode + "' AND activeflag='ACTIVE' AND interfacevar='" + sVar + "' AND interfaceres1='All'";
                var iCountVarAll = db.Database.SqlQuery<int>(sSql).FirstOrDefault();
                if (iCountVarAll > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public static string GetInterfaceWarning(string sVar)
        {
            return "Please Define some COA for Variable " + sVar + " in Accounting -> Master -> Interface before continue this form!";
        }

        public static string GetVarInterface(string sVar, string cmp)
        {
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            string BranchTrnCode = System.Configuration.ConfigurationManager.AppSettings["BranchTrnCode"];
            HRMEntities db = new HRMEntities();
            var sSql = "";
            if (Left(BranchTrnCode, 3) == "SBY")
            {
                BranchTrnCode = "SBY";
            }
            else if (Left(BranchTrnCode, 4) == "CAMP")
            {
                BranchTrnCode = "CAMP";
            }
            else
            {
                BranchTrnCode = "SMD";
            }
            sSql = "SELECT TOP 1 interfacevalue FROM QL_mstinterface WHERE interfacevar='" + sVar + "' AND cmpcode='" + CompnyCode + "' AND interfaceres2='" + BranchTrnCode + "' AND interfacegroup='" + cmp + "'";
            var sAcctgCode = db.Database.SqlQuery<string>(sSql).FirstOrDefault();
            if (!string.IsNullOrEmpty(sAcctgCode))
            {
                sSql = "SELECT TOP 1 interfacevalue FROM QL_mstinterface WHERE interfacevar='" + sVar + "' AND cmpcode='" + CompnyCode + "' AND interfacevalue<>''";
                sAcctgCode = db.Database.SqlQuery<string>(sSql).FirstOrDefault();
            }
            return sAcctgCode;
        }

        public static int GetAcctgOID(string sVar)
        {
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            HRMEntities db = new HRMEntities();
            var sSql = "";
            sSql = "SELECT TOP 1 acctgoid FROM QL_mstacctg WHERE acctgcode='" + sVar + "' and cmpcode='" + CompnyCode + "'";
            var iAcctgOid = db.Database.SqlQuery<int>(sSql).FirstOrDefault();
            return iAcctgOid;
        }

        public static Boolean IsQtyRounded(decimal dQty, decimal dRound)
        {
            long iRes;
            string sResult = Convert.ToString(dQty / dRound);
            if (long.TryParse(sResult, out iRes))
                return true;
            else
                return false;
        }
       
        public static bool EmailIsValid(string email)
        {
            //return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string GetQueryUpdateStockValue(decimal dQty, decimal dValIDR, decimal dValUSD, string sFormType, DateTime dLastDate, string sLastUpdUser, string sCmpCode, string sPeriod, int iRefOid, string sRefName)
        {
            return "UPDATE QL_stockvalue SET stockqty=stockqty + " + dQty + ", stockvalueidr=ROUND(ISNULL((CAST(((stockvalueidr * stockqty) + " + dQty * dValIDR + ") AS FLOAT) / NULLIF((stockqty + " + dQty + "), 0)), 0), 4), stockvalueusd=ROUND(ISNULL((CAST(((stockvalueusd * stockqty) + " + dQty * dValUSD + ") AS FLOAT) / NULLIF((stockqty + " + dQty + "), 0)), 0), 6), lasttranstype='" + sFormType + "', lasttransdate='" + dLastDate + "', upduser='" + sLastUpdUser + "', updtime=CURRENT_TIMESTAMP, backupqty=stockqty, backupvalueidr=stockvalueidr, backupvalueusd=stockvalueusd WHERE cmpcode='" + sCmpCode + "' AND periodacctg='" + sPeriod + "' AND refoid=" + iRefOid + " AND refname='" + sRefName + "'";
        }

        public static string GetQueryInsertStockValue(decimal dQty, decimal dValIDR, decimal dValUSD, string sFormType, DateTime dLastDate, string sLastUpdUser, string sCmpCode, string sPeriod, int iRefOid, string sRefName, int iOid)
        {
            return "INSERT INTO QL_stockvalue (cmpcode, stockvalueoid, periodacctg, refoid, refname, stockqty, stockvalueidr, stockvalueusd, lasttranstype, lasttransdate, note, upduser, updtime, backupqty, backupvalueidr, backupvalueusd, closeflag, flag_temp) VALUES ('" + sCmpCode + "', " + iOid + ", '" + sPeriod + "', " + iRefOid + ", '" + sRefName + "', " + dQty + ", " + dValIDR + ", " + dValUSD + ", '" + sFormType + "', '" + dLastDate + "', '', '" + sLastUpdUser + "', CURRENT_TIMESTAMP, 0, 0, 0, '', '')";
        }

        public static Boolean isLengthAccepted(string sField, string sTable, decimal dValue, ref string sErrReply)
        {
            int iPrec = 0, iScale = 0;
            using (var conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnString"]))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "SELECT col.precision, col.scale FROM sys.tables ta INNER JOIN sys.columns col ON col.object_id=ta.object_id WHERE col.name='" + sField + "' AND ta.name='" + sTable + "'";
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            iPrec = reader.GetOrdinal("precision");
                            iScale = reader.GetOrdinal("scale");
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    iPrec = 0; iScale = 0;
                    sErrReply = "- Can't check the result. Please check that the parameter has been assigned correctly!";
                    return false;
                }
                conn.Close();
            }
            if (iPrec > 0 && iScale > 0)
            {
                string sTextValue = "";
                for (var i = 0; i < (iPrec - iScale - 1); i++)
                {
                    sTextValue += "9";
                }
                if (iScale > 0)
                {
                    sTextValue += ".";
                    for (var i = 0; i < iScale - 1; i++)
                    {
                        sTextValue += "9";
                    }
                }
                if (dValue > Convert.ToDecimal(sTextValue))
                {
                    sErrReply = string.Format("{0:#,0.00}", Convert.ToDecimal(sTextValue));
                    return false;
                }
            }
            return true;
        }

        public static string GetDataAcctgOid(string sVar, string cmp, string sFilter = "")
        {
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            string BranchTrnCode = System.Configuration.ConfigurationManager.AppSettings["BranchTrnCode"];
            if (Left(BranchTrnCode, 3) == "SBY")
            {
                BranchTrnCode = "SBY";
            }
            else if (Left(BranchTrnCode, 4) == "CAMP")
            {
                BranchTrnCode = "CAMP";
            }
            else
            {
                BranchTrnCode = "SMD";
            }
            HRMEntities db = new HRMEntities();
            List<mstacctg> tbl = new List<mstacctg>();
            string sSql = "";
            sSql = "SELECT TOP 1 interfacevalue FROM QL_mstinterface WHERE cmpcode='" + CompnyCode + "' AND interfacevar='" + sVar + "' AND interfaceres2='" + BranchTrnCode + "' AND interfacegroup='" + cmp + "'";
            string sCode = db.Database.SqlQuery<string>(sSql).FirstOrDefault();
            if (string.IsNullOrEmpty(sCode))
            {
                sSql = "SELECT TOP 1 interfacevalue FROM QL_mstinterface WHERE cmpcode='" + CompnyCode + "' AND interfacevar='" + sVar + "' AND interfaceres2='All' AND interfacegroup='All'";
                sCode = db.Database.SqlQuery<string>(sSql).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(sCode))
            {
                sSql = "SELECT DISTINCT a.acctgoid FROM QL_mstacctg a WHERE a.cmpcode='" + CompnyCode + "' AND a.acctgflag='A' " + sFilter + " AND (";
                string[] sSplitCode = sCode.Split(',');
                for (var i = 0; i < sSplitCode.Length; i++)
                {
                    sSql += "a.acctgcode LIKE '" + sSplitCode[i].TrimStart() + "%'";
                    if (i < sSplitCode.Length - 1)
                    {
                        sSql += " OR ";
                    }
                }
                sSql += ") AND a.acctgoid NOT IN (SELECT DISTINCT ac.acctggrp3 FROM QL_mstacctg ac WHERE ac.acctggrp3 IS NOT NULL AND ac.cmpcode=a.cmpcode AND ac.deleteflag='') AND a.deleteflag=''";
            }
            tbl = db.Database.SqlQuery<mstacctg>(sSql).ToList();
            string sOid = "0";
            string sOidTemp = "";
            if (tbl != null)
            {
                if (tbl.Count() > 0)
                {
                    for (var i = 0; i < tbl.Count(); i++)
                    {
                        sOidTemp += tbl[i].acctgoid.ToString() + ", ";
                    }
                    if (sOidTemp != "")
                        sOid = Left(sOidTemp, sOidTemp.Length - 2);
                }
            }
            return sOid;
        }

        public static string GetDataAcctgOid(string[] sVar, string cmp, string sFilter = "")
        {
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            HRMEntities db = new HRMEntities();
            List<mstacctg> tbl = new List<mstacctg>();
            string sSql = "";
            string sCode = "";
            for (var i = 0; i < sVar.Length; i++)
            {
                sSql = "SELECT TOP 1 interfacevalue FROM QL_mstinterface WHERE cmpcode='" + CompnyCode + "' AND interfacevar='" + sVar[i] + "' AND interfaceres1='" + cmp + "'";
                string sTmp = db.Database.SqlQuery<string>(sSql).FirstOrDefault();
                if (sTmp == "")
                {
                    sSql = "SELECT TOP 1 interfacevalue FROM QL_mstinterface WHERE cmpcode='" + CompnyCode + "' AND interfacevar='" + sVar[i] + "' AND interfaceres1='All'";
                    sTmp = db.Database.SqlQuery<string>(sSql).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(sCode))
                    sCode = sTmp;
                else
                    sCode += ", " + sTmp;
            }
            if (!string.IsNullOrEmpty(sCode))
            {
                sSql = "SELECT DISTINCT a.acctgoid FROM QL_mstacctg a WHERE a.cmpcode='" + CompnyCode + "' AND a.activeflag='ACTIVE' " + sFilter + " AND (";
                string[] sSplitCode = sCode.Split(',');
                for (var i = 0; i < sSplitCode.Length; i++)
                {
                    sSql += "a.acctgcode LIKE '" + sSplitCode[i].TrimStart() + "%'";
                    if (i < sSplitCode.Length - 1)
                    {
                        sSql += " OR ";
                    }
                }
                sSql += ") AND a.acctgoid NOT IN (SELECT DISTINCT ac.acctggrp3 FROM QL_mstacctg ac WHERE ac.acctggrp3 IS NOT NULL AND ac.cmpcode=a.cmpcode) ";
            }
            tbl = db.Database.SqlQuery<mstacctg>(sSql).ToList();
            string sOid = "0";
            string sOidTemp = "";
            if (tbl != null)
            {
                if (tbl.Count() > 0)
                {
                    for (var i = 0; i < tbl.Count(); i++)
                    {
                        sOidTemp += tbl[i].acctgoid.ToString() + ", ";
                    }
                    if (sOidTemp != "")
                        sOid = Left(sOidTemp, sOidTemp.Length - 2);
                }
            }
            return sOid;
        }

        public static Boolean isPeriodAcctgClosed(string cmpcode, DateTime sDate)
        {
            HRMEntities db = new HRMEntities();
            string sPeriod = GetDateToPeriodAcctg(sDate);
            string sSql = "SELECT COUNT(*) FROM QL_crdgl WHERE cmpcode='" + cmpcode + "' AND crdglflag='CLOSED' AND periodacctg='" + sPeriod + "'";
            if (db.Database.SqlQuery<int>(sSql).FirstOrDefault() > 0)
                return true;
            else
                return false;
        }
        public static List<trngldtl> ShowCOAPosting(string sNoRef, string sCmpCode, string sRateType = "Default", string sOther = "")
        {
            HRMEntities db = new HRMEntities();
            List<trngldtl> tbl = new List<trngldtl>();
            var sSql = "";
            var sFieldAmt = "glamt";
            if (sRateType == "IDR")
                sFieldAmt = "glamtidr";
            else if (sRateType == "USD")
                sFieldAmt = "glamtusd";

            sSql = "SELECT * FROM (SELECT d.glseq, a.acctgcode, a.acctgdesc, (CASE d.gldbcr WHEN 'D' THEN d." + sFieldAmt + " ELSE 0 END) AS acctgdebet, (CASE d.gldbcr WHEN 'C' THEN d." + sFieldAmt + " ELSE 0 END) AS acctgcredit FROM QL_trngldtl d INNER JOIN QL_mstacctg a ON a.acctgoid=d.acctgoid WHERE d.noref='" + sNoRef + "' AND d.cmpcode='" + sCmpCode + "' AND d." + sFieldAmt + ">0 AND ISNULL(d.glother1, '')='" + sOther + "' UNION ALL SELECT d.glseq, a.acctgcode, a.acctgdesc, (CASE d.gldbcr WHEN 'D' THEN d." + sFieldAmt + " ELSE 0 END) AS acctgdebet, (CASE d.gldbcr WHEN 'C' THEN d." + sFieldAmt + " ELSE 0 END) AS acctgcredit FROM /*QL_trngldtl_hist*/QL_trngldtl d INNER JOIN QL_mstacctg a ON a.acctgoid=d.acctgoid WHERE d.noref='" + sNoRef + "' AND d.cmpcode='" + sCmpCode + "' AND d." + sFieldAmt + ">0 AND ISNULL(d.glother1, '')='" + sOther + "') tbl_posting ORDER BY glseq";
            tbl = db.Database.SqlQuery<trngldtl>(sSql).ToList();

            if (tbl == null)
            {
                sSql = "SELECT * FROM (SELECT d.glseq, a.acctgcode, a.acctgdesc, (CASE d.gldbcr WHEN 'D' THEN d." + sFieldAmt + " ELSE 0 END) AS acctgdebet, (CASE d.gldbcr WHEN 'C' THEN d." + sFieldAmt + " ELSE 0 END) AS accgtcredit FROM QL_trngldtl d INNER JOIN QL_mstacctg a ON a.acctgoid=d.acctgoid WHERE d.noref LIKE '%" + sNoRef + "' AND d.cmpcode='" + sCmpCode + "' AND d." + sFieldAmt + ">0 AND ISNULL(d.glother1, '')='" + sOther + "' UNION ALL SELECT d.glseq, a.acctgcode, a.acctgdesc, (CASE d.gldbcr WHEN 'D' THEN d." + sFieldAmt + " ELSE 0 END) AS acctgdebet, (CASE d.gldbcr WHEN 'C' THEN d." + sFieldAmt + " ELSE 0 END) AS accgtcredit FROM /*QL_trngldtl_hist*/QL_trngldtl d INNER JOIN QL_mstacctg a ON a.acctgoid=d.acctgoid WHERE d.noref LIKE '%" + sNoRef + "' AND d.cmpcode='" + sCmpCode + "' AND d." + sFieldAmt + ">0 AND ISNULL(d.glother1, '')='" + sOther + "') tbl_posting ORDER BY glseq";
                tbl = db.Database.SqlQuery<trngldtl>(sSql).ToList();
            }
            return tbl;
        }

        public static decimal GetStockValueIDR(string cmpcode, string periodacctg, string refname, int refoid)
        {
            return new HRMEntities().Database.SqlQuery<decimal>("SELECT ISNULL((SUM(ISNULL(stockqty, 0) * ISNULL(stockvalueidr, 0)) / NULLIF(SUM(ISNULL(stockqty, 0)), 0)), 0) FROM QL_stockvalue WHERE cmpcode='" + cmpcode + "' AND periodacctg IN ('" + periodacctg + "', '" + GetLastPeriod(periodacctg) + "') AND refoid=" + refoid + " AND refname='" + refname + "' AND closeflag=''").FirstOrDefault();
        }

        public static decimal GetStockValueUSD(string cmpcode, string periodacctg, string refname, int refoid)
        {
            return new HRMEntities().Database.SqlQuery<decimal>("SELECT ISNULL((SUM(ISNULL(stockqty, 0) * ISNULL(stockvalueusd, 0)) / NULLIF(SUM(ISNULL(stockqty, 0)), 0)), 0) FROM QL_stockvalue WHERE cmpcode='" + cmpcode + "' AND periodacctg IN ('" + periodacctg + "', '" + GetLastPeriod(periodacctg) + "') AND refoid=" + refoid + " AND refname='" + refname + "' AND closeflag=''").FirstOrDefault();
        }
        public static Boolean CheckDataExists(string sSql)
        {
            HRMEntities db = new HRMEntities();
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            Boolean cDataEx = false;
            int sTmp = 0;

            sTmp = db.Database.SqlQuery<int>(sSql).FirstOrDefault();
            if (sTmp > 0)
            {
                cDataEx = true;
            }
            return cDataEx;
        }

        public static Boolean CheckDataExists(Int64 iKey, string[] sColumnKeyName, string[] sTable) {
            HRMEntities db = new HRMEntities();
            string CompnyCode = System.Configuration.ConfigurationManager.AppSettings["CompnyCode"];
            Boolean cDataEx = false;
            string sSql = "";
            int sTmp = 0;

            for (int i = 0; i < sTable.Length; i++)
            {
                sSql = "SELECT COUNT(-1) FROM " + sTable[i] + " WHERE " + sColumnKeyName[i] + "=" + iKey;
                sTmp = db.Database.SqlQuery<int>(sSql).FirstOrDefault();
                if(sTmp > 0)
                {
                    cDataEx = true;
                    break;
                }
            }
            return cDataEx;
        }
        public static bool SendingEmail(string sUserSender, string sEmailRecipient, string sEmailSubject, string sNote)
        {
            bool isSuccess = false;
            var sEmailSender = "noreply@iil.co.id";
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(sEmailSender, sUserSender);
                message.To.Add(sEmailRecipient);
                message.Subject = sEmailSubject;
                message.Body = sNote;

                var SmtpServer = new SmtpClient();
                if (Right(sEmailRecipient.Trim(), 27) == "@integragroup-indonesia.com")
                {
                    SmtpServer.Credentials = new System.Net.NetworkCredential("hris", "Integra12345");
                    SmtpServer.Port = 25;
                    SmtpServer.Host = "mail.integragroup-indonesia.com";
                    SmtpServer.Send(message);
                } else if (Right(sEmailRecipient.Trim(), 10) == "@iil.co.id")
                {
                    SmtpServer.Credentials = new System.Net.NetworkCredential("hris", "Integra12345");
                    SmtpServer.Port = 25;
                    SmtpServer.Host = "mail.integragroup-indonesia.com";
                    SmtpServer.Send(message);
                }
                else
                {
                    SmtpServer.Credentials = new System.Net.NetworkCredential("hris", "Integra12345");
                    SmtpServer.Port = 25;
                    SmtpServer.Host = "mail.iil.co.id";
                    SmtpServer.Send(message);
                }
                isSuccess = true;
            }
            catch
            {
                return false;
            }
            return isSuccess;
        }
        public static bool SplitFilterLikeIn(string field, string value, out string filterlike, out string filterin)
        {
            filterlike = ""; filterin = "";
            if (!string.IsNullOrEmpty(value))
            {
                var data = value.Split(';');
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != "")
                    {
                        if (data[i].Contains("%"))
                            filterlike += field + " LIKE '" + data[i] + "' OR ";
                        else
                            filterin += "'" + data[i] + "', ";
                    }
                }
                if (filterlike != "")
                    filterlike = "(" + ClassFunction.Left(filterlike, filterlike.Length - 4) + ")";
                if (filterin != "")
                    filterin = "(" + field + " IN (" + ClassFunction.Left(filterin, filterin.Length - 2) + "))";
            }
            return (filterlike != "" || filterin != "");
        }

        public static List<T> DataTableToList<T>(DataTable table) where T : class, new()
        {
            try
            {
                T tempT = new T();
                var tType = tempT.GetType();
                List<T> list = new List<T>();
                if (table != null)
                    foreach (var row in table.Rows.Cast<DataRow>())
                    {
                        T obj = new T();
                        foreach (var prop in obj.GetType().GetProperties())
                        {
                            var propertyInfo = tType.GetProperty(prop.Name);
                            object safeValue = null;
                            try
                            {
                                var rowValue = row[prop.Name];
                                var t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                                safeValue = (rowValue == null || DBNull.Value.Equals(rowValue)) ? null : Convert.ChangeType(rowValue, t);
                            }
                            catch { }
                            propertyInfo.SetValue(obj, safeValue, null);
                        }
                        list.Add(obj);
                    }
                return list;
            }
            catch
            {
                return null;
            }
        }

    }
}