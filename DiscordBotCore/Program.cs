using System;
using System.IO;
using System.Threading.Tasks;

namespace Discord_Bot;

class Program
{
	private static async Task Main(string[] args) {
            try {
                var bot = DiscordBot.Instance;
                await bot.RunBotAsync();
            }
            catch (Exception e)
            {
                await using var writer = new StreamWriter($"Fatal Exception {DateTime.Now:yyyy MMMM dd HH mm ss}.txt");
                await writer.WriteAsync(e.ToString());
            }
        }
}