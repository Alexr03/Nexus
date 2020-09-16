using DSharpPlus;

namespace Nexus.SDK.Modules
{
    public class NexusAssemblyModule : NexusModule
    {
        public DiscordClient DiscordClient { get; internal set; }

        public Logger Logger { get; set; }

        public NexusAssemblyModule()
        {
            Logger = new Logger(Name);
        }

        public virtual void Main()
        {
        }
    }
}