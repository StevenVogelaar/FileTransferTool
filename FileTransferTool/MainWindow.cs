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

        public DownloadProgressWindow DownloadProgressWindow;
        public DataGridViewFileHandlerAdapter SharedGridManager;
        public DataGridViewFTTFileInfoAdapter AvailableGridManager;

        private ListViewConsoleAdapter _listViewConsoleAdapter;
        private WindowsUI _windowsUI;

        public MainWindow(WindowsUI windowsUI)
        {
            InitializeComponent();

            _windowsUI = windowsUI;

            this.SizeChanged += onWindowSizeChange;
            this.MinimumSize = new Size(this.Width, this.Height);
                       
            downloadButton.Hide();
            removeButton.Hide();

            openFileDialog1.Multiselect = true;

            // Init datagrid settings.
            this.availableFilesList.CellValueChanged += availableFileList_OnCellValueChanged;
            this.availableFilesList.CellMouseUp += availablesharedfileList_OnCellMouseUp;
            this.sharedFilesList.CellValueChanged += sharedFileList_OnCellValueChanged;
            this.sharedFilesList.CellMouseUp += sharedfileList_OnCellMouseUp;
            this.Load += onLoad;
            this.availableFilesList.CellValueChanged += AvailableFilesList_CellValueChanged;

            // Init managers.
            SharedGridManager = new DataGridViewFileHandlerAdapter(sharedFilesList);
            AvailableGridManager = new DataGridViewFTTFileInfoAdapter(availableFilesList);


            _listViewConsoleAdapter = new ListViewConsoleAdapter(MessageConsole);
            DownloadProgressWindow = new DownloadProgressWindow(delegate () { _windowsUI.MainWindowDownloadCancel(this, EventArgs.Empty);});
            

            
            this.BackColor = Color.LightGray;

            onWindowSizeChange(this, EventArgs.Empty);
        }


        /// <summary>
        /// Disallows checks on available files that have not had their size calculated yet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AvailableFilesList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if ((bool)availableFilesList.Rows[e.RowIndex].Cells[0].Value == true && FileHandler.ParseSize((String)availableFilesList.Rows[e.RowIndex].Cells[3].Value) == -1)
                {
                    availableFilesList.Rows[e.RowIndex].Cells[0].Value = false;
                }
            }
        }


        /// <summary>
        /// Initializes some stuff that has to be done after the form has loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLoad(object sender, EventArgs e)
        {
            FTTConsole.ConsoleMessage += _listViewConsoleAdapter.ConsoleMessaged;
        }

     
        /// <summary>
        /// Initializes grids with their grid managers.
        /// </summary>
        /// <param name="core"></param>
        public void Init()
        {
            SharedGridManager = new DataGridViewFileHandlerAdapter(sharedFilesList);
            AvailableGridManager = new DataGridViewFTTFileInfoAdapter(availableFilesList);
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
        /// Triggers a check for checked rows in the available files list.
        /// </summary>
        public void AvailFilesChanged()
        {
            checkAvailableChecks();
        }

        /// <summary>
        /// Checks if user has selected files. Fires a FilesSelected event if true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilesButton_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
               _windowsUI.MainWindowFilesSelected(this, new FTUI.FilesSelectedEventArgs(openFileDialog1.FileNames));      
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

            if (result == DialogResult.OK )
            {
                _windowsUI.MainWindowFilesSelected(this, new FTUI.FilesSelectedEventArgs(new String[] { folderBrowserDialog1.SelectedPath }));
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
        public void checkSharedChecks()
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

            if (count > 0)
            {
                _windowsUI.MainWindowFilesRemoved(this, new FTUI.FilesRemovedEventArgs(files));
            }

            //sharedFilesList.EndEdit();
            checkSharedChecks();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {

             _windowsUI.MainWindowRefresh(this, EventArgs.Empty);

        }


        /// <summary>
        /// Attempts to start a download operation when the download button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadButton_Click(object sender, EventArgs e)
        {
            bool thing = DownloadProgressWindow.DownloadInProggress;
            if (DownloadProgressWindow.DownloadInProggress) return;

            // Get location to download the files.
            DialogResult result =  folderBrowserDialog1.ShowDialog();
            if (result != DialogResult.OK) return;

            // Get names of selected available files.
            Dictionary<String, String> files = new Dictionary<string, string>();
            List<DownloadProgressWindow.ProgressData> progressFiles = new List<DownloadProgressWindow.ProgressData>();
            foreach (DataGridViewRow row in availableFilesList.Rows)
            {
                if ((bool)row.Cells[0].Value == true)
                {
                    files.Add((String)row.Cells[nameIndex].Value, (String)row.Cells[locationIndex].Value);
                    progressFiles.Add(new DownloadProgressWindow.ProgressData()
                    {
                        Alias = (String)row.Cells[nameIndex].Value,
                        IP = (String)row.Cells[locationIndex].Value,
                        Size = FileHandler.ParseSize((String)row.Cells[sizeIndex].Value),
                        Progress = 0
                    });
                }
            }


            FileDownloadProgress callbacks = new FileDownloadProgress();
            DownloadProgressWindow.StartDownload(progressFiles, callbacks);

            _windowsUI.MainWindowDownloadfiles(this, new FTUI.DownloadRequestEventArgs() { Files = files, Dest = folderBrowserDialog1.SelectedPath, CallBacks = callbacks });
            DownloadProgressWindow.Show();
        }

       
    }
}
