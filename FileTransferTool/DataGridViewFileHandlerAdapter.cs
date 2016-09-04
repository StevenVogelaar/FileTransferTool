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

            subscribeToEvents(files);
            _dataGrid.DataSource = files;
            _dataGrid.Update();
            _dataGrid.Refresh();
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
            subscribeToEvents(files);
            _dataGrid.DataSource = files;
            _dataGrid.Refresh();
        }


        /// <summary>
        /// Subscribes to each file in the list.
        /// </summary>
        /// <param name=""></param>
        private void subscribeToEvents(List<FileHandler> files)
        {
            foreach (FileHandler f in files)
            {
                f.FileInfoChanged += fileHandler_FileInfoChanged;
            }
        }


        /// <summary>
        /// Unsubscribes to each file in the list.
        /// </summary>
        /// <param name="files"></param>
        private void unsubscribeToEvents(List<FileHandler> files)
        {
            foreach (FileHandler f in files)
            {
                f.FileInfoChanged -= fileHandler_FileInfoChanged;
            }
        }


        /// <summary>
        /// Called when any of the contained FileHandler's properties change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileHandler_FileInfoChanged(object sender, FileHandler.FileInfoChangedEventArgs e)
        {
            _dataGrid.Refresh();
        }


    }
}
