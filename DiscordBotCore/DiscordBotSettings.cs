using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public partial class DiscordBot {
        public class DiscordBotSetttings {
            public int Port { get; set; }
            public string Token { get; set; }
            public IList<string> Prefixes { get; set; }
            public string ServerAuth { get; set; }
            public ulong AnnounceChannelId { get; set; }
            public ulong AdminAnnounceChannelId { get; set; }
            public ulong DebugAnnounceChannelId { get; set; }
            public string GameName { get; set; }
            public IList<ulong> AdminUsers { get; set; }
            public IList<CustomGlobalReaction> CustomGlobalReactions { get; set; }
        }
    }
}
