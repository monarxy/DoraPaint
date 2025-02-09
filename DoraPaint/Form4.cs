using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyApp
{
    public partial class Form4 : Form
    {
        private ArrayPoints arrayPoints = new ArrayPoints(2);
        private bool MouseDown = false;
        private Form2 ParentForm;
        private Bitmap bitmap = new Bitmap(10, 10);
        private Color color;
        private List<Point[]> array_of_arrays = new List<Point[]>();
        private List<Point> list = new List<Point>();
        private int back = 0;
        private Graphics graphics;
        private Graphics new_graphics;
        private int width = 0;
        private List<Point> list_of_two_points = new List<Point>(2) { new Point(0, 0), new Point(0, 0) };

        private LinkedList<List<Point>> main_node_list = new LinkedList<List<Point>>();
        private LinkedListNode<List<Point>> main_node = new LinkedListNode<List<Point>>([]); ////////////!!!!!!!!!!!!!
        private LinkedListNode<List<Point>> current = new LinkedListNode<List<Point>>([]);
        private LinkedList<Color> color_list = new LinkedList<Color>();
        private LinkedListNode<Color> color_list_node = new LinkedListNode<Color>(Color.Black);
        private LinkedListNode<Color> current_color = new LinkedListNode<Color>(Color.Black);

        private LinkedList<int> width_list = new LinkedList<int>();
        private LinkedListNode<int> current_width = new LinkedListNode<int>(0);

        private bool crop_value = false;
        private Pen crop_pen = new Pen(Color.Black);
        private int crpX;
        private int crpY;
        private int rectW;
        private int rectH;
        private Bitmap crpImg = new Bitmap(10, 10);
        private bool crop_for_printer = false;
        private bool picture_box_changed = false;
        private bool crop_global_flag = false;
        private bool start_flag = false;
        private bool end_flag = false;
        private int last_trackbar_value = 1;
        private int scroll_position_x = 0;
        private int scroll_position_y = 0;
        private bool drag_drop = false;
        private string[] filenames = Array.Empty<string>();

        private Bitmap sepia_bitmap = new Bitmap(10, 10);
        private Bitmap gray_bitmap = new Bitmap(10, 10);
        private Bitmap contrast_bitmap = new Bitmap(10, 10);

        private string bitmap_path = "";
        private string gray_path = "";
        private string contrast_path = "";

        private bool sepia = false;
        private bool gray = false;
        private bool contrast = false;

        private bool ellipse_regime = false;
        private List<Point> ellipse_list = new List<Point>();

        private bool square_regime = false;
        private List<Point> square_list = new List<Point>();

        private bool cut_regime = false;
        private List<Point> cut_list = new List<Point>();

        private bool line_regime = false;
        private List<Point> line_list = new List<Point>();

        private bool pipette_regime = false;
        private bool pipette_color = false;

        private bool eraser_regime = false;

        private int _bitmap_width;
        private int _bitmap_height;

        private int _current_paint_form;

        private Bitmap first_bitmap;
        private Bitmap first_sepia_bitmap;
        private Bitmap first_gray_bitmap;
        private Bitmap first_contrast_bitmap;
        private Bitmap drag_drop_load_bitmap;

        public Form4(Form2 ParentForm, int bitmap_width, int bitmap_height, int current_paint_form)
        {
            _bitmap_width = bitmap_width;
            _bitmap_height = bitmap_height;
            this.ParentForm = ParentForm;
            _current_paint_form = current_paint_form;
            InitializeComponent();
            SetSize();
        }

        public Form4(Form2 ParentForm, int current_paint_form, Bitmap bmp)
        {
            this.ParentForm = ParentForm;
            _current_paint_form = current_paint_form;
            first_bitmap = bmp;
            bitmap = new Bitmap(first_bitmap);
            graphics = Graphics.FromImage(bitmap);
            drag_drop = true;
            InitializeComponent();
            SetSize();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            if (!drag_drop)
            {
                bitmap = new Bitmap(_bitmap_width, _bitmap_height);
                for (int width = 1; width < _bitmap_width; width++)
                    for (int height = 1; height < _bitmap_height; height++)
                        bitmap.SetPixel(width, height, Color.White);
            }
            guna2PictureBox1.AllowDrop = true;
            if (drag_drop)
            {
                bitmap = new Bitmap(first_bitmap);
                graphics = Graphics.FromImage(bitmap);
            }


            guna2PictureBox1.Image = bitmap;
            guna2PictureBox1.Size = new Size(_bitmap_width, _bitmap_height);

            this.DoubleBuffered = true;
            trackBar1.Minimum = 1;
            trackBar1.Maximum = 4;
            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 1;

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
        }
        private void guna2PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ParentForm.ChangeBool(true, ParentForm.ReturnCurrent());
            if (picture_box_changed == false)
            {
                crop_value = ParentForm.SetCrop();
                ellipse_regime = ParentForm.SetEllipse();
                square_regime = ParentForm.SetSquare();
                pipette_regime = ParentForm.SetPipette();
                cut_regime = ParentForm.SetCut();
                line_regime = ParentForm.SetLine();
                eraser_regime = ParentForm.SetEraser();
                if (!crop_value && !ellipse_regime && !square_regime && !line_regime && !pipette_regime && !cut_regime && !line_regime)
                {
                    MouseDown = true;
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                }
                if (pipette_regime)
                    color = bitmap.GetPixel(e.X, e.Y);
                else
                {
                    base.OnMouseDown(e);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                        Cursor = Cursors.Cross;
                        crop_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        crpX = e.X;
                        crpY = e.Y;

                    }
                }
            }
        }

        private void guna2PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            guna2PictureBox1_MouseUp(sender, e, main_node);
        }

        private void guna2PictureBox1_MouseEnter(object sender, EventArgs e)
        {
            base.OnMouseEnter(e);
            Cursor = Cursors.Cross;
        }

        private void guna2PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > 0 && e.Y > 0)
                label1.Text = $"{e.X}; {e.Y}";
            if (picture_box_changed == false && trackBar1.Value == 1)
            {
                if (!crop_value && !ellipse_regime && !square_regime && !pipette_regime && !cut_regime && !line_regime)
                {
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                    if (trackBar1.Value == 1)
                    {
                        graphics = Graphics.FromImage(bitmap);
                        if (sepia)
                            graphics = Graphics.FromImage(sepia_bitmap);
                        if (gray)
                            graphics = Graphics.FromImage(gray_bitmap);
                        if (contrast)
                            graphics = Graphics.FromImage(contrast_bitmap);
                        color = ParentForm.SetColor();
                        if (!eraser_regime)
                            width = ParentForm.SetWidth();
                        else
                        {
                            width = ParentForm.SetEraserWidth();
                        }
                        var pen = new Pen(color);
                        var control_pen = new Pen(SystemColors.Control);
                        pen.Width = width;
                        pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        if (!MouseDown)
                            return;
                        arrayPoints.SetPoint(e.X, e.Y);


                        if (arrayPoints.GetCountPoints() >= 2)
                        {
                            Point[] array_points = arrayPoints.GetPoints();
                            graphics.DrawLines(pen, array_points);

                            foreach (var a in array_points)
                            {
                                list.Add(a);
                            }
                            if (sepia)
                                guna2PictureBox1.Image = sepia_bitmap;
                            if (gray)
                                guna2PictureBox1.Image = gray_bitmap;
                            if (contrast)
                                guna2PictureBox1.Image = contrast_bitmap;
                            if (!sepia && !gray && !contrast) guna2PictureBox1.Image = bitmap;
                            arrayPoints.SetPoint(e.X, e.Y);

                        }
                    }
                }

                if (crop_value)
                {
                    ellipse_regime = false;
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                    base.OnMouseMove(e);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {

                        rectW = e.X - crpX;
                        rectH = e.Y - crpY;
                        if (rectW >= 0 && rectH >= 0)
                        {
                            guna2PictureBox1.Refresh();
                            guna2Panel2.AutoScroll = false;
                            Graphics crop_graphics = guna2PictureBox1.CreateGraphics();
                            crop_graphics.DrawRectangle(crop_pen, crpX, crpY, rectW, rectH);
                            crop_graphics.Dispose();
                        }
                    }
                }

                if (ellipse_regime)
                {
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                    base.OnMouseMove(e);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        color = ParentForm.SetColor();
                        width = ParentForm.SetWidth();
                        rectW = e.X - crpX;
                        rectH = e.Y - crpY;
                        if (rectW >= 0 && rectH >= 0)
                        {
                            guna2PictureBox1.Refresh();
                            guna2Panel2.AutoScroll = false;
                            Graphics shapes_graphics = guna2PictureBox1.CreateGraphics();
                            if (ellipse_regime)
                                shapes_graphics.DrawEllipse(crop_pen, crpX, crpY, rectW, rectH);
                            shapes_graphics.Dispose();

                        }
                    }
                }

                if (square_regime)
                {
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                    base.OnMouseMove(e);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        color = ParentForm.SetColor();
                        width = ParentForm.SetWidth();
                        rectW = e.X - crpX;
                        rectH = e.Y - crpY;
                        if (rectW >= 0 && rectH >= 0)
                        {
                            guna2PictureBox1.Refresh();
                            guna2Panel2.AutoScroll = false;
                            Graphics square_graphics = guna2PictureBox1.CreateGraphics();
                            if (square_regime)
                                square_graphics.DrawRectangle(crop_pen, crpX, crpY, rectW, rectH);
                            square_graphics.Dispose();

                        }
                    }
                }
                if (cut_regime)
                {
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                    base.OnMouseMove(e);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        rectW = e.X - crpX;
                        rectH = e.Y - crpY;
                        if (rectW >= 0 && rectH >= 0)
                        {
                            guna2PictureBox1.Refresh();
                            guna2Panel2.AutoScroll = false;
                            Graphics cut_graphics = guna2PictureBox1.CreateGraphics();
                            if (cut_regime)
                                cut_graphics.DrawRectangle(crop_pen, crpX, crpY, rectW, rectH);
                            cut_graphics.Dispose();

                        }
                    }
                }
                if (line_regime)
                {
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                    base.OnMouseMove(e);
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        rectW = e.X - crpX;
                        rectH = e.Y - crpY;
                        if (rectW >= 0 && rectH >= 0)
                        {
                            guna2PictureBox1.Refresh();
                            guna2Panel2.AutoScroll = false;
                            Graphics line_graphics = guna2PictureBox1.CreateGraphics();
                            if (line_regime)
                                line_graphics.DrawLine(crop_pen, crpX, crpY, crpX + rectW, crpY + rectH);
                            line_graphics.Dispose();

                        }
                    }
                }
            }
        }

        private void guna2PictureBox1_MouseUp(object sender, MouseEventArgs e, LinkedListNode<List<Point>> main_node)
        {
            start_flag = false;
            MouseDown = false;
            if (!crop_value && !ellipse_regime && !square_regime && !line_regime && !pipette_regime && !cut_regime && !line_regime && trackBar1.Value == 1)
            {
                guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);

                arrayPoints.Reset();
                main_node.Value = list;

                for (int i = 0; i < Math.Abs(back); i++)
                {
                    main_node_list.RemoveLast();
                    color_list.RemoveLast();
                    width_list.RemoveLast();
                }

                if (main_node_list.Count == 0)
                {
                    main_node_list.AddFirst(list);
                    color_list.AddFirst(color);
                    width_list.AddFirst(width);
                }


                else
                {
                    main_node_list.AddLast(list);
                    color_list.AddLast(color);
                    width_list.AddLast(width);
                }

                array_of_arrays.Add(list.ToArray());
                list = new List<Point>();
                back = 0;
                crop_global_flag = false;
            }
            if (crop_value && trackBar1.Value == 1)
            {
                guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                last_trackbar_value = trackBar1.Value;
                guna2Panel2.AutoScroll = true;
                Cursor = Cursors.Default;
                for (int i = 0; i < Math.Abs(back); i++)
                {
                    main_node_list.RemoveLast();
                    color_list.RemoveLast();
                    width_list.RemoveLast();
                }
                if (rectH > 0 && rectW > 0)
                {
                    crpImg = new Bitmap(rectW, rectH);
                    if (!sepia && !gray && !contrast)
                    {
                        Bitmap bm = ZoomPicture(bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        for (int i = 0; i < rectW; i++)
                            for (int y = 0; y < rectH; y++)
                            {
                                if (trackBar1.Value == 1)
                                {
                                    Color pixel_color = bitmap.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                                else
                                {
                                    Color pixel_color = bm.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                            }
                    }
                    if (sepia)
                    {
                        Bitmap bm = ZoomPicture(sepia_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        for (int i = 0; i < rectW; i++)
                            for (int y = 0; y < rectH; y++)
                            {
                                if (trackBar1.Value == 1)
                                {
                                    Color pixel_color = sepia_bitmap.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                                else
                                {
                                    Color pixel_color = bm.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                            }
                    }
                    if (gray)
                    {
                        Bitmap bm = ZoomPicture(gray_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        for (int i = 0; i < rectW; i++)
                            for (int y = 0; y < rectH; y++)
                            {
                                if (trackBar1.Value == 1)
                                {
                                    Color pixel_color = gray_bitmap.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                                else
                                {
                                    Color pixel_color = bm.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                            }
                    }
                    if (contrast)
                    {
                        Bitmap bm = ZoomPicture(contrast_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        for (int i = 0; i < rectW; i++)
                            for (int y = 0; y < rectH; y++)
                            {
                                if (trackBar1.Value == 1)
                                {
                                    Color pixel_color = contrast_bitmap.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                                else
                                {
                                    Color pixel_color = bm.GetPixel(crpX + i, crpY + y);
                                    crpImg.SetPixel(i, y, pixel_color);
                                }
                            }
                    }

                    guna2PictureBox1.Image = (Image)crpImg;
                    picture_box_changed = true;
                    crop_global_flag = true;
                    trackBar1.Value = 1;
                    guna2PictureBox1.Enabled = false;
                    back = 0;
                    crop_for_printer = true;
                }
            }
            if (ellipse_regime && trackBar1.Value == 1)
            {
                if (sepia)
                    graphics = Graphics.FromImage(sepia_bitmap);
                if (gray)
                    graphics = Graphics.FromImage(gray_bitmap);
                if (contrast)
                    graphics = Graphics.FromImage(contrast_bitmap);

                guna2Panel2.AutoScroll = true;
                var pen = new Pen(color);
                pen.Width = width;
                EllipseUpLeft();
                EllipseUpRight();
                EllipseDownRight();
                EllipseDownLeft();
                EllipseUpLeft();

                graphics.DrawLines(pen, ellipse_list.ToArray());

                for (int i = 0; i < Math.Abs(back); i++)
                {
                    main_node_list.RemoveLast();
                    color_list.RemoveLast();
                    width_list.RemoveLast();
                }

                if (main_node_list.Count == 0)
                {
                    main_node_list.AddFirst(ellipse_list);
                    color_list.AddFirst(color);
                    width_list.AddFirst(width);
                }

                else
                {
                    main_node_list.AddLast(ellipse_list);
                    color_list.AddLast(color);
                    width_list.AddLast(width);
                }

                ellipse_list = new List<Point>();
                back = 0;

                if (sepia)
                    guna2PictureBox1.Image = sepia_bitmap;
                if (gray)
                    guna2PictureBox1.Image = gray_bitmap;
                if (contrast)
                    guna2PictureBox1.Image = contrast_bitmap;
                if (!sepia && !gray && !contrast) guna2PictureBox1.Image = bitmap;
            }
            if (square_regime && trackBar1.Value == 1)
            {
                if (sepia)
                    graphics = Graphics.FromImage(sepia_bitmap);
                if (gray)
                    graphics = Graphics.FromImage(gray_bitmap);
                if (contrast)
                    graphics = Graphics.FromImage(contrast_bitmap);

                guna2Panel2.AutoScroll = true;
                var pen = new Pen(color);
                pen.Width = width;
                DrawUpSquare();
                graphics.DrawLines(pen, square_list.ToArray());

                for (int i = 0; i < Math.Abs(back); i++)
                {
                    main_node_list.RemoveLast();
                    color_list.RemoveLast();
                    width_list.RemoveLast();
                }

                if (main_node_list.Count == 0)
                {
                    main_node_list.AddFirst(square_list);
                    color_list.AddFirst(color);
                    width_list.AddFirst(width);
                }


                else
                {
                    main_node_list.AddLast(square_list);
                    color_list.AddLast(color);
                    width_list.AddLast(width);
                }

                if (sepia)
                    guna2PictureBox1.Image = sepia_bitmap;
                if (gray)
                    guna2PictureBox1.Image = gray_bitmap;
                if (contrast)
                    guna2PictureBox1.Image = contrast_bitmap;
                if (!sepia && !gray && !contrast) guna2PictureBox1.Image = bitmap;

                square_list = new List<Point>();
                back = 0;
            }
            if (cut_regime && trackBar1.Value == 1)
            {
                if (sepia)
                    graphics = Graphics.FromImage(sepia_bitmap);
                if (gray)
                    graphics = Graphics.FromImage(gray_bitmap);
                if (contrast)
                    graphics = Graphics.FromImage(contrast_bitmap);

                guna2Panel2.AutoScroll = true;
                Color cut_color = SystemColors.Control;
                var pen = new Pen(cut_color);
                pen.Width = width;
                Cut();
                graphics.DrawLines(pen, cut_list.ToArray());

                for (int i = 0; i < Math.Abs(back); i++)
                {
                    main_node_list.RemoveLast();
                    color_list.RemoveLast();
                    width_list.RemoveLast();
                }

                if (main_node_list.Count == 0)
                {
                    main_node_list.AddFirst(cut_list);
                    color_list.AddFirst(SystemColors.Control);
                    width_list.AddFirst(width);
                }
                else
                {
                    main_node_list.AddLast(cut_list);
                    color_list.AddLast(SystemColors.Control);
                    width_list.AddLast(width);
                }

                if (sepia)
                    guna2PictureBox1.Image = sepia_bitmap;
                if (gray)
                    guna2PictureBox1.Image = gray_bitmap;
                if (contrast)
                    guna2PictureBox1.Image = contrast_bitmap;
                if (!sepia && !gray && !contrast) guna2PictureBox1.Image = bitmap;

                cut_list = new List<Point>();
                back = 0;
            }
            if (line_regime && trackBar1.Value == 1)
            {
                if (sepia)
                    graphics = Graphics.FromImage(sepia_bitmap);
                if (gray)
                    graphics = Graphics.FromImage(gray_bitmap);
                if (contrast)
                    graphics = Graphics.FromImage(contrast_bitmap);

                guna2Panel2.AutoScroll = true;
                var pen = new Pen(color);
                pen.Width = width;
                Line();
                graphics.DrawLines(pen, line_list.ToArray());

                for (int i = 0; i < Math.Abs(back); i++)
                {
                    main_node_list.RemoveLast();
                    color_list.RemoveLast();
                    width_list.RemoveLast();
                }

                if (main_node_list.Count == 0)
                {
                    main_node_list.AddFirst(line_list);
                    color_list.AddFirst(color);
                    width_list.AddFirst(width);
                }


                else
                {
                    main_node_list.AddLast(line_list);
                    color_list.AddLast(color);
                    width_list.AddLast(width);
                }

                if (sepia)
                    guna2PictureBox1.Image = sepia_bitmap;
                if (gray)
                    guna2PictureBox1.Image = gray_bitmap;
                if (contrast)
                    guna2PictureBox1.Image = contrast_bitmap;
                if (!sepia && !gray && !contrast) guna2PictureBox1.Image = bitmap;

                line_list = new List<Point>();
                back = 0;
            }
        }

        void EllipseUpLeft()
        {
            double a = 0.5 * rectW;
            double b = 0.5 * rectH;
            int number = (int)Math.Round(a, MidpointRounding.AwayFromZero);
            int b_round = (int)Math.Round(b, MidpointRounding.AwayFromZero);
            number *= -1;
            for (int x = number; x < 0; x++)
            {
                int y = (int)Math.Round(Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2)));
                if (y >= 0)
                    ellipse_list.Add(new Point(x + crpX - number, b_round - y + crpY));
            }
        }

        void EllipseDownLeft()
        {
            double a = 0.5 * rectW;
            double b = 0.5 * rectH;
            int number = (int)Math.Round(a, MidpointRounding.AwayFromZero);
            int b_round = (int)Math.Round(b, MidpointRounding.AwayFromZero);
            for (int x = 0; x < number; x++)
            {
                int y = (int)Math.Round(Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2)));
                if (y >= 0)
                    ellipse_list.Add(new Point(-1 * x + crpX + number, b_round + y + crpY));
            }

        }

        void EllipseUpRight()
        {
            double a = 0.5 * rectW;
            double b = 0.5 * rectH;
            int number = (int)Math.Round(a, MidpointRounding.AwayFromZero);
            int b_round = (int)Math.Round(b, MidpointRounding.AwayFromZero); ;
            for (int x = 0; x < number; x++)
            {
                int y = (int)Math.Round(Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2)));
                if (y >= 0)
                    ellipse_list.Add(new Point(x + crpX + number, b_round - y + crpY));
            }
        }

        void EllipseDownRight()
        {
            double a = 0.5 * rectW;
            double b = 0.5 * rectH;
            int number = (int)Math.Round(a, MidpointRounding.AwayFromZero);
            int b_round = (int)Math.Round(b, MidpointRounding.AwayFromZero);
            number *= -1;
            for (int x = number; x < 0; x++)
            {
                int y = (int)Math.Round(Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2)));
                if (y >= 0)
                    ellipse_list.Add(new Point(Math.Abs(x) + crpX - number, b_round + y + crpY));
            }
        }

        void DrawUpSquare()
        {

            for (int x = crpX; x < crpX + rectW; x++)
            {
                square_list.Add(new Point(x, crpY));
            }
            for (int y = crpY; y < crpY + rectH; y++)
            {
                square_list.Add(new Point(crpX + rectW, y));
            }
            for (int x = crpX + rectW; x > crpX; x--)
            {
                square_list.Add(new Point(x, crpY + rectH));
            }
            for (int y = crpY + rectH; y > crpY; y--)
            {
                square_list.Add(new Point(crpX, y));
            }
        }

        void Cut()
        {
            for (int x = crpX; x < crpX + rectW; x++)
                for (int y = crpY; y < crpY + rectH; y++)
                {
                    cut_list.Add(new Point(x, y));
                }
        }

        void Line()
        {
            line_list.Add(new Point(crpX, crpY));
            line_list.Add(new Point(crpX + rectW, crpY + rectH));
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Cursor = Cursors.Default;
        }



        private void guna2PictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void guna2PictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (!drag_drop)
            {
                ParentForm.ChangeBool(true, ParentForm.ReturnCurrent());
                var data = e.Data.GetData(DataFormats.FileDrop);
                if (data != null)
                {
                    filenames = data as string[];
                    if (filenames.Length == 1)
                    {
                        scroll_position_x = 0;
                        scroll_position_y = 0;
                        guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                        trackBar1.Value = 1;
                        bitmap = new Bitmap(Image.FromFile(filenames[0]));
                        first_bitmap = new Bitmap(Image.FromFile(filenames[0]));
                        graphics = Graphics.FromImage(bitmap);
                        guna2PictureBox1.Image = bitmap;
                        main_node_list.Clear();
                        color_list.Clear();
                        width_list.Clear();
                        back = 0;
                        drag_drop = true;
                    }
                    else MessageBox.Show("Перетащить можно только один файл.");
                }
            }
        }

        public void Eraser()
        {
            ParentForm.ChangeBool(true, ParentForm.ReturnCurrent());
            guna2PictureBox1.Enabled = true;
            if (start_flag != true)
            {
                end_flag = false;
                if (picture_box_changed == false)
                {
                    graphics.Clear(guna2PictureBox1.BackColor);

                    if (drag_drop)
                    {
                        if (!sepia && !gray && !contrast)
                        {
                            bitmap = new Bitmap(first_bitmap);
                            graphics = Graphics.FromImage(bitmap);
                        }
                        if (sepia)
                        {
                            sepia_bitmap = new Bitmap(first_sepia_bitmap);
                            graphics = Graphics.FromImage(sepia_bitmap);
                        }
                        if (gray)
                        {
                            gray_bitmap = new Bitmap(first_gray_bitmap);
                            graphics = Graphics.FromImage(gray_bitmap);
                        }
                        if (contrast)
                        {
                            contrast_bitmap = new Bitmap(first_contrast_bitmap);
                            graphics = Graphics.FromImage(contrast_bitmap);
                        }
                    }
                    back -= 1;


                    var list_length = main_node_list.Count;
                    current = main_node_list.First;
                    current_color = color_list.First;
                    current_width = width_list.First;
                    for (var i = 0; i < list_length + back; i++)
                    {
                        var pen = new Pen(current_color.Value);
                        pen.Width = current_width.Value;
                        graphics.DrawLines(pen, current.Value.ToArray());
                        current = current.Next;
                        current_color = current_color.Next;
                        current_width = current_width.Next;
                    }

                    if (!sepia && !gray && !contrast)
                        guna2PictureBox1.Image = ZoomPicture(bitmap, new Size(trackBar1.Value, trackBar1.Value));
                    if (sepia)
                        guna2PictureBox1.Image = ZoomPicture(sepia_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                    if (gray)
                        guna2PictureBox1.Image = ZoomPicture(gray_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                    if (contrast)
                        guna2PictureBox1.Image = ZoomPicture(contrast_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                    if (current == main_node_list.First)
                        start_flag = true;

                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                }
                else
                {
                    guna2PictureBox1.Image = null;
                    guna2PictureBox1.Width = bitmap.Width;
                    guna2PictureBox1.Height = bitmap.Height;
                    if (!sepia && !gray && !contrast)
                        guna2PictureBox1.Image = ZoomPicture(bitmap, new Size(last_trackbar_value, last_trackbar_value));
                    if (sepia)
                        guna2PictureBox1.Image = ZoomPicture(sepia_bitmap, new Size(last_trackbar_value, last_trackbar_value));
                    if (gray)
                        guna2PictureBox1.Image = ZoomPicture(gray_bitmap, new Size(last_trackbar_value, last_trackbar_value));
                    if (contrast)
                        guna2PictureBox1.Image = ZoomPicture(contrast_bitmap, new Size(last_trackbar_value, last_trackbar_value));

                    crop_value = ParentForm.SetCrop();
                    crop_for_printer = false;

                    picture_box_changed = false;
                    trackBar1.Value = last_trackbar_value;
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                }
            }
        }

        public void Forward()
        {
            ParentForm.ChangeBool(true, ParentForm.ReturnCurrent());
            if (end_flag != true)
            {
                start_flag = false;

                graphics.Clear(guna2PictureBox1.BackColor);

                if (drag_drop)
                {
                    if (!sepia && !gray && !contrast)
                    {
                        bitmap = new Bitmap(first_bitmap);
                        graphics = Graphics.FromImage(bitmap);
                    }
                    if (sepia)
                    {
                        sepia_bitmap = new Bitmap(first_sepia_bitmap);
                        graphics = Graphics.FromImage(sepia_bitmap);
                    }
                    if (gray)
                    {
                        gray_bitmap = new Bitmap(first_gray_bitmap);
                        graphics = Graphics.FromImage(gray_bitmap);
                    }
                    if (contrast)
                    {
                        contrast_bitmap = new Bitmap(first_contrast_bitmap);
                        graphics = Graphics.FromImage(contrast_bitmap);
                    }
                }
                back += 1;


                current = main_node_list.First;
                current_color = color_list.First;
                current_width = width_list.First;
                var list_length = main_node_list.Count;
                for (var i = 0; i < list_length + back; i++)
                {
                    if (crop_global_flag == true && back == 1)
                    {
                        guna2PictureBox1.Image = (Image)crpImg;
                        guna2PictureBox1.Enabled = false;
                        crop_for_printer = true;
                    }
                    else
                    if (back < 1)
                    {
                        var pen = new Pen(current_color.Value);
                        pen.Width = current_width.Value;
                        graphics.DrawLines(pen, current.Value.ToArray());
                        current = current.Next;
                        current_color = current_color.Next;
                        current_width = current_width.Next;
                        guna2PictureBox1.Image = null;
                        if (!sepia && !gray && !contrast)
                            guna2PictureBox1.Image = ZoomPicture(bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        if (sepia)
                            guna2PictureBox1.Image = ZoomPicture(sepia_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        if (gray)
                            guna2PictureBox1.Image = ZoomPicture(gray_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                        if (contrast)
                            guna2PictureBox1.Image = ZoomPicture(contrast_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                    }
                    if (back == 1)
                        end_flag = true;
                    guna2Panel2.AutoScrollPosition = new Point(scroll_position_x, scroll_position_y);
                }
            }

        }

        public Color Pipette(bool pipette)
        {
            return color;
        }

        private void Sepia()
        {
            sepia_bitmap = new Bitmap(first_bitmap);
            int x, y, red, green, blue, sepiaRed, sepiaGreen, sepiaBlue;
            for (var i = 0; i < bitmap.Width; i++)
                for (var j = 0; j < bitmap.Height; j++)
                {
                    Color clr = sepia_bitmap.GetPixel(i, j);
                    red = clr.R;
                    green = clr.G;
                    blue = clr.B;

                    sepiaRed = Convert.ToInt32(0.393 * red + 0.769 * green + 0.189 * blue);
                    sepiaGreen = Convert.ToInt32(0.349 * red + 0.686 * green + 0.168 * blue);
                    sepiaBlue = Convert.ToInt32(0.272 * red + 0.534 * green + 0.131 * blue);

                    if (sepiaRed > 255)
                        sepiaRed = 255;
                    if (sepiaGreen > 255)
                        sepiaGreen = 255;
                    if (sepiaBlue > 255)
                        sepiaBlue = 255;

                    clr = Color.FromArgb(clr.A, sepiaRed, sepiaGreen, sepiaBlue);
                    sepia_bitmap.SetPixel(i, j, clr);

                }
            first_sepia_bitmap = new Bitmap(sepia_bitmap);
        }

        private void Gray()
        {
            int red, blue, green;
            (red, blue, green) = (0, 0, 0);
            gray_bitmap = new Bitmap(first_bitmap);
            Color clr = new Color();

            for (int x = 3; x < gray_bitmap.Width - 3; x++)
                for (int y = 3; y < gray_bitmap.Height - 3; y++)
                {
                    clr = gray_bitmap.GetPixel(x, y);
                    (red, green, blue) = (clr.R, clr.G, clr.B);
                    int avg = (red + green + blue) / 3;
                    gray_bitmap.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                }
            first_gray_bitmap = new Bitmap(gray_bitmap);
        }

        private void Contrast()
        {
            int red, green, blue;
            (red, blue, green) = (0, 0, 0);
            contrast_bitmap = new Bitmap(first_bitmap);
            for (int i = 0; i < contrast_bitmap.Width; i++)
                for (int j = 0; j < contrast_bitmap.Height; j++)
                {
                    Color clr = contrast_bitmap.GetPixel(i, j);

                    double c = ((100.0 + 200 - 100) / 100.0) * ((100.0 + 200 - 100) / 100.0);
                    double t = ((((clr.R / 255.0) - 0.5) * c) + 0.5) * 255.0;
                    red = (int)t;
                    t = ((((clr.G / 255.0) - 0.5) * c) + 0.5) * 255.0;
                    green = (int)t;
                    t = ((((clr.B / 255.0) - 0.5) * c) + 0.5) * 255.0;
                    blue = (int)t;

                    if (red < 0) { red = 0; }
                    if (red > 255) { red = 255; }
                    if (green < 0) { green = 0; }
                    if (green > 255) { green = 255; }
                    if (blue < 0) { blue = 0; }
                    if (blue > 255) { blue = 255; }

                    contrast_bitmap.SetPixel(i, j, Color.FromArgb(clr.A, red, green, blue));
                }
            first_contrast_bitmap = new Bitmap(contrast_bitmap);
        }

        public Bitmap ReturnBitmap()
        {
            if (gray && !crop_for_printer)
                return gray_bitmap;
            if (sepia && !crop_for_printer)
                return sepia_bitmap;
            if (contrast && !crop_for_printer)
                return contrast_bitmap;
            if (crop_for_printer && gray || crop_for_printer && sepia || crop_for_printer && contrast || crop_for_printer)
                return crpImg;
            return bitmap;
        }

        private void SetSize()
        {
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            bitmap = new Bitmap(rectangle.Width, rectangle.Height);
            graphics = Graphics.FromImage(bitmap);
        }

        public void PictureBoxMax()
        {
            guna2Panel2.Size = new Size(2000, 1200);
            guna2PictureBox1.Size = new Size(2000, 1200);
        }

        public void PictureBoxMin()
        {
            guna2Panel2.Size = new Size(1252, 693);

        }

        Bitmap ZoomPicture(Image img, Size size)
        {
            Bitmap bm = new Bitmap(img, Convert.ToInt32(img.Width * size.Width), Convert.ToInt32(img.Height * size.Height));
            Graphics gpu = Graphics.FromImage(bm);
            gpu.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            return bm;
        }

        private void MouseEvents()
        {
            guna2PictureBox1.MouseDown += new MouseEventHandler(guna2PictureBox1_MouseDown);
            guna2PictureBox1.MouseMove += new MouseEventHandler(guna2PictureBox1_MouseMove);
            guna2PictureBox1.MouseEnter += new EventHandler(guna2PictureBox1_MouseEnter);
            Controls.Add(guna2PictureBox1);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (picture_box_changed == false)
            {
                if (!sepia && !gray && !contrast)
                {
                    guna2PictureBox1.Image = null;
                    guna2PictureBox1.Image = ZoomPicture(bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
                if (sepia)
                {
                    guna2PictureBox1.Image = null;
                    guna2PictureBox1.Image = ZoomPicture(sepia_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
                if (gray)
                {
                    guna2PictureBox1.Image = null;
                    guna2PictureBox1.Image = ZoomPicture(gray_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
                if (contrast)
                {
                    guna2PictureBox1.Image = null;
                    guna2PictureBox1.Image = ZoomPicture(contrast_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
            }
            else
            {
                guna2PictureBox1.Image = null;
                guna2PictureBox1.Image = ZoomPicture(crpImg, new Size(trackBar1.Value, trackBar1.Value));
            }

        }
        private void guna2Panel2_Scroll(object sender, EventArgs e)
        {
            scroll_position_x = Math.Abs(guna2Panel2.AutoScrollPosition.X);
            scroll_position_y = Math.Abs(guna2Panel2.AutoScrollPosition.Y);
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            if (drag_drop)
            {
                crop_global_flag = false;
                if (!picture_box_changed)
                {
                    sepia = true;
                    gray = false;
                    contrast = false;
                    if (bitmap_path == "")
                        Sepia();
                    guna2PictureBox1.Image = sepia_bitmap;
                    if (main_node_list.Count != 0)
                    {
                        graphics = Graphics.FromImage(sepia_bitmap);
                        var list_length = main_node_list.Count;
                        current = main_node_list.First;
                        current_color = color_list.First;
                        current_width = width_list.First;
                        for (var i = 0; i < list_length + back; i++)
                        {
                            var pen = new Pen(current_color.Value);
                            pen.Width = current_width.Value;
                            graphics.DrawLines(pen, current.Value.ToArray());
                            current = current.Next;
                            current_color = current_color.Next;
                            current_width = current_width.Next;
                        }
                    }
                    guna2PictureBox1.Image = ZoomPicture(sepia_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
            }
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            if (drag_drop)
            {
                crop_global_flag = false;
                if (!picture_box_changed)
                {
                    sepia = false;
                    contrast = false;
                    gray = true;

                    if (gray_path == "")
                        Gray();
                    guna2PictureBox1.Image = gray_bitmap;
                    if (main_node_list.Count != 0)
                    {
                        graphics = Graphics.FromImage(gray_bitmap);
                        var list_length = main_node_list.Count;
                        current = main_node_list.First;
                        current_color = color_list.First;
                        current_width = width_list.First;
                        for (var i = 0; i < list_length + back; i++)
                        {
                            var pen = new Pen(current_color.Value);
                            pen.Width = current_width.Value;
                            graphics.DrawLines(pen, current.Value.ToArray());
                            current = current.Next;
                            current_color = current_color.Next;
                            current_width = current_width.Next;
                        }
                    }
                    guna2PictureBox1.Image = ZoomPicture(gray_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
            }
        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {
            if (drag_drop)
            {
                crop_global_flag = false;
                if (!picture_box_changed)
                {
                    contrast = true;
                    gray = false;
                    sepia = false;

                    if (contrast_path == "")
                        Contrast();
                    guna2PictureBox1.Image = contrast_bitmap;
                    if (main_node_list.Count != 0)
                    {
                        graphics = Graphics.FromImage(contrast_bitmap);
                        var list_length = main_node_list.Count;
                        current = main_node_list.First;
                        current_color = color_list.First;
                        current_width = width_list.First;
                        for (var i = 0; i < list_length + back; i++)
                        {
                            var pen = new Pen(current_color.Value);
                            pen.Width = current_width.Value;
                            graphics.DrawLines(pen, current.Value.ToArray());
                            current = current.Next;
                            current_color = current_color.Next;
                            current_width = current_width.Next;
                        }
                    }
                    guna2PictureBox1.Image = ZoomPicture(contrast_bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
            }
        }

        private void guna2PictureBox7_Click(object sender, EventArgs e)
        {
            if (drag_drop)
            {

                crop_global_flag = false;
                if (!picture_box_changed)
                {
                    crop_global_flag = false;
                    sepia = false;
                    gray = false;
                    contrast = false;
                    bitmap = new Bitmap(first_bitmap);
                    graphics = Graphics.FromImage(bitmap);
                    var list_length = main_node_list.Count;
                    current = main_node_list.First;
                    current_color = color_list.First;
                    current_width = width_list.First;
                    for (var i = 0; i < list_length + back; i++)
                    {
                        var pen = new Pen(current_color.Value);
                        pen.Width = current_width.Value;
                        graphics.DrawLines(pen, current.Value.ToArray());
                        current = current.Next;
                        current_color = current_color.Next;
                        current_width = current_width.Next;
                    }
                    guna2PictureBox1.Image = ZoomPicture(bitmap, new Size(trackBar1.Value, trackBar1.Value));
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pipette_regime = true;

        }

    }
}
