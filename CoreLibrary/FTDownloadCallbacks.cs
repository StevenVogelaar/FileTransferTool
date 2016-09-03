using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public abstract class FTDownloadCallbacks
    {

        public delegate void CancelRequestedHandler(object sender, EventArgs e);
        public event CancelRequestedHandler CancelRequested;


        protected void InvokeCancelRequested(object sender)
        {
            if (CancelRequested != null) CancelRequested.Invoke(sender, EventArgs.Empty);
        }

        public abstract void DownloadProgress(String alias, int progress, String ip);
        public abstract void FolderDownloadProgress(String alias, long progress, String ip);
        public abstract void DownloadCompleted();
        public abstract void DownloadStarted();
    }
}
