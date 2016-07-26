using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;

namespace FileTransferTool
{

    public class Manager
    {

        MainWindow window;
        Core core;


        public Manager(MainWindow window)
        {
            this.window = window;
            

            core = new Core();
        }

        


    }
}
