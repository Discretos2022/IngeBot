using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IngeBot
{
    public class MessageTimeManager
    {

        public static List<Task> messageTime = new List<Task>();

        public static async Task InitMessageTimeFromFolder(DiscordClient client)
        {


            string[] serverDirs = Directory.GetDirectories("Data/");

            for (int i = 0; i < serverDirs.Length; i++)
            {

                if (Directory.Exists(serverDirs[i] + "/message_time"))
                {

                    string[] messageFiles = Directory.GetFiles(serverDirs[i] + "/message_time/");

                    foreach (string file in messageFiles)
                    {

                        Console.WriteLine(file);

                        string[] lines = File.ReadAllLines(file);

                        ulong guildId = ulong.Parse(serverDirs[i].Split(new char[] { '\\', '/' })[1]);
                        ulong channelId = ulong.Parse(lines[0]);

                        string name = file.Split("/").Last().Split(".")[0];
                        string date = lines[1];

                        string text = "";
                        for (int l = 2; l < lines.Length; l++)
                        {
                            text += lines[l] + "\n";
                        }

                        MessageTimeStruct mts = new MessageTimeStruct();
                        mts.Name = name;
                        mts.Text = text;
                        mts.Date = date;
                        mts.ChannelId = channelId.ToString();
                        mts.GuildId = guildId.ToString();
                        mts.Path = file;
                        
                        _ = MessageLoop(client, mts);

                    }


                    //string[] lines = File.ReadAllLines(dirs[i] + "/save/logchannel.txt");
                    //Stats.logChannels.Add(ulong.Parse(dirs[i].Split(new char[] { '\\', '/' })[1]), ulong.Parse(lines[0]));
                }



            }

        }

        public static async Task MessageLoop(DiscordClient client, MessageTimeStruct mts)
        {

            DiscordGuild guild = client.Guilds[ulong.Parse(mts.GuildId)];
            if (guild == null) return;

            DiscordChannel channel = guild.GetChannel(ulong.Parse(mts.ChannelId));
            if (channel == null) return;

            string[] part = mts.Date.Split(" ");

            int years = int.Parse(part[0].Split("/")[0]);
            int month = int.Parse(part[0].Split("/")[1]);
            int day = int.Parse(part[0].Split("/")[2]);

            int hour = int.Parse(part[1].Split(":")[0]);
            int minute = int.Parse(part[1].Split(":")[1]);

            DateTime dateTime = new DateTime(years, month, day, hour, minute, 0);

            TimeSpan time = TimeSpan.FromTicks(dateTime.Ticks - DateTime.Now.Ticks);

            if (time.Ticks < 0) time = TimeSpan.FromTicks(1);

            await Task.Delay(time);

            await channel.SendMessageAsync(mts.Text);

            try
            {
                File.Delete(mts.Path);
            }
            catch(IOException e)
            {

            }

        }


        public static Result CreateNewMessageTime(DiscordClient client, ulong guildId, ulong channelId, string name, string date, string message)
        {

            if (name.Contains("/") || name.Contains("\\"))
                return Result.BadFormatName;


            if (File.Exists("Data/" + guildId + "/message_time/" + name + ".txt"))
                return Result.AlreadyExist;

            if (!DateTime.TryParseExact(date, "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                return Result.BadFormatDate;

            try
            {

                StreamWriter stream = File.CreateText("Data/" + guildId + "/message_time/" + name + ".txt");

                string t = channelId + "\n" + date + "\n" + message;

                stream.WriteLine(t);
                stream.Flush();
                stream.Close();

                MessageTimeStruct mts = new MessageTimeStruct();
                mts.Name = name;
                mts.Text = message;
                mts.Date = date;
                mts.ChannelId = channelId.ToString();
                mts.GuildId = guildId.ToString();
                mts.Path = "Data/" + guildId + "/message_time/" + name + ".txt";

                _ = MessageLoop(client, mts);
                return Result.Success;

            }
            catch (ArgumentException e)
            {
                return Result.BadFormatName;
            }

            

        }

        public static List<MessageTimeStruct> GetMessageTimeList(ulong guildId)
        {

            List<MessageTimeStruct> result = new List<MessageTimeStruct>();

            string serverDir = "Data/" + guildId;

            if (Directory.Exists(serverDir + "/message_time"))
            {

                string[] messageFiles = Directory.GetFiles(serverDir + "/message_time/");

                foreach (string file in messageFiles)
                {

                    string[] lines = File.ReadAllLines(file);

                    ulong channelId = ulong.Parse(lines[0]);
                    string date = lines[1];

                    string text = "";
                    for (int l = 2; l < lines.Length; l++)
                    {
                        text += lines[l] + "\n";
                    }

                    string name = file.Split("/").Last().Split(".")[0];


                    MessageTimeStruct mts = new MessageTimeStruct();
                    mts.Name = name;
                    mts.Text = text;
                    mts.Date = date;
                    mts.ChannelId = channelId.ToString();
                    mts.GuildId = guildId.ToString();
                    mts.Path = file;


                    result.Add(mts);
                }

            }

            return result;

        }


        public static MessageTimeStruct GetMessageTime(string name, ulong guildId)
        {

            string serverDir = "Data/" + guildId;

            if (Directory.Exists(serverDir + "/message_time"))
            {

                string messageFile = serverDir + "/message_time/" + name + ".txt";


                if (!File.Exists(messageFile))
                {
                    return new MessageTimeStruct();
                }

                string[] lines = File.ReadAllLines(messageFile);

                ulong channelId = ulong.Parse(lines[0]);
                string date = lines[1];

                string text = "";
                for (int l = 2; l < lines.Length; l++)
                {
                    text += lines[l] + "\n";
                }

                MessageTimeStruct mts = new MessageTimeStruct();
                mts.Name = name;
                mts.Text = text;
                mts.Date = date;
                mts.ChannelId = channelId.ToString();
                mts.GuildId = guildId.ToString();
                mts.Path = messageFile;

                return mts;

            }
            
            return new MessageTimeStruct();

        }

        public static Result DeleteMessageTime(string name, ulong guildId)
        {

            string serverDir = "Data/" + guildId;

            if (Directory.Exists(serverDir + "/message_time"))
            {

                string messageFile = serverDir + "/message_time/" + name + ".txt";

                if (!File.Exists(messageFile))
                {
                    return Result.NotExist;
                }

                string[] lines = File.ReadAllLines(messageFile);

                /// TODO : Read first line -> if it's built-in message, cannot delete.

                
                try
                {
                    File.Delete(messageFile);
                    return Result.Success;
                }
                catch (Exception e)
                {
                    return Result.Error;
                }

            }

            return Result.NotExist;

        }


        public enum Result
        {
            Success,
            Error,
            AlreadyExist,
            BadFormatName,
            BadFormatDate,
            NotExist,
            CannotDelete,
        }

    }
}
