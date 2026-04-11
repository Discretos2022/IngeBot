using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using DSharpPlus.VoiceNext;
using IngeBot.DelayerEngine;
using IngeBot.Models;
using IngeBot.Modules;
using IngeBot.Services;
using System;
using System.Globalization;

namespace IngeBot
{
    internal class Program
    {

        private CommandsNextExtension cExtension;
        private static DiscordClient client;

        public static DiscordRestClient restClient;

        private Thread statusThread;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {


            if (IntPtr.Size == 8)
                Console.WriteLine("IngéBot " + Stats.version + " x64 (c) 2024 Joshua Siedel");
            else if (IntPtr.Size == 4)
                Console.WriteLine("IngéBot " + Stats.version + " x86 (c) 2024 Joshua Siedel");


            Configuration config = EnvLoader.LoadEnv();
            Console.WriteLine("Config File Loaded !");

            DatabaseSystem.Init(config.DataBaseHost, config.DataBasePort, config.DataBaseUsername, config.DataBasePassword, config.DataBaseName);

            Stats.PenduDataDataPath = config.DataPath;


            //DelayedMessage? m = DelayedMessage.FindById(3);
            //m?.Delete();

            //DelayedMessage m1 = new DelayedMessage(1010, 1010, 1010, "test", "text", DateTime.Now, false, true);
            //DelayedMessage m2 = new DelayedMessage(1010, 1010, 1010, "test", "text", DateTime.Now, false, true);
            //DelayedMessage m3 = new DelayedMessage(1010, 1010, 1010, "test", "text", DateTime.Now, false, true);
            //DelayedMessage m4 = new DelayedMessage(1010, 1010, 1010, "test", "text", DateTime.Now, false, true);

            //m1.Save();
            //m2.Save();
            //m3.Save();
            //m4.Save();



            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = config.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            client = new DiscordClient(discordConfig);

            client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            client.Ready += OnClientReady;
            client.MessageCreated += MessageCreatedHandler;
            client.MessageUpdated += MessageUpdatedHandler;
            client.MessageDeleted += MessageDeletedHandler;
            client.GuildMemberAdded += GuildMemberAddedHandler;
            client.GuildMemberRemoved += GuildMemberRemovedHandler;
            client.ComponentInteractionCreated += PressedButton;
            client.ModalSubmitted += ModalHandler;
            client.GuildDownloadCompleted += AfterGuildsLoading;

            client.SocketClosed += SocketClosedHandler;
            client.SocketErrored += SocketErroredHandler;
            client.Zombied += ZombiedHandler;

            client.GuildRoleCreated += RoleCreatedHandler;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { "/" },
                EnableDms = true,
                EnableMentionPrefix = true,
                DmHelp = true,
                //Services = services
            };

            cExtension = client.UseCommandsNext(commandsConfig);
            var slashUserCommandsConfiguration = client.UseSlashCommands();

            cExtension.RegisterCommands<Commands>();
            slashUserCommandsConfiguration.RegisterCommands<SlashCommands>();

            slashUserCommandsConfiguration.SlashCommandErrored += SlashCommandErroredHandler;

            client.UseVoiceNext();

            try
            {
                await client.ConnectAsync(status: UserStatus.Online);
            }
            catch (Exception)
            {
                Console.WriteLine("Discord Connection Has Failed !");
                Environment.Exit(1);
            }

            //statusThread = new Thread(() => UpdateStatusLoop());
            //statusThread.Start();

            restClient = new DiscordRestClient(discordConfig);

            Stats.sw.Start();

            await Task.Delay(-1);

        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {

            if (e.Message.Content.ToLower().Contains("cd"))
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(sender, ":CD2000x:")); // DiscordAttachment

            foreach (string i in Stats.saluts)
                if (e.Message.Content.ToLower().Contains(i))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(sender, ":wave:")); // DiscordAttachment
                    break;
                }

            Parameter? param = Parameter.FindByGuildIdAndKey(e.Guild?.Id ?? 0, Parameter.MODERATION);
            string moderationParam = param?.value ?? "false";
            bool moderation = bool.Parse((moderationParam == "true" || moderationParam == "false") ? moderationParam : "false");

            if (moderation)
            {

                if (e.Message.Content.Contains("merde"))
                {
                    //await e.Message.CreateReactionAsync(DiscordEmoji.FromName(sender, ":x:"));
                    await e.Message.DeleteAsync();

                    await e.Channel.SendMessageAsync("Message supprimé pour des raisons de sécurité !");

                    /*if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                    */


                }
                else if (e.Message.Content.Contains(" con "))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }
                else if (e.Message.Content.Contains("fdp"))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }
                else if (e.Message.Content.Contains("débile"))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }
                else if (e.Message.Content.Contains("crétin"))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }
                else if (e.Message.Content.Contains("idiot"))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }
                else if (e.Message.Content.Contains("zinzin"))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }
                else if (e.Message.Content.Contains("merdouille"))
                {
                    if (e.Author.Username == "discretos")
                        await e.Channel.SendMessageAsync("Le développeur qui insulte !?  Mais c'est pas vrai.  :grin: ");
                    else
                        await e.Channel.SendMessageAsync(e.Guild.GetMemberAsync(e.Author.Id).Result.DisplayName + " est un grossier personnage !");
                }

            }



            // Pendu

            string key = Stats.GetPenduKey(e.Author.Id, e.Channel.Id);

            if (Stats.penduDict.ContainsKey(key))
            {
                if(e.Message.Content.Length == 1)
                {
                    //await e.Channel.SendMessageAsync("lettre : " + e.Message.Content);
                    Stats.penduDict[key].UpdateWord(e.Message.Content.ToString()[0]);
                    //await e.Channel.SendMessageAsync("new word : `" + Stats.penduDict[Stats.GetPenduKey(e.Author.Id, e.Channel.Id)].word + "`");
                    await e.Message.DeleteAsync();

                    string letters = "";

                    for (int i = 0; i < Stats.penduDict[key].used.Count; i++)
                        letters += Stats.penduDict[key].used[i];

                    string result = "";

                    bool finish = false;

                    if (Stats.penduDict[key].used.Count == 10)
                    {
                        result = "Perdu ! Le mot à trouver est : ```" + Stats.penduDict[key].hidedWord + "```";
                        finish = true;
                    }
                    if (!Stats.penduDict[key].word.Contains('_'))
                    {
                        result = "Gagné ! Tu as trouvé le mot !";
                        finish = true;
                    }

                    var pendu = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.SpringGreen,
                        //ImageUrl = 
                        Title = "`" + Stats.penduDict[key].word + "`",
                        Description = Stats.penduDict[key].GetPenduGFX() + "" +
                        "```❌ : " + letters + "```" +
                        "\n" + result,
                        //Footer = f,
                    };

                    DiscordMessage m = e.Channel.GetMessageAsync(Stats.penduDict[key].initialMessage).Result;
                    await m.ModifyAsync(new DiscordMessageBuilder().WithContent("Attention, tu vas te faire pendre, et tu vas rien \"compendre\" !").AddEmbed(pendu));

                    if (finish)
                        Stats.penduDict.Remove(Stats.GetPenduKey(e.Author.Id, e.Channel.Id));

                }

                if (e.Message.Content.ToLower() == "exit")
                {
                    Stats.penduDict.Remove(Stats.GetPenduKey(e.Author.Id, e.Channel.Id));
                    await e.Channel.SendMessageAsync("La partie de pendu a été abandonné !");
                }

            }


        }

        private static async Task MessageUpdatedHandler(DiscordClient sender, MessageUpdateEventArgs e)
        {

            if (e.Guild == null) return;

            string oldMess = "";
            if (e.MessageBefore == null)
                oldMess = "/!\\ Il y a une erreur !";
            else
                oldMess = e.MessageBefore.Content;

            var t = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = e.Guild.GetMemberAsync(e.Message.Author.Id).Result.Nickname,
                IconUrl = e.Message.Author.AvatarUrl,
            };

            var message = new DiscordEmbedBuilder
            {
                
                Footer = t,
                Title = "Message modifié par " + e.Guild.GetMemberAsync(e.Message.Author.Id).Result.Nickname + " dans " + e.Channel.Mention,
                Color = DiscordColor.SpringGreen,
                Description = "**Ancien : **" +
                "\n" + oldMess +
                "\n" +
                "\n**Nouveau : **" +
                "\n" + e.Message.Content,

                Timestamp = DateTime.Now,
            };


            if (e.Author.Username != "IngéBot" && e.Author.Username != "IngéBot_Bêta")
                if (oldMess != e.Message.Content)
                    await MessageHelper.Log(e.Guild, new DiscordMessageBuilder().AddEmbed(message));
        }

        private static async Task MessageDeletedHandler(DiscordClient sender, MessageDeleteEventArgs e)
        {

            if (e.Guild == null) return;

            string oldMess = e.Message.Content;
            if (e.Message.Content == "")
                oldMess = "/!\\ Il y a une erreur !";

            var t = new DiscordEmbedBuilder.EmbedFooter
            {
                Text = e.Guild.GetMemberAsync(e.Message.Author.Id).Result.Nickname,
                IconUrl = e.Message.Author.AvatarUrl,
            };

            var deleted = new DiscordEmbedBuilder
            {
                Color = DiscordColor.SpringGreen,
                Title = "Message supprimé par " + e.Guild.GetMemberAsync(e.Message.Author.Id).Result.Nickname + " dans " + e.Channel.Mention,
                Description = "**Message : **" + oldMess,
                Timestamp = DateTime.Now,
                Footer = t,
            };

            if(e.Message.Author != null)
            {
                if (e.Message.Author.Username != "IngéBot" && e.Message.Author.Username != "IngéBot_Bêta")
                    await MessageHelper.Log(e.Guild, new DiscordMessageBuilder().AddEmbed(deleted));
            }

        }

        private static async Task GuildMemberAddedHandler(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            await MessageHelper.Welcome(e.Guild, "Bienvenu sur le serveur !   Accueillez : " + e.Member.DisplayName);
        }

        private static async Task GuildMemberRemovedHandler(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            await MessageHelper.Log(e.Guild, e.Member.DisplayName + " est partie...");
        }

        private static async Task RoleCreatedHandler(DiscordClient sender, GuildRoleCreateEventArgs e)
        {

            var audits = await e.Guild.GetAuditLogsAsync(1, action_type: AuditLogActionType.RoleCreate);
            var entry = audits.FirstOrDefault();
            var user = entry?.UserResponsible;

            string author = user?.Username ?? "inconnu";

            await MessageHelper.Log(e.Guild, "Le role " + e.Role.Mention + " a été créer par " + author + ".");
        }



        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }


        private static async Task PressedButton(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {

            var m = await e.Guild.GetMemberAsync(e.User.Id);
            var c = await m.CreateDmChannelAsync();

            bool welcomeEph = true;

            if (e.Interaction.Data.CustomId == "game_jam")
            {

                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1160611550063767683)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1160611550063767683));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle " + e.Guild.GetRole(1160611550063767683).Mention + " vous à été enlevé ! Tu n'as désormais plus accès aux salons concernant la Game Jam !").AsEphemeral(welcomeEph));
                }
                else
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1160611550063767683));

                    var t = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = e.Message.Author.AvatarUrl,
                        Text = "https://forms.office.com/e/T7zPcwnHNd"
                    };

                    var lien = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.SpringGreen,
                        Title = "**Pré-inscription Game Jam**",
                        Description = "Pour compter comme inscrit pour la gamejam, inscrivez-vous sur ce formulaire. " +
                        "\n (Ceci est la pré-inscription, nous reviendrons vers vous pour une inscription officielle)",
                        Timestamp = DateTime.Now,
                        Url = "https://forms.office.com/e/T7zPcwnHNd",
                        Footer = t,
                    };


                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle " + e.Guild.GetRole(1160611550063767683).Mention + " vous à été conféré ! Tu as désormais accès aux salons concernant la Game Jam !").AddEmbed(lien).AsEphemeral(welcomeEph));
                }

            }

            else if (e.Interaction.Data.CustomId == "warhammer")
            {

                /*if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1156946871571456040)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1156946871571456040));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @Warhammer vous à été enlevé ! Tu n'as désormais plus accès aux salons concernant l'événement sur Warhammer !").AsEphemeral(welcomeEph));
                }
                else
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1156946871571456040));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @Warhammer vous à été conféré ! Tu as désormais accès aux salons concernant l'événement sur Warhammer !").AsEphemeral(welcomeEph));
                }*/


                if (!e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1167037183861989428)))
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Accepte d'abord les règles avant d'appuyer sur ces boutons !").AsEphemeral(true));
                else
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("L'événement n'est pas encore totalement prêt ! (Le rôle n'existe pas encore)").AsEphemeral(welcomeEph));

            }

            else if (e.Interaction.Data.CustomId == "jeudi_soir")
            {

                if (!e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1354869344370163952)))
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Accepte d'abord les règles avant d'appuyer sur ces boutons !").AsEphemeral(true));
                    return;
                }


                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1167037183861989428)))
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1167037183861989428));
                else
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1167037183861989428));

                DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Primary, "jeu_soc", "Jeux de société", false);
                DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Primary, "jeu_vid", "Jeux Vidéo", false);

                var message = new DiscordEmbedBuilder
                {
                    Title = "Pour quel type de jeux ?",
                    Color = DiscordColor.Violet,
                };

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AddComponents(b1, b2).AsEphemeral(welcomeEph));

            }

            else if (e.Interaction.Data.CustomId == "jeu_soc")
            {

                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1156946871571456040)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1156946871571456040));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @jeux-société vous à été enlevé ! Tu n'as désormais plus accès aux salons concernant les jeux de société !").AsEphemeral(welcomeEph));
                }
                else
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1156946871571456040));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @jeux-société vous à été conféré ! Tu as désormais accès aux salons concernant les jeux de société !").AsEphemeral(welcomeEph));
                }

            }

            else if (e.Interaction.Data.CustomId == "jeu_vid")
            {

                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1156942741389987870)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1156942741389987870));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @jeux-vidéo vous à été enlevé ! Tu n'as désormais plus accès aux salons concernant les jeux vidéo !").AsEphemeral(welcomeEph));
                }
                else
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1156942741389987870));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @jeux-vidéo vous à été conféré ! Tu as désormais accès aux salons concernant les jeux vidéo !").AsEphemeral(welcomeEph));
                }

            }

            else if (e.Interaction.Data.CustomId == "accept_rules")
            {
                await c.SendMessageAsync("Tu as accepté les règles ! Sage décision !");


                await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1354869344370163952));

                DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Primary, "hes_yes", "HES", false);
                DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Primary, "hes_no", "Hors HES", false);

                var message = new DiscordEmbedBuilder
                {
                    Title = "Bienvenu sur le serveur Discord d'IngéGamEZ !",
                    Color = DiscordColor.Violet,
                    Description = "D'où vient-tu voyageur ?",
                };

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AddComponents(b1, b2).AsEphemeral(welcomeEph));

            }

            else if (e.Interaction.Data.CustomId == "hes_yes" || e.Interaction.Data.CustomId == "hes_no")
            {
                /*DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Primary, "game_jam", "Game Jam", false);
                DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Primary, "jeudi_soir", "Jeudi Soir", false);
                DiscordButtonComponent b100 = new DiscordButtonComponent(ButtonStyle.Primary, "???", "???", true);

                var message = new DiscordEmbedBuilder
                {
                    Title = "Bienvenu sur le serveur Discord d'IngéGamEZ !",
                    Color = DiscordColor.Violet,
                    Description = "Pourquoi êtes-tu ici ?",
                };*/

                //await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AddComponents(b1, b2, b100).AsEphemeral(welcomeEph));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Et bien, bienvenu, voyageur !").AsEphemeral(welcomeEph));
            }


            else if (e.Interaction.Data.CustomId == "no_accept_rules")
            {

                await MessageHelper.Log(e.Guild, e.Interaction.User.Username + " n'a pas accepté les règles...");

                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1354869344370163952)))
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1354869344370163952));

                await c.SendMessageAsync("Tu n'as pas accepté les règles ? Pourquoi ? Si il y a un problème avec, parles-en avec un @ingénieurs.");

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("...").AsEphemeral(false));
                await e.Interaction.DeleteOriginalResponseAsync();
            }

            else if (e.Interaction.Data.CustomId == "roles")
            {
                Console.WriteLine("DATA : " + e.Values[0]);
                Stats.role = e.Values[0];

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Role Séléctionné : " + e.Guild.GetRole(ulong.Parse(e.Values[0])).Name));
            }

            else if (e.Interaction.Data.CustomId == "user")
            {
                //Console.WriteLine("DATA : " + e.Values[0] + " / " + e.Guild.GetMemberAsync(ulong.Parse(e.Values[0])).Result.ToString());
                Stats.user = e.Values[0];

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Utilisateur Séléctionné : " + e.Guild.GetMemberAsync(ulong.Parse(e.Values[0])).Result.Nickname));

            }

            else if (e.Interaction.Data.CustomId == "valid")
            {

                if (Stats.role == "" || Stats.user == "")
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Selectionne un rôle et un utilisateur !"));

                string mess;

                if (Stats.date != "")
                {

                    mess = "IngéBot has grant role " + e.Guild.GetRole(ulong.Parse(Stats.role)).Mention.ToString() + " to user " + e.Guild.GetMemberAsync(ulong.Parse(Stats.user)).Result.Mention + " ! (End : " + Stats.date + ")";
                }
                else
                    mess = "IngéBot has grant role " + e.Guild.GetRole(ulong.Parse(Stats.role)).Mention.ToString() + " to user " + e.Guild.GetMemberAsync(ulong.Parse(Stats.user)).Result.Mention + " !";


                await e.Guild.GetMemberAsync(ulong.Parse(Stats.user)).Result.GrantRoleAsync(e.Guild.GetRole(ulong.Parse(Stats.role)));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(mess));

            }

            else if (e.Interaction.Data.CustomId == "valid_revoke")
            {

                if (Stats.role == "" || Stats.user == "")
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Selectionne un rôle et un utilisateur !"));

                string mess;

                mess = "IngéBot has revoke role " + e.Guild.GetRole(ulong.Parse(Stats.role)).Mention.ToString() + " to user " + e.Guild.GetMemberAsync(ulong.Parse(Stats.user)).Result.Mention + " !";


                await e.Guild.GetMemberAsync(ulong.Parse(Stats.user)).Result.RevokeRoleAsync(e.Guild.GetRole(ulong.Parse(Stats.role)));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(mess));

                Stats.role = "";
                Stats.user = "";

            }

            else if (e.Interaction.Data.CustomId == "seldate")
            {
                var modal = new DiscordInteractionResponseBuilder().WithTitle(" Sélectionne une date de fin").WithCustomId("modal_date_role").AddComponents(new TextInputComponent("Date (FORMAT : AAAA.MM.JJ HH.MM.SS) : ", "id_text_date", "Entre la date avec le BON format"));
                await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);
            }

            else if (e.Interaction.Data.CustomId == "archive")
            {
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Archivage !"));

                var guild = e.Guild;
                var overwrites = new[]
                {
                    new DiscordOverwriteBuilder(e.Guild.EveryoneRole).Deny(Permissions.AccessChannels),
                    new DiscordOverwriteBuilder(e.Guild.GetRole(1156894465924026438)).Allow(Permissions.AccessChannels),
                };

                await e.Channel.ModifyAsync(x => x.PermissionOverwrites = overwrites);

            }

            else if (e.Interaction.Data.CustomId == "delayed_message_delete")
            {

                string name = e.Interaction.Data.Values[0];

                DelayedMessage? dm = DelayedMessage.FindByNameAndGuild(name, (long)e.Guild.Id);

                if (dm == null)
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le message {name} n'existe pas"));
                    return;
                }

                dm.Delete();
                MessageDelayerService.DeleteDelayedMessage(dm);

                DiscordUser? user = await e.Interaction.Guild.GetMemberAsync((ulong)dm.ownerId);
                string username = user?.Mention ?? "Inconnu";

                DiscordChannel? channel = e.Interaction.Guild.GetChannel((ulong)dm.channelId);
                string channelName = channel?.Mention ?? "Inconnu";

                var msg = new DiscordEmbedBuilder
                {
                    Title = "Détails du message programmé :",
                    Color = DiscordColor.Gray,
                    Description = $"Nom : {dm.name}" +
                                  $"\nCréateur : {username}" +
                                  $"\nDate d'envoi : {dm.date.ToString("yyyy/MM/dd HH:mm")}" +
                                  $"\nSalon d'envoi : {channelName}" +
                                  $"\nTexte : {dm.text}",

                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = sender.CurrentUser.AvatarUrl,
                        Text = "Message Delayer System 2.0",
                    },
                };

                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le message {name} a été supprimer !").AddEmbed(msg));

            }


            if (e.Guild.GetRole(ulong.Parse(e.Interaction.Data.CustomId)) != null)
            {
                ulong id = ulong.Parse(e.Interaction.Data.CustomId);

                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(id)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(id));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le rôle {e.Guild.GetRole(id).Mention} vous à été enlevé ! Tu n'as désormais plus accès aux salons concernant {e.Guild.GetRole(ulong.Parse(e.Interaction.Data.CustomId)).Name} !").AsEphemeral(welcomeEph));
                }
                else
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(id));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Le rôle {e.Guild.GetRole(id).Mention} vous à été conféré ! Tu as désormais accès aux salons concernant {e.Guild.GetRole(ulong.Parse(e.Interaction.Data.CustomId)).Name} !").AsEphemeral(welcomeEph));
                }

            }


            //else
            //await c.SendMessageAsync("a, ... ce bouton ne doit pas être là...");
            //await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("a, ... ce bouton ne doit pas être là...").AsEphemeral(true));

        }

        private static async Task ModalHandler(DiscordClient sender, ModalSubmitEventArgs e)
        {

            if (e.Interaction.Data.CustomId == "modal_bot_game")
            {
                Stats.botGame = e.Values.Values.First();
                await client.UpdateStatusAsync(new DiscordActivity(Stats.botGame, ActivityType.Playing), userStatus: UserStatus.Online);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le jeu va être mis à jour avec : " + Stats.botGame));
            }

            else if (e.Interaction.Data.CustomId == "modal_date_role")
            {
                Stats.date = e.Values.Values.First();
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le jeu va être mis à jour avec : " + Stats.botGame));
                await e.Interaction.DeleteOriginalResponseAsync();
            }

            else if (e.Interaction.Data.CustomId == "event_generator")
            {
                string title = e.Values.Values.First();
                string info = e.Values.Values.ElementAt(1);
                string url = e.Values.Values.ElementAt(2);


                var message = new DiscordEmbedBuilder
                {
                    Title = title,
                    Color = DiscordColor.Yellow,
                    Description = info,
                    ImageUrl = url,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = client.CurrentUser.AvatarUrl,
                        Text = "Event Generator 1.0",
                    },
                };


                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));
            }

            else if (e.Interaction.Data.CustomId == "delayed-message-modal")
            {
                string name = e.Values.Values.First();
                string text = e.Values.Values.ElementAt(1);
                string date = e.Values.Values.ElementAt(2);


                if (name.StartsWith("ingebot_"))
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Les noms commençant par `ingebot_` sont réservés au système d'IngéBot !"));
                    return;
                }


                var message = new DiscordEmbedBuilder
                {
                    Title = "Résumé du nouveau message programmé :",
                    Color = DiscordColor.Gray,
                    Description = text + "\n\n Le message sera envoyé le : \n `" + date + "`",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = client.CurrentUser.AvatarUrl,
                        Text = "Message Delayer System 2.0",
                    },
                };


                if (!DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("La date doit avoir ce format : `yyyy/MM/dd HH:mm`"));
                    return;
                }


                DelayedMessage? m = DelayedMessage.FindByNameAndGuild(name, (long)e.Interaction.Guild.Id);

                if (m == null)
                {
                    m = new DelayedMessage(
                        (long)(e.Interaction.GuildId ?? 0),
                        (long)e.Interaction.ChannelId,
                        (long)e.Interaction.User.Id,
                        name,
                        text,
                        parsedDate,
                        false
                    );
                }
                else
                    message.Title = "Message programmé mis à jour :";

                if (m.Save())
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AsEphemeral(true));
                else
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Une erreur est survenu lors de la sauvegarde du message programmé...").AddEmbed(message).AsEphemeral(true));


                /// Mise à jour dans le MessageDelayerService
                MessageDelayerService.UpdateDelayedMessage(m, sender);

            }

        }

        private static async Task AfterGuildsLoading(DiscordClient sender, GuildDownloadCompletedEventArgs e)
        {
            // _ = DelayerService.Run(sender);
        }

        private async Task SlashCommandErroredHandler(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {

            var failedChecks = ((SlashExecutionChecksFailedException)e.Exception).FailedChecks;
            foreach (var failedCheck in failedChecks)
            {
                if (failedCheck is SlashRequireGuildAttribute)
                {
                    await e.Context.CreateResponseAsync($"La commande `/{e.Context.CommandName}` doit être lancé sur un serveur !");
                }

                else if (failedCheck is SlashRequireAdminAttribute)
                {
                    await e.Context.CreateResponseAsync($"Tu n'as pas la permission pour lancer la commande `/{e.Context.CommandName}`");
                }

                else if (failedCheck is SlashRequireDiscAttribute)
                {
                    await e.Context.CreateResponseAsync($"Seul le grand 𝕯𝖎𝖘𝖈𝖗𝖊𝖙𝖔𝖘 peut lancer la commande `/{e.Context.CommandName}`");
                }

            }

        }

        private async Task SocketClosedHandler(DiscordClient sender, SocketCloseEventArgs args)
        {
            Console.WriteLine("Socket Closed : Restart...");
            Environment.Exit(0);
        }

        private async Task SocketErroredHandler(DiscordClient sender, SocketErrorEventArgs args)
        {
            Console.WriteLine("Socket Error : Restart...");
            Environment.Exit(0);
        }

        private async Task ZombiedHandler(DiscordClient sender, ZombiedEventArgs args)
        {
            Console.WriteLine("Connection Is Too Slow !");
            Environment.Exit(1);
        }

    }
}
