using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScriptExNeo.Interface;
using ScriptExNeo.Interface.Pages;
using ScriptExNeo.Interface.Shelleton;
using ScriptExNeo.Tools;
using ScriptExNeo.Configuration;
using ScriptExNeo.Handlers;

/// <summary>
/// ScriptExNeo - System Integration Assistant
/// Haohan Liu (c) 2020
/// </summary>
namespace ScriptExNeo {

    class Program {

        // Program properties
        public static readonly string Copyright = "Haohan Liu (c) 2020";
        public static readonly string Version = "20.09.10";
        public static readonly string Name = "ScriptExNeo";
        public static readonly string MOTD = "Well would you look at that!";

        // Program configuration
        public static readonly string ConfigFile = "AppConfig.yml";
        public static readonly Config Config = ConfigDeserializer.Deserialize(ConfigFile);

        // Program logging
        public static readonly string LogFile = "ScriptExNeo.log";
        public static Log Log = new Log();

        static void Main(string[] args) {
            // Initialise crash handler
            ExceptionHandler.Inject();

            // Handle CLI execution
            if (args.Length > 0) {
                Console.WriteLine($"ScriptExNeo [{Version}] does not currently support CLI operations.");
                Environment.Exit(1);
            }

            // Handle typical execution
            Program.Start();
        }

        /// <summary>
        /// Initialise the program
        /// </summary>
        private static void Start() {
            Log.Add("*** PROGRAM INITIALISATION ***");

            Shell.Initialise();

            TitlePage.Display();

            Terminal.Start();
        }
    }

    
}
