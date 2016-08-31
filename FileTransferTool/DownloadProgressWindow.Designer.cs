namespace FileTransferTool
{
    partial class DownloadProgressWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.DownloadList = new System.Windows.Forms.DataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DownloadList)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(307, 13);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(630, 13);
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Value = 50;
            // 
            // DownloadList
            // 
            this.DownloadList.AllowUserToAddRows = false;
            this.DownloadList.AllowUserToDeleteRows = false;
            this.DownloadList.BackgroundColor = System.Drawing.Color.White;
            this.DownloadList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DownloadList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.LocationColumn});
            this.DownloadList.Location = new System.Drawing.Point(13, 13);
            this.DownloadList.Name = "DownloadList";
            this.DownloadList.ReadOnly = true;
            this.DownloadList.RowHeadersVisible = false;
            this.DownloadList.Size = new System.Drawing.Size(924, 285);
            this.DownloadList.TabIndex = 2;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 300;
            // 
            // LocationColumn
            // 
            this.LocationColumn.HeaderText = "Location";
            this.LocationColumn.Name = "LocationColumn";
            this.LocationColumn.ReadOnly = true;
            this.LocationColumn.Width = 150;
            // 
            // DownloadProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 310);
            this.Controls.Add(this.DownloadList);
            this.Controls.Add(this.progressBar1);
            this.Name = "DownloadProgressWindow";
            this.Text = "DownloadProgressWindow";
            ((System.ComponentModel.ISupportInitialize)(this.DownloadList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.DataGridView DownloadList;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationColumn;
    }
}