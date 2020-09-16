using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using FluentScheduler;
using Nexus.SDK.Plugins;

namespace Nexus
{
    public static class ModulesManager
    {
        public static readonly Logger Logger = new Logger("Modules Manager");
        public static readonly Registry Registry = new Registry();

        public static async Task ReloadModules()
        {
            try
            {
                RefreshAssemblyModules();
                RefreshCustomCommandModules();
                await RefreshCronScheduler();
            }
            catch (Exception e)
            {
                PrintModuleMessage("GLOBAL", "Error: " + e.Message);
                PrintModuleMessage("GLOBAL", e.StackTrace);
            }
        }

        private static void RefreshAssemblyModules()
        {
            foreach (var module in PluginRepository.Instance.AssemblyModules.Plugins)
            {
                module.DiscordClient = DiscordMain.Client;
                module.Logger = new Logger(module.Name);
                Task.Run(() => module.Main());
                PrintModuleMessage("AssemblyModules", "Loaded: " + module.GetType().Name);
            }
        }

        private static async Task RefreshCronScheduler()
        {
            foreach (var scheduledTask in PluginRepository.Instance.ScheduledTasksModules.Plugins)
            {
                Registry.Schedule(scheduledTask).ToRunNow().AndEvery(scheduledTask.RepeatEveryMilliseconds)
                    .Milliseconds();
                Logger.LogMessage("[Cron Scheduler] Scheduled " + scheduledTask.GetType().Name + " to fire every " +
                                  scheduledTask.RepeatEveryMilliseconds + "ms");
            }
        }

        private static void RefreshCustomCommandModules()
        {
            foreach (var commandModule in PluginRepository.Instance.CustomCommandsModules.Plugins)
            {
                Logger.LogMessage("Registered: " + commandModule.GetType().Name + " class for Custom Commands.");
                DiscordMain.Client.GetCommandsNext().RegisterCommands(commandModule.GetType());
            }
        }

        private static void PrintModuleMessage(string module, string message)
        {
            Logger.LogMessage($"[{module}] {message}");
        }
    }
}