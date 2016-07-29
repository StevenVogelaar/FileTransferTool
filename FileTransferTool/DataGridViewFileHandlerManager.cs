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
    public class DataGridViewFileHandlerAdapter
    {

        private DataGridView _dataGrid;
        private List<FileHandler> _files;

        
        public DataGridViewFileHandlerAdapter(DataGridView dataGrid, List<FileHandler> files)
        {
            _dataGrid = dataGrid;
            _files = files;
            dataGrid.SortCompare += new DataGridViewSortCompareEventHandler(dataViewGrid_SortCompare);
            initDataGrid();
        }


        /// <summary>
        /// Initializes the data grid if there are already files
        /// </summary>
        private void initDataGrid()
        {
            foreach (FileHandler f in _files)
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
        public void Core_FilesChanged(object obj, EventArgs e)
        {
            syncGrid();
        }


        /// <summary>
        /// Called when a change occurs. Will remove and add files to the grid view as needed.
        /// </summary>
        private void syncGrid()
        {

            // Check if files exist in the files list and not in grid view (adding files to grid).
            foreach (FileHandler f in _files)
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

                foreach (FileHandler f in _files)
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
        /// Custom sorting code for the size column.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void dataViewGrid_SortCompare(object obj, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "SharedSizeColumn" || e.Column.Name == "AvailSizeColumn")
            {

                int rank1 = parseSizeCategory((String)e.CellValue1);
                int rank2 = parseSizeCategory((String)e.CellValue2);

                // Try to sort by size category (Bytes, KiB, MiB, GiB).
                if (rank1 < rank2)
                {
                    e.SortResult = -1;
                    e.Handled = true;
                    return;
                }
                else if (rank2 < rank1)
                {
                    e.SortResult = 1;
                    e.Handled = true;
                    return;
                }

                // Size categories are the same, so sort based on number size
                try
                {
                    float length1 = float.Parse(((String)e.CellValue1).Split(' ')[0]);
                    float length2 = float.Parse(((String)e.CellValue2).Split(' ')[0]);

                    if (length1 < length2)
                    {
                        e.SortResult = -1;
                        e.Handled = true;
                        return;
                    }
                    else if (length2 < length1)
                    {
                        e.SortResult = 1;
                        e.Handled = true;
                        return;
                    }
                    else
                    {
                        e.SortResult = 0;
                        e.Handled = true;
                        return;
                    }
                }
                catch (Exception exception)
                {
                    e.Handled = false;
                }
            }
            else e.Handled = false;
        }

        /// <summary>
        /// Returns an integer based on the size category (Byte, KiB, MiB, GiB).
        /// </summary>
        /// <param name="size"></param>
        /// <returns>0 = Byte, 1 = KiB, 2 = MiB, 3 = GiB</returns>
        private int parseSizeCategory(String size)
        {
            int rank;
            if (size.Contains("Bytes")) rank = 0;
            else if (size.Contains("KiB")) rank = 1;
            else if (size.Contains("MiB")) rank = 2;
            else if (size.Contains("GiB")) rank = 3;
            else rank = 4;

            return rank;
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
