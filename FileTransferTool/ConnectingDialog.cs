using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileTransferTool
{
    public partial class ConnectingDialog : Form
    {
        public ConnectingDialog()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        internal void Invoke(Func<object> p)
        {
            throw new NotImplementedException();
        }
    }
}
