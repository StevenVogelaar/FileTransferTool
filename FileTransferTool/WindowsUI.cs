﻿using System;
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


        public MainWindow MainWindow;

        public WindowsUI()
        {
            MainWindow = new MainWindow(this);
            MainWindow.Load += mainWindow_OnLoad;
        }


        private void mainWindow_OnLoad(object sender, EventArgs e)
        {
            onRefreshClients(this, EventArgs.Empty);
        }

        /// <summary>
        /// Maps a mainWindows internal event to trigge the FilesSelected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowFilesSelected(object sender, FilesSelectedEventArgs e)
        {
            onFilesSelected(sender, e);
        }

        /// <summary>
        /// Maps a mainWindows internal event to trigger the FilesRemoved event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowFilesRemoved(object sender, FilesRemovedEventArgs e)
        {
            onFilesRemoved(sender, e);
        }


        /// <summary>
        /// Maps a mainWindows internal event to trigger the RefreshClients event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowRefresh(object sender, EventArgs e)
        {
            onRefreshClients(sender, e);
        }


        /// <summary>
        /// Maps a mainWindows internal event to to the DownloadRequest event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindowDownloadfiles(object sender, DownloadRequestEventArgs e)
        {
            onDownloadRequest(sender, e);
        }


        public override void AvailableFilesChanged(List<FTTFileInfo> files)
        {
            MainWindow.AvailableGridManager.FilesChanged(files);
            MainWindow.AvailFilesChanged();
        }

        public override void SharedFilesChanged(List<FileHandler> files)
        {
            MainWindow.SharedGridManager.FilesChanged(files);
            MainWindow.checkSharedChecks();
        }

        public override void DownloadStarted(FTTFileInfo file)
        {
            
        }

        
    }
}
