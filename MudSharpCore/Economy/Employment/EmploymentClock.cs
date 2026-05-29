using System;
using System.Globalization;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Economy.Property;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentClock
{
	private static readonly DateTime EncodedEpoch = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
	private static readonly long MaxEncodedSeconds = (DateTime.MaxValue.Ticks - DateTime.MinValue.Ticks) / TimeSpan.TicksPerSecond;

	public static IEconomicZone? EconomicZone(IEmploymentHost host)
	{
		return host switch
		{
			IShop shop => shop.EconomicZone,
			IAuctionHouse auctionHouse => auctionHouse.EconomicZone,
			ICombatArena arena => arena.EconomicZone,
			IBank bank => bank.EconomicZone,
			IStable stable => stable.EconomicZone,
			IHotel hotel => hotel.EconomicZone,
			_ => host.Market?.EconomicZone
		};
	}

	public static DateTimeOffset CurrentInstant(IEmploymentHost host)
	{
		var current = CurrentDateTime(host);
		if (current?.Date is null)
		{
			return DateTimeOffset.UtcNow;
		}

		return FromMudDateTime(current);
	}

	public static MudDateTime? CurrentDateTime(IEmploymentHost host)
	{
		return EconomicZone(host)?.FinancialPeriodReferenceCalendar?.CurrentDateTime;
	}

	public static DateTimeOffset FromMudDateTime(MudDateTime dateTime)
	{
		var instant = MudInstant.FromMudDateTime(dateTime);
		if (instant.IsNever)
		{
			return DateTimeOffset.UtcNow;
		}

		var seconds = Math.Clamp(instant.Ticks, 0L, MaxEncodedSeconds);
		return new DateTimeOffset(EncodedEpoch.AddTicks(seconds * TimeSpan.TicksPerSecond), TimeSpan.Zero);
	}

	public static bool IsEncodedInstant(DateTimeOffset instant)
	{
		return instant.UtcDateTime.Year < 1900;
	}

	public static DateTimeOffset NormaliseLoadedInstant(IEmploymentHost host, DateTime value)
	{
		var offset = new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
		return EconomicZone(host) is not null && !IsEncodedInstant(offset)
			? CurrentInstant(host)
			: offset;
	}

	public static DateTimeOffset? NormaliseLoadedInstant(IEmploymentHost host, DateTime? value)
	{
		return value.HasValue ? NormaliseLoadedInstant(host, value.Value) : null;
	}

	public static MudDateTime? ToMudDateTime(IEmploymentHost host, DateTimeOffset instant)
	{
		var zone = EconomicZone(host);
		if (zone?.FinancialPeriodReferenceCalendar is null || zone.FinancialPeriodReferenceClock is null ||
		    !IsEncodedInstant(instant))
		{
			return null;
		}

		var seconds = Math.Max(0L, (instant.UtcDateTime.Ticks - EncodedEpoch.Ticks) / TimeSpan.TicksPerSecond);
		return new MudInstant(
				MudInstant.CurrentEpoch,
				seconds,
				zone.FinancialPeriodReferenceCalendar.Id,
				zone.FinancialPeriodReferenceClock.Id)
			.ToMudDateTime(
				zone.FinancialPeriodReferenceCalendar,
				zone.FinancialPeriodReferenceClock,
				zone.FinancialPeriodTimezone);
	}

	public static string DescribeInstant(IEmploymentHost host, DateTimeOffset instant, ICharacter? voyeur)
	{
		var dateTime = ToMudDateTime(host, instant);
		if (dateTime?.Date is not null)
		{
			return dateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short);
		}

		return instant.ToString("g", (IFormatProvider?)voyeur ?? CultureInfo.InvariantCulture);
	}
}
