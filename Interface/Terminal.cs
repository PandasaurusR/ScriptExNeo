using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
                Shelleton.Shell.Run(Input);
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
        
        // LINE OUTPUT
        public static void Write(string text) {
            Console.Write(text);
        }

        static void Write(string text, string prefix) {
            Console.Write($"[{prefix}] {text}");
            Program.Log.Add($"=> [{prefix}] {text}");
        }

        public static void WriteError(string text) {
            Terminal.Write(text + '\n', "!");
        }

        public static void WriteAlert(string text) {
            Terminal.Write(text + '\n', "?");
        }

        public static void WriteInfo(string text) {
            Terminal.Write(text + '\n', "-");
        }

        public static void WriteLine(string text) {
            Terminal.Write(text + '\n', "*");
        }

        // SPECIFIC OUTPUT

        /// <summary>
        /// Write text at specific console coordinate offsets.
        /// </summary>
        public static void WriteAt(int x, int y, string text) {
            // Store initial cursor position
            int _x = Console.CursorLeft;
            int _y = Console.CursorTop;

            // Write text block
            foreach (string line in text.Split('\n')) {
                Console.SetCursorPosition(x, y);
                Console.Write(line);
                y += 1;
            }

            // Reset cursor position
            Console.SetCursorPosition(_x, _y);
        }

        /// <summary>
        /// Print horizontal line break with the character provided
        /// </summary>
        public static void WriteLineBreak(char c='-') {
            Console.WriteLine(new String(c, 119));
        }


        #endregion
    }
}
