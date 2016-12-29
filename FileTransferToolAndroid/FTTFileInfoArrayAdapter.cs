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
using Java.Lang;

namespace FileTransferToolAndroid
{
    class FTTFileInfoArrayAdapter : FileArrayAdapter<CheckableFileInfo>
    {


        public FTTFileInfoArrayAdapter(Context context, List<CheckableFileInfo> files) : base(context, Resource.Layout.AvailableFilesListItem, files)
        {
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            View fileView = inflater.Inflate(Resource.Layout.AvailableFilesListItem, parent, false);


            TextView textView = fileView.FindViewById<TextView>(Resource.Id.Name);
            textView.SetText(Files[position].Alias, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Location);
            textView.SetText( Files[position].IP, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Size);
            textView.SetText( Files[position].Size, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.ID);
            textView.SetText(Files[position].ID.ToString(), TextView.BufferType.Normal);

            CheckBox checkBox = fileView.FindViewById<CheckBox>(Resource.Id.FileCheckbox);
            checkBox.Click += CheckBox_Click;
            checkBox.Checked = Files.ElementAt(position).Checked;

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