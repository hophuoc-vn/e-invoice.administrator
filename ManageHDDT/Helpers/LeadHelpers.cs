using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ManageHDDT.Helpers
{
    public class LeadHelpers
    {
        public DataTable GetDataFromExcel(string ExcelCnxStr)
        {
            try
            {
                DataTable data_table = new DataTable();

                //Create Connection to Excel work book 
                using (OleDbConnection excel_cnx = new OleDbConnection(ExcelCnxStr))
                {
                    excel_cnx.Open();

                    var ole_db_table = excel_cnx.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    if (Utils.CheckDataTable(ole_db_table))
                    {
                        string sheet_0_name = string.Format("{0}", ole_db_table.Rows[0]["TABLE_NAME"]);
                        //Create OleDbCommand to fetch data from Excel 
                        using (OleDbCommand ole_db_cmd = new OleDbCommand("SELECT * FROM [" + sheet_0_name + "]", excel_cnx))
                        {
                            using (OleDbDataAdapter ole_db_data_adapter = new OleDbDataAdapter())
                            {
                                ole_db_data_adapter.SelectCommand = ole_db_cmd;
                                ole_db_data_adapter.Fill(data_table);
                            }
                        }
                    }

                    excel_cnx.Close();
                }

                return data_table;
            }
            catch (Exception exc)
            {
                Logs.WriteToLogFile(string.Format("[LỖI] LeadHelpers/GetDataFromExcel: {0}", exc.Message));
            }

            return null;
        }

        public bool BulkInsert(string sqlCnnStr, string tableName, DataTable tableOrg)
        {
            if (tableOrg == null || tableOrg.Rows == null || tableOrg.Rows.Count == 0)
            {
                return true;
            }

            SqlBulkCopy bulkCopy = null;

            try
            {
                //sqlCnnStr = @"Persist Security Info=False;Integrated Security=true;Initial Catalog=filmdemo;server=127.0.0.1";
                using (bulkCopy = new SqlBulkCopy(sqlCnnStr))
                {
                    bulkCopy.BatchSize = tableOrg.Rows.Count;
                    bulkCopy.DestinationTableName = tableName;

                    foreach (DataColumn col in tableOrg.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    //DateTime frm = DateTime.Now;
                    bulkCopy.WriteToServer(tableOrg);
                    //var timeSpan = DateTime.Now - frm;
                    //Logs.WriteToLogFile("Bulk.BulkInsertMulti - Time: " + timeSpan.TotalMilliseconds.ToString());
                }

                if (bulkCopy != null)
                {
                    bulkCopy.Close();
                }

                return true;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}