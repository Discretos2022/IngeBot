using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using IngeBot;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Channels;

namespace Bot.Modules
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
            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("ID :" + ctx.Channel.Id));

            Console.WriteLine(ctx.Channel.Id);
            //await ctx.Channel.SendMessageAsync(embed: message);
        }

        /*[SlashCommand("sum", "Verifier une simple addition.")]
        public async Task ParamTestAsync(InteractionContext ctx, [Option("num1", "le 1er numéro")] string param, [Option("num2", "le 2e numéro")] string param2)
        {
            int num = int.Parse(param) + int.Parse(param2);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Result : " + num));

        }*/

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

        /*[SlashCommand("devine", "J'essaye de deviner ta personnalité... (ça marche pas bien :) )")]
        public async Task DevineurAsync(InteractionContext ctx)
        {
            int num = new Random().Next(10 + 1);
            string per = "";

            switch (num)
            {
                case 0: per = "Sympa"; break;
                case 1: per = "Hipocrite"; break;
                case 2: per = "Abruti"; break;
                case 3: per = "Intello"; break;
                case 4: per = "Je sais pas"; break;
                case 5: per = "..."; break;

                case 6: per = "Puant"; break;
                case 7: per = "Gentil"; break;
                case 8: per = "J'ai pas le droit de le dire."; break;
                case 9: per = "Patate"; break;
                case 10: per = "Haaaaaaa !"; break;

            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Alors..."));

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Yellow,
                Title = "Vous êtes : " + per,
                Description = "Ceci n'est pas forcément vrai ! (Les personnalités sont choisit selon le Random())",
            };

            await ctx.Channel.SendMessageAsync(embed: message);
        }*/




        /*[SlashCommand("db", "Un convertisseur decimal à binaire !")]
        public async Task DecToBinAsync(InteractionContext ctx, [Option("decimal", "decimal à binariser")] string param)
        {
            int div = int.Parse(param);
            string result = "";
            int bit = 0;

            while (div >= 1)
            {
                result += (div % 2).ToString();
                div /= 2;
                bit += 1;
                if (bit == 4) { result += " "; bit = 0; }
            }

            result = result.Trim();
            result = ArrayReverseString(result);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(param + " = " + result.ToString()));
        }*/

        /*[SlashCommand("bd", "Un convertisseur binaire à decimal !")]
        public async Task BinToDecAsync(InteractionContext ctx, [Option("binaire", "binaire à décimaliser")] string param)
        {

            int result = 0;
            string num = ArrayReverseString(param);
            int puissance = 0;

            for (int i = 0; i < num.Length; i++)
            {
                char b = num.ToCharArray()[i];
                if (b == '1') { result += (int)Math.Pow(2, puissance); puissance += 1; }
                if (b == '0') { puissance += 1; }
            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(param + " = " + result));
        }*/


        /*[SlashCommand("setRole", "Une commande pour ajouter des permissions à un utilisateur.")]
        public async Task SetRoleAsync(InteractionContext ctx, [Option("user", "Utilisateur à modifier")] DiscordUser user, [Option("role", "role à effecter")] DiscordRole role)
        {

            if (ctx.User.Username == "discretos")
            {
                await ctx.Guild.GetMemberAsync(user.Id).Result.GrantRoleAsync(role);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("L'utilisateur " + user.Mention + " à maintenant le role : " + role.Mention));
            }
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Vous n'avez pas les droits pour éxécuter cette commande ! HA HA HA"));

        }*/

        /*[SlashCommand("revokerole", "Une commande pour enlever un role à un utilisateur.")]
        public async Task RevokeRoleAsync(InteractionContext ctx, [Option("user", "Utilisateur à modifier")] DiscordUser user, [Option("role", "role à enlever")] DiscordRole role)
        {

            if (ctx.User.Username == "discretos")
            {
                await ctx.Guild.GetMemberAsync(user.Id).Result.RevokeRoleAsync(role);
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("L'utilisateur " + user.Mention + " n'a plus le role : " + role.Mention));
            }
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Vous n'avez pas les droits pour éxécuter cette commande !"));

        }*/

        /*[SlashCommand("expulse", "Une commande pour expulser un utilisateur.")]
        public async Task ExpluseAsync(InteractionContext ctx, [Option("user", "Utilisateur à expulser")] DiscordUser user, [Option("raison", "Raidon de l'expulsion")] string reason)
        {

            if (ctx.User.Username == "discretos")
            {
                await ctx.Guild.GetMemberAsync(user.Id).Result.RemoveAsync();
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("L'utilisateur " + user.Mention + " a été expulsé par " + ctx.Member.Mention + " ! Raison : " + reason));
            }
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Vous n'avez pas les droits pour éxécuter cette commande !"));

        }*/


        /*[SlashCommand("img", "Une image...")]
        public async Task ImageAsync(InteractionContext ctx)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Je cherche dans la base de données..."));

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Aquamarine,
                Title = "Une image",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/18/ISO_C%2B%2B_Logo.svg/1822px-ISO_C%2B%2B_Logo.svg.png",
            };

            await ctx.Channel.SendMessageAsync(embed: message);

        }*/

        [SlashCommand("ip", "L'adresse IP du server qui héberge le bot. (𝕯𝖎𝖘𝖈𝖗𝖊𝖙𝖔𝖘)")]
        public async Task GetIPAsync(InteractionContext ctx)
        {

            if (ctx.User.Username != "discretos")
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

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
                String direction = "";
                HttpWebRequest request = HttpWebRequest.CreateHttp("http://checkip.dyndns.org/");
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


        /*[SlashCommand("savetext", "Une commande pour enregistrer du text.")]
        public async Task SaveText(InteractionContext ctx, [Option("Titre", "Titre du text")] string titre, [Option("Text", "Texte à enregistrer")] string text)
        {

            Directory.CreateDirectory("Data/" + ctx.User.Username);

            StreamWriter outputFile = new StreamWriter("Data/" + ctx.User.Username + "/" + titre + ".txt");
            outputFile.WriteLine(text);
            outputFile.Close();

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le text a été enregistrer !"));

        }*/

        /*[SlashCommand("enumtext", "Une commande pour énumérer ses texts.")]
        public async Task EnumText(InteractionContext ctx)
        {

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Je cherche dans la base de données..."));

            string file = "";
            if (!Directory.Exists("Data/" + ctx.User.Username))
                file = "Vous n'avez pas enregistrer de text !";
            else
            {
                var dirs = Directory.GetFiles("Data/" + ctx.User.Username);

                for (int i = 0; i < dirs.Length; i++)
                {
                    file += "- " + dirs[i].Substring(("Data/" + ctx.User.Username).Length + 1) + "\n";
                }
            }

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Aquamarine,
                Description = file,
            };

            await ctx.Channel.SendMessageAsync(embed: message);

        }

        [SlashCommand("gettext", "Une commande pour récupérer un text.")]
        public async Task GetText(InteractionContext ctx, [Option("Titre", "Titre du text à récupérer")] string titre)
        {

            Console.WriteLine("Guild : " + ctx.Guild);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Je cherche dans la base de données..."));

            string file;
            if (!Directory.Exists("Data/" + ctx.User.Username))
                file = "Vous n'avez pas enregistrer de text !";
            else if (!File.Exists("Data/" + ctx.User.Username + "\\" + titre + ".txt"))
                file = $"Ce text \"{titre}\" n'existe pas !";
            else
            {

                StreamReader r = new StreamReader("Data/" + ctx.User.Username + "/" + titre + ".txt");
                file = r.ReadToEnd();
                r.Close();

            }

            var message = new DiscordEmbedBuilder
            {
                Color = DiscordColor.SpringGreen,
                Description = file,
            };

            await ctx.Channel.SendMessageAsync(embed: message);

        }*/


        /*[SlashCommand("getfile", "Une commande pour recevoir un fichier .txt")]
        public async Task SendFileTest(InteractionContext ctx)
        {

            var fs = new FileStream("Files/test.exe", FileMode.Open, FileAccess.Read);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Siedel Software :").AddFile("test.exe", fs));

            fs.Close();

        }*/

        /*[SlashCommand("addevent", "Une commande pour ajouter des évenements. (admin)")]
        public async Task SendAddEvent(InteractionContext ctx, [Option("Titre", "Titre de l'évenement")] string titre, [Option("Date", "Date de l'évenement")] string date)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos")
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            if (!Directory.Exists("EventData"))
                Directory.CreateDirectory("EventData");

            File.CreateText("EventData/" + titre + ".txt");

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("L'évenement " + titre + " à été créé ! Date : " + date));
            return;

        }


        [SlashCommand("event", "Une commande pour afficher les events en cours.")]
        public async Task SendEvent(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (!Directory.Exists("EventData"))
            {
                Directory.CreateDirectory("EventData");
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Il n'y a pas d'évenement en cours !"));
                return;
            }
            else
            {
                var dirs = Directory.GetFiles("EventData/");

                if (dirs.Length == 0)
                {
                    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Il n'y a pas d'évenement en cours !"));
                    return;
                }
                else
                {
                    var chain = new DiscordEmbedBuilder
                    {
                        Color = DiscordColor.SpringGreen,
                        Description = dirs[0],
                    };

                    var mess = new DiscordInteractionResponseBuilder().WithContent("Voici les évenements : " + " \n" + "(Si vous voyez des erreurs ou des bugs, prévenez nous !)").AddEmbed(chain);

                    for (int i = 1; i < dirs.Length; i++)
                    {
                        var message = new DiscordEmbedBuilder
                        {
                            Color = DiscordColor.SpringGreen,
                            Description = dirs[i],
                        };
                        mess.AddEmbed(message);
                    }

                    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, mess);
                    return;
                }

            }

        }*/


        [SlashCommand("flyer", "Un flyer...")]
        public async Task ImageAsync(InteractionContext ctx)
        {

            if (ctx.Guild == null || ctx.Guild.Name != "IngéGamEZ")
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur le serveur IngéGamEZ !"));
                return;
            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Envoie en cours..."));
            var fs = new FileStream("Data/1156894161761476648/res/flyer945x540.png", FileMode.Open, FileAccess.Read);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("flyer945x540.png").AddFile(fs));

            fs.Close();
        }


        [SlashCommand("moderation", "Fonction de modération (true/false)")]
        public async Task EnableModerationAsync(InteractionContext ctx, [Option("activer", "true or false")] string response)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            if (response == "true" || response == "false")
            {
                Directory.CreateDirectory("Data/" + ctx.Guild.Id);
                string fileName = "Data/" + ctx.Guild.Id + "/save/moderation.txt";

                FileStream stream = File.OpenWrite(fileName);
                StreamWriter file = new StreamWriter(stream);

                file.WriteLine(response);
                file.Close();
            }

            if (response == "true")
            {
                Stats.moderationEnabled = true;
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le système de modération a été activé !"));
            }
            else if (response == "false")
            {
                Stats.moderationEnabled = false;
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le système de modération a été désactivé !"));
            }
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("La commande n'est pas valide !"));

        }


        [SlashCommand("setbotgame", "Une commande pour le jeu auquel le bot joue. (admin)")]
        public async Task SetBotGame(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            var modal = new DiscordInteractionResponseBuilder().WithTitle("Set Bot Game").WithCustomId("modal_bot_game").AddComponents(new TextInputComponent("Nom du jeu : ", "id", "Entre Le nom d'un jeu"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1).AddComponents(b2).AddComponents(b3));


        }


        [SlashCommand("welcome", "Une commande pour dire BIENVENU ! (admin)")]
        public async Task Welcome(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (!Stats.ContainsRole(ctx.Member, Stats.adminRole)) // ctx.User.Username != "discretos" && 
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "accept_rules", "Accepter", false);
            DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Danger, "no_accept_rules", "Refuser", false);

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


            DiscordChannel channel = ctx.Guild.GetDefaultChannel();
            if (Stats.logChannels.ContainsKey(ctx.Guild.Id))
                channel = ctx.Guild.GetChannel(Stats.logChannels[ctx.Guild.Id]);

            await channel.SendMessageAsync("L'utilisateur " + ctx.User.Username + " a utilisé la commande /welcome");




            DiscordButtonComponent e1 = new DiscordButtonComponent(ButtonStyle.Primary, "game_jam", "Game Jam", false);
            DiscordButtonComponent e2 = new DiscordButtonComponent(ButtonStyle.Primary, "jeudi_soir", "Jeudi Soir", false);
            DiscordButtonComponent e3 = new DiscordButtonComponent(ButtonStyle.Primary, "warhammer", "Warhammer 40K", false);
            DiscordButtonComponent e100 = new DiscordButtonComponent(ButtonStyle.Primary, "???", "???", true);

            var message2 = new DiscordEmbedBuilder
            {
                Title = "Pourquoi êtes-tu ici ?",
                Color = DiscordColor.Violet,
                Description = "Tu peux appuyer sur les boutons pour t'ajouter les rôles pour avoir accès aux salons en rapport avec les événements.",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Text = "Welcome message 2.0",
                },
            };

            await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(message).AddComponents(b1, b2));
            await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(message2).AddComponents(e2, e3, e100));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Welcome message 2.0").AsEphemeral(true));

        }


        [SlashCommand("grant", "Une commande pour ajouter un rôle ! (admin)")]
        public async Task GrantRole(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
            {
                Console.WriteLine(ctx.Guild.Roles.ElementAt(i).Value);
            }


            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            Stats.role = "";
            Stats.user = "";

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "valid", "Valider", false);
            DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Success, "seldate", "Date", false);

            /*if(Stats.mess_role_id != 0)
            {
                try
                {
                    await ctx.Interaction.Channel.GetMessageAsync(Stats.mess_role_id).Result.DeleteAsync();
                }
                catch(NotFoundException e) { }

            }*/

            var message = new DiscordInteractionResponseBuilder().WithTitle("Ajouter un rôle à un utilisateur").AddComponents(new DiscordRoleSelectComponent("roles", "Roles")).AddComponents(new DiscordUserSelectComponent("user", "Utilisateur")).AddComponents(b1, b2);     //  .AddComponents(new DiscordChannelSelectComponent("123", "1234"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

            Stats.mess_role_id = ctx.GetOriginalResponseAsync().Result.Id;

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1).AddComponents(b2).AddComponents(b3));

        }

        [SlashCommand("revoke", "Une commande pour ajouter un rôle ! (admin)")]
        public async Task RevokeRole(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
            {
                Console.WriteLine(ctx.Guild.Roles.ElementAt(i).Value);
            }


            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            Stats.role = "";
            Stats.user = "";

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "valid_revoke", "Valider", false);

            /*if(Stats.mess_role_id != 0)
            {
                try
                {
                    await ctx.Interaction.Channel.GetMessageAsync(Stats.mess_role_id).Result.DeleteAsync();
                }
                catch(NotFoundException e) { }

            }*/

            var message = new DiscordInteractionResponseBuilder().WithTitle("Supprimer un rôle à un utilisateur").AddComponents(new DiscordRoleSelectComponent("roles", "Roles")).AddComponents(new DiscordUserSelectComponent("user", "Utilisateur")).AddComponents(b1);     //  .AddComponents(new DiscordChannelSelectComponent("123", "1234"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

            Stats.mess_role_id = ctx.GetOriginalResponseAsync().Result.Id;

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1).AddComponents(b2).AddComponents(b3));

        }


        [SlashCommand("setchannellog", "Une commande pour définir un salon comme log ! (admin)")]
        public async Task SetChannelLog(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            if (Stats.logChannels.ContainsKey(ctx.Guild.Id))
                Stats.logChannels.Remove(ctx.Guild.Id);

            Stats.logChannels.Add(ctx.Guild.Id, ctx.Channel.Id);


            var message = new DiscordInteractionResponseBuilder().WithContent("Le salon pour les logs viens d'être défini dans : " + ctx.Channel.Mention);
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);


            Directory.CreateDirectory("Data/" + ctx.Guild.Id + "/save");
            string fileName = "Data/" + ctx.Guild.Id + "/save/logchannel.txt";

            FileStream stream = File.OpenWrite(fileName);
            StreamWriter file = new StreamWriter(stream);

            file.WriteLine(ctx.Channel.Id);
            file.Close();


            DiscordChannel channel = ctx.Guild.GetDefaultChannel();
            if (Stats.logChannels.ContainsKey(ctx.Guild.Id))
                channel = ctx.Guild.GetChannel(Stats.logChannels[ctx.Guild.Id]);

            await channel.SendMessageAsync("Le salon de log a été défini dans " + channel.Name + " par " + ctx.User.Username + ".");

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1).AddComponents(b2).AddComponents(b3));

        }

        [SlashCommand("setwelcomechannel", "Une commande pour définir un salon comme salon de bienvenu ! (admin)")]
        public async Task SetWelcomeChannel(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            if (Stats.welcomeChannels.ContainsKey(ctx.Guild.Id))
                Stats.welcomeChannels.Remove(ctx.Guild.Id);

            Stats.welcomeChannels.Add(ctx.Guild.Id, ctx.Channel.Id);

            var message = new DiscordInteractionResponseBuilder().WithContent("Le salon pour les bienvenus viens d'être défini dans : " + ctx.Channel.Mention);
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);


            Directory.CreateDirectory("Data/" + ctx.Guild.Id + "/save");
            string fileName = "Data/" + ctx.Guild.Id + "/save/welcomechannel.txt";

            FileStream stream = File.OpenWrite(fileName);
            StreamWriter file = new StreamWriter(stream);

            file.WriteLine(ctx.Channel.Id);
            file.Close();

            DiscordChannel channel = ctx.Guild.GetDefaultChannel();
            if (Stats.logChannels.ContainsKey(ctx.Guild.Id))
                channel = ctx.Guild.GetChannel(Stats.logChannels[ctx.Guild.Id]);

            await channel.SendMessageAsync("Le salon de bienvenu a été défini dans " + channel.Name + " par " + ctx.User.Username + ".");

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1).AddComponents(b2).AddComponents(b3));

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
                    grid[x, y] = (-grid[x, y]) - 1;
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




        [SlashCommand("saveinfo", "Une commande pour afficher les données sauvegardées. (admin)")]
        public async Task GetSaveInfo(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }


            if (ctx.User.Username != "discretos")
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Recherche des données... (attention, tu n'es pas Discretos, tu n'as pas accès à la base de données)"));
            else
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Recherche des données..."));


            var m = new DiscordEmbedBuilder
            {
                Title = "Données sauvegardées par IngéBot",
                Color = DiscordColor.Gray,
                Description = ""
            };

            m.Description += "**Channel : **";
            m.Description += "\n";
            m.Description += "Log Channel : " + ctx.Guild.GetChannel(Stats.logChannels[ctx.Guild.Id]).Mention;
            m.Description += "\n";
            m.Description += "Welcome Channel : " + ctx.Guild.GetChannel(Stats.welcomeChannels[ctx.Guild.Id]).Mention;
            m.Description += "\n\n";
            m.Description += "**Modération : **";
            m.Description += "\n";
            m.Description += "Modération : " + Stats.moderationEnabled;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(m));


            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1).AddComponents(b2).AddComponents(b3));


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


        [SlashCommand("restart", "Une commande pour restart le bot ! (admin)")]
        public async Task Restart(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Restarting...").AsEphemeral(false));
        }



        [SlashCommand("help", "Une commande pour afficher toutes les commandes ! (admin)")]
        public async Task Help(InteractionContext ctx)
        {

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
            else if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole)) // && !Stats.ContainsRole(ctx.Member, Stats.adminRole)
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
            else
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


            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message));
        }


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

            DiscordChannel log = ctx.Guild.GetDefaultChannel();
            if (Stats.logChannels.ContainsKey(ctx.Guild.Id))
                log = ctx.Guild.GetChannel(Stats.logChannels[ctx.Guild.Id]);
            await log.SendMessageAsync(ctx.Member + " a créé un ticket : " + name);

        }



        [SlashCommand("runtime", "Donne le temps pendant lequel le bot ne s'est pas interrompu !")]
        public async Task GetRunTime(InteractionContext ctx)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Temps : " + Stats.sw.Elapsed.Days + "j " + Stats.sw.Elapsed.Hours + "h " + Stats.sw.Elapsed.Minutes + "min " + Stats.sw.Elapsed.Seconds + "sec"));
        }


        [SlashCommand("pendu", "Créer un pendu !")]
        public async Task Pendu(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            string[] lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "/Data/Bot/word.txt");
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

            string[] existantWords = File.ReadAllLines(Directory.GetCurrentDirectory() + "/Data/Bot/word.txt");

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
            File.AppendAllLines(Directory.GetCurrentDirectory() + "/Data/Bot/word.txt", words);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le mot `" + newWord + "` a bien été ajouté !"));

        }


        /*[SlashCommand("deleteallmess", "Supprime tout les messages de ce salon ! (admin)")]
        public async Task DeleteMessage(InteractionContext ctx)
        {

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande n'est pas encore au point... "));

            //ulong id = ctx.Channel.LastMessageId.GetValueOrDefault();

            //await ctx.Channel.DeleteMessagesAsync(ctx.Channel.GetMessagesBeforeAsync(id).Result);

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Test ?"));

        }*/


        [SlashCommand("createevent", "Créer le message d'évenement ! (admin)")]
        public async Task EventMess(InteractionContext ctx) // , [Option("Title", "Titre de l'évenement")] string title, [Option("Info", "Info de l'évenement")] string info, [Option("URL", "URL de l'image")] string url
        {

            var title = new TextInputComponent("Titre : ", "event_title", required: true, style: TextInputStyle.Short);
            var info = new TextInputComponent("Info : ", "event_info", required: true, style: TextInputStyle.Paragraph);
            var url = new TextInputComponent("URL de l'image : ", "event_url", required: true, style: TextInputStyle.Short);


            var modal = new DiscordInteractionResponseBuilder()
                .WithTitle("Annonce de jeu")
                .WithCustomId("event_generator")
                .AddComponents(title)
                .AddComponents(info)
                .AddComponents(url);

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modal);

            //ulong id = ctx.Channel.LastMessageId.GetValueOrDefault();

            //await ctx.Channel.DeleteMessagesAsync(ctx.Channel.GetMessagesBeforeAsync(id).Result);

            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Test ?"));

        }


        [SlashCommand("website", "Donne l'url du site !")]
        public async Task GetURL(InteractionContext ctx)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("https://ingegamez.isc-vs.ch/"));
        }





        [SlashCommand("minecraft", "Une commande pour afficher le message pour Minecraft ! (admin)")]
        public async Task Minecraft(InteractionContext ctx)
        {

            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "addminecraft", "Prendre le rôle", false);
            DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Success, "remminecraft", "Retirer le rôle", false);


            var message = new DiscordInteractionResponseBuilder().WithTitle("Vouz pouvez vous ajouter ou supprimer le rôle @Minecraft avec ces boutons").AddComponents(b1, b2);     //  .AddComponents(new DiscordChannelSelectComponent("123", "1234"));
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, message);

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
                "\n" +       $"Players : {str[1].Substring(0, str[1].Length - 1)} / {str[2]}" +
                "```",
            };

            await ctx.Interaction.DeleteOriginalResponseAsync();
            await ctx.Interaction.Channel.SendMessageAsync(embed: message);

        }







        /*[SlashCommand("connection", "???")]
        public async Task IsConnected(InteractionContext ctx, [Option("user", "???")] DiscordUser user)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(user.Username + " est : " + user.Presence.Status.ToString()));
        }*/


        /*[SlashCommand("readexcel", "Une commande ! (admin)")]
        public async Task ChannelLevel(InteractionContext ctx) // , [Option("Title", "Titre de l'évenement")] string title, [Option("Info", "Info de l'évenement")] string info, [Option("URL", "URL de l'image")] string url
        {

            if (ctx.User.Username != "discretos")
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            string m = "";

            if (File.Exists("Data/1156894161761476648/res/test.csv"))
            {
                string[] lines = File.ReadAllLines("Data/1156894161761476648/res/test.csv");

                for (int i = 0; i < lines.Length; i++)
                {
                    Console.WriteLine(lines[i]);
                    m += lines[i] + "\n";
                }

                

            }

            

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(m));

        }*/


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



        /*[SlashCommand("welcome", "Une commande pour afficher les règles du serveur ! (admin)")]
        public async Task Rules(InteractionContext ctx)
        {

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }

            DiscordButtonComponent b1 = new DiscordButtonComponent(ButtonStyle.Success, "accept_rules", "Accepter", false);
            DiscordButtonComponent b2 = new DiscordButtonComponent(ButtonStyle.Danger, "no_accept_rules", "Refuser", false);

            var message = new DiscordEmbedBuilder
            {
                Title = "Bienvenu sur le serveur Discord d'IngéGamEZ !",
                Color = DiscordColor.Violet,
                Description = "**Veuillez lire et accepter les règles du serveur**" +
                " - Ne pas insulter " +
                " - Être respectueux les uns envers les autres " +
                " - J'ai plus d'idée... ",
            };

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(message).AddComponents(b1, b2));
            //await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pourquoi êtes-vous ici ?").AddComponents(b1, b2, b3));

        }*/



        /*[SlashCommand("roles", "(admin)")]
        public async Task Role(InteractionContext ctx)
        {
            if (ctx.Guild == null)
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Cette commande doit être éxécuté sur un serveur !"));
                return;
            }

            if (ctx.User.Username != "discretos" && !Stats.ContainsRole(ctx.Member, Stats.adminRole))
            {
                await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Tu n'es pas autorisé à utiliser cette commande !"));
                return;
            }


            string message = "";

            for (int i = 0; i < ctx.Guild.Roles.Count; i++)
            {
                message += ctx.Guild.Roles.ElementAt(i).Value + " \n";
            }

            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message));

        }*/

















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















    }
}
