using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CoreLibrary
{
    [DataContract]
    [KnownType(typeof(FTTFileInfo))]
    public class FTTFileInfo
    {
        [DataMember(Name = "FileName")]
        public String Name { get; set; }
        [DataMember(Name = "Size")]
        public String Size { get; set; }
        [DataMember(Name = "IsDirectory")]
        public bool IsDirectory { get; set; }

        public String Path { get; set; }
        public String IP { get; set; }


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
                result[i] = new FTTFileInfo() { Name = f.Name, Size = f.Size, Path = f.Path , IP = ip, IsDirectory = f.IsDirectory};
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
