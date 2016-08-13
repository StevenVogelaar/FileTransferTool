using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileTransferTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow();
            Manager manager = new Manager(mainWindow);

            while (true)
            {
                if (mainWindow != null)
                {
                    try
                    {
                        Application.Run(mainWindow);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("AKWTKAWTAWKTGKAWTG\n" + e.StackTrace);
                    }
                }
            }
        }
    }
}
