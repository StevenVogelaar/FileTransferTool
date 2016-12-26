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

namespace FileTransferToolAndroid
{

    class FileBrowserFileInfo
    {

        public string Name { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public Boolean IsDirectory { get; set; }
    }
}