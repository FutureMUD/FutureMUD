using System;
using System.Linq;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Property;

#nullable enable

namespace MudSharp.Economy.Hotels;

public static class HotelPersistenceStore
{
	public static IHotel? LoadIfExists(IProperty property)
	{
		using (new FMDB())
		{
			var hotelId = FMDB.Context.Hotels
			                  .Where(x => x.PropertyId == property.Id)
			                  .Select(x => (long?)x.Id)
			                  .FirstOrDefault();
			return hotelId.HasValue ? new Hotel(property, hotelId.Value) : null;
		}
	}

	public static IHotel LoadOrCreate(IProperty property)
	{
		using (new FMDB())
		{
			var context = FMDB.Context;
			var hotel = context.Hotels.FirstOrDefault(x => x.PropertyId == property.Id);
			var definition = property is MudSharp.Economy.Property.Property concrete
				? concrete.CurrentHotelDefinitionXml()
				: string.Empty;
			if (hotel is null)
			{
				hotel = new MudSharp.Models.Hotel
				{
					PropertyId = property.Id,
					CreatedAt = DateTime.UtcNow
				};
				context.Hotels.Add(hotel);
			}

			WriteHotelFields(hotel, property, definition);
			context.SaveChanges();
			return new Hotel(property, hotel.Id);
		}
	}

	public static string DefinitionForProperty(long propertyId, string? legacyDefinition)
	{
		using (new FMDB())
		{
			var definition = FMDB.Context.Hotels
			                     .Where(x => x.PropertyId == propertyId)
			                     .Select(x => x.HotelDefinition)
			                     .FirstOrDefault();
			return string.IsNullOrWhiteSpace(definition) ? legacyDefinition ?? string.Empty : definition;
		}
	}

	public static void ShadowWrite(IProperty property, string definition)
	{
		var context = FMDB.Context;
		if (context is null)
		{
			return;
		}

		var hotel = context.Hotels.FirstOrDefault(x => x.PropertyId == property.Id);
		if (hotel is null)
		{
			return;
		}

		WriteHotelFields(hotel, property, definition);
	}

	private static void WriteHotelFields(MudSharp.Models.Hotel hotel, IProperty property, string definition)
	{
		hotel.BankAccountId = property.HotelBankAccount?.Id;
		hotel.LicenseStatus = (int)property.HotelLicenseStatus;
		hotel.CanRentProgId = property.HotelCanRentProg?.Id;
		hotel.LostPropertyRetention = property.HotelLostPropertyRetention.GetRoundTripParseText;
		hotel.OutstandingTaxes = property.HotelOutstandingTaxes;
		hotel.HotelDefinition = definition;
		hotel.LastUpdatedAt = DateTime.UtcNow;
	}
}
