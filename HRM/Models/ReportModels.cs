using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class ReportModels
    {
        public class DDLBusinessUnitModel
        {
            public string divcode { get; set; }
            public string divname { get; set; }
        }

        public class DDLDepartmentModel
        {
            public int deptoid { get; set; }
            public string deptname { get; set; }
        }

        public class DDLAccountModel
        {
            public int acctgoid { get; set; }
            public string acctgdesc { get; set; }
        }

        public class DDLSingleField
        {
            public string sfield { get; set; }
        }

        public class DDLDoubleField
        {
            public int ifield { get; set; }
            public string sfield { get; set; }
        }

        public class DDLDoubleFieldString
        {
            public string valuefield { get; set; }
            public string textfield { get; set; }
        }

        public class CheckBoxField
        {
            public string valuefield { get; set; }
            public string textfield { get; set; }
            public bool checkflag { get; set; }
        }

        public class DDLReasonModel
        {
            public int genoid { get; set; }
            public string gendesc { get; set; }
        }
		
		public class DDLDivisionModel
        {
            public int groupoid { get; set; }
            public string groupcode { get; set; }
        }
		
		public class DDLWarehouseModel
        {
            public int genoid { get; set; }
            public string gendesc { get; set; }
        }

        public class DDLGroupModel
        {
            public int suppoid { get; set; }
            public string suppname { get; set; }
        }
		
		public class DDLUserModel
        {
            public string a { get; set; }

            public string b { get; set; }
        }

        public class DDLBAReturnUserModel
        {
            public string createuser { get; set; }

            public string createuser1 { get; set; }
        }

        public class DDLSupModel
        {
            public int suppoid { get; set; }
            public string suppname { get; set; }
        }

        public class FullFormType
        {
            public string formtype { get; set; }
            public string formtitle { get; set; }
            public string reftype { get; set; }
            public string mattype { get; set; }
            public string flagtype { get; set; }
            public string formref { get; set; }
            public string formabbr { get; set; }
            public string stocktype { get; set; }

            public FullFormType()
            {
                this.formtype = "";
                this.formtitle = "";
                this.reftype = "";
                this.mattype = "";
                this.flagtype = "";
                this.formref = "";
                this.formabbr = "";
                this.stocktype = "";
            }

            public FullFormType(string formparam)
            {
                if (formparam.Replace(" ", "").ToUpper() == "RAWMATERIAL" || formparam.Replace(" ", "").ToUpper() == "RAW")
                {
                    this.formtype = "RawMaterial";
                    this.formtitle = "Raw Material";
                    this.reftype = "raw";
                    this.mattype = "matraw";
                    this.flagtype = "A";
                    this.formref = "Raw";
                    this.formabbr = "RM";
                    this.stocktype = "RAW MATERIAL";
                }
                else if (formparam.Replace(" ", "").ToUpper() == "GENERALMATERIAL" || formparam.Replace(" ", "").ToUpper() == "GENERAL" || formparam.ToUpper() == "GEN")
                {
                    this.formtype = "GeneralMaterial";
                    this.formtitle = "General Material";
                    this.reftype = "gen";
                    this.mattype = "matgen";
                    this.flagtype = "A";
                    this.formref = "General";
                    this.formabbr = "GM";
                    this.stocktype = "GENERAL MATERIAL";
                }
                else if (formparam.Replace(" ", "").ToUpper() == "SPAREPART" || formparam.ToUpper() == "SP")
                {
                    this.formtype = "SparePart";
                    this.formtitle = "Spare Part";
                    this.reftype = "sp";
                    this.mattype = "sparepart";
                    this.flagtype = "A";
                    this.formref = "Spare Part";
                    this.formabbr = "SP";
                    this.stocktype = "SPARE PART";
                }
                else if (formparam.Replace(" ", "").ToUpper() == "FINISHGOOD" || formparam.ToUpper() == "ITEM")
                {
                    this.formtype = "FinishGood";
                    this.formtitle = "Finish Good";
                    this.reftype = "item";
                    this.mattype = "item";
                    this.flagtype = "A";
                    this.formref = "Finish Good";
                    this.formabbr = "FG";
                    this.stocktype = "FINISH GOOD";
                }
                else if (formparam.Replace(" ", "").ToUpper() == "FIXEDASSETS")
                {
                    this.formtype = "FixedAssets";
                    this.formtitle = "Fixed Assets";
                    this.reftype = "asset";
                    this.mattype = "";
                    this.flagtype = "B";
                    this.formref = "Asset";
                    this.formabbr = "FA";
                    this.stocktype = "";
                }
                else if (formparam.Replace(" ", "").ToUpper() == "LOG")
                {
                    this.formtype = "Log";
                    this.formtitle = "Log";
                    this.reftype = "wip";
                    this.mattype = "log";
                    this.flagtype = "C";
                    this.formref = "Log";
                    this.formabbr = "LOG";
                    this.stocktype = "LOG";
                }
                else if (formparam.Replace(" ", "").ToUpper() == "SAWNTIMBER" || formparam.Replace(" ", "").ToUpper() == "SAWN")
                {
                    this.formtype = "SawnTimber";
                    this.formtitle = "Sawn Timber";
                    this.reftype = "sawn";
                    this.mattype = "pallet";
                    this.flagtype = "C";
                    this.formref = "Sawn";
                    this.formabbr = "ST";
                    this.stocktype = "PALLET";
                }
            }
        }
    }
}