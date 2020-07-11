using LPHP_Preprocessor;
using Syncfusion.WinForms.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace LPHP_UI
{
    public partial class LPHP_UI : SfForm
    {
        private string watchFolder = "";

        public LPHP_UI()
        {
            InitializeComponent();
            
            LPHPCompiler.DebugOutput = txbConsoleLog;

        }

        private void bgwCompiler_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(watchFolder))
                {
                    Dictionary<string, string> lphpFiles = new Dictionary<string, string>();

                    while (true)
                    {
                        foreach (string filePath in Directory.EnumerateFiles(watchFolder, "*.*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                if (Path.GetExtension(filePath) == ".lphp")
                                {
                                    using (var md5 = MD5.Create())
                                    {
                                        try
                                        {
                                            using (var stream = File.OpenRead(filePath))
                                            {
                                                byte[] md5Bytes = md5.ComputeHash(stream);

                                                string md5Hash = Encoding.UTF8.GetString(md5Bytes, 0, md5Bytes.Length);
                                                if (!lphpFiles.ContainsKey(md5Hash))
                                                {

                                                    foreach (KeyValuePair<string, string> entry in lphpFiles.ToArray())
                                                        if (entry.Value == filePath) lphpFiles[entry.Key] = null;

                                                    foreach (var item in lphpFiles.Where(kvp => kvp.Value == null).ToList())
                                                        lphpFiles.Remove(item.Key);

                                                    lphpFiles.Add(md5Hash, filePath);

                                                    Console.WriteLine($"\r\nChange detected in {filePath}...");
                                                    LPHPCompiler.Run(lphpFiles);

                                                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                                                    Console.ForegroundColor = ConsoleColor.White;
                                                    Console.WriteLine($"Compiled successfully!");
                                                    Console.BackgroundColor = ConsoleColor.Black;
                                                    Console.ForegroundColor = ConsoleColor.White;
                                                }
                                            }
                                        }
                                        catch (IOException)
                                        {
                                            LPHPCompiler.PrintWarning("Can't keep up! Compilation-Cycle skipped.");
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                LPHPCompiler.PrintWarning("Compilation aborted. Please fix all errors shown above and try again.");
                            }
                        }
                        Thread.Sleep(500);
                        Thread.Sleep(100);
                    }
                }
            }
            catch
            {
                LPHPCompiler.PrintError("*** Error reading the directory ***");
                LPHPCompiler.PrintError("Please make sure the given directory exists.");
            }

            Console.ReadKey();
            return;
        }
    }
}
