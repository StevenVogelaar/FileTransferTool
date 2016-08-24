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

        public static int selectIndex= 0;
        public static int nameIndex = 1;
        public static int locationIndex = 2;
        public static int sizeIndex = 3;

        public delegate void FilesSelectedHandler(object sender, FilesSelectedEventArgs e);
        public event FilesSelectedHandler FilesSelected;

        public delegate void FilesRemovedHandler(object sender, FilesRemovedEventArgs e);
        public event FilesRemovedHandler FilesRemoved;

        public delegate void RefreshClientsHandler(object sender, EventArgs e);
        public event RefreshClientsHandler RefreshClients;

        public delegate void DownloadFilesHandler(object sender, DownloadFilesEventArgs e);
        public event DownloadFilesHandler DownloadFiles;

        private DataGridViewFileHandlerAdapter _sharedGridManager;
        private DataGridViewFTTFileInfoAdapter _availableGridManager;
        private ListViewConsoleAdapter _listViewConsoleAdapter;

        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += onWindowSizeChange;
            this.MinimumSize = new Size(this.Width, this.Height);
                       
            downloadButton.Hide();
            removeButton.Hide();

            openFileDialog1.Multiselect = true;

            // Init datagrid settings
            this.availableFilesList.CellValueChanged += availableFileList_OnCellValueChanged;
            this.availableFilesList.CellMouseUp += availablesharedfileList_OnCellMouseUp;
            this.sharedFilesList.CellValueChanged += sharedFileList_OnCellValueChanged;
            this.sharedFilesList.CellMouseUp += sharedfileList_OnCellMouseUp;
            this.Load += onLoad;
            _listViewConsoleAdapter = new ListViewConsoleAdapter(MessageConsole);


            
            this.BackColor = Color.LightGray;

            onWindowSizeChange(this, EventArgs.Empty);
        }



        private void onLoad(object sender, EventArgs e)
        {
            FTTConsole.ConsoleMessage += _listViewConsoleAdapter.ConsoleMessaged;
            if (RefreshClients != null)
            {
                RefreshClients.Invoke(this, EventArgs.Empty);
            }
        }

     
        /// <summary>
        /// Initializes grids with their grid managers.
        /// </summary>
        /// <param name="core"></param>
        public void Init(Core core)
        {
            _sharedGridManager = new DataGridViewFileHandlerAdapter(sharedFilesList, Core.SharedFiles.CopyOfList());
            core.SharedFilesChanged += _sharedGridManager.Core_FilesChanged;
            _availableGridManager = new DataGridViewFTTFileInfoAdapter(availableFilesList, Core.AvailableFiles.CopyOfList());
            core.AvailableFilesChanged += _availableGridManager.Core_FilesChanged;
        }

        /// <summary>
        /// Handles resizing views when the window size changes.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void onWindowSizeChange(object obj, EventArgs e)
        {
            int width = this.Width;
            int height = this.Height;

            // Adjust available files list size and location.
            availableFilesList.Height = (height - 206) / 2;
            availableFilesList.Location = new Point(12, height - availableFilesList.Height - 150);
            availableFilesList.Width = width - 40;

            // Adjust shared files list size and location.
            sharedFilesList.Location = new Point(12, sharedFilesList.Location.Y);
            sharedFilesList.Width = width - 40;
            sharedFilesList.Height = (availableFilesList.Location.Y - sharedFilesList.Location.Y) - 30;

            // Move labes to match new list locations.
            sharedLable.Location = new Point(sharedLable.Location.X, sharedFilesList.Location.Y - 20);
            availableLable.Location = new Point(availableLable.Location.X, availableFilesList.Location.Y - 20);

            // Auto size the size column to fit new list width.
            SharedSizeColumn.Width = sharedFilesList.Width - (SharedCheckColumn.Width + SharedNameColumn.Width + SharedLocationColumn.Width) - 3;
            AvailSizeColumn.Width = availableFilesList.Width - (AvailCheckColumn.Width + AvailNameColumn.Width + AvailLocationColumn.Width) - 3;

            // Change devider width to match window.
            this.panel1.Width = width;
            this.panel2.Width = width;

            // Adjust console width.
            MessageConsole.Width = width - 40;
            MessageConsole.Location = new Point(12, height - 125);
            MessageColumn.Width = MessageConsole.Width - 25;
        }


       
        /// <summary>
        /// Checks if user has selected files. Fires a FilesSelected event if true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilesButton_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK && FilesSelected != null)
            {
                FilesSelected.Invoke(this, new FilesSelectedEventArgs(openFileDialog1.FileNames));      
            }
        }

        /// <summary>
        /// Checks if user selected a folder. Fires a FilesSelected event if true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFolderButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK && FilesSelected != null)
            {
                FilesSelected.Invoke(this, new FilesSelectedEventArgs(new String[] { folderBrowserDialog1.SelectedPath }));
            }
        }

        /// <summary>
        /// Checks if changed cell is from the shared checked column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sharedFileList_OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == SharedCheckColumn.Index && e.RowIndex != -1)
            {
                checkSharedChecks();
            }
        }


        /// <summary>
        /// Prevents double clicks on checkboxes from not registering.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sharedfileList_OnCellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (e.ColumnIndex == SharedCheckColumn.Index && e.RowIndex != -1)
            {
                sharedFilesList.EndEdit();
            }
        }

        /// <summary>
        /// Checks if changed cell is from the available checked column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void availableFileList_OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == AvailCheckColumn.Index && e.RowIndex != -1)
            {
                checkAvailableChecks();
            }
        }

        /// <summary>
        /// Checks for checked cells in shared checked column.
        /// </summary>
        private void checkSharedChecks()
        {
            foreach (DataGridViewRow row in sharedFilesList.Rows)
            {
                if (Convert.ToBoolean(row.Cells[selectIndex].Value) == true)
                {
                    removeButton.Show();
                    return;
                }
            }
            removeButton.Hide();
        }


        /// <summary>
        /// Checks for checked cells in the available checked column.
        /// </summary>
        private void checkAvailableChecks()
        {
            foreach (DataGridViewRow row in availableFilesList.Rows)
            {
                if (Convert.ToBoolean(row.Cells[selectIndex].Value) == true)
                {
                    downloadButton.Show();
                    return;
                }
            }

            downloadButton.Hide();
        }

        /// <summary>
        /// Prevents double clicks on check boxes from not registering.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void availablesharedfileList_OnCellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (e.ColumnIndex == AvailCheckColumn.Index && e.RowIndex != -1)
            {
                availableFilesList.EndEdit();
            }
        }


        /// <summary>
        /// Invokes a FilesRemoved event with the list of checked shared files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, EventArgs e)
        {
            String[] files = new String[sharedFilesList.Rows.Count];
            int count = 0;

            foreach (DataGridViewRow row in sharedFilesList.Rows)
            {
                if (Convert.ToBoolean(row.Cells[selectIndex].Value) == true)
                {
                    files[count] = row.Cells[locationIndex].Value.ToString();
                    count++;
                }
            }

            if (count > 0 && FilesRemoved != null)
            {
                FilesRemoved.Invoke(this, new FilesRemovedEventArgs(files));
            }

            //sharedFilesList.EndEdit();
            checkSharedChecks();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (RefreshClients != null)
            {
                RefreshClients.Invoke(this, EventArgs.Empty);
            }
        }

        private void availableFilesList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            // Get names of selected available files.
            Dictionary<String, String> files = new Dictionary<string, string>();
            foreach (DataGridViewRow row in availableFilesList.Rows)
            {
                files.Add((String)row.Cells[nameIndex].Value, (String)row.Cells[locationIndex].Value);
            }

            if (DownloadFiles != null)
            {
                if (DownloadFiles != null)
                {
                    DownloadFiles.Invoke(this, new DownloadFilesEventArgs() { Files = files });
                }
            }
        }



        public class FilesSelectedEventArgs : EventArgs
        {
            public String[] Files { get; }
            public FilesSelectedEventArgs(String[] files)
            {
                this.Files = files;
            }
        }

        public class FilesRemovedEventArgs : EventArgs
        {
            public String[] Files { get; }
            public FilesRemovedEventArgs(String[] files)
            {
                this.Files = files;
            }
        }

        public class DownloadFilesEventArgs : EventArgs
        {
            public Dictionary<String, String> Files { get; set; }
        }

       
    }
}
