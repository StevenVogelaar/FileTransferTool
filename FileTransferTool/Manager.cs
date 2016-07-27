﻿using System;
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
            window.FilesRemoved += MainWindow_FilesUnSelected;

            window.InitGrids(_core);
        }



        /// <summary>
        /// Makes changes to the shared files list in the UI
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void SharedFilesChanged_handler(object obj, EventArgs args)
        {
            
        }


        public void MainWindow_FileSelected(object obj, MainWindow.FilesSelectedEventArgs e)
        {
            foreach (String f in e.Files){
                _core.AddSharedFile(f);
            }
        }

        public void MainWindow_FilesUnSelected(object obj, MainWindow.FilesUnSelectedEventArgs e)
        {
            foreach (String f in e.Files)
            {
                _core.RemoveSharedFile(f);
            }
        }


    }
}