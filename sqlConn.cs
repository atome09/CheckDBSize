using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CheckDBSize
{
    class sqlConn
    {
        private static SqlConnection conn;
        private static SqlCommand cmd;
        private static SqlDataAdapter da;
        private static DataSet ds;
        private static SqlDataReader dr;

        public static string Connection(string sqlconn)
        {
            try
            {
                //string sqlconn = "Data Source=storesysdb01;Initial Catalog=StoreSys Integrated Security=SSPI";
                conn = new SqlConnection(sqlconn);
                conn.Open();
            }
            catch (SqlException e)
            {
                return e.Message;


            }

            return conn.State.ToString();


        }

        public static void Terminate()
        {
            conn.Close();
            conn.Dispose();
        }


        public static DataTable sqlQuery(String sql)
        {
            ds = new DataSet();
            cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            return ds.Tables[0];
        }

        public static int ExecuteNonQuery(string sql)
        {
            try
            {
                int affec;
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Connection = conn;
                affec = cmd.ExecuteNonQuery();
                return affec;
            }
            catch (SqlException ex)
            {
                throw ex;
            }

            return -1;

        }



    }
}
