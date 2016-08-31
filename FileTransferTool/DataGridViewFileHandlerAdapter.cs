using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;
using System.Windows.Forms;

namespace FileTransferTool
{

    /// <summary>
    /// Manages a given data grid to reflect what is in list of files.
    /// 
    /// Add the Core_FilesChange to an event trigger for updating the grid view.
    /// </summary>
    public class DataGridViewFileHandlerAdapter : DataGridViewManager
    {

        private DataGridView _dataGrid;
        
        public DataGridViewFileHandlerAdapter(DataGridView dataGrid)
        {
            _dataGrid = dataGrid;
            dataGrid.SortCompare += new DataGridViewSortCompareEventHandler(dataViewGrid_SortCompare);
        }


        /// <summary>
        /// Initializes the data grid if there are already files
        /// </summary>
        private void initDataGrid(List<FileHandler> files)
        {
            foreach (FileHandler f in files)
            {
                f.FileInfoChanged += fileHandler_FileInfoChanged;

                DataGridViewRow row = (DataGridViewRow)_dataGrid.Rows[0].Clone();
                row.Cells[MainWindow.nameIndex].Value = f.Name;
                row.Cells[MainWindow.locationIndex].Value = f.Path;
                row.Cells[MainWindow.sizeIndex].Value = f.Size;
                _dataGrid.Rows.Add(row);
            }
        }


        /// <summary>
        /// To be assigned to listen to changes in either the shared file list or available file list
        /// </summary>
        /// <param name="e"></param>
        /// <param name="e"></param>
        public void FilesChanged(List<FileHandler> files)
        {
            syncGrid(files);
        }


        /// <summary>
        /// Called when a change occurs. Will remove and add files to the grid view as needed.
        /// </summary>
        private void syncGrid(List<FileHandler> files)
        {

            // Check if files exist in the files list and not in grid view (adding files to grid).
            foreach (FileHandler f in files)
            {
                bool found = false;

                foreach (DataGridViewRow row in _dataGrid.Rows)
                {
                    object value = row.Cells[MainWindow.locationIndex].Value;
                    if (value != null && ((String)value).Equals(f.Path))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    addRowToGrid(f);
                }
            }

            // Check if files exist in the grid view and not in the files list (removing files from grid)
            DataGridViewRow[] delete_queue = new DataGridViewRow[_dataGrid.RowCount];
            int count = 0;
            foreach (DataGridViewRow row in _dataGrid.Rows)
            {
                if (row.IsNewRow) break;

                object value = row.Cells[MainWindow.locationIndex].Value;
                bool found = false;

                foreach (FileHandler f in files)
                {
                    
                    if (value == null || f.Path.Equals((String)value))
                    {
                        found = true;
                        break;  
                    }                 
                }  
                
                if (!found)
                {
                    delete_queue[count] = row;
                    count++;
                } 
            }
           
            // Remove files.
            for (int i = 0; i < delete_queue.Length; i++)
            {
                if (delete_queue[i] != null)
                {
                    _dataGrid.Rows.Remove(delete_queue[i]);
                }
            }
        }


        /// <summary>
        /// Adds new rows to file grid
        /// </summary>
        /// <param name="file"></param>
        private void addRowToGrid(FileHandler file)
        {
            file.FileInfoChanged += fileHandler_FileInfoChanged;
            _dataGrid.Rows.Add(new object[] {false, file.Name, file.Path, file.Size });
      
            
            //_dataGrid.Refresh();
        }


        /// <summary>
        /// Called when any of the contained FileHandler's properties change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileHandler_FileInfoChanged(object sender, FileHandler.FileInfoChangedEventArgs e)
        {
            foreach (DataGridViewRow row in _dataGrid.Rows)
            {
                if (((String)row.Cells[MainWindow.locationIndex].Value).Equals(e.Path)){
                    row.Cells[MainWindow.sizeIndex].Value = e.Size;
                    break;
                }
            }


        }


    }
}
