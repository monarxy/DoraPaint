using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Types;
using MySql.Data.MySqlClient;

namespace MyApp
{


    public class DataIntoUserForm
    {
        public string UserId;
        public string Email;
        public string Login;
        public string Password;

        public DataIntoUserForm(string UserId)
        {
            this.UserId = UserId;
        }
        public void UserLoad()
        {
            var dbQuery = new DBQuery();
            using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM User_Table WHERE UserId = '{UserId}'", con))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Login = reader["Login"].ToString();
                            Email = reader["Email"].ToString();
                            Password = reader["Password"].ToString();
                        }
                    }
                }
            }
        }
    }
}
