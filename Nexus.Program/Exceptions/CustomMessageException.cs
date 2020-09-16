using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Nexus.Exceptions
{
    public class CustomMessageException : Exception
    {
        public CustomMessageException()
        {
        }

        public CustomMessageException(DiscordEmbed embed)
        {
            Embed = embed;
        }

        public CustomMessageException(string message)
        {
            Message = message;
        }

        public virtual Task DoAction()
        {
            return Task.Delay(0);
        }

        public override string Message { get; }

        public CommandContext Context;

        public DiscordEmbed Embed { get; set; }

        public bool Handled { get; set; } = true;
    }
}