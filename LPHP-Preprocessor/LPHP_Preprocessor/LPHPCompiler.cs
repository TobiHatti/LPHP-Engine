using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace LPHP_Preprocessor
{
    public class LPHPCompiler
    {
        /// <summary>
        /// Removes HTML-Tags (<!-- -->) from the output
        /// </summary>
        private static bool COMP_REMOVE_HTML_COMMENTS { get; set; } = true;

        /// <summary>
        /// Enable Min-Output (*.min.php)
        /// </summary>
        private static bool COMP_MIN_OUTPUT_ENABLED { get; set; } = true;

        /// <summary>
        /// Enable XML-Foramted output with linebreaks and indents
        /// </summary>
        private static bool COMP_XML_OUTPUT_ENABLED { get; set; } = false;


        private static Dictionary<string, object> COMPOPT = null;

        private static readonly List<string> instructionSessionBuffer = new List<string>();
        private static readonly Dictionary<string, string> fileBuffer = new Dictionary<string, string>();
        private static readonly Dictionary<string, object> localVariables = new Dictionary<string, object>();
        private static readonly Dictionary<string, object> globalVariables = new Dictionary<string, object>();

        private static string currentCompileFile = "";

        public static void Init()
        {
            COMPOPT = new Dictionary<string, object>
            {
                { "REMOVE_HTML_COMMENTS", true },
                { "MIN_OUTPUT_ENABLED", true },
                { "XML_OUTPUT_ENABLED", false }
            };

            if (!File.Exists("LPHP.ini"))
            {
                using (StreamWriter sw = new StreamWriter("LPHP.ini"))
                {
                    foreach (KeyValuePair<string, object> entry in COMPOPT)
                        sw.WriteLine($"{entry.Key}={entry.Value}");
                    sw.Close();
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader("LPHP.ini"))
                {
                    string line;
                    while((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] keyValueEntry = line.Split('=');
                            try
                            {
                                COMPOPT[keyValueEntry[0]] = Convert.ChangeType(keyValueEntry[1], COMPOPT[keyValueEntry[0]].GetType());
                            }
                            catch
                            {
                                PrintError("*** Error in \"LPHP.ini\" ***");
                                PrintError("Unknown key: " + keyValueEntry[0]);
                                PrintError("Please check or delete the config-file and restart the program.");
                                throw new ApplicationException();
                            }
                        }
                    }
                }
            }
        }

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

                        currentCompileFile = file.Value;

                        PrepareInstructions();

                        output = SetLocalVariables(output);

                        // Add file to buffer. Required for global variables and saving.
                        fileBuffer.Add(file.Value, output);
                    }
                    else
                    {
                        DeleteFile(file.Value);
                    }
                }   
            }

            foreach(KeyValuePair<string, string> file in fileBuffer)
            {
                currentCompileFile = file.Key;

                string fileContent = file.Value;

                // Set global variables
                fileContent = SetGlobalVariables(fileContent);

                // Remove tabs, linebreaks and unneccecary whitespaces
                fileContent = SourceCleanup(fileContent);

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
            currentCompileFile = pFilePath;

            // Read entire file (without comments)
            string fileContent = LoadWithoutComments(pFilePath);
            currentCompileFile = pFilePath;

            

            // Extract header-instructions
            fileContent = ExtractHeaderInstructions(fileContent, out string layout);
            currentCompileFile = pFilePath;

            // Load the layout around the source-file
            fileContent = LoadIntoLayout(fileContent, layout, pFilePath);
            currentCompileFile = pFilePath;

            // Load RenderPages into source-file
            fileContent = LoadRenderPages(fileContent, pFilePath);
            currentCompileFile = pFilePath;



            return fileContent;
        }

        private static string SetLocalVariables(string pFileContent)
        {
            // Read in functions and variable calls
            foreach (Match ItemMatch in Regex.Matches(pFileContent, @"\$\$\??(?!\{)\w*(\([\S\s]*?\))?"))
            {
                // Check if the matched object is not a function-call
                if (!Regex.IsMatch(ItemMatch.Value, @"\$\$\??(?!\{)\w*(\([\S\s]*?\))"))
                {
                    string key = ItemMatch.Value.TrimStart('$');
                     
                    try
                    {
                        if (key.StartsWith("?") && !localVariables.ContainsKey(key.TrimStart('?')))
                            pFileContent = pFileContent.Replace(ItemMatch.Value, "");
                        else
                            pFileContent = pFileContent.Replace(ItemMatch.Value, localVariables[key.TrimStart('?')].ToString());
                    }
                    catch
                    {
                        // No warning here, because the variable could still be a global variable
                    }
                }
            }
            return pFileContent;
        }

        private static string SetGlobalVariables(string pFileContent)
        {
            // Read in functions and variable calls
            foreach (Match ItemMatch in Regex.Matches(pFileContent, @"\$\$\??(?!\{)\w*(\([\S\s]*?\))?"))
            {
                // Check if the matched object is not a function-call
                if (!Regex.IsMatch(ItemMatch.Value, @"\$\$\??(?!\{)\w*(\([\S\s]*?\))"))
                {
                    string key = ItemMatch.Value.TrimStart('$');
#if !DEBUG
                    try
                    {
#endif
                        if (key.StartsWith("?") && !globalVariables.ContainsKey(key.TrimStart('?')))
                            pFileContent = pFileContent.Replace(ItemMatch.Value, "");
                        else
                            pFileContent = pFileContent.Replace(ItemMatch.Value, globalVariables[key.TrimStart('?')].ToString());
#if !DEBUG
                    }
                    catch
                    {
                        PrintError("Unknown variable in " + currentCompileFile);
                        PrintError("Variable: " + key.TrimStart('?'));
                        throw new ApplicationException();
                    
                    }
#endif
                }
            }
            return pFileContent;
        }

        private static string LoadIntoLayout(string pFileContent, string pLayoutFile, string pParentFilePath)
        {
            if(pLayoutFile != null)
            {
                string layoutFilePath;
#if !DEBUG
                try
                {
#endif
                    layoutFilePath = Path.Combine(Path.GetDirectoryName(pParentFilePath), pLayoutFile);
#if !DEBUG
                }
                catch
                {
                    PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    PrintError("\"" + pLayoutFile + "\" is not a valid file-path.");
                    throw new ApplicationException();
                }
#endif

                if (File.Exists(layoutFilePath))
                {
                    // Load Layout and execute RenderBody
                    string layoutContent = LoadFile(layoutFilePath);
                    if (layoutContent.Contains("$$RenderBody()"))
                        pFileContent = layoutContent.Replace("$$RenderBody()", pFileContent);
                    else
                    {
                        PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                        PrintError("RenderBody() doesn't get called! Layout-Pages require exactly one call to RenderBody()!");
                        throw new ApplicationException();
                    }
                }
                else
                {
                    PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    PrintError("Could not load Layout-Page: ");
                    PrintError("\"" + layoutFilePath + "\" does not exist.");
                    throw new ApplicationException();
                }
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

                string renderPageFilePath = "";
#if !DEBUG
                try
                {
#endif
                    renderPageFilePath = Path.Combine(Path.GetDirectoryName(pParentFilePath), renderPageFile);
#if !DEBUG
                }
                catch
                {
                    PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    PrintError("\"" + renderPageFile + "\" is not a valid file-path.");
                    throw new ApplicationException();
                }
#endif

                if (File.Exists(renderPageFilePath))
                    pFileContent = pFileContent.Replace(originalRenderPageCommand, LoadFile(renderPageFilePath));
                else
                {
                    PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    PrintError("Could not render Page: ");
                    PrintError("\"" + renderPageFilePath + "\" does not exist.");
                    throw new ApplicationException();
                }
            }

            return pFileContent;
        }

        private static string LoadWithoutComments(string pFilePath)
        {
            string fileContent;
            using (StreamReader sr = new StreamReader(pFilePath))
            {
                fileContent = sr.ReadToEnd();

                // Replace LPHP-Comments
                foreach (Match match in Regex.Matches(fileContent, @"\$\$\{[\S\s]*?\}"))
                    foreach (Match comment in Regex.Matches(match.Value, @"(?<=\$\$\{[\S\s]*?)((?<!(https?|ftp):)(\/\/[\S\s]*?)|(\/\*[\S\s]*?\*\/))(?=\n[\S\s]*?\})"))
                        fileContent = ReplaceFromPosition(fileContent, match.Index + comment.Index, comment.Value.Length, ' ');

                // Replace PHP-Comments
                foreach (Match match in Regex.Matches(fileContent, @"\<\?php[\S\s]*?\?\>"))
                    foreach (Match comment in Regex.Matches(match.Value, @"(?<=\<\?php[\S\s]*?)((?<!(https?|ftp):)(\/\/[\S\s]*?)|(\/\*[\S\s]*?\*\/))(?=\n[\S\s]*?\?\>)"))
                        fileContent = ReplaceFromPosition(fileContent, match.Index + comment.Index, comment.Value.Length, ' ');

                // Remove HTML-Comments, if compiler-flag is set
                if ((bool)COMPOPT["REMOVE_HTML_COMMENTS"])
                    fileContent = Regex.Replace(fileContent, @"\<\!\-\-[\S\s]*?\-\-\>", "");

                sr.Close();
            }

            return fileContent;
        }

        private static string ReplaceFromPosition(string pFileContent, int startIndex, int length, char replaceChar)
        {
            StringBuilder sb = new StringBuilder(pFileContent);
            sb.Remove(startIndex, length);
            sb.Insert(startIndex, new String(' ', length));
            return sb.ToString();
        }

        private static string SourceCleanup(string rawFileContent)
        {
            // Remove tabs and linebreaks
            string cleanedContent = rawFileContent.Replace("\t", " ").Replace("\r\n", " ");

            // Remove double WS (HTML-Part only)
            cleanedContent = WSReplace(cleanedContent);

            return cleanedContent;
        }

        private static string WSReplace(string pFileContent)
        {
            Dictionary<int, Tuple<string, string>> beatufifyFlags = new Dictionary<int, Tuple<string, string>>();

            beatufifyFlags.Add(0, new Tuple<string, string>("<?php", "?>"));
            beatufifyFlags.Add(1, new Tuple<string, string>("<?=", "?>"));
            beatufifyFlags.Add(2, new Tuple<string, string>("<script", "</script>"));


            StringBuilder sb = new StringBuilder();

            int activeFlag = -1;
            char lastChar = '\0';

            for (int i = 0; i < pFileContent.Length; i++)
            {
                int remainingSourceLength = pFileContent.Length - i;

                if (activeFlag == -1)
                {
                    foreach (KeyValuePair<int, Tuple<string, string>> flag in beatufifyFlags)
                    {
                        if (remainingSourceLength >= flag.Value.Item1.Length)
                        {
                            if (flag.Value.Item1[0] == pFileContent[i])
                            {
                                if (pFileContent.Substring(i, flag.Value.Item1.Length) == flag.Value.Item1)
                                {
                                    activeFlag = flag.Key;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(beatufifyFlags[activeFlag].Item2[0] == pFileContent[i])
                    {
                        if (pFileContent.Substring(i, beatufifyFlags[activeFlag].Item2.Length) == beatufifyFlags[activeFlag].Item2)
                        {
                            activeFlag = -1;
                        }
                    }
                }

                if(activeFlag == -1)
                {
                    if (!(pFileContent[i] == ' ' && lastChar == ' ')) sb.Append(pFileContent[i]);
                    lastChar = pFileContent[i];
                }
                else sb.Append(pFileContent[i]);               
            }

            return sb.ToString();
        }

        private static string ExtractHeaderInstructions(string pFileContent, out string layoutFile)
        {
            layoutFile = null;

            if (Regex.IsMatch(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}"))
            {
                string[] headerInstructions = Regex.Match(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}").Value.TrimStart('$','{').TrimEnd('}').Split(';');

                foreach (string operation in headerInstructions)
                {
                    string operationTrimmed = operation.Trim();

                    if (!string.IsNullOrEmpty(operationTrimmed))
                    {
                        instructionSessionBuffer.Add(operationTrimmed);

                        if (Regex.IsMatch(operationTrimmed, @"Layout\s*?=\s*?\""[\S\s]*?\"""))
                            layoutFile = Regex.Replace(operationTrimmed, @"^Layout\s*?=\s*?\""|\""$", "");
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
                    {
                        PrintWarning("Warning in " + currentCompileFile + ":");
                        PrintWarning("Variable \"" + varName + "\" gets assigned more than once!");
                        throw new ApplicationException();
                    }
                }
                // [GLOB] Global variable
                else if (Regex.IsMatch(instruction, @"^glob\s*?\S*?\s*?=\s*?[\S\s]*?$"))
                {
                    string varName = Regex.Replace(instruction, @"^glob\s*|\s*?=[\S\s]*?$", "");
                    object varValue = ValueParser(Regex.Replace(instruction, @"^glob[\S\s]*?=\s*", ""));

                    if (!globalVariables.ContainsKey(varName))
                        globalVariables.Add(varName, varValue);
                    else
                    {
                        PrintWarning("Warning in " + currentCompileFile + ":");
                        PrintWarning("Global variable \"" + varName + "\" gets assigned more than once!");
                        throw new ApplicationException();
                    }

                }
                // Ignored instructions
                else if (
                    Regex.IsMatch(instruction, @"^Layout\s*?=\s*?\""[\S\s]*?\""$") ||
                    Regex.IsMatch(instruction, @"^NoCompile\s*?=\s*?(true|false)$") 
                    )
                { 
                    // Ignore. 
                }
                // Invalid instruction
                else
                {
                    PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    PrintError($"Unknown instruction: \"{instruction}\"");
                    throw new ApplicationException();
                }

                // Remove entry from List
                instructionSessionBuffer.Remove(instruction);
            }

            
        }

        private static void SaveFile(string pOriginalFilename, string pFileContent)
        {
            string targetFileXML = Path.Combine(Path.GetDirectoryName(pOriginalFilename), Path.GetFileNameWithoutExtension(pOriginalFilename) + ".php");
            string targetFileMIN = Path.Combine(Path.GetDirectoryName(pOriginalFilename), Path.GetFileNameWithoutExtension(pOriginalFilename) + ".min.php");

            // Delete old, unused generated php files
            try { File.Delete(targetFileXML); } catch { }
            try { File.Delete(targetFileMIN); } catch { }

            bool fileGenerated = false;


            string selectedFileExt;
            if ((bool)COMPOPT["MIN_OUTPUT_ENABLED"])
            {
                // If only min is selected, no min gets added to the extension
                if ((bool)COMPOPT["XML_OUTPUT_ENABLED"]) selectedFileExt = targetFileMIN;
                else selectedFileExt = targetFileXML;

                using (StreamWriter sw = new StreamWriter(selectedFileExt))
                {
                    sw.Write(pFileContent);
                    sw.Close();
                }

                fileGenerated = true;
            }

            // TODO: Output XML-Formated document
            if ((bool)COMPOPT["XML_OUTPUT_ENABLED"])
            {
                using (StreamWriter sw = new StreamWriter(targetFileXML))
                {
                    sw.Write(pFileContent);
                    sw.Close();
                }

                fileGenerated = true;
            }

            if(!fileGenerated)
            {
                PrintError("Warning: No Output-Type selected!");
                PrintError("Please set MIN_OUTPUT_ENABLED and/or XML_OUTPUT_ENABLED to \"True\" in LPHP.ini");
                throw new ApplicationException();
            }
        }

        private static void DeleteFile(string pOriginalFilename)
        {
            string targetFile = Path.Combine(Path.GetDirectoryName(pOriginalFilename), Path.GetFileNameWithoutExtension(pOriginalFilename) + ".php");

            File.Delete(targetFile);
        }

        private static object ValueParser(string pVariableValue)
        {
            if (pVariableValue.StartsWith("\"")) return pVariableValue.TrimStart('"').TrimEnd('"');
            else if (pVariableValue.ToLower() == "true") return true;
            else if (pVariableValue.ToLower() == "false") return false;
            else if (decimal.TryParse(pVariableValue, out _)) return decimal.Parse(pVariableValue);
            else return pVariableValue;
        }

        public static void PrintWarning(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(pMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintError(string pMessage)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(pMessage);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
