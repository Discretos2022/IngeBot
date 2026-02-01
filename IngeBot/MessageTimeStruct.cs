using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngeBot
{
    public struct MessageTimeStruct
    {

        public string Name;
        public string Text;
        public string Date;
        public string Path;

        public string GuildId;
        public string ChannelId;


        public MessageTimeStruct(string name, string text, string date, string path, string guildId, string channelId)
        {
            Name = name;
            Text = text;
            Date = date;
            Path = path;
            GuildId = guildId;
            ChannelId = channelId;
        }

    }
}
