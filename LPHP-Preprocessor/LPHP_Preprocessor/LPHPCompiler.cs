using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LPHP_Preprocessor
{
    class LPHPCompiler
    {
        public static void Run(Dictionary<string, string> lphpFiles)
        {
            foreach(KeyValuePair<string, string> file in lphpFiles)
            {
                // Load file into Memory
                string fileContent;
                using (StreamReader sr = new StreamReader(file.Value))
                {
                    fileContent = sr.ReadToEnd();
                    sr.Close();
                }

                // Check for NoCompile-Flag
                if (Regex.IsMatch(fileContent, @"\$\$\{[\S\s]*?NoCompile\s*?=\s*?(true|false);[\S\s]*?\}")) continue;


                // Filter RenderPage-Command:
                // \$\$RenderPage\(\"[\S\s]*?\"\)
                // Action: Look inside file and paste into source.

                // Filter Layout-Section:
                // ...
                // Action: get body-part and save temporary

                


                // Step 1: Merge all linked files into 1
                //LPHPCompiler.FileMerger(file.Value);
            }

        }


        private static bool FileMerger(string pInitialFilePath, StreamWriter pFileStream = null)
        {
            bool initialFile = false;
            if(pFileStream == null)
            {
                // Target file (temp)
                StreamWriter sw = new StreamWriter("journal.lphp-temp");

                initialFile = true;
            }

            string prepFile = LPHPSourcePrep(pInitialFilePath);

            // Check for NoCompile-Flag
            if (initialFile)
            {
                
            }




            // ...






            return true;
        }

        private static string LPHPSourcePrep(string pRawFilePath)
        {
            StreamReader sr = new StreamReader(pRawFilePath);
            StringBuilder sb = new StringBuilder();
            string line;

            // Read each line and remove comments
            while ((line = sr.ReadLine()) != null)
                sb.Append(Regex.Replace(line, @"\/\*[\s\S]*?\*\/|\/\/.*", ""));
            sr.Close();

            // Remove Tabs and Line-Breaks
            return sb.ToString().Replace("\t", "").Replace("\r\n", "");

        }

        private static string LPHPCommandExtracor(string pFileContent)
        {
            // Filter LPHP-Commands ($${ ... })
            foreach (Match lphpMatch in Regex.Matches(pFileContent, @"\$\$\{[\s\S]*?\}"))
            {
                Console.WriteLine(Regex.Replace(lphpMatch.Value, @"^\$\$\{|\}$", "").Trim());
            }

            foreach (Match lphpMatch in Regex.Matches(pFileContent, @"\$\$[^\{][\w]*"))
            {
                Console.WriteLine(Regex.Replace(lphpMatch.Value, @"^\$\$\{|\}$", "").Trim());
            }

            return "";
        }
    }
}
