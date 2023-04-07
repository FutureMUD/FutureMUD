namespace MudSharp.GameItems.Interfaces {
    public interface IStackable : IGameItemComponent {
        int Quantity { get; set; }
        ItemGetResponse CanGet(int quantity);
        IGameItem Get(int quantity);
        bool DropsWhole(int quantity);
        IGameItem Split(int quantity);

        /// <summary>
        ///     This function returns what the results of the split would be but DOESN'T reduce the actual stack.
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        IGameItem PeekSplit(int quantity);
    }
}