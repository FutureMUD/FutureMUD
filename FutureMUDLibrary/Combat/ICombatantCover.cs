using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

namespace MudSharp.Combat
{
    public interface ICombatantCover
    {
        IMortalPerceiver Owner { get; }
        IRangedCover Cover { get; }
        IProvideCover CoverItem { get; }
        void RegisterEvents();
        void LeaveCover();
        void ReleaseEvents();
    }
}