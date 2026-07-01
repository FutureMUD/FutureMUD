using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Property;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Employment;

public static class EmploymentHostAccessExtensions
{
	public static IEnumerable<IEmploymentContract> ActiveEmploymentContracts(this IEmploymentHost host)
	{
		return host.EmploymentContracts.Where(x => x.Status == EmploymentStatus.Active);
	}

	public static bool HasActiveEmploymentContract(this IEmploymentHost host, ICharacter? actor)
	{
		if (actor is null)
		{
			return false;
		}

		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return host.ActiveEmploymentContracts().Any(x => x.Employee.Id == actorIdentityId);
	}

	public static bool HasActiveEmploymentRole(this IEmploymentHost host, ICharacter? actor,
		params EmploymentRole[] roles)
	{
		if (actor is null)
		{
			return false;
		}

		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return host.ActiveEmploymentContracts().Any(x =>
			x.Employee.Id == actorIdentityId &&
			roles.Contains(x.Role));
	}

	public static bool HasManagerEmploymentAccess(this IEmploymentHost host, ICharacter? actor)
	{
		return actor?.IsAdministrator() == true ||
		       host.HasActiveEmploymentRole(actor, EmploymentRole.Manager, EmploymentRole.Proprietor);
	}

	public static bool HasProprietorEmploymentAccess(this IEmploymentHost host, ICharacter? actor)
	{
		return actor?.IsAdministrator() == true ||
		       host.HasActiveEmploymentRole(actor, EmploymentRole.Proprietor);
	}

	public static string ActiveEmploymentContractsTable(this IEmploymentHost host, ICharacter actor)
	{
		var contracts = host.ActiveEmploymentContracts()
		                    .OrderBy(x => x.Role)
		                    .ThenBy(x => x.Employee.Name)
		                    .ToList();
		if (!contracts.Any())
		{
			return "\tNone.".ColourError();
		}

		return StringUtilities.GetTextTable(
			from contract in contracts
			select new List<string>
			{
				contract.Id.ToString("N0", actor),
				contract.Employee.HowSeen(actor, colour: false),
				contract.Role.DescribeEnum(),
				contract.Authority.Authorities.DescribeEnum(),
				contract.StartedAt.ToString("g", actor)
			},
			new List<string>
			{
				"Contract",
				"Employee",
				"Role",
				"Authority",
				"Started"
			},
			actor,
			Telnet.Yellow);
	}

	public static IReadOnlyCollection<ICell> EmploymentHostLocations(this IEmploymentHost host)
	{
		var locations = new List<ICell>();
		switch (host)
		{
			case IPermanentShop shop:
				AddLocations(locations, shop.AllShopCells);
				AddLocations(locations, shop.ShopfrontCells);
				AddLocation(locations, shop.StockroomCell);
				AddLocation(locations, shop.WorkshopCell);
				break;
			case IShop shop:
				AddLocations(locations, shop.CurrentLocations);
				break;
			case IAuctionHouse auctionHouse:
				AddLocation(locations, auctionHouse.AuctionHouseCell);
				break;
			case ICombatArena arena:
				AddLocations(locations, arena.WaitingCells);
				AddLocations(locations, arena.ArenaCells);
				AddLocations(locations, arena.ObservationCells);
				AddLocations(locations, arena.InfirmaryCells);
				AddLocations(locations, arena.NpcStablesCells);
				AddLocations(locations, arena.AfterFightCells);
				break;
			case IBank bank:
				AddLocations(locations, bank.BranchLocations);
				break;
			case IStable stable:
				AddLocation(locations, stable.Location);
				break;
			case IHotel hotel:
				AddLocations(locations, hotel.Locations);
				break;
			case IClan clan:
				AddClanLocations(locations, clan);
				break;
		}

		return locations.DistinctBy(x => x.Id).ToList();
	}

	private static void AddClanLocations(List<ICell> locations, IClan clan)
	{
		if (clan is IHaveFuturemud { Gameworld: not null } haveFuturemud &&
		    haveFuturemud.Gameworld.Properties is not null)
		{
			AddLocations(locations,
				haveFuturemud.Gameworld.Properties
				             .Where(x => x.PropertyOwners.Any(y =>
					             y.Owner is IClan ownerClan &&
					             ownerClan.Id == clan.Id))
				             .SelectMany(x => x.PropertyLocations));
		}

		AddLocations(locations, clan.ClanHallCells);
	}

	public static IReadOnlyCollection<ICharacter> PresentEmploymentObservers(this IEmploymentHost host)
	{
		return host.EmploymentHostLocations()
		           .SelectMany(x => x.Characters ?? Enumerable.Empty<ICharacter>())
		           .DistinctBy(x => x.Id)
		           .Where(x =>
			           x.IsAdministrator() ||
			           host.HasActiveEmploymentRole(x, EmploymentRole.Manager, EmploymentRole.Proprietor))
		           .ToList();
	}

	public static void EchoToPresentEmploymentObservers(this IEmploymentHost host,
		Func<ICharacter, string> messageFactory, ICharacter? except = null)
	{
		foreach (var observer in host.PresentEmploymentObservers())
		{
			if (except is not null && observer.Id == except.Id)
			{
				continue;
			}

			observer.OutputHandler?.Send(messageFactory(observer));
		}
	}

	public static void EchoToPresentEmploymentObservers(this IEmploymentHost host, string message,
		ICharacter? except = null)
	{
		host.EchoToPresentEmploymentObservers(_ => message, except);
	}

	public static void DebugEmployment(this IEmploymentHost host, string message, IFuturemud? fallbackGameworld = null)
	{
		var gameworld = fallbackGameworld ??
		                (host as IHaveFuturemud)?.Gameworld ??
		                host.EmploymentHostLocations()
		                    .SelectMany(x => x.Characters ?? Enumerable.Empty<ICharacter>())
		                    .Select(x => x.Gameworld)
		                    .FirstOrDefault(x => x is not null);
		gameworld?.DebugMessage(
			$"[Employment] {host.EmploymentHostType.DescribeEnum()} #{host.Id:N0} {host.EmploymentHostName}: {message}");
	}

	private static void AddLocations(List<ICell> locations, IEnumerable<ICell>? cells)
	{
		if (cells is null)
		{
			return;
		}

		foreach (var cell in cells)
		{
			AddLocation(locations, cell);
		}
	}

	private static void AddLocation(List<ICell> locations, ICell? cell)
	{
		if (cell is not null)
		{
			locations.Add(cell);
		}
	}
}
