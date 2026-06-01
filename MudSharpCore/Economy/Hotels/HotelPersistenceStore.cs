using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using DbHotel = MudSharp.Models.Hotel;
using DbHotelBannedPatron = MudSharp.Models.HotelBannedPatron;
using DbHotelLostProperty = MudSharp.Models.HotelLostProperty;
using DbHotelPatronBalance = MudSharp.Models.HotelPatronBalance;
using DbHotelRoom = MudSharp.Models.HotelRoom;
using DbHotelRoomFurnishing = MudSharp.Models.HotelRoomFurnishing;
using DbHotelRoomKey = MudSharp.Models.HotelRoomKey;
using DbHotelRoomRental = MudSharp.Models.HotelRoomRental;

#nullable enable

namespace MudSharp.Economy.Hotels;

public static class HotelPersistenceStore
{
	public static IHotel? LoadIfExists(IProperty property)
	{
		using (new FMDB())
		{
			var hotelId = ExistingHotelId(FMDB.Context, property.Id);
			return hotelId.HasValue ? new Hotel(property, hotelId.Value) : null;
		}
	}

	public static IHotel LoadOrCreate(IProperty property)
	{
		using (new FMDB())
		{
			var context = FMDB.Context;
			var existingHotelId = ExistingHotelId(context, property.Id);
			if (existingHotelId.HasValue)
			{
				return new Hotel(property, existingHotelId.Value);
			}

			var hotel = SaveInternal(context, property, forceCreate: true)!;
			context.SaveChanges();
			return new Hotel(property, hotel.Id);
		}
	}

	public static void Save(IProperty property)
	{
		var hadContext = FMDB.Context is not null;
		using (new FMDB())
		{
			var context = FMDB.Context ?? throw new InvalidOperationException("Could not open the FutureMUD database context.");
			SaveInternal(context, property, forceCreate: false);
			if (!hadContext)
			{
				context.SaveChanges();
			}
		}
	}

	private static long? ExistingHotelId(FuturemudDatabaseContext context, long propertyId)
	{
		return context.Hotels
		              .AsNoTracking()
		              .Where(x => x.PropertyId == propertyId)
		              .Select(x => (long?)x.Id)
		              .FirstOrDefault();
	}

	private static DbHotel? SaveInternal(FuturemudDatabaseContext context, IProperty property, bool forceCreate)
	{
		var hotel = TrackedHotel(context, property.Id);
		if (hotel is null)
		{
			if (!forceCreate && !HasPersistableHotelState(property))
			{
				return null;
			}

			var now = DateTime.UtcNow;
			hotel = new DbHotel
			{
				PropertyId = property.Id,
				CreatedAt = now,
				LastUpdatedAt = now
			};
			context.Hotels.Add(hotel);
		}

		WriteHotelFields(hotel, property);
		ReplaceChildRows(context, hotel, property);
		return hotel;
	}

	private static bool HasPersistableHotelState(IProperty property)
	{
		return property.HotelLicenseStatus != HotelLicenseStatus.None ||
		       property.HotelBankAccount is not null ||
		       property.HotelCanRentProg is not null ||
		       property.HotelLostPropertyRetention != MudSharp.TimeAndDate.MudTimeSpan.FromDays(14) ||
		       property.HotelOutstandingTaxes != 0.0M ||
		       property.HotelRooms.Any() ||
		       property.HotelLostProperties.Any() ||
		       property.HotelPatronBalances.Any(x => x.Balance != 0.0M) ||
		       property.HotelBannedPatronIds.Any();
	}

	private static DbHotel? TrackedHotel(FuturemudDatabaseContext context, long propertyId)
	{
		return context.Hotels
		              .Include(x => x.Rooms)
		              .ThenInclude(x => x.Keys)
		              .Include(x => x.Rooms)
		              .ThenInclude(x => x.Furnishings)
		              .Include(x => x.Rooms)
		              .ThenInclude(x => x.ActiveRental)
		              .Include(x => x.LostProperties)
		              .Include(x => x.PatronBalances)
		              .Include(x => x.BannedPatrons)
		              .AsSplitQuery()
		              .FirstOrDefault(x => x.PropertyId == propertyId);
	}

	private static void WriteHotelFields(DbHotel hotel, IProperty property)
	{
		hotel.BankAccountId = property.HotelBankAccount?.Id;
		hotel.LicenseStatus = (int)property.HotelLicenseStatus;
		hotel.CanRentProgId = property.HotelCanRentProg?.Id;
		hotel.LostPropertyRetention = property.HotelLostPropertyRetention.GetRoundTripParseText;
		hotel.OutstandingTaxes = property.HotelOutstandingTaxes;
		hotel.LastUpdatedAt = DateTime.UtcNow;
	}

	private static void ReplaceChildRows(FuturemudDatabaseContext context, DbHotel hotel, IProperty property)
	{
		var rooms = hotel.Rooms.ToList();
		context.HotelRoomRentals.RemoveRange(rooms.Select(x => x.ActiveRental).OfType<DbHotelRoomRental>().ToList());
		context.HotelRoomKeys.RemoveRange(rooms.SelectMany(x => x.Keys).ToList());
		context.HotelRoomFurnishings.RemoveRange(rooms.SelectMany(x => x.Furnishings).ToList());
		context.HotelLostProperties.RemoveRange(hotel.LostProperties.ToList());
		context.HotelRooms.RemoveRange(rooms);
		hotel.Rooms.Clear();
		hotel.LostProperties.Clear();

		var roomRecords = property.HotelRooms
		                          .Where(x => x.Cell is not null)
		                          .Select(room => (Room: room, Record: CreateRoomRecord(hotel, room)))
		                          .ToList();
		foreach (var (_, record) in roomRecords)
		{
			context.HotelRooms.Add(record);
		}

		foreach (var (room, record) in roomRecords)
		{
			foreach (var key in room.Keys.Where(x => x is not null))
			{
				context.HotelRoomKeys.Add(new DbHotelRoomKey
				{
					HotelRoom = record,
					PropertyKeyId = key.Id
				});
			}

			foreach (var furnishing in room.Furnishings.Where(x => x is not null))
			{
				context.HotelRoomFurnishings.Add(new DbHotelRoomFurnishing
				{
					HotelRoom = record,
					GameItemId = furnishing.GameItemId,
					Description = furnishing.Description,
					ReplacementValue = furnishing.ReplacementValue,
					OriginalCondition = furnishing.OriginalCondition,
					OriginalDamageCondition = furnishing.OriginalDamageCondition
				});
			}

			if (room.ActiveRental is { } rental)
			{
				context.HotelRoomRentals.Add(new DbHotelRoomRental
				{
					HotelRoom = record,
					GuestId = rental.GuestId,
					StartTime = rental.StartTime.GetDateTimeString(),
					EndTime = rental.EndTime.GetDateTimeString(),
					RentalCharge = rental.RentalCharge,
					SecurityDeposit = rental.SecurityDeposit,
					TaxCharged = rental.TaxCharged
				});
			}
		}

		foreach (var lost in property.HotelLostProperties.Where(x => x is not null))
		{
			var roomRecord = roomRecords.FirstOrDefault(x => ReferenceEquals(x.Room, lost.Room)).Record ??
			                 roomRecords.FirstOrDefault(x => x.Room.Cell.Id == lost.Room.Cell.Id).Record;
			if (roomRecord is null)
			{
				continue;
			}

			context.HotelLostProperties.Add(new DbHotelLostProperty
			{
				Hotel = hotel,
				HotelRoom = roomRecord,
				OwnerId = lost.OwnerId,
				BundleId = lost.BundleId,
				StoredUntil = lost.StoredUntil.GetDateTimeString(),
				Status = (int)lost.Status,
				AuctionHouseId = lost.AuctionHouseId,
				ReservePrice = lost.ReservePrice,
				Description = lost.Description
			});
		}

		SyncPatronBalances(context, hotel, property);
		SyncBannedPatrons(context, hotel, property);
	}

	private static void SyncPatronBalances(FuturemudDatabaseContext context, DbHotel hotel, IProperty property)
	{
		var desired = property.HotelPatronBalances
		                      .Where(x => x is not null && x.PatronId > 0 && x.Balance != 0.0M)
		                      .GroupBy(x => x.PatronId)
		                      .ToDictionary(x => x.Key, x => x.Last().Balance);

		foreach (var existing in hotel.PatronBalances.ToList())
		{
			if (desired.TryGetValue(existing.PatronId, out var balance))
			{
				existing.Balance = balance;
				desired.Remove(existing.PatronId);
				continue;
			}

			context.HotelPatronBalances.Remove(existing);
		}

		foreach (var (patronId, balance) in desired)
		{
			context.HotelPatronBalances.Add(new DbHotelPatronBalance
			{
				Hotel = hotel,
				PatronId = patronId,
				Balance = balance
			});
		}
	}

	private static void SyncBannedPatrons(FuturemudDatabaseContext context, DbHotel hotel, IProperty property)
	{
		var desired = property.HotelBannedPatronIds
		                      .Where(x => x > 0)
		                      .Distinct()
		                      .ToHashSet();

		foreach (var existing in hotel.BannedPatrons.ToList())
		{
			if (desired.Remove(existing.PatronId))
			{
				continue;
			}

			context.HotelBannedPatrons.Remove(existing);
		}

		foreach (var patronId in desired)
		{
			context.HotelBannedPatrons.Add(new DbHotelBannedPatron
			{
				Hotel = hotel,
				PatronId = patronId
			});
		}
	}

	private static DbHotelRoom CreateRoomRecord(DbHotel hotel, IHotelRoom room)
	{
		return new DbHotelRoom
		{
			Hotel = hotel,
			CellId = room.Cell.Id,
			Name = room.Name,
			Listed = room.Listed,
			PricePerDay = room.PricePerDay,
			SecurityDeposit = room.SecurityDeposit,
			MinimumDurationTicks = room.MinimumDuration.Ticks,
			MaximumDurationTicks = room.MaximumDuration.Ticks
		};
	}
}
