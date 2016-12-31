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
    class DownloadFileArrayAdapter : FileArrayAdapter<CheckableFileInfo>
    {
        public DownloadFileArrayAdapter(Context context, List<CheckableFileInfo> files) : base(context, Resource.Layout.DownloadItem, files)
        {
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);

            View fileView = inflater.Inflate(Resource.Layout.DownloadItem, parent, false);

            TextView textView = fileView.FindViewById<TextView>(Resource.Id.Name);
            textView.SetText(Files[position].Alias, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Location);
            textView.SetText(Files[position].IP, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Size);
            textView.SetText(Files[position].Size, TextView.BufferType.Normal);

            fileView.FindViewById<ProgressBar>(Resource.Id.progress_bar).Progress = Files[position].progress;


            /**
            if (position % 2 == 0)
            {
                //fileView.SetBackgroundColor(new Android.Graphics.Color(20,20,20));
                fileView.SetBackgroundColor(new Android.Graphics.Color(_context.GetColor(Resource.Color.primary_dark)));
            }
            */

            return fileView;
        }

    }
}