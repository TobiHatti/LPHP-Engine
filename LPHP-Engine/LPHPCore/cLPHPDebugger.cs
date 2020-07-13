using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;


//LPHP Layout Engine
//Copyright(C) 2020 Tobias Hattinger

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<https://www.gnu.org/licenses/>.
namespace LPHPCore
{
    /// <summary>
    /// Debug-Message types for LPHPDebugger
    /// </summary>
    public enum LPHPMessageType
    {
        LPHPMessage,
        LPHPSuccess,
        LPHPWarning,
        LPHPError
    }

    /// <summary>
    /// Provides Debug information and logs debug-data.
    /// </summary>
    public class LPHPDebugger
    {
        public const string LPHPLogFile = "compile.lphp.log";

        /// <summary>
        /// Prints and logs debug information
        /// </summary>
        /// <param name="pMessage">Message-string to be shown and logged</param>
        /// <param name="pType">Messagetype</param>
        public delegate void Print(string pMessage, LPHPMessageType pType);

        /// <summary>
        /// Flag to create a log file with detailed compiler-output
        /// </summary>
        public static bool CreateLogFile { get; set; } = true;

        private static Print printDebug = null;

        /// <summary>
        /// Prints and logs debug information
        /// </summary>
        public static Print PrintDebug
        {
            get
            {
                if (printDebug == null) return DebugOutputs.None;
                else return printDebug;
            }
            set => printDebug = value;
        }

        /// <summary>
        /// Prints and logs a generic message
        /// </summary>
        /// <param name="pMessage">Message-string to be shown and logged</param>
        public static void PrintMessage(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPMessage);

        /// <summary>
        /// Prints and logs a success-message 
        /// </summary>
        /// <param name="pMessage">Message-string to be shown and logged</param>
        public static void PrintSuccess(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPSuccess);

        /// <summary>
        /// Prints and logs a warning-message
        /// </summary>
        /// <param name="pMessage">Message-string to be shown and logged</param>
        public static void PrintWarning(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPWarning);

        /// <summary>
        /// Prints and logs an error-message
        /// </summary>
        /// <param name="pMessage">Message-string to be shown and logged</param>
        public static void PrintError(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPError);

        /// <summary>
        /// Logs the provided message to the LPHP log-file
        /// </summary>
        /// <param name="pMessage">Message-string to be logged</param>
        /// <param name="pType">Messagetype</param>
        public static void LogDebugData(string pMessage, LPHPMessageType pType)
        {
            try
            {
                if ((bool)LPHPCompiler.COMPOPT["ENABLE_CONSOLE_LOG"])
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(LPHPWatchdog.ProjectRoot, LPHPLogFile), true))
                    {
                        sw.WriteLine($"[{DateTime.Now:yyyy-MM-ddTHH:mm:ss}]<{pType}> {pMessage}");
                    }
                }
                else File.Delete(Path.Combine(LPHPWatchdog.ProjectRoot, LPHPLogFile));
            }
            catch { }
        }

        /// <summary>
        /// Provides pre-defined log and output-methods for the debugger
        /// </summary>
        public class DebugOutputs
        {
            /// <summary>
            /// Disables all logging-systems
            /// </summary>
            /// <param name="pMessage">Message-string to be shown and logged</param>
            /// <param name="pType">Messagetype</param>
            public static void None(string pMessage, LPHPMessageType pType) { }

            /// <summary>
            /// Shows the debug-info in the debug-window
            /// </summary>
            /// <param name="pMessage">Message-string to be shown and logged</param>
            /// <param name="pType">Messagetype</param>
            public static void ToDebug(string pMessage, LPHPMessageType pType)
            {
                Debug.Print(pMessage);
                LogDebugData(pMessage, pType);
            }

            /// <summary>
            /// Shows the debug-info in the console-window
            /// </summary>
            /// <param name="pMessage">Message-string to be shown and logged</param>
            /// <param name="pType">Messagetype</param>
            public static void ToConsole(string pMessage, LPHPMessageType pType)
            {
                // Save previous console color
                ConsoleColor foreColor = Console.ForegroundColor;
                ConsoleColor backColor = Console.BackgroundColor;

                // Custom console color style based on message-type
                switch(pType)
                {
                    case LPHPMessageType.LPHPSuccess:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;
                    case LPHPMessageType.LPHPWarning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LPHPMessageType.LPHPError:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        break;
                }

                // Output message
                Console.WriteLine(pMessage);

                // Reset to old console colors
                Console.ForegroundColor = foreColor;
                Console.BackgroundColor = backColor;

                LogDebugData(pMessage, pType);
            }
        }
    }
}
