namespace LPHPUI
{
    partial class LPHPUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LPHPUI));
            this.rtbLogOutput = new System.Windows.Forms.RichTextBox();
            this.bgwLPHPCompiler = new System.ComponentModel.BackgroundWorker();
            this.btnStopPreprocessor = new System.Windows.Forms.Button();
            this.btnStartPreprocessor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbLogOutput
            // 
            this.rtbLogOutput.BackColor = System.Drawing.Color.Black;
            this.rtbLogOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLogOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtbLogOutput.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbLogOutput.ForeColor = System.Drawing.Color.White;
            this.rtbLogOutput.Location = new System.Drawing.Point(0, 182);
            this.rtbLogOutput.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.rtbLogOutput.Name = "rtbLogOutput";
            this.rtbLogOutput.ReadOnly = true;
            this.rtbLogOutput.Size = new System.Drawing.Size(684, 279);
            this.rtbLogOutput.TabIndex = 0;
            this.rtbLogOutput.Text = "";
            // 
            // bgwLPHPCompiler
            // 
            this.bgwLPHPCompiler.WorkerReportsProgress = true;
            this.bgwLPHPCompiler.WorkerSupportsCancellation = true;
            this.bgwLPHPCompiler.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwLPHPCompiler_DoWork);
            this.bgwLPHPCompiler.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwLPHPCompiler_ProgressChanged);
            // 
            // btnStopPreprocessor
            // 
            this.btnStopPreprocessor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.btnStopPreprocessor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopPreprocessor.Font = new System.Drawing.Font("Calibri Light", 16F);
            this.btnStopPreprocessor.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnStopPreprocessor.Location = new System.Drawing.Point(0, 97);
            this.btnStopPreprocessor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStopPreprocessor.Name = "btnStopPreprocessor";
            this.btnStopPreprocessor.Size = new System.Drawing.Size(144, 76);
            this.btnStopPreprocessor.TabIndex = 1;
            this.btnStopPreprocessor.Text = "Stop\r\nLPHP";
            this.btnStopPreprocessor.UseVisualStyleBackColor = false;
            // 
            // btnStartPreprocessor
            // 
            this.btnStartPreprocessor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.btnStartPreprocessor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartPreprocessor.Font = new System.Drawing.Font("Calibri Light", 16F);
            this.btnStartPreprocessor.ForeColor = System.Drawing.Color.LawnGreen;
            this.btnStartPreprocessor.Location = new System.Drawing.Point(0, 13);
            this.btnStartPreprocessor.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartPreprocessor.Name = "btnStartPreprocessor";
            this.btnStartPreprocessor.Size = new System.Drawing.Size(144, 76);
            this.btnStartPreprocessor.TabIndex = 1;
            this.btnStartPreprocessor.Text = "Start\r\nLPHP";
            this.btnStartPreprocessor.UseVisualStyleBackColor = false;
            this.btnStartPreprocessor.Click += new System.EventHandler(this.btnStartPreprocessor_Click);
            // 
            // LPHPUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 461);
            this.Controls.Add(this.btnStartPreprocessor);
            this.Controls.Add(this.btnStopPreprocessor);
            this.Controls.Add(this.rtbLogOutput);
            this.Font = new System.Drawing.Font("Calibri Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "LPHPUI";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.Style.InactiveShadowOpacity = ((byte)(0));
            this.Style.MdiChild.IconHorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.Style.MdiChild.IconVerticalAlignment = System.Windows.Forms.VisualStyles.VerticalAlignment.Center;
            this.Style.ShadowOpacity = ((byte)(0));
            this.Style.TitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.Style.TitleBar.BottomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.Style.TitleBar.CloseButtonForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(17)))), ((int)(((byte)(35)))));
            this.Style.TitleBar.ForeColor = System.Drawing.SystemColors.Control;
            this.Style.TitleBar.HelpButtonForeColor = System.Drawing.SystemColors.Control;
            this.Style.TitleBar.MaximizeButtonForeColor = System.Drawing.SystemColors.Control;
            this.Style.TitleBar.MaximizeButtonHoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(114)))), ((int)(((byte)(172)))));
            this.Style.TitleBar.MaximizeButtonPressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(135)))), ((int)(((byte)(195)))));
            this.Style.TitleBar.MinimizeButtonForeColor = System.Drawing.SystemColors.Control;
            this.Style.TitleBar.MinimizeButtonHoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(114)))), ((int)(((byte)(172)))));
            this.Style.TitleBar.MinimizeButtonPressedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(135)))), ((int)(((byte)(195)))));
            this.Text = "LPHP Layout Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LPHPUI_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbLogOutput;
        private System.ComponentModel.BackgroundWorker bgwLPHPCompiler;
        private System.Windows.Forms.Button btnStopPreprocessor;
        private System.Windows.Forms.Button btnStartPreprocessor;
    }
}

