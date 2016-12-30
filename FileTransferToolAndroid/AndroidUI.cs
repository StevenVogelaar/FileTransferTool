using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileTransferTool.CoreLibrary;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.AndroidApp
{
    public class AndroidUI : FTUI
    {

        private Activity _activity;

        public delegate void AvailableFilesChangedHandler(object sender, AvailableFilesChangedEventArgs e);
        public event AvailableFilesChangedHandler AvailableFilesChangedEvent;

        public delegate void SharedFilesChangedHandler(object sender, SharedFilesChangedEventArgs e);
        public event SharedFilesChangedHandler SharedFilesChangedEvent;

        public delegate void FailedToConnectHandler(object sender, ConnectionFailedEventArgs e);
        public event FailedToConnectHandler ConnectionFailed;

        public AndroidUI(Activity activity) : base(){

            _activity = activity;
        }




 
        /// <summary>
        /// Called by the core when a change in available files has been finalized.
        /// </summary>
        /// <param name="files"></param>
        public override void AvailableFilesChanged(List<FTTFileInfo> files)
        {
            if (AvailableFilesChangedEvent != null)
            {
                _activity.RunOnUiThread(delegate () 
                {
                    AvailableFilesChangedEvent.Invoke(this, new AvailableFilesChangedEventArgs(files));
                });
               
            }
        }


        public override void FailedToConnect(string ip)
        {
            if (ConnectionFailed != null)
            {
                ConnectionFailed.Invoke(this, new ConnectionFailedEventArgs() { IP = ip });
            }
        }


        /// <summary>
        /// Called by the core when a change in the shared files is finalized.
        /// </summary>
        /// <param name="files"></param>
        public override void SharedFilesChanged(List<FileHandler> files)
        {
            if (SharedFilesChangedEvent != null)
            {
                _activity.RunOnUiThread(delegate ()
                {
                    SharedFilesChangedEvent.Invoke(this, new SharedFilesChangedEventArgs(files));
                });
            }
        }


        public class AvailableFilesChangedEventArgs : EventArgs
        {
            public List<FTTFileInfo> Files { get; set; }

            public AvailableFilesChangedEventArgs(List<FTTFileInfo> files)
            {
                Files = files;
            }
        }

        public class SharedFilesChangedEventArgs : EventArgs
        {
            public List<FileHandler> Files { get; set; }

            public SharedFilesChangedEventArgs(List<FileHandler> files)
            {
                Files = files;
            }
        }

        public class ConnectionFailedEventArgs : EventArgs
        {
            public String IP { get; set; }
        }

    }
}