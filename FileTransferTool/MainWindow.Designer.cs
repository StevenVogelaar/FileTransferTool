﻿namespace FileTransferTool
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.availableLable = new System.Windows.Forms.Label();
            this.sharedLable = new System.Windows.Forms.Label();
            this.addFilesButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.addFolderButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.connectionStatusLable = new System.Windows.Forms.Label();
            this.sharedFilesList = new System.Windows.Forms.DataGridView();
            this.SharedCheckColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SharedNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SharedLocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SharedSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.availableFilesList = new System.Windows.Forms.DataGridView();
            this.AvailCheckColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.AvailNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvailLocationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvailSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.sharedFilesList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableFilesList)).BeginInit();
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
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(638, 84);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 10;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
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
            this.connectionStatusLable.BackColor = System.Drawing.Color.White;
            this.connectionStatusLable.Location = new System.Drawing.Point(12, 51);
            this.connectionStatusLable.Name = "connectionStatusLable";
            this.connectionStatusLable.Size = new System.Drawing.Size(79, 13);
            this.connectionStatusLable.TabIndex = 12;
            this.connectionStatusLable.Text = "Not Connected";
            // 
            // sharedFilesList
            // 
            this.sharedFilesList.AllowUserToAddRows = false;
            this.sharedFilesList.AllowUserToDeleteRows = false;
            this.sharedFilesList.AllowUserToResizeRows = false;
            this.sharedFilesList.BackgroundColor = System.Drawing.Color.White;
            this.sharedFilesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sharedFilesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SharedCheckColumn,
            this.SharedNameColumn,
            this.SharedLocationColumn,
            this.SharedSizeColumn});
            this.sharedFilesList.Location = new System.Drawing.Point(16, 145);
            this.sharedFilesList.Name = "sharedFilesList";
            this.sharedFilesList.RowHeadersVisible = false;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.sharedFilesList.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.sharedFilesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sharedFilesList.Size = new System.Drawing.Size(778, 169);
            this.sharedFilesList.TabIndex = 13;
            // 
            // SharedCheckColumn
            // 
            this.SharedCheckColumn.FalseValue = "false";
            this.SharedCheckColumn.HeaderText = "Select";
            this.SharedCheckColumn.Name = "SharedCheckColumn";
            this.SharedCheckColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SharedCheckColumn.TrueValue = "true";
            this.SharedCheckColumn.Width = 43;
            // 
            // SharedNameColumn
            // 
            this.SharedNameColumn.HeaderText = "Name";
            this.SharedNameColumn.Name = "SharedNameColumn";
            this.SharedNameColumn.ReadOnly = true;
            this.SharedNameColumn.Width = 250;
            // 
            // SharedLocationColumn
            // 
            this.SharedLocationColumn.HeaderText = "Location";
            this.SharedLocationColumn.Name = "SharedLocationColumn";
            this.SharedLocationColumn.ReadOnly = true;
            this.SharedLocationColumn.Width = 390;
            // 
            // SharedSizeColumn
            // 
            this.SharedSizeColumn.HeaderText = "Size";
            this.SharedSizeColumn.Name = "SharedSizeColumn";
            this.SharedSizeColumn.ReadOnly = true;
            this.SharedSizeColumn.Width = 250;
            // 
            // availableFilesList
            // 
            this.availableFilesList.AllowUserToAddRows = false;
            this.availableFilesList.AllowUserToDeleteRows = false;
            this.availableFilesList.AllowUserToResizeRows = false;
            this.availableFilesList.BackgroundColor = System.Drawing.Color.White;
            this.availableFilesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.availableFilesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AvailCheckColumn,
            this.AvailNameColumn,
            this.AvailLocationColumn,
            this.AvailSizeColumn});
            this.availableFilesList.Location = new System.Drawing.Point(16, 336);
            this.availableFilesList.Name = "availableFilesList";
            this.availableFilesList.RowHeadersVisible = false;
            this.availableFilesList.RowHeadersWidth = 40;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this.availableFilesList.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.availableFilesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.availableFilesList.Size = new System.Drawing.Size(778, 251);
            this.availableFilesList.TabIndex = 14;
            // 
            // AvailCheckColumn
            // 
            this.AvailCheckColumn.HeaderText = "Select";
            this.AvailCheckColumn.Name = "AvailCheckColumn";
            this.AvailCheckColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AvailCheckColumn.Width = 43;
            // 
            // AvailNameColumn
            // 
            this.AvailNameColumn.HeaderText = "Name";
            this.AvailNameColumn.Name = "AvailNameColumn";
            this.AvailNameColumn.ReadOnly = true;
            this.AvailNameColumn.Width = 250;
            // 
            // AvailLocationColumn
            // 
            this.AvailLocationColumn.HeaderText = "Location";
            this.AvailLocationColumn.Name = "AvailLocationColumn";
            this.AvailLocationColumn.ReadOnly = true;
            this.AvailLocationColumn.Width = 390;
            // 
            // AvailSizeColumn
            // 
            this.AvailSizeColumn.HeaderText = "Size";
            this.AvailSizeColumn.Name = "AvailSizeColumn";
            this.AvailSizeColumn.ReadOnly = true;
            this.AvailSizeColumn.Width = 250;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.Location = new System.Drawing.Point(0, 118);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(782, 2);
            this.panel1.TabIndex = 15;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 118);
            this.panel2.TabIndex = 16;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 621);
            this.Controls.Add(this.sharedFilesList);
            this.Controls.Add(this.availableFilesList);
            this.Controls.Add(this.connectionStatusLable);
            this.Controls.Add(this.addFolderButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.addFilesButton);
            this.Controls.Add(this.sharedLable);
            this.Controls.Add(this.availableLable);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "MainWindow";
            this.Text = "File Transfer Tool";
            ((System.ComponentModel.ISupportInitialize)(this.sharedFilesList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.availableFilesList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label availableLable;
        private System.Windows.Forms.Label sharedLable;
        private System.Windows.Forms.Button addFilesButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button addFolderButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label connectionStatusLable;
        private System.Windows.Forms.DataGridView sharedFilesList;
        private System.Windows.Forms.DataGridView availableFilesList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SharedCheckColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SharedNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SharedLocationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SharedSizeColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn AvailCheckColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailLocationColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailSizeColumn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}

