using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CoreLibrary
{
    [DataContract]
    class Message
    {
        [DataMember (Name = "RequestBroadcast")]
        public bool RequestBroadcast { get; set; }
        [DataMember (Name = "Msg")]
        public String Msg { get; set; }
        [DataMember (Name = "IPAddress")]
        public String IPAddress { get; set; }
        [DataMember(Name = "SharedFiles")]
        public FTTFileInfo[] SharedFiles { get; set; }

       
        [DataContract]
        [KnownType(typeof(FTTFileInfo))]
        public class FTTFileInfo
        {
            [DataMember (Name = "FileName")]
            public String FileName { get; set; }
            [DataMember (Name = "Size")]
            public String Size { get; set; }
        }

    }
}
