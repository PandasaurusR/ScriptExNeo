using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ScriptExNeo.Tools {
    /// <summary>
    /// Functionality to provide information on program functionalities
    /// </summary>
    public static class Debug {

        /// <summary>
        /// Return YAML dump of object representation
        /// </summary>
        public static string Dump(object _o) {
            var _s = new Serializer();
            string _dump = _s.Serialize(_o);

            return _dump;
        }

        /// <summary>
        /// Print dumped object
        /// </summary>
        public static void WriteDump(object _o) {
            Console.WriteLine(Dump(_o));
        }
    }
}
