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
        public delegate void CancelCallack();
        public bool DownloadInProggress { get; private set; }


        /// <summary>
        /// (Alias, IP)
        /// </summary>
        private FileDownloadProgress _fileDownloadProgress;
        private List<ProgressData> _data;
        private CancelCallack _cancelCallback;
        

        public DownloadProgressWindow(CancelCallack cancelCallback)
        {
            InitializeComponent();
            DownloadInProggress = false;
            _cancelCallback = cancelCallback;
            FormClosing += DownloadProgressWindow_FormClosing;
            SizeChanged += DownloadProgressWindow_SizeChanged;
            this.MinimumSize = new Size(this.Width, this.Height);

            DownloadProgressWindow_SizeChanged(this, EventArgs.Empty);
        }


        // Handle changes to the window size.
        private void DownloadProgressWindow_SizeChanged(object sender, EventArgs e)
        {
            // Change size of datagrid.
            DownloadList.Width = Width - 40;
            DownloadList.Height = Height - 100;

            // Move buttons.
            closeButton.Location = new Point(Width - 101, Height - 70);
            cancelButton.Location = new Point(closeButton.Location.X - 101, Height - 70);
        }

        private void DownloadProgressWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DownloadInProggress)
            {

                if (MessageBox.Show("Cancel downloads?", "Cancel Downloads", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    cancelButton.PerformClick();
                }
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Load download window with the list of files that are to be downloaded.
        /// </summary>
        /// <param name="filesAliases"></param>
        /// <param name="fileDownloadProgress"></param>
        public void StartDownload(List<ProgressData> filesAliases, FileDownloadProgress fileDownloadProgress)
        {
            _data = filesAliases;
            _fileDownloadProgress = fileDownloadProgress;
            _fileDownloadProgress.DownloadComplete += _fileDownloadProgress_DownloadComplete;
            _fileDownloadProgress.DownloadHasStarted += _fileDownloadProgress_DownloadHasStarted;
            _fileDownloadProgress.DownloadProgressed += _fileDownloadProgress_DownloadProgressed;
            _fileDownloadProgress.FolderDownloadProgressed += _fileDownloadProgress_FolderDownloadProgressed;
            DownloadInProggress = true;

            // Reset buttons.
            cancelButton.Enabled = true;
            closeButton.Enabled = false;

            initList();
        }


        /// <summary>
        /// Updates UI to reflect folder download progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileDownloadProgress_FolderDownloadProgressed(object sender, FileDownloadProgress.FolderDownloadProgressEventArgs e)
        {
            foreach (ProgressData file in _data)
            {
                if (file.Alias.Equals(e.Alias) && file.IP.Equals(e.IP))
                {
                    double temp = (((double)e.Progress / (double)file.Size) * 100);
                    file.Progress = (int)(((double)e.Progress / (double)file.Size) * 100);
                }
            }

            DownloadList.Refresh();
        }


        /// <summary>
        /// Updates UI to reflect file download progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileDownloadProgress_DownloadProgressed(object sender, FileDownloadProgress.DownloadProgressEventArgs e)
        {
            foreach (ProgressData file in _data)
            {
                if (file.Alias.Equals(e.Alias) && file.IP.Equals(e.IP))
                {
                    file.Progress = e.Progress;
                }
            }

            DownloadList.Refresh();
        }

        private void _fileDownloadProgress_DownloadHasStarted(object sender, EventArgs e)
        {
            Show();
        }

        private void _fileDownloadProgress_DownloadComplete(object sender, EventArgs e)
        {
            DownloadInProggress = false;
            _fileDownloadProgress.DownloadComplete -= _fileDownloadProgress_DownloadComplete;
            _fileDownloadProgress.DownloadHasStarted -= _fileDownloadProgress_DownloadHasStarted;
            _fileDownloadProgress.DownloadProgressed -= _fileDownloadProgress_DownloadProgressed;

            cancelButton.Enabled = false;
            closeButton.Enabled = true;
        }

        private void initList()
        {

            // Bind Data
            DownloadList.DataSource = _data;

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancelCallback();
            Hide();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public class ProgressData
        {
            public String Alias { get; set; }
            public String IP { get; set; }
            public int Progress { get; set; }
            public long Size { get; set; }
        }

    }
}
