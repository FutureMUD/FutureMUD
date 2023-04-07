using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Effects.Interfaces {
    public interface IInventoryPlanItemEffect : IEffectSubtype {
        IInventoryPlan Plan { get; }
        IGameItem TargetItem { get; }
        IGameItem SecondaryItem { get; }
        DesiredItemState DesiredState { get; }
    }
}