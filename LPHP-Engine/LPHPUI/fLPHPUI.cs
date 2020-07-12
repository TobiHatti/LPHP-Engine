using LPHPCore;
using Syncfusion.WinForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LPHPUI
{
    public partial class LPHPUI : SfForm
    {
        private bool QueueLPHPRestart = false;

        public LPHPUI()
        {
            InitializeComponent();
            this.Style.Border = new Pen(Color.FromArgb(79, 93, 149), 2);
            this.Style.InactiveBorder = new Pen(Color.FromArgb(101, 114, 172), 2);
            ShowStartupBanner();

            LPHPCompiler.Init();

            try
            {
                if (LPHPCompiler.COMPOPT.ContainsKey("UI_LAST_PROJECT_PATH")) txbProjectDirectory.Text = LPHPCompiler.COMPOPT["UI_LAST_PROJECT_PATH"].ToString();
                else txbProjectDirectory.Text = "";

                if (LPHPCompiler.COMPOPT.ContainsKey("REMOVE_HTML_COMMENTS") && Convert.ToBoolean(LPHPCompiler.COMPOPT["REMOVE_HTML_COMMENTS"])) tglRemoveHTMLComments.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else tglRemoveHTMLComments.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                if (LPHPCompiler.COMPOPT.ContainsKey("MIN_OUTPUT_ENABLED") && Convert.ToBoolean(LPHPCompiler.COMPOPT["MIN_OUTPUT_ENABLED"])) tglEnableMinOutput.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else tglEnableMinOutput.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                if (LPHPCompiler.COMPOPT.ContainsKey("XML_OUTPUT_ENABLED") && Convert.ToBoolean(LPHPCompiler.COMPOPT["XML_OUTPUT_ENABLED"])) tglEnableXMLOutput.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active;
                else tglEnableXMLOutput.ToggleState = Syncfusion.Windows.Forms.Tools.ToggleButtonState.Inactive;

                if (LPHPCompiler.COMPOPT.ContainsKey("ENABLE_CONSOLE_LOG")) chbSaveConsoleLog.Checked = Convert.ToBoolean(LPHPCompiler.COMPOPT["ENABLE_CONSOLE_LOG"]);
            }
            catch { }
        }

        private void LPHPUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwLPHPCompiler.WorkerSupportsCancellation)
                bgwLPHPCompiler.CancelAsync();
        }

        private void ShowStartupBanner()
        {
            AppendText("=====================================================", Color.Cyan);
            AppendText("*                 LPHP Preprocessor                 *", Color.Cyan);
            AppendText("*                Version " + typeof(LPHPCore.LPHPCompiler).Assembly.GetName().Version.ToString(3) + " ALPHA                *", Color.Cyan);
            AppendText("*        (c) Copyright 2020 Tobias Hattinger        *", Color.Cyan);
            AppendText("*                                                   *", Color.Cyan);
            AppendText("*                       Visit                       *", Color.Cyan);
            AppendText("*              https:/endev.at/p/LPHP               *", Color.Cyan);
            AppendText("*                    for updates                    *", Color.Cyan);
            AppendText("=====================================================\r\n\r\n", Color.Cyan);
        }

        private void bgwLPHPCompiler_DoWork(object sender, DoWorkEventArgs e)
        {
            // Initialize LPHP-Compiler
            LPHPCompiler.Init();

            // Initialize LPHP-Watchdog
            LPHPWatchdog.Init(txbProjectDirectory.Text);

            // Set LPHP-Debug Mode
            LPHPDebugger.PrintDebug = DebugToTxb;

            // Enable the creation of a log file
            LPHPDebugger.CreateLogFile = true;
            while(true)
            {
                if (bgwLPHPCompiler.CancellationPending) 
                    return;

                if(QueueLPHPRestart)
                {
                    bgwLPHPCompiler.ReportProgress(0, new Tuple<string, Color>("Reloading LPHP-Config...", Color.Yellow));
                    LPHPCompiler.Init();
                    LPHPWatchdog.Init(txbProjectDirectory.Text);
                    QueueLPHPRestart = false;
                }

                // Run the LPHP-Watchdog on the given directory
                int watchdogResult = LPHPWatchdog.RunOnce();

                if(watchdogResult == -1)
                {
                    bgwLPHPCompiler.ReportProgress(0, new Tuple<string, Color>("Stopping LPHP-Engine.", Color.OrangeRed));
                    bgwLPHPCompiler.CancelAsync();
                }

                if(watchdogResult == -2)
                {
                    bgwLPHPCompiler.ReportProgress(0, new Tuple<string, Color>("Problem detected. Halting LPHP for 5 seconds.", Color.OrangeRed));
                    Thread.Sleep(5000);
                }
            }
            
        }

        private void DebugToTxb(string pMessage, LPHPMessageType pType)
        {
            Color foreColor = Color.White;

            switch (pType)
            {
                case LPHPMessageType.LPHPSuccess:
                    foreColor = Color.Lime;
                    break;
                case LPHPMessageType.LPHPWarning:
                    foreColor = Color.OrangeRed;
                    break;
                case LPHPMessageType.LPHPError:
                    foreColor = Color.Red;
                    break;
            }

            bgwLPHPCompiler.ReportProgress(0, new Tuple<string, Color>(pMessage, foreColor));


            LPHPDebugger.LogDebugData(pMessage.Replace(Environment.NewLine, ""), pType);
        }

        private void bgwLPHPCompiler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<string, Color> reportData = e.UserState as Tuple<string, Color>;

            AppendText(reportData.Item1, reportData.Item2);
        }

        private void AppendText(string text, Color foreColor)
        {
            if (rtbLogOutput.TextLength > 210000000)
            {
                rtbLogOutput.Clear();
                AppendText("Console cleared. Max length exeeded.", Color.Yellow);
            }

            rtbLogOutput.SelectionStart = rtbLogOutput.TextLength;
            rtbLogOutput.SelectionLength = 0;

            rtbLogOutput.SelectionColor = foreColor;
            rtbLogOutput.AppendText(text + Environment.NewLine);
            rtbLogOutput.SelectionColor = rtbLogOutput.ForeColor;

            rtbLogOutput.SelectionStart = rtbLogOutput.Text.Length;
            rtbLogOutput.ScrollToCaret();
        }

        private void btnStartPreprocessor_Click(object sender, EventArgs e)
        {
            AppendText($"Starting LPHP-Engine in \"{txbProjectDirectory.Text}\"", Color.Lime);

            if (!bgwLPHPCompiler.IsBusy)
                bgwLPHPCompiler.RunWorkerAsync();
            else
                AppendText("LPHP-Engine is already running.", Color.OrangeRed);
        }

        private void btnStopPreprocessor_Click(object sender, EventArgs e)
        {
            if (bgwLPHPCompiler.IsBusy)
            {
                AppendText("Stopping LPHP-Engine.", Color.OrangeRed);
                bgwLPHPCompiler.CancelAsync();
            }
            else AppendText("LPHP-Engine is not running.", Color.OrangeRed);
        }

        private void btnBrowseDirectories_Click(object sender, EventArgs e)
        {
            if(fbdFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                txbProjectDirectory.Text = fbdFolderBrowser.SelectedPath;
                LPHPCompiler.COMPOPT["UI_LAST_PROJECT_PATH"] = fbdFolderBrowser.SelectedPath;
                LPHPCompiler.SaveConfig();
            }
        }

        private void tglRemoveHTMLComments_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            QueueLPHPRestart = true;
            LPHPCompiler.COMPOPT["REMOVE_HTML_COMMENTS"] = (tglRemoveHTMLComments.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
            LPHPCompiler.SaveConfig();
        }

        private void tglEnableMinOutput_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            QueueLPHPRestart = true;
            LPHPCompiler.COMPOPT["MIN_OUTPUT_ENABLED"] = (tglEnableMinOutput.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
            LPHPCompiler.SaveConfig();
        }

        private void tglEnableXMLOutput_ToggleStateChanged(object sender, Syncfusion.Windows.Forms.Tools.ToggleStateChangedEventArgs e)
        {
            QueueLPHPRestart = true;
            LPHPCompiler.COMPOPT["XML_OUTPUT_ENABLED"] = (tglEnableXMLOutput.ToggleState == Syncfusion.Windows.Forms.Tools.ToggleButtonState.Active);
            LPHPCompiler.SaveConfig();
        }

        private void chbSaveConsoleLog_CheckedChanged(object sender, EventArgs e)
        {
            QueueLPHPRestart = true;
            LPHPCompiler.COMPOPT["ENABLE_CONSOLE_LOG"] = chbSaveConsoleLog.Checked;
            LPHPCompiler.SaveConfig();
        }

        private void btnClearConsoleLog_Click(object sender, EventArgs e)
        {
            rtbLogOutput.Clear();
            AppendText("Console cleared.", Color.Yellow);
        }

        private void btnCopyConsoleLog_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbLogOutput.Text);
            AppendText("Console log coppied to clipboard.", Color.Yellow);
        }

        private void btnOpenProjectLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txbProjectDirectory.Text))
            {
                if (File.Exists(Path.Combine(txbProjectDirectory.Text, LPHPDebugger.LPHPLogFile)))
                {
                    Process.Start(Path.Combine(txbProjectDirectory.Text, LPHPDebugger.LPHPLogFile));
                }
                else
                {
                    MessageBox.Show("No log file was found. Enable logging to save LPHP log-files.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid propject folder.", "No folder selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnOpenProjectFolder_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txbProjectDirectory.Text))
            {
                if(Directory.Exists(txbProjectDirectory.Text))
                {
                    Process.Start(txbProjectDirectory.Text);
                }
                else
                {
                    MessageBox.Show("The selected directory could not be opened.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid propject folder.", "No folder selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
