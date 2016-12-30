using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransferTool.CoreLibrary;
using System.Windows.Forms;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.Windows
{
    public class DataGridViewFTTFileInfoAdapter : DataGridViewManager, IDisposable
    {
        private DataGridView _dataGrid;

        private delegate void addRowCallback(FTTFileInfo file);
        private delegate void removeRowCallback(DataGridViewRow row);


        private List<CheckableFileInfo> _files;
        private BindingSource bs;

        public DataGridViewFTTFileInfoAdapter(DataGridView dataGrid)
        {
            _dataGrid = dataGrid;
            dataGrid.SortCompare += new DataGridViewSortCompareEventHandler(dataViewGrid_SortCompare);

            _files = new List<CheckableFileInfo>();
            bs = new BindingSource();
            bs.DataSource = _files;
            _dataGrid.DataSource = bs;
            bs.ResetBindings(false);
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

            List<CheckableFileInfo> removeFiles = new List<CheckableFileInfo>();

            // Add new files
            foreach (FTTFileInfo f in files)
            {
                bool found = false;

                foreach (CheckableFileInfo cf in _files)
                {
                    if (cf.Alias == f.Alias && cf.IP == f.IP)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    _files.Add(new CheckableFileInfo(f));
                }
            }


            // Check if files are in datagrid but not in the arg.
            foreach (CheckableFileInfo cf in _files)
            {
                bool found = false;

                foreach (FTTFileInfo f in files)
                {
                    if (cf.Alias == f.Alias && cf.IP == f.IP)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    removeFiles.Add(cf);
                }
            }

            foreach (CheckableFileInfo f in removeFiles)
            {
                _files.Remove(f);
            }


            bs.ResetBindings(false);
        }

        public void Dispose()
        {
            bs.Dispose();
            _dataGrid.Dispose();
        }
    }
}
