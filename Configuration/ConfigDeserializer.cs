using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;

namespace ScriptExNeo.Configuration {

    /// <summary>
    /// Convert yaml file into configuration object
    /// </summary>
    static class ConfigDeserializer {
        public static Config Deserialize(string path) {
            try {
                var _input = new StreamReader(path);

                var _deserializer = new DeserializerBuilder().Build();

                var _config = _deserializer.Deserialize<Config>(_input);

                return _config;
            }
            catch (Exception e) {
                throw new IOException($"Unable to deserialize '{path}'", e);
            }
        }
    }
}
