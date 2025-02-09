using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MySql.Data.Types;
using MySql.Data.MySqlClient;

namespace MyApp
{
    public class DBQuery
    {
        // public string connection = @"server=31.31.198.110;uid=u2690469_default;pwd=168M7ZzqB6HJ2sty;database=u2690469_default";
        public string connection = @"server=sohumufo.beget.app;uid=default-db;pwd=oy33jS!7FF5t;database=default-db";
        public void queryExecute(string query)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            conn.Open();
            MySqlDataAdapter SDA = new MySqlDataAdapter(query, conn);
            SDA.SelectCommand.ExecuteNonQuery();
            conn.Close();
        }
    }
}
