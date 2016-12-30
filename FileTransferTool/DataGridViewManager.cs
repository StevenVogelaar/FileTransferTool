using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.Windows
{
    public class DataGridViewManager
    {

        /// <summary>
        /// Custom sorting code for the size column.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        protected void dataViewGrid_SortCompare(object obj, DataGridViewSortCompareEventArgs e)
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

    }
}
