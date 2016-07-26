namespace FileTransferTool
{
    partial class MainWindow
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
            this.availableLable = new System.Windows.Forms.Label();
            this.sharedLable = new System.Windows.Forms.Label();
            this.addFilesButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.availableFilesList = new System.Windows.Forms.CheckedListBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.addFolderButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.connectionStatusLable = new System.Windows.Forms.Label();
            this.sharedFilesList = new System.Windows.Forms.DataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.sharedFilesList)).BeginInit();
            this.SuspendLayout();
            // 
            // availableLable
            // 
            this.availableLable.AutoSize = true;
            this.availableLable.Location = new System.Drawing.Point(13, 319);
            this.availableLable.Name = "availableLable";
            this.availableLable.Size = new System.Drawing.Size(74, 13);
            this.availableLable.TabIndex = 2;
            this.availableLable.Text = "Available Files";
            // 
            // sharedLable
            // 
            this.sharedLable.AutoSize = true;
            this.sharedLable.Location = new System.Drawing.Point(12, 120);
            this.sharedLable.Name = "sharedLable";
            this.sharedLable.Size = new System.Drawing.Size(65, 13);
            this.sharedLable.TabIndex = 3;
            this.sharedLable.Text = "Shared Files";
            // 
            // addFilesButton
            // 
            this.addFilesButton.Location = new System.Drawing.Point(12, 12);
            this.addFilesButton.Name = "addFilesButton";
            this.addFilesButton.Size = new System.Drawing.Size(75, 23);
            this.addFilesButton.TabIndex = 4;
            this.addFilesButton.Text = "Add Files";
            this.addFilesButton.UseVisualStyleBackColor = true;
            this.addFilesButton.Click += new System.EventHandler(this.addFilesButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(12, 84);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(75, 23);
            this.refreshButton.TabIndex = 6;
            this.refreshButton.Text = "Refresh List";
            this.refreshButton.UseVisualStyleBackColor = true;
            // 
            // downloadButton
            // 
            this.downloadButton.Location = new System.Drawing.Point(94, 84);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(75, 23);
            this.downloadButton.TabIndex = 7;
            this.downloadButton.Text = "Download";
            this.downloadButton.UseVisualStyleBackColor = true;
            // 
            // availableFilesList
            // 
            this.availableFilesList.FormattingEnabled = true;
            this.availableFilesList.Items.AddRange(new object[] {
            "123",
            "124",
            "43653456",
            "345345"});
            this.availableFilesList.Location = new System.Drawing.Point(16, 336);
            this.availableFilesList.Name = "availableFilesList";
            this.availableFilesList.Size = new System.Drawing.Size(778, 244);
            this.availableFilesList.TabIndex = 9;
            this.availableFilesList.SelectedIndexChanged += new System.EventHandler(this.availableFilesList_SelectedIndexChanged);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(638, 84);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 10;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog1
            // 


            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(94, 12);
            this.addFolderButton.Name = "addFolderButton";
            this.addFolderButton.Size = new System.Drawing.Size(75, 23);
            this.addFolderButton.TabIndex = 11;
            this.addFolderButton.Text = "Add Folder";
            this.addFolderButton.UseVisualStyleBackColor = true;
            this.addFolderButton.Click += new System.EventHandler(this.addFolderButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // connectionStatusLable
            // 
            this.connectionStatusLable.AutoSize = true;
            this.connectionStatusLable.Location = new System.Drawing.Point(12, 51);
            this.connectionStatusLable.Name = "connectionStatusLable";
            this.connectionStatusLable.Size = new System.Drawing.Size(79, 13);
            this.connectionStatusLable.TabIndex = 12;
            this.connectionStatusLable.Text = "Not Connected";
            // 
            // sharedFilesList
            // 
            this.sharedFilesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sharedFilesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.LocationColumn,
            this.SizeColumn,
            this.IndexColumn});
            this.sharedFilesList.Location = new System.Drawing.Point(16, 136);
            this.sharedFilesList.Name = "sharedFilesList";
            this.sharedFilesList.Size = new System.Drawing.Size(778, 169);
            this.sharedFilesList.TabIndex = 13;
            this.sharedFilesList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.sharedFilesList_CellContentClick);
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            // 
            // LocationColumn
            // 
            this.LocationColumn.HeaderText = "Location";
            this.LocationColumn.Name = "LocationColumn";
            // 
            // SizeColumn
            // 
            this.SizeColumn.HeaderText = "Size";
            this.SizeColumn.Name = "SizeColumn";
            // 
            // IndexColumn
            // 
            this.IndexColumn.HeaderText = "Index";
            this.IndexColumn.Name = "IndexColumn";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 599);
            this.Controls.Add(this.sharedFilesList);
            this.Controls.Add(this.connectionStatusLable);
            this.Controls.Add(this.addFolderButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.availableFilesList);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.addFilesButton);
            this.Controls.Add(this.sharedLable);
            this.Controls.Add(this.availableLable);
            this.Name = "MainWindow";
            this.Text = "File Transfer Tool";
            ((System.ComponentModel.ISupportInitialize)(this.sharedFilesList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label availableLable;
        private System.Windows.Forms.Label sharedLable;
        private System.Windows.Forms.Button addFilesButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.CheckedListBox availableFilesList;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button addFolderButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label connectionStatusLable;
        private System.Windows.Forms.DataGridView sharedFilesList;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexColumn;
    }
}

