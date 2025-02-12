using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.Types;
using MySql.Data.MySqlClient;

namespace MyApp
{
    public partial class ChangePassword : Form
    {
        private Form4 PaintForm;
        private Form2 ParentForm;
        private string UserId;
        public ChangePassword(Form2 ParentForm, Form4 PaintForm, string UserId)
        {
            this.PaintForm = PaintForm;
            this.UserId = UserId;
            this.ParentForm = ParentForm;
            InitializeComponent();
        }

        private void ChangePassword_Load1(object sender, EventArgs e)
        {
            ParentForm.HideButtons();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            
            ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
        }

        private bool PasswordCheck(string Password)
        {
            var dbQuery = new DBQuery();
            var getHash = new Hashing();
            var exists = false;
            try
            {
                using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
                {
                    con.Open();
                    using (MySqlCommand command = new MySqlCommand($"SELECT * FROM User_Table WHERE UserId = '{UserId}'", con))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                if (reader["Password"].ToString() == getHash.Hash(Password))
                                    exists = true;
                                else
                                {
                                    guna2TextBox1.Clear();
                                    MessageBox.Show("Пароль введен некорректно");
                                }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Соединение с сервером недоступно, попробуйте еще раз.");
                this.Hide();
                ParentForm.HideFileButtons();
                ParentForm.OpenForm(PaintForm);
                ParentForm.ShowButtons();
            }
            return exists;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
                if (PasswordCheck(guna2TextBox1.Text))
                    ParentForm.OpenForm(new ChangeUserData(ParentForm, PaintForm, UserId));
        }
        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked)
            guna2TextBox1.UseSystemPasswordChar = true;
            else
                guna2TextBox1.UseSystemPasswordChar = false;
        }
    }

}
