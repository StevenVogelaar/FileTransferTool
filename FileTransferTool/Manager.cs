using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;

namespace FileTransferTool
{

    public class Manager
    {

        private MainWindow _window;
        private Core _core;
 


        public Manager(MainWindow window)
        {
            _window = window;
            _core = new Core();

            _core.SharedFilesChanged += new Core.SharedFilesChangedHandler(SharedFilesChanged_handler);
            window.FilesSelected += MainWindow_FileSelected;
            window.FilesRemoved += MainWindow_FilesRemoved;
            window.RefreshClients += MainWindow_RefreshClients;
            window.FormClosing += window_closing;
            window.DownloadFiles += MainWindow_DownloadFiles;

            window.Init(_core);
        }


        private void window_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _core.Dispose();
        }

        /// <summary>
        /// Makes changes to the shared files list in the UI
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void SharedFilesChanged_handler(object obj, EventArgs args)
        {
            
        }

        public void MainWindow_RefreshClients(object obj, EventArgs args)
        {
            _core.RefreshClients();
        }


        public void MainWindow_FileSelected(object obj, MainWindow.FilesSelectedEventArgs e)
        {
            _core.AddSharedFile(e.Files);
        }

        public void MainWindow_FilesRemoved(object obj, MainWindow.FilesRemovedEventArgs e)
        {
            _core.RemoveSharedFile(e.Files);
        }

        public void MainWindow_DownloadFiles(object obj, MainWindow.DownloadFilesEventArgs e)
        {

        }


    }
}
