using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using Nexus.SDK.Modules;

namespace Nexus.SDK.Plugins
{
    public class PluginRepository
    {
        private const string ModulesLocation = "./Modules";

        public static PluginRepository Instance;

        private readonly Logger _logger = new Logger("Plugin Repository");

        public PluginManager<NexusCommandModule> CustomCommandsModules;

        public PluginManager<NexusScheduledTaskModule> ScheduledTasksModules;

        public PluginManager<NexusAssemblyModule> AssemblyModules;

        public PluginRepository()
        {
            Instance = this;
            RefreshRepository();
        }

        private void RefreshRepository()
        {
            _logger.LogMessage("Module change detected");
            _logger.LogMessage("Refreshing plugin repository!");

            var tempModules = GetModulesRef();

            CustomCommandsModules = new PluginManager<NexusCommandModule>(tempModules);
            ScheduledTasksModules = new PluginManager<NexusScheduledTaskModule>(tempModules);
            AssemblyModules = new PluginManager<NexusAssemblyModule>(tempModules);

            Task.Run(async () => await ModulesManager.ReloadModules());
            _logger.LogMessage("Refreshed!");
        }

        private List<object> GetModulesRef()
        {
            var modules = new List<object>();

            var assemblyFiles =
                Directory.GetFiles(ModulesLocation, "*.dll", SearchOption.AllDirectories).Select(Path.GetFullPath)
                    .ToArray();
            try
            {
                List<Assembly> assemblies = new List<Assembly>();
                foreach (var assemblyFile in assemblyFiles)
                {
                    var assembly = Assembly.LoadFrom(assemblyFile);
                    assembly.GetTypes();
                    assemblies.Add(assembly);
                }

                var nexusModuleType = typeof(NexusModule);
                var commandModuleType = typeof(NexusCommandModule);

                var types = assemblies.SelectMany(s => s.GetTypes())
                    .Where(p => nexusModuleType.IsAssignableFrom(p) ||
                                commandModuleType.IsAssignableFrom(p)).ToList();

                modules.AddRange(types.Select(Activator.CreateInstance));
            }
            catch (BadImageFormatException image)
            {
                _logger.LogMessage(LogLevel.Error, $"{image.FileName} is a bad image. Ensure this file is compiled with .NET 4.6.1 and 64bit compatible.");
            }
            catch (Exception e)
            {
                _logger.LogException(e);

                if (e is ReflectionTypeLoadException typeLoadException)
                {
                    var loaderExceptions = typeLoadException.LoaderExceptions;

                    foreach (var loaderException in loaderExceptions)
                    {
                        _logger.LogException(loaderException);
                    }
                }

                if (e.InnerException != null)
                {
                    _logger.LogException(e.InnerException);
                }
            }

            return modules;
        }
    }
}