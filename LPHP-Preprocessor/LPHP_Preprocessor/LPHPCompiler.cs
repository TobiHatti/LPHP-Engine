using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LPHP_Preprocessor
{
    public class LPHPCompiler
    {
        private static List<string> instructionSessionBuffer = new List<string>();
        private static Dictionary<string, string> fileBuffer = new Dictionary<string, string>();
        private static Dictionary<string, object> localVariables = new Dictionary<string, object>();
        private static Dictionary<string, object> globalVariables = new Dictionary<string, object>();

        public static void Run(Dictionary<string, string> lphpFiles)
        {
            // Clear file-buffer
            fileBuffer.Clear();

            // Clear global variables
            globalVariables.Clear();

            // Loop through all LPHP-Files and compile them
            foreach (KeyValuePair<string, string> file in lphpFiles)
            {
                if (!fileBuffer.ContainsKey(file.Value))
                {
                    // Clear instruction-buffer
                    instructionSessionBuffer.Clear();

                    // Clear local variables
                    localVariables.Clear();

                    // Check if the file should be compiled and saved TODO NOW!
                    if (!IsNoCompile(file.Value))
                    {
                        // Load and compile file
                        string output = LoadFile(file.Value);

                        PrepareInstructions();

                        output = SetLocalVariables(output);

                        // Add file to buffer. Required for global variables and saving.
                        fileBuffer.Add(file.Value, output);
                    }
                }   
            }

            foreach(KeyValuePair<string, string> file in fileBuffer)
            {
                string fileContent = file.Value;

                // Set global variables
                fileContent = SetGlobalVariables(fileContent);

                // Save the file
                SaveFile(file.Key, fileContent);
            }
        }

        private static bool IsNoCompile(string pFilePath)
        {
            bool isNoCompile = false;
            using(StreamReader sr = new StreamReader(pFilePath))
            {
                if (Regex.IsMatch(sr.ReadToEnd(), @"\$\$\{\s*?NoCompile\s*?=\s*?true;[\S\s]*?\}")) isNoCompile = true;
                sr.Close();
            }
            return isNoCompile;
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

        private static string SetLocalVariables(string pFileContent)
        {
            foreach (Match ItemMatch in Regex.Matches(pFileContent, @"\$\$\??(?!\{)\w*"))
            {
                string key = ItemMatch.Value.TrimStart('$');

                try
                {
                    if (key.StartsWith("?") && !localVariables.ContainsKey(key.TrimStart('?')))
                        pFileContent = pFileContent.Replace(ItemMatch.Value, "");
                    else
                        pFileContent = pFileContent.Replace(ItemMatch.Value, localVariables[key.TrimStart('?')].ToString());
                }
                catch { }
            }
            return pFileContent;
        }

        private static string SetGlobalVariables(string pFileContent)
        {
            return pFileContent;
        }

        private static string LoadIntoLayout(string pFileContent, string pLayoutFile, string pParentFilePath)
        {
            if(pLayoutFile != null)
            {
                // Load Layout and execute RenderBody
                string layoutContent = LoadFile(Path.Combine(Path.GetDirectoryName(pParentFilePath), pLayoutFile));
                if (layoutContent.Contains("$$RenderBody()"))
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
                {
                    if (!string.IsNullOrEmpty(operation))
                    {
                        instructionSessionBuffer.Add(operation);

                        if (Regex.IsMatch(operation, @"Layout\s*?=\s*?\""[\S\s]*?\"""))
                            layoutFile = Regex.Replace(operation, @"^Layout\s*?=\s*?\""|\""$", "");
                    }
                }
            }

            return Regex.Replace(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}", "");
        }

        private static void PrepareInstructions()
        {
            foreach (string instruction in instructionSessionBuffer.ToArray())
            {
                // [SET] Local variable
                if(Regex.IsMatch(instruction, @"^set\s*?\S*?\s*?=\s*?[\S\s]*?$"))
                {
                    string varName = Regex.Replace(instruction, @"^set\s*|\s*?=[\S\s]*?$", "");
                    object varValue = ValueParser(Regex.Replace(instruction, @"^set[\S\s]*?=\s*", ""));

                    if (!localVariables.ContainsKey(varName))
                        localVariables.Add(varName, varValue);
                    else
                        Console.WriteLine("Warning: " + varName + " gets assigned more than once!");
                }
                // [GLOB] Global variable
                else if (Regex.IsMatch(instruction, @"^glob\s*?\S*?\s*?=\s*?[\S\s]*?$"))
                {
                    string varName = Regex.Replace(instruction, @"^glob\s*|\s*?=[\S\s]*?$", "");
                    object varValue = ValueParser(Regex.Replace(instruction, @"^glob[\S\s]*?=\s*", ""));

                    if (!globalVariables.ContainsKey(varName))
                        globalVariables.Add(varName, varValue);
                    else
                        Console.WriteLine("Warning: " + varName + " gets assigned more than once!");

                }
                // Ignored instructions
                else if (
                    Regex.IsMatch(instruction, @"^Layout\s*?=\s*?\""[\S\s]*?\""$") ||
                    Regex.IsMatch(instruction, @"^NoCompile\s*?=\s*?(true|false)$") 
                    )
                {
                    

                }
                // Invalid instruction
                else
                {
                    Console.WriteLine($"{instruction} is not a valid instruction");
                }

                // Remove entry from List
                instructionSessionBuffer.Remove(instruction);
            }

            
        }

        private static void SaveFile(string pOriginalFilename, string pFileContent)
        {
            bool minOutput = false;

            string targetFile = Path.Combine(Path.GetDirectoryName(pOriginalFilename), Path.GetFileNameWithoutExtension(pOriginalFilename) + ".php");

            StreamWriter sw = new StreamWriter(targetFile);
            if (minOutput) sw.WriteLine(pFileContent);
            else sw.WriteLine(System.Xml.Linq.XElement.Parse(pFileContent).ToString());
            sw.Close();
        }      
        
        private static object ValueParser(string pVariableValue)
        {
            if (pVariableValue.StartsWith("\"")) return pVariableValue.TrimStart('"').TrimEnd('"');
            else if (pVariableValue.ToLower() == "true") return true;
            else if (pVariableValue.ToLower() == "false") return false;
            else if (decimal.TryParse(pVariableValue, out _)) return decimal.Parse(pVariableValue);
            else return pVariableValue;
        }
    }
}
