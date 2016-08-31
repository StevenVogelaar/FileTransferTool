using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    public interface FTDownloadCallbacks
    {


        void DownloadFailed(FTTFileInfo file);
        void DownloadProgress(FTTFileInfo file);
        void DownloadCompleted(FTTFileInfo file);

    }
}
