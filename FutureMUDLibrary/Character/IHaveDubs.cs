using MudSharp.Framework;
using System.Collections.Generic;

namespace MudSharp.Character
{
    public interface IHaveDubs
    {
        IList<IDub> Dubs { get; }
        bool HasDubFor(IKeyworded target, IEnumerable<string> keywords);
        bool HasDubFor(IKeyworded target, string keyword);
    }
}