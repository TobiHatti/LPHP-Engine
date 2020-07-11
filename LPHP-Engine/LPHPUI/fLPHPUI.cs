using LPHPCore;
using Syncfusion.WinForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LPHPUI
{
    public partial class LPHPUI : SfForm
    {
        public LPHPUI()
        {
            InitializeComponent();
            this.Style.Border = new Pen(Color.FromArgb(79, 93, 149), 2);
            this.Style.InactiveBorder = new Pen(Color.FromArgb(101, 114, 172), 2);
            ShowStartupBanner();
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
            LPHPWatchdog.Init();

            // Set LPHP-Debug Mode
            LPHPDebugger.PrintDebug = DebugToTxb;

            // Enable the creation of a log file
            LPHPDebugger.CreateLogFile = true;
            while(true)
            {
                if (bgwLPHPCompiler.CancellationPending) 
                    return;

                // Run the LPHP-Watchdog on the given directory
                LPHPWatchdog.RunOnce(txbProjectDirectory.Text);
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
        }

        private void bgwLPHPCompiler_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Tuple<string, Color> reportData = e.UserState as Tuple<string, Color>;

            AppendText(reportData.Item1, reportData.Item2);
        }

        private void AppendText(string text, Color foreColor)
        {
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
            AppendText("Starting LPHP-Engine in \"Directory Here\"", Color.Lime);

            if (!bgwLPHPCompiler.IsBusy)
                bgwLPHPCompiler.RunWorkerAsync();
            else
                AppendText("LPHP-Engine is already running", Color.OrangeRed);
        }

        private void btnStopPreprocessor_Click(object sender, EventArgs e)
        {
            AppendText("Stopping LPHP-Engine...", Color.OrangeRed);
            bgwLPHPCompiler.CancelAsync();
        }
    }
}
