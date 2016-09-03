using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLibrary;

namespace FileTransferTool
{
    public class WindowsUI : FTUI
    {


        public MainWindow Window;

        private delegate void availFilesChanged();

        public WindowsUI()
        {
            Window = new MainWindow(this);
            Window.Load += mainWindow_OnLoad;
            Window.FormClosing += Window_FormClosing;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            // First check if there are current download operations.
            if (Window.DownloadProgressWindow.DownloadInProggress)
            {
                // Redirect closing event to download window.
                Window.DownloadProgressWindow.Close();
                e.Cancel = true;
                return;
            }


            WindowClosingEventArgs args = new WindowClosingEventArgs() { CancelClosing = false };

            InvokeWindowClosing(this, args);

            // Check to see if the closing event was canceled due to pending operations.
            if (args.CancelClosing)
            {
                if (MessageBox.Show("Files are currently being uploaded to other computers, do you wish to cancel uploads?", "Cancel Uploads", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    // Closing event was not canceled, so let the core know the program is going to be closed so it can dispose properly.
                    InvokeExit(this, EventArgs.Empty);
                }
                else
                {
                    // Closing event canceled.
                    e.Cancel = true;
                }
            }
            else
            {
                InvokeExit(this, EventArgs.Empty);
            }
        }

        private void mainWindow_OnLoad(object sender, EventArgs e)
        {
            InvokeRefreshClients(this, EventArgs.Empty);
        }

        /// <summary>
        /// Maps a mainWindows internal event to trigge the FilesSelected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowFilesSelected(object sender, FilesSelectedEventArgs e)
        {
            InvokeFilesSelected(sender, e);
        }

        /// <summary>
        /// Maps a mainWindows internal event to trigger the FilesRemoved event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowFilesRemoved(object sender, FilesRemovedEventArgs e)
        {
            InvokeFilesRemoved(sender, e);
        }


        /// <summary>
        /// Maps a mainWindows internal event to trigger the RefreshClients event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowRefresh(object sender, EventArgs e)
        {
            InvokeRefreshClients(sender, e);
        }


        /// <summary>
        /// Maps a mainWindows internal event to trigger the DownloadCancel event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowDownloadCancel(object sender, EventArgs e)
        {
            InvokeDownloadCancel(sender, e);
        }


        /// <summary>
        /// Maps a mainWindows internal event to to the DownloadRequest event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowDownloadfiles(object sender, DownloadRequestEventArgs e)
        {
            InvokeDownloadRequest(sender, e);
        }


        public override void AvailableFilesChanged(List<FTTFileInfo> files)
        {

            Window.AvailableGridManager.FilesChanged(files);
            Window.Invoke((availFilesChanged)delegate { Window.AvailFilesChanged(); });
        }

        public override void SharedFilesChanged(List<FileHandler> files)
        {
            Window.SharedGridManager.FilesChanged(files);
            Window.Invoke((availFilesChanged)delegate { Window.checkSharedChecks(); });
        }

        
    }
}
