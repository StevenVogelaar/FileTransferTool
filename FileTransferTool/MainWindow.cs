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
    public partial class MainWindow : Form
    {

       

        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += onWindowSizeChange;
            onWindowSizeChange(this, EventArgs.Empty);
            this.MinimumSize = new Size(this.Width, this.Height);

            // Set up shared files list
            sharedFilesList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
          



            downloadButton.Hide();
            removeButton.Hide();

            availableFilesList.CheckOnClick = true;
            openFileDialog1.Multiselect = true;
        }


        void onWindowSizeChange(object obj, EventArgs e)
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

        private void availableFilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (availableFilesList.CheckedIndices.Count > 0) downloadButton.Show();
            else downloadButton.Hide();
        }

       

        private void addFilesButton_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                sharedFilesChanged.Invoke(this, new SharedListChangedEventArgs(
                    SharedListChangedEventArgs.ChangeType.added, openFileDialog1.FileNames));
                addToSharedList(openFileDialog1.FileNames);       
            }
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                sharedFilesChanged.Invoke(this, new SharedListChangedEventArgs(
                    SharedListChangedEventArgs.ChangeType.removed, new String[] { folderBrowserDialog1.SelectedPath }));
                addToSharedList(new String[] { folderBrowserDialog1.SelectedPath });
            }
        }

        private void sharedFilesList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("Clicked!");
        }

        /// <summary>
        /// Handles adding elements to shared dictionary and shared checkedbox.
        /// </summary>
        /// <param name="files"></param>
        private void addToSharedList(String[] files)
        {
            
        }

        /// <summary>
        /// Handles adding elements to available dictionary and available checkedbox.
        /// </summary>
        /// <param name="files"></param>
        private void addToAvailableList(String[] files)
        {

        }

    }
}
