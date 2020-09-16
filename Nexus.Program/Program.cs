using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alexr03.Common.Configuration;
using Nexus.SDK;
using Serilog;

namespace Nexus
{
    internal static class Program
    {
        public static bool Debug;

        public static readonly NexusConfiguration NexusConfiguration =
            new LocalConfiguration<NexusConfiguration>().GetConfiguration();

        private static void Main(string[] args)
        {
            Console.Title = "Nexus";
            Startup();

            Console.WriteLine(
                "|---------------------------------------------------------------------------------------------------------------------------|");

            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var bot = new DiscordMain();
            await bot.RunAsync();

            while (true)
            {
                try
                {
                    CommandManager.WritePrompt();
                    var input = Console.ReadLine();

                    ProcessCommand(input);
                }
                catch
                {
                    Console.ReadLine();
                }

                await Task.Delay(100);
            }
        }

        private static void ProcessCommand(string command)
        {
            var cmdParent = command.ToLower().Split(' ')[0];
            var arguments = cmdParent.Split(' ');

            switch (cmdParent)
            {
                case "debug":
                    Debug = !Debug;
                    Console.WriteLine("Debugging: " + Debug);
                    break;
                case "repeat":
                    Console.WriteLine(command.Substring(cmdParent.Length + 1));
                    break;
                case "ping":
                    Console.Write("Ping: " + DiscordMain.Client.Ping + "ms\n");
                    break;
                case "arg":
                    var argId = 1;
                    Console.Write("Arguments: " + string.Join(argId++ + ") ", arguments));
                    break;
                case "clr":
                case "clear":
                    Console.Clear();
                    break;
                case "exit":
                case "quit":
                    Environment.Exit(0);
                    break;
            }
        }

        private static void Startup()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console()
                .WriteTo.File("./Logs/Nexus/Nexus.log").CreateLogger();

            var requiredDirectories = new[] {"./Modules", "./Shared"};
            foreach (var requiredDirectory in requiredDirectories)
                if (!Directory.Exists(requiredDirectory))
                    Directory.CreateDirectory(requiredDirectory);

            if (!string.IsNullOrEmpty(NexusConfiguration.DiscordToken) && NexusConfiguration.Prefixes.Any()) return;
            Console.WriteLine("Please fill out the NexusConfiguration file.");
            Environment.Exit(0);
        }
    }
}