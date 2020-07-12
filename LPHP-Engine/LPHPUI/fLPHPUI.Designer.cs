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
            Syncfusion.Windows.Forms.Tools.SliderCollection sliderCollection1 = new Syncfusion.Windows.Forms.Tools.SliderCollection();
            Syncfusion.Windows.Forms.Tools.SliderCollection sliderCollection2 = new Syncfusion.Windows.Forms.Tools.SliderCollection();
            Syncfusion.Windows.Forms.Tools.SliderCollection sliderCollection3 = new Syncfusion.Windows.Forms.Tools.SliderCollection();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LPHPUI));
            this.rtbLogOutput = new System.Windows.Forms.RichTextBox();
            this.bgwLPHPCompiler = new System.ComponentModel.BackgroundWorker();
            this.btnStopPreprocessor = new System.Windows.Forms.Button();
            this.btnStartPreprocessor = new System.Windows.Forms.Button();
            this.tglRemoveHTMLComments = new Syncfusion.Windows.Forms.Tools.ToggleButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txbProjectDirectory = new System.Windows.Forms.TextBox();
            this.btnBrowseDirectories = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tglEnableMinOutput = new Syncfusion.Windows.Forms.Tools.ToggleButton();
            this.tglEnableXMLOutput = new Syncfusion.Windows.Forms.Tools.ToggleButton();
            this.fbdFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.tglRemoveHTMLComments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tglEnableMinOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tglEnableXMLOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbLogOutput
            // 
            this.rtbLogOutput.BackColor = System.Drawing.Color.Black;
            this.rtbLogOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLogOutput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtbLogOutput.Font = new System.Drawing.Font("Consolas", 10F);
            this.rtbLogOutput.ForeColor = System.Drawing.Color.White;
            this.rtbLogOutput.Location = new System.Drawing.Point(0, 112);
            this.rtbLogOutput.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.rtbLogOutput.Name = "rtbLogOutput";
            this.rtbLogOutput.ReadOnly = true;
            this.rtbLogOutput.Size = new System.Drawing.Size(684, 284);
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
            this.btnStopPreprocessor.Font = new System.Drawing.Font("Calibri Light", 14F);
            this.btnStopPreprocessor.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnStopPreprocessor.Location = new System.Drawing.Point(0, 66);
            this.btnStopPreprocessor.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopPreprocessor.Name = "btnStopPreprocessor";
            this.btnStopPreprocessor.Size = new System.Drawing.Size(144, 33);
            this.btnStopPreprocessor.TabIndex = 1;
            this.btnStopPreprocessor.Text = "Stop LPHP";
            this.btnStopPreprocessor.UseVisualStyleBackColor = false;
            this.btnStopPreprocessor.Click += new System.EventHandler(this.btnStopPreprocessor_Click);
            // 
            // btnStartPreprocessor
            // 
            this.btnStartPreprocessor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.btnStartPreprocessor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartPreprocessor.Font = new System.Drawing.Font("Calibri Light", 16F);
            this.btnStartPreprocessor.ForeColor = System.Drawing.Color.LawnGreen;
            this.btnStartPreprocessor.Location = new System.Drawing.Point(0, 12);
            this.btnStartPreprocessor.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartPreprocessor.Name = "btnStartPreprocessor";
            this.btnStartPreprocessor.Size = new System.Drawing.Size(144, 46);
            this.btnStartPreprocessor.TabIndex = 1;
            this.btnStartPreprocessor.Text = "Start LPHP";
            this.btnStartPreprocessor.UseVisualStyleBackColor = false;
            this.btnStartPreprocessor.Click += new System.EventHandler(this.btnStartPreprocessor_Click);
            // 
            // tglRemoveHTMLComments
            // 
            this.tglRemoveHTMLComments.CanOverrideStyle = true;
            this.tglRemoveHTMLComments.DisplayMode = Syncfusion.Windows.Forms.Tools.DisplayType.Image;
            this.tglRemoveHTMLComments.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tglRemoveHTMLComments.ForeColor = System.Drawing.Color.Black;
            this.tglRemoveHTMLComments.Location = new System.Drawing.Point(334, 12);
            this.tglRemoveHTMLComments.MinimumSize = new System.Drawing.Size(52, 20);
            this.tglRemoveHTMLComments.Name = "tglRemoveHTMLComments";
            this.tglRemoveHTMLComments.Size = new System.Drawing.Size(61, 25);
            this.tglRemoveHTMLComments.Slider = sliderCollection1;
            this.tglRemoveHTMLComments.TabIndex = 4;
            this.tglRemoveHTMLComments.Text = "toggleButton1";
            this.tglRemoveHTMLComments.ThemeName = "Office2016Colorful";
            this.tglRemoveHTMLComments.ThemeStyle.ActiveBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglRemoveHTMLComments.ThemeStyle.ActiveBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglRemoveHTMLComments.ThemeStyle.ActiveHoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglRemoveHTMLComments.ThemeStyle.ActiveHoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglRemoveHTMLComments.ThemeStyle.ToggleButttonSliderStyle.BorderThickness = 1;
            this.tglRemoveHTMLComments.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            this.tglRemoveHTMLComments.VisualStyle = Syncfusion.Windows.Forms.Tools.ToggleButtonStyle.Office2016Colorful;
            this.tglRemoveHTMLComments.ToggleStateChanged += new Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventHandler(this.tglRemoveHTMLComments_ToggleStateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Remove HTML comments";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(196, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 19);
            this.label2.TabIndex = 5;
            this.label2.Text = "Enable Min-Output";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(193, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 19);
            this.label3.TabIndex = 5;
            this.label3.Text = "Enable XML-Output";
            // 
            // txbProjectDirectory
            // 
            this.txbProjectDirectory.Location = new System.Drawing.Point(401, 43);
            this.txbProjectDirectory.Name = "txbProjectDirectory";
            this.txbProjectDirectory.ReadOnly = true;
            this.txbProjectDirectory.Size = new System.Drawing.Size(271, 27);
            this.txbProjectDirectory.TabIndex = 6;
            // 
            // btnBrowseDirectories
            // 
            this.btnBrowseDirectories.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.btnBrowseDirectories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseDirectories.Font = new System.Drawing.Font("Calibri Light", 12F);
            this.btnBrowseDirectories.ForeColor = System.Drawing.Color.White;
            this.btnBrowseDirectories.Location = new System.Drawing.Point(520, 73);
            this.btnBrowseDirectories.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseDirectories.Name = "btnBrowseDirectories";
            this.btnBrowseDirectories.Size = new System.Drawing.Size(151, 27);
            this.btnBrowseDirectories.TabIndex = 7;
            this.btnBrowseDirectories.Text = "Browse directories";
            this.btnBrowseDirectories.UseVisualStyleBackColor = false;
            this.btnBrowseDirectories.Click += new System.EventHandler(this.btnBrowseDirectories_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(543, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "LPHP Project Path:";
            // 
            // tglEnableMinOutput
            // 
            this.tglEnableMinOutput.CanOverrideStyle = true;
            this.tglEnableMinOutput.DisplayMode = Syncfusion.Windows.Forms.Tools.DisplayType.Image;
            this.tglEnableMinOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tglEnableMinOutput.ForeColor = System.Drawing.Color.Black;
            this.tglEnableMinOutput.Location = new System.Drawing.Point(334, 43);
            this.tglEnableMinOutput.MinimumSize = new System.Drawing.Size(52, 20);
            this.tglEnableMinOutput.Name = "tglEnableMinOutput";
            this.tglEnableMinOutput.Size = new System.Drawing.Size(61, 25);
            this.tglEnableMinOutput.Slider = sliderCollection2;
            this.tglEnableMinOutput.TabIndex = 4;
            this.tglEnableMinOutput.Text = "toggleButton1";
            this.tglEnableMinOutput.ThemeName = "Office2016Colorful";
            this.tglEnableMinOutput.ThemeStyle.ActiveBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableMinOutput.ThemeStyle.ActiveBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableMinOutput.ThemeStyle.ActiveHoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableMinOutput.ThemeStyle.ActiveHoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableMinOutput.ThemeStyle.ToggleButttonSliderStyle.BorderThickness = 1;
            this.tglEnableMinOutput.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            this.tglEnableMinOutput.VisualStyle = Syncfusion.Windows.Forms.Tools.ToggleButtonStyle.Office2016Colorful;
            this.tglEnableMinOutput.ToggleStateChanged += new Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventHandler(this.tglEnableMinOutput_ToggleStateChanged);
            // 
            // tglEnableXMLOutput
            // 
            this.tglEnableXMLOutput.CanOverrideStyle = true;
            this.tglEnableXMLOutput.DisplayMode = Syncfusion.Windows.Forms.Tools.DisplayType.Image;
            this.tglEnableXMLOutput.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tglEnableXMLOutput.ForeColor = System.Drawing.Color.Black;
            this.tglEnableXMLOutput.Location = new System.Drawing.Point(334, 74);
            this.tglEnableXMLOutput.MinimumSize = new System.Drawing.Size(52, 20);
            this.tglEnableXMLOutput.Name = "tglEnableXMLOutput";
            this.tglEnableXMLOutput.Size = new System.Drawing.Size(61, 25);
            this.tglEnableXMLOutput.Slider = sliderCollection3;
            this.tglEnableXMLOutput.TabIndex = 4;
            this.tglEnableXMLOutput.Text = "toggleButton1";
            this.tglEnableXMLOutput.ThemeName = "Office2016Colorful";
            this.tglEnableXMLOutput.ThemeStyle.ActiveBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableXMLOutput.ThemeStyle.ActiveBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableXMLOutput.ThemeStyle.ActiveHoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableXMLOutput.ThemeStyle.ActiveHoverBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(93)))), ((int)(((byte)(149)))));
            this.tglEnableXMLOutput.ThemeStyle.ToggleButttonSliderStyle.BorderThickness = 1;
            this.tglEnableXMLOutput.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
            this.tglEnableXMLOutput.VisualStyle = Syncfusion.Windows.Forms.Tools.ToggleButtonStyle.Office2016Colorful;
            this.tglEnableXMLOutput.ToggleStateChanged += new Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventHandler(this.tglEnableXMLOutput_ToggleStateChanged);
            // 
            // LPHPUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 396);
            this.Controls.Add(this.btnBrowseDirectories);
            this.Controls.Add(this.txbProjectDirectory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tglEnableXMLOutput);
            this.Controls.Add(this.tglEnableMinOutput);
            this.Controls.Add(this.tglRemoveHTMLComments);
            this.Controls.Add(this.btnStartPreprocessor);
            this.Controls.Add(this.btnStopPreprocessor);
            this.Controls.Add(this.rtbLogOutput);
            this.Font = new System.Drawing.Font("Calibri Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IconSize = new System.Drawing.Size(30, 30);
            this.Margin = new System.Windows.Forms.Padding(4);
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
            ((System.ComponentModel.ISupportInitialize)(this.tglRemoveHTMLComments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tglEnableMinOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tglEnableXMLOutput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbLogOutput;
        private System.ComponentModel.BackgroundWorker bgwLPHPCompiler;
        private System.Windows.Forms.Button btnStopPreprocessor;
        private System.Windows.Forms.Button btnStartPreprocessor;
        private Syncfusion.Windows.Forms.Tools.ToggleButton tglRemoveHTMLComments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txbProjectDirectory;
        private System.Windows.Forms.Button btnBrowseDirectories;
        private System.Windows.Forms.Label label4;
        private Syncfusion.Windows.Forms.Tools.ToggleButton tglEnableMinOutput;
        private Syncfusion.Windows.Forms.Tools.ToggleButton tglEnableXMLOutput;
        private System.Windows.Forms.FolderBrowserDialog fbdFolderBrowser;
    }
}

