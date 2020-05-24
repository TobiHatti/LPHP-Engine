using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LPHP_Preprocessor
{
    public class LPHPCompiler
    {
        public static void Run(Dictionary<string, string> lphpFiles)
        {
            // Loop through all LPHP-Files and compile them
            foreach (KeyValuePair<string, string> file in lphpFiles)
            {
                // Load and compile file
                string output = LPHPCompiler.LoadFile(file.Value);
                Console.WriteLine(output + "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
                // Write result to temp journal
                StreamWriter sw = new StreamWriter("journal.lphp-temp");
                sw.WriteLine(output);
                sw.Close();

                // Copy temp-file to actual destination
                // TODO
            }
        }

        private static string LoadFile(string pFilePath)
        {
            // Read entire file (without comments)
            string fileContent = LoadWithoutComments(pFilePath);

            // Remove tabs and linebreaks
            fileContent = SourceCleanup(fileContent);

            // Find Layout-Command and set the layout of the page
            if(Regex.IsMatch(fileContent, @"\$\$\{[\S\s]*?Layout\s*?=\s*?\""[\S\s]*?\""\;[\S\s]*?\}"))
            {
                // Determine Layout-File
                string layoutFile = Regex.Match(fileContent, @"\$\$\{[\S\s]*?Layout\s*?=\s*?\""[\S\s]*?\""\;[\S\s]*?\}").Value;
                layoutFile = Regex.Match(layoutFile, @"Layout\s*?=\s*?\""[\S\s]*?\""\;").Value;
                layoutFile = Regex.Match(layoutFile, @"\""[\S\s]*?\""").Value.Replace("\"", "");

                string layoutContent = LoadFile(Path.Combine(Path.GetDirectoryName(pFilePath), layoutFile));

                string sourceBody = Regex.Replace(fileContent, @"^\$\$\{[\S\s]*?\}", "");

                if (Regex.IsMatch(layoutContent, @"\$\$RenderBody\(\)"))
                {
                    fileContent = Regex.Replace(layoutContent, @"\$\$RenderBody\(\)", sourceBody);
                }
                else Console.WriteLine("********* NO RENDERBODY ***********");
            }

            // Find RenderPage()-Command, and load Child-File
            foreach (Match ItemMatch in Regex.Matches(fileContent, @"\$\$RenderPage\(\""[\S\s]*?\""\)"))
            {
                string originalRenderPageCommand = ItemMatch.Value;
                string renderPageFile = Regex.Match(ItemMatch.Value, @"\""[\S\s]*?\""").Value.Replace("\"","");

                fileContent = fileContent.Replace(originalRenderPageCommand, LoadFile(Path.Combine(Path.GetDirectoryName(pFilePath),renderPageFile)));
            }

            return fileContent;
        }

        private static string LoadWithoutComments(string pFilePath)
        {
            StreamReader sr = new StreamReader(pFilePath);
            StringBuilder sb = new StringBuilder();
            string line;

            // Read each line and remove comments
            while ((line = sr.ReadLine()) != null)
                sb.Append(Regex.Replace(line, @"\/\*[\s\S]*?\*\/|\/\/.*", ""));
            sr.Close();

            return sb.ToString();
        }

        private static string SourceCleanup(string rawFileContent)
        {
            // Remove tabs and linebreaks
            return rawFileContent.Replace("\t", "").Replace("\r\n", "");
        }
    }
}
