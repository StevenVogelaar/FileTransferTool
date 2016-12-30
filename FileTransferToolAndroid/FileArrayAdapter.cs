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
    abstract class FileArrayAdapter<T> : ArrayAdapter<T> where T : Checkable
    {
        public delegate void CheckBoxCheckedHandler(object sender, EventArgs e);
        public event CheckBoxCheckedHandler CheckBoxChecked;


        protected Context _context;
        public List<T> Files { get; private set; }
        protected List<View> _items;
        protected ListView _listView;


        public FileArrayAdapter(Context context, int itemLayout, List<T> files ) : base(context, itemLayout, files)
        {
            Files = files;
            _context = context;
            CheckBoxChecked += FileArrayAdapter_CheckBoxChecked;
        }


        public List<T> GetChecked()
        {
            List<T> files = new List<T>();

            foreach (T f in Files)
            {
                if (f.Checked)
                {
                    files.Add(f);
                }
            }

            return files;
        }

        public void SetListView(ListView listView)
        {
            _listView = listView;
        }

        private void FileArrayAdapter_CheckBoxChecked(object sender, EventArgs e)
        {

            if (_listView == null)
            {
                return;
            }

            for (int i = 0; i < _listView.LastVisiblePosition - _listView.FirstVisiblePosition + 1; i++)
            {

                View view = _listView.GetChildAt(i);
                CheckBox checkBox = view.FindViewById<CheckBox>(Resource.Id.FileCheckbox);

                if (checkBox == null) break;

                foreach (Checkable f in Files)
                {
                    if (f.ID == Int32.Parse(view.FindViewById<TextView>(Resource.Id.ID).Text))
                    {
                        if (checkBox.Checked)
                        {
                            f.Checked = true;
                        }
                        else f.Checked = false;

                        break;
                    }
                }
            }
        }

        protected void CheckBox_Click(object sender, EventArgs e)
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
                return Files.Count;
            }
        }
    }
}