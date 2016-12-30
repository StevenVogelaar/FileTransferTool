using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.CoreLibrary.Net
{
    public abstract class FTDownloadCallbacks
    {

        public delegate void CancelRequestedHandler(object sender, EventArgs e);
        public event CancelRequestedHandler CancelRequested;


        protected void InvokeCancelRequested(object sender)
        {
            if (CancelRequested != null) CancelRequested.Invoke(sender, EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="progress">Percentage downloaded.</param>
        /// <param name="ip"></param>
        public abstract void DownloadProgress(String alias, int progress, String ip);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="progress">Total number of bytes downloaded.</param>
        /// <param name="ip"></param>
        public abstract void FolderDownloadProgress(String alias, long progress, String ip);
        public abstract void DownloadCompleted(String ip, bool error);
        public abstract void DownloadStarted();
    }
}
