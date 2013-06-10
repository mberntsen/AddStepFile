namespace AddStepFile
{
    partial class AddStepFileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddStepFileForm));
            this._btnStart = new System.Windows.Forms.Button();
            this._btnStop = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this._checkBoxIncludeSubitems = new System.Windows.Forms.CheckBox();
            this._listView = new System.Windows.Forms.ListView();
            this._colNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colRevision = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._ColFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._ColFileRevision = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colProcessStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._btnGoToFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _btnStart
            // 
            this._btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnStart.Location = new System.Drawing.Point(708, 8);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(75, 23);
            this._btnStart.TabIndex = 0;
            this._btnStart.Text = "Start";
            this._btnStart.UseVisualStyleBackColor = true;
            this._btnStart.Click += new System.EventHandler(this.Handle_BtnStart_Click);
            // 
            // _btnStop
            // 
            this._btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnStop.Enabled = false;
            this._btnStop.Location = new System.Drawing.Point(708, 37);
            this._btnStop.Name = "_btnStop";
            this._btnStop.Size = new System.Drawing.Size(75, 23);
            this._btnStop.TabIndex = 0;
            this._btnStop.Text = "Stop";
            this._btnStop.UseVisualStyleBackColor = true;
            this._btnStop.Click += new System.EventHandler(this.Handle_BtnStop_Click);
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.Location = new System.Drawing.Point(708, 66);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(75, 23);
            this._btnClose.TabIndex = 0;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = true;
            this._btnClose.Click += new System.EventHandler(this.Handle_BtnClose_Click);
            // 
            // _checkBoxIncludeSubitems
            // 
            this._checkBoxIncludeSubitems.AutoSize = true;
            this._checkBoxIncludeSubitems.Checked = true;
            this._checkBoxIncludeSubitems.CheckState = System.Windows.Forms.CheckState.Checked;
            this._checkBoxIncludeSubitems.Enabled = false;
            this._checkBoxIncludeSubitems.Location = new System.Drawing.Point(13, 12);
            this._checkBoxIncludeSubitems.Name = "_checkBoxIncludeSubitems";
            this._checkBoxIncludeSubitems.Size = new System.Drawing.Size(107, 17);
            this._checkBoxIncludeSubitems.TabIndex = 1;
            this._checkBoxIncludeSubitems.Text = "Include Subitems";
            this._checkBoxIncludeSubitems.UseVisualStyleBackColor = true;
            // 
            // _listView
            // 
            this._listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colNumber,
            this._colRevision,
            this._colTitle,
            this._ColFilename,
            this._ColFileRevision,
            this._colProcessStatus});
            this._listView.FullRowSelect = true;
            this._listView.HideSelection = false;
            this._listView.Location = new System.Drawing.Point(13, 95);
            this._listView.Name = "_listView";
            this._listView.Size = new System.Drawing.Size(770, 351);
            this._listView.SmallImageList = this._imageList;
            this._listView.TabIndex = 2;
            this._listView.UseCompatibleStateImageBehavior = false;
            this._listView.View = System.Windows.Forms.View.Details;
            this._listView.SelectedIndexChanged += new System.EventHandler(this.Handle_ListView_SelectedIndexChanged);
            // 
            // _colNumber
            // 
            this._colNumber.Text = "Number";
            this._colNumber.Width = 110;
            // 
            // _colRevision
            // 
            this._colRevision.Text = "Revision";
            this._colRevision.Width = 55;
            // 
            // _colTitle
            // 
            this._colTitle.Text = "Title";
            this._colTitle.Width = 220;
            // 
            // _ColFilename
            // 
            this._ColFilename.Text = "Part Filename";
            this._ColFilename.Width = 100;
            // 
            // _ColFileRevision
            // 
            this._ColFileRevision.Text = "Revision";
            this._ColFileRevision.Width = 55;
            // 
            // _colProcessStatus
            // 
            this._colProcessStatus.Text = "Process Status";
            this._colProcessStatus.Width = 200;
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "Document.ico");
            // 
            // _btnGoToFile
            // 
            this._btnGoToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnGoToFile.Enabled = false;
            this._btnGoToFile.Location = new System.Drawing.Point(-88, 66);
            this._btnGoToFile.Name = "_btnGoToFile";
            this._btnGoToFile.Size = new System.Drawing.Size(75, 23);
            this._btnGoToFile.TabIndex = 3;
            this._btnGoToFile.Text = "Go To";
            this._btnGoToFile.UseVisualStyleBackColor = true;
            // 
            // AddStepFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 458);
            this.Controls.Add(this._btnGoToFile);
            this.Controls.Add(this._listView);
            this.Controls.Add(this._checkBoxIncludeSubitems);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnStop);
            this.Controls.Add(this._btnStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddStepFileForm";
            this.ShowInTaskbar = false;
            this.Text = "Add Step Files";
            this.Load += new System.EventHandler(this.Handle_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.Button _btnStop;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.CheckBox _checkBoxIncludeSubitems;
        private System.Windows.Forms.ListView _listView;
        private System.Windows.Forms.ColumnHeader _colTitle;
        private System.Windows.Forms.ColumnHeader _colProcessStatus;
        private System.Windows.Forms.ColumnHeader _colRevision;
        private System.Windows.Forms.ImageList _imageList;
        private System.Windows.Forms.ColumnHeader _colNumber;
        private System.Windows.Forms.ColumnHeader _ColFilename;
        private System.Windows.Forms.ColumnHeader _ColFileRevision;
        private System.Windows.Forms.Button _btnGoToFile;
    }
}