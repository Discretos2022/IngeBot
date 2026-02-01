using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IngeBot.DelayerEngine
{
    internal class MessageBirthday : ChronoInstructionBase
    {

        public string Text;
        public string BaseDate;

        public MessageBirthday(string name, string text, string date, string baseDate, string guildId, string channelId) : base(name, date, guildId, channelId)
        {
            InstructionType = ChronoSystem.InstructionType.MessageBirthday;
            Name = (int)InstructionType + "_" + name;
            Text = text;
            BaseDate = baseDate;

        }

        public static ChronoInstructionBase ReadFromStringSystem(string[] lines, string name, string guildId)
        {

            string channelId = lines[0];
            string date = lines[1];
            string baseDate = lines[2];

            string text = "";
            for (int l = 3; l < lines.Length; l++)
            {
                text += lines[l] + "\n";
            }

            return new MessageBirthday(name, text, date, baseDate, guildId, channelId);

        }

        public override async Task Register(DiscordInteraction ctx)
        {

            try
            {

                if (File.Exists(GetFolderPath() + Name + ".txt"))
                {

                    ChronoSystem.KillMessageLoop(Name);

                    File.Delete(GetFolderPath() + Name + ".txt");

                    StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                    string t = ChannelId + "\n" + Date + "\n" + BaseDate + "\n" + Text;

                    stream.WriteLine(t);
                    stream.Flush();
                    stream.Close();

                    ChronoSystem.RelaunchMessageLoop(this);

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ton anniversaire a été mis à jour : `{BaseDate}`"));

                }

                else
                {

                    StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                    string t = ChannelId + "\n" + Date + "\n" + BaseDate + "\n" + Text;

                    stream.WriteLine(t);
                    stream.Flush();
                    stream.Close();

                    ChronoSystem.StartLoop(this);

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ton anniversaire a été enregistré : `{BaseDate}`"));

                }

            }
            catch(Exception e)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !"));
                // await Debug.LogError(ctx, e);
            }



        }

        public override async Task Execute(DiscordChannel channel)
        {

            var message = new DiscordEmbedBuilder
            {
                Description = Text,
                Color = DiscordColor.Yellow,
            };

            await channel.SendMessageAsync(message);

            try
            {

                Date = DateTime.Now.Year + 1 +Date.Substring(4);

                StreamWriter stream = File.CreateText(GetFolderPath() + Name + ".txt");

                string t = ChannelId + "\n" + Date + "\n" + BaseDate + "\n" + Text;

                stream.WriteLine(t);
                stream.Flush();
                stream.Close();

                ChronoSystem.RelaunchMessageLoop(this);

            }
            catch (Exception e)
            {

            }

        }

        public override async Task Unregister(DiscordInteraction ctx)
        {

            try
            {

                if (File.Exists(GetFolderPath() + Name + ".txt"))
                {

                    ChronoSystem.KillMessageLoop(Name);

                    File.Delete(GetFolderPath() + Name + ".txt");

                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ton anniversaire a été supprimé"));

                }

                else
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ton anniversaire n'est pas défini"));
                }

            }
            catch (Exception e)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !"));
            }

        }

    }
}
