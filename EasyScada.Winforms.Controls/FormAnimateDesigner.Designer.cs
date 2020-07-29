namespace EasyScada.Winforms.Controls
{
    partial class s
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
            this.easyManager1 = new EasyScada.Winforms.Controls.EasyManager(this.components);
            this.easyPalette1 = new EasyScada.Winforms.Controls.EasyPalette(this.components);
            this.SuspendLayout();
            // 
            // easyManager1
            // 
            this.easyManager1.GlobalPalette = this.easyPalette1;
            this.easyManager1.GlobalPaletteMode = EasyScada.Winforms.Controls.PaletteModeManager.Custom;
            // 
            // easyPalette1
            // 
            this.easyPalette1.BasePaletteMode = EasyScada.Winforms.Controls.PaletteMode.Office2010Silver;
            // 
            // s
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 545);
            this.Name = "s";
            this.Text = "FormAnimateDesigner";
            this.ResumeLayout(false);

        }

        #endregion

        private EasyManager easyManager1;
        private EasyPalette easyPalette1;
    }
}