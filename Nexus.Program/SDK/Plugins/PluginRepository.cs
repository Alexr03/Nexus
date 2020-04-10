using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using DSharpPlus;
using Nexus.SDK.Modules;

namespace Nexus.SDK.Plugins
{
    public class PluginRepository
    {
        public string ModulesLocation = "./Modules";

        public static PluginRepository Instance;
        
        public Logger Logger = new Logger("Plugin Repository");

        public PluginManager<NexusCommandModule> CustomCommandsModules;

        public PluginManager<NexusScheduledTaskModule> ScheduledTasksModules;

        public PluginManager<NexusAssemblyModule> AssemblyModules;

        public PluginRepository()
        {
            Instance = this;
            RefreshRepository();
            SubscribeToFileWatcher();
        }

        public void RefreshRepository()
        {
            Logger.LogMessage("Module change detected");
            Logger.LogMessage("Refreshing plugin repository!");

            List<object> tempModules = GetModulesRef();

            CustomCommandsModules = new PluginManager<NexusCommandModule>(tempModules);
            ScheduledTasksModules = new PluginManager<NexusScheduledTaskModule>(tempModules);
            AssemblyModules = new PluginManager<NexusAssemblyModule>(tempModules);

            Task.Run(async () => await ModulesManager.ReloadModules());
            Logger.LogMessage("Refreshed!");
        }

        private List<object> GetModulesRef()
        {
            List<object> modules = new List<object>();

            string[] assemblyFiles =
                Directory.GetFiles(this.ModulesLocation, "*.dll", SearchOption.AllDirectories).Select(Path.GetFullPath)
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

                foreach (Type moduleType in types)
                {
                    modules.Add(Activator.CreateInstance(moduleType));
                }
            }
            catch (BadImageFormatException image)
            {
                Logger.LogMessage(LogLevel.Error, $"{image.FileName} is a bad image. Ensure this file is compiled with .NET 4.6.1 and 64bit compatible.");
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                if (e is ReflectionTypeLoadException typeLoadException)
                {
                    var loaderExceptions = typeLoadException.LoaderExceptions;

                    foreach (var loaderException in loaderExceptions)
                    {
                        Logger.LogException(loaderException);
                    }
                }

                if (e.InnerException != null)
                {
                    var inner = e.InnerException;
                    Logger.LogException(e.InnerException);
                }
            }

            return modules;
        }

        private void SubscribeToFileWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetFullPath("./Modules"),
                NotifyFilter = NotifyFilters.Attributes |
                               NotifyFilters.CreationTime |
                               NotifyFilters.FileName |
                               NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.Size |
                               NotifyFilters.Security,
                Filter = "*.dll"
            };


            watcher.Created += OnChanged;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            RefreshRepository();
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            RefreshRepository();
        }
    }
}