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
using System.IO;

namespace FileTransferToolAndroid
{

    [Activity(Label = "FileBrowserActivity", MainLauncher = false, Icon = "@drawable/icon72", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FileBrowserActivity : Activity
    {

        public const int SELECT_FOLDER = 0;
        public const int SELECT_FILES = 1;
        public const int SELECT_FOLDER_FOR_SHARE = 2;
        public const string FOLDER_SELECT_RESULT = "FOLDER_RESULT";
        public const string FILE_SELECT_RESULT = "FILE_RESULT";
        public const string OPERATION_TYPE = "OPERATION_TYPE";

        private static string _currently_selected_path = "";
        private static Stack<String> _history;


        private FileBrowserArrayAdapter _arrayAdapter;
        private ListView _listView;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private TextView _directoryPath;
        private List<FileBrowserFileInfo> _files;
        private Boolean select_folder = false;
        private string root = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.FileBrowser);
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _directoryPath = _toolbar.FindViewById<TextView>(Resource.Id.directory_path);
            _listView = FindViewById<ListView>(Resource.Id.FileBrowserListView);
            _files = new List<FileBrowserFileInfo>();
            _history = new Stack<string>();

            Button button = FindViewById<Button>(Resource.Id.select_folder_button);
            button.Click += _button_click;

            if (Intent.GetIntExtra(OPERATION_TYPE, 0) == SELECT_FOLDER || (Intent.GetIntExtra(OPERATION_TYPE, 0) == SELECT_FOLDER_FOR_SHARE))
            {
                select_folder = true;
                button.SetText("Select Folder", TextView.BufferType.Normal);
            }
            else if (Intent.GetIntExtra(OPERATION_TYPE, 0) == SELECT_FILES)
            {
                select_folder = false;
                button.SetText("Select File(s)", TextView.BufferType.Normal);
            }


            if (_currently_selected_path == "/" || _currently_selected_path == "") ;
            {
                _currently_selected_path = root;
            }

            changeDirectory(_currently_selected_path);

            _listView.ItemClick += _listView_ItemClick;
        }



        private void _button_click(object sender, EventArgs e)
        {
            Intent result = new Intent();

            if (select_folder)
            {
                result.PutExtra(FOLDER_SELECT_RESULT, _currently_selected_path);
            }
            else
            {


                List<string> selectedFiles = new List<string>();

                List<FileBrowserFileInfo> temp = _arrayAdapter.GetChecked();

                foreach (FileBrowserFileInfo f in _arrayAdapter.GetChecked())
                {
                    selectedFiles.Add(f.Path + "/" + f.Name);
                }
                

                result.PutExtra(FILE_SELECT_RESULT, selectedFiles.ToArray());
            }

            SetResult(Result.Ok, result);
            Finish();
        }

        private void _listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            FileBrowserFileInfo fileInfo = _files.ElementAt(e.Position);

            if (fileInfo.IsDirectory)
            {
                _history.Push(_currently_selected_path);
                changeDirectory(fileInfo.Path);
            }
        }

        private void changeDirectory(string path)
        {
            DirectoryInfo[] subDirectories = null;

            
            _currently_selected_path = path;

            _directoryPath.SetText(_currently_selected_path, TextView.BufferType.Normal);

            try
            {
                DirectoryInfo directory = new DirectoryInfo(_currently_selected_path);
                subDirectories = directory.GetDirectories();
            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Error reading files.", ToastLength.Long).Show();
                return;
            }

            _files.Clear();

            for (int i = 0; i < subDirectories.Length; i++)
            {
                DirectoryInfo directory = subDirectories[i];
                _files.Add(new FileBrowserFileInfo() {Name = directory.Name, IsDirectory = true, Path = directory.FullName});
            }

            if (_arrayAdapter == null)
            {
                _arrayAdapter = new FileBrowserArrayAdapter(this, _files);
                _listView.Adapter = _arrayAdapter;
                _arrayAdapter.SetListView(_listView);
            }


            // Code for files if file selection is desired
            if (!select_folder)
            {
                DirectoryInfo directory = new DirectoryInfo(_currently_selected_path);
                FileInfo[] files = directory.GetFiles();
                

                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = files[i];
                    FileBrowserFileInfo fileBrowserInfo = new FileBrowserFileInfo() {Name = fileInfo.Name, IsDirectory = false,
                        Path = fileInfo.Directory.FullName, Size = CoreLibrary.FileHandler.ParseBytes(fileInfo.Length) };
                    _files.Add(fileBrowserInfo);
                }
            }

            _arrayAdapter.NotifyDataSetChanged();

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


        private void backPressed()
        {
            try
            {
                String path = _history.Pop();
                changeDirectory(path);
            }
            catch (Exception e)
            {

                if (_currently_selected_path != "/")
                {
                    String temp = _currently_selected_path.Substring(0, _currently_selected_path.LastIndexOf('/'));

                    if (!temp.Contains('/')) temp = "/";
                    changeDirectory(temp);
                }
                else
                {
                    Finish();
                }

                return;
            }

        }

    }
}