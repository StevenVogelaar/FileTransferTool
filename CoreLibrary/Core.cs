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

        public delegate void SharedFilesChangedHandler(object obj, SharedFilesChangedEventArgs e);
        public event SharedFilesChangedHandler SharedFilesChanged;

        public delegate void AvailableFilesChangedHandler(object obj, AvailableFilesChangedEventArgs e);
        public event AvailableFilesChangedHandler AvailableFilesChanged;

        public static SyncList<FileHandler> SharedFiles { get; private set; }
        public static SyncList<FTTFileInfo> AvailableFiles { get; private set; }

        private ConnectionManager _connectionManager;


        public Core()
        {
            FTTConsole.Init();
            SharedFiles = new SyncList<FileHandler>();
            AvailableFiles = new SyncList<FTTFileInfo>();
            _connectionManager = new ConnectionManager();
            _connectionManager.AvailableFilesReceived += connectionManager_AvailableFilesReceived;
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
            foreach (FileHandler f in SharedFiles.CopyOfList())
            {
                if (f.Path.Equals(path)) return;
            }

            SharedFiles.Add(new FileHandler(path));
            if (SharedFilesChanged != null)
            {
                SharedFilesChanged.Invoke(this, new SharedFilesChangedEventArgs() { Files = SharedFiles.CopyOfList() });
            }
        }

        /// <summary>
        /// Removes a local shared file from the list available to othe clients.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveSharedFile(String path)
        {
            // Check to see if file with the given path name exists in the list.
            foreach (FileHandler f in SharedFiles.CopyOfList())
            {
                if (f.Path == path)
                {
                    SharedFiles.Remove(f);
                    SharedFilesChanged.Invoke(this, new SharedFilesChangedEventArgs() { Files = SharedFiles.CopyOfList() });
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


        /// <summary>
        /// Add a remote file to the available file list.
        /// </summary>
        /// <param name="file"></param>
        private void addAvailableFile(FTTFileInfo file)
        {

            // Check if a file with the same name and IP already exists in the list.
            List<FTTFileInfo> fileList = AvailableFiles.CopyOfList();
            foreach (FTTFileInfo f in fileList)
            {
                //Console.WriteLine("f.name: " + f.Name + " file.Name: " + file.Name + " f.IP: " + f.IP + " file.IP: " + file.IP);

                if (f.Name.Equals(file.Name) && f.IP.Equals(file.IP)) return;
            }

            AvailableFiles.Add(file);
        }

        /// <summary>
        /// Calls the addAvialableFile() method with each file received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectionManager_AvailableFilesReceived(object sender, ConnectionManager.AvailableFilesReceivedEventArgs e)
        {
            syncAvailableFiles(e.Files, e.SourceIP);

            if (AvailableFilesChanged != null)
            {
                AvailableFilesChanged.Invoke(this, new AvailableFilesChangedEventArgs() { Files = AvailableFiles.CopyOfList() });
            }
        }


        private void syncAvailableFiles(FTTFileInfo[] files, String sourceIP)
        {

            foreach (FTTFileInfo f in files)
            {
                addAvailableFile(f);
            }

            List<FTTFileInfo> coreFiles = AvailableFiles.CopyOfList();
            List<FTTFileInfo> deleteQueue = new List<FTTFileInfo>();

            foreach (FTTFileInfo cf in coreFiles)
            {
                bool found = false;

                foreach (FTTFileInfo f in files)
                {
                    if (cf.Name.Equals(f.Name) && cf.IP == f.IP)
                    {
                        found = true;
                        break;
                    } 
                }

                if (!found)
                {
                    deleteQueue.Add(cf);
                }
            }

            // Delete files in queue.
            foreach (FTTFileInfo f in deleteQueue)
            {
                AvailableFiles.Remove(f);
            }
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


        public class SharedFilesChangedEventArgs : EventArgs
        {
            public List<FileHandler> Files { get; set; }
        }

        public class AvailableFilesChangedEventArgs : EventArgs
        {
            public List<FTTFileInfo> Files { get; set; }
        }

    }
}
