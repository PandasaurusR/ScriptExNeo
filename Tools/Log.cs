using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptExNeo.Tools {

    /// <summary>
    /// Provides logging functionality to the terminal
    /// </summary>
    public class Log {
        List<string> Content { get; set; }

        public Log() {
            Content = new List<string>();
        }

        /// <summary>
        /// Add an entry into the log
        /// </summary>
        /// <param name="log"></param>
        public void Add(string log) {
            string _time = $"[{DateTime.Now.ToLongTimeString()}] ";

            Content.Add(_time + log.Trim('\n'));
        }

        /// <summary>
        /// Dump log into file, reset in-memory log storage
        /// </summary>
        /// <param name="filename"></param>
        public void Dump(string filename) {
            using (StreamWriter w = File.AppendText(filename)) {
                w.WriteLine(this.ToString());
            }
            Content = new List<string>();
        }

        /// <summary>
        /// Convert log object into one big string
        /// </summary>
        public override string ToString() {
            StringBuilder _log = new StringBuilder();

            foreach (string log in Content) {
                _log.AppendLine(log);
            }

            return _log.ToString();
        }
    }
}
