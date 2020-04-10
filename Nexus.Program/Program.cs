using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Figgle;
using Nexus.SDK;
using Nexus.SDK.Modules;
using Sentry;

namespace Nexus
{
    internal static class Program
    {
        public static bool Debug;
        
        public static readonly NexusConfiguration NexusConfiguration =
            new NexusModuleConfiguration<NexusConfiguration>().GetConfiguration();

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            
            Startup();

            Console.Title = "Nexus";
            Console.WriteLine(FiggleFonts.Doom.Render("NEXUS"));
            Console.WriteLine(
                "|---------------------------------------------------------------------------------------------------------------------------|");

            if (NexusConfiguration.Diagnostics.EnableSendingDiagnostics)
            {
                using (SentrySdk.Init("https://e61f34cc7659497d91e322c2068a9659@sentry.openshift.alexr03.dev/2"))
                {
                    MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            else
            {
                MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var logger = new Logger("Dependency Loader");
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            // check for assemblies already loaded
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
            {
                logger.LogDebugMessage("Loading " + assembly.FullName + " from: " + assembly.Location);
                return assembly;
            }

            // Try to load by filename - split out the filename of the full assembly name
            // and append the base path of the original assembly (ie. look in the same dir)
            string filename = args.Name.Split(',')[0] + ".dll".ToLower();

            var files = Directory.GetFiles(@".\ModuleDependencies", "*.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Contains(filename))
                {
                    try
                    {
                        logger.LogDebugMessage("Loading " + args.Name + " from: " + file);
                        return Assembly.LoadFrom(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return null;
                    }
                }
            }

            return null;
        }

        private static async Task MainAsync()
        {
            DiscordMain bot = new DiscordMain();
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
            var requiredDirectories = new[] {"./Modules", "./ModuleDependencies", "./Config", "./Shared"};
            foreach (var requiredDirectory in requiredDirectories)
            {
                if (!Directory.Exists(requiredDirectory))
                {
                    Directory.CreateDirectory(requiredDirectory);
                }
            }

            if (string.IsNullOrEmpty(NexusConfiguration.DiscordToken) || !NexusConfiguration.Prefixes.Any())
            {
                Console.WriteLine("Please fill out the NexusConfiguration file.");
                Environment.Exit(0);
            }
        }
    }
}