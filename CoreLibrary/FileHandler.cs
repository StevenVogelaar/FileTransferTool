using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoreLibrary
{
    public class FileHandler
    {

        public String Path { get;}
        public String Name { get;}
   

        public FileHandler(String path)
        {
            this.Path = path;
            Name = Path.Split('\\').Last<String>();
        }


        public String Size()
        {
            if (File.Exists(Path))
            {
                FileInfo info = new FileInfo(Path);
                long sizeInBytes = info.Length;

                if (sizeInBytes < 1048576)
                {
                    return (sizeInBytes / 1024f).ToString("0.00") + " kb";
                }
                else return (info.Length / 1048576f).ToString("0.00") + " mb";
            }
            else return "Null";
        }


        public override bool Equals(object obj)
        {
            FileHandler otherFile = (FileHandler)obj;
            if (otherFile.Path == Path) return true;
            else return false;
        }

        
    }
}
