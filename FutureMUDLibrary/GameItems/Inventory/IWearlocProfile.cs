namespace MudSharp.GameItems.Inventory
{
    public interface IWearlocProfile
    {
        bool Transparent { get; set; }
        bool NoArmour { get; set; }
        bool PreventsRemoval { get; set; }
        bool Mandatory { get; set; }
        bool HidesSeveredBodyparts { get; set; }
    }
}