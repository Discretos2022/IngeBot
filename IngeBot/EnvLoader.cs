using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot
{


    public struct Configuration
    {
        public Configuration() { }


        public string DataPath = "";
        public string DiscordToken = "";

        public string DataBaseHost = "";
        public string DataBasePort = "";
        public string DataBaseUsername = "";
        public string DataBasePassword = "";
        public string DataBaseName = "";

    }


    public class EnvLoader
    {

        public static Configuration LoadEnv()
        {

            string configPath = "../config/ingebot.config";

            if (!File.Exists(configPath))
            {
                Console.WriteLine("404 Error : Base Config File Not Found !");
                Environment.Exit(1);
            }

            string[] lines = File.ReadAllLines(configPath);

            Configuration configuration = new Configuration();

            foreach (string line in lines) 
            {

                if (line.StartsWith("//") || line.StartsWith("#") || line.Length == 0) continue;

                string[] param = line.Split("=");
                string key = param[0];
                string value = param[1];
                
                if (value == "")
                {
                    Console.WriteLine($"Error : Config File is Not Correct -> {key} Has No Value !");
                    Environment.Exit(1);
                }

                switch (key)
                {

                    case "DATA_PATH": configuration.DataPath = value; continue;
                    case "DISCORD_TOKEN": configuration.DiscordToken = value; continue;

                    case "DATABASE_HOST": configuration.DataBaseHost = value; continue;
                    case "DATABASE_PORT": configuration.DataBasePort = value; continue;
                    case "DATABASE_USERNAME": configuration.DataBaseUsername = value; continue;
                    case "DATABASE_PASSWORD": configuration.DataBasePassword = value; continue;
                    case "DATABASE_NAME": configuration.DataBaseName = value; continue;
                }

            }

            return configuration;

        }

    }
}
