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

        public static string version = "1.1.1.2";

        public static Stopwatch sw = new Stopwatch();


        public static Dictionary<ulong, ulong> logChannels = new Dictionary<ulong, ulong>();
        public static Dictionary<ulong, ulong> welcomeChannels = new Dictionary<ulong, ulong>();


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

            public PenduData(ulong user, ulong channel) 
            {
                this.user = user;
                this.channel = channel;

                hidedWord = "Discretos";
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
                    result += " ______   " + "\n";
                    result += " |/   |   " + "\n";
                    result += " |    o   " + "\n";
                    result += " |   -0-  " + "\n";
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
