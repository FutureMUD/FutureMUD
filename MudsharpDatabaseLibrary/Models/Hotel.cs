using System;

#nullable enable

namespace MudSharp.Models;

public partial class Hotel
{
	public long Id { get; set; }
	public long PropertyId { get; set; }
	public long? BankAccountId { get; set; }
	public int LicenseStatus { get; set; }
	public long? CanRentProgId { get; set; }
	public string LostPropertyRetention { get; set; } = string.Empty;
	public decimal OutstandingTaxes { get; set; }
	public string HotelDefinition { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdatedAt { get; set; }

	public virtual Property Property { get; set; } = null!;
	public virtual BankAccount? BankAccount { get; set; }
	public virtual FutureProg? CanRentProg { get; set; }
}
