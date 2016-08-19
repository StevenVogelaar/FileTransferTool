using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;
using System.Windows.Forms;

namespace FileTransferTool
{
    class DataGridViewFTTFileInfoAdapter : DataGridViewManager
    {
        private DataGridView _dataGrid;

        private delegate void addRowCallback(FTTFileInfo file);
        private delegate void removeRowCallback(DataGridViewRow row);

        public DataGridViewFTTFileInfoAdapter(DataGridView dataGrid, List<FTTFileInfo> files)
        {
            _dataGrid = dataGrid;
            dataGrid.SortCompare += new DataGridViewSortCompareEventHandler(dataViewGrid_SortCompare);
            initDataGrid(files);
        }


        /// <summary>
        /// Initializes the data grid if there are already files
        /// </summary>
        private void initDataGrid(List<FTTFileInfo> files)
        {
            foreach (FTTFileInfo f in files)
            {

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
        public void Core_FilesChanged(object obj, Core.AvailableFilesChangedEventArgs e)
        {
            syncGrid(e.Files);
        }


        /// <summary>
        /// Called when a change occurs. Will remove and add files to the grid view as needed.
        /// </summary>
        private void syncGrid(List<FTTFileInfo> files)
        {

            // Check if files exist in the files list and not in grid view (adding files to grid).
            foreach (FTTFileInfo f in files)
            {
                bool found = false;

                foreach (DataGridViewRow row in _dataGrid.Rows)
                {
                    object nameValue = row.Cells[MainWindow.nameIndex].Value;
                    object locationValue = row.Cells[MainWindow.locationIndex].Value;
                    if (nameValue != null && ((String)nameValue).Equals(f.Name) && ((String)locationValue).Equals(f.IP))
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

            // Check if files with the same IP exist in the grid view and not in the files list (removing files from grid)
            DataGridViewRow[] delete_queue = new DataGridViewRow[_dataGrid.RowCount];
            int count = 0;
            foreach (DataGridViewRow row in _dataGrid.Rows)
            {
                if (row.IsNewRow) break;

                object nameValue = row.Cells[MainWindow.nameIndex].Value;
                object locationValue = row.Cells[MainWindow.locationIndex].Value;
                bool found = false;

                foreach (FTTFileInfo f in files)
                {
                    //Console.WriteLine("location1: " + locationValue + " location2: " + f.IP);
                    if (!f.IP.Equals(locationValue))
                    {
                        // IPs dont match so skip.
                        continue;
                    }

                    if (nameValue == null || f.Name.Equals((String)nameValue))
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
                    removeRow(delete_queue[i]);
                }
            }
        }


        /// <summary>
        /// Adds new rows to file grid by using the datagrids invoke method.
        /// </summary>
        /// <param name="file"></param>
        private void addRowToGrid(FTTFileInfo file)
        {
            addRowCallback d = new addRowCallback(dataGridAddRow);
            _dataGrid.Invoke(d, new object[] { file });
        }


        /// <summary>
        /// Adds row to datagrid.
        /// </summary>
        /// <param name="file"></param>
        private void dataGridAddRow(FTTFileInfo file)
        {
            _dataGrid.Rows.Add(new object[] { false, file.Name, file.IP, file.Size });
        }


        private void removeRow(DataGridViewRow row)
        {
            removeRowCallback d = new removeRowCallback(dataGridRemoveRow);
            _dataGrid.Invoke(d, new object[] { row });
        }

        /// <summary>
        /// Removes row from datagrid.
        /// </summary>
        /// <param name="row"></param>
        private void dataGridRemoveRow(DataGridViewRow row)
        {
            _dataGrid.Rows.Remove(row);
        }



        
    }
}
