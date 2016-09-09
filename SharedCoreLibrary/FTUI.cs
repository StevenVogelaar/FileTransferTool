﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public abstract class FTUI
    {

        public delegate void WindowClosingHandler(object sender, WindowClosingEventArgs e);
        public event WindowClosingHandler WindowClosing;

        public delegate void ExitHandler(object sender, EventArgs e);
        public event ExitHandler Exit;

        public delegate void RefreshClientsHandler(object sender, EventArgs e);
        public event RefreshClientsHandler RefreshClients;

        public delegate void FilesSelectedHandler(object sender, FilesSelectedEventArgs e);
        public event FilesSelectedHandler FilesSelected;

        public delegate void FilesRemovedHandler(object sender, FilesRemovedEventArgs e);
        public event FilesRemovedHandler FilesRemoved;

        public delegate void DownloadRequestHandler(object sender, DownloadRequestEventArgs e);
        public event DownloadRequestHandler DownloadRequest;

        public delegate void DownloadCancelHandler(object sender, EventArgs e);
        public event DownloadCancelHandler DownloadCancel;

        /// <summary>
        /// Notify UI that the available files have changed.
        /// </summary>
        /// <param name="files"></param>
        public abstract void AvailableFilesChanged(List<FTTFileInfo> files);

        /// <summary>
        /// Notify the UI that the shared files have changed.
        /// </summary>
        /// <param name="files"></param>
        public abstract void SharedFilesChanged(List<FileHandler> files);

        /// <summary>
        /// Notify the UI that a connection attempt has failed.
        /// </summary>
        /// <param name="ip"></param>
        public abstract void FailedToConnect(String ip);

        /// <summary>
        /// For derrived classes to invoke the WindowClosing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeWindowClosing(object sender, WindowClosingEventArgs e)
        {
            if (WindowClosing != null) WindowClosing.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived classes to invoke the Exit event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeExit(object sender, EventArgs e)
        {
            if (Exit != null)
            {
                Exit.Invoke(sender, e);
            }
        }

        /// <summary>
        /// For derrived classes to invoke the RefreshClients event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeRefreshClients(object sender, EventArgs e)
        {
            if (RefreshClients != null) RefreshClients.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived classes to invoke the FilesSelected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeFilesSelected(object sender, FilesSelectedEventArgs e)
        {
            if (FilesSelected != null)
            {
                FilesSelected.Invoke(sender, e);
            }
        }

        /// <summary>
        /// For derrived clases to invoke the FilesRemoves event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeFilesRemoved(object sender, FilesRemovedEventArgs e)
        {
            if (FilesRemoved != null) FilesRemoved.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived clasess to invoke the DownloadRequest event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeDownloadRequest(object sender, DownloadRequestEventArgs e)
        {
            if (DownloadRequest != null) DownloadRequest.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived classes to invoke the DownloadCancel event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void InvokeDownloadCancel(object sender, EventArgs e)
        {
            if (DownloadCancel != null) DownloadCancel.Invoke(sender, e);
        }

        public class FilesSelectedEventArgs : EventArgs
        {
            public String[] Files { get; }
            public FilesSelectedEventArgs(String[] files)
            {
                this.Files = files;
            }
        }

        public class FilesRemovedEventArgs : EventArgs
        {
            public String[] Files { get; }
            public FilesRemovedEventArgs(String[] files)
            {
                this.Files = files;
            }
        }

        public class DownloadRequestEventArgs : EventArgs
        {
            public FTDownloadCallbacks CallBacks { get; set; }
            public FTTFileInfo[] FileResult { get; set; }
            public Dictionary<String, String> Files { get; set; }
            public String Dest { get; set; }
        }

        public class WindowClosingEventArgs : EventArgs
        {
            public bool CancelClosing { get; set; }
        }

        public class DownloadStartedEventArgs : EventArgs
        {

        }
    }
}
