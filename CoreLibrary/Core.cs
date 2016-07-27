﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class Core
    {

        public delegate void SharedFilesChangedHandler(object obj, EventArgs e);
        public event SharedFilesChangedHandler SharedFilesChanged;

        public delegate void AvailableFilesChangedHandler(object obj, EventArgs e);
        public event AvailableFilesChangedHandler AvailableFilesChanged;

        public List<FileHandler> SharedFiles { get; }
        public List<FileHandler> AvailableFiles { get; }


        public Core()
        {
            SharedFiles = new List<FileHandler>();
            AvailableFiles = new List<FileHandler>();
        }

        public void AddSharedFile(String path)
        {
            // Check if a file with the same path already exists in the list.
            foreach (FileHandler f in SharedFiles)
            {
                if (f.Path.Equals(path)) return;
            }

            SharedFiles.Add(new FileHandler(path));
            SharedFilesChanged.Invoke(this, EventArgs.Empty);
        }

        public void RemoveSharedFile(String path)
        {
            // Check to see if file with the given path name exists in the list.
            foreach (FileHandler f in SharedFiles)
            {
                if (f.Path == path)
                {
                    SharedFiles.Remove(f);
                    SharedFilesChanged.Invoke(this, EventArgs.Empty);
                    return;
                }
            }         
        }

        private void addAvailableFile(String path)
        {
            
        }

        private void removeAvailableFile(String path)
        {
            
        }


        /**
        public class SharedListChangedEventArgs : EventArgs
        {
            public enum ChangeType { added, removed };

            public ChangeType Change { get; }
            public FileHandler File { get; }

            public SharedListChangedEventArgs(ChangeType changeType, FileHandler file)
            {
                this.File = file;
                Change = changeType;
            }  
        }
        */


    }
}