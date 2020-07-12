using LPHPCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPHPConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string watchDirectory = null;

            ShowStartupBanner();

            // Initialize LPHP-Compiler
            LPHPCompiler.Init();

            // Set LPHP-Debug Mode
            LPHPDebugger.PrintDebug = LPHPDebugger.DebugOutputs.ToConsole;

            // Enable the creation of a log file
            LPHPDebugger.CreateLogFile = true;

            if (args.Length > 0)
            {
                if (Directory.Exists(args[0].ToString()))
                {
                    watchDirectory = args[0].ToString();
                }
                else
                {
                    LPHPDebugger.PrintError("*** LPHP Startup Error ***");
                    LPHPDebugger.PrintError("The path provided is not a valid directory.");
                }
            }
            else
            {
                LPHPDebugger.PrintWarning("No path provided upon startup. Please provide a path to your LPHP-project at startup, or enter it below:");

                do
                {
                    Console.Write("LPHP Project Path > ");
                    watchDirectory = Console.ReadLine();
                    Console.WriteLine("");

                    if(!Directory.Exists(watchDirectory))
                    {
                        LPHPDebugger.PrintError("The entered path is not valid! Please try again.");
                    }
                }
                while (!Directory.Exists(watchDirectory));
            }

            Console.Write("Watching directory \"");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(watchDirectory);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\"");

            LPHPWatchdog.Init(watchDirectory);

            // Run the LPHP-Watchdog on the given directory
            LPHPWatchdog.Run();
        }

        private static void ShowStartupBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("=====================================================");
            Console.WriteLine("*                 LPHP Preprocessor                 *");
            Console.WriteLine("*                Version " + typeof(LPHPCore.LPHPCompiler).Assembly.GetName().Version.ToString(3) + " ALPHA                *");
            Console.WriteLine("*        (c) Copyright 2020 Tobias Hattinger        *");
            Console.WriteLine("*                                                   *");
            Console.WriteLine("*                       Visit                       *");
            Console.Write("*              ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("https:/endev.at/p/LPHP");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("               *");
            Console.WriteLine("*                    for updates                    *");
            Console.WriteLine("=====================================================\r\n\r\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
