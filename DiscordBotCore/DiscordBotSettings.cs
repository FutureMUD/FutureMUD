using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord_Bot
{
    public partial class DiscordBot {
        public class DiscordBotSetttings {
            public int Port { get; init; }
            public string Token { get; init; }
            public IList<string> Prefixes { get; init; }
            public string ServerAuth { get; init; }
            public ulong AnnounceChannelId { get; init; }
            public ulong AdminAnnounceChannelId { get; init; }
            public ulong DebugAnnounceChannelId { get; init; }
            public string GameName { get; init; }
            public IList<ulong> AdminUsers { get; init; }
            public IList<CustomGlobalReaction> CustomGlobalReactions { get; init; }
        }
    }
}
