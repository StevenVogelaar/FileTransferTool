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
    class FileBrowserArrayAdapter : ArrayAdapter<FileBrowserFileInfo>
    {

        private Context _context;
        private List<FileBrowserFileInfo> _files;
        private List<View> _items;

        public FileBrowserArrayAdapter(Context context, List<FileBrowserFileInfo> files) : base(context, Resource.Layout.FileBrowserItem, files)
        {
            _context = context;
            _files = files;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
            View fileView = inflater.Inflate(Resource.Layout.FileBrowserItem, parent, false);

            TextView textView = fileView.FindViewById<TextView>(Resource.Id.Name);
            textView.SetText(_files[position].Name, TextView.BufferType.Normal);

            if (_files[position].IsDirectory)
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
               

                fileView.FindViewById<CheckBox>(Resource.Id.file_browser_checkbox).Visibility = ViewStates.Visible;

                textView = fileView.FindViewById<TextView>(Resource.Id.Size);
                textView.Visibility = ViewStates.Visible;
                textView.SetText(_files[position].Size, TextView.BufferType.Normal);

                textView = fileView.FindViewById<TextView>(Resource.Id.Name);

                ImageView imageView = fileView.FindViewById<ImageView>(Resource.Id.file_image);
                imageView.Visibility = ViewStates.Gone;
            }


            return fileView;

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