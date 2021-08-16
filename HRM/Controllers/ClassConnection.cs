using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace HRM.Controllers
{
    public class ClassConnection
    {
        private SqlConnection objCon;
        private SqlDataAdapter objdtAdapter;
        private SqlCommand objCmd;
        private DataSet objDtSet;

        public ClassConnection()
        {
            objCon = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnString"]);
        }

        public DataTable GetDataTable(string SQLQuery, string TableName)
        {
            DataTable objDtTable = null;

            if (objCon.State == ConnectionState.Closed)
                objCon.Open();

            objCmd = new SqlCommand(SQLQuery, objCon);
            objCmd.CommandTimeout = 0;
            objDtSet = new DataSet();
            objdtAdapter = new SqlDataAdapter(objCmd);
            objdtAdapter.Fill(objDtSet, TableName);
            objDtTable = objDtSet.Tables[TableName];
            objCon.Close();

            return objDtTable;
        }

        public string GetDataScalar(string SQLQuery)
        {
            var result = "";

            if (objCon.State == ConnectionState.Closed)
                objCon.Open();

            objCmd = new SqlCommand(SQLQuery, objCon);
            objCmd.CommandTimeout = 0;
            try
            {
                var reader = objCmd.ExecuteReader();
                while (reader.Read())
                    result = reader.GetValue(0).ToString();

                reader.Close();
            }
            catch
            {
                result = "";
            }

            objCon.Close();

            return result;
        }
    }
}