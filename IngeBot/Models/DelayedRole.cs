using DSharpPlus;
using DSharpPlus.Entities;
using IngeBot.Models.System;
using IngeBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace IngeBot.Models
{
    public class DelayedRole : Model<DelayedRole>
    {

        [ColumnAttribut("guild_id")]
        public long guildId = 0;

        [ColumnAttribut("owner_id")]
        public long ownerId = 0;

        [ColumnAttribut("target_id")]
        public long targetId = 0;

        [ColumnAttribut("role_id")]
        public long roleId = 0;

        [ColumnAttribut("name")]
        public string name = "";

        [ColumnAttribut("date_start")]
        public DateTime date_start;

        [ColumnAttribut("date_end")]
        public DateTime date_end;


        public override (string column, object value)[] GetColumn()
        {
            return new (string, object)[]
            {
                ("guild_id", guildId),
                ("owner_id", ownerId),
                ("target_id", targetId),
                ("role_id",  roleId),
                ("name", name),
                ("date_start", date_start),
                ("date_end", date_end),
            };
        }

        public override string GetTableName() => "delayed_role";


        protected DelayedRole() { }
        public DelayedRole(long guildId, long ownerId, long targetId, long roleId, string name, DateTime start, DateTime end)
        {
            this.id = -1;
            this.guildId = guildId;
            this.ownerId = ownerId;
            this.targetId = targetId;
            this.roleId = roleId;
            this.name = name;
            this.date_start = start;
            this.date_end = end;
        }


        public async Task Execute(DiscordClient client)
        {

            DiscordGuild guild = client.Guilds[(ulong)guildId];
            if (guild == null) return;

            DiscordMember target = await guild.GetMemberAsync((ulong)targetId);
            if (target == null) return;

            DiscordRole role = guild.GetRole((ulong)roleId);
            if (role == null) return;

            if (date_start != DateTime.MinValue)
            {
                await target.GrantRoleAsync(role);
                await MessageHelper.Log(guild, $"Le role {role.Mention} a été donné automatiquement à {target.Mention} !");
                await target.SendMessageAsync($"Le role {role.Mention} t'a été donné automatiquement par IngéBot™ !");

                date_start = DateTime.MinValue;
                Save();

            }
            else if (date_end != DateTime.MinValue)
            {
                await target.RevokeRoleAsync(role);
                await MessageHelper.Log(guild, $"Le role {role.Mention} a été enlevé automatiquement à {target.Mention} !");
                await target.SendMessageAsync($"Le role {role.Mention} t'a été supprimé automatiquement par IngéBot™ !");

                Delete();
            }
            else
                Delete();

        }


        public static DelayedRole[] FindNext()
        {
            return FindWhere("start", "<", $"\'{DateTime.Now.AddMinutes(40)}\'", "OR", "end", "<", $"\'{DateTime.Now.AddMinutes(40)}\'");
        }

        public static DelayedRole[] FindByGuild(long guildId)
        {
            return FindWhere("guild_id", "=", guildId);
        }

        public static DelayedRole? FindByName(string name)
        {
            return FindOneWhere("name", "=", $"\'{name}\'");
        }

        public static DelayedRole? FindByNameAndGuild(string name, long guildId)
        {
            return FindOneWhere("name", "=", $"\'{name}\'", "AND", "guild_id", "=", $"{guildId}");
        }

    }
}
