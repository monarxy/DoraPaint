using Guna.UI2.AnimatorNS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Windows.Security.Authentication.OnlineId;
//using Windows.Storage.Pickers.Provider;

namespace MyApp
{
    public partial class Form2 : Form
    {
        private string UserId;
        private Form4 PaintForm;
        private Form5 FileForm;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private Color color = Color.Black;
        private int width;
        private int eraser_width;
        private bool crop_value = false;
        private bool maximized = false;
        private bool ellipse_value = false;
        private bool square_value = false;
        private bool pipette_regime = false;
        private bool pipette_color = false;
        private bool cut_regime = false;
        private bool line_regime = false;
        private bool eraser_regime = false;
        private string saved_file_name = "";
        private bool bitmap_changed = false;
        private int bitmap_width;
        private int bitmap_height;
        private List<Form> list_of_painting_forms = new List<Form>();
        private List<string> list_of_saved_names = new List<string>();
        private int current_paint_form = -1;
        private List<bool> list_of_bools = new List<bool>();
        private string childform_name;
        private StartingDialogWindow Starting_dialog;
        private Form5 Saving_window;
        private Bitmap bitmap_from_file;
        private bool bitmap_opened_from_file = false;
        private int painting_forms_counter = 0;

        public Form2(string UserId)
        {
            this.UserId = UserId;
            InitializeComponent();
        }

        public Form2(string UserId, Bitmap bmp)
        {
            this.UserId = UserId;
            bitmap_from_file = bmp;
            bitmap_opened_from_file = true;
            InitializeComponent();
        }

        public void OpenForm(Form childForm)
        {
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            guna2Panel7.Controls.Add(childForm);
            guna2Panel7.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            if (childForm.Name == "Form3")
                guna2Button1.Visible = false;
            if (childForm.Name == "Form4")
                guna2Button1.Visible = true;
        }

        public void AddPaintingForm(Form4 paintingForm)
        {
            list_of_painting_forms.Add(paintingForm);
            list_of_saved_names.Add("Несохраненный");
            list_of_bools.Add(false);
            painting_forms_counter += 1;
            PaintForm = paintingForm;
        }
        public void ShowButtons()
        {
            guna2Button1.Visible = true;
            guna2CircleButton1.Visible = true;
            guna2Button2.Visible = true;
            guna2Button3.Visible = true;
            guna2Button4.Visible = true;
            trackBar1.Visible = true;
            iconButton4.Visible = true;
            guna2Button7.Visible = true;
            guna2Button8.Visible = true;
            guna2Button9.Visible = true;
            guna2Button10.Visible = true;
            guna2Button11.Visible = true;
            guna2Button12.Visible = true;
            panel1.Visible = true;
            iconButton5.Visible = true;
            iconButton6.Visible = true;
            iconButton7.Visible = true;
            iconButton8.Visible = true;
            iconButton9.Visible = true;
            iconButton10.Visible = true;
            iconButton11.Visible = true;
            iconButton12.Visible = true;
            iconButton16.Visible = true;

            if (painting_forms_counter == 1)
            {
                guna2Button5.Visible = true;
                iconButton13.Visible = true;
            }
            if (painting_forms_counter == 2)
            {
                guna2Button5.Visible = true;
                iconButton13.Visible = true;
                guna2Button6.Visible = true;
                iconButton14.Visible = true;

            }
            if (painting_forms_counter == 3)
            {
                guna2Button5.Visible = true;
                iconButton13.Visible = true;
                guna2Button6.Visible = true;
                iconButton14.Visible = true;
                guna2Button13.Visible = true;
                iconButton15.Visible = true;
            }
        }

        public void HideButtons()
        {
            guna2Button1.Visible = false;
            guna2CircleButton1.Visible = false;
            guna2Button2.Visible = false;
            guna2Button3.Visible = false;
            guna2Button4.Visible = false;
            trackBar1.Visible = false;
            iconButton4.Visible = false;
            guna2Button7.Visible = false;
            guna2Button8.Visible = false;
            guna2Button9.Visible = false;
            guna2Button10.Visible = false;
            guna2Button11.Visible = false;
            guna2Button12.Visible = false;
            panel1.Visible = false;
            iconButton5.Visible = false;
            iconButton6.Visible = false;
            iconButton7.Visible = false;
            iconButton8.Visible = false;
            iconButton9.Visible = false;
            iconButton10.Visible = false;
            iconButton11.Visible = false;
            iconButton12.Visible = false;
            guna2Button5.Visible = false;
            guna2Button6.Visible = false;
            guna2Button13.Visible = false;
            iconButton13.Visible = false;
            iconButton14.Visible = false;
            iconButton15.Visible = false;
            guna2Button14.Visible = false;
            iconButton16.Visible = false;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            HideButtons();
            if (UserId != "")
            {
                guna2Button1.Text = "Мой аккаунт";
                guna2CircleButton1.Text = "";
            }
            if (!bitmap_opened_from_file)
            {
                StartingDialogWindow starting_window = new StartingDialogWindow(this, current_paint_form, PaintForm);
                starting_window.HideButton();
                OpenForm(starting_window);
            }
                
            else
            {
                ShowButtons();
                Form4 paintingForm = new Form4(this, current_paint_form, bitmap_from_file);
                AddPaintingForm(paintingForm);
                ChangeCurrent();
                OpenForm(paintingForm);
            }
            guna2Button5.Text = "Несохраненный";
            guna2Button6.Text = "Несохраненный";
            guna2Button13.Text = "Несохраненный";
        }

        public void HideFileButtons()
        {

            if (list_of_painting_forms.Count() == 1)
            {
                guna2Button5.Visible = true;
                iconButton13.Visible = true;
                guna2Button6.Visible = false;
                iconButton14.Visible = false;
                guna2Button13.Visible = false;
                iconButton15.Visible = false;
            }
            if (list_of_painting_forms.Count() == 2)
            {
                guna2Button5.Visible = true;
                iconButton13.Visible = true;
                guna2Button6.Visible = true;
                iconButton14.Visible = true;
                guna2Button13.Visible = false;
                iconButton15.Visible = false;
            }
            if (list_of_painting_forms.Count() == 3)
            {
                guna2Button5.Visible = true;
                iconButton13.Visible = true;
                guna2Button6.Visible = true;
                iconButton14.Visible = true;
                guna2Button13.Visible = true;
                iconButton15.Visible = true;
            }
        }

        public void SetStartingWidth(int width)
        {
            bitmap_width = width;
        }

        public void SetStartingHeight(int height)
        {
            bitmap_height = height;
        }
        public void ButtonChange()
        {
            guna2Button1.Text = "Мой аккаунт";
        }

        public void ButtonChangeBack()
        {
            guna2Button1.Text = "Регистрация/Вход";
        }

        private void guna2Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void guna2Panel1_MouseUp_1(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void guna2Panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }
        public void ChangeUserId(string userId)
        {
            UserId = userId;
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            if (guna2Button1.Text == "Мой аккаунт")
            {
                HideButtons();
                OpenForm(new Form3(this, PaintForm, UserId));
            }

            else
            {
                Form1 RegistryForm = new Form1(this, "");
                RegistryForm.Show();
            }
        }

        public bool ReturnBitmapChanged()
        {
            return bitmap_changed;
        }

        public void BitmapChangedFalse()
        {
            if (bitmap_changed)
                bitmap_changed = false;
        }

        public void BitmapChangedTrue()
        {
            bitmap_changed = true;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            bool flag = true;
            for (int i = 0; i < list_of_bools.Count(); i++)
            {
                if (list_of_bools[i])
                {
                    flag = false;
                    break;
                }
            }

            if (!flag)
            {

                for (int i = 0; i < list_of_bools.Count(); i++)
                {
                    if (list_of_bools[i])
                    {
                        if (MessageBox.Show("Сохранить изменения?", $"В файле {list_of_saved_names[i]} есть несохраненные изменения.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Form4 form = (Form4)list_of_painting_forms[i];
                            Bitmap saved_bitmap = form.ReturnBitmap();
                            for (int width = 1; width < saved_bitmap.Width; width++)
                                for (int height = 1; height < saved_bitmap.Height; height++)
                                    if (saved_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                                        saved_bitmap.SetPixel(width, height, System.Drawing.Color.Transparent);
                            if (list_of_saved_names[i] != "Несохраненный")
                            {
                                saved_bitmap.Save(list_of_saved_names[i]);
                                list_of_bools[i] = false;
                            }
                            else
                            {
                                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                                {
                                    if (saved_bitmap != null)
                                    {
                                        saved_bitmap.Save(saveFileDialog1.FileName);
                                        list_of_bools[i] = false;
                                    }
                                    SetFileName(saveFileDialog1.FileName);
                                }
                            }
                            BitmapChangedFalse();
                        }
                    }
                }
                Application.Exit();
            }
            else
                Application.Exit();
        }

        public void ChangeNameOfPaintForm(string new_name, int i)
        {
            list_of_saved_names[i] = new_name;
        }

        public void ChangeBool(bool b, int i)
        {
            list_of_bools[i] = b;
        }

        public void ChangeCurrent()
        {
            if (current_paint_form < 2)
                current_paint_form += 1;
        }

        public int ReturnCurrent()
        {
            return current_paint_form;
        }

        public bool ListOfPaintFormsIsEmpty()
        {
            if (list_of_painting_forms.Count() == 0)
                return true;
            else return false;
        }
        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public Color SetColor()
        {
            if (pipette_color)
                return PaintForm.Pipette(pipette_color);

            if (color == Color.White)
                return Color.White;
            if (color == Color.Red)
                return Color.Red;
            if (color == Color.Blue)
                return Color.Blue;
            if (color == Color.Green)
                return Color.Green;
            if (color == Color.Yellow)
                return Color.Yellow;
            else return color;
        }

        public void SetFileName(string filename)
        {
            saved_file_name = filename;
            var list_of_filenames = saved_file_name.Split('\\');
            var edited_file_name = list_of_filenames[list_of_filenames.Length - 1];
            if (current_paint_form == 0)
                guna2Button5.Text = edited_file_name;
            if (current_paint_form == 1)
                guna2Button6.Text = edited_file_name;
            if (current_paint_form == 2)
                guna2Button13.Text = edited_file_name;
        }

        public void SetNewFileName()
        {
            var count = list_of_painting_forms.Count();
            if (count == 1)
                guna2Button6.Text = "Несохраненный";
            if (current_paint_form == 2)
                guna2Button13.Text = "Несохраненный";
        }

        public string FileName(int i)
        {
            return list_of_saved_names[i];
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            pipette_color = false;
            color = Color.WhiteSmoke;
            width = trackBar1.Value;
            eraser_regime = false;
            guna2Button14.FillColor = Color.White;
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            pipette_color = false;
            color = Color.Black;
            width = trackBar1.Value;
            eraser_regime = false;
            guna2Button14.FillColor = Color.Black;
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            pipette_color = false;
            color = Color.Red;
            width = trackBar1.Value;
            eraser_regime = false;
            guna2Button14.FillColor = Color.Red;
        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            pipette_color = false;
            color = Color.Blue;
            width = trackBar1.Value;
            eraser_regime = false;
            guna2Button14.FillColor = Color.Blue;
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            pipette_color = false;
            color = Color.Green;
            width = trackBar1.Value;
            eraser_regime = false;
            guna2Button14.FillColor = Color.Green;
        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            pipette_color = false;
            color = Color.Yellow;
            width = trackBar1.Value;
            eraser_regime = false;
            guna2Button14.FillColor = Color.Yellow;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            width = trackBar1.Value;
        }

        public int SetWidth()
        {
            return width;
        }

        public int SetEraserWidth()
        {
            return width;
        }

        public bool SetCrop()
        {
            return crop_value;
        }

        public bool SetEllipse()
        {
            return ellipse_value;
        }

        public bool SetSquare()
        {
            return square_value;
        }

        public bool SetPipette()
        {
            return pipette_regime;
        }

        public bool SetCut()
        {
            return cut_regime;
        }

        public bool SetLine()
        {
            return line_regime;
        }

        public bool SetEraser()
        {
            return eraser_regime;
        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void SetChildFormName(string new_name)
        {
            childform_name = new_name;
        }
        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (!maximized)
            {
                guna2BorderlessForm1.BorderRadius = 0;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                if (childform_name == "paint")
                    PaintForm.PictureBoxMax();
                if (childform_name == "starting_window")
                    Starting_dialog.PictureBoxMax();
                if (childform_name == "saving_window")
                    Saving_window.PictureBoxMax();
                maximized = true;
                iconButton2.IconChar = FontAwesome.Sharp.IconChar.WindowRestore;
            }
            else
            {
                guna2BorderlessForm1.BorderRadius = 30;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Normal;
                if (childform_name == "paint")
                    PaintForm.PictureBoxMin();
                if (childform_name == "starting_window")
                    Starting_dialog.PictureBoxMin();
                if (childform_name == "saving_window")
                    Saving_window.PictureBoxMin();
                maximized = false;
                iconButton2.IconChar = FontAwesome.Sharp.IconChar.Square;
            }


        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            PaintForm.Eraser();
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            PaintForm.Forward();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            HideButtons();
            FileForm = new Form5(this, PaintForm, PaintForm.ReturnBitmap(), current_paint_form);
            OpenForm(FileForm);
            SetChildFormName("starting_window");
        }

        private void iconButton4_Click_1(object sender, EventArgs e)
        {
            if (crop_value == false)
                crop_value = true;
            else crop_value = false;
            ellipse_value = false;
            square_value = false;
            pipette_regime = false;
            cut_regime = false;
            line_regime = false;
            eraser_regime = false;
    }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            if (ellipse_value)
                ellipse_value = false;
            else ellipse_value = true;
            square_value = false;
            crop_value = false;
            pipette_regime = false;
            cut_regime = false;
            line_regime = false;
            eraser_regime = false;
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            if (square_value)
                square_value = false;
            else square_value = true;
            crop_value = false;
            ellipse_value = false;
            pipette_regime = false;
            cut_regime = false;
            line_regime = false;
            eraser_regime = false;
        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            if (pipette_regime)
                pipette_regime = false;
            else pipette_regime = true;
            crop_value = false;
            ellipse_value = false;
            square_value = false;
            pipette_color = true;
            cut_regime = false;
            line_regime = false;
            eraser_regime = false;
        }

        private void iconButton11_Click(object sender, EventArgs e)
        {
            if (cut_regime)
                cut_regime = false;
            else cut_regime = true;
            crop_value = false;
            ellipse_value = false;
            square_value = false;
            pipette_regime = false;
            line_regime = false;
            eraser_regime = false;
        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            if (line_regime)
                line_regime = false;
            else line_regime = true;
            crop_value = false;
            ellipse_value = false;
            square_value = false;
            pipette_regime = false;
            cut_regime = false;
            eraser_regime = false;
        }

        private void iconButton16_Click(object sender, EventArgs e)
        {
            if (eraser_regime)
                eraser_regime = false;
            else eraser_regime = true;
            crop_value = false;
            ellipse_value = false;
            square_value = false;
            pipette_regime = false;
            cut_regime = false;
            line_regime = false;
            color = Color.White;
        }

        private void iconButton12_Click(object sender, EventArgs e)
        {

        }

        private void iconButton12_Click_1(object sender, EventArgs e)
        {
            Bitmap saved_bitmap = PaintForm.ReturnBitmap();
            if (list_of_saved_names[current_paint_form] != "Несохраненный")
            {
                saved_bitmap.Save(saved_file_name);
                ChangeBool(false, current_paint_form);
            }
            else
            {
                HideButtons();
                FileForm = new Form5(this, PaintForm, PaintForm.ReturnBitmap(), current_paint_form);
                OpenForm(FileForm);
                SetChildFormName("starting_window");
            }

        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            PaintForm = (Form4)list_of_painting_forms[0];
            OpenForm(list_of_painting_forms[0]);
            guna2Button5.FillColor = SystemColors.Control;
            guna2Button6.FillColor = Color.Gray;
            guna2Button13.FillColor = Color.Gray;
            current_paint_form = 0;
        }

        private void guna2Button6_Click_1(object sender, EventArgs e)
        {
            PaintForm = (Form4)list_of_painting_forms[1];
            OpenForm(list_of_painting_forms[1]);
            guna2Button5.FillColor = Color.Gray;
            guna2Button6.FillColor = SystemColors.Control;
            guna2Button13.FillColor = Color.Gray;
            current_paint_form = 1;
        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            PaintForm = (Form4)list_of_painting_forms[2];
            OpenForm(list_of_painting_forms[2]);
            guna2Button5.FillColor = Color.Gray;
            guna2Button6.FillColor = Color.Gray;
            guna2Button13.FillColor = SystemColors.Control;
            current_paint_form = 2;
        }

        private void iconButton13_Click(object sender, EventArgs e)
        {
            if (list_of_painting_forms.Count == 3 || list_of_painting_forms.Count == 2)
            {
                if (list_of_bools[0])
                {
                        if (MessageBox.Show("Сохранить изменения?", $"В файле {list_of_saved_names[0]} есть несохраненные изменения.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Form4 form = (Form4)list_of_painting_forms[0];
                            Bitmap saved_bitmap = form.ReturnBitmap();
                            for (int width = 1; width < saved_bitmap.Width; width++)
                                for (int height = 1; height < saved_bitmap.Height; height++)
                                    if (saved_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                                        saved_bitmap.SetPixel(width, height, Color.Transparent);
                            if (list_of_saved_names[0] != "Несохраненный")
                                saved_bitmap.Save(list_of_saved_names[0]);
                            else
                            {
                                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                                {
                                    if (saved_bitmap != null)
                                        saved_bitmap.Save(saveFileDialog1.FileName);
                                    SetFileName(saveFileDialog1.FileName);
                                }
                            }
                        }
                }

                list_of_painting_forms.RemoveAt(0);
                list_of_bools.RemoveAt(0);
                list_of_saved_names.RemoveAt(0);
                if (current_paint_form == 0)
                    OpenForm(list_of_painting_forms[0]);
                current_paint_form -= 1;
                HideFileButtons();
                guna2Button5.Text = guna2Button6.Text;
                guna2Button6.Text = guna2Button13.Text;
                painting_forms_counter -= 1;
            }
            else if (list_of_painting_forms.Count == 1)
            {
                if (list_of_bools[0])
                {
                    if (MessageBox.Show("Сохранить изменения?", $"В файле {list_of_saved_names[0]} есть несохраненные изменения.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Form4 form = (Form4)list_of_painting_forms[0];
                        Bitmap saved_bitmap = form.ReturnBitmap();
                        for (int width = 1; width < saved_bitmap.Width; width++)
                            for (int height = 1; height < saved_bitmap.Height; height++)
                                if (saved_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                                    saved_bitmap.SetPixel(width, height, Color.Transparent);
                        if (list_of_saved_names[0] != "Несохраненный")
                            saved_bitmap.Save(list_of_saved_names[0]);
                        else
                        {
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                if (saved_bitmap != null)
                                    saved_bitmap.Save(saveFileDialog1.FileName);
                                SetFileName(saveFileDialog1.FileName);
                            }
                        }
                    }
                }
                Application.Exit();
            }
        }

        private void iconButton14_Click(object sender, EventArgs e)
        {
            if (list_of_painting_forms.Count == 3 || list_of_painting_forms.Count == 2)
            {
                if (list_of_bools[1])
                {
                    if (MessageBox.Show("Сохранить изменения?", $"В файле {list_of_saved_names[1]} есть несохраненные изменения.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Form4 form = (Form4)list_of_painting_forms[1];
                        Bitmap saved_bitmap = form.ReturnBitmap();
                        for (int width = 1; width < saved_bitmap.Width; width++)
                            for (int height = 1; height < saved_bitmap.Height; height++)
                                if (saved_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                                    saved_bitmap.SetPixel(width, height, Color.Transparent);
                        if (list_of_saved_names[1] != "Несохраненный")
                            saved_bitmap.Save(list_of_saved_names[1]);
                        else
                        {
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                if (saved_bitmap != null)
                                    saved_bitmap.Save(saveFileDialog1.FileName);
                                SetFileName(saveFileDialog1.FileName);
                            }
                        }
                    }
                }

                list_of_painting_forms.RemoveAt(1);
                list_of_bools.RemoveAt(1);
                list_of_saved_names.RemoveAt(1);
                if (current_paint_form == 1)
                    OpenForm(list_of_painting_forms[0]);
                current_paint_form -= 1;
                HideFileButtons();
                guna2Button6.Text = guna2Button13.Text;
                painting_forms_counter -= 1;
            }
            if (list_of_painting_forms.Count == 3)
            {
                if (list_of_bools[1])
                {
                    if (MessageBox.Show("Сохранить изменения?", $"В файле {list_of_saved_names[1]} есть несохраненные изменения.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Form4 form = (Form4)list_of_painting_forms[1];
                        Bitmap saved_bitmap = form.ReturnBitmap();
                        for (int width = 1; width < saved_bitmap.Width; width++)
                            for (int height = 1; height < saved_bitmap.Height; height++)
                                if (saved_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                                    saved_bitmap.SetPixel(width, height, Color.Transparent);
                        if (list_of_saved_names[1] != "Несохраненный")
                            saved_bitmap.Save(list_of_saved_names[1]);
                        else
                        {
                            saveFileDialog1.Filter = "PNG(*.PNG)|*.png";
                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                if (saved_bitmap != null)
                                    saved_bitmap.Save(saveFileDialog1.FileName);
                                SetFileName(saveFileDialog1.FileName);
                            }
                        }
                    }
                }

                list_of_painting_forms.RemoveAt(1);
                list_of_bools.RemoveAt(1);
                list_of_saved_names.RemoveAt(1);
                if (current_paint_form == 1)
                    OpenForm(list_of_painting_forms[1]);
                current_paint_form -= 1;
                HideFileButtons();
                guna2Button6.Text = guna2Button13.Text;
                painting_forms_counter -= 1;
            }
        }

        private void iconButton15_Click(object sender, EventArgs e)
        {
            if (list_of_bools[2])
            {
                if (MessageBox.Show("Сохранить изменения?", $"В файле {list_of_saved_names[2]} есть несохраненные изменения.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Form4 form = (Form4)list_of_painting_forms[2];
                    Bitmap saved_bitmap = form.ReturnBitmap();
                    for (int width = 1; width < saved_bitmap.Width; width++)
                        for (int height = 1; height < saved_bitmap.Height; height++)
                            if (saved_bitmap.GetPixel(width, height).ToArgb().Equals(SystemColors.Control.ToArgb()))
                                saved_bitmap.SetPixel(width, height, Color.Transparent);
                    if (list_of_saved_names[2] != "Несохраненный")
                        saved_bitmap.Save(list_of_saved_names[1]);
                    else
                    {
                        saveFileDialog1.Filter = "PNG(*.PNG)|*.png";
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            if (saved_bitmap != null)
                                saved_bitmap.Save(saveFileDialog1.FileName);
                            SetFileName(saveFileDialog1.FileName);
                        }
                    }
                }
                painting_forms_counter -= 1;
            }

            list_of_painting_forms.RemoveAt(2);
            list_of_bools.RemoveAt(2);
            list_of_saved_names.RemoveAt(2);
            OpenForm(list_of_painting_forms[1]);
            current_paint_form -= 1;
            HideFileButtons();
        }
        public int ReturnNumberOfPaintForms()
        {
            return list_of_painting_forms.Count();
        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            guna2Button14.FillColor = cd.Color;
            pipette_color = false;
            color = cd.Color;
        }


    }


}

