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
using FileTransferTool.CoreLibrary;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.AndroidApp
{

    class FileBrowserFileInfo : Checkable
    {

        public static int IDCount { get; set; }

        public bool Checked { get; set; }
        public int ID { get; set; }


        public FileBrowserFileInfo()
        {
            ID = IDCount;
            IDCount++;
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public Boolean IsDirectory { get; set; }
    }
}