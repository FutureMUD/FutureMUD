using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

public partial class HotelRoom
{
	public HotelRoom()
	{
		Keys = new HashSet<HotelRoomKey>();
		Furnishings = new HashSet<HotelRoomFurnishing>();
	}

	public long Id { get; set; }
	public long HotelId { get; set; }
	public long CellId { get; set; }
	public string Name { get; set; } = string.Empty;
	public bool Listed { get; set; }
	public decimal PricePerDay { get; set; }
	public decimal SecurityDeposit { get; set; }
	public long MinimumDurationTicks { get; set; }
	public long MaximumDurationTicks { get; set; }

	public virtual Hotel Hotel { get; set; } = null!;
	public virtual Cell Cell { get; set; } = null!;
	public virtual ICollection<HotelRoomKey> Keys { get; set; }
	public virtual ICollection<HotelRoomFurnishing> Furnishings { get; set; }
	public virtual HotelRoomRental? ActiveRental { get; set; }
}

public partial class HotelRoomKey
{
	public long HotelRoomId { get; set; }
	public long PropertyKeyId { get; set; }

	public virtual HotelRoom HotelRoom { get; set; } = null!;
	public virtual PropertyKey PropertyKey { get; set; } = null!;
}

public partial class HotelRoomFurnishing
{
	public long Id { get; set; }
	public long HotelRoomId { get; set; }
	public long GameItemId { get; set; }
	public string Description { get; set; } = string.Empty;
	public decimal ReplacementValue { get; set; }
	public double OriginalCondition { get; set; }
	public double OriginalDamageCondition { get; set; }

	public virtual HotelRoom HotelRoom { get; set; } = null!;
}

public partial class HotelRoomRental
{
	public long Id { get; set; }
	public long HotelRoomId { get; set; }
	public long GuestId { get; set; }
	public string StartTime { get; set; } = string.Empty;
	public string EndTime { get; set; } = string.Empty;
	public decimal RentalCharge { get; set; }
	public decimal SecurityDeposit { get; set; }
	public decimal TaxCharged { get; set; }

	public virtual HotelRoom HotelRoom { get; set; } = null!;
}

public partial class HotelLostProperty
{
	public long Id { get; set; }
	public long HotelId { get; set; }
	public long HotelRoomId { get; set; }
	public long OwnerId { get; set; }
	public long BundleId { get; set; }
	public string StoredUntil { get; set; } = string.Empty;
	public int Status { get; set; }
	public long? AuctionHouseId { get; set; }
	public decimal ReservePrice { get; set; }
	public string Description { get; set; } = string.Empty;

	public virtual Hotel Hotel { get; set; } = null!;
	public virtual HotelRoom HotelRoom { get; set; } = null!;
}

public partial class HotelPatronBalance
{
	public long HotelId { get; set; }
	public long PatronId { get; set; }
	public decimal Balance { get; set; }

	public virtual Hotel Hotel { get; set; } = null!;
}

public partial class HotelBannedPatron
{
	public long HotelId { get; set; }
	public long PatronId { get; set; }

	public virtual Hotel Hotel { get; set; } = null!;
}
