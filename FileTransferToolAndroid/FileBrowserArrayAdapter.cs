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
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.AndroidApp
{
    class FileBrowserArrayAdapter : FileArrayAdapter<FileBrowserFileInfo>
    {



        public FileBrowserArrayAdapter(Context context, List<FileBrowserFileInfo> files) : base(context, Resource.Layout.FileBrowserItem, files)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            View fileView = inflater.Inflate(Resource.Layout.FileBrowserItem, parent, false);


            TextView textView = fileView.FindViewById<TextView>(Resource.Id.ID);
            textView.SetText(Files[position].ID.ToString(), TextView.BufferType.Normal);

            textView = fileView.FindViewById<TextView>(Resource.Id.Name);
            textView.SetText(Files[position].Name, TextView.BufferType.Normal);

            if (Files[position].IsDirectory)
            {

                RelativeLayout.LayoutParams param = new RelativeLayout.LayoutParams(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                param.AddRule(LayoutRules.RightOf, Resource.Id.file_image);
                textView.LayoutParameters = param;

                textView = fileView.FindViewById<TextView>(Resource.Id.Size);
                textView.Visibility = ViewStates.Gone;

                ImageView imageView = fileView.FindViewById<ImageView>(Resource.Id.file_image);
                imageView.SetImageResource(Resource.Drawable.ic_folder_black_48dp);
                imageView.Visibility = ViewStates.Visible;

                
            }
            else
            {

                CheckBox checkBox = fileView.FindViewById<CheckBox>(Resource.Id.FileCheckbox);
                checkBox.Visibility = ViewStates.Visible;
                checkBox.Checked = Files[position].Checked;
                checkBox.Click += CheckBox_Click;

                textView = fileView.FindViewById<TextView>(Resource.Id.Size);
                textView.Visibility = ViewStates.Visible;
                textView.SetText(Files[position].Size, TextView.BufferType.Normal);

                textView = fileView.FindViewById<TextView>(Resource.Id.Name);

                ImageView imageView = fileView.FindViewById<ImageView>(Resource.Id.file_image);
                imageView.Visibility = ViewStates.Gone;
            }


            return fileView;

        }

    }

}