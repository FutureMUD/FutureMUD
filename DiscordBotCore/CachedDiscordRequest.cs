using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot;

public record CachedDiscordRequest()
{
    private static ulong _nextId;
    private static readonly object NextIdLock = new();
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
    public Func<string, CommandContext, Task> OnResponseAction { get; init; }
}