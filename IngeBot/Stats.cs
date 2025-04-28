using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*

Role 1168496848391127134; YAGPDB.xyz
Role 1156894465924026438; ingénieurs
Role 1156894161761476648; @everyone
Role 1319636688691007488; IngéBot
Role 1168864525542490132; bots
Role 1156942741389987870; jeux-vidéo
Role 1156946871571456040; jeux société
Role 1169423512147070999; Mudae
Role 1319643473137242125; IngeBot_Beta
Role 1169422452628140033; invité hors HES
Role 1169423334862237771; DraftBot
Role 1168494505729740834; Pancake
Role 1160611550063767683; Game Jam Participant
Role 1217909889167265802; Warhammer~master
Role 1167037183861989428; Thursday gamEZ

Guild 1169326745384648714; Dimension Discretos
Guild 1156894161761476648; IngéGamEZ

 */

namespace IngeBot
{
    public static class Stats
    {

        public static bool moderationEnabled = false;

        public static string botGame = "";

        public static string adminRole = "ingénieurs";

        public static ulong mess_role_id = 0;
        public static string role = "";
        public static string user = "";
        public static string date = "";

        public static string version = "1.1.4.3";

        public static Stopwatch sw = new Stopwatch();


        public static Dictionary<ulong, ulong> logChannels = new Dictionary<ulong, ulong>();
        public static Dictionary<ulong, ulong> welcomeChannels = new Dictionary<ulong, ulong>();

        //public static Dictionary<ulong, int> userMessages = new Dictionary<ulong, int>();


        public static bool ContainsRole(DiscordMember member, string role)
        {

            for (int i = 0; i < member.Roles.Count(); i++)
            {
                if (member.Roles.ElementAt(i).Name == role)
                {
                    return true;
                }
            }

            return false;

        }


        public static List<string> blague = new List<string>
        {
            "Je suis un poisson et je suis pané. Qui suis-je ? # || Personne, puisque je ne suis pané... ||",
            "Qu'est ce qu'un chat tout terrain ? # || Un cat-cat (4x4) ||",
            "3 # || 3 ||",
            "4 # || 4 ||",
            "5 # || 5 ||",
        };

        public static List<string> saluts = new List<string> { "salut", "hi ", "bonjour", "slt", "hello", "hallo", "allo", "👋", "ola" };



        public static string SlashCommandBase = "**La liste des commandes slash :**" +
                " \n /help : Afficher toutes les commandes et leurs actions" +
                " \n /hello : Vérifie si le bot fonctionne" +
                " \n /demineur : Génere une grille de démineur" +
                " \n /blague : Créer une blague aléatoire" +
                " \n /send : Permet d'envoyer un message incognito" +
                " \n /ticket : Créer un salon avec toi et les staffs (ne fonctionne pas)" +
                " \n /flyer : Affiche le flyer d'un évènement en cours" +
                " \n /info : Affiche quelques infos sur le bot" +
                " \n /runtime : Affiche le temps pendant lequel le bot ne s'est pas arrêté" +
                " \n /pendu : Jouer au pendu" +
                " \n /addword : Ajouter un mot pour le pendu" +
                " \n /ticket : Créer un salon avec toi et les staffs";

        public static string SlashCommandAdmin = "**La liste des commandes slash admin :**" +
                " \n /welcome : Message welcome" +
                " \n /setchannellog : Défini le salon pour les logs" +
                " \n /setwelcomechannel : Défini le salon pour les messages de bienvenu" +
                " \n /grant : Ajoute un rôle à quelqu'un (ne fonctione pas avec un temps)" +
                " \n /revoke : Enlève un rôle à quelqu'un" +
                " \n /setbotgame : Défini le jeu auquel le bot joue" +
                " \n /moderation : Active/Désactive la modération auto" +
                " \n /saveinfo : Affiche les données sauvegardées par le bot" +
                " \n /createevent : Génere le message d'évènement avec une image et du texte" +
                " \n /ip : Affiche l'adresse ip publique du serveur du bot";

        public static string NativeCommandBasic = "**La liste des commandes natives :**" +
                " \n /hellotest : Test natif";




        public static List<string> words = new List<string>();

        public static Dictionary<string, PenduData> penduDict = new Dictionary<string, PenduData>();

        public static string GetPenduKey(ulong user, ulong channel)
        {
            return user.ToString() + channel.ToString();
        }

        public class PenduData
        {

            public ulong user;
            public ulong channel;
            public ulong initialMessage;
            public List<char> used;
            public string hidedWord;
            public string word;

            public PenduData(string hidedWord, ulong user, ulong channel) 
            {
                this.user = user;
                this.channel = channel;

                this.hidedWord = hidedWord;
                word = "";
                InitWord();

                used = new List<char>();

            }

            public void SetInitialMess(ulong initialMessage)
            {
                this.initialMessage = initialMessage;
            }

            public void InitWord()
            {
                word = "";

                for (int i = 0; i < hidedWord.Length; i++)
                {
                    word += "_";
                }

            }


            public void UpdateWord(char letter)
            {
                string newWord = "";

                for (int i = 0; i < word.Length; i++)
                {

                    if (hidedWord[i].ToString().ToLower() == letter.ToString().ToLower())
                    {
                        newWord += hidedWord[i];
                    }
                    else
                    {
                        newWord += word[i];
                    }

                }

                Console.WriteLine(letter.ToString());

                if(word == newWord)
                    if(!used.Contains(letter))
                        used.Add(letter);

                word = newWord;

            }

            public string GetPenduGFX()
            {
                string result = "";

                result += "```";


                if (used.Count == 0)
                {

                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 1)
                {

                    result += "          " + "\n";
                    result += "          " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 2)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 3)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 4)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/       " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 5)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 6)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |    o   " + "\n";
                    result += " |        " + "\n";
                    result += " |        " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 7)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |    o   " + "\n";
                    result += " |    █   " + "\n";
                    result += " |        " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 8)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |    o   " + "\n";
                    result += " |   ‾█‾  " + "\n";
                    result += " |        " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 9)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |    o   " + "\n";
                    result += " |   ‾█‾  " + "\n";
                    result += " |     \\  " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }
                else if (used.Count == 10)
                {

                    result += "          " + "\n";
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |    o   " + "\n";
                    result += " |   ‾█‾  " + "\n";
                    result += " |   / \\  " + "\n";
                    result += "/|\\       " + "\n";
                    result += "‾‾‾‾‾‾‾‾‾‾" + "\n";

                }

                result += "```";

                return result;

            }

        }



    }
}
