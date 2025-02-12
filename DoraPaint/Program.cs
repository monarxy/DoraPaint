using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;

namespace MyApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string UserId = Properties.Settings.Default.userid;
            if (UserId != "")
            {
                if (args != null && args.Length > 0)
                {
                    string filename = "";
                    if (args.Count() != 1)
                        for (int i = 0; i < args.Count(); i++)
                            filename += args[i];
                    else
                        filename = args[0];
                    if (File.Exists(filename))
                    {
                        Bitmap bmp = new Bitmap(Image.FromFile(filename));
                        ApplicationConfiguration.Initialize();
                        Application.Run(new Form2(UserId, bmp));
                    }
                }
                ApplicationConfiguration.Initialize();
                Application.Run(new Form2(UserId));
            }
            else
            {
                if (args != null && args.Length > 0)
                {
                    string filename = args[0];
                    if (File.Exists(filename))
                    {
                        Bitmap bmp = new Bitmap(Image.FromFile(filename));
                        ApplicationConfiguration.Initialize();
                        Application.Run(new Form1(new Form2(UserId, bmp), UserId));
                    }
                }
                else
                {
                    ApplicationConfiguration.Initialize();
                    Application.Run(new Form1(new Form2(UserId), UserId));
                }
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

        }
    }
}