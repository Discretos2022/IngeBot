using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.VoiceNext;
using IngeBot;
using IngeBot.DelayerEngine;
using IngeBot.Models;
using IngeBot.Models.System;
using IngeBot.Services;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace IngeBot.Modules
{
    public class Commands : BaseCommandModule
    {

        [Command("helloTest")]
        public async Task PingAsync(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Salut ! Je suis le bot de Discretos !  (Pour plus d'informations, voir avec le développeur)");
        }

    }

    public class SlashCommands : ApplicationCommandModule
    {

        public string ArrayReverseString(string stringToReverse)
        {
            var charArray = stringToReverse.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        [SlashCommand("hello", "Le test...")]
        public async Task Ping2Async(InteractionContext ctx)
        {

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Yellow,
                Title = "IngéBot est opérationnel !",
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Salut ! Je suis IngéBot !  (Pour plus d'informations, voir avec Joshua)").AddEmbed(message));
        }

        [SlashCommand("info", "Information concernant le Bot !")]
        public async Task InfoAsync(InteractionContext ctx)
        {

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Yellow,
                Title = "Information : ",
                Description = "`IngéBot ver " + Stats.version +
                "\nCopyright (c) 2024-2025 SIEDEL Joshua" +
                "\nIP : Tu croyais que j'allais vraiment mettre l'adresse ip ! X)`",

            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

        }

        [SlashCommand("latency", "Retourne la latence de réponse du bot")]
        public async Task GetLatency(InteractionContext ctx)
        {

            int latency = ctx.Client.Ping;

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Yellow,
                Description = "La latence est de `" + latency + " ms`",

            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

        }

        [SlashRequireDisc]
        [SlashCommand("ip", "L'adresse IP du server qui héberge le bot. (𝕯𝖎𝖘𝖈𝖗𝖊𝖙𝖔𝖘)")]
        public async Task GetIPAsync(InteractionContext ctx)
        {

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Recherche en cours..."));

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Aquamarine,
                Description = "Public IP : " + SearchPublicIP() + "",
            };

            await ctx.Interaction.DeleteOriginalResponseAsync();
            await ctx.Channel.SendMessageAsync(embed: message);

        }

        public static string SearchPublicIP()
        {
            try
            {
                string direction = "";
                HttpWebRequest request = WebRequest.CreateHttp("http://checkip.dyndns.org/");
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        direction = stream.ReadToEnd();
                    }
                }
                //Search for the ip in the html
                int first = direction.IndexOf("Address: ") + 9;
                int last = direction.LastIndexOf("");
                direction = direction.Substring(first, last - first - 16);
                return direction;
            }
            catch (Exception ex)
            {
                return "127.0.0.1";
            }
        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("moderation", "Fonction de modération (true/false)")]
        public async Task EnableModerationAsync(InteractionContext ctx, [Option("activer", "true or false")] string response)
        {

            if (response == "true" || response == "false")
            {

                Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.MODERATION);
                if (param == null) param = new Parameter((long)ctx.Guild.Id, Parameter.MODERATION, "");

                param.value = response;
                param.Save();

            }

            if (response == "true")
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le système de modération a été activé !"));
            else if (response == "false")
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le système de modération a été désactivé !"));
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("La commande n'est pas valide !"));

        }


        [SlashRequireSuperAdmin]
        [SlashCommand("setbotgame", "Une commande pour le jeu auquel le bot joue. (admin)")]
        public async Task SetBotGame(InteractionContext ctx)
        {
            var modal = new DiscordInteractionResponseBuilder().WithTitle("Set Bot Game").WithCustomId("modal_bot_game").AddComponents(new TextInputComponent("Nom du jeu : ", "id", "Entre Le nom d'un jeu"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("grant", "Une commande pour ajouter un rôle ! (admin)")]
        public async Task GrantRole(InteractionContext ctx)
        {

            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
            {
                Console.WriteLine(ctx.Guild.Roles.ElementAt(i).Value);
            }

            Stats.role = "";
            Stats.user = "";

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "valid", "Valider", false);
            DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Success, "seldate", "Date", false);

            var message = new DiscordInteractionResponseBuilder().WithTitle("Ajouter un rôle à un utilisateur").AddComponents(new DiscordRoleSelectComponent("roles", "Roles")).AddComponents(new DiscordUserSelectComponent("user", "Utilisateur")).AddComponents(b1, b2);     //  .AddComponents(new DiscordChannelSelectComponent("123", "1234"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

            Stats.mess_role_id = ctx.GetOriginalResponseAsync().Result.Id;

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("revoke", "Une commande pour enlever un rôle ! (admin)")]
        public async Task RevokeRole(InteractionContext ctx)
        {

            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
            {
                Console.WriteLine(ctx.Guild.Roles.ElementAt(i).Value);
            }

            Stats.role = "";
            Stats.user = "";

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "valid_revoke", "Valider", false);

            var message = new DiscordInteractionResponseBuilder().WithTitle("Supprimer un rôle à un utilisateur").AddComponents(new DiscordRoleSelectComponent("roles", "Roles")).AddComponents(new DiscordUserSelectComponent("user", "Utilisateur")).AddComponents(b1);     //  .AddComponents(new DiscordChannelSelectComponent("123", "1234"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

            Stats.mess_role_id = ctx.GetOriginalResponseAsync().Result.Id;

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("setchannellog", "Une commande pour définir un salon comme log ! (admin)")]
        public async Task SetChannelLog(InteractionContext ctx)
        {

            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.LOG_CHANNEL);
            if (param == null) param = new Parameter((long)ctx.Guild.Id, Parameter.LOG_CHANNEL, "");

            param.value = ctx.Channel.Id.ToString();
            param.Save();

            var message = new DiscordInteractionResponseBuilder().WithContent("Le salon pour les logs viens d'être défini dans : " + ctx.Channel.Mention);
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

            await MessageHelper.Log(ctx.Guild, "Le salon de log a été défini dans " + ctx.Channel.Name + " par " + ctx.User.Username + ".");

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("setwelcomechannel", "Une commande pour définir un salon comme salon de bienvenu ! (admin)")]
        public async Task SetWelcomeChannel(InteractionContext ctx)
        {

            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.WELCOME_CHANNEL);
            if (param == null) param = new Parameter((long)ctx.Guild.Id, Parameter.WELCOME_CHANNEL, "");

            param.value = ctx.Channel.Id.ToString();
            param.Save();

            var message = new DiscordInteractionResponseBuilder().WithContent("Le salon pour les bienvenus viens d'être défini dans : " + ctx.Channel.Mention);
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

            await MessageHelper.Log(ctx.Guild, "Le salon de bienvenu a été défini dans " + ctx.Channel.Name + " par " + ctx.User.Username + ".");
        }


        [SlashCommand("demineur", "Une commande pour créer une partie de démineur !")]
        public async Task CreateDemineur(InteractionContext ctx, [Choice("Facile", "Facile")][Choice("Moyen", "Moyen")][Choice("Difficile", "Difficile")][Option("Difficulté", "Difficulté")] string response)
        {

            int s = 6;
            int mines = 6;

            if (response == "Moyen")
            {
                s = 8;
                mines = 10;
            }
            else if (response == "Difficile")
            {
                s = 10;
                mines = 12;
            }

            int m = mines;

            int[,] grid = new int[s, s];

            while (mines > 0)
            {
                int x = Random.Shared.Next(0, grid.GetLength(0));
                int y = Random.Shared.Next(0, grid.GetLength(1));

                if (grid[x, y] != -100)
                {
                    grid[x, y] = -100;
                    mines -= 1;
                }
            }

            for (int x = 0; x < grid.GetLength(0); x++)
            {

                for (int y = 0; y < grid.GetLength(1); y++)
                {

                    if (grid[x, y] != -100)
                    {

                        int num = 0;

                        if (x > 0)
                            if (grid[x - 1, y] == -100)
                                num += 1;

                        if (x > 0 && y > 0)
                            if (grid[x - 1, y - 1] == -100)
                                num += 1;

                        if (y > 0)
                            if (grid[x, y - 1] == -100)
                                num += 1;

                        if (x < grid.GetLength(0) - 1 && y > 0)
                            if (grid[x + 1, y - 1] == -100)
                                num += 1;

                        if (x < grid.GetLength(0) - 1)
                            if (grid[x + 1, y] == -100)
                                num += 1;

                        if (x < grid.GetLength(0) - 1 && y < grid.GetLength(1) - 1)
                            if (grid[x + 1, y + 1] == -100)
                                num += 1;

                        if (y < grid.GetLength(1) - 1)
                            if (grid[x, y + 1] == -100)
                                num += 1;

                        if (x > 0 && y < grid.GetLength(1) - 1)
                            if (grid[x - 1, y + 1] == -100)
                                num += 1;

                        grid[x, y] = num;

                    }

                }

            }



            bool isSet = false;

            while (!isSet)
            {
                int x = Random.Shared.Next(0, grid.GetLength(0));
                int y = Random.Shared.Next(0, grid.GetLength(1));

                if (grid[x, y] == 0)
                {
                    grid[x, y] = -grid[x, y] - 1;
                    isSet = true;
                }

            }

            string result = "";


            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Console.Write(grid[i, j] + ", ");
                }
                Console.WriteLine();
            }


            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {

                    if (grid[i, j] == 0)
                        result += "||" + ":zero:" + "||";
                    if (grid[i, j] == 1)
                        result += "||" + ":one:" + "||";
                    if (grid[i, j] == 2)
                        result += "||" + ":two:" + "||";
                    if (grid[i, j] == 3)
                        result += "||" + ":three:" + "||";
                    if (grid[i, j] == 4)
                        result += "||" + ":four:" + "||";
                    if (grid[i, j] == 5)
                        result += "||" + ":five:" + "||";
                    if (grid[i, j] == 6)
                        result += "||" + ":six:" + "||";
                    if (grid[i, j] == 7)
                        result += "||" + ":seven:" + "||";
                    if (grid[i, j] == 8)
                        result += "||" + ":eight:" + "||";

                    if (grid[i, j] == -100)
                        result += "||" + ":boom:" + "||";

                    if (grid[i, j] == -1)
                        result += ":zero:";
                    if (grid[i, j] == -2)
                        result += ":one:";
                    if (grid[i, j] == -3)
                        result += ":two:";
                    if (grid[i, j] == -4)
                        result += ":three:";
                    if (grid[i, j] == -5)
                        result += ":four:";
                    if (grid[i, j] == -6)
                        result += ":five:";
                    if (grid[i, j] == -7)
                        result += ":six:";
                    if (grid[i, j] == -8)
                        result += ":seven:";
                    if (grid[i, j] == -9)
                        result += ":eight:";

                }

                result += "\n";
            }


            var f = new DiscordEmbedBuilder.EmbedFooter();
            f.Text = "Grille " + s + "x" + s + " | " + m + " mines";
            f.IconUrl = ctx.User.AvatarUrl;

            var gridMess = new DiscordEmbedBuilder
            {
                Color = DiscordColor.SpringGreen,
                //ImageUrl = 
                Title = "Demineur 3.0",
                Description = result,
                Footer = f,
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(gridMess));

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("config", "Une commande pour afficher les données sauvegardées. (admin)")]
        public async Task GetSaveInfo(InteractionContext ctx)
        {

            Parameter? logChannelParam = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.LOG_CHANNEL);
            string logChannel = logChannelParam?.value ?? "";
            logChannel = ctx.Guild.GetChannel(ulong.Parse(logChannel))?.Mention ?? "-";

            Parameter? welcomeChannelParam = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.WELCOME_CHANNEL);
            string welcomeChannel = welcomeChannelParam?.value ?? "";
            welcomeChannel = ctx.Guild.GetChannel(ulong.Parse(welcomeChannel))?.Mention ?? "-";

            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.MODERATION);
            string moderationParam = param?.value ?? "false";
            bool moderation = bool.Parse(moderationParam == "true" || moderationParam == "false" ? moderationParam : "false");

            var m = new DiscordEmbedBuilder
            {
                Title = "Données sauvegardées par IngéBot",
                Color = DiscordColor.Gray,
                Description = ""
            };

            m.Description += "**Channel : **";
            m.Description += "\n";
            m.Description += "Log Channel : " + logChannel;
            m.Description += "\n";
            m.Description += "Welcome Channel : " + welcomeChannel;
            m.Description += "\n\n";
            m.Description += "**Modération : **";
            m.Description += "\n";
            m.Description += "Modération : " + moderation;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(m));

        }


        [SlashCommand("send", "Une commande pour envoyer un message incognito. héhéhé !")]
        public async Task SendMess(InteractionContext ctx, [Option("Message", "Message à envoyer")] string m)
        {
            await ctx.Interaction.Channel.SendMessageAsync(m);
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le message a fonctionné").AsEphemeral(true));
        }


        [SlashCommand("blague", "Une commande pour une blague.")]
        public async Task SendBlague(InteractionContext ctx)
        {

            string b = Stats.blague[Random.Shared.Next(0, Stats.blague.Count)];


            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Cyan,
                Description = b.Split("#")[0] +
                       "\n" + b.Split("#")[1]
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));
        }


        //[SlashCommand("restart", "Une commande pour restart le bot ! (admin)")]
        //public async Task Restart(InteractionContext ctx)
        //{

        //    if (ctx.Guild == null)
        //    {
        //        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
        //        return;
        //    }

        //    if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
        //    {
        //        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
        //        return;
        //    }

        //    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Restarting...").AsEphemeral(false));
        //}



        [SlashCommand("help", "Une commande pour afficher toutes les commandes ! (admin)")]
        public async Task Help(InteractionContext ctx)
        {

            ulong id = 0;
            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.ADMIN_ROLE);
            if (param != null && param.value != "") id = ulong.Parse(param.value); 

            DiscordEmbedBuilder message = null;

            if (ctx.Guild == null)
            {
                message = new DiscordEmbedBuilder
                {
                    Title = "/help !",
                    Color = DiscordColor.Gray,
                    Description = Stats.SlashCommandBase +
                                " \n " +
                                " \n " + Stats.NativeCommandBasic

                };
            }
            else if (ctx.User.Username == "discretos" || ctx.Guild.OwnerId == ctx.User.Id)
            {
                message = new DiscordEmbedBuilder
                {
                    Title = "/help !",
                    Color = DiscordColor.Gray,
                    Description = Stats.SlashCommandBase +
                                " \n " +
                                " \n " + Stats.SlashCommandAdmin +
                                " \n " +
                                " \n " + Stats.SlashCommandSuperAdmin +
                                " \n " +
                                " \n " + Stats.NativeCommandBasic

                };
            }
            else if (ctx.User.Id == id)
            {
                message = new DiscordEmbedBuilder
                {
                    Title = "/help !",
                    Color = DiscordColor.Gray,
                    Description = Stats.SlashCommandBase +
                                " \n " +
                                " \n " + Stats.SlashCommandAdmin +
                                " \n " +
                                " \n " + Stats.NativeCommandBasic

                };
            }
            else
            {
                message = new DiscordEmbedBuilder
                {
                    Title = "/help !",
                    Color = DiscordColor.Gray,
                    Description = Stats.SlashCommandBase +
                                " \n " +
                                " \n " + Stats.NativeCommandBasic

                };
            }
            


            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));
        }


        [SlashRequireGuild]
        [SlashCommand("ticket", "Une commande pour créer un ticket !")]
        public async Task CreateTicket(InteractionContext ctx, [Option("Nom", "Message du ticket")] string name)
        {

            var guild = ctx.Guild;
            var overwrites = new[]
            {
                new DiscordOverwriteBuilder(ctx.Guild.EveryoneRole).Deny(Permissions.AccessChannels),
                new DiscordOverwriteBuilder(ctx.Guild.GetRole(1156894465924026438)).Allow(Permissions.AccessChannels),
                new DiscordOverwriteBuilder(ctx.Member).Allow(Permissions.AccessChannels),
            };

            var channel = await guild.CreateChannelAsync(name, ChannelType.Text, overwrites: overwrites);

            string contenu = ctx.Member.Mention + " a créer un ticket. " + ctx.Guild.GetRole(1156894465924026438).Mention;

            await channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(contenu).AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "archive", "Archiver", false)));

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Création du ticket réussi !").AsEphemeral(true));

            await MessageHelper.Log(ctx.Guild, ctx.Member + " a créé un ticket : " + name);

        }



        [SlashCommand("runtime", "Donne le temps pendant lequel le bot ne s'est pas interrompu !")]
        public async Task GetRunTime(InteractionContext ctx)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Temps : " + Stats.sw.Elapsed.Days + "j " + Stats.sw.Elapsed.Hours + "h " + Stats.sw.Elapsed.Minutes + "min " + Stats.sw.Elapsed.Seconds + "sec"));
        }


        [SlashRequireGuild]
        [SlashCommand("pendu", "Créer un pendu !")]
        public async Task Pendu(InteractionContext ctx)
        {

            string path = Path.GetFullPath(Directory.GetCurrentDirectory() + "/" + Stats.PenduDataDataPath + "word.txt");

            string[] lines = File.ReadAllLines(path);
            int r = Random.Shared.Next(0, lines.Length);

            Stats.PenduData data = new Stats.PenduData(lines[r], ctx.Member.Id, ctx.Interaction.ChannelId);

            var pendu = new DiscordEmbedBuilder
            {
                Color = DiscordColor.SpringGreen,
                //ImageUrl = 
                Title = "`" + data.word + "`",
                Description = data.GetPenduGFX() + "" +
                        "```❌ : ```",
                //Footer = f,
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Attention, tu vas te faire pendre, et tu vas rien \"compendre\" !").AddEmbed(pendu));

            ulong messID = ctx.Interaction.GetOriginalResponseAsync().Result.Id;

            data.SetInitialMess(messID);

            Stats.penduDict.Add(Stats.GetPenduKey(ctx.Member.Id, ctx.Interaction.ChannelId), data);

        }


        [SlashCommand("addword", "Ajouter un mot pour le pendu !")]
        public async Task AddWord(InteractionContext ctx, [Option("Mot", "Mot à ajouter")] string mot)
        {

            string validLetter = "abcdefjhijklmnopqrstuvwxyz";

            string newWord = mot.ToLower();

            string path = Path.GetFullPath(Directory.GetCurrentDirectory() + "/" + Stats.PenduDataDataPath + "word.txt");

            string[] existantWords = File.ReadAllLines(path);

            for (int i = 0; i < mot.Length; i++)
            {

                if (!validLetter.Contains(newWord[i]))
                {
                    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le mot `" + mot + "` n'est pas valide, il ne doit contenir que les lettres de base !"));
                    return;
                }

            }

            newWord = newWord.Substring(0, 1).ToUpper() + newWord.Substring(1);

            if (existantWords.Contains(newWord))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le mot `" + mot + "` est déjà dans les fichiers du pendu !"));
                return;
            }

            List<string> words = new List<string> { newWord };
            File.AppendAllLines(path, words);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le mot `" + newWord + "` a bien été ajouté !"));

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("event", "Créer le message d'évenement ! (admin)")]
        public async Task EventMess(InteractionContext ctx)
        {

            var title = new TextInputComponent("Titre : ", "event_title", "Le titre de l'évenement", required: true, style: TextInputStyle.Short);
            var info = new TextInputComponent("Info : ", "event_info", "Description...", required: true, style: TextInputStyle.Paragraph);
            var url = new TextInputComponent("URL de l'image : ", "event_url", "Le lien vers l'image", required: false, style: TextInputStyle.Short);

            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Annonce de jeu")
                .WithCustomId("event_generator")
                .AddComponents(title)
                .AddComponents(info)
                .AddComponents(url);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

        }


        [SlashCommand("website", "Donne l'url du site !")]
        public async Task GetURL(InteractionContext ctx)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("https://ingegamez.isc-vs.ch/"));
        }


        [SlashCommand("mcstatus", "Une commande pour afficher l'état du serveur Minecraft !")]
        public async Task MinecraftStatus(InteractionContext ctx)
        {

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ping..."));

            string fileName = Token.MC_STATUS_PATH;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo(Token.PYTHON_PATH, fileName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine(output);

            string[] str = output.Split("\n");


            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Title = "Minecraft Java Server Status",
                Description = "```" +
                "\n" +        "Version : " + str[0] +
                "\n" +        "Latency : " + str[3].Substring(0, 5) + " ms" +
                "\n" +       $"Players : {str[1]} / {str[2]}" +                 // str[1].Substring(0, str[1].Length - 1)
                "```",
            };

            await ctx.Interaction.DeleteOriginalResponseAsync();
            await ctx.Interaction.Channel.SendMessageAsync(embed: message);

        }


        /**[SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("update", "Une commande pour mettre le bot à jour ! (admin)")]
        public async Task Update(InteractionContext ctx)
        {

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Mise à jour ..."));

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("./autoexe.sh")
            {
                UseShellExecute = true,
                CreateNoWindow = false,
                WorkingDirectory = "../../../../.."
            };
            p.Start();

            Console.WriteLine("MISE A JOUR");

            p.WaitForExit();

            await ctx.Interaction.Channel.SendMessageAsync("Mise à jour terminé !");

            Console.WriteLine("MISE A JOUR TERMINE");



            Process p2 = new Process();
            p2.StartInfo = new ProcessStartInfo("./IngeBot")
            {
                UseShellExecute = true,
                CreateNoWindow = true,
            };

            await ctx.Interaction.Channel.SendMessageAsync("Redémarrage en cours ...");
            p2.Start();

            Environment.Exit(0);

        }**/


        /*[SlashCommand("level", "Une commande ! (admin)")]
        public async Task ChannelLevel(InteractionContext ctx) // , [Option("Title", "Titre de l'évenement")] string title, [Option("Info", "Info de l'évenement")] string info, [Option("URL", "URL de l'image")] string url
        {

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            string s = "Channels";

            for (int i = 0; i < ctx.Guild.Channels.Count; i++)
            {

                if(ctx.Guild.Channels.ElementAt(i).Value.IsThread)
                    s += "\n" + ctx.Guild.Channels.ElementAt(i).Value.Name;

                Console.WriteLine(ctx.Guild.Channels.ElementAt(i).Value);

            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(s));

        }*/


        /*[SlashCommand("level", "Montre ton niveau ! (admin)")]
        public async Task ChannelLevel(InteractionContext ctx) // , [Option("Title", "Titre de l'évenement")] string title, [Option("Info", "Info de l'évenement")] string info, [Option("URL", "URL de l'image")] string url
        {

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }


            var message2 = new DiscordEmbedBuilder
            {
                Title = "Le niveau de " + ctx.Interaction.User.Username,
                Color = DiscordColor.Violet,
                Description = "```Level " + 1 +
                              "\n" + "Next level : " + "[======>   ]```",
            };


            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message2));

        }*/


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("roles", "(admin)")]
        public async Task Role(InteractionContext ctx)
        {

            string message = "```";

            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
            {
                message += ctx.Guild.Roles.ElementAt(i).Value + " \n";
            }

            message += "```";

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));

        }


        //[SlashCommand("profil", "(admin)")]
        //public async Task Profil(InteractionContext ctx)
        //{
        //    if (ctx.Guild == null)
        //    {
        //        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
        //        return;
        //    }

        //    if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
        //    {
        //        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
        //        return;
        //    }

        //    var inge = DiscordEmoji.FromName(ctx.Client, ":IngeDie:");
        //    var cd = DiscordEmoji.FromGuildEmote(ctx.Client, 1280589593871388774);

        //    var message = new DiscordEmbedBuilder
        //    {

        //        Author = new DiscordEmbedBuilder.EmbedAuthor()
        //        {
        //            IconUrl = ctx.Interaction.User.AvatarUrl,
        //            Name = ctx.Interaction.User.Username,
        //        },



        //        //Title = "",
        //        Color = DiscordColor.CornflowerBlue,
        //        Description = $"{cd} {inge}",

        //        //Footer = new DiscordEmbedBuilder.EmbedFooter()
        //        //{
        //        //    Text = $"{ingeEmoji}", // "<:IngeDie:1407765372089798726>",
        //        //}

        //    };

        //    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

        //}







        /*[SlashCommand("join", "3754")]
        public async Task JoinCommand(InteractionContext ctx, [Option("salon", "Titre de l'évenement")] DiscordChannel channel)
        {
            channel ??= ctx.Member.VoiceState?.Channel;
            await channel.ConnectAsync();

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Connection au salon vocal " + channel.Mention + " réussi !"));

        }

        [SlashCommand("play", "7483")]
        public async Task PlayCommand(InteractionContext ctx, [Choice("Super Mario", "mario.mp3")][Choice("Worms", "worms.wav")][Choice(".", ".")][Option("Son", "Son")] string path)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var connection = vnext.GetConnection(ctx.Guild);

            var transmit = connection.GetTransmitSink();

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le son " + path + " est joué !"));

            var pcm = ConvertAudioToPcm(path);
            await pcm.CopyToAsync(transmit);
            await pcm.DisposeAsync();

        }

        [SlashCommand("leave", "4037")]
        public async Task LeaveCommand(InteractionContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();
            var connection = vnext.GetConnection(ctx.Guild);
            connection.Disconnect();

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Deconnection du salon vocal réussi !"));

        }


        private Stream ConvertAudioToPcm(string filePath)
        {
            var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            return ffmpeg.StandardOutput.BaseStream;
        }




        [SlashCommand("level", "Montre ton niveau ! (admin)")]
        public async Task ChannelLevel(InteractionContext ctx) // , [Option("Title", "Titre de l'évenement")] string title, [Option("Info", "Info de l'évenement")] string info, [Option("URL", "URL de l'image")] string url
        {

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }


            var message2 = new DiscordEmbedBuilder
            {
                Title = "Le niveau de " + ctx.Interaction.User.Username,
                Color = DiscordColor.Yellow,
                Description = "Nombre de messages envoyés : " + Stats.userMessages[ctx.Interaction.User.Id]
                //Description = "```Level " + 1 +
                              //"\n" + "Next level : " + "[======>   ]```",
            };


            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message2));

        }*/





        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("welcome", "Une commande totalement stateless pour dire BIENVENU 4.0 ! (admin)")]
        public async Task Welcome4(InteractionContext ctx, [Option("Builder", "Constructeur pour les boutons")] string builder)
        {

            try
            {

                var message = new DiscordEmbedBuilder
                {
                    Title = "Bienvenu sur le serveur Discord d'IngéGamEZ !",
                    Color = DiscordColor.Violet,
                    Description = "**Veuillez lire et accepter les règles du serveur**" +
                    " \n - Ne pas insulter " +
                    " \n - Être respectueux les uns envers les autres " +
                    " \n - La publicité doit etre permise au préalable par l'un des membres du comité" +
                    " \n - Restez courtois et respectueux" +
                    " \n - Si le bot a des bugs, dites le nous !",
                };


                await MessageHelper.Log(ctx.Guild, "L'utilisateur " + ctx.User.Username + " a utilisé la commande /welcome [`" + builder + "`]");



                string[] split = builder.Split("/");

                DiscordButtonComponent[] buttons = new DiscordButtonComponent[split.Length];

                for (int i = 0; i < split.Length; i++)
                {
                    ulong id = ulong.Parse(split[i].Trim().Substring(3, split[i].Trim().Length - 4));
                    buttons[i] = new DiscordButtonComponent(ButtonStyle.Primary, id.ToString(), ctx.Guild.GetRole(id).Name, false);
                }


                var message2 = new DiscordEmbedBuilder
                {
                    Title = "Pourquoi êtes-tu ici ?",
                    Color = DiscordColor.Violet,
                    Description = "Tu peux appuyer sur les boutons pour t'ajouter les rôles pour avoir accès aux salons en rapport avec les événements.",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                        Text = "Welcome message 4.0",
                    },
                };

                DiscordMessageBuilder mBuilder = new DiscordMessageBuilder();
                mBuilder.AddEmbed(message2);

                for (int i = 0; i < buttons.Chunk(5).Count(); i++)
                    mBuilder.AddComponents(buttons.Chunk(5).ToList()[i]);


                await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(message));
                await ctx.Channel.SendMessageAsync(mBuilder);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Welcome message 4.0").AsEphemeral(true));


            }
            catch (Exception e)
            {
                var error = new DiscordEmbedBuilder
                {
                    Title = "Une erreur est survenu !",
                    Color = DiscordColor.DarkRed,
                    Description = e.Message + "\nLe format doit être : @role / @role / ...",
                };
                await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(error));
            }

        }



        [SlashRequireGuild]
        [SlashCommand("birthday-set", "Ajouter ton anniversaire !")]
        public async Task SetBirthday(InteractionContext ctx, [Option("Date", "Format : 2025/01/01")] string date)
        {

            if (!DateTime.TryParseExact(date, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime formattedDate))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("La date doit avoir ce format : `yyyy/MM/dd`"));
                return;
            }

            DateTime d = new DateTime(
                DateTime.Now.Year,
                formattedDate.Month,
                formattedDate.Day,
                9,
                0,
                0
            );

            int diff = DateTime.Now.CompareTo(d);
            if (diff > 0) d = d.AddYears(1);


            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.WELCOME_CHANNEL);
            DiscordChannel channel = ctx.Interaction.Guild.GetChannel(ulong.Parse(param?.value ?? "0")) ?? ctx.Interaction.Guild.GetDefaultChannel();


            DelayedMessage? m = DelayedMessage.FindByNameAndGuild($"ingebot_birthday_{ctx.Interaction.User.Id}", (long)ctx.Guild.Id);

            if (m == null)
                m = new DelayedMessage((long)ctx.Guild.Id, (long)channel.Id, (long)ctx.Client.CurrentUser.Id, $"ingebot_birthday_{ctx.Interaction.User.Id}", ctx.Interaction.User.Id.ToString() + "_" + formattedDate.Year, d, true);
            else
                m.date = d;

            m.Save();

            MessageDelayerService.UpdateDelayedMessage(m, ctx.Client);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ton anniversaire a été défini : {formattedDate.ToString("yyyy/MM/dd")}").AsEphemeral(true));

        }

        [SlashRequireGuild]
        [SlashCommand("birthday-del", "Supprimer ton anniversaire !")]
        public async Task DeleteBirthday(InteractionContext ctx)
        {

            string name = $"ingebot_birthday_{ctx.Interaction.User.Id}";

            DelayedMessage? m = DelayedMessage.FindByNameAndGuild(name, (long)ctx.Guild.Id);

            if (m == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ton anniversaire n'est pas défini !").AsEphemeral(true));
                return;
            }

            m.Delete();

            MessageDelayerService.DeleteDelayedMessage(m);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ton anniversaire a été supprimer !").AsEphemeral(true));
        }

        [SlashRequireGuild]
        [SlashCommand("birthday", "Donne la date à laquelle tu as défini ton anniversaire !")]
        public async Task GetBirthday(InteractionContext ctx)
        {

            string name = $"ingebot_birthday_{ctx.Interaction.User.Id}";

            DelayedMessage? m = DelayedMessage.FindByNameAndGuild(name, (long)ctx.Guild.Id);

            if (m == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Ton anniversaire n'est pas défini !\nTu peux le définir à l'aide de la commande `/birthday-set` !").AsEphemeral(true));
                return;
            }

            DateTime birthday = new DateTime(
                int.Parse(m.text.Split("_")[1]),
                m.date.Month,
                m.date.Day
            );

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Ton anniversaire est défini le : {birthday.ToString("yyyy/MM/dd")} !").AsEphemeral(true));

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-message-create", "Créer un message programmé ! (admin)")]
        public async Task CreateDelayedMessage(InteractionContext ctx)
        {

            var name = new TextInputComponent("Nom du message", "message_name", "Utiliser un nom déjà existant le mettra juste à jour !", required: true, style: TextInputStyle.Short);
            var text = new TextInputComponent("Message : ", "message", "Le message...", required: true, style: TextInputStyle.Paragraph);
            var date = new TextInputComponent("Date et heure de l'envoi : ", "message_time", "Format : 2025/01/01 12:45", required: true, style: TextInputStyle.Short);

            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Nouveau message programmé")
                .WithCustomId("delayed-message-modal")
                .AddComponents(name)
                .AddComponents(text)
                .AddComponents(date);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-message-info", "Affiche les infos d'un message programmé ! (admin)")]
        public async Task InfoDelayedMessage(InteractionContext ctx, [Option("Nom", "Nom du message à afficher.")] string name)
        {

            DelayedMessage? ms = DelayedMessage.FindByNameAndGuild(name, (long)ctx.Guild.Id);

            if (ms == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le message ```{name}``` n'existe pas !"));
                return;
            }

            DiscordUser? user = await ctx.Interaction.Guild.GetMemberAsync((ulong)ms.ownerId);
            string username = user?.Mention ?? "Inconnu";

            DiscordChannel? channel = ctx.Interaction.Guild.GetChannel((ulong)ms.channelId);
            string channelName = channel?.Mention ?? "Inconnu";

            var msg = new DiscordEmbedBuilder
            {
                Title = "Détails du message programmé :",
                Color = DiscordColor.Gray,
                Description = $"Nom : {ms.name}" +
                                  $"\nCréateur : {username}" +
                                  $"\nDate d'envoi : {ms.date.ToString("yyyy/MM/dd HH:mm")}" +
                                  $"\nSalon d'envoi : {channelName}" +
                                  $"\nTexte : {ms.text}",

                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Text = "Message Delayer System 2.0",
                },
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(msg));

        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-message-delete", "Supprimer un message programmé ! (admin)")]
        public async Task DeleteDelayedMessage(InteractionContext ctx)
        {

            DelayedMessage[] ms = DelayedMessage.FindByGuild((long)ctx.Guild.Id);

            List<DiscordSelectComponentOption> options = new List<DiscordSelectComponentOption>();

            for (int i = 0; i < ms.Length; i++)
            {
                if (!ms[i].name.StartsWith("ingebot_"))
                    options.Add(new DiscordSelectComponentOption(ms[i].name, ms[i].name));
            }

            if (options.Count == 0)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Il n'y a pas de message programmé !"));
                return;
            }

            var msg = new DiscordEmbedBuilder
            {
                Title = $"Veux-tu supprimer un message ?",
                Color = DiscordColor.Orange,
                Description = "Quel message veux-tu supprimer ?",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Text = "Delayed Message System 2.0",
                },
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(msg).AddComponents(new DiscordSelectComponent("delayed_message_delete", "Message", options)));

        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-message-list", "Permet d'afficher la liste des messages programmés ! (admin)")]
        public async Task ListDelayedMessage(InteractionContext ctx)
        {

            DelayedMessage[] ms = DelayedMessage.FindByGuild((long)ctx.Guild.Id);

            string desc = "```\n";

            for (int i = 0; i < ms.Length; i++)
            {
                if (!ms[i].name.StartsWith("ingebot_"))
                    desc += $"{ms[i].name}\n";
            }
            desc += "```";

            if (desc == "```\n```")
                desc = "Il n'y a pas de message programmé pour le moment...";

            var message = new DiscordEmbedBuilder
            {
                Title = "List des messages programmés :",
                Color = DiscordColor.Gray,
                Description = desc,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Text = "Message Delayer System 2.0",
                },
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

        }


        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("setrole-2-0", "Permet de donner un rôle temporaire à un membre du serveur ! (admin)")]
        public async Task SetRole(InteractionContext ctx, [Option("Nom", "Permet, après, de le supprimer à partir de son nom.")] string name, [Option("Utilisateur", "L'utilisateur")] DiscordUser user, [Option("Rôle", "Le rôle")] DiscordRole role, [Option("Début", "Format : 2025/01/01 10:00 ou -")] string start, [Option("Fin", "Format : 2025/01/01 10:00 ou -")] string end)
        {

            if (!DateTime.TryParseExact(start, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateStart) && start != "-")
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("La date de début doit avoir ce format : `yyyy/MM/dd HH:mm`"));
                return;
            }

            if (!DateTime.TryParseExact(end, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateEnd) && end != "-")
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("La date de début doit avoir ce format : `yyyy/MM/dd HH:mm`"));
                return;
            }

            if (start == "-")
                parsedDateStart = DateTime.Now.AddSeconds(1);

            if (end == "-")
                parsedDateEnd = DateTime.MinValue;


            DelayedRole? r = DelayedRole.FindByNameAndGuild(name, (long)ctx.Guild.Id);

            string title = "Détails du rôle temporaire :";
            if (r == null)
            {
                r = new DelayedRole(
                    (long)ctx.Guild.Id, 
                    (long)ctx.User.Id, 
                    (long)user.Id, 
                    (long)role.Id, 
                    name, 
                    parsedDateStart, 
                    parsedDateEnd
                );
            }
            else
                title = "Rôle temporaire mis à jour :";


            DiscordUser? discordUser = await ctx.Interaction.Guild.GetMemberAsync((ulong)r.targetId);
            string username = discordUser?.Mention ?? "Inconnu";

            DiscordRole? discordRole = ctx.Interaction.Guild.GetRole((ulong)r.roleId);
            string rolename = discordRole?.Mention ?? "Inconnu";

            var message = new DiscordEmbedBuilder
            {
                Title = title,
                Color = DiscordColor.Gray,
                Description = $"Utilisateur cible : {username}" +
                                $"\nRôle : {rolename}" +
                                $"\nDate de début : {r.date_start.ToString("yyyy/MM/dd HH:mm")}" +
                                $"\nDate de fin : {r.date_end.ToString("yyyy/MM/dd HH:mm")}",

                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Text = "Role Delayer System 2.0",
                },
            };


            if (r.Save())
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AsEphemeral(true));
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Une erreur est survenu lors de la sauvegarde du rôle temporaire...").AddEmbed(message).AsEphemeral(true));

            /// Mise à jour dans le RoleDelayerService
            RoleDelayerService.UpdateDelayedRole(r, ctx.Client);

        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-role-list", "Permet d'afficher la liste des rôles temporaires programmés ! (admin)")]
        public async Task ListDelayedRole(InteractionContext ctx)
        {

            DelayedRole[] rs = DelayedRole.FindByGuild((long)ctx.Guild.Id);

            string desc = "```\n";

            for (int i = 0; i < rs.Length; i++)
            {
                desc += $"{rs[i].name}\n";
            }
            desc += "```";

            if (rs.Length == 0) desc = "Il n'y a pas de rôle temporaire programmé pour le moment...";

            var message = new DiscordEmbedBuilder
            {
                Title = "List des rôles temporaires :",
                Color = DiscordColor.Gray,
                Description = desc,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.User.AvatarUrl,
                    Text = "Role Delayer System 2.0",
                },
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-role-delete", "Permet de supprimer un rôle temporaire programmé ! (admin)")]
        public async Task DeleteDelayedRole(InteractionContext ctx, [Option("Nom", "Nom du role temporaire")] string name)
        {

            DelayedRole? r = DelayedRole.FindByNameAndGuild(name, (long)ctx.Guild.Id);

            if (r == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le rôle temporaire ```{name}``` n'existe pas !"));
            }
            else
            {

                r.Delete();

                /// Mise à jour dans le RoleDelayerService
                RoleDelayerService.DeleteDelayedRole(r);

                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le rôle temporaire ```{name}``` a été supprimé !"));
            }

        }

        [SlashRequireGuild]
        [SlashRequireAdmin]
        [SlashCommand("delayed-role-info", "Permet d'afficher les détails d'un rôle temporaire programmé ! (admin)")]
        public async Task InfoDelayedRole(InteractionContext ctx, [Option("Nom", "Nom du role temporaire")] string name)
        {

            DelayedRole? r = DelayedRole.FindByNameAndGuild(name, (long)ctx.Guild.Id);

            if (r == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le rôle temporaire ```{name}``` n'existe pas !"));
                return;
            }
            
            DiscordUser? user = await ctx.Interaction.Guild.GetMemberAsync((ulong)r.targetId);
            string username = user?.Mention ?? "Inconnu";

            DiscordRole? role = ctx.Interaction.Guild.GetRole((ulong)r.roleId);
            string rolename = role?.Mention ?? "Inconnu";


            var message = new DiscordEmbedBuilder
            {
                Title = "Détails du rôle temporaire :",
                Color = DiscordColor.Gray,
                Description = $"Utilisateur cible : {username}" +
                                $"\nRôle : {rolename}" +
                                $"\nDate de début : {r.date_start.ToString("yyyy/MM/dd HH:mm")}" +
                                $"\nDate de fin : {r.date_end.ToString("yyyy/MM/dd HH:mm")}",

                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Text = "Role Delayer System 2.0",
                },
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));

        }


        [SlashRequireGuild]
        [SlashRequireSuperAdmin]
        [SlashCommand("set-admin-role", "Permet de définir le rôle qui a le droit de lancer les commandes d'admin ! (super admin)")]
        public async Task SetAdminRole(InteractionContext ctx, [Option("Rôle", "Le rôle")] DiscordRole role)
        {

            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.ADMIN_ROLE);
            if (param == null) param = new Parameter((long)ctx.Guild.Id, Parameter.ADMIN_ROLE, "");

            param.value = role.Id.ToString();
            param.Save();

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le role d'admin a été defini sur : {role.Mention}\nToute personne ayant ce role pourra lancer les commandes d'admin !"));
        }

        [SlashRequireGuild]
        [SlashRequireSuperAdmin]
        [SlashCommand("reset-admin-role", "Permet de réinitiliser le rôle qui a le droit de lancer les commandes d'admin ! (super admin)")]
        public async Task ResetAdminRole(InteractionContext ctx)
        {

            Parameter? param = Parameter.FindByGuildIdAndKey(ctx.Guild.Id, Parameter.ADMIN_ROLE);
            if (param == null) param = new Parameter((long)ctx.Guild.Id, Parameter.ADMIN_ROLE, "");

            param.Delete();

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le role d'admin a été réinitialisé !"));
        }

        //[SlashCommand("sendto", "Une commande pour envoyer un message incognito. héhéhé !")]
        //public async Task SendMessTo(InteractionContext ctx, [Option("User", "Destinataire")] DiscordUser u, [Option("Message", "Message à envoyer")] string m)
        //{

        //    var m2 = await ctx.Guild.GetMemberAsync(u.Id);
        //    var c = await m2.CreateDmChannelAsync();

        //    await c.SendMessageAsync(m);
        //    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le message a fonctionné").AsEphemeral(true));
        //}


    }
}
