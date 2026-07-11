using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Community;
using MudSharp.Economy.Property;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentHostDiscovery
{
	public static IEnumerable<IEmploymentHost> LoadedHosts(IFuturemud gameworld)
	{
		foreach (var shop in gameworld.Shops ?? Enumerable.Empty<IShop>())
		{
			yield return shop;
		}

		foreach (var auction in gameworld.AuctionHouses ?? Enumerable.Empty<IAuctionHouse>())
		{
			yield return auction;
		}

		foreach (var arena in gameworld.CombatArenas ?? Enumerable.Empty<ICombatArena>())
		{
			yield return arena;
		}

		foreach (var bank in gameworld.Banks ?? Enumerable.Empty<IBank>())
		{
			yield return bank;
		}

		foreach (var stable in gameworld.Stables ?? Enumerable.Empty<IStable>())
		{
			yield return stable;
		}

		foreach (var hospital in gameworld.Hospitals ?? Enumerable.Empty<IHospital>())
		{
			yield return hospital;
		}

		foreach (var clan in (gameworld.Clans ?? Enumerable.Empty<IClan>())
		         .Where(x => !x.IsTemplate))
		{
			yield return clan;
		}

		foreach (var hotel in (gameworld.Properties ?? Enumerable.Empty<IProperty>())
		                  .Select(x => x.ExistingHotel)
		                  .Where(x => x is not null)
		                  .Cast<IHotel>())
		{
			yield return hotel;
		}
	}
}
