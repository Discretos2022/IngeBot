using IngeBot.Models.System;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot.Models
{
    public class Parameter : Model<Parameter>
    {

        [ColumnAttribut("guild_id")]
        public long guildId = 0;

        [ColumnAttribut("key")]
        public string key = "";

        [ColumnAttribut("value")]
        public string value = "";

        public const string LOG_CHANNEL = "LOG_CHANNEL";
        public const string WELCOME_CHANNEL = "WELCOME_CHANNEL";
        public const string MODERATION = "MODERATION";
        public const string ADMIN_ROLE = "ADMIN_ROLE";


        public override string GetTableName() => "parameters";

        public override (string column, object value)[] GetColumn()
        {
            return new (string, object)[]
            {
                ("guild_id", guildId),
                ("key", key),
                ("value", value)
            };
        }


        protected Parameter() { }
        public Parameter(long guildId, string key, string value)
        {
            this.id = -1;
            this.guildId = guildId;
            this.key = key;
            this.value = value;
        }



        public static Parameter? FindByGuildIdAndKey(ulong guildId, string key)
        {
            return FindOneWhere("guild_id", "=", guildId, "AND", "key", "=", $"\'{key}\'");
        }

    }
}
