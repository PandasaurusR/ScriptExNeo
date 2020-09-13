using ScriptExNeo.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Interface;

namespace ScriptExNeo.Handlers {

    /// <summary>
    /// Functionality to convert string commands to command objects
    /// </summary>
    public class TerminalHandler {

        private Config Config { get; set; }

        private List<string> CommandQueue { get; set; }

        public ModeConfig CurrentMode { get; set; }

        /// <summary>
        /// CommandHandler constructor
        /// </summary>
        public TerminalHandler(Config config, string mode="") {
            // Initialise
            Config = config;

            // Initialise starting mode
            CurrentMode = GetMode(mode);
            if (CurrentMode is null) {
                CurrentMode = GetMode(Config.Program.DefaultMode);
            }
        }

        #region # Command parsing and validation

        /// <summary>
        /// Validate commands string and convert into individual items.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<string> ParseCommands(string input) {
            // Initialise new list
            List<string> _cmdlist = new List<string>();

            // Check for shell calls with parameters
            if (IsValidFullInvoke(input)) {
                _cmdlist.Add(input);
                return _cmdlist;
            }

            // Simulate mode switches
            ModeConfig _curMode = CurrentMode;

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
                    _curMode = this.GetMode(_cmd);
                    continue;
                }

                // Check for invoke command
                if (IsValidInvoke(_cmd)) {
                    _cmdlist.Add(_cmd);
                    continue;
                }

                // Check for valid macro
                if (IsValidMacro(_cmd)) {
                    MacroItem _macro = Config.Macros[_cmd];
                    // Check for infinite recursion
                    if (_macro.Command.Contains(_cmd)) {
                        throw new BadConfigException($"Macro {_cmd} references self. Unable to resolve.");
                    }
                    // Generate mode key
                    string _mode = $"{Config.Program.ModeKey}{_macro.SetMode} ";

                    // Recursively resolve macro
                    _cmdlist.AddRange(ParseCommands(_mode + _macro.Command));
                    continue;
                }

                // Check for valid command
                if (IsValidCommand(_cmd, _curMode)) {
                    _cmdlist.Add(_cmd);
                    continue;
                }

                // No valid command has been found.
                // If not ignoring, return null
                if (!Config.Program.SkipInvalidCommands) {
                    Terminal.WriteError($"'{_cmd}' is not a valid command in the current mode.");
                    return null;
                }
            }

            return _cmdlist;
        }

        /// <summary>
        /// Validate Invoke command. Assume '/' is the InvokeKey.
        /// '/(string)' calls the shell with no parameters.
        /// </summary>
        private bool IsValidInvoke(string command) {
            if (command.StartsWith(Config.Program.InvokeKey)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate Invoke command. Assume '/' is the InvokeKey.
        /// '/(string) [parameters] ...' calls the shell with parameters.
        /// </summary>
        private bool IsValidFullInvoke(string input) {
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
        private bool IsValidBatchKey(string command) {
            if (command.StartsWith(Config.Program.BatchKey) &&
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
        private bool IsValidModeSwitch(string command) {
            // Check if command is formatted correctly
            if (!command.StartsWith(Config.Program.ModeKey)) {
                return false;
            }
            // Check if mode exists in config object
            if (Config.Program.Modes.ContainsKey(command.Substring(1))) {
                return true;
            }
            // Mode not found
            return false;
        }

        /// <summary>
        /// Validate macro.
        /// </summary>
        private bool IsValidMacro(string command) {
            if (Config.Macros.ContainsKey(command)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validate command. 
        /// </summary>
        private bool IsValidCommand(string command, ModeConfig config) {
            if (config.Commands.ContainsKey(command)) {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Retrieve requested ModeConfig object from Config, or null if not
        /// able to be located.
        /// </summary>
        private ModeConfig GetMode(string mode) {

            // Clean command formatting
            if (mode.StartsWith(Config.Program.ModeKey)) {
                mode = mode.Substring(1);
            }

            // Store mode long name
            string _mode;

            // Check program mode directory
            if (Config.Program.Modes.ContainsKey(mode)) {
                _mode = Config.Program.Modes[mode];
            }
            else {
                return null;
            }

            // Return requested mode
            if (Config.Modes.ContainsKey(_mode)) {
                return Config.Modes[_mode];
            }
            // If mode does not exist, configuration file is problematic
            else {
                ExceptionHandler.ReportException(
                    new BadConfigException($"'{_mode}' mode not configured properly in configuration file."), 
                    "Missing mode in config"
                );
                return null;
            }
        }
    }
}
