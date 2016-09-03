using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;

namespace FileTransferTool
{
    /// <summary>
    /// A class that can be used to receive call backs for file downloads. Sent as part of the DownloadFilesEventArgs
    /// </summary>
    public class FileDownloadProgress : FTDownloadCallbacks
    {
        public delegate void DownloadCompletedHandler(object sender, EventArgs e);
        public event DownloadCompletedHandler DownloadComplete;

        public delegate void DownloadProgressEventHandler(object sender, DownloadProgressEventArgs e);
        public event DownloadProgressEventHandler DownloadProgressed;

        public delegate void FolderDownloadProgressEventHandler(object sender, FolderDownloadProgressEventArgs e);
        public event FolderDownloadProgressEventHandler FolderDownloadProgressed;

        public delegate void DownloadedStartedHandler(object sender, EventArgs e);
        public event DownloadedStartedHandler DownloadHasStarted;

        public void RequestCancel()
        {
            InvokeCancelRequested(this);
        }

        /// <summary>
        /// Called when all downloads are completed for a single remote host.
        /// </summary>
        public override void DownloadCompleted()
        {
            if (DownloadComplete != null)
            {
                DownloadComplete.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Reports download progress for individual files.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="progress"></param>
        public override void DownloadProgress(string alias, int progress, string ip)
        {
            if (DownloadProgressed != null)
            {
                DownloadProgressed.Invoke(this, new DownloadProgressEventArgs { Alias = alias, Progress = progress, IP = ip });
            }
        }

        /// <summary>
        /// Report that the downloading has started.
        /// </summary>
        public override void DownloadStarted()
        {
            if (DownloadHasStarted != null)
            {
                DownloadHasStarted.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Progress report for files that are part of a folder that is being downloaded.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="progress"></param>
        public override void FolderDownloadProgress(string alias, long progress, string ip)
        {
            if (FolderDownloadProgressed != null)
            {
                FolderDownloadProgressed.Invoke(this, new FolderDownloadProgressEventArgs() {Alias = alias, IP = ip, Progress = progress });
            }
        }


        public class DownloadProgressEventArgs : EventArgs
        {
            public String Alias { get; set; }
            public int Progress { get; set; }
            public String IP { get; set; }
        }

        public class FolderDownloadProgressEventArgs : EventArgs
        {
            public String Alias { get; set; }
            public long Progress { get; set; }
            public String IP { get; set; }
        }

    }
}
