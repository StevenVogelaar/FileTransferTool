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

namespace FileTransferToolAndroid
{
    class FTTFragmentPageAdapter : FragmentPagerAdapter
    {

        public SharedFilesFragment SharedFilesFragment { get; }
        public AvailableFilesFragment AvailableFilesFragment { get; }

        public override int Count
        {
            get
            {
                return 2;
            }
        }


        public FTTFragmentPageAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
            SharedFilesFragment = new SharedFilesFragment();
            AvailableFilesFragment = new AvailableFilesFragment();
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