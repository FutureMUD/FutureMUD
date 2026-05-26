using MudSharp.Economy.Hotels;

#nullable enable

namespace MudSharp.Economy.Property;

public partial class Property
{
	private IHotel? _hotel;

	public IHotel Hotel => _hotel ??= HotelPersistenceStore.LoadOrCreate(this);
}
