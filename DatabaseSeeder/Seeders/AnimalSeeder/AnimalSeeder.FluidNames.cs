#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	internal static string RaceBloodMaterialNameForTesting(string raceName) => GetRaceBloodMaterialName(raceName);
	internal static string RaceSweatMaterialNameForTesting(string raceName) => GetRaceSweatMaterialName(raceName);
	internal static string RaceBloodLiquidNameForTesting(string raceName) => GetRaceBloodLiquidName(raceName);
	internal static string RaceSweatLiquidNameForTesting(string raceName) => GetRaceSweatLiquidName(raceName);
	internal static bool HasLegacyAnimalFluidNamesForTesting(FuturemudDatabaseContext context) =>
		HasLegacyAnimalFluidNames(context);
	internal static void RepairLegacyAnimalFluidNamesForTesting(FuturemudDatabaseContext context) =>
		RepairLegacyAnimalFluidNames(context);

	private static string GetRaceBloodMaterialName(string raceName) => $"dried {raceName.ToLowerInvariant()} blood";
	private static string GetRaceSweatMaterialName(string raceName) => $"dried {raceName.ToLowerInvariant()} sweat";
	private static string GetRaceBloodLiquidName(string raceName) => $"{raceName} Blood";
	private static string GetRaceSweatLiquidName(string raceName) => $"{raceName} Sweat";

	private static string GetLegacyRaceBloodMaterialName(AnimalRaceTemplate template) =>
		$"dried {template.Adjective.ToLowerInvariant()} blood";

	private static string GetLegacyRaceSweatMaterialName(AnimalRaceTemplate template) =>
		$"dried {template.Adjective.ToLowerInvariant()} sweat";

	private static string GetLegacyRaceBloodLiquidName(AnimalRaceTemplate template) => $"{template.Adjective} Blood";
	private static string GetLegacyRaceSweatLiquidName(AnimalRaceTemplate template) => $"{template.Adjective} Sweat";

	private static bool HasLegacyAnimalFluidNames(FuturemudDatabaseContext context)
	{
		var racesByName = context.Races
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var liquidsById = context.Liquids
			.ToDictionary(x => x.Id);
		var materialsById = context.Materials
			.ToDictionary(x => x.Id);

		return RaceTemplates.Values.Any(template =>
			racesByName.TryGetValue(template.Name, out var race) &&
			(HasLegacyLiquidName(
					race.BloodLiquidId,
					GetRaceBloodLiquidName(template.Name),
					GetLegacyRaceBloodLiquidName(template),
					GetRaceBloodMaterialName(template.Name),
					GetLegacyRaceBloodMaterialName(template)) ||
			 (template.Sweats &&
			  HasLegacyLiquidName(
				  race.SweatLiquidId,
				  GetRaceSweatLiquidName(template.Name),
				  GetLegacyRaceSweatLiquidName(template),
				  GetRaceSweatMaterialName(template.Name),
				  GetLegacyRaceSweatMaterialName(template)))));

		bool HasLegacyLiquidName(
			long? liquidId,
			string expectedLiquidName,
			string legacyLiquidName,
			string expectedResidueName,
			string legacyResidueName)
		{
			if (legacyLiquidName.Equals(expectedLiquidName, StringComparison.OrdinalIgnoreCase) &&
			    legacyResidueName.Equals(expectedResidueName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (liquidId is null || !liquidsById.TryGetValue(liquidId.Value, out var liquid))
			{
				return false;
			}

			if (!legacyLiquidName.Equals(expectedLiquidName, StringComparison.OrdinalIgnoreCase) &&
			    liquid.Name.Equals(legacyLiquidName, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return !legacyResidueName.Equals(expectedResidueName, StringComparison.OrdinalIgnoreCase) &&
			       liquid.DriedResidueId is not null &&
			       materialsById.TryGetValue(liquid.DriedResidueId.Value, out var residue) &&
			       residue.Name.Equals(legacyResidueName, StringComparison.OrdinalIgnoreCase);
		}
	}

	private static void RepairLegacyAnimalFluidNames(FuturemudDatabaseContext context)
	{
		var racesByName = context.Races
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var liquids = context.Liquids.ToList();
		var liquidsById = liquids
			.ToDictionary(x => x.Id);
		var materials = context.Materials.ToList();
		var materialsById = materials
			.ToDictionary(x => x.Id);
		var dirty = false;

		foreach (var template in RaceTemplates.Values)
		{
			if (!racesByName.TryGetValue(template.Name, out var race))
			{
				continue;
			}

			RepairLiquidName(
				race.BloodLiquidId,
				GetRaceBloodLiquidName(template.Name),
				GetLegacyRaceBloodLiquidName(template),
				GetRaceBloodMaterialName(template.Name),
				GetLegacyRaceBloodMaterialName(template));

			if (template.Sweats)
			{
				RepairLiquidName(
					race.SweatLiquidId,
					GetRaceSweatLiquidName(template.Name),
					GetLegacyRaceSweatLiquidName(template),
					GetRaceSweatMaterialName(template.Name),
					GetLegacyRaceSweatMaterialName(template));
			}
		}

		if (dirty)
		{
			context.SaveChanges();
		}

		void RepairLiquidName(
			long? liquidId,
			string expectedLiquidName,
			string legacyLiquidName,
			string expectedResidueName,
			string legacyResidueName)
		{
			if (liquidId is null || !liquidsById.TryGetValue(liquidId.Value, out var liquid))
			{
				return;
			}

			if (!legacyLiquidName.Equals(expectedLiquidName, StringComparison.OrdinalIgnoreCase) &&
			    liquid.Name.Equals(legacyLiquidName, StringComparison.OrdinalIgnoreCase) &&
			    liquids.All(x => x.Id == liquid.Id ||
			                     !x.Name.Equals(expectedLiquidName, StringComparison.OrdinalIgnoreCase)))
			{
				liquid.Name = expectedLiquidName;
				dirty = true;
			}

			if (liquid.DriedResidueId is null ||
			    !materialsById.TryGetValue(liquid.DriedResidueId.Value, out var residue))
			{
				return;
			}

			if (!legacyResidueName.Equals(expectedResidueName, StringComparison.OrdinalIgnoreCase) &&
			    residue.Name.Equals(legacyResidueName, StringComparison.OrdinalIgnoreCase) &&
			    materials.All(x => x.Id == residue.Id ||
			                       !x.Name.Equals(expectedResidueName, StringComparison.OrdinalIgnoreCase)))
			{
				residue.Name = expectedResidueName;
				dirty = true;
			}
		}
	}
}
