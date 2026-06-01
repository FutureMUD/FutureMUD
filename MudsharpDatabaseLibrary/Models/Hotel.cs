using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

public partial class Hotel
{
	public Hotel()
	{
		Rooms = new HashSet<HotelRoom>();
		LostProperties = new HashSet<HotelLostProperty>();
		PatronBalances = new HashSet<HotelPatronBalance>();
		BannedPatrons = new HashSet<HotelBannedPatron>();
	}

	public long Id { get; set; }
	public long PropertyId { get; set; }
	public long? BankAccountId { get; set; }
	public int LicenseStatus { get; set; }
	public long? CanRentProgId { get; set; }
	public string LostPropertyRetention { get; set; } = string.Empty;
	public decimal OutstandingTaxes { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime LastUpdatedAt { get; set; }

	public virtual Property Property { get; set; } = null!;
	public virtual BankAccount? BankAccount { get; set; }
	public virtual FutureProg? CanRentProg { get; set; }
	public virtual ICollection<HotelRoom> Rooms { get; set; }
	public virtual ICollection<HotelLostProperty> LostProperties { get; set; }
	public virtual ICollection<HotelPatronBalance> PatronBalances { get; set; }
	public virtual ICollection<HotelBannedPatron> BannedPatrons { get; set; }
}
