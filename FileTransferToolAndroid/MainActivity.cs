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
    [Activity(Label = "File Transfer Tool", MainLauncher = true, Icon = "@drawable/icon72", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity
    {
        int count = 1;
        
        private AndroidUI _AndroidUI;
        private Core _core;
        private FTTFragmentPageAdapter _pageAdapter;
        private ViewPager _viewPager;


        private String[] mPlanetTitles;
        private DrawerLayout mDrawerLayout;
        private ListView mDrawerList;
        private Android.Support.V7.App.ActionBarDrawerToggle _drawerToggle;

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

            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            initDrawer();

            // Init view pager.
            _pageAdapter = new FTTFragmentPageAdapter(SupportFragmentManager);
            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            if (_viewPager == null)
            {
                Console.WriteLine("AWDKAWKDKAWKDAWKKDAW");
                return;
            }
            _viewPager.Adapter = _pageAdapter;


            _AndroidUI = new AndroidUI(this);
            _AndroidUI.AvailableFilesChangedEvent += _AndroidUI_AvailableFilesChangedEvent;
            _core = new Core(_AndroidUI);

            // Init toolbar.
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout), toolbar, Resource.String.open_drawer_acc, Resource.String.close_drawer_acc);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout);
            drawer.AddDrawerListener(_drawerToggle);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.actionbar, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_refresh:
                    Console.WriteLine("REFRESH HERE");
                    break;

            }

            return true;
        }


        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            _drawerToggle.SyncState();
        }

        private void _AndroidUI_AvailableFilesChangedEvent(object sender, AndroidUI.AvailableFilesChangedEventArgs e)
        {
            _pageAdapter.AvailableFilesFragment.FilesChanged(e.Files);
        }


        private void initDrawer()
        {
            mPlanetTitles = new string[] { "  title1", "  title2", "  title3" };
            mDrawerLayout = (DrawerLayout)FindViewById(Resource.Id.main_drawer_layout);
            mDrawerList = (ListView)FindViewById(Resource.Id.left_drawer);

            // Set the adapter for the list view
            mDrawerList.Adapter = new ArrayAdapter<String>(this, Resource.Layout.DrawerItem, mPlanetTitles);
            // Set the list's click listener
            mDrawerList.ItemClick += (delegate (object sender, AdapterView.ItemClickEventArgs e) { });
        }

        protected override void OnStop()
        {
            _core.Dispose();
            base.OnStop();
        }


    }


}

