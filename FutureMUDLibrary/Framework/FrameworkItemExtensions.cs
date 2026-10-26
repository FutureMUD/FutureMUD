using System;

namespace MudSharp.Framework;

public static class FrameworkItemExtensions
{
    /// <summary>
    /// Compares a framework item with a persisted reference; a null item matches only the null or zero ID sentinel.
    /// </summary>
    public static bool FrameworkItemEquals(this IFrameworkItem item, long? id, string type)
    {
        if (item == null)
        {
            return (id ?? 0) == 0;
        }

        return (item.Id == id) && item.MatchesPersistedReferenceType(type);
    }
}
