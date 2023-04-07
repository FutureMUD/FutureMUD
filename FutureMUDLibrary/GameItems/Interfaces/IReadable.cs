using System.Collections.Generic;
using MudSharp.Communication;
using MudSharp.Communication.Language;

namespace MudSharp.GameItems.Interfaces {
    public interface IReadable : IGameItemComponent {
        IEnumerable<IWriting> Writings { get; }
        IEnumerable<IDrawing> Drawings { get; }
        IEnumerable<ICanBeRead> Readables { get; }
    }
}
