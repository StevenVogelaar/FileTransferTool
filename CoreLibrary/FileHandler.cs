using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace CoreLibrary
{
    public class FileHandler
    {

        public String Path { get; }
        public String Name { get; }
        public String Size { get; private set; }
        //public long SizeInBytes { get; private set; }


        public delegate void FileInfoChangedHandler(object sender, FileInfoChangedEventArgs e);
        public event FileInfoChangedHandler FileInfoChanged;


        private BackgroundWorker sizeCheckWorker;


        public FileHandler(String path)
        {
            this.Path = path;
            Name = Path.Split('\\').Last<String>();
            Size = "Calculating...";


            sizeCheckWorker = new BackgroundWorker();
            sizeCheckWorker.WorkerSupportsCancellation = true;
            sizeCheckWorker.DoWork += size;
            sizeCheckWorker.RunWorkerCompleted += sizeCheckWorker_RunWorkerCompleted;
            sizeCheckWorker.RunWorkerAsync(Path);
        }


        /// <summary>
        /// Calculates the size of the file or folder and sets SizeInBytes to the raw bytes.
        /// </summary>
        /// <returns></returns>
        private void size(object sender, DoWorkEventArgs e)
        {
            String argPath = (String)e.Argument;

            if (File.Exists(argPath))
            {
                FileInfo info = new FileInfo(argPath);
                e.Result = parseBytes(info.Length);
            }
            else if (Directory.Exists(Path))
            {
                long _sizeInBytes = getDirectorySize(argPath);

                if (_sizeInBytes == -1)
                {
                    e.Cancel = true;
                    return;
                }

                e.Result = parseBytes(_sizeInBytes);
            }

            
        }


        /// <summary>
        /// Converts bytes to readable form (x, Bytes, x KiB, x MiB, x GiB)
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string parseBytes(long bytes)
        {
            String result;

            if (bytes < 1024)
            {
                result = bytes.ToString("0.00") + " Bytes"; 
            }
            else if (bytes < 1048576)
            {
                result = (bytes / 1024f).ToString("0.00") + " KiB";
            }
            else if (bytes < 1073741824)
            {
                result = (bytes / 1048576f).ToString("0.00") + " MiB";
            }
            else
            {
                result = (bytes / 1073741824f).ToString("0.00") + " GiB";
            }

            return result;
        }



        /// <summary>
        /// Calculates the size of all contents of a directory (including sub directories).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private long getDirectorySize(String path)
        {

            if (sizeCheckWorker.CancellationPending) return -1;

            long size = 0;
            
            // Recurse into sub directories
            foreach (String d in Directory.GetDirectories(path)){
                size += getDirectorySize(d);
            }

            // Count size of each file
            foreach (String f in Directory.GetFiles(path))
            {
                FileInfo info = new FileInfo(f);
                size += info.Length;
            }

            return size;
        }


        public override bool Equals(object obj)
        {
            FileHandler otherFile = (FileHandler)obj;
            if (otherFile.Path == Path) return true;
            else return false;
        }


        public class FileInfoChangedEventArgs
        {
            public String Name { get; set; }
            public String Size { get; set; }
            public String Path { get; set; }
        }


        /// <summary>
        /// Called after the size of the file or folder has been calculated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sizeCheckWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true || e.Error != null)
            {
                Size = "Error";
            }
            else
            {
                Size = (String)e.Result;

                if (FileInfoChanged != null)
                {
                    FileInfoChanged.Invoke(this, new FileInfoChangedEventArgs { Name = this.Name, Path = this.Path, Size = this.Size });
                }
            }
        }


        /// <summary>
        /// Cancels background operations.
        /// </summary>
        public void Dispose()
        {
            sizeCheckWorker.CancelAsync();
        }
    }
}
