﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace LPHP_Preprocessor
{
    public class LPHPCompiler
    {
        private static readonly List<string> instructionSessionBuffer = new List<string>();
        private static readonly Dictionary<string, string> fileBuffer = new Dictionary<string, string>();
        private static readonly Dictionary<string, object> localVariables = new Dictionary<string, object>();
        private static readonly Dictionary<string, object> globalVariables = new Dictionary<string, object>();

        private static string currentCompileFile = "";

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
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(pFilePath))
            {
                string line;

                // Read each line and remove comments
                while ((line = sr.ReadLine()) != null)
                    sb.Append(Regex.Replace(line, @"\/\*[\s\S]*?\*\/|\/\/.*", "") + " ");
                sr.Close();
            }

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
            bool phpATagActive = false; // <?php ?>
            bool phpBTagActive = false; // <?= ?>

            string phpATestTag = "<?php";
            string phpBTestTag = "<?=";
            string phpEndTag = "?>";

            char commonStartChar = '<'; // <?php, <?=
            char commonEndChar = '?';    // ?>

            int phpATestTagLength = phpATestTag.Length;
            int phpBTestTagLength = phpBTestTag.Length;
            int phpEndTagLength = phpEndTag.Length;

            int longestTestTag = Math.Max(phpATestTagLength, Math.Max(phpBTestTagLength, phpEndTagLength));

            char lastChar = '\0';

            int remainingContentLength = 0;


            // Note: SubString is slow. Really. Slow.

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < pFileContent.Length; i++)
            {
                // Calculate remaining length
                remainingContentLength = pFileContent.Length - i;

                // Test if checks can still be made
                if (remainingContentLength >= longestTestTag)
                {
                    // Check if php-tag is active
                    if (!phpATagActive && !phpBTagActive)
                    {
                        // Check if common tag is set (runtime-improvement)
                        if (pFileContent[i] == commonStartChar)
                        {
                            // Check for tag-starts
                            if (pFileContent.Substring(i, phpATestTagLength) == phpATestTag) phpATagActive = true;
                            else if (pFileContent.Substring(i, phpBTestTagLength) == phpBTestTag) phpBTagActive = true;
                        }
                    }

                    // Check if pdp-tag is active, check common end-char (runtime-improvement), check for exact end-tag
                    if ((phpATagActive || phpBTagActive) && pFileContent[i] == commonEndChar && pFileContent.Substring(i - phpEndTagLength, phpEndTagLength) == phpEndTag)
                    {
                        if (phpATagActive) phpATagActive = false;
                        if (phpBTagActive) phpBTagActive = false;
                    }
                }
     
                // Check if php-tag active
                if(!phpATagActive && !phpBTagActive)
                {
                    // Skip duplicate whitespaces
                    if(!(pFileContent[i] == ' ' && lastChar == ' ')) sb.Append(pFileContent[i]);
                }
                else sb.Append(pFileContent[i]);

                lastChar = pFileContent[i];
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
            bool minOutput = true;

            string targetFile = Path.Combine(Path.GetDirectoryName(pOriginalFilename), Path.GetFileNameWithoutExtension(pOriginalFilename) + ".php");

            using (StreamWriter sw = new StreamWriter(targetFile))
            {
                if (minOutput) sw.WriteLine(pFileContent);
                // TODO
                else sw.WriteLine(pFileContent);
                sw.Close();
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
