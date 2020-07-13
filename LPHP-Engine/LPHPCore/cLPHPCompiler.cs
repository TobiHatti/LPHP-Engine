using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LPHPCore
{
    /// <summary>
    /// Provides tools to compile LPHP-files into functional PHP-files
    /// </summary>
    public class LPHPCompiler
    {
        public static Dictionary<string, object> COMPOPT { get; set; } = null;

        private static readonly List<string> instructionSessionBuffer = new List<string>();
        private static readonly Dictionary<string, string> fileBuffer = new Dictionary<string, string>();
        private static readonly Dictionary<string, object> localVariables = new Dictionary<string, object>();
        private static readonly Dictionary<string, object> globalVariables = new Dictionary<string, object>();

        private static string currentCompileFile = "";

        private static readonly string LPHPIniFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Endev", "LPHP", "LPHP.ini");

        /// <summary>
        /// Initializes the LPHP-Compiler
        /// </summary>
        public static void Init()
        {
            COMPOPT = new Dictionary<string, object>
            {
                { "REMOVE_HTML_COMMENTS", true },
                { "MIN_OUTPUT_ENABLED", true },
                { "XML_OUTPUT_ENABLED", false },
                { "UI_LAST_PROJECT_PATH", "" },
                { "ENABLE_CONSOLE_LOG", true }
            };

            if (!File.Exists(LPHPIniFile))
            {
                SaveConfig();
            }
            else
            {
                using (StreamReader sr = new StreamReader(LPHPIniFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
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
                                LPHPDebugger.PrintError("*** Error in \"LPHP.ini\" ***");
                                LPHPDebugger.PrintError("Unknown key: " + keyValueEntry[0]);
                                LPHPDebugger.PrintError("Please check or delete the config-file and restart the program.");
                                throw new ApplicationException();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the config to the ini-file
        /// </summary>
        public static void SaveConfig()
        {
            if(!Directory.Exists(Path.GetDirectoryName(LPHPIniFile))) Directory.CreateDirectory(Path.GetDirectoryName(LPHPIniFile));

            using (StreamWriter sw = new StreamWriter(LPHPIniFile))
            {
                foreach (KeyValuePair<string, object> entry in COMPOPT)
                    sw.WriteLine($"{entry.Key}={entry.Value}");
                sw.Close();
            }
        }

        /// <summary>
        /// Runs the LPHP-compiler on the provided set of files
        /// </summary>
        /// <param name="lphpFiles"></param>
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

            foreach (KeyValuePair<string, string> file in fileBuffer)
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

        /// <summary>
        /// Checks if a file has the NoCompile-flag set
        /// </summary>
        /// <param name="pFilePath">Filepath of the file that should be checked</param>
        /// <returns>Return true if the file has the NoCompile-flag set</returns>
        private static bool IsNoCompile(string pFilePath)
        {
            bool isNoCompile = false;
            using (StreamReader sr = new StreamReader(pFilePath))
            {
                if (Regex.IsMatch(sr.ReadToEnd(), @"\$\$\{\s*?NoCompile\s*?=\s*?true;[\S\s]*?\}")) isNoCompile = true;
                sr.Close();
            }
            return isNoCompile;
        }

        /// <summary>
        /// Loads a file from disk to memory and prepares it for compilation
        /// </summary>
        /// <param name="pFilePath">File that should be loaded</param>
        /// <returns>Returns the prepared and pre-compiled file-contents</returns>
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

        /// <summary>
        /// Sets local LPHP-variables inside the given file
        /// </summary>
        /// <param name="pFileContent">Target file</param>
        /// <returns>File-content with set (local) variables</returns>
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

        /// <summary>
        /// Sets global LPHP-variables inside the given file
        /// </summary>
        /// <param name="pFileContent">Target file</param>
        /// <returns>File-content with set (global) variables</returns>
        private static string SetGlobalVariables(string pFileContent)
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

                        if (key.StartsWith("?") && !globalVariables.ContainsKey(key.TrimStart('?')))
                            pFileContent = pFileContent.Replace(ItemMatch.Value, "");
                        else
                            pFileContent = pFileContent.Replace(ItemMatch.Value, globalVariables[key.TrimStart('?')].ToString());
                    }
                    catch
                    {
                        LPHPDebugger.PrintError("Unknown variable in " + currentCompileFile);
                        LPHPDebugger.PrintError("Variable: " + key.TrimStart('?'));
                        throw new ApplicationException();
                    }
                }
            }
            return pFileContent;
        }

        /// <summary>
        /// Loads an LPHP content-file and merges it with an LPHP layout-file
        /// </summary>
        /// <param name="pFileContent">File-content of the source-file</param>
        /// <param name="pLayoutFile">Path to the layout-file</param>
        /// <param name="pParentFilePath">Parent file-path of the source</param>
        /// <returns>Page-contents combined with layout</returns>
        private static string LoadIntoLayout(string pFileContent, string pLayoutFile, string pParentFilePath)
        {
            if (pLayoutFile != null)
            {
                string layoutFilePath;

                try
                {

                    layoutFilePath = Path.Combine(Path.GetDirectoryName(pParentFilePath), pLayoutFile);

                }
                catch
                {
                    LPHPDebugger.PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    LPHPDebugger.PrintError("\"" + pLayoutFile + "\" is not a valid file-path.");
                    throw new ApplicationException();
                }

                if (File.Exists(layoutFilePath))
                {
                    // Load Layout and execute RenderBody
                    string layoutContent = LoadFile(layoutFilePath);
                    if (layoutContent.Contains("$$RenderBody()"))
                        pFileContent = layoutContent.Replace("$$RenderBody()", pFileContent);
                    else
                    {
                        LPHPDebugger.PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                        LPHPDebugger.PrintError("RenderBody() doesn't get called! Layout-Pages require exactly one call to RenderBody()!");
                        throw new ApplicationException();
                    }
                }
                else
                {
                    LPHPDebugger.PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    LPHPDebugger.PrintError("Could not load Layout-Page: ");
                    LPHPDebugger.PrintError("\"" + layoutFilePath + "\" does not exist.");
                    throw new ApplicationException();
                }
            }

            return pFileContent;
        }

        /// <summary>
        /// Loads and includes pages included by the RenderPage-Function
        /// </summary>
        /// <param name="pFileContent">File-content of the source-file</param>
        /// <param name="pParentFilePath">>Parent file-path of the source</param>
        /// <returns>Source file-contents with rendered pages included</returns>
        private static string LoadRenderPages(string pFileContent, string pParentFilePath)
        {
            // Find RenderPage()-Command, and load Child-File
            foreach (Match ItemMatch in Regex.Matches(pFileContent, @"\$\$RenderPage\(\""[\S\s]*?\""\)"))
            {
                string originalRenderPageCommand = ItemMatch.Value;
                string renderPageFile = Regex.Match(ItemMatch.Value, @"\""[\S\s]*?\""").Value.Replace("\"", "");

                string renderPageFilePath = "";

                try
                {
                    renderPageFilePath = Path.Combine(Path.GetDirectoryName(pParentFilePath), renderPageFile);
                }
                catch
                {
                    LPHPDebugger.PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    LPHPDebugger.PrintError("\"" + renderPageFile + "\" is not a valid file-path.");
                    throw new ApplicationException();
                }

                if (File.Exists(renderPageFilePath))
                    pFileContent = pFileContent.Replace(originalRenderPageCommand, LoadFile(renderPageFilePath));
                else
                {
                    LPHPDebugger.PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    LPHPDebugger.PrintError("Could not render Page: ");
                    LPHPDebugger.PrintError("\"" + renderPageFilePath + "\" does not exist.");
                    throw new ApplicationException();
                }
            }

            return pFileContent;
        }

        /// <summary>
        /// Loads an LPHP-file without LPHP and PHP comments
        /// </summary>
        /// <param name="pFilePath">Filepath of the LPHP-file</param>
        /// <returns>Filecontents without comments</returns>
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

        /// <summary>
        /// Replaces a section of a string with a specified character, without altering the source-length
        /// </summary>
        /// <param name="pFileContent">Source file-content</param>
        /// <param name="startIndex">Start-Index from which the replacement should start</param>
        /// <param name="length">Length that should be replaced</param>
        /// <param name="replaceChar">Character thar shoiuld be inserted at the replacement-position</param>
        /// <returns>Filecontent with replaced section</returns>
        private static string ReplaceFromPosition(string pFileContent, int startIndex, int length, char replaceChar)
        {
            StringBuilder sb = new StringBuilder(pFileContent);
            sb.Remove(startIndex, length);
            sb.Insert(startIndex, new String(replaceChar, length));
            return sb.ToString();
        }

        /// <summary>
        /// Cleans up the raw loaded filecontents
        /// </summary>
        /// <param name="rawFileContent">Raw file-content</param>
        /// <returns>Cleaned and refined file-content</returns>
        private static string SourceCleanup(string rawFileContent)
        {
            // Remove tabs and linebreaks
            string cleanedContent = rawFileContent.Replace("\t", " ").Replace("\r\n", " ");

            // Remove double WS (HTML-Part only)
            cleanedContent = WSReplace(cleanedContent);

            return cleanedContent;
        }

        /// <summary>
        /// Replaces concurrent whitespaces in the source-file, except in php and script tags
        /// </summary>
        /// <param name="pFileContent">Source filecontent</param>
        /// <returns>Filecontent with double-whitespaces removed</returns>
        private static string WSReplace(string pFileContent)
        {
            Dictionary<int, Tuple<string, string>> beatufifyFlags = new Dictionary<int, Tuple<string, string>>
            {
                { 0, new Tuple<string, string>("<?php", "?>") },
                { 1, new Tuple<string, string>("<?=", "?>") },
                { 2, new Tuple<string, string>("<script", "</script>") }
            };

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
                    if (beatufifyFlags[activeFlag].Item2[0] == pFileContent[i])
                    {
                        if (pFileContent.Substring(i, beatufifyFlags[activeFlag].Item2.Length) == beatufifyFlags[activeFlag].Item2)
                        {
                            activeFlag = -1;
                        }
                    }
                }

                if (activeFlag == -1)
                {
                    if (!(pFileContent[i] == ' ' && lastChar == ' ')) sb.Append(pFileContent[i]);
                    lastChar = pFileContent[i];
                }
                else sb.Append(pFileContent[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Extracts the LPHP header-information from the given filecontent
        /// </summary>
        /// <param name="pFileContent">Source filecontent</param>
        /// <param name="layoutFile">Out > Layout-file of the sourcefile</param>
        /// <returns>Returns the source filecontent without the LPHP-header</returns>
        private static string ExtractHeaderInstructions(string pFileContent, out string layoutFile)
        {
            layoutFile = null;

            if (Regex.IsMatch(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}"))
            {
                string[] headerInstructions = Regex.Match(pFileContent, @"^[\s]*?\$\$\{[\S\s]*?\}").Value.TrimStart('$', '{').TrimEnd('}').Split(';');

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

        /// <summary>
        /// Prepares LPHP-instructions for processing
        /// </summary>
        private static void PrepareInstructions()
        {
            foreach (string instruction in instructionSessionBuffer.ToArray())
            {
                // [SET] Local variable
                if (Regex.IsMatch(instruction, @"^set\s*?\S*?\s*?=\s*?[\S\s]*?$"))
                {
                    string varName = Regex.Replace(instruction, @"^set\s*|\s*?=[\S\s]*?$", "");
                    object varValue = ValueParser(Regex.Replace(instruction, @"^set[\S\s]*?=\s*", ""));

                    if (!localVariables.ContainsKey(varName))
                        localVariables.Add(varName, varValue);
                    else
                    {
                        LPHPDebugger.PrintWarning("Warning in " + currentCompileFile + ":");
                        LPHPDebugger.PrintWarning("Variable \"" + varName + "\" gets assigned more than once!");
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
                        LPHPDebugger.PrintWarning("Warning in " + currentCompileFile + ":");
                        LPHPDebugger.PrintWarning("Global variable \"" + varName + "\" gets assigned more than once!");
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
                    LPHPDebugger.PrintError("*** Error in \"" + currentCompileFile + "\" ***");
                    LPHPDebugger.PrintError($"Unknown instruction: \"{instruction}\"");
                    throw new ApplicationException();
                }

                // Remove entry from List
                instructionSessionBuffer.Remove(instruction);
            }
        }

        /// <summary>
        /// Saves a file as php
        /// </summary>
        /// <param name="pOriginalFilename">Original LPHP filename (Source-filename)</param>
        /// <param name="pFileContent">Processed filecontent of the source LPHP-file</param>
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
                    sw.Write(AddCopyrightNotice(pFileContent, true));
                    sw.Close();
                }

                fileGenerated = true;
            }

            // TODO: Output XML-Formated document
            if ((bool)COMPOPT["XML_OUTPUT_ENABLED"])
            {
                using (StreamWriter sw = new StreamWriter(targetFileXML))
                {
                    sw.Write(LPHPHTML2XMLFormatter(AddCopyrightNotice(pFileContent, false)));
                    sw.Close();
                }

                fileGenerated = true;
            }

            if (!fileGenerated)
            {
                LPHPDebugger.PrintError("Warning: No Output-Type selected!");
                LPHPDebugger.PrintError("Please set MIN_OUTPUT_ENABLED and/or XML_OUTPUT_ENABLED to \"True\" in LPHP.ini");
                throw new ApplicationException();
            }
        }

        /// <summary>
        /// Deletes the corresponding php-file from the given lphp-file
        /// </summary>
        /// <param name="pOriginalFilename">Original filename (.lphp)</param>
        private static void DeleteFile(string pOriginalFilename)
        {
            string targetFile = Path.Combine(Path.GetDirectoryName(pOriginalFilename), Path.GetFileNameWithoutExtension(pOriginalFilename) + ".php");
            if (File.Exists(targetFile)) File.Delete(targetFile);
        }

        /// <summary>
        /// Parses variables from the LPHP-header and converts them into their propper datatypes
        /// </summary>
        /// <param name="pVariableValue">Value-string read from the LPHP-header</param>
        /// <returns>Returns the value of the variable with the correct datatype</returns>
        private static object ValueParser(string pVariableValue)
        {
            if (pVariableValue.StartsWith("\"")) return pVariableValue.TrimStart('"').TrimEnd('"');
            else if (pVariableValue.ToLower() == "true") return true;
            else if (pVariableValue.ToLower() == "false") return false;
            else if (decimal.TryParse(pVariableValue, out _)) return decimal.Parse(pVariableValue);
            else return pVariableValue;
        }

        /// <summary>
        /// Attempts to convert unformated HTML+PHP string into XML-foramted output
        /// </summary>
        /// <param name="pRawHTML"></param>
        /// <returns></returns>
        private static string LPHPHTML2XMLFormatter(string pRawHTML)
        {
            Dictionary<int, Tuple<string, string>> restrictionFlags = new Dictionary<int, Tuple<string, string>>
            {
                { 0, new Tuple<string, string>("<?php", "?>") },
                { 1, new Tuple<string, string>("<?=", "?>") },
                { 2, new Tuple<string, string>("<script", "</script>") }
            };

            List<string> breakConditions = new List<string>()
            {
                ">",
                "/>"
            };

            List<string> breakRestrictions = new List<string>()
            {
                //"?>",         // Not required: Linebreaks only get inserted if a whitespace already exists
                //"</script>" 
            };

            StringBuilder sbXML = new StringBuilder();

            int restrictedFlag = -1;

            for (int i = 0; i < pRawHTML.Length; i++)
            {
                // Check if restricted flag starts
                if (restrictedFlag == -1)
                {
                    foreach (KeyValuePair<int, Tuple<string, string>> restrict in restrictionFlags)
                    {
                        if (restrict.Value.Item1.Length < pRawHTML.Length - i)
                        {
                            if (restrictedFlag == -1 && pRawHTML.Substring(i, restrict.Value.Item1.Length) == restrict.Value.Item1)
                            {
                                restrictedFlag = restrict.Key;
                            }
                        }
                    }
                }
                // Check if restricted flag ends
                else
                {
                    if (pRawHTML.Substring(i - restrictionFlags[restrictedFlag].Item2.Length, restrictionFlags[restrictedFlag].Item2.Length) == restrictionFlags[restrictedFlag].Item2)
                    {
                        restrictedFlag = -1;
                    }
                }


                if (restrictedFlag == -1)
                {
                    bool breakValid = false;

                    // Add line-breaks in HTML-section
                    if (pRawHTML[i] == ' ')
                    {
                        foreach (string condition in breakConditions)
                        {
                            if (i > condition.Length)
                                if (pRawHTML.Substring(i - condition.Length, condition.Length) == condition) breakValid = true;
                        }

                        foreach (string restriction in breakRestrictions)
                        {
                            if (i > restriction.Length)
                                if (pRawHTML.Substring(i - restriction.Length, restriction.Length) == restriction) breakValid = false;
                        }

                        if (breakValid) sbXML.Append("\r\n");
                        else sbXML.Append(' ');
                    }
                    else sbXML.Append(pRawHTML[i]);
                }
                else sbXML.Append(pRawHTML[i]);
            }

            string xmlPrepared = sbXML.ToString();

            sbXML.Clear();



            List<string> tabIncrements = new List<string>()
            {
                @"^\<[\S\s]+?\>$",      // HTML open tags (only in line)
            };

            List<string> tabDecrements = new List<string>()
            {
                @"^\<\/[\S\s]+?\>$",    // HTML close tags (only one in line)
            };

            List<string> tabIgnores = new List<string>()
            {
                @"^\<\?[\S\s]+?\?\>$",                  // Ignore PHP-Tags
                @"^\<script[\S\s]+?\<\/script\>$",      // Ignore JS-Blocks (short)
                @"^\<\![\S\s]+?\>$",                    // HTML Comments and doctype
                @"^\<(?!\?)[\S\s]+?(?<!\?)\/\>$",       // HTML Single line statements (with PHP included)
                @"^\<[\S\s]+?\>[\S\s]*?\<\/[\S\s]+?\>$" // HTML inline open and close
            };

            int tabCount = 0;

            // Trim each line and add indents
            foreach (string line in xmlPrepared.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                string lineTrim = line.Trim();
                bool ignoreTab = false;
                short tabChanged = 0;

                // Check for ignore cases
                foreach (string tabIgnore in tabIgnores)
                    if (Regex.IsMatch(lineTrim, tabIgnore)) ignoreTab = true;

                if (!ignoreTab)
                {
                    // Check for decrement cases
                    foreach (string tabDecrement in tabDecrements)
                        if (tabChanged == 0 && Regex.IsMatch(lineTrim, tabDecrement))
                        {
                            tabChanged = -1;
                        }

                    // Check for increment cases
                    foreach (string tabIncrement in tabIncrements)
                        if (tabChanged == 0 && Regex.IsMatch(lineTrim, tabIncrement))
                        {
                            tabChanged = 1;
                        }
                }

                // Decrement tab count
                if (tabChanged == -1 && tabCount > 0) tabCount--;

                // Print line
                for (int i = 0; i < tabCount; i++) sbXML.Append('\t');
                sbXML.AppendFormat("{0}{1}", lineTrim, "\r\n");

                // Increment tab count
                if (tabChanged == 1) tabCount++;
            }

            return sbXML.ToString();
        }

        /// <summary>
        /// Adds a copyright note to the file
        /// </summary>
        private static string AddCopyrightNotice(string pContent, bool isMinOutput)
        {
            string copyrightNote = $"<!-- CREATED USING LPHP VERSION { typeof(LPHPCore.LPHPCompiler).Assembly.GetName().Version.ToString(3) } BY ENDEV. COPYRIGHT 2020 TOBIAS HATTINGER. https://endev.at/p/LPHP -->";

            if (isMinOutput) return string.Format("{1} {0}", copyrightNote, pContent);
            else return string.Format("{1}\r\n{0}", copyrightNote, pContent);

        }
    }
}
