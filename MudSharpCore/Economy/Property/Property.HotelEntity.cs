using MudSharp.Economy.Hotels;

#nullable enable

namespace MudSharp.Economy.Property;

public partial class Property
{
	private IHotel? _hotel;
	private bool _checkedExistingHotel;

	public IHotel Hotel => _hotel ??= HotelPersistenceStore.LoadOrCreate(this);

	internal IHotel? ExistingHotel
	{
		get
		{
			if (_hotel is not null)
			{
				return _hotel;
			}

			if (_checkedExistingHotel)
			{
				return null;
			}

			_checkedExistingHotel = true;
			return _hotel = HotelPersistenceStore.LoadIfExists(this);
		}
	}
}
