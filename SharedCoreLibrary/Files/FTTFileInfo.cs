using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FileTransferTool.CoreLibrary.Files
{
    [DataContract]
    [KnownType(typeof(FTTFileInfo))]
    public class FTTFileInfo
    {
        [DataMember(Name = "FileName"), Browsable(false)]
        public String Name { get; set; }
        

        [DataMember(Name = "IsDirectory"), Browsable(false)]
        public bool IsDirectory { get; set; }
        [DataMember(Name = "Alias")]
        public String Alias { get; set; }
        public String IP { get; set; }
        [DataMember(Name = "Size")]
        public String Size { get; set; }

        [Browsable(false)]
        public String Path { get; set; }
       


        /// <summary>
        /// Converts a List of FileHandlers to an array of FTTFileInfo.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static FTTFileInfo[] ConvertFileHandler(List<FileHandler> files, String ip)
        {

            FTTFileInfo[] result = new FTTFileInfo[files.Count];
            int i = 0;

            foreach (FileHandler f in files)
            {
                result[i] = new FTTFileInfo() { Alias = f.Alias, Name = f.Name, Size = f.Size, Path = f.Path , IP = ip, IsDirectory = f.IsDirectory};
                i++;
            }

            return result;
        }

        public override string ToString()
        {
            return "Name: " + Name + " IP: " + IP;
        }


        public override bool Equals(object obj)
        {

            if (obj is FTTFileInfo)
            {
                FTTFileInfo file = (FTTFileInfo)obj;

                if (file.Name.Equals(Name) && file.IP.Equals(IP)) return true;
                else return false;
            }
            else return false;
        }

    }
}
