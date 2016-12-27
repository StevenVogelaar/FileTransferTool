using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLibrary;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FileTransferToolAndroid
{
    class AndroidUI : FTUI
    {

        private Activity _activity;

        public delegate void AvailableFilesChangedHandler(object sender, AvailableFilesChangedEventArgs e);
        public event AvailableFilesChangedHandler AvailableFilesChangedEvent;

        public delegate void SharedFilesChangedHandler(object sender, SharedFilesChangedEventArgs e);
        public event SharedFilesChangedHandler SharedFilesChangedEvent;

        public AndroidUI(Activity activity) : base(){

            _activity = activity;
        }


 
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
            throw new NotImplementedException();
        }

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

    }
}