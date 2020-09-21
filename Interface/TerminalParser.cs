using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Configuration;
using ScriptExNeo.Handlers;

namespace ScriptExNeo.Interface {
    /// <summary>
    /// Tools for conversion between input strings and command objects
    /// </summary>
    static class TerminalParser {

        /// <summary>
        /// Validate commands string and convert into individual items.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> ParseCommands(string input, ModeConfig mode) {
            // Initialise new list
            List<string> _cmdlist = new List<string>();

            // Check for shell calls with parameters
            if (IsValidFullInvoke(input)) {
                _cmdlist.Add(input);
                return _cmdlist;
            }

            // Simulate current mode and mode switches
            ModeConfig _curMode = mode;

            // Check each command item
            foreach (string _cmd in input.Trim().Split(' ')) {
                // Ignore empty entries
                if (_cmd == "") {
                    continue;
                }
                // Check for batch commands
                if (IsValidBatchKey(_cmd)) {
                    _cmdlist.Add(_cmd);
                    continue;
                }

                // Check for mode changes
                if (IsValidModeSwitch(_cmd)) {
                    _cmdlist.Add(_cmd);
                    _curMode = TerminalMode.GetMode(_cmd);
                    continue;
                }

                // Check for invoke command
                if (IsValidInvoke(_cmd)) {
                    _cmdlist.Add(_cmd);
                    continue;
                }

                // Check for valid macro
                if (IsValidMacro(_cmd)) {
                    MacroItem _macro = Program.Config.Macros[_cmd];
                    // Check for infinite recursion
                    if (_macro.Command.Contains(_cmd)) {
                        throw new BadConfigException($"Macro {_cmd} references self. Unable to resolve.");
                    }
                    // Generate mode key
                    string _mode = $"{Program.Config.Program.ModeKey}{_macro.SetMode} ";

                    // Recursively resolve macro
                    _cmdlist.AddRange(ParseCommands(_mode + _macro.Command, _curMode));
                    continue;
                }

                // Check for valid command
                if (IsValidCommand(_cmd, _curMode)) {
                    _cmdlist.Add(_cmd);
                    continue;
                }

                // Write to terminal of invalid command
                Terminal.WriteError(
                    $"'{_cmd}' is not a valid command in '{TerminalMode.GetModeName(_curMode)} Mode'."
                );

                // No valid command has been found.
                // If not ignoring, return null
                if (!Program.Config.Program.SkipInvalidCommands) {
                    return null;
                }
            }

            return _cmdlist;
        }

        #region # Validators

        /// <summary>
        /// Validate Invoke command. Assume '/' is the InvokeKey.
        /// '/(string)' calls the shell with no parameters.
        /// </summary>
        private static bool IsValidInvoke(string command) {
            if (command.StartsWith(Program.Config.Program.InvokeKey)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate Invoke command. Assume '/' is the InvokeKey.
        /// '/(string) [parameters] ...' calls the shell with parameters.
        /// </summary>
        private static bool IsValidFullInvoke(string input) {
            if (IsValidInvoke(input) && input.Count(f => f == '/') == 1) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate Batch command. Assume '|' is the BatchKey.
        /// '|' is a batch command.
        /// '|(Int)' is valid formatting for an Int delay.
        /// </summary>
        private static bool IsValidBatchKey(string command) {
            if (command.StartsWith(Program.Config.Program.BatchKey) &&
                command.Substring(1).All(char.IsDigit)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate Mode switch command. Assume '!' is the ModeKey. 
        /// '!' switches to default mode.
        /// '!(String)' is valid formatting to switch to (String) mode.
        /// </summary>
        private static bool IsValidModeSwitch(string command) {
            // Check if command is formatted correctly
            if (!command.StartsWith(Program.Config.Program.ModeKey)) {
                return false;
            }
            // Check for default mode switch
            if (command.Equals(Program.Config.Program.ModeKey)) {
                return true;
            }
            // Check if mode exists in config object
            if (Program.Config.Program.Modes.ContainsKey(command.Substring(1))) {
                return true;
            }
            // Mode not found
            return false;
        }

        /// <summary>
        /// Validate macro.
        /// </summary>
        private static bool IsValidMacro(string command) {
            if (Program.Config.Macros.ContainsKey(command)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate command. 
        /// </summary>
        private static bool IsValidCommand(string command, ModeConfig config) {
            if (config.Commands.ContainsKey(command)) {
                return true;
            }
            return false;
        }

        #endregion

    }
}
