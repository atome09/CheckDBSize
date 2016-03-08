using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace CheckDBSize
{
    class Program
    {
        static void Main(string[] args)
        {
            string sqlcon = "db01;Database=Store; Integrated Security=SSPI";
            string _sqlcon = sqlConn.Connection(sqlcon);
            Console.WriteLine(_sqlcon);
            string sqlQuery = "SELECT StoreId,Status,IPAddress FROM stores where status = 'Open'";
            DataTable getSql = sqlConn.sqlQuery(sqlQuery);

            string SqlTrunc = "TRUNCATE TABLE StoresDBSize";
            sqlConn.ExecuteNonQuery(SqlTrunc);

            foreach (DataRow row in getSql.Rows)
            {
                string storeDB = "";
                string storeid = row["storeid"].ToString();
                string ipAddress = row["IPAddress"].ToString();

                storeDB = "Server=" + ipAddress + ";Database=xstore;User Id=user;Password=pass;Connection Timeout=60";

                using (SqlConnection _storeDB = new SqlConnection())
                {
                    try
                    {
                        _storeDB.ConnectionString = storeDB;
                        _storeDB.Open();

                        string insertCmd = "";
                        string DBcmd = "SELECT DB_NAME() AS Name," +
                                           "(size * 8) / 1024 DBSizeMB," +
                                           "b.LogSizeMB" +
                                        " FROM sys.master_files a join" +
                                                " ( SELECT DB_NAME() AS DatabaseName, database_id," +
                                                       " Name AS LogFile," +
                                                        "Physical_Name, (size * 8) / 1024 LogSizeMB" +
                                                    " FROM sys.master_files" +
                                                    " WHERE DB_NAME(database_id) = 'xstore' and Name like '%_log%' ) b" +
                                                " on a.database_id = b.database_id" +
                                        " WHERE DB_NAME(a.database_id) = 'xstore' and Name like '%_data%'";

                        if (_storeDB.State == ConnectionState.Open)
                        {
                            using (SqlCommand sqlCmd = new SqlCommand(DBcmd, _storeDB))
                            {
                                sqlCmd.CommandType = CommandType.Text;
                                SqlDataReader reader = sqlCmd.ExecuteReader();

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {

                                        string dbSize = reader["DBSizeMB"].ToString();
                                        string logSize = reader["LogSizeMB"].ToString();
                                        Console.WriteLine("Name: " + reader["name"]);
                                        Console.WriteLine("Size: " + reader["DBSizeMB"]);

                                        insertCmd = "INSERT INTO StoresDBSize (StoreId,IPAddress,DBSizeMB,LogSizeMB,updateDate) VALUES ( " + "'" + storeid + "'," + "'" + ipAddress + "'," + "'" + dbSize + "'," + "'" + logSize + "',"+ "'" +DateTime.Now +"')";
                                        sqlConn.ExecuteNonQuery(insertCmd);

                                    }
                                }

                            }
                        }
                    }
                    catch(SqlException e)
                    {
                        Console.WriteLine(e.Message);
                        string insertError = "INSERT INTO StoresDBSize (StoreId,IPAddress,errorMsg,updateDate) VALUES ( " + "'" + storeid + "'," + "'" + ipAddress + "'," + "'" + e.Message + "'," + "'" + DateTime.Now + "')";
                        sqlConn.ExecuteNonQuery(insertError);
                    }

                    _storeDB.Close();
                    
                }

                                    
            }
            sqlConn.Terminate();
            Console.WriteLine("Process Done");
            //Console.Read();


        }
    }
}
