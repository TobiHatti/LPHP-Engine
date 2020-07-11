using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace LPHPCore
{
    public class LPHPWatchdog
    {
        /// <summary>
        /// Constantly checks the given directory for changes and re-compiles as soon as a change is detected.
        /// </summary>
        /// <param name="pWatchFolder">Folder to watch</param>
        public static void Run(string pWatchFolder)
        {
            try
            {
                LPHPCompiler.Init();
            }
            catch
            {
                Console.ReadKey();
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(pWatchFolder))
                {
                    Dictionary<string, string> lphpFiles = new Dictionary<string, string>();

                    while (true)
                    {
                        foreach (string filePath in Directory.EnumerateFiles(pWatchFolder, "*.*", SearchOption.AllDirectories))
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
                            }
                        }
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    LPHPDebugger.PrintError("*** LPHP Watchdog Error ***");
                    LPHPDebugger.PrintError("Please provide a path to the target folder and try again.");
                }
            }
            catch
            {
                LPHPDebugger.PrintError("*** Error reading the directory ***");
                LPHPDebugger.PrintError("Please make sure the given directory exists.");
            }

            return;
        }
    }
}