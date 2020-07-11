using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPHP_Preprocessor
{
    public enum LPHPMessageType
    {
        LPHPMessage,
        LPHPSuccess,
        LPHPWarning,
        LPHPError
    }

    public class LPHPDebugger
    {
        public delegate void Print(string pMessage, LPHPMessageType pType);
        private static Print printDebug = null;
        public static Print PrintDebug 
        { 
            get
            {
                if (printDebug == null)
                {
                    return LPHPBlankOutput;
                }
                else return printDebug;
            }
            set => printDebug = value;
        }

        private static void LPHPBlankOutput(string pMessage, LPHPMessageType pType) { }

        public static void PrintMessage(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPMessage);

        public static void PrintSuccess(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPSuccess);

        public static void PrintWarning(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPWarning);
        //{
        //    Console.ForegroundColor = ConsoleColor.Yellow;
        //    Console.WriteLine(pMessage);
        //    Console.ForegroundColor = ConsoleColor.White;
        //}

        public static void PrintError(string pMessage)
            => PrintDebug(pMessage, LPHPMessageType.LPHPError);
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.BackgroundColor = ConsoleColor.DarkRed;
        //    Console.WriteLine(pMessage);
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.BackgroundColor = ConsoleColor.Black;
        //}
    }
}
