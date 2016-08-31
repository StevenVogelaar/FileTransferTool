using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLibrary;

namespace FileTransferTool
{
    public partial class DownloadProgressWindow : Form
    {


        public DownloadProgressWindow()
        {
            InitializeComponent();
            DownloadList.Rows.Add(new object[] { "A file here", "A location here" });
        }


        private void downloadStarted(object sender, FTUI.DownloadStartedEventArgs e)
        {

        }

    }
}
