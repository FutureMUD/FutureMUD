#nullable enable

using MudSharp.GameItems;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal sealed record SeededNaturalAttackLink(
	WeaponAttack Attack,
	ItemQuality Quality,
	IReadOnlyList<string>? BodypartAliases = null);

internal static class NonHumanNaturalAttackReconciler
{
	internal static void Reconcile(
		FuturemudDatabaseContext context,
		Race race,
		IEnumerable<SeededNaturalAttackLink> expectedLinks,
		IReadOnlyCollection<string> managedAttackNames)
	{
		var expected = new Dictionary<(long AttackId, long BodypartId), (WeaponAttack Attack, BodypartProto Bodypart, ItemQuality Quality)>();
		foreach (SeededNaturalAttackLink expectedLink in expectedLinks)
		{
			IEnumerable<BodypartProto?> bodyparts = expectedLink.BodypartAliases is { Count: > 0 }
				? expectedLink.BodypartAliases.Select(x => SeederBodyUtilities.FindBodypartOnBodyOrAncestors(context, race.BaseBody, x))
				: SeederBodyUtilities.GetEffectiveBodyparts(context, race.BaseBody)
					.Where(x => x.BodypartShapeId == expectedLink.Attack.BodypartShapeId);
			foreach (BodypartProto bodypart in bodyparts.Where(x => x is not null).Cast<BodypartProto>())
			{
				expected[(expectedLink.Attack.Id, bodypart.Id)] = (expectedLink.Attack, bodypart, expectedLink.Quality);
			}
		}

		HashSet<long> managedAttackIds = context.WeaponAttacks
			.Where(x => managedAttackNames.Contains(x.Name))
			.Select(x => x.Id)
			.ToHashSet();
		foreach (RacesWeaponAttacks stale in context.RacesWeaponAttacks
			         .Where(x => x.RaceId == race.Id && managedAttackIds.Contains(x.WeaponAttackId))
			         .ToList())
		{
			if (!expected.ContainsKey((stale.WeaponAttackId, stale.BodypartId)))
			{
				context.RacesWeaponAttacks.Remove(stale);
			}
		}

		foreach (((long attackId, long bodypartId), (WeaponAttack attack, BodypartProto bodypart, ItemQuality quality)) in expected)
		{
			RacesWeaponAttacks? link = context.RacesWeaponAttacks.FirstOrDefault(x =>
				x.RaceId == race.Id && x.WeaponAttackId == attackId && x.BodypartId == bodypartId);
			if (link is null)
			{
				context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
				{
					Race = race,
					WeaponAttack = attack,
					Bodypart = bodypart,
					Quality = (int)quality
				});
				continue;
			}

			link.Quality = (int)quality;
		}
	}
}
