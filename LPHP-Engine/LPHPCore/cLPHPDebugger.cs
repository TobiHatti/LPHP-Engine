using System;
using System.Net;

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

        public class DebugOutputs
        {
            public static void None(string pMessage, LPHPMessageType pType) { }
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
            }
        }
    }
}
