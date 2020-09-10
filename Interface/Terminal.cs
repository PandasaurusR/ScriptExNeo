using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Logging;

namespace ScriptExNeo.Interface {

    /// <summary>
    /// Provides tools for formatting output and gathering 
    /// </summary>
    public static class Terminal {
        public static string Input { get; set; }

        public static void Start() {

            // Begin terminal input output loop
            while (true) {
                ReadInput();
                break;
            }
        }

        /// <summary>
        /// Read and log user input
        /// </summary>
        static void ReadInput() {

            string _input = Console.ReadLine();

            Input = _input;
            Program.Log.Add($"<= '{_input}'");
        }
    }
}
