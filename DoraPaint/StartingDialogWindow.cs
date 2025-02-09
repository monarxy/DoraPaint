using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TheArtOfDevHtmlRenderer.Adapters;

namespace MyApp
{

    public partial class StartingDialogWindow : Form
    {
        private int width = 0;
        private int height = 0;
        private int _current_paint_form;
        private Form2 ParentForm;
        private Form4 PaintForm;
        private Bitmap bmp;
        public StartingDialogWindow(Form2 ParentForm, int current_paint_form, Form4 PaintForm)
        {

            this.ParentForm = ParentForm;
            this.PaintForm = PaintForm;
            InitializeComponent();
            _current_paint_form = current_paint_form;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (ParentForm.ReturnNumberOfPaintForms() < 3)
            {
                if (textBox1.Text != "" && textBox1.Text != "0" && textBox2.Text != "" && textBox2.Text != "0")
                {
                    ParentForm.SetStartingWidth(Int32.Parse(textBox1.Text));
                    ParentForm.SetStartingHeight(Int32.Parse(textBox2.Text));
                    this.Hide();
                    ParentForm.ShowButtons();
                    Form4 paintingForm = new Form4(ParentForm, Int32.Parse(textBox1.Text), Int32.Parse(textBox2.Text), _current_paint_form + 1);
                    ParentForm.AddPaintingForm(paintingForm);
                    ParentForm.OpenForm(paintingForm);
                    ParentForm.HideFileButtons();
                    ParentForm.ChangeCurrent();
                    ParentForm.SetNewFileName();
                    ParentForm.SetChildFormName("paint");
                }
                else
                    MessageBox.Show("Параметры файла введены некорректно!");
            }
            else
                MessageBox.Show("Одновременно можно создать только три файла!");

        }

        public void HideButton()
        {
            guna2Button2.Visible = false;
            label3.Location = new Point(38, 30) ;
            label1.Location = new Point(38, 95);
            label2.Location = new Point(38, 136);
            label4.Location = new Point(38, 176);
            textBox1.Location = new Point(112, 94);
            textBox2.Location = new Point(112, 133);
            guna2Button1.Location = new Point(38, 402);
            guna2PictureBox1.Location = new Point(38, 208);
        }

        private void guna2PictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void guna2PictureBox1_DragDrop(object sender, DragEventArgs e)
        {


            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var filenames = data as string[];
                if (filenames.Length == 1)
                {
                    this.Hide();
                    ParentForm.ShowButtons();

                    Form4 paintingForm = new Form4(ParentForm, _current_paint_form + 1, new Bitmap(Image.FromFile(filenames[0])));
                    ParentForm.AddPaintingForm(paintingForm);
                    ParentForm.OpenForm(paintingForm);
                    ParentForm.ChangeCurrent();
                    ParentForm.HideFileButtons();
                    ParentForm.SetChildFormName("paint");

                }
                else MessageBox.Show("Перетащить можно только один файл.");
            }
        }

        public void PictureBoxMax()
        {
            guna2Panel1.Size = new Size(2000, 1200);
            guna2PictureBox1.Size = new Size(2000, 1200);
        }

        public void PictureBoxMin()
        {
            guna2Panel1.Size = new Size(1252, 693);

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ParentForm.OpenForm(new Form5(ParentForm, PaintForm, PaintForm.ReturnBitmap(), _current_paint_form));
            ParentForm.SetChildFormName("saving_window");
        }
    }
}
