using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CoreLibrary
{
    interface Checkable
    {
        int ID { get; set; }
        bool Checked { get; set; }
    }
}