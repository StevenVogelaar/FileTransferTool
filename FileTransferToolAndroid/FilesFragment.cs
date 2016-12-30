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
    class FilesFragment<T> : Android.Support.V4.App.Fragment where T:Checkable 
    {

        public delegate void FilesCheckedHandler(object sender, FilesCheckedEventArgs e);
        public event FilesCheckedHandler FilesChecked;

        private ListView _listView;


        protected View _rootView;
        protected FileArrayAdapter<T> _adapter;

        public FilesFragment(FileArrayAdapter<T> adapter)
        {
            _adapter = adapter;
            _adapter.CheckBoxChecked += _adapter_CheckBoxChecked;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.FilesListPage, container, false);
            _listView = _rootView.FindViewById<ListView>(Resource.Id.FilesList);
            _listView.Adapter = _adapter;
            _adapter.SetListView(_listView);
            return _rootView;
        }

        private void _adapter_CheckBoxChecked(object sender, EventArgs e)
        {
            CheckCheckBoxes();
        }

        public void RefreshList()
        {
            _adapter.NotifyDataSetChanged();
        }




        public void CheckCheckBoxes()
        {
            ListView listView = _rootView.FindViewById<ListView>(Resource.Id.FilesList);

            foreach (T f in _adapter.Files)
            {
                if (f.Checked)
                {
                    if (FilesChecked != null)
                    {
                        FilesChecked.Invoke(this, new FilesCheckedEventArgs() { SomeChecked = true });
                        return;
                    }
                }
            }

            if (FilesChecked != null)
            {
                FilesChecked.Invoke(this, new FilesCheckedEventArgs() { SomeChecked = false });
            }
        }

        public T[] getChecked()
        {

            return _adapter.GetChecked().ToArray();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }


        public override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// Called when the available files are changed.
        /// </summary>
        /// <param name="files"></param>
        public void FilesChanged(List<T> files)
        {
            //_adapter.Files.Clear();

            foreach (T f in files)
            {

                if (!_adapter.Files.Contains(f))
                {
                    _adapter.Files.Add(f);
                }
            }

            List<T> removeList = new List<T>();

            foreach (T f in _adapter.Files)
            {
                if (!files.Contains(f))
                {
                    // A file that was in the list didnt come back from the refresh, so remove it.
                    removeList.Add(f);
                }
            }

            foreach (T f in removeList)
            {
                _adapter.Files.Remove(f);
            }

            _adapter.NotifyDataSetChanged();
            CheckCheckBoxes();
        }



        public class FilesCheckedEventArgs : EventArgs
        {
            public bool SomeChecked { get; set; }
        }

    }
}