using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LPHP_Preprocessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string watchFolder;
            try
            {
                watchFolder = args[1];
            }
            catch(IndexOutOfRangeException)
            {
                Console.WriteLine("Could not start the LPHP-Preprocessor. Please provide a path to the target folder.");
            }

#if DEBUG
            watchFolder = @"H:\LPHPTest\";
#endif

            Dictionary<string, string> lphpFiles = new Dictionary<string, string>();

            while (true)
            {
                foreach (string filePath in Directory.GetFiles(watchFolder))
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

                                        Console.WriteLine($"Change detected in {filePath}!");

                                        LPHPCompiler.Run(lphpFiles);
                                    }
                                }
                            }
                        }
#if !DEBUG
                    }
                    catch { }
#endif
                }
                Thread.Sleep(100);
            }
        }
    }
}
