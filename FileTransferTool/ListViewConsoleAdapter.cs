using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;


namespace FileTransferTool
{
    class ListViewConsoleAdapter
    {

        private ListView _listView;
        delegate void AddMessageCallback(String message);

        public ListViewConsoleAdapter(ListView listView)
        {

            _listView = listView;
        }


        public void ConsoleMessaged (CoreLibrary.FTTConsole.ConsoleMessageEventArgs e)
        {
            try
            {
                AddMessageCallback d = new AddMessageCallback(AddMessage);
                _listView.Invoke(d, new object[] { e.Message.Type.ToString() + ": " + e.Message.Msg });
            }
            catch (Exception f)
            {

            }
        }


        private void AddMessage(String message)
        {
            _listView.Items.Add(message);
            _listView.Items[_listView.Items.Count - 1].EnsureVisible();
        }


    }
}
