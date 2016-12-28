using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CoreLibrary;

namespace FileTransferToolAndroid
{
    class DownloadManager : FTDownloadCallbacks
    {





        public override void DownloadCompleted(string ip, bool error)
        {
            throw new NotImplementedException();
        }

        public override void DownloadProgress(string alias, int progress, string ip)
        {
            throw new NotImplementedException();
        }

        public override void DownloadStarted()
        {
            throw new NotImplementedException();
        }

        public override void FolderDownloadProgress(string alias, long progress, string ip)
        {
            throw new NotImplementedException();
        }
    }
}