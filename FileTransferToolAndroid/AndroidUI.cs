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





        public override void AvailableFilesChanged(List<FTTFileInfo> files)
        {
            throw new NotImplementedException();
        }

        public override void FailedToConnect(string ip)
        {
            throw new NotImplementedException();
        }

        public override void SharedFilesChanged(List<FileHandler> files)
        {
            throw new NotImplementedException();
        }
    }
}