namespace MudSharp.Models;

public partial class EstateClaim
{
    public long Id { get; set; }
    public long EstateId { get; set; }
    public long ClaimantId { get; set; }
    public string ClaimantType { get; set; }
    public long? TargetId { get; set; }
    public string TargetType { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public int ClaimStatus { get; set; }
    public string StatusReason { get; set; }
    public bool IsSecured { get; set; }
    public string ClaimDate { get; set; }

    public virtual Estate Estate { get; set; }
}
