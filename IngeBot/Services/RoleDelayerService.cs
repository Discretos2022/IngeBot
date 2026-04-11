using DSharpPlus;
using IngeBot.Models;

namespace IngeBot.Services
{
    public class RoleDelayerService
    {

        private static Dictionary<int, (DelayedRole role, CancellationTokenSource cts)> delayedRolesList = new Dictionary<int, (DelayedRole message, CancellationTokenSource cts)>();


        public static async Task Run(DiscordClient client)
        {

            while (true)
            {


                try
                {


                    // DelayedMessage message = new DelayedMessage(1010, 1010, 1010, "name", "Ceci est un text", DateTime.Now.AddMinutes(10), false);
                    // message.Save();

                    DelayedRole[] delayedRoles = DelayedRole.FindNext();

                    foreach (var r in delayedRoles)
                    {

                        if (delayedRolesList.ContainsKey(r.id)) continue;

                        CancellationTokenSource cts = new CancellationTokenSource();

                        delayedRolesList.Add(
                            r.id,
                        (
                            r,
                            cts
                        ));

                        _ = StartDelayedRole(r, cts, client);

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


        private static async Task StartDelayedRole(DelayedRole r, CancellationTokenSource cts, DiscordClient client)
        {

            TimeSpan time;
            if (r.date_start != DateTime.MinValue)
                time = TimeSpan.FromTicks(r.date_start.Ticks - DateTime.Now.Ticks);
            else if (r.date_end != DateTime.MinValue)
                time = TimeSpan.FromTicks(r.date_end.Ticks - DateTime.Now.Ticks);
            else
            {
                r.Delete();
                return;
            }


            if (time.Ticks <= 0) time = TimeSpan.FromTicks(1);

            try
            {
                await Task.Delay(time, cts.Token);
            }
            catch (TaskCanceledException e)
            {
                return;
            }

            await r.Execute(client);

        }

        public static void UpdateDelayedRole(DelayedRole r, DiscordClient client)
        {

            TimeSpan span;
            if (r.date_start != DateTime.MinValue)
                span = TimeSpan.FromTicks(r.date_start.Ticks - DateTime.Now.Ticks);
            else if (r.date_end != DateTime.MinValue)
                span = TimeSpan.FromTicks(r.date_end.Ticks - DateTime.Now.Ticks);
            else
                return;

            if (span.TotalMinutes < 40)
            {

                r.Save();

                CancellationTokenSource cts = new CancellationTokenSource();

                if (delayedRolesList.ContainsKey(r.id))
                {
                    delayedRolesList[r.id].cts.Cancel();
                    delayedRolesList[r.id] = (r, cts);
                }
                else
                {

                    delayedRolesList.Add(
                        r.id,
                    (
                        r,
                        cts
                    ));

                }

                _ = StartDelayedRole(r, cts, client);

            }

        }

        public static void DeleteDelayedRole(DelayedRole r)
        {
            if (delayedRolesList.ContainsKey(r.id))
            {
                delayedRolesList.Remove(r.id);
            }
        }

    }
}
