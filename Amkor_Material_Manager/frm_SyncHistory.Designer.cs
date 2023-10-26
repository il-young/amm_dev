namespace Amkor_Material_Manager
{
    partial class frm_SyncHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_SyncHistory));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgv_SyncHistory = new System.Windows.Forms.DataGridView();
            this.dtp_from = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtp_to = new System.Windows.Forms.DateTimePicker();
            this.btn_search = new System.Windows.Forms.Button();
            this.btn_Excel = new System.Windows.Forms.Button();
            this.DATETIME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EQUIP_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TOWER_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LOTID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QTY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.INCH_INFO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SYNC_INFO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EMPLOYEE_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_directory = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SyncHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgv_SyncHistory);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btn_directory);
            this.splitContainer1.Panel2.Controls.Add(this.btn_Excel);
            this.splitContainer1.Panel2.Controls.Add(this.btn_search);
            this.splitContainer1.Panel2.Controls.Add(this.dtp_to);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.dtp_from);
            this.splitContainer1.Size = new System.Drawing.Size(1021, 450);
            this.splitContainer1.SplitterDistance = 409;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgv_SyncHistory
            // 
            this.dgv_SyncHistory.AllowUserToAddRows = false;
            this.dgv_SyncHistory.AllowUserToDeleteRows = false;
            this.dgv_SyncHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_SyncHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DATETIME,
            this.EQUIP_ID,
            this.TOWER_NO,
            this.UID,
            this.SID,
            this.LOTID,
            this.QTY,
            this.INCH_INFO,
            this.SYNC_INFO,
            this.EMPLOYEE_NO});
            this.dgv_SyncHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_SyncHistory.Location = new System.Drawing.Point(0, 0);
            this.dgv_SyncHistory.Name = "dgv_SyncHistory";
            this.dgv_SyncHistory.ReadOnly = true;
            this.dgv_SyncHistory.RowHeadersVisible = false;
            this.dgv_SyncHistory.RowTemplate.Height = 23;
            this.dgv_SyncHistory.Size = new System.Drawing.Size(1021, 409);
            this.dgv_SyncHistory.TabIndex = 0;
            // 
            // dtp_from
            // 
            this.dtp_from.CalendarFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtp_from.Dock = System.Windows.Forms.DockStyle.Left;
            this.dtp_from.Location = new System.Drawing.Point(0, 0);
            this.dtp_from.Name = "dtp_from";
            this.dtp_from.Size = new System.Drawing.Size(200, 21);
            this.dtp_from.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(200, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "->";
            // 
            // dtp_to
            // 
            this.dtp_to.CalendarFont = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.dtp_to.Dock = System.Windows.Forms.DockStyle.Left;
            this.dtp_to.Location = new System.Drawing.Point(229, 0);
            this.dtp_to.Name = "dtp_to";
            this.dtp_to.Size = new System.Drawing.Size(200, 21);
            this.dtp_to.TabIndex = 2;
            // 
            // btn_search
            // 
            this.btn_search.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_search.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_search.Location = new System.Drawing.Point(429, 0);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(75, 37);
            this.btn_search.TabIndex = 3;
            this.btn_search.Text = "Search";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // btn_Excel
            // 
            this.btn_Excel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_Excel.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Excel.Image = ((System.Drawing.Image)(resources.GetObject("btn_Excel.Image")));
            this.btn_Excel.Location = new System.Drawing.Point(969, 0);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.Size = new System.Drawing.Size(52, 37);
            this.btn_Excel.TabIndex = 4;
            this.btn_Excel.UseVisualStyleBackColor = true;
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // DATETIME
            // 
            this.DATETIME.HeaderText = "DATETIME";
            this.DATETIME.Name = "DATETIME";
            this.DATETIME.ReadOnly = true;
            // 
            // EQUIP_ID
            // 
            this.EQUIP_ID.HeaderText = "EQUIP_ID";
            this.EQUIP_ID.Name = "EQUIP_ID";
            this.EQUIP_ID.ReadOnly = true;
            // 
            // TOWER_NO
            // 
            this.TOWER_NO.HeaderText = "TOWER_NO";
            this.TOWER_NO.Name = "TOWER_NO";
            this.TOWER_NO.ReadOnly = true;
            // 
            // UID
            // 
            this.UID.HeaderText = "UID";
            this.UID.Name = "UID";
            this.UID.ReadOnly = true;
            // 
            // SID
            // 
            this.SID.HeaderText = "SID";
            this.SID.Name = "SID";
            this.SID.ReadOnly = true;
            // 
            // LOTID
            // 
            this.LOTID.HeaderText = "LOTID";
            this.LOTID.Name = "LOTID";
            this.LOTID.ReadOnly = true;
            // 
            // QTY
            // 
            this.QTY.HeaderText = "QTY";
            this.QTY.Name = "QTY";
            this.QTY.ReadOnly = true;
            // 
            // INCH_INFO
            // 
            this.INCH_INFO.HeaderText = "INCH_INFO";
            this.INCH_INFO.Name = "INCH_INFO";
            this.INCH_INFO.ReadOnly = true;
            // 
            // SYNC_INFO
            // 
            this.SYNC_INFO.HeaderText = "SYNC_INFO";
            this.SYNC_INFO.Name = "SYNC_INFO";
            this.SYNC_INFO.ReadOnly = true;
            // 
            // EMPLOYEE_NO
            // 
            this.EMPLOYEE_NO.HeaderText = "사번";
            this.EMPLOYEE_NO.Name = "EMPLOYEE_NO";
            this.EMPLOYEE_NO.ReadOnly = true;
            // 
            // btn_directory
            // 
            this.btn_directory.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_directory.Image = ((System.Drawing.Image)(resources.GetObject("btn_directory.Image")));
            this.btn_directory.Location = new System.Drawing.Point(920, 0);
            this.btn_directory.Name = "btn_directory";
            this.btn_directory.Size = new System.Drawing.Size(49, 37);
            this.btn_directory.TabIndex = 5;
            this.btn_directory.UseVisualStyleBackColor = true;
            this.btn_directory.Click += new System.EventHandler(this.btn_directory_Click);
            // 
            // frm_SyncHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1021, 450);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_SyncHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sync History";
            this.Load += new System.EventHandler(this.frm_SyncHistory_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SyncHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgv_SyncHistory;
        private System.Windows.Forms.Button btn_Excel;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.DateTimePicker dtp_to;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtp_from;
        private System.Windows.Forms.DataGridViewTextBoxColumn DATETIME;
        private System.Windows.Forms.DataGridViewTextBoxColumn EQUIP_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TOWER_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn UID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SID;
        private System.Windows.Forms.DataGridViewTextBoxColumn LOTID;
        private System.Windows.Forms.DataGridViewTextBoxColumn QTY;
        private System.Windows.Forms.DataGridViewTextBoxColumn INCH_INFO;
        private System.Windows.Forms.DataGridViewTextBoxColumn SYNC_INFO;
        private System.Windows.Forms.DataGridViewTextBoxColumn EMPLOYEE_NO;
        private System.Windows.Forms.Button btn_directory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}