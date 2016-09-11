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

namespace FileTransferToolAndroid
{
    [Activity(Label = "File Transfer Tool", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
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
    }
}

