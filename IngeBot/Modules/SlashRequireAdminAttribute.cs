using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using IngeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot.Modules
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlashRequireAdminAttribute : SlashCheckBaseAttribute
    {

        public SlashRequireAdminAttribute() { }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {

            ulong id = 0;
            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.ADMIN_ROLE);
            if (param != null && param.value != "") id = ulong.Parse(param.value);

            return Task.FromResult(ctx.User.Username == "discretos" || ctx.Guild.OwnerId == ctx.User.Id || ctx.User.Id == id);
        }
    }
}
