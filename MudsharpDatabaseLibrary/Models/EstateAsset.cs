namespace MudSharp.Models;

public partial class EstateAsset
{
    public long Id { get; set; }
    public long EstateId { get; set; }
    public long FrameworkItemId { get; set; }
    public string FrameworkItemType { get; set; }
    public bool IsPresumedOwnership { get; set; }
    public decimal OwnershipShare { get; set; }
    public bool IsTransferred { get; set; }
    public bool IsLiquidated { get; set; }
    public decimal? LiquidatedValue { get; set; }

    public virtual Estate Estate { get; set; }
}
