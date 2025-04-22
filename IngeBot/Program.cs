using Bot.Modules;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net.Models;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Xml.Linq;

// On Windows   : dotnet publish -c release -r ubuntu.16.04-x64 --self-contained
// On Windows   : dotnet publish -c release -r win-x86 --self-contained
// On Linux     : chmod 777 ./IngeBot

// Figgle.FiggleFonts.Standard.Render("Hello, World!")

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

            /*string file = "Data/defaultChannel.txt";
            string[] lines = File.ReadAllLines(file);
            Stats.logChannel = lines[0];

            string file2 = "Data/welcomeChannel.txt";
            string[] lines2 = File.ReadAllLines(file2);
            Stats.welcomeChannel = lines2[0];*/

            string[] dirs = Directory.GetDirectories("Data");

            for (int i = 0; i < dirs.Length; i++)
            {

                if (File.Exists(dirs[i] + "/save/logchannel.txt"))
                {
                    string[] lines = File.ReadAllLines(dirs[i] + "/save/logchannel.txt");
                    Stats.logChannels.Add(ulong.Parse(dirs[i].Split(new char[] { '\\', '/' })[1]), ulong.Parse(lines[0]));
                }

                if (File.Exists(dirs[i] + "/save/welcomechannel.txt"))
                {
                    string[] lines = File.ReadAllLines(dirs[i] + "/save/welcomechannel.txt");
                    Stats.welcomeChannels.Add(ulong.Parse(dirs[i].Split(new char[] { '\\', '/' })[1]), ulong.Parse(lines[0]));
                }

                if (File.Exists(dirs[i] + "/save/moderation.txt"))
                {
                    string[] lines = File.ReadAllLines(dirs[i] + "/save/moderation.txt");
                    try
                    {
                        Stats.moderationEnabled = bool.Parse(lines[0]);
                    }
                    catch(FormatException e)
                    {
                        Console.WriteLine("Bad format moderation !");
                        Stats.moderationEnabled = false;
                    }
                }

            }



            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,

                // Version Bêta     : MTMxODk2ODg3NTg3ODA1NTkzNg.Gsc7u6.K0qlw-PUGsupdoxbKyFWYs7E6YTeBO0Rl9HJ9Y
                // Version Stable   : MTMxNjg1NjY5NzI4NDcyMjczMA.GFdwui.lxUXaUgfYsyWEiFze5FXkpwW_L_CcjNCqBIjRM
                Token = Token.token,
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

            client.UseVoiceNext();

            await client.ConnectAsync(status: UserStatus.Online); // new DiscordActivity("Bang", ActivityType.Playing), 

            statusThread = new Thread(() => UpdateStatusLoop());
            statusThread.Start();

            restClient = new DiscordRestClient(discordConfig);

            Stats.sw.Start();

            await Task.Delay(-1);

        }


        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {

            /*if (Stats.userMessages.ContainsKey(e.Author.Id))
            {
                Stats.userMessages[e.Author.Id] += 1;

                try
                {
                    //Pass the filepath and filename to the StreamWriter Constructor
                    StreamWriter sw = new StreamWriter("Test.txt");
                    for (int i = 0; i < Stats.userMessages.Count; i++)
                        sw.WriteLine(Stats.userMessages.Keys.ElementAt(i) + "," + Stats.userMessages[Stats.userMessages.Keys.ElementAt(i)]);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }

            }

            else if (!Stats.userMessages.ContainsKey(e.Author.Id))
            {
                Stats.userMessages.Add(e.Author.Id, 1);

                try
                {
                    //Pass the filepath and filename to the StreamWriter Constructor
                    StreamWriter sw = new StreamWriter("Test.txt");
                    for(int i = 0; i < Stats.userMessages.Count; i++)
                        sw.WriteLine(Stats.userMessages.Keys.ElementAt(i) + "," + Stats.userMessages[Stats.userMessages.Keys.ElementAt(i)]);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }

            }*/




            if (e.Message.Content.ToLower().Contains("cd"))
                await e.Message.CreateReactionAsync(DiscordEmoji.FromName(sender, ":CD2000x:")); // DiscordAttachment

            //if (e.Message.Author.Username == "discretos") //  || e.Message.Author.Username == "mimisorrey"
                //await e.Message.CreateReactionAsync(DiscordEmoji.FromName(sender, ":cd:"));

            foreach (string i in Stats.saluts)
                if (e.Message.Content.ToLower().Contains(i))
                {
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromName(sender, ":wave:")); // DiscordAttachment
                    break;
                }

            //if (e.Author.Username != "ThunderBot")
            //await e.Message.RespondAsync("Server : " + e.Guild.Name + " / " + e.Channel.Name + " / " + e.Author.Mention + " / " + e.Author.AvatarUrl + " / " + e.Guild.IconUrl); // "Tu parles beaucoup !"

            if (Stats.moderationEnabled)
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

            DiscordChannel channel = e.Guild.GetDefaultChannel();
            if (Stats.logChannels.ContainsKey(e.Guild.Id))
                channel = e.Guild.GetChannel(Stats.logChannels[e.Guild.Id]);


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

                // = ,
                Title = "Message modifié par " + e.Guild.GetMemberAsync(e.Message.Author.Id).Result.Nickname + " dans " + e.Channel.Mention,
                Color = DiscordColor.SpringGreen,
                Description = "**Ancien : **" +
                "\n" + oldMess +
                "\n" +
                "\n**Nouveau : **" +
                "\n" + e.Message.Content,

                Timestamp = DateTime.Now,
            };


            if (e.Author.Username != "IngéBot")
                await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(message));
        }

        private static async Task MessageDeletedHandler(DiscordClient sender, MessageDeleteEventArgs e)
        {

            if (e.Guild == null) return;

            DiscordChannel channel = e.Guild.GetDefaultChannel();

            if (Stats.logChannels.ContainsKey(e.Guild.Id))
                channel = e.Guild.GetChannel(Stats.logChannels[e.Guild.Id]);

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
                if (e.Message.Author.Username != "IngéBot")
                    await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(deleted));
                //else
                    //await channel.SendMessageAsync(new DiscordMessageBuilder().WithContent("L'utilisateur " + "ERREUR" + " a supprimé un message.").AddEmbed(deleted));

            }

        }

        private static async Task GuildMemberAddedHandler(DiscordClient sender, GuildMemberAddEventArgs e)
        {

            DiscordChannel channel = e.Guild.GetDefaultChannel();

            if (Stats.welcomeChannels.ContainsKey(e.Guild.Id))
                channel = e.Guild.GetChannel(Stats.welcomeChannels[e.Guild.Id]);

            await channel.SendMessageAsync("Bienvenu sur le serveur !   Accueillez : " + e.Member.DisplayName);
        }

        private static async Task GuildMemberRemovedHandler(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {

            DiscordChannel channel = e.Guild.GetDefaultChannel();

            if (Stats.logChannels.ContainsKey(e.Guild.Id))
                channel = e.Guild.GetChannel(Stats.logChannels[e.Guild.Id]);

            await channel.SendMessageAsync(e.Member.DisplayName + " est partie...");
        }

        private static async Task RoleCreatedHandler(DiscordClient sender, GuildRoleCreateEventArgs e)
        {

            DiscordChannel channel = e.Guild.GetDefaultChannel();

            if (Stats.logChannels.ContainsKey(e.Guild.Id))
                channel = e.Guild.GetChannel(Stats.logChannels[e.Guild.Id]);

            await channel.SendMessageAsync("Le role " + e.Role.Mention + " a été créer par " + sender.CurrentUser.Username + ".");
        }



        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }



        private void UpdateStatusLoop()
        {

            while (true)
            {

                // Bonne année !
                if (DateTime.Now.Day == 01 && DateTime.Now.Month == 01 && DateTime.Now.Hour == 00 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00) //DateTime.Now == new DateTime(2024, 12, 25, 12, 0, 0)
                {

                    DiscordChannel channel = client.Guilds[1156894161761476648].GetDefaultChannel();

                    if (Stats.welcomeChannels.ContainsKey(client.Guilds[1156894161761476648].Id))
                        channel = client.Guilds[1156894161761476648].GetChannel(Stats.welcomeChannels[client.Guilds[1156894161761476648].Id]);

                    /*var mess = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "ℬ𝒪𝒩𝒩ℰ 𝒜𝒩𝒩ℰℰ 𝟚𝟘𝟚𝟝 !",
                    };*/

                    channel.SendMessageAsync("Bonne année 𝟚𝟘𝟚𝟝 !");

                }

                // Joyeux Noel !
                if (DateTime.Now.Day == 25 && DateTime.Now.Month == 12 && DateTime.Now.Hour == 09 && DateTime.Now.Minute == 00 && DateTime.Now.Second == 00) //DateTime.Now == new DateTime(2024, 12, 25, 12, 0, 0)
                {

                    DiscordChannel channel = client.Guilds[1156894161761476648].GetDefaultChannel();

                    if (Stats.welcomeChannels.ContainsKey(client.Guilds[1156894161761476648].Id))
                        channel = client.Guilds[1156894161761476648].GetChannel(Stats.welcomeChannels[client.Guilds[1156894161761476648].Id]);

                    var mess = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.Red,
                        Title = "𝒥𝑜𝓎𝑒𝓊𝓍 𝒩𝑜𝑒𝓁 *!*",
                    };

                    channel.SendMessageAsync(mess);

                }

                Thread.Sleep(1000);
            }

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

                DiscordChannel channel = e.Guild.GetDefaultChannel();

                if (Stats.logChannels.ContainsKey(e.Guild.Id))
                    channel = e.Guild.GetChannel(Stats.logChannels[e.Guild.Id]);

                await channel.SendMessageAsync(e.Interaction.User.Username + " n'a pas accepté les règles...");

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

            else if (e.Interaction.Data.CustomId == "addminecraft")
            {

                if (!e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1364176381440950314)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.GrantRoleAsync(e.Guild.GetRole(1364176381440950314));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @Minecraft vous à été conféré ! Tu as désormais accès aux salons concernant Minecraft !").AsEphemeral(welcomeEph));
                }
                else
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu as déjà le rôle pour Minecraft !").AsEphemeral(welcomeEph));

            }

            else if (e.Interaction.Data.CustomId == "remminecraft")
            {

                if (e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.Roles.Contains(e.Guild.GetRole(1364176381440950314)))
                {
                    await e.Guild.GetMemberAsync(e.Interaction.User.Id).Result.RevokeRoleAsync(e.Guild.GetRole(1364176381440950314));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le rôle @Minecraft vous à été enlevé ! Tu n'as désormais plus accès aux salons concernant Minecraft !").AsEphemeral(welcomeEph));
                }
                else
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'as pas le rôle pour Minecraft, je ne peux pas te l'enlever !").AsEphemeral(welcomeEph));

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
                    ImageUrl = url
                };


                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));
            }

        }

    }
}
