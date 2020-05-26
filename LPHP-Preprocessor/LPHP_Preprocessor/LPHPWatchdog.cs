using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LPHP_Preprocessor
{
    class LPHPWatchdog
    {
        static void Main(string[] args)
        {
            Console.Title = "LPHP Preprocessor - Version " + typeof(LPHPCompiler).Assembly.GetName().Version.ToString(3);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("=================================================");
            Console.WriteLine("                LPHP Preprocessor                ");
            Console.WriteLine("               Version " + typeof(LPHPCompiler).Assembly.GetName().Version.ToString(3) + " ALPHA");
            Console.WriteLine("       (c) Copyright 2020 Tobias Hattinger       ");
            Console.WriteLine("                                                 ");
            Console.WriteLine("                      Visit                      ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine("https://github.com/TobiHatti/LPHP-Engine/releases");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("                   for updates                   ");
            Console.WriteLine("=================================================\r\n\r\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            string watchFolder = "";
            try
            {
                watchFolder = args[0];
                Console.Write("Watching directory \"");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(watchFolder);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\"");
            }
            catch(IndexOutOfRangeException)
            {
                LPHPCompiler.PrintError("*** LPHP Startup Error ***");
                LPHPCompiler.PrintError("Please provide a path to the target folder and try again.");
            }

#if DEBUG
            watchFolder = @"H:\LPHPTest\";
#endif
            try
            {
                if (!string.IsNullOrEmpty(watchFolder))
                {

                    Dictionary<string, string> lphpFiles = new Dictionary<string, string>();

                    while (true)
                    {
                        foreach (string filePath in Directory.EnumerateFiles(watchFolder, "*.*", SearchOption.AllDirectories))
                        {
#if !DEBUG
                        try
                        {
#endif
                            if (Path.GetExtension(filePath) == ".lphp")
                            {
                                using (var md5 = MD5.Create())
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
                            }
#if !DEBUG
                        }
                        catch
                        {
                            LPHPCompiler.PrintWarning("Compilation aborted. Please fix all errors shown above and try again.");
                        }
#endif
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch(Exception)
            {
                LPHPCompiler.PrintError("*** Error reading the directory ***");
                LPHPCompiler.PrintError("Please make sure the given directory exists.");
            }

            Console.ReadKey();
            return;
        }
    }
}
