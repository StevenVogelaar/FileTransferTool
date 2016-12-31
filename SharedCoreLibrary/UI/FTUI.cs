using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.CoreLibrary.UI
{
    public abstract class FTUI
    {

        public delegate void WindowClosingHandler(object sender, WindowClosingEventArgs e);
        /// <summary>
        /// Invoked when a closing operation is requested, so that it can be canceled if there are pending operations.
        /// </summary>
        public event WindowClosingHandler WindowClosing;

        public delegate void ExitHandler(object sender, EventArgs e);
        /// <summary>
        /// Exit is requested with no cancelations.
        /// </summary>
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
        /// For derrived classes to invoke the WindowClosing event. This has the posibility of canceling to get user input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeWindowClosing(object sender, WindowClosingEventArgs e)
        {
            if (WindowClosing != null) WindowClosing.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived classes to invoke the Exit event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeExit(object sender, EventArgs e)
        {
            if (Exit != null)
            {
                Exit.Invoke(sender, e);
            }
        }

        /// <summary>
        /// For derrived classes to invoke the RefreshClients event. This forces an exit without a chance to cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeRefreshClients(object sender, EventArgs e)
        {
            if (RefreshClients != null) RefreshClients.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived classes to invoke the FilesSelected event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeFilesSelected(object sender, FilesSelectedEventArgs e)
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
        public void InvokeFilesRemoved(object sender, FilesRemovedEventArgs e)
        {
            if (FilesRemoved != null) FilesRemoved.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived clasess to invoke the DownloadRequest event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeDownloadRequest(object sender, DownloadRequestEventArgs e)
        {
            if (DownloadRequest != null) DownloadRequest.Invoke(sender, e);
        }

        /// <summary>
        /// For derrived classes to invoke the DownloadCancel event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void InvokeDownloadCancel(object sender, EventArgs e)
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
            public String[] FilePaths { get; }
            public FilesRemovedEventArgs(String[] filesPaths)
            {
                this.FilePaths = filesPaths;
            }
        }

        public class DownloadRequestEventArgs : EventArgs
        {
            public FTDownloadCallbacks CallBacks { get; set; }
            public FTTFileInfo[] FileResult { get; set; }
            public FTTFileInfo[] Files { get; set; }
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
