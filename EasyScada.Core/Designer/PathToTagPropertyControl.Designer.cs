﻿namespace EasyScada.Core.Designer
{
    partial class PathToTagPropertyControl
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
            this.searchControl = new System.Windows.Forms.Integration.ElementHost();
            this.searchListBox1 = new EasyScada.Core.SearchListBox();
            this.SuspendLayout();
            // 
            // searchControl
            // 
            this.searchControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchControl.Location = new System.Drawing.Point(0, 0);
            this.searchControl.Name = "searchControl";
            this.searchControl.Size = new System.Drawing.Size(224, 302);
            this.searchControl.TabIndex = 0;
            this.searchControl.Text = "elementHost1";
            this.searchControl.Child = this.searchListBox1;
            // 
            // PathToTagPropertyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.searchControl);
            this.Name = "PathToTagPropertyControl";
            this.Size = new System.Drawing.Size(224, 302);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost searchControl;
        private SearchListBox searchListBox1;
    }
}
