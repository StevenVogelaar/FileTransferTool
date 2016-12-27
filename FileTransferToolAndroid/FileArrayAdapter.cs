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
    abstract class FileArrayAdapter<T> : ArrayAdapter<T>
    {
        public delegate void CheckBoxCheckedHandler(object sender, EventArgs e);
        public event CheckBoxCheckedHandler CheckBoxChecked;


        protected Context _context;
        public List<T> Files { get; private set; }
        protected List<View> _items;


        public FileArrayAdapter(Context context, int itemLayout, List<T> files ) : base(context, itemLayout, files)
        {
            Files = files;
            _context = context;
            
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