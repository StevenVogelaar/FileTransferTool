using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CoreLibrary
{
    class CheckableFileInfo : FTTFileInfo, Checkable
    {

        public static int IDCount { get; set; }


        public int ID { get; set; }
        public bool Checked { get; set; }


        public CheckableFileInfo(FTTFileInfo fileInfo)
        {

            ID = IDCount;
            IDCount++;

            Alias = fileInfo.Alias;
            IsDirectory = fileInfo.IsDirectory;
            Name = fileInfo.Name;
            Path = fileInfo.Path;
            Size = fileInfo.Size;
            IP = fileInfo.IP;
        }
    }
}