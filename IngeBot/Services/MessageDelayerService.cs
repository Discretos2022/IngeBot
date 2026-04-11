using DSharpPlus;
using IngeBot.Models;
using IngeBot.Models.System;
using System;

namespace IngeBot.Services
{
    public class MessageDelayerService
    {

        private static Dictionary<int, (DelayedMessage message, CancellationTokenSource cts)> delayedMessagesList = new Dictionary<int, (DelayedMessage message, CancellationTokenSource cts)>();


        public static async Task Run(DiscordClient client)
        {

            while (true)
            {


                try
                {


                    // DelayedMessage message = new DelayedMessage(1010, 1010, 1010, "name", "Ceci est un text", DateTime.Now.AddMinutes(10), false);
                    // message.Save();

                    DelayedMessage[] delayedMessages = DelayedMessage.FindNext();

                    foreach (var m in delayedMessages)
                    {

                        if (delayedMessagesList.ContainsKey(m.id)) continue;

                        CancellationTokenSource cts = new CancellationTokenSource();

                        delayedMessagesList.Add(
                            m.id,
                        (
                            m,
                            cts
                        ));

                        _ = StartDelayedMessage(m, cts, client);

                    }


                }
                catch (TaskCanceledException e)
                {
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(TimeSpan.FromMinutes(30));

            }

        }


        private static async Task StartDelayedMessage(DelayedMessage m, CancellationTokenSource cts, DiscordClient client)
        {

            TimeSpan time = TimeSpan.FromTicks(m.date.Ticks - DateTime.Now.Ticks);

            if (time.Ticks <= 0) time = TimeSpan.FromTicks(1);

            try
            {
                await Task.Delay(time, cts.Token);
            }
            catch (TaskCanceledException e)
            {
                return;
            }

            await m.Execute(client);

        }

        public static void UpdateDelayedMessage(DelayedMessage m, DiscordClient client)
        {

            int span = (int)(DateTime.Now - m.date).TotalMinutes;

            if (span < 40)
            {

                m.Save();

                CancellationTokenSource cts = new CancellationTokenSource();

                if (delayedMessagesList.ContainsKey(m.id))
                {
                    delayedMessagesList[m.id].cts.Cancel();
                    delayedMessagesList[m.id] = (m, cts);
                }
                else
                {

                    delayedMessagesList.Add(
                        m.id,
                    (
                        m,
                        cts
                    ));

                }

                _ = StartDelayedMessage(m, cts, client);

            }

        }

        public static void DeleteDelayedRole(DelayedMessage m)
        {
            if (delayedMessagesList.ContainsKey(m.id))
            {
                delayedMessagesList.Remove(m.id);
            }
        }

    }
}
