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


        public delegate void AvailableFilesCheckedHandler(object sender, AvailabledFilesChckedEventArgs e);
        public event AvailableFilesCheckedHandler AvailableFilesChecked;


        private List<FTTFileInfo> _files;
        private View _rootView;
        private FTTFileInfoArrayAdapter _adapter;

        public AvailableFilesFragment()
        {
            _files = new List<FTTFileInfo>();
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _rootView = inflater.Inflate(Resource.Layout.AvailableFiles, container, false);
            ListView listView = _rootView.FindViewById<ListView>(Resource.Id.availableFilesList);
            _adapter = (new FTTFileInfoArrayAdapter(Context, _files));
            listView.Adapter = _adapter;
            _adapter.CheckBoxChecked += _adapter_CheckBoxChecked;
            return _rootView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


        }

        /// <summary>
        /// Called when the available files are changed.
        /// </summary>
        /// <param name="files"></param>
        public void FilesChanged(List<FTTFileInfo> files)
        {
            _files.Clear();
            
            foreach (FTTFileInfo f in files)
            {
                _files.Add(f);
            }

            _adapter.NotifyDataSetChanged();
            CheckCheckBoxes();
        }

       

        public override void OnPause()
        {
            base.OnPause();
        }


        private void _adapter_CheckBoxChecked(object sender, EventArgs e)
        {
            CheckCheckBoxes();
        }

        /// <summary>
        /// Checkes for checked check boxes. Invokes an event with the result.
        /// </summary>
        public void CheckCheckBoxes()
        {

            ListView listView = _rootView.FindViewById<ListView>(Resource.Id.availableFilesList);

            for (int i = 0; i < listView.LastVisiblePosition - listView.FirstVisiblePosition + 1; i++)
            {

                View view = listView.GetChildAt(i);
                CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.AvailCheckbox);

                if (checkBox.Checked)
                {
                    if (AvailableFilesChecked != null)
                    {
                        AvailableFilesChecked.Invoke(this, new AvailabledFilesChckedEventArgs() { SomeChecked = true });
                        return;
                    }
                }
            }

            AvailableFilesChecked.Invoke(this, new AvailabledFilesChckedEventArgs() { SomeChecked = false });
        }


        public class AvailabledFilesChckedEventArgs : EventArgs
        {
            public bool SomeChecked { get; set; }
        }

    }
}