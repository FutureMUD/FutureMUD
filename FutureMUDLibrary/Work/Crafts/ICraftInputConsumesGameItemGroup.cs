namespace MudSharp.Work.Crafts
{
    /// <summary>
    /// A craft input that implements this interface guarantees that it targets and consumes a group of game items as part of its' process and that its CraftInputData.Perceivable field will be a reference to a PerceivableGroup composed of gameitems
    /// </summary>
    public interface ICraftInputConsumesGameItemGroup : ICraftInput
    {
    }
}
