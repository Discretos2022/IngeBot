using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IngeBot.DelayerEngine
{
    public class ChronoSystem
    {

        public static DiscordClient client;

        public static Dictionary<string, ChronoInstructionBase> instructions;

        public static void Initialize(DiscordClient _client)
        {
            client = _client;
            instructions = new Dictionary<string, ChronoInstructionBase>();

            string[] serverDirs = Directory.GetDirectories("Data/");
            for (int i = 0; i < serverDirs.Length; i++)
            {

                if (Directory.Exists(serverDirs[i] + "/chrono_instruction"))
                {

                    string[] messageFiles = Directory.GetFiles(serverDirs[i] + "/chrono_instruction/");

                    foreach (string file in messageFiles)
                    {

                        string[] lines = File.ReadAllLines(file);

                        string fileName = file.Split("/").Last();

                        InstructionType instructionType = (InstructionType)int.Parse(fileName.Split("_")[0]);
                        string name = fileName.Split(".")[0].Substring(2);
                        string guildId = serverDirs[i].Split(new char[] { '\\', '/' })[1];

                        ChronoInstructionBase instruction = ChronoInstructionBase.ReadFromString(lines, name, guildId, instructionType);

                        if (instruction != null)
                        {
                            StartLoop(instruction);
                        }

                    }

                }

            }

            Console.WriteLine("Chrono System 1.0 started !");

        }


        public static void StartLoop(ChronoInstructionBase instruction)
        {
            instruction.cts = new CancellationTokenSource();
            instructions.Add(instruction.Name, instruction);
            _ = MessageLoop(instruction, instruction.cts.Token);
        }

        private static async Task MessageLoop(ChronoInstructionBase instuction, CancellationToken cts)
        {

            DiscordGuild guild = client.Guilds[ulong.Parse(instuction.GuildId)];
            if (guild == null) return;

            DiscordChannel channel = guild.GetChannel(ulong.Parse(instuction.ChannelId));
            if (channel == null) return;

            TimeSpan time = GetTimeSpan(instuction.Date);

            if (time.Ticks <= 0) time = TimeSpan.FromTicks(1);

            try
            {
                await Task.Delay(time, cts);
            }
            catch (TaskCanceledException e)
            {
                return;
            }

            await instuction.Execute(channel);

        }

        public static void KillMessageLoop(string name)
        {
            instructions.TryGetValue(name, out var instruction);
            if (instruction == null) return;

            instruction.cts.Cancel();
            instructions.Remove(name);
        }

        public static void KillInstruction(string name)
        {
            instructions.Remove(name);
        }

        public static void RelaunchMessageLoop(ChronoInstructionBase instruction)
        {
            instruction.cts = new CancellationTokenSource();
            instructions.Add(instruction.Name, instruction);
            _ = MessageLoop(instruction, instruction.cts.Token);
        }


        public static TimeSpan GetTimeSpan(string date)
        {
            string[] part = date.Split(" ");

            int years = int.Parse(part[0].Split("/")[0]);
            int month = int.Parse(part[0].Split("/")[1]);
            int day = int.Parse(part[0].Split("/")[2]);

            int hour = int.Parse(part[1].Split(":")[0]);
            int minute = int.Parse(part[1].Split(":")[1]);

            DateTime dateTime = new DateTime(years, month, day, hour, minute, 0);
            TimeSpan time = TimeSpan.FromTicks(dateTime.Ticks - DateTime.Now.Ticks);

            return time;
        }


        public static async Task RegisterChronoInstruction(ChronoInstructionBase instruction, DiscordInteraction ctx)
        {

            if (instruction.Name.Contains("/") || instruction.Name.Contains("\\") || instruction.Name.Contains("_"))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Le nom de doit pas contenir de \"/\", \"\\\\\", \"_\" !"));
                return;
            }

            await instruction.Register(ctx);

        }


        public static async Task UnregisterChronoInstruction(string name, DiscordInteraction ctx)
        {

            if (instructions.TryGetValue(name, out var instruction))
                await instruction.Unregister(ctx);
            else
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Une erreur est survenu !"));

        }


        public enum InstructionType
        {
            MessageTime = 0,
            MessageBirthday = 1,
            RoleTime = 2,
        }

    }
}
