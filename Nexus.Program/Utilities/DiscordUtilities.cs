using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Nexus.Utilities
{
    public static class DiscordUtilities
    {
        public static async Task<bool> SendMessageToDm(CommandContext ctx, DiscordChannel dmChannel, string message,
            DiscordEmbed embed = null)
        {
            try
            {
                await dmChannel.SendMessageAsync(message, embed: embed);
                return true;
            }
            catch
            {
                await ctx.RespondAsync(
                    $"**I can't seem to DM you {ctx.User.Mention}. Please make sure your DMs are open.**");
                return false;
            }
        }
    }
}