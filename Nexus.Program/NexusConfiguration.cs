using DSharpPlus.Entities;
using Nexus.SDK.Modules;

namespace Nexus
{
    public class NexusConfiguration : NexusModuleConfiguration<NexusConfiguration>
    {
        public string DiscordToken { get; set; } = "";

        public string[] Prefixes { get; set; } = {";"};

        public string PlayingStatus { get; set; } = "Nexus Bot";

        public ActivityType PlayingType { get; set; } = ActivityType.Playing;
        
        public Diagnostics Diagnostics { get; set; } = new Diagnostics();
    }

    public class Diagnostics
    {
        public bool EnableSendingDiagnostics { get; set; } = true;
        
        public bool DebugMode { get; set; } = false;
    }
}