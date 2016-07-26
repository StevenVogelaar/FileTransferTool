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

        public String Path { get; set; }
        public String Name { get;}
   

        public FileHandler(String path)
        {
            this.Path = path;
            Name = Path.Split('\\').Last<String>();
        }


        public String size()
        {
            if (File.Exists(Path))
            {
                FileInfo info = new FileInfo(Path);
                return info.Length.ToString();
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
