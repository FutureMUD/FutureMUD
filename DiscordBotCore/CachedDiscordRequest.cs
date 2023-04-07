using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace Discord_Bot
{
    public record CachedDiscordRequest()
    {
        private static ulong _nextId;
        private static readonly object NextIdLock = new object();
        public static ulong NextId
        {
            get
            {
                lock (NextIdLock)
                {
                    _nextId += 1;
                    return _nextId;
                }
            }
        }

        public ulong RequestId { get; private init; } = NextId;
        public CommandContext Context { get; init; }
        public Func<string,CommandContext,Task> OnResponseAction { get; init; }
    }
}
