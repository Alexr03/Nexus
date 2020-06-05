using DSharpPlus;
using Nexus.SDK.Plugins;

namespace Nexus.Utilities
{
    public static class NexusInformation
    {
        public static DiscordClient DiscordClient => DiscordMain.Client;
        
        public static Logger Logger = new Logger("Nexus Information");
        
        public static PluginRepository PluginRepository;
    }
}