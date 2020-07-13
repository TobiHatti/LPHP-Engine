using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


//LPHP Layout Engine
//Copyright(C) 2020 Tobias Hattinger

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<https://www.gnu.org/licenses/>.
namespace LPHPCore
{
    /// <summary>
    /// Watches for changes in the selected directory and invokes the compiler when changes are detected.
    /// </summary>
    public class LPHPWatchdog
    {
        /// <summary>
        /// Root-Path of the project
        /// </summary>
        public static string ProjectRoot { get; set; } = "";

        private static Dictionary<string, string> lphpFiles;

        /// <summary>
        /// Runs the watchdog until the program is terminated
        /// </summary>
        public static void Run()
            => Run(true);

        /// <summary>
        /// Runs the watchdog once and returns the result-state from the watchdog
        /// </summary>
        /// <returns>Result of the watchdog-cycle</returns>
        public static int RunOnce()
            => Run(false);

        /// <summary>
        /// Initializes the watchdog and sets the target folder
        /// </summary>
        /// <param name="pWatchFolder">Root-folder of the LPHP-project</param>
        public static void Init(string pWatchFolder)
        {
            ProjectRoot = pWatchFolder;
            lphpFiles = new Dictionary<string, string>();
        }

        /// <summary>
        /// Constantly checks the given directory for changes and re-compiles as soon as a change is detected.
        /// </summary>
        /// <param name="pWatchFolder">Folder to watch</param>
        private static int Run(bool pRunInfinite = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(ProjectRoot))
                {
                    if(pRunInfinite) lphpFiles = new Dictionary<string, string>();

                    do
                    {
                        foreach (string filePath in Directory.EnumerateFiles(ProjectRoot, "*.*", SearchOption.AllDirectories))
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

                                                    LPHPDebugger.PrintMessage($"\r\nChange detected in {filePath}...");
                                                    LPHPCompiler.Run(lphpFiles);
                                                    LPHPDebugger.PrintSuccess($"Compiled successfully!");
                                                }
                                            }
                                        }
                                        catch (IOException)
                                        {
                                            LPHPDebugger.PrintWarning("Can't keep up! Compilation-Cycle skipped.");
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                LPHPDebugger.PrintWarning("Compilation aborted. Please fix all errors shown above and try again.");
                                if (!pRunInfinite) return -2;
                            }
                        }
                        Thread.Sleep(100);
                    }
                    while (pRunInfinite);
                }
                else
                {
                    LPHPDebugger.PrintError("*** LPHP Watchdog Error ***");
                    LPHPDebugger.PrintError("Please provide a path to the target folder and try again.");
                    if (!pRunInfinite) return -1;
                }
            }
            catch
            {
                LPHPDebugger.PrintError("*** Error reading the directory ***");
                LPHPDebugger.PrintError("Please make sure the given directory exists.");
                if (!pRunInfinite) return -1;
            }

            return 0;
        }
    }
}