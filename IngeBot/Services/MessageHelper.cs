using DSharpPlus.Entities;
using IngeBot.Models;
using System;

namespace IngeBot.Services
{
    public class MessageHelper
    {

        public static async Task Log(DiscordGuild guild, string message)
        {

            Parameter? param = null;
            param = Parameter.FindByGuildIdAndKey(guild.Id, Parameter.LOG_CHANNEL);

            if (param == null || param.value == "") return;

            DiscordChannel channel = guild.GetChannel(ulong.Parse(param.value));
            await channel.SendMessageAsync(message);

        }

        public static async Task Log(DiscordGuild guild, DiscordMessageBuilder message)
        {

            Parameter? param = null;
            param = Parameter.FindByGuildIdAndKey(guild.Id, Parameter.LOG_CHANNEL);

            if (param == null || param.value == "") return;

            DiscordChannel channel = guild.GetChannel(ulong.Parse(param.value));
            await channel.SendMessageAsync(message);

        }

        public static async Task Welcome(DiscordGuild guild, string message)
        {

            Parameter? param = null;
            param = Parameter.FindByGuildIdAndKey(guild.Id, Parameter.WELCOME_CHANNEL);

            if (param == null || param.value == "") return;

            DiscordChannel channel = guild.GetChannel(ulong.Parse(param.value));
            await channel.SendMessageAsync(message);

        }

    }
}
