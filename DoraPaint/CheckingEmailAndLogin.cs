using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace MyApp
{
    public class CheckingEmailAndLogin
    {
        public bool CheckEmail(string email)
        {
            bool exists = false;
            var dbQuery = new DBQuery();
            var getHash = new Hashing();

            using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
            {
                con.Open();
                using (MySqlCommand command = new MySqlCommand($"SELECT * FROM User_Table WHERE Email ='{email}'", con))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Email"].ToString() == email)
                                exists = true;
                        }
                    }
                }
            }

            return exists;
        }


        public bool CheckLogin(string login)
        {
            bool exists = false;
            var dbQuery = new DBQuery();
            var getHash = new Hashing();

            using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
            {
                con.Open();
                using (MySqlCommand command = new MySqlCommand($"SELECT * FROM User_Table WHERE Login ='{login}'", con))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["Login"].ToString() == login)
                                exists = true;
                        }
                    }
                }
            }
            return exists;
        }
    }
}
