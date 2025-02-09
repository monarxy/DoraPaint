using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Drawing2D;
using System.Windows.Media;
using Guna.UI2.WinForms;

namespace MyApp
{
    public partial class Form5 : Form
    {
        private Form2 ParentForm;
        private Form4 PaintForm;
        private Bitmap Current_bitmap;
        private string filename;
        private int Current;
        public Form5(Form2 ParentForm, Form4 PaintForm, Bitmap Current_bitmap, int current)
        {
            this.ParentForm = ParentForm;
            this.PaintForm = PaintForm;
            this.Current_bitmap = Current_bitmap;
            this.Current = current;
            InitializeComponent();

        }

        private void Form5_Load(object sender, EventArgs e)
        {
            if (Current_bitmap.Width <= 1000 && Current_bitmap.Height <= 800)
            {
                int w = (int)Math.Round((double)Current_bitmap.Width / 5, MidpointRounding.AwayFromZero);
                int h = (int)Math.Round((double)Current_bitmap.Height / 5, MidpointRounding.AwayFromZero);
                Bitmap resized_bitmap = new Bitmap(Current_bitmap, w, h);
                guna2PictureBox1.Image = resized_bitmap;
            }
            if (Current_bitmap.Width > 1000 && Current_bitmap.Height > 800 && Current_bitmap.Width <= 3000 && Current_bitmap.Height <= 2000)
            {
                int w = (int)Math.Round((double)Current_bitmap.Width / 10, MidpointRounding.AwayFromZero);
                int h = (int)Math.Round((double)Current_bitmap.Height / 10, MidpointRounding.AwayFromZero);
                Bitmap resized_bitmap = new Bitmap(Current_bitmap, w, h);
                guna2PictureBox1.Image = resized_bitmap;
            }
            else
            {
                int w = (int)Math.Round((double)Current_bitmap.Width / 15, MidpointRounding.AwayFromZero);
                int h = (int)Math.Round((double)Current_bitmap.Height / 15, MidpointRounding.AwayFromZero);
                Bitmap resized_bitmap = new Bitmap(Current_bitmap, w, h);
                guna2PictureBox1.Image = resized_bitmap;
            }
            label1.Text = $"{Current_bitmap.Width} * {Current_bitmap.Height}";
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            ParentForm.ShowButtons();
            ParentForm.HideFileButtons();
            ParentForm.OpenForm(PaintForm);
            ParentForm.SetChildFormName("paint");
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintPageHandler;
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
                printDialog.Document.Print();
        }

        void PrintPageHandler(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(Current_bitmap, 0, 0);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            for (int width = 1; width < Current_bitmap.Width; width++)
                for (int height = 1; height < Current_bitmap.Height; height++)
                    if (Current_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                        Current_bitmap.SetPixel(width, height, System.Drawing.Color.Transparent);

            filename = ParentForm.FileName(Current);
            if (filename != "Несохраненный")
                Current_bitmap.Save(filename);
            else
            {
                if (comboBox1.Text == ".jpg")
                    saveFileDialog1.Filter = "JPG(*.JPG)|*.jpg";
                if (comboBox1.Text == ".png")
                    saveFileDialog1.Filter = "PNG(*.PNG)|*.png";
                if (comboBox1.Text == ".tiff")
                    saveFileDialog1.Filter = "TIFF(*.TIFF)|*.tiff";
                if (comboBox1.Text == ".svg")
                    saveFileDialog1.Filter = "SVG(*.SVG)|*.svg";
                if (comboBox1.Text == ".raw")
                    saveFileDialog1.Filter = "RAW(*.RAW)|*.raw";
                if (comboBox1.Text == ".bmp")
                    saveFileDialog1.Filter = "BMP(*.BMP)|*.bmp";
                else
                    saveFileDialog1.Filter = "PNG(*.PNG)|*.png";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (Current_bitmap != null)
                        Current_bitmap.Save(saveFileDialog1.FileName);
                    ParentForm.SetFileName(saveFileDialog1.FileName);
                    ParentForm.ChangeNameOfPaintForm(saveFileDialog1.FileName, Current);
                    ParentForm.ChangeBool(false, Current);
                }
            }
            ParentForm.BitmapChangedFalse();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            ParentForm.OpenForm(new StartingDialogWindow(ParentForm, Current, PaintForm));
            ParentForm.SetChildFormName("starting_window");
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
    }
}
