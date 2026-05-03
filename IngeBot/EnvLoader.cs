using System;
using System.Runtime.InteropServices;

namespace IngeBot
{


    public struct Configuration
    {
        public Configuration() { }


        public string DiscordToken = "";
        public string DataPath = "";
        public string PythonPath = "";
        public string MCStatucPath = "";

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

            string configPath = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
#if DEBUG
                configPath = "/var/opt/IngeBot-Beta/ingebot.config";
#else
                configPath = "/var/opt/IngeBot/ingebot.config";
#endif
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                configPath = "../../config/ingebot.config"; // Dev
            }


            if (!File.Exists(configPath))
            {
                Console.WriteLine("404 Error : Base Config File Not Found !");

                Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);

                var content =
@"DISCORD_TOKEN=
DATA_PATH=
PYTHON_PATH=
MCSTATUS_PATH=

DATABASE_HOST=
DATABASE_PORT=
DATABASE_USERNAME=
DATABASE_PASSWORD=
DATABASE_NAME=";

                File.WriteAllText(configPath, content);

                Console.WriteLine($"Config File Was Generated In {configPath} !");
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

                    case "DISCORD_TOKEN": configuration.DiscordToken = value; continue;
                    case "DATA_PATH": configuration.DataPath = value; continue;
                    case "PYTHON_PATH": configuration.PythonPath = value; continue;
                    case "MCSTATUS_PATH": configuration.MCStatucPath = value; continue;

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
