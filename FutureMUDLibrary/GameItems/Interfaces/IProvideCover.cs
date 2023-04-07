using MudSharp.Combat;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public interface IProvideCover : IGameItemComponent {
        IRangedCover Cover { get; }
        bool IsProvidingCover { get; set; }
        event PerceivableEvent OnNoLongerProvideCover;
    }
}