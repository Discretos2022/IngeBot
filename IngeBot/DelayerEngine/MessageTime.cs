using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IngeBot.DelayerEngine
{
    internal class MessageTime : ChronoInstructionBase
    {

        public string Text;
        public string IsLoop;

        public MessageTime(string name, string text, string isLoop, string date, string guildId, string channelId) : base(name, date, guildId, channelId)
        {

            InstructionType = ChronoSystem.InstructionType.MessageTime;
            Name = (int)InstructionType + "_" + name;
            Text = text;
            IsLoop = isLoop;

        }

        public static ChronoInstructionBase ReadFromStringSystem(string[] lines, string name, string guildId)
        {
            string channelId = lines[0];
            string date = lines[1];
            string isLoop = lines[2];

            string text = "";
            for (int l = 3; l < lines.Length; l++)
            {
                text += lines[l] + "\n";
            }

            return new MessageTime(name, text, isLoop, date, guildId, channelId);
        }

        public override async Task Register(DiscordInteraction ctx)
        {

            try
            {

                var message = new DiscordEmbedBuilder
                {
                    Title = "Résumé du nouveau message programmé :",
                    Color = DiscordColor.Gray,
                    Description = Text + "\n\n Le message sera envoyé le : \n `" + Date + "`",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        // IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                        Text = "Message Time System 1.0",
                    },
                };

                if (File.Exists(GetFolderPath() + Name + ".txt"))
                {

                    message.Title = $":warning: Un message avec le nom {Name.Split("_")[1]} existe déjà !";
                    message.Color = DiscordColor.Red;
                    message.Description = Text + "\n\n Le message NE sera PAS envoyé le : \n `" + Date + "`";

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AsEphemeral(true));
                }

                else
                {

                    StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                    string t = ChannelId + "\n" + Date + "\n" + IsLoop + "\n" + Text;

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

            }
            catch (Exception e)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !"));
            }


        }

        public override async Task Execute(DiscordChannel channel)
        {

            await channel.SendMessageAsync(Text);

            try
            {
                if (bool.Parse(IsLoop))
                {

                    Date = DateTime.Now.Year + 1 + Date.Substring(4);

                    StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                    string t = ChannelId + "\n" + Date + "\n" + IsLoop + "\n" + Text;

                    stream.WriteLine(t);
                    stream.Flush();
                    stream.Close();

                    ChronoSystem.RelaunchMessageLoop(this);

                }
                else
                    File.Delete(GetFolderPath() + Name + ".txt");

            }
            catch (Exception e)
            {

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

                var message = new DiscordEmbedBuilder
                {
                    Title = "Message supprimé :",
                    Color = DiscordColor.Gray,
                    Description = Text + "\n Date d'envoi : \n `" + Date + "`",
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
