using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary;
using System.Windows.Forms;

namespace FileTransferTool
{
    public class DataGridViewFTTFileInfoAdapter : DataGridViewManager
    {
        private DataGridView _dataGrid;

        private delegate void addRowCallback(FTTFileInfo file);
        private delegate void removeRowCallback(DataGridViewRow row);

        public DataGridViewFTTFileInfoAdapter(DataGridView dataGrid)
        {
            _dataGrid = dataGrid;
            dataGrid.SortCompare += new DataGridViewSortCompareEventHandler(dataViewGrid_SortCompare);
        }


        /// <summary>
        /// Initializes the data grid if there are already files
        /// </summary>
        private void initDataGrid(List<FTTFileInfo> files)
        {
            _dataGrid.DataSource = files;
            
            _dataGrid.Refresh();
        }


        /// <summary>
        /// To be assigned to listen to changes in either the shared file list or available file list
        /// </summary>
        /// <param name="e"></param>
        /// <param name="e"></param>
        public void FilesChanged(List<FTTFileInfo> files)
        {
            syncGrid(files);
        }


        /// <summary>
        /// Called when a change occurs. Will remove and add files to the grid view as needed.
        /// </summary>
        private void syncGrid(List<FTTFileInfo> files)
        {
            _dataGrid.DataSource = files;
            _dataGrid.Refresh();
        }

    }
}
