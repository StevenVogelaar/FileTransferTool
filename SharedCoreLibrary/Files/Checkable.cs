using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FileTransferTool.CoreLibrary.Files
{
    interface Checkable
    {
        int ID { get; set; }
        bool Checked { get; set; }
    }
}