using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTransferTool.CoreLibrary.Files
{
    class CheckableFileHandler : FileHandler, Checkable
    {

        public static int IDCount { get; set; }


        public int ID { get; set; }
        public bool Checked { get; set; }
        public FileHandler OrgFileHandler { get; set; }

        public CheckableFileHandler(FileHandler file) : base(file)
        {
            OrgFileHandler = file;

            ID = IDCount;
            IDCount++;
        }

    }
}