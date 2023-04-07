namespace MudSharp.Work.Crafts
{
    /// <summary>
    /// A craft input that implements this interface guarantees that it targets and consumes a game item as part of its' process and that its CraftInputData.Perceivable field will be a reference to this item
    /// </summary>
    public interface ICraftInputConsumesGameItem : ICraftInput
    {
    }
}
