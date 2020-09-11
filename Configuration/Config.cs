using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;

namespace ScriptExNeo.Configuration {

    /// <summary>
    /// Program configuration object
    /// </summary>
    public class Config {
        public ProgramConfig Program { get; set; }
        public Dictionary<string, ModeConfig> Modes { get; set; }
        public Dictionary<string, MacroItem> Macros { get; set; }
    }

    /// <summary>
    /// Program configuration object
    /// </summary>
    public class ProgramConfig {
        /// ROBOCOPY CONFIG
        // Local
        public string SrcDriveName { get; set; }
        public DriveType SrcDriveType { get; set; }
        public string SrcDriveRoot { get; set; }

        // Remote
        public bool SrcUseNetwork { get; set; }
        public string SrcNetworkRoot { get; set; }

        // Destination
        public string DstDriveRoot { get; set; }

        // Debug
        public bool ForceSrcDriveLetter { get; set; }
        public char SrcDriveLetter { get; set; }

        /// STARTUP
        public List<string> Startup { get; set; }

        /// COMMAND BEHAVIOUR
        public bool SkipInvalidCommands { get; set; }
        public int CommandExecutionDelay { get; set; }

        /// TERMINAL BINDINGS
        public char InvokeKey { get; set; }
        public char ModeKey { get; set; }
        public char HelpKey { get; set; }
        public char BatchKey { get; set; }

        /// MODE DIRECTORY
        public Dictionary<char, string> Modes { get; set; }
        public char DefaultMode { get; set; }
    }

    /// <summary>
    /// Macro configuration object
    /// </summary>
    public class MacroItem {
        public string Name { get; set; }
        public char SetMode { get; set; }
        public string Command { get; set; }
    }

    /// <summary>
    /// Mode configuration object
    /// </summary>
    public class ModeConfig {
        public bool SrcCopy { get; set; }
        public string SrcLocal { get; set; }
        public string DstLocal { get; set; }
        public string SrcNetwork { get; set; }
        public List<string> Categories { get; set; }

        public Dictionary<string, CommandItem> Commands { get; set; }
    }

    /// <summary>
    /// Command configuration object
    /// </summary>
    public class CommandItem {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Exec { get; set; }
        public string Args { get; set; }
        public string SrcPath { get; set; }
        public string NetPath { get; set; }
        public int Delay { get; set; }
        public bool IgnoreBatch { get; set; }
    }
}
