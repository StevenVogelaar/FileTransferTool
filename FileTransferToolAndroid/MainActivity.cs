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

        private static readonly int SELECT_FOLDER_CODE = 0;

        private AndroidUI _AndroidUI;
        private Core _core;
        private FTTFragmentPageAdapter _pageAdapter;
        private ViewPager _viewPager;


        private String[] mPlanetTitles;
        private DrawerLayout mDrawerLayout;
        private ListView mDrawerList;
        private Android.Support.V7.App.ActionBarDrawerToggle _drawerToggle;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private VisibleToolbarActions _visibleToolbarActions;

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
            _visibleToolbarActions = new VisibleToolbarActions() { add = true, refresh = false, download = false, remove = false };

            initDrawer();

            // Init view pager.
            _pageAdapter = new FTTFragmentPageAdapter(SupportFragmentManager);
            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = _pageAdapter;
            _viewPager.PageSelected += _viewPager_PageSelected;
            _pageAdapter.AvailableFilesFragment.AvailableFilesChecked += AvailableFilesFragment_AvailableFilesChecked;


            _AndroidUI = new AndroidUI(this);
            _AndroidUI.AvailableFilesChangedEvent += _AndroidUI_AvailableFilesChangedEvent;
            _core = new Core(_AndroidUI);

            // Init toolbar.
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout), toolbar, Resource.String.open_drawer_acc, Resource.String.close_drawer_acc);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout);
            drawer.AddDrawerListener(_drawerToggle);

            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

      

        /// <summary>
        /// Creates the toolbar action buttons.
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            /*
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.actionbar, menu);
            */

            setToolbarActions(menu);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    showFileChooser();
                    break;
                case 3:
                    _AndroidUI.RefreshClients();   
                    break;

            }

            return true;
        }


        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            _drawerToggle.SyncState();
        }

        private void showFileChooser()
        {
            Intent intent = new Intent(Intent.ActionGetContent);
            intent.SetType("*/*");
            intent.AddCategory(Intent.CategoryOpenable);

            try
            {
                StartActivityForResult(
                        Intent.CreateChooser(intent, "Select Destination Folder"),
                        SELECT_FOLDER_CODE);
            }
            catch (Android.Content.ActivityNotFoundException ex)
            {
                // Potentially direct the user to the Market with a Dialog
                Toast.MakeText(this, "Please install a File Manager.", ToastLength.Short).Show();
            }
        }

        private void setToolbarActions(IMenu menu)
        {

            menu.Clear();

            if (_visibleToolbarActions.add)
            {
                menu.Add(Menu.None, 1, Menu.None, "Add File(s)").SetIcon(Resource.Drawable.ic_add_white_48dp).SetShowAsAction(ShowAsAction.Always);
            }
            if (_visibleToolbarActions.refresh)
            {
                menu.Add(Menu.None, 3, Menu.None, "Refresh").SetIcon(Resource.Drawable.ic_sync_white_64dp_1x).SetShowAsAction(ShowAsAction.Always);
            }
            if (_visibleToolbarActions.remove)
            {
                menu.Add(Menu.None, 2, Menu.None, "Remove File(s)").SetIcon(Resource.Drawable.ic_remove_white_48dp).SetShowAsAction(ShowAsAction.Always);
            }
            if (_visibleToolbarActions.download)
            {
                menu.Add(Menu.None, 0, Menu.None, "Download File").SetIcon(Resource.Drawable.ic_file_download_white_48dp).SetShowAsAction(ShowAsAction.Always);
            }
        }

        /// <summary>
        /// Sets the download toolbar action visible and redraws toolbar if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvailableFilesFragment_AvailableFilesChecked(object sender, AvailableFilesFragment.AvailabledFilesChckedEventArgs e)
        {
            if (e.SomeChecked)
            {
                if (!_visibleToolbarActions.download)
                {
                    _visibleToolbarActions.download = true;
                    _toolbar.Menu.Add(Menu.None, 0, Menu.None, "Download File").SetIcon(Resource.Drawable.ic_file_download_white_48dp).SetShowAsAction(ShowAsAction.Always);
                }
            }
            else
            {
                if (_visibleToolbarActions.download)
                _visibleToolbarActions.download = false;
                _toolbar.Menu.RemoveItem(0);
            }
        }

        private void _AndroidUI_AvailableFilesChangedEvent(object sender, AndroidUI.AvailableFilesChangedEventArgs e)
        {
            _pageAdapter.AvailableFilesFragment.FilesChanged(e.Files);
        }

        private void _viewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            // Make it check if there is a selection allready.

            if (e.Position == 0)
            {
                _visibleToolbarActions.add = true;
                _visibleToolbarActions.remove = false;
                _visibleToolbarActions.refresh = false;
                _visibleToolbarActions.download = false;
            }
            else
            {
                _visibleToolbarActions.add = false;
                _visibleToolbarActions.remove = false;
                _visibleToolbarActions.refresh = true;

                _pageAdapter.AvailableFilesFragment.CheckCheckBoxes();
            }

            setToolbarActions(_toolbar.Menu);
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


        private struct VisibleToolbarActions
        {
            public Boolean refresh;
            public Boolean download;
            public Boolean add;
            public Boolean remove;
        }

    }


}

