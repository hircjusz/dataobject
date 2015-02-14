namespace SoftwareMind.SimpleWorkflow.Designer
{
    partial class VariableMapEditorDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VariableMapEditorDialog));
            this.mapDataGridView = new System.Windows.Forms.DataGridView();
            this.nameColumn = new PropertyPathColumn();
            this.typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pathColumn = new PropertyPathColumn();
            this.rowContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.removeToolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.mapDataGridView)).BeginInit();
            this.rowContextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapDataGridView
            // 
            this.mapDataGridView.AllowUserToAddRows = false;
            this.mapDataGridView.AllowUserToDeleteRows = false;
            this.mapDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mapDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mapDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameColumn,
            this.typeColumn,
            this.pathColumn});
            this.mapDataGridView.ContextMenuStrip = this.rowContextMenuStrip;
            this.mapDataGridView.Location = new System.Drawing.Point(0, 25);
            this.mapDataGridView.Name = "mapDataGridView";
            this.mapDataGridView.RowHeadersVisible = false;
            this.mapDataGridView.Size = new System.Drawing.Size(723, 478);
            this.mapDataGridView.TabIndex = 0;
            this.mapDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.mapDataGridView_CellBeginEdit);
            this.mapDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.mapDataGridView_CellEndEdit);
            this.mapDataGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.mapDataGridView_CellMouseDown);
            this.mapDataGridView.SelectionChanged += new System.EventHandler(this.mapDataGridView_SelectionChanged);
            this.mapDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mapDataGridView_KeyDown);
            // 
            // nameColumn
            // 
            this.nameColumn.FillWeight = 250F;
            this.nameColumn.HeaderText = "Parametr metody";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.nameColumn.VariablesDef = null;
            this.nameColumn.Width = 250;
            // 
            // typeColumn
            // 
            this.typeColumn.HeaderText = "Typ";
            this.typeColumn.Name = "typeColumn";
            this.typeColumn.ReadOnly = true;
            // 
            // pathColumn
            // 
            this.pathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.pathColumn.HeaderText = "Ścieżka";
            this.pathColumn.Name = "pathColumn";
            this.pathColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.pathColumn.VariablesDef = null;
            // 
            // rowContextMenuStrip
            // 
            this.rowContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.rowContextMenuStrip.Name = "rowContextMenuStrip";
            this.rowContextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.rowContextMenuStrip.Size = new System.Drawing.Size(176, 70);
            this.rowContextMenuStrip.Opened += new System.EventHandler(this.contextMenuStrip_Opened);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.addToolStripMenuItem.Text = "Dodaj nowy wiersz";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.removeToolStripMenuItem.Text = "Usuń wiersz";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.save.Location = new System.Drawing.Point(505, 509);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(100, 25);
            this.save.TabIndex = 2;
            this.save.Text = "Zapisz";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(611, 509);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(100, 25);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Anuluj";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripButton,
            this.removeToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(723, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addToolStripButton
            // 
            this.addToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("addToolStripButton.Image")));
            this.addToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addToolStripButton.Name = "addToolStripButton";
            this.addToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.addToolStripButton.Text = "Dodaj nowye wiersz";
            this.addToolStripButton.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripButton
            // 
            this.removeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeToolStripButton.Enabled = false;
            this.removeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("removeToolStripButton.Image")));
            this.removeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeToolStripButton.Name = "removeToolStripButton";
            this.removeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.removeToolStripButton.Text = "Unuń zaznaczony wiersz";
            this.removeToolStripButton.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // VariableMapEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 546);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.save);
            this.Controls.Add(this.mapDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "VariableMapEditorDialog";
            this.ShowInTaskbar = false;
            this.Text = "Edytor mapowania";
            ((System.ComponentModel.ISupportInitialize)(this.mapDataGridView)).EndInit();
            this.rowContextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView mapDataGridView;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.ContextMenuStrip rowContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private PropertyPathColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeColumn;
        private PropertyPathColumn pathColumn;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addToolStripButton;
        private System.Windows.Forms.ToolStripButton removeToolStripButton;
    }
}