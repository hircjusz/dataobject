namespace SoftwareMind.SimpleWorkflow.Designer
{
    partial class ScriptEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scriptTabPage2 = new System.Windows.Forms.TabPage();
            this.scriptTextBox = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.outputTabPage = new System.Windows.Forms.TabPage();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.validateButton = new System.Windows.Forms.Button();
            this.scriptTabPage2.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.outputTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // scriptTabPage2
            // 
            this.scriptTabPage2.Controls.Add(this.scriptTextBox);
            this.scriptTabPage2.Location = new System.Drawing.Point(4, 22);
            this.scriptTabPage2.Name = "scriptTabPage2";
            this.scriptTabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.scriptTabPage2.Size = new System.Drawing.Size(722, 292);
            this.scriptTabPage2.TabIndex = 1;
            this.scriptTabPage2.Text = "Edytor skryptów";
            this.scriptTabPage2.UseVisualStyleBackColor = true;
            // 
            // scriptTextBox
            // 
            this.scriptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptTextBox.Location = new System.Drawing.Point(3, 3);
            this.scriptTextBox.Multiline = true;
            this.scriptTextBox.Name = "scriptTextBox";
            this.scriptTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.scriptTextBox.Size = new System.Drawing.Size(716, 286);
            this.scriptTextBox.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.scriptTabPage2);
            this.tabControl.Controls.Add(this.outputTabPage);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(730, 318);
            this.tabControl.TabIndex = 0;
            // 
            // outputTabPage
            // 
            this.outputTabPage.Controls.Add(this.outputTextBox);
            this.outputTabPage.Location = new System.Drawing.Point(4, 22);
            this.outputTabPage.Name = "outputTabPage";
            this.outputTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.outputTabPage.Size = new System.Drawing.Size(469, 317);
            this.outputTabPage.TabIndex = 2;
            this.outputTabPage.Text = "Komunikaty";
            this.outputTabPage.UseVisualStyleBackColor = true;
            // 
            // outputTextBox
            // 
            this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextBox.Location = new System.Drawing.Point(3, 3);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.outputTextBox.Size = new System.Drawing.Size(463, 311);
            this.outputTextBox.TabIndex = 1;
            // 
            // validateButton
            // 
            this.validateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.validateButton.Location = new System.Drawing.Point(4, 324);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(122, 24);
            this.validateButton.TabIndex = 2;
            this.validateButton.Text = "Sprawdź poprawność";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.tabControl);
            this.Name = "ScriptEditor";
            this.Size = new System.Drawing.Size(730, 351);
            this.scriptTabPage2.ResumeLayout(false);
            this.scriptTabPage2.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.outputTabPage.ResumeLayout(false);
            this.outputTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage scriptTabPage2;
        private System.Windows.Forms.TextBox scriptTextBox;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage outputTabPage;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button validateButton;
    }
}
