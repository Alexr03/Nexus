using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Nexus.SDK.Modules
{
    public class NexusModuleConfiguration<T>
    {
        private readonly string _configName;

        private readonly string _configLocation;

        public NexusModuleConfiguration()
        {
            var type = typeof(T);
            this._configName = type.Name + ".json";
            this._configLocation = "./Config/";
            Directory.CreateDirectory(_configLocation);
        }

        public NexusModuleConfiguration(string configName, string configLocation = "./Config/")
        {
            Directory.CreateDirectory(configLocation);
            this._configName = configName.EndsWith(".json") ? configName : configName + ".json";
            this._configLocation = configLocation;
        }

        public T GetConfiguration(bool generateIfDontExist = true)
        {
            if (!File.Exists(_configLocation + _configName))
            {
                if (generateIfDontExist)
                {
                    Console.WriteLine("Config does not exist. Auto generating.");
                    var defaultT = GetTObject();
                    SetConfiguration(defaultT);
                }
                else
                {
                    return default(T);
                }
            }

            try
            {
                var fileContents = File.ReadAllText(_configLocation + _configName);
                var deserializeObject = JsonConvert.DeserializeObject<T>(fileContents);
                return deserializeObject;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to parse {_configName} to object {this.GetType().FullName}. Please ensure that the JSON is correct.");
                Console.WriteLine($"Message: {e.Message}");
            }

            return default;
        }

        public bool SetConfiguration(T config)
        {
            try
            {
                var serializedObject = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configLocation + _configName, serializedObject, Encoding.Default);

                Console.WriteLine("Saved new Configuration to " + _configName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save configuration because {e.Message}");
                return false;
            }
        }

        private T GetTObject()
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                return default;
            }
            return (T) Activator.CreateInstance(typeof(T));
        }
    }
}