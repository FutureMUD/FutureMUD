using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Property;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MudSharp.RPG.Law;

public static class AutomaticCrimeExtensions
{
	public const string GreviousBodilyHarmMinimumSeveritySetting = "AutomaticGreviousBodilyHarmMinimumSeverity";
	public const string MurderMinimumSeveritySetting = "AutomaticMurderMinimumWoundSeverity";
	public const string MurderWoundAttributionWindowSecondsSetting = "AutomaticMurderWoundAttributionWindowSeconds";
	public const string MurderIncludeFriendlyWoundsSetting = "AutomaticMurderIncludeFriendlyWounds";

	public static bool HandleCrimeAndLawfulActing(IFuturemud gameworld, ICharacter criminal, CrimeTypes crime,
		ICharacter victim = null, IGameItem target = null, string additionalInformation = "", ICell crimeLocation = null)
	{
		if (criminal.IsAdministrator())
		{
			return false;
		}

		if (!CheckWouldBeACrime(gameworld, criminal, crime, victim, target, additionalInformation, crimeLocation))
		{
			return false;
		}

		if (criminal.Account?.ActLawfully == true)
		{
			criminal.OutputHandler.Send($"That action would be a crime.\n{CrimeExtensions.StandardDisableIllegalFlagText}");
			return true;
		}

		CheckPossibleCrime(gameworld, criminal, crime, victim, target, additionalInformation, null, true, crimeLocation);
		return false;
	}

	public static bool CheckWouldBeACrime(IFuturemud gameworld, ICharacter criminal, CrimeTypes crime,
		ICharacter victim = null, IGameItem target = null, string additionalInformation = "", ICell crimeLocation = null)
	{
		foreach (var authority in gameworld.LegalAuthorities)
		{
			var isCrime = crimeLocation is null
				? authority.WouldBeACrime(criminal, crime, victim, target, additionalInformation)
				: authority.WouldBeACrimeAtLocation(criminal, crime, victim, target, additionalInformation, crimeLocation);
			if (isCrime)
			{
				return true;
			}
		}

		return false;
	}

	public static void CheckPossibleCrime(IFuturemud gameworld, ICharacter criminal, CrimeTypes crime,
		ICharacter victim = null, IGameItem target = null, string additionalInformation = "",
		IEnumerable<ICharacter> witnesses = null, bool notifyVictim = true, ICell crimeLocation = null)
	{
		foreach (var authority in gameworld.LegalAuthorities)
		{
			if (crimeLocation is null)
			{
				authority.CheckPossibleCrime(criminal, crime, victim, target, additionalInformation, witnesses, notifyVictim);
			}
			else
			{
				authority.CheckPossibleCrime(criminal, crime, victim, target, additionalInformation, witnesses,
					notifyVictim, crimeLocation);
			}
		}
	}

	public static void CheckMurderForDeath(ICharacter victim)
	{
		var deathLocation = victim.Location;
		if (deathLocation is null)
		{
			return;
		}

		var now = DateTime.UtcNow;
		var minimumSeverity = MinimumMurderWoundSeverity(victim.Gameworld);
		var attributionWindow = MurderWoundAttributionWindow(victim.Gameworld);
		var includeFriendlyWounds = IncludeFriendlyMurderWounds(victim.Gameworld);
		var presentCharacters = deathLocation
			.LayerCharacters(victim.RoomLayer)
			.Except(victim)
			.ToList();

		foreach (var group in victim.Body.Wounds
			         .Where(x => IsEligibleMurderWound(x, victim, now, minimumSeverity, attributionWindow,
				         includeFriendlyWounds))
			         .GroupBy(x => x.ActorOrigin))
		{
			var attacker = group.Key;
			if (attacker.IsAdministrator())
			{
				continue;
			}

			var mostSerious = group.OrderByDescending(x => x.Severity).First();
			var attackerPresent = presentCharacters.Contains(attacker);
			var witnesses = attackerPresent
				? presentCharacters
					.Where(x => x != attacker)
					.Where(x => x.CanSee(victim))
					.Where(x => x.CanSee(attacker))
					.ToList()
				: new List<ICharacter>();
			var context =
				$"automatic=death; victim=#{CharacterInstanceIdentityComparer.IdentityId(victim)}; wounds={group.Count()}; maxseverity={mostSerious.Severity.Describe()}; damagetype={mostSerious.DamageType.Describe()}; bodypart={ContextValue(mostSerious.Bodypart?.FullDescription() ?? "unknown")}; woundage={ContextValue(DescribeWoundAge(mostSerious.RealTimeOfWound, now))}; friendly={mostSerious.IsFriendlyWound.ToString().ToLowerInvariant()}; attackerpresent={attackerPresent.ToString().ToLowerInvariant()}";
			CheckPossibleCrime(attacker.Gameworld, attacker, CrimeTypes.Murder, victim, mostSerious.ToolOrigin, context,
				witnesses, false, deathLocation);
		}
	}

	private static bool IsEligibleMurderWound(IWound wound, ICharacter victim, DateTime now,
		WoundSeverity minimumSeverity, TimeSpan attributionWindow, bool includeFriendlyWounds)
	{
		if (wound.ActorOrigin is null || wound.ActorOrigin == victim || wound.ActorOrigin.IsAdministrator())
		{
			return false;
		}

		if (!includeFriendlyWounds && wound.IsFriendlyWound)
		{
			return false;
		}

		if (wound.Severity < minimumSeverity)
		{
			return false;
		}

		if (wound.RealTimeOfWound is not { } woundTime)
		{
			return false;
		}

		var woundAge = now - woundTime;
		return woundAge >= TimeSpan.Zero && woundAge <= attributionWindow;
	}

	public static void CheckGreviousBodilyHarmForWound(IWound wound)
	{
		if (wound.Parent is not ICharacter victim || wound.ActorOrigin is null || wound.ActorOrigin == victim)
		{
			return;
		}

		var minimumSeverity = MinimumGreviousBodilyHarmSeverity(victim.Gameworld);
		if (wound.Severity < minimumSeverity)
		{
			return;
		}

		var context =
			$"automatic=wound; victim=#{CharacterInstanceIdentityComparer.IdentityId(victim)}; severity={wound.Severity.Describe()}; damagetype={wound.DamageType.Describe()}; bodypart={ContextValue(wound.Bodypart?.FullDescription() ?? "unknown")}";
		CheckPossibleCrime(wound.ActorOrigin.Gameworld, wound.ActorOrigin, CrimeTypes.GreviousBodilyHarm, victim,
			wound.ToolOrigin, context, null, true, victim.Location);
	}

	public static WoundSeverity MinimumMurderWoundSeverity(IFuturemud gameworld)
	{
		var setting = gameworld.GetStaticConfiguration(MurderMinimumSeveritySetting);
		return WoundExtensions.TryParseWoundSeverity(setting, out var severity) ? severity : WoundSeverity.Severe;
	}

	public static TimeSpan MurderWoundAttributionWindow(IFuturemud gameworld)
	{
		var setting = gameworld.GetStaticConfiguration(MurderWoundAttributionWindowSecondsSetting);
		if (!double.TryParse(setting, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) || seconds <= 0.0)
		{
			seconds = 7200.0;
		}

		return TimeSpan.FromSeconds(seconds);
	}

	public static bool IncludeFriendlyMurderWounds(IFuturemud gameworld)
	{
		var setting = gameworld.GetStaticConfiguration(MurderIncludeFriendlyWoundsSetting);
		return bool.TryParse(setting, out var value) && value;
	}

	public static WoundSeverity MinimumGreviousBodilyHarmSeverity(IFuturemud gameworld)
	{
		var setting = gameworld.GetStaticConfiguration(GreviousBodilyHarmMinimumSeveritySetting);
		return WoundExtensions.TryParseWoundSeverity(setting, out var severity) ? severity : WoundSeverity.Grievous;
	}

	public static bool CheckLawfulMovement(ICharacter actor, ICellExit exit)
	{
		return CheckLawfulMovement(new[] { actor }, exit, actor);
	}

	public static bool CheckLawfulMovement(IEnumerable<ICharacter> actors, ICellExit exit, ICharacter movementLeader = null)
	{
		foreach (var actor in actors.Where(x => x is not null).Distinct())
		{
			if (actor.Account?.ActLawfully != true)
			{
				continue;
			}

			if (!WouldTrespass(actor.Gameworld, actor, exit.Destination, out _) &&
			    !WouldTrafficContraband(actor.Gameworld, actor, exit, out _))
			{
				continue;
			}

			actor.OutputHandler.Send($"That movement would be a crime.\n{CrimeExtensions.StandardDisableIllegalFlagText}");
			if (movementLeader is not null && movementLeader != actor)
			{
				movementLeader.OutputHandler.Send(
					$"{actor.HowSeen(movementLeader, true)} refuses to move because that movement would be a crime.");
			}

			return true;
		}

		return false;
	}

	public static void CheckLocationEntryCrimes(ICharacter actor, ICell enteredCell, ICellExit exit,
		bool isVoluntaryEntry = true)
	{
		if (!isVoluntaryEntry || actor.IsAdministrator())
		{
			return;
		}

		if (WouldTrespass(actor.Gameworld, actor, enteredCell, out var trespassContext))
		{
			CheckPossibleCrime(actor.Gameworld, actor, CrimeTypes.Trespassing, null, null, trespassContext, null, true,
				enteredCell);
		}

		if (!IsLegalAuthorityBoundaryEntry(actor.Gameworld, exit))
		{
			return;
		}

		foreach (var item in InventoryItemsForContrabandCheck(actor))
		{
			var context = ContrabandTrafficContext(actor, exit, item);
			if (!CheckWouldBeACrime(actor.Gameworld, actor, CrimeTypes.TrafficingContraband, null, item, context,
				    enteredCell))
			{
				continue;
			}

			CheckPossibleCrime(actor.Gameworld, actor, CrimeTypes.TrafficingContraband, null, item, context, null, true,
				enteredCell);
		}
	}

	private static bool WouldTrespass(IFuturemud gameworld, ICharacter actor, ICell destination, out string context)
	{
		context = string.Empty;
		var property = gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(destination));
		if (property is null || !property.ApplyCriminalCodeInProperty || IsAuthorisedForProperty(actor, property, destination))
		{
			return false;
		}

		context = TrespassContext(property, destination);
		return CheckWouldBeACrime(gameworld, actor, CrimeTypes.Trespassing, null, null, context, destination);
	}

	private static bool IsAuthorisedForProperty(ICharacter actor, IProperty property, ICell destination)
	{
		return property.IsAuthorisedOwner(actor) ||
		       property.IsAuthorisedLeaseHolder(actor) ||
		       property.Lease?.IsTenant(actor, false) == true ||
		       property.HotelRoomForCell(destination)?.ActiveRental?.Guest == actor ||
		       actor.AffectedBy<PermitWork>(x => x.Property == property || x.Cell == destination);
	}

	private static string TrespassContext(IProperty property, ICell destination)
	{
		return $"automatic=property-entry; property=#{property.Id}; propertyname={ContextValue(property.Name)}; cell=#{destination.Id}";
	}

	private static bool WouldTrafficContraband(IFuturemud gameworld, ICharacter actor, ICellExit exit, out string context)
	{
		context = string.Empty;
		if (!IsLegalAuthorityBoundaryEntry(gameworld, exit))
		{
			return false;
		}

		foreach (var item in InventoryItemsForContrabandCheck(actor))
		{
			context = ContrabandTrafficContext(actor, exit, item);
			if (CheckWouldBeACrime(gameworld, actor, CrimeTypes.TrafficingContraband, null, item, context,
				    exit.Destination))
			{
				return true;
			}
		}

		context = string.Empty;
		return false;
	}

	private static bool IsLegalAuthorityBoundaryEntry(IFuturemud gameworld, ICellExit exit)
	{
		return exit?.Origin?.Zone is not null &&
		       exit.Destination?.Zone is not null &&
		       gameworld.LegalAuthorities.Any(x =>
			       x.EnforcementZones.Contains(exit.Destination.Zone) &&
			       !x.EnforcementZones.Contains(exit.Origin.Zone));
	}

	private static string ContrabandTrafficContext(ICharacter actor, ICellExit exit, IGameItem item)
	{
		return
			$"automatic=contraband-boundary; item=#{item.Id}; itemname={ContextValue(item.Name)}; from=#{exit.Origin.Id}; to=#{exit.Destination.Id}; actor=#{CharacterInstanceIdentityComparer.IdentityId(actor)}";
	}

	private static string DescribeWoundAge(DateTime? woundTime, DateTime now)
	{
		if (woundTime is null)
		{
			return "unknown";
		}

		var age = now - woundTime.Value;
		if (age < TimeSpan.Zero)
		{
			age = TimeSpan.Zero;
		}

		return age.Describe();
	}

	private static string ContextValue(string value)
	{
		return value
			.Replace(';', ',')
			.Replace('\r', ' ')
			.Replace('\n', ' ')
			.Trim();
	}

	private static IEnumerable<IGameItem> InventoryItemsForContrabandCheck(ICharacter actor)
	{
		var seen = new HashSet<IGameItem>();
		foreach (var item in actor.Inventory)
		{
			foreach (var candidate in InventoryItemsForContrabandCheck(item, seen))
			{
				yield return candidate;
			}
		}
	}

	private static IEnumerable<IGameItem> InventoryItemsForContrabandCheck(IGameItem item, ISet<IGameItem> seen)
	{
		if (item is null || !seen.Add(item))
		{
			yield break;
		}

		yield return item;
		if (item.GetItemType<IContainer>() is not { } container)
		{
			yield break;
		}

		foreach (var content in container.Contents)
		{
			foreach (var candidate in InventoryItemsForContrabandCheck(content, seen))
			{
				yield return candidate;
			}
		}
	}
}
