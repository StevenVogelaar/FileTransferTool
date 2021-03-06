﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileTransferTool.CoreLibrary;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.Windows
{
    public class WindowsUI : FTUI, IDisposable
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
                if (MessageBox.Show("Files are currently being uploaded to other computers, do you wish to cancel uploads?", "Cancel Uploads", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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


        public override void AvailableFilesChanged(List<FTTFileInfo> files)
        {
            if (!Window.IsDisposed)
            {
                Window.Invoke((availFilesChanged)delegate () { Window.AvailableGridManager.FilesChanged(files); });
                Window.Invoke((availFilesChanged)delegate () { Window.AvailFilesChanged(); });
            }
        }

        public override void SharedFilesChanged(List<FileHandler> files)
        {
            if (!Window.IsDisposed)
            {
                Window.SharedGridManager.FilesChanged(files);
                Window.Invoke((availFilesChanged)delegate { Window.checkSharedChecks(); });
            }
        }

        public override void FailedToConnect(string ip)
        {
            if (!Window.IsDisposed)
            {
                Window.DownloadProgressWindow.ConnectionFailed(ip);
            }
        }

        public void Dispose()
        {
            Window.Dispose();
        }
    }
}
