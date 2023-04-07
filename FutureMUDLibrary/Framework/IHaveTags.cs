using System.Collections.Generic;

namespace MudSharp.Framework
{
    public interface IHaveTags {
        IEnumerable<ITag> Tags { get; }
        bool AddTag(ITag tag);
        bool RemoveTag(ITag tag);
        bool IsA(ITag tag);
    }
}