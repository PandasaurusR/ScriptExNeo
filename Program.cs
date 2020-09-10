using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// ScriptExNeo - System Integration Assistant
/// Haohan Liu (c) 2020
/// </summary>
namespace ScriptExNeo {

    class Program {

        // Program properties
        public static string Version = "20.09.10";
        public static string Name = "ScriptExNeo";
        public static string MOTD = "Well would you look at that!";

        // Program configuration
        public static string ConfigFile = "AppConfig.yml";
        public static object Config = null;

        static void Main(string[] args) {
            // Initialise crash handler
            CrashHandler.Inject();

            // Handle CLI execution
            if (args.Length > 0) {
                Console.WriteLine($"ScriptExNeo [{Version}] does not currently support CLI operations.");
            }

            // Handle typical execution
            Program.Start();
        }

        /// <summary>
        /// Initialise the program
        /// </summary>
        private static void Start() {

        }
    }

    /// <summary>
    /// Custom unhandled exception handler for graceful program termination
    /// </summary>
    static class CrashHandler {

        /// <summary>
        /// Attach crash handler to program
        /// </summary>
        public static void Inject() {
            // Redirect unhandled exception to custom handler
            // See: https://stackoverflow.com/questions/31366174/enable-console-window-to-show-exception-error-details
            if (!System.Diagnostics.Debugger.IsAttached) {
                AppDomain.CurrentDomain.UnhandledException += ReportUnhandledException;
            }
        }

        /// <summary>
        /// Report unexpected exception to console
        /// </summary>
        private static void ReportUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Console.WriteLine('\n' + new String('=', 119));
            Console.WriteLine("PROGRAM HAS TERMINATED UNEXPECTEDLY. STACKTRACE IS BELOW.");
            Console.WriteLine(new String('=', 119));
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Please press any key to end the program...");
            Console.WriteLine(new String('=', 119));
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
