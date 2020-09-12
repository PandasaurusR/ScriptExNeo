using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ScriptExNeo.Interface.Shelleton.Commands {
    public static class DefaultCommands {

        #region # Terminal functionality
        /// <summary>
        /// Show shelleton help
        /// </summary>
        public static void h() {
            Terminal.WriteLineBreak();
            Console.WriteLine(" SHELLETON COMMANDS (<> Required argument, [] Optional argument):");
            PrintCommands();
        }

        /// <summary>
        /// Serlialize loaded configuration and show
        /// </summary>
        public static void showconfig() {
            // Serialize loaded configuration
            var _s = new Serializer();
            string _dump = _s.Serialize(Program.Config);

            // Write to terminal
            Terminal.WriteLineBreak();
            Terminal.Write(_dump);
            Terminal.WriteLineBreak();
        }

        #endregion

        #region # Helper functionality
        static void PrintCommands() {
            Terminal.WriteLineBreak();
            // Iterate through each command
            foreach (var key in Shell.GetCommands()) {
                // Create list of arguments
                List<string> Arguments = new List<string>(key.RequiredArgs);
                Arguments.AddRange(key.OptionalArgs);

                // Display command
                Console.WriteLine(
                    string.Format(
                        " {0, -35} │ {1}",
                        key.Name, 
                        string.Join(" ", Arguments)
                    )
                );
            }
            Terminal.WriteLineBreak();
        }
        #endregion
    }
}
