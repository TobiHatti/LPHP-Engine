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
        private static List<string> instructionSessionBuffer = new List<string>();

        public static void Run(Dictionary<string, string> lphpFiles)
        {
            // Loop through all LPHP-Files and compile them
            foreach (KeyValuePair<string, string> file in lphpFiles)
            {
                // Clear instruction-Buffer
                instructionSessionBuffer.Clear();

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

            // Extract header-instructions
            fileContent = ExtractHeaderInstructions(fileContent, out string layout);

            // Load the layout around the source-file
            fileContent = LoadIntoLayout(fileContent, layout, pFilePath);

            // Load RenderPages into source-file
            fileContent = LoadRenderPages(fileContent, pFilePath);

           

            return fileContent;
        }

        private static string LoadIntoLayout(string pFileContent, string pLayoutFile, string pParentFilePath)
        {
            if(pLayoutFile != null)
            {
                // Load Layout and execute RenderBody
                string layoutContent = LoadFile(Path.Combine(Path.GetDirectoryName(pParentFilePath), pLayoutFile));
                if (layoutContent.Contains("$$RenderBody()"))
                    //pFileContent = Regex.Replace(layoutContent, @"\$\$RenderBody\(\)", pFileContent, RegexOptions);
                    pFileContent = layoutContent.Replace("$$RenderBody()", pFileContent);
                else
                    Console.WriteLine("********* NO RENDERBODY ***********");
            }

            return pFileContent;
        }

        private static string LoadRenderPages(string pFileContent, string pParentFilePath)
        {
            // Find RenderPage()-Command, and load Child-File
            foreach (Match ItemMatch in Regex.Matches(pFileContent, @"\$\$RenderPage\(\""[\S\s]*?\""\)"))
            {
                string originalRenderPageCommand = ItemMatch.Value;
                string renderPageFile = Regex.Match(ItemMatch.Value, @"\""[\S\s]*?\""").Value.Replace("\"", "");

                pFileContent = pFileContent.Replace(originalRenderPageCommand, LoadFile(Path.Combine(Path.GetDirectoryName(pParentFilePath), renderPageFile)));
            }

            return pFileContent;
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

        private static string ExtractHeaderInstructions(string pFileContent, out string layoutFile)
        {
            layoutFile = null;

            if (Regex.IsMatch(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}"))
            {
                string[] headerInstructions = Regex.Match(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}").Value.TrimStart('$','{').TrimEnd('}').Split(';');

                foreach (string operation in headerInstructions)
                    if (Regex.IsMatch(operation, @"Layout\s*?=\s*?\""[\S\s]*?\"""))
                        layoutFile = Regex.Replace(operation, @"^Layout\s*?=\s*?\""|\""$", "");
            }

            return Regex.Replace(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}", "");
        }
    }
}
