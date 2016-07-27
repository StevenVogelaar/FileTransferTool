using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoreLibrary;

namespace FileTransferTool
{
    public partial class MainWindow : Form
    {

        public delegate void FilesSelectedHandler(object obj, FilesSelectedEventArgs e);
        public event FilesSelectedHandler FilesSelected;

        public delegate void FilesUnSelectedHandler(object obj, FilesUnSelectedEventArgs e);
        public event FilesUnSelectedHandler FilesRemoved;

        private DataGridViewFileHandlerManager _sharedGridManager;
        private DataGridViewFileHandlerManager _availableGridManager;

        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += onWindowSizeChange;
            onWindowSizeChange(this, EventArgs.Empty);
            this.MinimumSize = new Size(this.Width, this.Height);
                       
            downloadButton.Hide();
            removeButton.Hide();

            openFileDialog1.Multiselect = true;
        }

        public void InitGrids(Core core)
        {
            _sharedGridManager = new DataGridViewFileHandlerManager(sharedFilesList, core.SharedFiles);
            core.SharedFilesChanged += _sharedGridManager.Core_FilesChanged;
            _availableGridManager = new DataGridViewFileHandlerManager(availableFilesList, core.AvailableFiles);
            core.AvailableFilesChanged += _availableGridManager.Core_FilesChanged;
        }


        private void onWindowSizeChange(object obj, EventArgs e)
        {
            int width = this.Width;
            int height = this.Height;

            availableFilesList.Location = new Point(12, height - availableFilesList.Height - 50);
            availableFilesList.Width = width - 40;

            sharedFilesList.Location = new Point(12, availableFilesList.Location.Y - sharedFilesList.Height - 30);
            sharedFilesList.Width = width - 40;

            sharedLable.Location = new Point(sharedLable.Location.X, sharedFilesList.Location.Y - 20);
            availableLable.Location = new Point(availableLable.Location.X, availableFilesList.Location.Y - 20);
        }
       

        private void addFilesButton_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                FilesSelected.Invoke(this, new FilesSelectedEventArgs(openFileDialog1.FileNames));      
            }
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                FilesSelected.Invoke(this, new FilesSelectedEventArgs(new String[] { folderBrowserDialog1.SelectedPath }));
            }
        }

        private void sharedFilesList_SelectionChanged(object sender, EventArgs e)
        {
            // Check for any selected rows
            foreach (DataGridViewRow row in sharedFilesList.Rows)
            {
                if (row.Selected)
                {
                    removeButton.Show();
                    return;
                }
            }
            removeButton.Hide();
            
        }

        private void availableFilesList_SelectionChanged(object sender, EventArgs e)
        {
            // Check for any selected rows
            foreach (DataGridViewRow row in availableFilesList.Rows)
            {
                if (row.Selected)
                {
                    downloadButton.Show();
                    return;
                }
            }

            downloadButton.Hide();
        }


        public class FilesSelectedEventArgs : EventArgs
        {
            public String[] Files { get; }
            public FilesSelectedEventArgs(String[] files)
            {
                this.Files = files;
            }
        }

        public class FilesUnSelectedEventArgs : EventArgs
        {
            public String[] Files { get; }
            public FilesUnSelectedEventArgs(String[] files)
            {
                this.Files = files;
            }
        }

        
    }
}
