using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CoreLibrary;
using Android.Hardware;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Support.V4;

namespace FileTransferToolAndroid
{
    [Activity(Label = "File Transfer Tool", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : FragmentActivity
    {
        int count = 1;
        
        private AndroidUI _AndroidUI;
        private Core _core;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _AndroidUI = new AndroidUI();
            _core = new Core(_AndroidUI);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            
        }


        protected override void OnStop()
        {
            _core.Dispose();
            base.OnStop();
        }


    }


}

