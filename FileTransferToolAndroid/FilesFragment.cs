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
    class FilesFragment<T> : Android.Support.V4.App.Fragment
    {

        public delegate void FilesCheckedHandler(object sender, FilesCheckedEventArgs e);
        public event FilesCheckedHandler FilesChecked;


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
            ListView listView = _rootView.FindViewById<ListView>(Resource.Id.FilesList);
            listView.Adapter = _adapter;
            return _rootView;
        }

        private void _adapter_CheckBoxChecked(object sender, EventArgs e)
        {
            CheckCheckBoxes();
        }

        public void refreshList()
        {
            _adapter.NotifyDataSetChanged();
        }

        public void CheckCheckBoxes()
        {
            ListView listView = _rootView.FindViewById<ListView>(Resource.Id.FilesList);

            for (int i = 0; i < listView.LastVisiblePosition - listView.FirstVisiblePosition + 1; i++)
            {

                View view = listView.GetChildAt(i);
                CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.FileCheckbox);

                if (checkBox.Checked)
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
            _adapter.Files.Clear();

            foreach (T f in files)
            {
                _adapter.Files.Add(f);
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