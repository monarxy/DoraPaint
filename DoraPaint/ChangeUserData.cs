using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace MyApp
{
    public partial class ChangeUserData : Form
    {
        private Form2 ParentForm;
        private Form4 PaintForm;
        private string UserId;
        private string login;
        private string password;
        private string email;
        public ChangeUserData(Form2 ParentForm, Form4 PaintForm, string UserId)
        {
            this.ParentForm = ParentForm;
            this.PaintForm = PaintForm;
            this.UserId = UserId;
            InitializeComponent();
        }

        private void ChangeUserData_Load(object sender, EventArgs e)
        {
            var hash = new Hashing();
            var dbQuery = new DBQuery();
            ParentForm.HideButtons();
            using (MySqlConnection con = new MySqlConnection(dbQuery.connection))
            {
                con.Open();
                using (MySqlCommand command = new MySqlCommand($"SELECT * FROM User_Table WHERE UserId = '{UserId}'", con))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            login = reader["Login"].ToString();
                            password = reader["Password"].ToString();
                            email = reader["Email"].ToString();
                            guna2TextBox1.UseSystemPasswordChar = true;
                            guna2TextBox4.UseSystemPasswordChar = true;
                        }
                    }
                }
            }
            guna2TextBox3.Text = login;
            guna2TextBox2.Text = email;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            var Numbers = new Regex(@"[0-9]");
            var BigLetters = new Regex(@"[A-Z]");
            var LittleLetters = new Regex(@"[a-z]");
            var Length = new Regex(@".{6,12}");
            var dbQuery = new DBQuery();
            var check = new CheckingEmailAndLogin();
            var getHash = new Hashing();
            var smtp = new SMTP();

            if (password == getHash.Hash(guna2TextBox1.Text))
            {
                MessageBox.Show("Пароль не должен совпадать с предыдущим");
            }
            if (guna2TextBox1 == null && guna2TextBox4 != null || guna2TextBox1 != null && guna2TextBox4 == null)
            {
                MessageBox.Show("Форма заполнена некорректно");
            }
            if (guna2TextBox4.Text == "" && guna2TextBox1.Text == "" && guna2TextBox2.Text == email && guna2TextBox3.Text == login)
            {
                this.Hide();
                ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
            }
            if (guna2TextBox4.Text == "" && guna2TextBox1.Text! != "" || guna2TextBox4.Text != "" && guna2TextBox1.Text == "")
            {
                MessageBox.Show("Введенные пароли не совпадают");
            }
            if (guna2TextBox4.Text != "" && guna2TextBox1.Text! != "" && guna2TextBox4.Text != guna2TextBox1.Text)
            {
                MessageBox.Show("Пароли должны совпадать");
            }
            if (guna2TextBox4.Text != "" && guna2TextBox1.Text! != "" && guna2TextBox4.Text == guna2TextBox1.Text && guna2TextBox2.Text == email && guna2TextBox3.Text == login)
            {
                if (Length.IsMatch(guna2TextBox1.Text) && Numbers.IsMatch(guna2TextBox1.Text) && BigLetters.IsMatch(guna2TextBox1.Text) && LittleLetters.IsMatch(guna2TextBox1.Text))
                {
                    if (password != getHash.Hash(guna2TextBox1.Text))
                    {
                        dbQuery.queryExecute($"UPDATE User_Table SET Password = '{getHash.Hash(guna2TextBox1.Text)}' WHERE UserId = '{UserId}'");
                        this.Hide();
                        ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
                        smtp.SendMessage($"Ваш новый пароль: {guna2TextBox1.Text}", email);
                    }
                    else MessageBox.Show("Новый пароль совпадает со старым");
                }
                else MessageBox.Show(" 1. Пароль должен содержать хотя бы одну строчную букву \n " +
                    "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                    "4. Длина логина должна быть от 6 до 12 символов");
            }

            //Изменение почты
            if (guna2TextBox4.Text == "" && guna2TextBox1.Text == "" && guna2TextBox2.Text != email && guna2TextBox3.Text == login)
            {
                if (guna2TextBox2.Text.Contains("@gmail.com") || guna2TextBox2.Text.Contains("@yandex.ru") || guna2TextBox2.Text.Contains("@mail.ru"))
                {
                    if (!check.CheckEmail(guna2TextBox2.Text))
                    {
                        dbQuery.queryExecute($"UPDATE User_Table SET Email = '{guna2TextBox2.Text}' WHERE UserId = '{UserId}'");
                        smtp.SendMessage($"Почта в приложении PhotoViewer была изменена на: {guna2TextBox2.Text}", guna2TextBox2.Text);
                        this.Hide();
                        ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
                    }

                    else MessageBox.Show("Почта уже используется");
                }
                else MessageBox.Show("Почта введена некорректно");
            }

            //Изменение логина
            if (guna2TextBox4.Text == "" && guna2TextBox1.Text == "" && guna2TextBox2.Text == email && guna2TextBox3.Text != login)
            {
                if (Numbers.IsMatch(guna2TextBox3.Text) && LittleLetters.IsMatch(guna2TextBox3.Text) && BigLetters.IsMatch(guna2TextBox3.Text) && Length.IsMatch(guna2TextBox3.Text))
                {
                    if (!check.CheckLogin(guna2TextBox3.Text))
                    {
                        if (getHash.Hash(guna2TextBox3.Text) != password)
                        {
                            dbQuery.queryExecute($"UPDATE User_Table SET Login = '{guna2TextBox3.Text}' WHERE UserId = '{UserId}'");
                            smtp.SendMessage($"Логин в приложении PhotoViewer был изменен на: {guna2TextBox3.Text}", email);
                            this.Hide();
                            ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));

                        }
                        else MessageBox.Show("Логин и пароль не должны совпадать");

                    }
                    else MessageBox.Show("Логин уже используется");
                }
                else MessageBox.Show(" 1. Логин должен содержать хотя бы одну строчную букву \n " +
                "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                "4. Длина логина должна быть от 6 до 12 символов");
            }

            //Изменение логина и почты
            if (guna2TextBox4.Text == "" && guna2TextBox1.Text == "" && guna2TextBox2.Text != email && guna2TextBox3.Text != login)
            {
                if (Numbers.IsMatch(guna2TextBox3.Text) && LittleLetters.IsMatch(guna2TextBox3.Text) && BigLetters.IsMatch(guna2TextBox3.Text) && Length.IsMatch(guna2TextBox3.Text))
                {
                    if (!check.CheckLogin(guna2TextBox3.Text))
                    {
                        if (!check.CheckEmail(guna2TextBox2.Text))
                        {
                            if (getHash.Hash(guna2TextBox3.Text) != password)
                            {
                                if (guna2TextBox2.Text.Contains("@yandex.ru") || guna2TextBox2.Text.Contains("@mail.ru") || guna2TextBox2.Text.Contains("@gmail.com"))
                                {
                                    dbQuery.queryExecute($"UPDATE User_Table SET Login = '{guna2TextBox3.Text}', Email = '{guna2TextBox2.Text}' WHERE UserId = '{UserId}'");
                                    smtp.SendMessage($"Новый логин: {guna2TextBox3.Text} и новая почта: {guna2TextBox2.Text} для аккаунта PhotoViewer", guna2TextBox2.Text);
                                    this.Hide();
                                    ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
                                }
                                else MessageBox.Show("Почта введена некорректно");
                            }
                            else MessageBox.Show("Логин и пароль не должны совпадать");
                        }
                        else MessageBox.Show("Почта уже используется");
                    }
                    else MessageBox.Show("Логин уже используется");
                }
                else MessageBox.Show(" 1. Логин должен содержать хотя бы одну строчную букву \n " +
                "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                "4. Длина логина должна быть от 6 до 12 символов");
            }

            //Изменили пароль, почту и логин

            if (guna2TextBox4.Text != "" && guna2TextBox1.Text != "" && guna2TextBox2.Text != email && guna2TextBox3.Text != login)
            {
                if (guna2TextBox1.Text == guna2TextBox4.Text)
                {
                    if (Numbers.IsMatch(guna2TextBox3.Text) && LittleLetters.IsMatch(guna2TextBox3.Text) && BigLetters.IsMatch(guna2TextBox3.Text) && Length.IsMatch(guna2TextBox3.Text))
                    {
                        if (!check.CheckLogin(guna2TextBox3.Text))
                        {
                            if (!check.CheckEmail(guna2TextBox2.Text))
                            {
                                if (getHash.Hash(guna2TextBox3.Text) != password)
                                {
                                    if (guna2TextBox2.Text.Contains("@yandex.ru") || guna2TextBox2.Text.Contains("@mail.ru") || guna2TextBox2.Text.Contains("@gmail.com"))
                                    {
                                        if (Numbers.IsMatch(guna2TextBox1.Text) && LittleLetters.IsMatch(guna2TextBox1.Text) && BigLetters.IsMatch(guna2TextBox1.Text) && Length.IsMatch(guna2TextBox1.Text))
                                        {
                                            if (password != getHash.Hash(guna2TextBox1.Text))
                                            {
                                                dbQuery.queryExecute($"UPDATE User_Table SET Login = '{guna2TextBox3.Text}', Email = '{guna2TextBox2.Text}', Password = '{getHash.Hash(guna2TextBox1.Text)}' WHERE UserId = '{UserId}'");
                                                smtp.SendMessage($"Новый логин: {guna2TextBox3.Text}, новая почта: {guna2TextBox2.Text} и новый пароль: {guna2TextBox1.Text} для аккаунта PhotoViewer", guna2TextBox2.Text);
                                                this.Hide();
                                                ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
                                            }
                                            else MessageBox.Show("Новый пароль совпадает со старым");

                                        }
                                        else MessageBox.Show(" 1. Пароль должен содержать хотя бы одну строчную букву \n " +
                                        "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                                        "4. Длина логина должна быть от 6 до 12 символов");
                                    }
                                    else MessageBox.Show("Почта введена некорректно");
                                }
                                else MessageBox.Show("Логин и пароль не должны совпадать");
                            }
                            else MessageBox.Show("Почта уже используется");
                        }
                        else MessageBox.Show("Логин уже используется");
                    }
                    else MessageBox.Show(" 1. Логин должен содержать хотя бы одну строчную букву \n " +
                    "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                    "4. Длина логина должна быть от 6 до 12 символов");
                }
                else MessageBox.Show("Введенные пароли не совпадают");
            }



            ////Изменили пароль и почту

            if (guna2TextBox4.Text != "" && guna2TextBox1.Text != "" && guna2TextBox2.Text != email && guna2TextBox3.Text == login)
            {
                if (guna2TextBox1.Text == guna2TextBox4.Text)
                {
                    if (!check.CheckEmail(guna2TextBox2.Text))
                    {
                        if (getHash.Hash(guna2TextBox3.Text) != password)
                        {
                            if (guna2TextBox2.Text.Contains("@yandex.ru") || guna2TextBox2.Text.Contains("@mail.ru") || guna2TextBox2.Text.Contains("@gmail.com"))
                            {
                                if (Numbers.IsMatch(guna2TextBox1.Text) && LittleLetters.IsMatch(guna2TextBox1.Text) && BigLetters.IsMatch(guna2TextBox1.Text) && Length.IsMatch(guna2TextBox1.Text))
                                {
                                    if (password != getHash.Hash(guna2TextBox1.Text))
                                    {
                                        dbQuery.queryExecute($"UPDATE User_Table SET Password = '{getHash.Hash(guna2TextBox1.Text)}', Email = '{guna2TextBox2.Text}' WHERE UserId = '{UserId}'");
                                        smtp.SendMessage($"Новая почта: {guna2TextBox2.Text} и новый пароль: {guna2TextBox1.Text} для аккаунта PhotoViewer", guna2TextBox2.Text);
                                        this.Hide();
                                        ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
                                    }
                                    else MessageBox.Show("Новый пароль совпадает со старым");

                                }
                                else MessageBox.Show(" 1. Пароль должен содержать хотя бы одну строчную букву \n " +
                                "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                                "4. Длина логина должна быть от 6 до 12 символов");
                            }
                            else MessageBox.Show("Почта введена некорректно");
                        }
                        else MessageBox.Show("Логин и пароль не должны совпадать");
                    }
                    else MessageBox.Show("Почта уже используется");
                }
                else MessageBox.Show("Введенные пароли не совпадают");
            }


            if (guna2TextBox4.Text != "" && guna2TextBox1.Text != "" && guna2TextBox2.Text == email && guna2TextBox3.Text != login)
            {
                if (guna2TextBox1.Text == guna2TextBox4.Text)
                {
                    if (Numbers.IsMatch(guna2TextBox3.Text) && LittleLetters.IsMatch(guna2TextBox3.Text) && BigLetters.IsMatch(guna2TextBox3.Text) && Length.IsMatch(guna2TextBox3.Text))
                    {
                        if (!check.CheckLogin(guna2TextBox3.Text))
                        {
                            if (getHash.Hash(guna2TextBox3.Text) != password)
                            {
                                if (Numbers.IsMatch(guna2TextBox1.Text) && LittleLetters.IsMatch(guna2TextBox1.Text) && BigLetters.IsMatch(guna2TextBox1.Text) && Length.IsMatch(guna2TextBox1.Text))
                                {
                                    if (password != getHash.Hash(guna2TextBox1.Text))
                                    {
                                        dbQuery.queryExecute($"UPDATE User_Table SET Login = '{guna2TextBox3.Text}', Password = '{getHash.Hash(guna2TextBox1.Text)}' WHERE UserId = '{UserId}'");
                                        smtp.SendMessage($"Новый логин: {guna2TextBox3.Text} и новый пароль: {guna2TextBox1.Text} для аккаунта PhotoViewer", guna2TextBox2.Text);
                                        this.Hide();
                                        ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
                                    }
                                    else MessageBox.Show("Новый пароль совпадает со старым");

                                }
                                else MessageBox.Show(" 1. Пароль должен содержать хотя бы одну строчную букву \n " +
                                "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                                "4. Длина логина должна быть от 6 до 12 символов");
                            }
                            else MessageBox.Show("Логин и пароль не должны совпадать");
                        }
                        else MessageBox.Show("Логин уже используется");
                    }
                    else MessageBox.Show(" 1. Логин должен содержать хотя бы одну строчную букву \n " +
                    "2. Логин должен содержать хотя бы одну цифру \n " + "3. Логин должен содержать хотя бы одну прописную букву \n " +
                    "4. Длина логина должна быть от 6 до 12 символов");
                }
                else MessageBox.Show("Введенные пароли не совпадают");
            }

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ParentForm.OpenForm(new Form3(ParentForm, PaintForm, UserId));
        }
    }
}
