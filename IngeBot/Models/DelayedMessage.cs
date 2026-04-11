using DSharpPlus;
using DSharpPlus.Entities;
using IngeBot.Models.System;
using System;

namespace IngeBot.Models
{
    public class DelayedMessage : Model<DelayedMessage>
    {

        [ColumnAttribut("guild_id")]
        public long guildId = 0;

        [ColumnAttribut("channel_id")]
        public long channelId = 0;

        [ColumnAttribut("owner_id")]
        public long ownerId = 0;

        [ColumnAttribut("name")]
        public string name = "";

        [ColumnAttribut("text")]
        public string text = "";

        [ColumnAttribut("date")]
        public DateTime date;

        [ColumnAttribut("repeat")]
        public bool repeat = false;


        public override (string column, object value)[] GetColumn()
        {
            return new (string, object)[]
            {
                ("guild_id", guildId),
                ("channel_id", channelId),
                ("owner_id", ownerId),
                ("name", name),
                ("text", text),
                ("date", date),
                ("repeat", repeat),
            };
        }

        public override string GetTableName() => "delayed_message";


        protected DelayedMessage() { }
        public DelayedMessage(long guildId, long channelId, long ownerId, string name, string text, DateTime date, bool repeat)
        {
            this.id = -1;
            this.guildId = guildId;
            this.channelId = channelId;
            this.ownerId = ownerId;
            this.name = name;
            this.text = text;
            this.date = date;
            this.repeat = repeat;
        }

        public static DelayedMessage[] FindNext()
        {
            return FindWhere("date", "<", $"\'{DateTime.Now.AddMinutes(40)}\'");
        }

        public static DelayedMessage? FindByName(string name)
        {
            return FindOneWhere("name", "=", $"\'{name}\'");
        }

        public static DelayedMessage[] FindByGuild(long guildId)
        {
            return FindWhere("guild_id", "=", guildId);
        }


        public async Task Execute(DiscordClient client)
        {

            DiscordGuild guild = client.Guilds[(ulong)guildId];
            if (guild == null) return;

            DiscordChannel channel = guild.GetChannel((ulong)channelId);
            if (channel == null) return;

            await channel.SendMessageAsync(text);

            if (!repeat)
                Delete();
            else
            {
                date = date.AddYears(1);
                Save();
            }

        }

    }
}
