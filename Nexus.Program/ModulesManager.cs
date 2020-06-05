using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Nexus.SDK.Modules;
using Nexus.SDK.Plugins;
using Quartz;
using Quartz.Impl;

namespace Nexus
{
    public static class ModulesManager
    {
        public static readonly Logger Logger = new Logger("Modules Manager");
        
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
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Clear();
            await scheduler.Start();

            scheduler.Context.Put("Client", DiscordMain.Client);

            foreach (var scheduledTask in PluginRepository.Instance.ScheduledTasksModules.Plugins)
            {
                var type = scheduledTask.GetType();
                var build = JobBuilder.Create(type).WithIdentity(type.Name + "_" + new Random().Next(1000)).Build();
                var schedule = TriggerBuilder.Create().WithIdentity(type.Name + "_" + new Random().Next(1000))
                    .StartNow().WithSimpleSchedule(x =>
                        x.WithInterval(TimeSpan.FromMilliseconds(scheduledTask.RepeatEveryMilliseconds))
                            .RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(build, schedule);
                Logger.LogMessage("[Cron Scheduler] Scheduled " + type.Name + " to fire every " +
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