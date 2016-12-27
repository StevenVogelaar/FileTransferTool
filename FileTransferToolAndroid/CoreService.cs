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
using Java.IO;

namespace FileTransferToolAndroid
{
    class CoreService : Service
    {


        public AndroidUI AndroidUI { get; set; }


        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

    }

}