using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord_Bot;

class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            DiscordBot bot = DiscordBot.Instance;
            await bot.RunBotAsync();
        }
        catch (Exception e)
        {
            await using StreamWriter writer = new($"Fatal Exception {DateTime.Now:yyyy MMMM dd HH mm ss}.txt");
            await writer.WriteAsync(e.ToString());
        }
    }
}