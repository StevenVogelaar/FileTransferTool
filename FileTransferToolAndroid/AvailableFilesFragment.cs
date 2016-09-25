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

namespace FileTransferToolAndroid
{
    class AvailableFilesFragment : Android.Support.V4.App.Fragment
    {
        private List<FTTFileInfo> _files;
        private View _rootView;
        private FTTFileInfoArrayAdapter _adapter;

        public AvailableFilesFragment()
        {
            _files = new List<FTTFileInfo>();
        }

        public void FilesChanged(List<FTTFileInfo> files)
        {
            _files.Clear();
            
            foreach (FTTFileInfo f in files)
            {
                _files.Add(f);
            }

            _adapter.NotifyDataSetChanged();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.AvailableFiles, container, false);
            ListView listView = _rootView.FindViewById<ListView>(Resource.Id.availableFilesList);
            _adapter = (new FTTFileInfoArrayAdapter(Context, _files));
            listView.Adapter = _adapter;
            return _rootView;
        }

        public override void OnPause()
        {
            base.OnPause();
        }
    }
}