using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Logging;

namespace ScriptExNeo.Interface {

    /// <summary>
    /// Provides tools for formatting output and gathering 
    /// </summary>
    public static class Terminal {

        /// <summary>
        /// The last submitted input string
        /// </summary>
        public static string Input { get; set; }

        /// <summary>
        /// Is true if the terminal is awaiting user input
        /// </summary>
        public static bool IsAwaitingInput { get; set; }

        public static void Start() {
            // Initialise Terminal
            Theme.Apply();
            IsAwaitingInput = false;

            // Begin terminal input output loop
            while (true) {
                ReadInput();

                WriteError(Input);
                WriteAlert(Input);
                WriteInfo(Input);
                WriteLine(Input);

                Console.WriteLine(Program.Log.ToString());
            }
        }

        /// <summary>
        /// Read and log user input
        /// </summary>
        static void ReadInput() {
            // Set formatting for accepting user input
            IsAwaitingInput = true;
            Console.Write("> ");
            string _input = Console.ReadLine();
            IsAwaitingInput = false;

            // Save and log user input
            Input = _input;
            Program.Log.Add($"<= '{_input}'");
        }

        #region # Terminal Output
        
        static void Write(string message, string prefix) {
            Console.Write($"[{prefix}] {message}");
            Program.Log.Add($"=> [{prefix}] {message}");
        }

        public static void WriteError(string message) {
            Terminal.Write(message + '\n', "!");
        }

        public static void WriteAlert(string message) {
            Terminal.Write(message + '\n', "?");
        }

        public static void WriteInfo(string message) {
            Terminal.Write(message + '\n', "-");
        }

        public static void WriteLine(string message) {
            Terminal.Write(message + '\n', "*");
        }

        #endregion
    }
}
