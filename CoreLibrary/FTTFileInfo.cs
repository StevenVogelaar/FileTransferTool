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
                result[i] = new FTTFileInfo() { Name = f.Name, Size = f.Size, Path = f.Path , IP = ip};
                i++;
            }

            return result;
        }


    }
}
