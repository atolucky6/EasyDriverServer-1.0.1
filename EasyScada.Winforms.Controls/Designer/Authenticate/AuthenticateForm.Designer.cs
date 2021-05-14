namespace EasyScada.Winforms.Controls
{
    partial class AuthenticateForm
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
            this.easyLogin1 = new EasyScada.Winforms.Controls.EasyLogin();
            this.SuspendLayout();
            // 
            // easyLogin1
            // 
            this.easyLogin1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.easyLogin1.Location = new System.Drawing.Point(4, 4);
            // 
            // 
            // 
            this.easyLogin1.LoginButton.Location = new System.Drawing.Point(3, 81);
            this.easyLogin1.LoginButton.Name = "btnLogin";
            this.easyLogin1.LoginButton.Size = new System.Drawing.Size(90, 23);
            this.easyLogin1.LoginButton.TabIndex = 3;
            this.easyLogin1.LoginButton.Text = "Login";
            this.easyLogin1.LoginButton.UseVisualStyleBackColor = true;
            this.easyLogin1.Margin = new System.Windows.Forms.Padding(4);
            this.easyLogin1.Name = "easyLogin1";
            // 
            // 
            // 
            this.easyLogin1.PasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLogin1.PasswordTextBox.Location = new System.Drawing.Point(3, 55);
            this.easyLogin1.PasswordTextBox.Name = "txbPassword";
            this.easyLogin1.PasswordTextBox.PasswordChar = '*';
            this.easyLogin1.PasswordTextBox.Size = new System.Drawing.Size(383, 20);
            this.easyLogin1.PasswordTextBox.TabIndex = 2;
            this.easyLogin1.Size = new System.Drawing.Size(389, 108);
            this.easyLogin1.TabIndex = 3;
            // 
            // 
            // 
            this.easyLogin1.UsernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.easyLogin1.UsernameTextBox.Location = new System.Drawing.Point(3, 16);
            this.easyLogin1.UsernameTextBox.Name = "txbUsername";
            this.easyLogin1.UsernameTextBox.Size = new System.Drawing.Size(383, 20);
            this.easyLogin1.UsernameTextBox.TabIndex = 1;
            // 
            // AuthenticateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 116);
            this.Controls.Add(this.easyLogin1);
            this.Name = "AuthenticateForm";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.Text = "Authenticate";
            this.ResumeLayout(false);

        }

        #endregion

        private EasyLogin easyLogin1;
    }
}