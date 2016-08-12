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
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public String Msg { get; set; }
        [DataMember]
        public String IPAddress { get; set; }

        public Message()
        {
        }

    }
}
