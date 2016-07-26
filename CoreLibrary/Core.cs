using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public class Core
    {

        public delegate void SharedFilesChangedHandler(object obj, SharedListChangedEventArgs e);
        public event SharedFilesChangedHandler sharedFilesChanged;

        List<FileHandler> files;

        public Core()
        {
            files = new List<FileHandler>();
        }

        public void addSharedFile(String path)
        {
            // Check if a file with the same path already exists in the list.
            foreach (FileHandler f in files)
            {
                if (f.Path.Equals(path)) return;
            }
        }

        public void removeSharedFile(String path)
        {
            // Check to see if file with the given path name exists in the list.
            foreach (FileHandler f in files)
            {
                if (f.Path == path)
                {
                    files.Remove(f);
                    sharedFilesChanged.Invoke(this, new SharedListChangedEventArgs(SharedListChangedEventArgs.ChangeType.removed, f));
                    return;
                }
            }         
        }

        public void addSharedFolder()
        {

        }

        public void removeSharedFolder()
        {

        }


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
    }

}
