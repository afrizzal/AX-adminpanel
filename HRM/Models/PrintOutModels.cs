using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class PrintOutModels
    {
        public class printout_pr
        {
            public int prmstoid { get; set; }
            public string cmpcode { get; set; }
            public string BUName { get; set; }
            public string BUAddress { get; set; }
            public string BUCity { get; set; }
            public string BUProvince { get; set; }
            public string BUPhone { get; set; }
            public string BUEmail { get; set; }
            public string BUPhone2 { get; set; }
            public string BUPostCode { get; set; }
            public string BUFax1 { get; set; }
            public string BUFax2 { get; set; }
            public string BUCountry { get; set; }
            public string prno { get; set; }
            public string draftno { get; set; }
            public DateTime prdate { get; set; }
            public string department { get; set; }
            public string prmstnote { get; set; }
            public string prmststatus { get; set; }
            public int prdtlseq { get; set; }
            public int matoid { get; set; }
            public string matcode { get; set; }
            public string matlongdesc { get; set; }
            public decimal prqty { get; set; }
            public decimal requireqty { get; set; }
            public string RequireNo { get; set; }
            public DateTime RequireDate { get; set; }
            public string RequireHeaderNote { get; set; }
            public string RequireStatus { get; set; }
            public DateTime RequireCreateDate { get; set; }
            public string RequireCreateUser { get; set; }
            public string RequireDateType { get; set; }
            public string prunit { get; set; }
            public string prdtlnote { get; set; }
            public DateTime prarrdatereq { get; set; }
            public string UserNameApproved { get; set; }
            public DateTime ApproveTime { get; set; }
            public string CreateUserName { get; set; }
            public DateTime CreateTime { get; set; }
        }

        public class printout_bom
        {
            public string BusinessUnit { get; set; }
            public int ID { get; set; }
            public string WIPCode { get; set; }
            public string WIPDesc { get; set; }
            public string FGCode { get; set; }
            public string FinishGood { get; set; }
            public string FromDept { get; set; }
            public string ToDept { get; set; }
            public decimal WIPQty { get; set; }
            public string WIPUnit { get; set; }
            public string HeaderNote { get; set; }
            public string CreateUser { get; set; }
            public DateTime CreateDateTime { get; set; }
            public string LastUpdUser { get; set; }
            public DateTime LastUpdDateTime { get; set; }
            public decimal Thick { get; set; }
            public decimal Width { get; set; }
            public decimal Length { get; set; }
            public int IDDetail { get; set; }
            public int Nmr { get; set; }
            public string Type { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public decimal Qty { get; set; }
            public string Unit { get; set; }
            public string DetailNote { get; set; }

        }
    }
}