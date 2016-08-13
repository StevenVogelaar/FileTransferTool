using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        private ConnectionManager _connectionManager;


        public Core()
        {
            FTTConsole.Init();
            SharedFiles = new List<FileHandler>();
            AvailableFiles = new List<FileHandler>();
            _connectionManager = new ConnectionManager();
            FTTConsole.Init();
        }


        /// <summary>
        /// Add a local shared file to the list, which will be made avaialable to other clients.
        /// </summary>
        /// <param name="path"></param>
        public void AddSharedFile(String path)
        {

            if (!checkExists(path)) return;

            // Check if a file with the same path already exists in the list.
            foreach (FileHandler f in SharedFiles)
            {
                if (f.Path.Equals(path)) return;
            }

            SharedFiles.Add(new FileHandler(path));
            SharedFilesChanged.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Removes a local shared file from the list available to othe clients.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveSharedFile(String path)
        {
            // Check to see if file with the given path name exists in the list.
            foreach (FileHandler f in SharedFiles)
            {
                if (f.Path == path)
                {
                    SharedFiles.Remove(f);
                    SharedFilesChanged.Invoke(this, EventArgs.Empty);
                    f.Dispose();
                    return;
                }
            }         
        }


        /// <summary>
        /// Broadcasts info request to all other clients to update client list.
        /// </summary>
        public void RefreshClients()
        {
            _connectionManager.RefreshConnections();
            //_connectionManager.Connect();
        }

        /// <summary>
        /// Checks if a file or directory exists at the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool checkExists(String path)
        {
            if (File.Exists(path) || Directory.Exists(path)) return true;

            return false;
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
