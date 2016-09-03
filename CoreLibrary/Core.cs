using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace CoreLibrary
{
    public class Core : IDisposable
    {

        public static SyncList<FileHandler> SharedFiles { get; private set; }
        public static SyncList<FTTFileInfo> AvailableFiles { get; private set; }

        private FTUI _ui;
        private BroadcastManager _broadcastManager;
        private FTConnectionManager _ftConnectionManager;


        public Core(FTUI ui)
        {
            _ui = ui;
            _ui.WindowClosing += _ui_WindowClosing;
            _ui.FilesRemoved += _ui_FilesRemoved;
            _ui.FilesSelected += _ui_FilesSelected;
            _ui.RefreshClients += _ui_RefreshClients;
            _ui.DownloadRequest += _ui_DownloadRequest;
            _ui.DownloadCancel += _ui_DownloadCancel;
            _ui.Exit += _ui_Exit;


            FTTConsole.Init();
            SharedFiles = new SyncList<FileHandler>();
            AvailableFiles = new SyncList<FTTFileInfo>();
            _broadcastManager = new BroadcastManager();
            _ftConnectionManager = new FTConnectionManager(GetFilePath);

            _broadcastManager.AvailableFilesReceived += connectionManager_AvailableFilesReceived;
            
        }


        /// <summary>
        /// Add a list of local shared files to internal list. These are the files that will be available to other clients.
        /// </summary>
        /// <param name="paths"></param>
        public void AddSharedFile(String[] paths)
        {
            foreach (String path in paths)
            {
                if (!checkExists(path)) return;

                List<FileHandler> sharedFiles = SharedFiles.CopyOfList();
                bool duplicateName = false;
                String fileName = path.Substring(path.LastIndexOf("\\") + 1);

                // Check if a file with the same path already exists in the list.
                foreach (FileHandler f in sharedFiles)
                {
                    duplicateName = false;
                    if (f.Path.Equals(path)) return;

                    // Now check for duplicated names. If a duplicate name is found, set a alias for the file.
                    String fName = f.Name;
                    if (f.Name.Equals(fileName))
                    {
                        duplicateName = true;
                        break;
                    }
                }

                if (duplicateName)
                {
                    String alias = fileName;
                    // Check if aliases are allready taken
                    foreach (FileHandler f in sharedFiles)
                    {
                        for (int i = 1; i != -1; i++)
                        {
                            if (!f.Alias.Equals(fileName + " (" + i + ")"))
                            {
                                // Assign alias to file.
                                alias = fileName + " (" + i + ")";
                                SharedFiles.Add(new FileHandler(path, alias));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    SharedFiles.Add(new FileHandler(path));
                }

                _ui.SharedFilesChanged(SharedFiles.CopyOfList());
            }

            RefreshClientsAsync();
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
                    _ui.SharedFilesChanged(SharedFiles.CopyOfList());
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
                        _ui.SharedFilesChanged(SharedFiles.CopyOfList());
                        f.Dispose();
                        break;
                    }
                }
            }

            RefreshClientsAsync();
        }


        /// <summary>
        /// Broadcasts info request to all other clients to update client list.
        /// </summary>
        public void RefreshClientsAsync()
        {
            Thread thread = new Thread(refreshClients);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Internal refreshClients method that is run on another thread.
        /// </summary>
        private void refreshClients()
        {
            _broadcastManager.RefreshConnections();
            AvailableFiles.Clear();
            syncAvailableFiles(new FTTFileInfo[0], BroadcastManager.LocalIPAddress().ToString());
        }


        /// <summary>
        /// Will start a file download operation.
        /// </summary>
        /// <param name="files">Key: FileName, Value: Location(ip address)</param>
        public void DownloadFiles(Dictionary<String, String> files, String dest, FTDownloadCallbacks callbacks) {

            FTTFileInfo[] availablefiles = AvailableFiles.CopyOfList().ToArray();
            List<FTTFileInfo> foundFiles = new List<FTTFileInfo>();

            // Collect the requested files if they exist.
            foreach (String f in files.Keys)
            {
                for (int i = 0; i < availablefiles.Length; i++)
                {
                    String value;
                    bool foundValue = files.TryGetValue(f, out value);

                    if (foundValue && availablefiles[i].Alias.Equals(f) && availablefiles[i].IP.Equals(value))
                    {
                        foundFiles.Add(availablefiles[i]);
                    }
                }
            }


            // Sort by IP into individual file lists.
            HashSet<String> uniqueIPs = new HashSet<string>();
            foreach (FTTFileInfo f in foundFiles)
            {
                uniqueIPs.Add(f.IP);
            }

            // Generate index values for each ip address.
            String[] ipIndexes = uniqueIPs.ToArray();

            List<FTTFileInfo>[] sortedFiles = new List<FTTFileInfo>[uniqueIPs.Count];
            for (int i = 0; i < sortedFiles.Length; i++)
            {
                sortedFiles[i] = new List<FTTFileInfo>();
            }

            foreach (FTTFileInfo f in foundFiles)
            {
                for (int i = 0; i < ipIndexes.Length; i++)
                {
                    if (ipIndexes[i].Equals(f.IP))
                    {
                        sortedFiles[i].Add(f);
                    }
                }
            }


            // Start download request for each file set.
            foreach (List<FTTFileInfo> f in sortedFiles)
            {
                _ftConnectionManager.DownloadFile(f, dest, callbacks);
            }
            

        }


        /// <summary>
        /// Returns the path of the given file name.
        /// </summary>
        /// <param name="alias">alias of the file.</param>
        /// <returns>Path of the file (includes the filename at the end).</returns>
        public String GetFilePath(String alias)
        {
            List<FileHandler> files = SharedFiles.CopyOfList();

            foreach (FileHandler f in files)
            {
                
                if (f.Alias.Equals(alias))
                {
                    FTTConsole.AddDebug("Found Filename: " + f.Name);
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

                if (f.Alias.Equals(file.Alias) && f.IP.Equals(file.IP)) return;
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

            _ui.AvailableFilesChanged(AvailableFiles.CopyOfList());
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

       

        /// <summary>
        /// Handles download requests from the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ui_DownloadRequest(object sender, FTUI.DownloadRequestEventArgs e)
        {
            if (e.Dest != null && e.Files != null) DownloadFiles(e.Files, e.Dest, e.CallBacks);
        }

        /// <summary>
        /// Handles client refresh requests from the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ui_RefreshClients(object sender, EventArgs e)
        {
            RefreshClientsAsync();
        }

        /// <summary>
        /// Handles file selection event from the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ui_FilesSelected(object sender, FTUI.FilesSelectedEventArgs e)
        {
            if (e.Files != null)
            {
                AddSharedFile(e.Files);
            }
        }

        /// <summary>
        /// Handles file removed event from the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ui_FilesRemoved(object sender, FTUI.FilesRemovedEventArgs e)
        {
            if (e.Files != null)
            {
                RemoveSharedFile(e.Files);
            }
        }

        /// <summary>
        /// Checks for current download/upload operations and sets the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ui_WindowClosing(object sender, FTUI.WindowClosingEventArgs e)
        {
            if (_ftConnectionManager.CurrentUploadOperations())
            {
                e.CancelClosing = true;
            }
        }

        public void Dispose()
        {
            _broadcastManager.Dispose();
            _ftConnectionManager.Dispose();
        }

        private void _ui_Exit(object sender, EventArgs e)
        {
            Dispose();
        }

        private void _ui_DownloadCancel(object sender, EventArgs e)
        {
            _ftConnectionManager.CancelDownloads();
        }


    }
}
