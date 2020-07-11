using System;

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


        private static Print printDebug = null;

        /// <summary>
        /// Prints and logs debug information
        /// </summary>
        public static Print PrintDebug
        {
            get
            {
                if (printDebug == null) return LPHPBlankOutput;
                else return printDebug;
            }
            set => printDebug = value;
        }

        /// <summary>
        /// Print-Delegate to disable all log and debug information.
        /// </summary>
        /// <param name="pMessage">Message-string to be shown and logged</param>
        /// <param name="pType">Messagetype</param>
        private static void LPHPBlankOutput(string pMessage, LPHPMessageType pType) { }

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
    }
}
