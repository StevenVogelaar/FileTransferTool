using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;

namespace FileTransferTool
{
    /// <summary>
    /// A class that can be used to receive call backs for file downloads. Sent as part of the DownloadFilesEventArgs
    /// </summary>
    class FileDownloadProgress : FTDownloadCallbacks
    {

        private String _ip;
        private String _name;


        /// <summary>
        /// </summary>
        /// <param name="fileName">Name of file.</param>
        /// <param name="ip">The remote IP address of file.</param>
        public FileDownloadProgress(String fileName, String ip)
        {
            _name = fileName;
            _ip = ip;
        }



        public void DownloadCompleted(FTTFileInfo file)
        {
            throw new NotImplementedException();
        }

        public void DownloadFailed(FTTFileInfo file)
        {
            throw new NotImplementedException();
        }

        void FTDownloadCallbacks.DownloadProgress(FTTFileInfo file)
        {
            throw new NotImplementedException();
        }
    }
}
