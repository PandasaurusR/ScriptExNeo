using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Assets;

namespace ScriptExNeo.Handlers {

    /// <summary>
    /// Custom unhandled exception handler for graceful program termination
    /// </summary>
    static class ExceptionHandler {

        /// <summary>
        /// Attach crash handler to program
        /// </summary>
        public static void Inject() {
            // Redirect unhandled exception to custom handler
            // See: https://stackoverflow.com/questions/31366174/enable-console-window-to-show-exception-error-details
            if (!System.Diagnostics.Debugger.IsAttached) {
                AppDomain.CurrentDomain.UnhandledException += ReportUnhandledException;
            }
            else {
                Program.Log.Add("* PROGRAM HOOKED INTO EXTERNAL DEBUGGER *");
            }
        }

        /// <summary>
        /// Report unexpected exception to console
        /// </summary>
        private static void ReportUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            if (Program.Log != null) {
                Program.Log.Add("*** UNHANDLED EXCEPTION ***");
                Program.Log.Add(e.ExceptionObject.ToString());
            }

            Console.WriteLine('\n' + new String('=', 119));
            Console.WriteLine(Asset.CrashText);
            Console.WriteLine(new String('=', 119));
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine(new String('-', 119));
            Console.WriteLine("Please press any key to end the program...");
            Console.WriteLine(new String('-', 119));
            Console.ReadKey();
            ExceptionHandler.Exit(1);
        }

        /// <summary>
        /// Report an exception and log details. Does not terminate program.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="message"></param>
        public static void ReportException(Exception e, string message) {
            if (Program.Log != null) {
                Program.Log.Add($"** {message.ToUpper()} **");
                Program.Log.Add(e.StackTrace);
            }
        }

        /// <summary>
        /// Terminate the program with code and dump log
        /// </summary>
        public static void Exit(int code) {
            if (Program.Log != null) {
                Program.Log.Add("*** PROGRAM TERMINATED ***");
                Program.Log.Dump(Program.LogFile);
            }
            Environment.Exit(code);
        }
    }
}
