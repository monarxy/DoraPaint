using Microsoft.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Windows.Forms;
using System;

namespace MyApp
{
    public partial class Form1 : Form
    {
        private Form2 MainForm;
        private string UserId;
        public Form1(Form2 MainForm, string UserId)
        {
            this.UserId = UserId;
            this.MainForm = MainForm;
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {


            if (string.IsNullOrEmpty(guna2TextBox1.Text) || string.IsNullOrEmpty(guna2TextBox2.Text))
            {
                MessageBox.Show("Форма заполнена некорректно!");
            }
            else
            {
                Register(guna2TextBox1.Text, guna2TextBox2.Text, guna2TextBox5.Text);
            }

        }

        private void Register(string login, string password, string email)
        {
            var Number = new Regex(@"[0-9]");
            var BigLetter = new Regex(@"[A-Z]");
            var Length = new Regex(@".{6,12}");
            var LittleLetter = new Regex(@"[a-z]");
            var smtp = new SMTP();
            var check = new CheckingEmailAndLogin();

            try
            {
                if (login == password)
                {
                    MessageBox.Show("Пароль и логин не должны совпадать");
                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();
                    return;
                }
                if (check.CheckLogin(login))
                {
                    MessageBox.Show("Логин уже используется");
                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();
                    return;
                }

                if (check.CheckEmail(email))
                {
                    MessageBox.Show("Почта уже используется");
                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();
                    return;
                }

                if (!Number.IsMatch(login) || !Length.IsMatch(login) || !LittleLetter.IsMatch(login) || !BigLetter.IsMatch(login))
                {
                    MessageBox.Show(" 1. Логин должен содержать хотя бы одну строчную букву \n " +
                        "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                        "4. Длина логина должна быть от 6 до 12 символов");

                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();
                    return;
                }

                if (!Number.IsMatch(password) || !Length.IsMatch(password) || !LittleLetter.IsMatch(password) || !BigLetter.IsMatch(password))
                {
                    MessageBox.Show(" 1. Пароль должен содержать хотя бы одну строчную букву \n " +
                        "2. Пароль должен содержать хотя бы одну цифру \n " + "3. Пароль должен содержать хотя бы одну прописную букву \n " +
                        "4. Длина пароля должна быть от 6 до 12 символов");

                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();
                    return;
                }

                if (!email.Contains("gmail.com") && !email.Contains("yandex.ru") && !email.Contains("mail.ru"))
                {
                    MessageBox.Show("Ведите электронную почту корректно");
                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();
                    return;
                }

                else
                {
                    while (true)
                    {
                        Random generator = new Random();
                        string userId = generator.Next(100000, 999999).ToString();
                        if (!CheckUserId(userId))
                        {
                            UserId = userId;
                            break;
                        }
                    }

                    Hashing GH = new Hashing();
                    string query = $"INSERT INTO User_Table(Email, Password, Login, UserId) VALUES('{email}', '{GH.Hash(password)}', '{login}', '{UserId}')";

                    var dbQuery = new DBQuery();
                    dbQuery.queryExecute(query);

                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    guna2TextBox5.Clear();

                    smtp.SendMessage($" Ваш логин:{login}\n Пароль: {password}", email);
                }
            }
            catch
            {
                MessageBox.Show("Соединение с сервером недоступно, попробуйте еще раз.");
            }

        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2TextBox3.Text != null && guna2TextBox4.Text != null)
            {
                AuthoriseUser(guna2TextBox3.Text, guna2TextBox4.Text);
            }
            else
            {
                MessageBox.Show("Введены не все данные");
                guna2TextBox3.Clear();
                guna2TextBox4.Clear();
            }
        }

        private void AuthoriseUser(string login, string password)
        {
            bool isAuthorised = false;
            var dbQuery = new DBQuery();
            var getHash = new Hashing();

            try
            {
                using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
                {
                    con.Open();
                    using (MySqlCommand command = new MySqlCommand($"SELECT * FROM User_Table WHERE Login ='{login}' and Password = '{getHash.Hash(password)}'", con))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Password"].ToString() == getHash.Hash(password) && reader["Login"].ToString() == login)
                                {
                                    isAuthorised = true;
                                    UserId = reader["UserId"].ToString();
                                    Properties.Settings.Default.userid = UserId;
                                    Properties.Settings.Default.Save();
                                    MainForm.ButtonChange();
                                    MainForm.ChangeUserId(UserId);
                                    MainForm.Show();
                                    this.Hide();
                                }
                            }
                            else
                                MessageBox.Show("Данные введены некорректно");
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Соединение с сервером недоступно, попробуйте еще раз.");
            }
        }


        private bool CheckLogin(string login)
        {
            bool exists = false;
            var dbQuery = new DBQuery();

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

        private bool CheckUserId(string UserId)
        {
            bool exists = false;
            var dbQuery = new DBQuery();

            using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
            {
                con.Open();
                using (MySqlCommand command = new MySqlCommand($"SELECT * FROM User_Table WHERE UserId = '{UserId}'", con))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["UserId"].ToString() == UserId)
                                exists = true;
                        }
                    }
                }
            }
            return exists;
        }


        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox1.Checked)
                guna2TextBox2.UseSystemPasswordChar = true;
            else
                guna2TextBox2.UseSystemPasswordChar = false;
        }
        private void guna2CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CheckBox2.Checked)
                guna2TextBox4.UseSystemPasswordChar = true;
            else guna2TextBox4.UseSystemPasswordChar = false;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MainForm.ButtonChangeBack();
            MainForm.Show();
            this.Hide();
        }
    }
}
