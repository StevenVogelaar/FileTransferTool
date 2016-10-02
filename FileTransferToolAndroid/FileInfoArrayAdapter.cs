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
    class FTTFileInfoArrayAdapter : ArrayAdapter<FTTFileInfo>
    {

        public delegate void CheckBoxCheckedHandler(object sender, EventArgs e);
        public event CheckBoxCheckedHandler CheckBoxChecked;

        private Context _context;
        private List<FTTFileInfo> _files;
        private List<View> _items;

        public FTTFileInfoArrayAdapter(Context context, List<FTTFileInfo> files) : base(context, Resource.Layout.SharedFileListItem, files)
        {
            _context = context;
            _files = files;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);

            View fileView = inflater.Inflate(Resource.Layout.AvailableFilesListItem, parent, false);

            TextView textView = fileView.FindViewById<TextView>(Resource.Id.Name);
            textView.SetText(_files[position].Alias, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Location);
            textView.SetText( _files[position].IP, TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Size);
            textView.SetText( _files[position].Size, TextView.BufferType.Normal);

            CheckBox checkBox = fileView.FindViewById<CheckBox>(Resource.Id.AvailCheckbox);
            checkBox.Click += CheckBox_Click;

            /**
            if (position % 2 == 0)
            {
                //fileView.SetBackgroundColor(new Android.Graphics.Color(20,20,20));
                fileView.SetBackgroundColor(new Android.Graphics.Color(_context.GetColor(Resource.Color.primary_dark)));
            }
            */

            return fileView;
        }


        private void CheckBox_Click(object sender, EventArgs e)
        {
            if (CheckBoxChecked != null)
            {
                CheckBoxChecked.Invoke(this, EventArgs.Empty);
            }
        }


        public override int Count
        {
            get
            {
                return _files.Count;
            }
        }


       

    }
}