using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoreLibrary
{
    public class Core : IDisposable
    {

        public delegate void SharedFilesChangedHandler(object obj, SharedFilesChangedEventArgs e);
        public event SharedFilesChangedHandler SharedFilesChanged;

        public delegate void AvailableFilesChangedHandler(object obj, AvailableFilesChangedEventArgs e);
        public event AvailableFilesChangedHandler AvailableFilesChanged;

        public static SyncList<FileHandler> SharedFiles { get; private set; }
        public static SyncList<FTTFileInfo> AvailableFiles { get; private set; }

        private BroadcastManager _connectionManager;
        private FTConnectionManager _ftConnectionManager;


        public Core()
        {
            FTTConsole.Init();
            SharedFiles = new SyncList<FileHandler>();
            AvailableFiles = new SyncList<FTTFileInfo>();
            _connectionManager = new BroadcastManager();
            _connectionManager.AvailableFilesReceived += connectionManager_AvailableFilesReceived;

            _ftConnectionManager = new FTConnectionManager(GetFilePath);
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

            RefreshClients();
        }

        public void AddSharedFile(String[] paths)
        {
            foreach (String path in paths)
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

            RefreshClients();
        }

        /// <summary>
        /// Removes a local shared file from the list available to other clients.
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
                    break;
                }
            }    
        }


        public void RemoveSharedFile(String[] paths)
        {
            foreach (String path in paths){

                // Check to see if file with the given path name exists in the list.
                foreach (FileHandler f in SharedFiles.CopyOfList())
                {
                    if (f.Path == path)
                    {
                        SharedFiles.Remove(f);
                        SharedFilesChanged.Invoke(this, new SharedFilesChangedEventArgs() { Files = SharedFiles.CopyOfList() });
                        f.Dispose();
                        break;
                    }
                }
            }

            RefreshClients();
        }


        /// <summary>
        /// Broadcasts info request to all other clients to update client list.
        /// </summary>
        public void RefreshClients()
        {
            System.Threading.Thread.Sleep(100);
            _connectionManager.RefreshConnections();
            AvailableFiles.Clear();
            syncAvailableFiles(new FTTFileInfo[0], BroadcastManager.LocalIPAddress().ToString());
        }


        /// <summary>
        /// Will start a file download operation.
        /// </summary>
        /// <param name="files">Key: FileName, Value: Location(ip address)</param>
        public void DownloadFiles(Dictionary<String, String> files) {

            Console.WriteLine("Download button pressed");

            FTTFileInfo[] sharedfiles = AvailableFiles.CopyOfList().ToArray();
            List<FTTFileInfo> foundFiles = new List<FTTFileInfo>();

            // Collect the requested files if they exist.
            foreach (String f in files.Keys)
            {
                for (int i = 0; i < sharedfiles.Length; i++)
                {
                    if (sharedfiles[i].Name.Equals(f))
                    {
                        foundFiles.Add(sharedfiles[i]);
                    }
                }
            }


            // Create download requests for each file.
            foreach (FTTFileInfo f in foundFiles)
            {
                _ftConnectionManager.DownloadFile(f);
            }

        }


        /// <summary>
        /// Returns the path of the given file name.
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <returns>Path of the file (includes the filename at the end).</returns>
        public String GetFilePath(String name)
        {
            List<FileHandler> files = SharedFiles.CopyOfList();

            foreach (FileHandler f in files)
            {
                FTTConsole.AddDebug("Found Filename: " + f.Name);
                Console.WriteLine("Found Filename:" + f.Name);
                if (f.Name.Equals(name))
                {
                    return f.Path;
                }
            }

            FTTConsole.AddDebug("File not found.");

            return null;
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
        private void connectionManager_AvailableFilesReceived(object sender, BroadcastManager.AvailableFilesReceivedEventArgs e)
        {
            syncAvailableFiles(e.Files, e.SourceIP);
        }


        private void syncAvailableFiles(FTTFileInfo[] files, String sourceIP)
        {
            // First remove every file from source IP in the list.
            List<FTTFileInfo> coreFiles = AvailableFiles.CopyOfList();
            List<FTTFileInfo> deleteQueue = new List<FTTFileInfo>();

            foreach (FTTFileInfo cf in coreFiles)
            {
                if (cf.IP.Equals(sourceIP))
                {
                    deleteQueue.Add(cf);
                }
            }

            removeAvailableFiles(deleteQueue);

            // Add files received from IP
            foreach (FTTFileInfo f in files)
            {
                addAvailableFile(f);
            }

            if (AvailableFilesChanged != null)
            {
                AvailableFilesChanged.Invoke(this, new AvailableFilesChangedEventArgs() { Files = AvailableFiles.CopyOfList() });
            }
        }


        /// <summary>
        /// Removes files from the available files list without fireing any events.
        /// </summary>
        /// <param name="files"></param>
        private void removeAvailableFiles(List<FTTFileInfo> files)
        {
            foreach (FTTFileInfo f in files)
            {
                AvailableFiles.Remove(f);
            }
        }

        public void Dispose()
        {
            _connectionManager.Dispose();
        }

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
