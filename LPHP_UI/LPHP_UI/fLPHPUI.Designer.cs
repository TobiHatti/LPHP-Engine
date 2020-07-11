namespace LPHP_UI
{
    partial class LPHP_UI
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
            this.txbConsoleLog = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.bgwCompiler = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.txbConsoleLog)).BeginInit();
            this.SuspendLayout();
            // 
            // txbConsoleLog
            // 
            this.txbConsoleLog.BackColor = System.Drawing.Color.Black;
            this.txbConsoleLog.BeforeTouchSize = new System.Drawing.Size(640, 155);
            this.txbConsoleLog.Location = new System.Drawing.Point(5, 239);
            this.txbConsoleLog.Multiline = true;
            this.txbConsoleLog.Name = "txbConsoleLog";
            this.txbConsoleLog.Size = new System.Drawing.Size(640, 155);
            this.txbConsoleLog.TabIndex = 0;
            // 
            // bgwCompiler
            // 
            this.bgwCompiler.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwCompiler_DoWork);
            // 
            // LPHP_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 399);
            this.Controls.Add(this.txbConsoleLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LPHP_UI";
            this.Style.InactiveShadowOpacity = ((byte)(30));
            this.Style.MdiChild.IconHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Style.MdiChild.IconVerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.Style.MdiChild.TitleBarBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.Style.ShadowOpacity = ((byte)(60));
            this.Style.TitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.Style.TitleBar.BottomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.Text = "LPHP";
            ((System.ComponentModel.ISupportInitialize)(this.txbConsoleLog)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Syncfusion.Windows.Forms.Tools.TextBoxExt txbConsoleLog;
        private System.ComponentModel.BackgroundWorker bgwCompiler;
    }
}

