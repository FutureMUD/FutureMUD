using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.Character {
    public interface IHaveDubs {
        IList<IDub> Dubs { get; }
        bool HasDubFor(IKeyworded target, IEnumerable<string> keywords);
        bool HasDubFor(IKeyworded target, string keyword);
    }
}