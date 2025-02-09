using Guna.UI2.WinForms;
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
//using Windows.Security.Authentication.OnlineId;

namespace MyApp
{
    public partial class Form3 : Form
    {
        private Form4 PaintForm;
        private Form2 ParentForm;
        private string UserId;
        public Form3(Form2 ParentForm, Form4 PaintForm, string UserId)
        {
            this.PaintForm = PaintForm;
            this.ParentForm = ParentForm;
            this.UserId = UserId;
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadPersonalData();
        }

        private void LoadPersonalData()
        {
            DataIntoUserForm User = new DataIntoUserForm(UserId);
            User.UserLoad();
            guna2TextBox1.Text = User.UserId;
            guna2TextBox3.Text = User.Login;
            guna2TextBox2.Text = User.Email;
            guna2TextBox4.Text = User.Password;
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            ParentForm.ShowButtons();
            ParentForm.OpenForm(new ChangePassword(ParentForm, PaintForm, UserId));
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ParentForm.ShowButtons();
            ParentForm.OpenForm(PaintForm);
        }

        private void guna2TextBox4_Click(object sender, EventArgs e)
        {
            guna2TextBox4.UseSystemPasswordChar = true;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Properties.Settings.Default.userid = "";
            Properties.Settings.Default.Save();
            ParentForm.ButtonChangeBack();
            ParentForm.ShowButtons();
            ParentForm.OpenForm(PaintForm);
        }
    }
}
