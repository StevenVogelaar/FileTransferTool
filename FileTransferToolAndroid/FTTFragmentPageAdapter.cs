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
using Android.Support.V4.App;
using Java.Lang;
using CoreLibrary;

namespace FileTransferToolAndroid
{
    class FTTFragmentPageAdapter : FragmentPagerAdapter
    {

        public FilesFragment<FileHandler> SharedFilesFragment { get; }
        public FilesFragment<FTTFileInfo> AvailableFilesFragment { get; }

        public override int Count
        {
            get
            {
                return 2;
            }
        }


        public FTTFragmentPageAdapter(Android.Support.V4.App.FragmentManager fm, Context context) : base(fm)
        {
            SharedFilesFragment = new FilesFragment<FileHandler>(new FTTFileHandlerArrayAdapter(context, new List<FileHandler>()));
            AvailableFilesFragment = new FilesFragment<FTTFileInfo>(new FTTFileInfoArrayAdapter(context, new List<FTTFileInfo>()));
        }

        
        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            if (position == 0)
            {
                return SharedFilesFragment;
            }
            else if (position == 1)
            {
                return AvailableFilesFragment;
            }
            else return null;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            if (position == 0)
            {
                return new Java.Lang.String("Shared Files");
            }
            else if (position == 1)
            {
                return new Java.Lang.String("Available Files");
            }
            else return new Java.Lang.String("Null");
        }
    }
}