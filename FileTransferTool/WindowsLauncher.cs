using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLibrary;

namespace FileTransferTool
{
    class WindowsLauncher
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WindowsUI ui = new WindowsUI();
            Core core = new Core(ui);

            // Run program.
            Application.Run(ui.MainWindow);
        }
    }
}
