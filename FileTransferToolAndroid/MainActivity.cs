using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using CoreLibrary;
using Android.Hardware;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Support.V4;
using Android.Support.V4.View;
using Android.Support.V4.Widget;

namespace FileTransferToolAndroid
{
    [Activity(Label = "File Transfer Tool", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : FragmentActivity
    {
        int count = 1;
        
        private AndroidUI _AndroidUI;
        private Core _core;
        private FTTFragmentPageAdapter _pageAdapter;
        private ViewPager _viewPager;


        private String[] mPlanetTitles;
        private DrawerLayout mDrawerLayout;
        private ListView mDrawerList;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            try
            {
                SetContentView(Resource.Layout.Main);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            //ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            mPlanetTitles = new string[] {"  title1", "  title2", "  title3" };
            mDrawerLayout = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);
            mDrawerList = (ListView)FindViewById(Resource.Id.left_drawer);

            // Set the adapter for the list view
            mDrawerList.Adapter =  new ArrayAdapter<String>(this,Resource.Layout.DrawerItem, mPlanetTitles);
            // Set the list's click listener
            mDrawerList.ItemClick +=  (delegate(object sender, AdapterView.ItemClickEventArgs e) { });


            //_AndroidUI = new AndroidUI();
            //_core = new Core(_AndroidUI);

            
            _pageAdapter = new FTTFragmentPageAdapter(SupportFragmentManager);
            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            if (_viewPager == null)
            {
                Console.WriteLine("AWDKAWKDKAWKDAWKKDAW");
                return;
            }
            _viewPager.Adapter = _pageAdapter;
            

            
        }


        protected override void OnStop()
        {
            _core.Dispose();
            base.OnStop();
        }


    }


}

