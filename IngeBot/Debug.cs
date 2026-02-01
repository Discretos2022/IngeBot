using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot
{
    public class Debug
    {

        public static async Task LogError(InteractionContext ctx, Exception e)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu :\n `{e}`"));
        }

    }
}
