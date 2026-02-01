using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot.DelayerEngine
{
    public abstract class ChronoInstructionBase
    {

        public ChronoSystem.InstructionType InstructionType;
        public string Name;
        public string Date;
        public string GuildId;
        public string ChannelId;

        public CancellationTokenSource cts;

        public ChronoInstructionBase(string name, string date, string guildId, string channelId)
        {
            Name = "";
            Date = date;
            GuildId = guildId;
            ChannelId = channelId;
        }


        public abstract Task Register(DiscordInteraction ctx);
        public abstract Task Execute(DiscordChannel channel);
        public abstract Task Unregister(DiscordInteraction ctx);


        public static ChronoInstructionBase ReadFromString(string[] textFile, string name, string guildId, ChronoSystem.InstructionType type)
        {

            switch (type)
            {
                case ChronoSystem.InstructionType.MessageBirthday: return MessageBirthday.ReadFromStringSystem(textFile, name, guildId);
                case ChronoSystem.InstructionType.MessageTime: return MessageTime.ReadFromStringSystem(textFile, name, guildId);
                case ChronoSystem.InstructionType.RoleTime: return RoleTime.ReadFromStringSystem(textFile, name, guildId);
                default : return null;
            }

        }

        public string GetFolderPath()
        {
            return "Data/" + GuildId + "/chrono_instruction/";
        }

    }
}
