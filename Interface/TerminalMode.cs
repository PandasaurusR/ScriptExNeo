using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptExNeo.Configuration;
using ScriptExNeo.Handlers;

namespace ScriptExNeo.Interface {
    /// <summary>
    /// Tools for manipulating the current operating mode of the terminal
    /// </summary>
    static class TerminalMode {

        public static ModeConfig ActiveModeConfig { get; set; }
        public static string ActiveModeKey { get; set; }
        public static string ActiveModeName { get; set; }

        /// <summary>
        /// Initialise default operating mode
        /// </summary>
        static TerminalMode() {
            if (!SwitchMode(Program.Config.Program.DefaultMode)) {
                throw new BadConfigException($"DefaultMode flag '{Program.Config.Program.DefaultMode}' is not a valid mode.");
            }
        }
        
        #region # Mode manipulation
        /// <summary>
        /// Switch terminal modes. Returns true if successful, false otherwise.
        /// </summary>
        public static bool SwitchMode(string mode) {
            if (Program.Config.Program.Modes.ContainsKey(mode)) {
                ActiveModeKey = mode;
                ActiveModeName = GetModeName(mode);
                ActiveModeConfig = GetMode(mode);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieve requested ModeConfig object from Config, or null if not
        /// able to be located.
        /// </summary>
        public static ModeConfig GetMode(string mode) {

            // Clean command formatting
            if (mode.StartsWith(Program.Config.Program.ModeKey)) {
                mode = mode.Substring(1);
            }

            // Store mode long name
            string _mode;

            // Check program mode directory
            if (Program.Config.Program.Modes.ContainsKey(mode)) {
                _mode = Program.Config.Program.Modes[mode];
            }
            else {
                return null;
            }

            // Return requested mode
            if (Program.Config.Modes.ContainsKey(_mode)) {
                return Program.Config.Modes[_mode];
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

        /// <summary>
        /// Return the string name of the mode given.
        /// </summary>
        public static string GetModeName(ModeConfig mode) {
            return Program.Config.Modes.Keys.Where(k => Program.Config.Modes[k] == mode).FirstOrDefault();
        }

        /// <summary>
        /// Return the string name of modekey given. NULL if not found.
        /// </summary>
        public static string GetModeName(string modekey) {
            if (Program.Config.Program.Modes.TryGetValue(modekey, out string val)) {
                return val;
            }
            return null;
        }

        #endregion

    }
}
