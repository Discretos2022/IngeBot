using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot.Modules
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlashRequireSuperAdminAttribute : SlashCheckBaseAttribute
    {

        public SlashRequireSuperAdminAttribute() { }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            if (ctx.Guild != null)
                return Task.FromResult(ctx.User.Username == "discretos" || ctx.Guild.OwnerId == ctx.User.Id);
            else
                return Task.FromResult(ctx.User.Username == "discretos");

        }
    }
}
