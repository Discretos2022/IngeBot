using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IngeBot.DelayerEngine
{
    internal class RoleTime : ChronoInstructionBase
    {

        public string StartDate, EndDate;
        public string UserId;
        public string RoleId;

        public RoleTime(string name, string startDate, string endDate, string userId, string roleId, string date, string guildId, string channelId) : base(name, date, guildId, channelId)
        {

            InstructionType = ChronoSystem.InstructionType.RoleTime;
            Name = (int)InstructionType + "_" + name;
            StartDate = startDate;
            EndDate = endDate;
            UserId = userId;
            RoleId = roleId;

        }

        public static ChronoInstructionBase ReadFromStringSystem(string[] lines, string name, string guildId)
        {
            string channelId = lines[0];
            string date = lines[1];
            string start = lines[2];
            string end = lines[3];
            string userId = lines[4];
            string roleId = lines[5];

            return new RoleTime(name, start, end, userId, roleId, date, guildId, channelId);
        }

        public override async Task Register(DiscordInteraction ctx)
        {

            try
            {

                DiscordMember user = await ctx.Guild.GetMemberAsync(ulong.Parse(UserId));
                DiscordRole role = ctx.Guild.GetRole(ulong.Parse(RoleId));

                var message = new DiscordEmbedBuilder
                {
                    Title = "Nouveau role temporaire programmé : ",
                    Color = DiscordColor.Gray,
                    Description = "Nom : " + Name.Substring(2) + "\n" +
                                  "Utilisateur : " + user.DisplayName + "\n" +
                                  "Role : " + role.Mention + "\n" +
                                  "Début : " + StartDate + "\n" +
                                  "Fin : " + EndDate,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        // IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                        Text = "Message Time System 1.0",
                    },
                };

                if (File.Exists(GetFolderPath() + Name + ".txt"))
                {

                    message.Title = $":warning: Un rôle temporaire avec le nom {Name.Split("_")[1]} existe déjà !";
                    message.Color = DiscordColor.Red;

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AsEphemeral(false));
                    return;
                }
                

                StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                string t = ChannelId + "\n" + Date + "\n" + StartDate + "\n" + EndDate + "\n" + UserId + "\n" + RoleId;

                stream.WriteLine(t);
                stream.Flush();
                stream.Close();

                ChronoSystem.StartLoop(this);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AsEphemeral(true));

                DiscordChannel channel = ctx.Guild.GetDefaultChannel();
                if (Stats.logChannels.ContainsKey(ctx.Guild.Id))
                    channel = ctx.Guild.GetChannel(Stats.logChannels[ctx.Guild.Id]);
                await channel.SendMessageAsync(message);


            }
            catch (Exception e)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !" + e));
            }


        }

        public override async Task Execute(DiscordChannel channel)
        {

            Console.WriteLine(Date + " / " + StartDate);

            if (Date == StartDate)
            {

                DiscordMember m = await channel.Guild.GetMemberAsync(ulong.Parse(UserId));
                DiscordRole r = channel.Guild.GetRole(ulong.Parse(RoleId));

                await m.GrantRoleAsync(r);

                var c = await m.CreateDmChannelAsync();
                /// await c.SendMessageAsync($"Tu as optenu le rôle {r.Name} sur le serveur {channel.Guild.Name} au moyen d'une commande automatique !");


                DiscordChannel log = channel.Guild.GetDefaultChannel();
                if (Stats.logChannels.ContainsKey(channel.Guild.Id))
                    log = channel.Guild.GetChannel(Stats.logChannels[channel.Guild.Id]);
                await log.SendMessageAsync($"Le rôle {r.Mention} a été donné à {m.Mention} automatiquement !");

                if (EndDate != "-")
                {
                    Date = EndDate;

                    StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                    string t = ChannelId + "\n" + Date + "\n" + StartDate + "\n" + EndDate + "\n" + UserId + "\n" + RoleId;

                    stream.WriteLine(t);
                    stream.Flush();
                    stream.Close();

                    ChronoSystem.KillInstruction(Name);
                    ChronoSystem.StartLoop(this);
                }
                else
                {
                    File.Delete(GetFolderPath() + Name + ".txt");
                }

            }

            else
            {

                DiscordMember m = await channel.Guild.GetMemberAsync(ulong.Parse(UserId));
                DiscordRole r = channel.Guild.GetRole(ulong.Parse(RoleId));

                await m.RevokeRoleAsync(r);

                var c = await m.CreateDmChannelAsync();
                /// await c.SendMessageAsync($"Tu n'as plus le rôle {r.Name} sur le serveur {channel.Guild.Name} au moyen d'une commande automatique !");


                DiscordChannel log = channel.Guild.GetDefaultChannel();
                if (Stats.logChannels.ContainsKey(channel.Guild.Id))
                    log = channel.Guild.GetChannel(Stats.logChannels[channel.Guild.Id]);
                await log.SendMessageAsync($"Le rôle {r.Mention} de {m.Mention} a été enlevé automatiquement !");

                ChronoSystem.KillInstruction(Name);
                File.Delete(GetFolderPath() + Name + ".txt");

            }

        }


        /// <summary>
        ///  Revoir
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public override async Task Unregister(DiscordInteraction ctx)
        {

            try
            {

                DiscordMember user = await ctx.Guild.GetMemberAsync(ulong.Parse(UserId));
                DiscordRole role = ctx.Guild.GetRole(ulong.Parse(RoleId));

                var message = new DiscordEmbedBuilder
                {
                    Title = "Rôle temporaire supprimé :",
                    Color = DiscordColor.Gray,
                    Description = "Nom : " + Name.Substring(2) + "\n" +
                                  "Utilisateur : " + user.DisplayName + "\n" +
                                  "Role : " + role.Mention + "\n" +
                                  "Début : " + StartDate + "\n" +
                                  "Fin : " + EndDate,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        // IconUrl = client.CurrentUser.AvatarUrl,
                        Text = "Message Time System 1.0",
                    },
                };


                if (File.Exists(GetFolderPath() + Name + ".txt"))
                {

                    ChronoSystem.KillMessageLoop(Name);
                    File.Delete(GetFolderPath() + Name + ".txt");
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

                }

                else
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !"));
                }

            }
            catch (Exception e)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !"));
            }

        }

    }
}
