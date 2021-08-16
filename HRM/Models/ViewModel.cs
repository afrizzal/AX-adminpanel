using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class DataLogin
    {
        public string userid { get; set; }
        public string userpwd { get; set; }
    }

    public class RoleDetail
    {
        public string formtype { get; set; }
        public string formname { get; set; }
        public string formaddress { get; set; }
        public string formmodule { get; set; }
        public int formnumber { get; set; }
        public string formmenu { get; set; }
        public string formimage { get; set; }
    }

    public class RoleSpecial
    {
        public string formaddress { get; set; }
        public string special { get; set; }
    }

    public class ModelFilter
    {
        public DateTime filterperiodfrom { get; set; }
        public DateTime filterperiodto { get; set; }
        public string filterstatus { get; set; }
        public string filterddl { get; set; }
        public string filtertext { get; set; }
        public bool isperiodchecked { get; set; }
    }

    public class PersonHRIS
    {
        public string cmpcode { get; set; }
        public int personoid { get; set; }
        public string nip { get; set; }
        public string personname { get; set; }
        public string deptname { get; set; }
    }

    public class MaterialMix
    {
        public int matoid_mix { get; set; }
        public string matcode_mix { get; set; }
        public string matlongdesc_mix { get; set; }
        public string matunit_mix { get; set; }
        public int unitoid_mix { get; set; }
    }

    public class CheckBoxListItem
    {
        public int ID { get; set; }
        public string Display { get; set; }
        public bool IsChecked { get; set; }
    }

    public class vmMatDtl_hdr
    {
        public int oid { get; set; }
        public int matoid { get; set; }
        public string matcode { get; set; }
        public string matlongdesc { get; set; }
        public string matunit { get; set; }
        public string flag { get; set; }
        public string note { get; set; }
        public string createuser { get; set; }
        public DateTime createtime { get; set; }
        public string upduser { get; set; }
        public DateTime updtime { get; set; }
        public List<vmMatDtl_dtl> datadetail { get; set; }

        public vmMatDtl_hdr()
        {
            oid = 0;
            datadetail = new List<vmMatDtl_dtl>();
        }
    }

    public class vmMatDtl_dtl
    {
        public int seq { get; set; }
        public int suppoid { get; set; }
        public string suppcode { get; set; }
        public string suppname { get; set; }
        public string extcode { get; set; }
        public string extname { get; set; }
        public decimal convertqty { get; set; }
        public int convertunitoid { get; set; }
        public string convertunit { get; set; }
        public decimal packqty { get; set; }
        public int packunitoid { get; set; }
        public string packunit { get; set; }
        public decimal minqty { get; set; }
        public int minunitoid { get; set; }
        public string minunit { get; set; }
        public decimal leadday { get; set; }
        public string extnote { get; set; }
    }

    public class vmMstAcctg
    {
        public string cmpcode { get; set; }
        public int acctgoid { get; set; }
        public string acctgcode { get; set; }
        public string acctgdesc { get; set; }
        public int curroid { get; set; }
        public int nmr { get; set; }
    }
}