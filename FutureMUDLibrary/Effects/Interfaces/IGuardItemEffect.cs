using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces {
    public interface IGuardItemEffect : IRemoveOnMeleeCombat, IRemoveOnStateChange, IAffectedByChangeInGuarding
    {
        IGameItem TargetItem { get; }
        bool IncludeVicinity { get; }
    }
}