namespace MudSharp.Models;

public partial class EstatePayout
{
    public long Id { get; set; }
    public long EstateId { get; set; }
    public long RecipientId { get; set; }
    public string RecipientType { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public string CreatedDate { get; set; }
    public string CollectedDate { get; set; }

    public virtual Estate Estate { get; set; }
}
