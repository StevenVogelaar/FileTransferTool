using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Support.V4;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;
using FileTransferTool.CoreLibrary;
using System.Threading.Tasks;


namespace FileTransferTool.AndroidApp
{
    [Activity(Label = "File Transfer Tool", MainLauncher = true, Icon = "@drawable/icon72", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity
    {


        public static List<FTTFileInfo> DownloadFiles = new List<FTTFileInfo>();
        public static AndroidUI _AndroidUI;

        private const int DOWNLOAD = 0;
        private const int ADD_FILES = 1;
        private const int REMOVE_FILES = 2;
        private const int REFRESH = 3;
        private const int ADD_FOLDER = 4;
        

        
        private Core _core;
        private FTTFragmentPageAdapter _pageAdapter;
        private ViewPager _viewPager;


        private String[] mPlanetTitles;
        private DrawerLayout mDrawerLayout;
        private ListView mDrawerList;
        private Android.Support.V7.App.ActionBarDrawerToggle _drawerToggle;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private VisibleToolbarActions _visibleToolbarActions;
        private ProgressDialog _progressDialog;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            try
            {
                SetContentView(Resource.Layout.Main);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message + "\n" + e.StackTrace); 
            }

            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            _visibleToolbarActions = new VisibleToolbarActions() { add = true, refresh = false, download = false, remove = false, add_folder = true };

            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage("Refreshing...");


            initDrawer();

            // Init view pager.
            _pageAdapter = new FTTFragmentPageAdapter(SupportFragmentManager, this);
            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = _pageAdapter;
            _viewPager.PageSelected += _viewPager_PageSelected;
            _pageAdapter.AvailableFilesFragment.FilesChecked += AvailableFilesFragment_FilesChecked;
            _pageAdapter.SharedFilesFragment.FilesChecked += SharedFilesFragment_FilesChecked;


            _AndroidUI = new AndroidUI(this);
            _AndroidUI.AvailableFilesChangedEvent += _AndroidUI_AvailableFilesChangedEvent;
            _AndroidUI.SharedFilesChangedEvent += _AndroidUI_SharedFilesChangedEvent;
            _core = new Core(_AndroidUI);

            // Init toolbar.
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_toolbar);
            _drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout), _toolbar, Resource.String.open_drawer_acc, Resource.String.close_drawer_acc);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout);
            drawer.AddDrawerListener(_drawerToggle);


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
            refreshToolbar();

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case DOWNLOAD:
                    chooseFolder(false);
                    break;
                case REFRESH:
                    refreshWithLoading(); 
                    break;
                case ADD_FILES:
                    chooseFiles();
                    break;
                case REMOVE_FILES:
                    removeSharedFiles();
                    break;
                case ADD_FOLDER:
                    chooseFolder(true);
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
        /// Removes files that have been checked in the shared files list.
        /// </summary>
        private void removeSharedFiles()
        {

            FileHandler[] files = _pageAdapter.SharedFilesFragment.getChecked();
            String[] paths = new String[files.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = files[i].Path;
            }

            _AndroidUI.InvokeFilesRemoved(this, new FTUI.FilesRemovedEventArgs(paths));

            _visibleToolbarActions.refresh = false;
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
        private void chooseFolder(bool share)
        {
            Intent location_intent = new Intent(this, typeof(FileBrowserActivity));

            if (!share)
            {
                location_intent.PutExtra(FileBrowserActivity.OPERATION_TYPE, FileBrowserActivity.SELECT_FOLDER);
                StartActivityForResult(location_intent, FileBrowserActivity.SELECT_FOLDER);
            }
            else
            {
                location_intent.PutExtra(FileBrowserActivity.OPERATION_TYPE, FileBrowserActivity.SELECT_FOLDER_FOR_SHARE);
                StartActivityForResult(location_intent, FileBrowserActivity.SELECT_FOLDER_FOR_SHARE);
            }
            
        }



        private void startDownload(String destPath)
        {

            DownloadFiles = new List<FTTFileInfo>(_pageAdapter.AvailableFilesFragment.getChecked());

            Intent intent = new Intent(this, typeof(DownloadActivity));
            intent.PutExtra(DownloadActivity.DOWNLOAD_DESTINATION, destPath);
            StartActivity(intent);

            //delegate FTDownloadCallbacks() { };

            //_AndroidUI.InvokeDownloadRequest(this, new FTUI.DownloadRequestEventArgs() { Dest = destPath, CallBacks = })
        }

        private void setToolbarActions(IMenu menu)
        {
            menu.Clear();
            

            menu.Add(Menu.None, DOWNLOAD, Menu.None, "Download File").SetIcon(Resource.Drawable.ic_file_download_white_48dp).SetShowAsAction(ShowAsAction.Always);
            menu.Add(Menu.None, ADD_FILES, Menu.None, "Add File(s)").SetIcon(Resource.Drawable.ic_add_white_48dp).SetShowAsAction(ShowAsAction.Always);
            menu.Add(Menu.None, REMOVE_FILES, Menu.None, "Remove File(s)").SetIcon(Resource.Drawable.ic_remove_white_48dp).SetShowAsAction(ShowAsAction.Always);
            menu.Add(Menu.None, REFRESH, Menu.None, "Refresh").SetIcon(Resource.Drawable.ic_sync_white_64dp_1x).SetShowAsAction(ShowAsAction.Always);
            menu.Add(Menu.None, ADD_FOLDER, Menu.None, "Refresh").SetIcon(Resource.Drawable.ic_create_new_folder_white_48dp).SetShowAsAction(ShowAsAction.Always);
        }

        /// <summary>
        /// Sets the download toolbar action visible and redraws toolbar if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvailableFilesFragment_FilesChecked(object sender, FilesFragment<CheckableFileInfo>.FilesCheckedEventArgs e)
        {
            if (e.SomeChecked)
            {
                if (!_visibleToolbarActions.download)
                {
                    _visibleToolbarActions.download = true;
                }
            }
            else
            {
                if (_visibleToolbarActions.download)
                {
                    _visibleToolbarActions.download = false;
                }
            }

            refreshToolbar();
        }


        /// <summary>
        /// Sets the remove toolbar action visible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SharedFilesFragment_FilesChecked(object sender, FilesFragment<CheckableFileHandler>.FilesCheckedEventArgs e)
        {
            if (e.SomeChecked)
            {
                if (!_visibleToolbarActions.remove)
                {
                    _visibleToolbarActions.remove = true;
                }
            }
            else
            {
                if (_visibleToolbarActions.remove)
                {
                    _visibleToolbarActions.remove = false;
                }
            }

            refreshToolbar();
        }

        private void _AndroidUI_AvailableFilesChangedEvent(object sender, AndroidUI.AvailableFilesChangedEventArgs e)
        {


            List<CheckableFileInfo> fileInfos = new List<CheckableFileInfo>();

            foreach (FTTFileInfo f in e.Files)
            {
                fileInfos.Add(new CheckableFileInfo(f));
            }

            _pageAdapter.AvailableFilesFragment.FilesChanged(fileInfos);

            _progressDialog.Hide();
        }




        private void _AndroidUI_SharedFilesChangedEvent(object sender, AndroidUI.SharedFilesChangedEventArgs e)
        {

            foreach (FileHandler f in e.Files)
            {
                f.FileInfoChanged += F_FileInfoChanged;
            }

            List<CheckableFileHandler> fileHandlers = new List<CheckableFileHandler>();

            foreach (FileHandler f in e.Files)
            {
                fileHandlers.Add(new CheckableFileHandler(f));
            }

            _pageAdapter.SharedFilesFragment.FilesChanged(fileHandlers);
        }

        private void F_FileInfoChanged(object sender, FileHandler.FileInfoChangedEventArgs e)
        {

            _pageAdapter.SharedFilesFragment.RefreshList();
        }

        private void _viewPager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            // Make it check if there is a selection already.

            if (e.Position == 0)
            {
                _visibleToolbarActions.add = true;
                _visibleToolbarActions.remove = false;
                _visibleToolbarActions.refresh = false;
                _visibleToolbarActions.download = false;
                _visibleToolbarActions.add_folder = true;

                _pageAdapter.SharedFilesFragment.CheckCheckBoxes();
            }
            else
            {
                _visibleToolbarActions.add = false;
                _visibleToolbarActions.remove = false;
                _visibleToolbarActions.refresh = true;
                _visibleToolbarActions.download = false;
                _visibleToolbarActions.add_folder = false;

                refreshWithLoading();

                _pageAdapter.AvailableFilesFragment.CheckCheckBoxes();
            }


            refreshToolbar();

            //setToolbarActions(_toolbar.Menu);
        }


        /// <summary>
        /// Shows a loading dialog and refreshes clients.
        /// </summary>
        private void refreshWithLoading()
        {

            _progressDialog.Show();

            Task task = Task.Delay(3000).ContinueWith(_ =>
            {
                _progressDialog.Hide();
              
            });

            _AndroidUI.InvokeRefreshClients(this, EventArgs.Empty);
        }

        private void refreshToolbar()
        {
            _toolbar.Menu.GetItem(ADD_FILES).SetVisible(_visibleToolbarActions.add);
            _toolbar.Menu.GetItem(REMOVE_FILES).SetVisible(_visibleToolbarActions.remove);
            _toolbar.Menu.GetItem(REFRESH).SetVisible(_visibleToolbarActions.refresh);
            _toolbar.Menu.GetItem(DOWNLOAD).SetVisible(_visibleToolbarActions.download);
            _toolbar.Menu.GetItem(ADD_FOLDER).SetVisible(_visibleToolbarActions.add_folder);
        }




        private void initDrawer()
        {
            mPlanetTitles = new string[] { "  Exit" };
            mDrawerLayout = (DrawerLayout)FindViewById(Resource.Id.main_drawer_layout);
            mDrawerList = (ListView)FindViewById(Resource.Id.left_drawer);

            // Set the adapter for the list view
            mDrawerList.Adapter = new ArrayAdapter<String>(this, Resource.Layout.DrawerItem, mPlanetTitles);
            // Set the list's click listener
            mDrawerList.ItemClick += MDrawerList_ItemClick;
        }

        private void MDrawerList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Id == 0)
            {
                backPressed();
            }
        }

        protected override void OnStop()
        {
           // _core.Dispose();
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            _progressDialog.Dismiss();
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
                        startDownload(data.GetStringExtra(FileBrowserActivity.FOLDER_SELECT_RESULT));
                    }
                    break;

                case FileBrowserActivity.SELECT_FILES:
                    if (resultCode == Result.Ok)
                    {
                        if(data == null) return;

                        Toast.MakeText(this, "File(s) Selected", ToastLength.Long).Show();
                        String[] temp = data.GetStringArrayExtra(FileBrowserActivity.FILE_SELECT_RESULT);
                        _AndroidUI.InvokeFilesSelected(this, new FTUI.FilesSelectedEventArgs(temp));
                    }
                    break;
                case FileBrowserActivity.SELECT_FOLDER_FOR_SHARE:

                    if (data == null) return;

                    String[] folder = new string[1];
                    folder[0] = data.GetStringExtra(FileBrowserActivity.FOLDER_SELECT_RESULT);
                    _AndroidUI.InvokeFilesSelected(this, new FTUI.FilesSelectedEventArgs(folder));
                    break;
            }
        }


        private void backPressed()
        {

            FTUI.WindowClosingEventArgs args = new FTUI.WindowClosingEventArgs();

            _AndroidUI.InvokeWindowClosing(this, args);

            if (args.CancelClosing)
            {
                EventHandler<DialogClickEventArgs> yesHandler = new EventHandler<DialogClickEventArgs>(delegate (object sender, DialogClickEventArgs e)
                {
                    _AndroidUI.InvokeExit(this, EventArgs.Empty);
                    Finish();
                });

                EventHandler<DialogClickEventArgs> noHandler = new EventHandler<DialogClickEventArgs>(delegate (object sender, DialogClickEventArgs e)
                {

                });

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Pending Upload Operations");
                builder.SetMessage("This application is currently uploading files to another client, do you wish to exit anyways?");
                builder.SetPositiveButton("Yes", yesHandler);
                builder.SetNegativeButton("No", noHandler);

                Dialog dialog = builder.Create();
                dialog.Show();
            }
            else
            {
                _AndroidUI.InvokeExit(this, EventArgs.Empty);
                Finish();
            }
           
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {

            if (keyCode == Keycode.Back)
            {
                backPressed();
                return true;
            }

            return base.OnKeyDown(keyCode, e);
        }


        private struct VisibleToolbarActions
        {
            public Boolean refresh;
            public Boolean download;
            public Boolean add;
            public Boolean remove;
            public Boolean add_folder;
        }

    }


}

