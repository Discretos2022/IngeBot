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
    public class SlashRequireAdminAttribute : SlashCheckBaseAttribute
    {

        public SlashRequireAdminAttribute() { }

        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            return Task.FromResult(ctx.User.Username == "discretos" || Stats.ContainsRole(ctx.Member, Stats.adminRole));
        }
    }
}
