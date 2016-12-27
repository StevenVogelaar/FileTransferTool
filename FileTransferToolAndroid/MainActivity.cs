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

        private const int DOWNLOAD = 0;
        private const int ADD_FILES = 1;
        private const int REMOVE_FILES = 2;
        private const int REFRESH = 3;

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
            _pageAdapter = new FTTFragmentPageAdapter(SupportFragmentManager, this);
            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = _pageAdapter;
            _viewPager.PageSelected += _viewPager_PageSelected;
            _pageAdapter.AvailableFilesFragment.FilesChecked += AvailableFilesFragment_FilesChecked;


            _AndroidUI = new AndroidUI(this);
            _AndroidUI.AvailableFilesChangedEvent += _AndroidUI_AvailableFilesChangedEvent;
            _AndroidUI.SharedFilesChangedEvent += _AndroidUI_SharedFilesChangedEvent;
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
                case DOWNLOAD:
                    chooseFolder();
                    break;
                case REFRESH:
                    _AndroidUI.InvokeRefreshClients(this, EventArgs.Empty);   
                    break;
                case ADD_FILES:
                    chooseFiles();
                    break;
            }

            return true;
        }


        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);

            _drawerToggle.SyncState();
        }


        /// <summary>
        /// Starts a choose files operation, creates a new activity.
        /// </summary>
        private void chooseFiles()
        {
            Intent location_intent = new Intent(this, typeof(FileBrowserActivity));
            location_intent.PutExtra(FileBrowserActivity.OPERATION_TYPE, FileBrowserActivity.SELECT_FILES);
            StartActivityForResult(location_intent, FileBrowserActivity.SELECT_FILES);
        }

        /// <summary>
        /// Starts a choose folder operation, creates new activity.
        /// </summary>
        private void chooseFolder()
        {
            Intent location_intent = new Intent(this, typeof(FileBrowserActivity));
            location_intent.PutExtra(FileBrowserActivity.OPERATION_TYPE, FileBrowserActivity.SELECT_FOLDER);
            StartActivityForResult(location_intent, FileBrowserActivity.SELECT_FOLDER);
        }

        private void setToolbarActions(IMenu menu)
        {

            menu.Clear();

            if (_visibleToolbarActions.add)
            {
                menu.Add(Menu.None, ADD_FILES, Menu.None, "Add File(s)").SetIcon(Resource.Drawable.ic_add_white_48dp).SetShowAsAction(ShowAsAction.Always);
            }
            if (_visibleToolbarActions.refresh)
            {
                menu.Add(Menu.None, REFRESH, Menu.None, "Refresh").SetIcon(Resource.Drawable.ic_sync_white_64dp_1x).SetShowAsAction(ShowAsAction.Always);
            }
            if (_visibleToolbarActions.remove)
            {
                menu.Add(Menu.None, REMOVE_FILES, Menu.None, "Remove File(s)").SetIcon(Resource.Drawable.ic_remove_white_48dp).SetShowAsAction(ShowAsAction.Always);
            }
            if (_visibleToolbarActions.download)
            {
                menu.Add(Menu.None, DOWNLOAD, Menu.None, "Download File").SetIcon(Resource.Drawable.ic_file_download_white_48dp).SetShowAsAction(ShowAsAction.Always);
            }
        }

        /// <summary>
        /// Sets the download toolbar action visible and redraws toolbar if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvailableFilesFragment_FilesChecked(object sender, FilesFragment<FTTFileInfo>.FilesCheckedEventArgs e)
        {
            if (e.SomeChecked)
            {
                if (!_visibleToolbarActions.download)
                {
                    _visibleToolbarActions.download = true;
                    _toolbar.Menu.Add(Menu.None, DOWNLOAD, Menu.None, "Download File").SetIcon(Resource.Drawable.ic_file_download_white_48dp).SetShowAsAction(ShowAsAction.Always);
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


        private void _AndroidUI_SharedFilesChangedEvent(object sender, AndroidUI.SharedFilesChangedEventArgs e)
        {

            foreach (FileHandler f in e.Files)
            {
                f.FileInfoChanged += F_FileInfoChanged;
            }

            _pageAdapter.SharedFilesFragment.FilesChanged(e.Files);
        }

        private void F_FileInfoChanged(object sender, FileHandler.FileInfoChangedEventArgs e)
        {
            _pageAdapter.SharedFilesFragment.refreshList();
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
           // _core.Dispose();
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            _core.Dispose();
            base.OnDestroy();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);


            switch (requestCode)
            {
                case FileBrowserActivity.SELECT_FOLDER:
                    if (resultCode == Result.Ok)
                    {
                        Toast.MakeText(this, data.GetStringExtra(FileBrowserActivity.FOLDER_SELECT_RESULT), ToastLength.Long).Show();
                    }
                    break;

                case FileBrowserActivity.SELECT_FILES:
                    if (resultCode == Result.Ok)
                    {
                        Toast.MakeText(this, "Files Selected", ToastLength.Long).Show();
                        _AndroidUI.InvokeFilesSelected(this, new FTUI.FilesSelectedEventArgs(data.GetStringArrayExtra(FileBrowserActivity.FILE_SELECT_RESULT)));
                    }
                    break;
            }
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

