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

        public delegate void HideCaller();
        public delegate void RefreshCaller();


        /// <summary>
        /// (Alias, IP)
        /// </summary>
        private FileDownloadProgress _fileDownloadProgress;
        private List<ProgressData> _data;
        private List<RemoteHostStatus> _remoteHostStatus;
        private CancelCallack _cancelCallback;
        private ConnectingDialog _connectingDialog;
        

        public DownloadProgressWindow(CancelCallack cancelCallback)
        {
            InitializeComponent();
            DownloadInProggress = false;
            _cancelCallback = cancelCallback;
            FormClosing += DownloadProgressWindow_FormClosing;
            SizeChanged += DownloadProgressWindow_SizeChanged;
            this.MinimumSize = new Size(this.Width, this.Height);
            _connectingDialog = new ConnectingDialog();
            _connectingDialog.Show();
            _connectingDialog.Hide();
            _connectingDialog.TopMost = true;

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

            _connectingDialog.Show();
            

            _data = filesAliases;
            _fileDownloadProgress = fileDownloadProgress;
            _fileDownloadProgress.DownloadComplete += _fileDownloadProgress_DownloadComplete;
            _fileDownloadProgress.DownloadHasStarted += _fileDownloadProgress_DownloadHasStarted;
            _fileDownloadProgress.DownloadProgressed += _fileDownloadProgress_DownloadProgressed;
            _fileDownloadProgress.FolderDownloadProgressed += _fileDownloadProgress_FolderDownloadProgressed;

            // Reset buttons.
            cancelButton.Enabled = true;
            closeButton.Enabled = false;


            // Create list of ip's with their completed status.
            HashSet<String> ipList = new HashSet<string>();
            _remoteHostStatus = new List<RemoteHostStatus>();

            foreach (ProgressData file in _data)
            {
                ipList.Add(file.IP);
            }

            foreach (String ip in ipList)
            {
                _remoteHostStatus.Add(new RemoteHostStatus() { IP = ip, Completed = false });
            }

            initList();

        }

        public void ConnectionFailed(String ip)
        {
            MessageBox.Show("Could not connect to: " + ip, "Connection Error.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            HideCaller d = _connectingDialog.Hide;
            _connectingDialog.Invoke(d);
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

            if (DownloadList.IsHandleCreated)
            {
                DownloadList.Invoke((RefreshCaller)delegate () { DownloadList.Refresh(); });
            }
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

            if (DownloadList.IsHandleCreated)
            {
                RefreshCaller d = DownloadList.Refresh;
                DownloadList.Invoke(d);
            }
            
        }

        private void _fileDownloadProgress_DownloadHasStarted(object sender, EventArgs e)
        {
            DownloadInProggress = true;
            HideCaller d = _connectingDialog.Hide;
            _connectingDialog.Invoke(d);

            HideCaller s = Show;
            this.Invoke(s);
        }

        private void _fileDownloadProgress_DownloadComplete(object sender, FileDownloadProgress.DownloadCompletedEventArgs e)
        {

            bool allCompleted = true;
            foreach (RemoteHostStatus host in _remoteHostStatus)
            {
                if (host.IP.Equals(e.IP))
                {
                    host.Completed = true;
                }

                if (!host.Completed) allCompleted = false;
            }

            if (allCompleted)
            {
                DownloadInProggress = false;

                _fileDownloadProgress.DownloadComplete -= _fileDownloadProgress_DownloadComplete;
                _fileDownloadProgress.DownloadHasStarted -= _fileDownloadProgress_DownloadHasStarted;
                _fileDownloadProgress.DownloadProgressed -= _fileDownloadProgress_DownloadProgressed;

                if (IsHandleCreated)
                {
                    this.Invoke((HideCaller)delegate ()
                    {
                        cancelButton.Enabled = false;
                        closeButton.Enabled = true;
                    });
                }
                

                // Dont set everything to 100 if an error occured.
                if (e.Error)
                {
                    MessageBox.Show("There was an issue with the remote end.", "Connection closed by remote.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Set all progress to 100 if they are close (prescision loss).
                foreach (ProgressData file in _data)
                {
                    if (file.Progress > 95)
                    {
                        file.Progress = 100;
                    }
                }


                if (DownloadList.IsHandleCreated)
                {
                    DownloadList.Invoke((RefreshCaller)delegate() { DownloadList.Refresh(); });
                }
                
            }
        }

        private void initList()
        {

            // Bind Data
            DownloadList.DataSource = _data;

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancelCallback();
            _connectingDialog.Hide();
            DownloadInProggress = false;
            Close();
            
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public class ProgressData
        {
            public String Alias { get; set; }
            public String IP { get; set; }
            public int Progress { get; set; }
            public long Size { get; set; }
        }

        private class RemoteHostStatus
        {
            public String IP { get; set; }
            public bool Completed { get; set; }
        }

    }
}
