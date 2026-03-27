using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YugiohShop
{
    public static class DbHelper
    {
        public static  DataTable Query(string sql)
        {
            using var conn = new SqlConnection(DbConfig.ConnectionString);
            using var da = new SqlDataAdapter(sql, conn);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static int Execute(string sql)
        {
            using var conn = new SqlConnection(DbConfig.ConnectionString);
            using var cmd = new SqlCommand(sql, conn);

            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
