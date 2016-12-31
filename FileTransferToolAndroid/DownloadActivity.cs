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
using Java.IO;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.AndroidApp
{

    [Activity(Label = "Download", MainLauncher = false, Icon = "@drawable/icon72", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    class DownloadActivity : Android.Support.V7.App.AppCompatActivity
    {
        public const string DOWNLOAD_DESTINATION = "DOWNLOAD_DESTINATION";


        private const int CANCEL = 0;
        private const int DONE = 1;

        private ListView _listView;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private DownloadFileArrayAdapter _adapter;
        private DownloadCallbacks _download_callbacks;
        private int _unique_ips = 0;
        private int completed = 0;
        private string _download_dest;
        private bool _download_in_progress;

        private delegate void clickEventHandler(object sender, DialogClickEventArgs e);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Downloader);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);


            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_toolbar);

            _download_dest = Intent.GetStringExtra(DOWNLOAD_DESTINATION);

            // Get names of files

            List<CheckableFileInfo> downloadFiles = new List<CheckableFileInfo>();
            foreach (FTTFileInfo f in MainActivity.DownloadFiles)
            {
                downloadFiles.Add(new CheckableFileInfo(f));
            }

            _adapter = new DownloadFileArrayAdapter(this, new List<CheckableFileInfo>(downloadFiles));

            _listView = FindViewById<ListView>(Resource.Id.download_list);
            _listView.Adapter = _adapter;

            MainActivity._AndroidUI.ConnectionFailed += _AndroidUI_ConnectionFailed;

            init();
        }

        private void _AndroidUI_ConnectionFailed(object sender, AndroidUI.ConnectionFailedEventArgs e)
        {
            try
            {
                RunOnUiThread(
                    delegate ()
                    {
                        try
                        {
                            AlertDialog.Builder builder = new AlertDialog.Builder(this);
                            builder.SetTitle("Error");
                            builder.SetMessage("Failed to connect to: " + e.IP);
                            Dialog dialog = builder.Create();
                            setDownloadInProgress(false);
                            dialog.Show();
                        }
                        catch (Exception f)
                        {

                        }
                    });
            }
            catch (Exception f)
            {

            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case CANCEL:
                    _download_callbacks.Cancel();
                    setDownloadInProgress(false);
                    Toast.MakeText(this, "Download(s) Canceled.", ToastLength.Long).Show();
                    break;
                case DONE:
                    Finish();
                    break;
            }

            return true;
        }

        private void setDownloadInProgress(bool state)
        {
            if (_toolbar.Menu.Size() > 1)
            {

                if (state)
                {
                    _download_in_progress = true;

                    _toolbar.Menu.GetItem(CANCEL).SetVisible(true);
                    _toolbar.Menu.GetItem(DONE).SetVisible(false);
                }
                else
                {
                    _download_in_progress = false;

                    _toolbar.Menu.GetItem(CANCEL).SetVisible(false);
                    _toolbar.Menu.GetItem(DONE).SetVisible(true);
                    setAllProgressMax();
                }
            }
        }


        private void init()
        {

       

            List<string> unique_addresses = new List<string>();

            // Count the number of unique ip addresses that are being downloaded from.

            CheckableFileInfo[] filesCopy = new CheckableFileInfo[_adapter.Files.Count];
            _adapter.Files.CopyTo(filesCopy);

           
            // Gather unique ip addresses
            foreach (CheckableFileInfo f in filesCopy)
            {
                if (!unique_addresses.Contains(f.IP))
                {
                    unique_addresses.Add(f.IP);
                } 
            }
   

            _unique_ips = unique_addresses.Count;


            // Start the download operation.
            _download_callbacks = new DownloadCallbacks(this, downloadCompleted, downloadProgress, folderDownloadProgress, downloadStarted);

      

            MainActivity._AndroidUI.InvokeDownloadRequest(this, new FTUI.DownloadRequestEventArgs() { Dest = _download_dest, Files = filesCopy,
            CallBacks = _download_callbacks});
           
        }

        private void setToolbarActions(IMenu menu)
        {
            menu.Clear();
            menu.Add(Menu.None, CANCEL, Menu.None, "Cancel Downloads").SetIcon(Resource.Drawable.ic_clear_white_48dp).SetShowAsAction(ShowAsAction.Always);
            menu.GetItem(CANCEL).SetVisible(true);
            menu.Add(Menu.None, DONE, Menu.None, "Done").SetIcon(Resource.Drawable.ic_done_white_48dp).SetShowAsAction(ShowAsAction.Always);
            menu.GetItem(DONE).SetVisible(false);
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



        /// <summary>
        /// Sometimes things will finish with 99 instead of 100. This will set everything to 100.
        /// </summary>
        private void setAllProgressMax()
        {
            for (int i = 0; i < _adapter.Files.Count; i++)
            {
                _adapter.Files[i].progress = 100;
            }

            _adapter.NotifyDataSetChanged();
        }

        private void downloadCompleted(String ip, bool error)
        {
            // Need to check how many times this has been fired.
            completed ++;

            if (error)
            {
                Toast.MakeText(this, "Error: Connection closed by remote.", ToastLength.Long).Show();
            }

            if (completed == _unique_ips)
            {
                // All downloads have completed
                setDownloadInProgress(false);
            }
        }

        private void downloadProgress(string alias, int progress, string ip)
        {
            for (int i = 0; i < _adapter.Files.Count; i ++)
            {
                CheckableFileInfo f = _adapter.Files.ElementAt(i);

                if (f.Alias == alias && f.IP == ip)
                {
                    f.progress = progress;
                }
            }

            _adapter.NotifyDataSetChanged();
        }

        private void folderDownloadProgress(string alias, long progress, string ip)
        {
            for (int i = 0; i < _adapter.Files.Count; i++)
            {
                CheckableFileInfo f = _adapter.Files.ElementAt(i);

                if (f.Alias == alias && f.IP == ip)
                {
                    f.progress = (int)(((float)progress / (FileHandler.ParseSize(f.Size))) * 100);
                }
            }

            _adapter.NotifyDataSetChanged();
        }

        private void downloadStarted()
        {
            setDownloadInProgress(true);
        }


        private void showCancelDialog()
        {

            EventHandler<DialogClickEventArgs> yesHandler = new EventHandler<DialogClickEventArgs>(delegate (object sender, DialogClickEventArgs e)
            {
                _download_callbacks.Cancel();
                setDownloadInProgress(false);
                Finish();
            });

            EventHandler<DialogClickEventArgs> noHandler = new EventHandler<DialogClickEventArgs>(delegate (object sender, DialogClickEventArgs e)
            {

            });

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Cancel Downloads?");
            builder.SetPositiveButton("Yes", yesHandler);
            builder.SetNegativeButton("No", noHandler);

            Dialog dialog = builder.Create();
            dialog.Show();
        }

        private void backPressed()
        {

            if (_download_in_progress)
            {
                showCancelDialog();
            }
            else
            {
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



        private class DownloadCallbacks : FTDownloadCallbacks
        {

            public delegate void DownloadCompletedHandler(String ip, bool error);
            public delegate void DownloadProgressHandler(string alias, int progress, string ip);
            public delegate void FolderDownloadProgressHandler(string alias, long progress, string ip);
            public delegate void DownloadStartedHandler();

            private Activity _activity;
            private DownloadProgressHandler _downloadProgressHandler;
            private DownloadCompletedHandler _downloadCompletedHandler;
            private FolderDownloadProgressHandler _folderDownloadCompletedHandler;
            private DownloadStartedHandler _downloadStartedHandler;


            public DownloadCallbacks(Activity activity, DownloadCompletedHandler completedHandler, DownloadProgressHandler progressHandler, FolderDownloadProgressHandler folderProgressHandler
                , DownloadStartedHandler startedHandler)
            {
                _downloadCompletedHandler = completedHandler;
                _downloadProgressHandler = progressHandler;
                _folderDownloadCompletedHandler = folderProgressHandler;
                _downloadStartedHandler = startedHandler;
                _activity = activity;
            }

            public void Cancel()
            {
                InvokeCancelRequested(this);
            }

            public override void DownloadCompleted(string ip, bool error)
            {
                _activity.RunOnUiThread(delegate ()
                {
                    _downloadCompletedHandler(ip, error);
                });
                
            }

            public override void DownloadProgress(string alias, int progress, string ip)
            {
                _activity.RunOnUiThread(delegate ()
                {
                    _downloadProgressHandler(alias, progress, ip);
                });
            }

            public override void DownloadStarted()
            {
                _activity.RunOnUiThread(delegate ()
                {
                    _downloadStartedHandler();
                });
            }

            public override void FolderDownloadProgress(string alias, long progress, string ip)
            {
                _activity.RunOnUiThread(delegate ()
                {
                    _folderDownloadCompletedHandler(alias, progress, ip);
                });
            }
        }

    }
}