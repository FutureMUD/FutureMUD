namespace MudSharp.GameItems.Inventory.Plans {
    public class InventoryPlanActionResult {
        public IGameItem PrimaryTarget { get; set; }
        public IGameItem SecondaryTarget { get; set; }
        public object TertiaryTarget { get; set; }
        public DesiredItemState ActionState { get; set; }
        public object OriginalReference { get; set; }
    }
}
