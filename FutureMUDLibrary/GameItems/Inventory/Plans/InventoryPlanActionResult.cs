namespace MudSharp.GameItems.Inventory.Plans {
    public class InventoryPlanActionResult {
        public IGameItem PrimaryTarget { get; init; }
        public IGameItem SecondaryTarget { get; init; }
        public object TertiaryTarget { get; init; }
        public DesiredItemState ActionState { get; init; }
        public object OriginalReference { get; init; }
    }
}
