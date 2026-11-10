using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord_Bot;

public partial class DiscordBot
{
    public class DiscordBotSetttings
    {
        public int Port { get; init; }
        public string ListenAddress { get; init; } = "127.0.0.1";
        public string Token { get; init; }
        public IList<string> Prefixes { get; init; }
        public string ServerAuth { get; init; }
        public ulong AnnounceChannelId { get; init; }
        public ulong AdminAnnounceChannelId { get; init; }
        public ulong DebugAnnounceChannelId { get; init; }
        public string GameName { get; init; }
        public IList<ulong> AdminUsers { get; init; }
        public IList<ulong> OwnerUserIds { get; init; } = Array.Empty<ulong>();
        public IList<CustomGlobalReaction> CustomGlobalReactions { get; init; }
    }
}
