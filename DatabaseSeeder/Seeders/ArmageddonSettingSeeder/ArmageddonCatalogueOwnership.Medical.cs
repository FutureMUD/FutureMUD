#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonCatalogueOwnership
{
	private static IReadOnlyDictionary<Type, string[]> MedicalOwnedFields(IReadOnlyDictionary<Type, string[]> existing)
	{
		var fields = existing.ToDictionary(x => x.Key, x => x.Value);
		fields.Add(typeof(WearProfile), ["Name", "BodyPrototypeId", "WearStringInventory", "WearAction1st", "WearAction3rd", "WearAffix", "WearlocProfiles", "Type", "Description", "RequireContainerIsEmpty"]);
		fields.Add(typeof(Drug), ["Name", "DrugVectors", "IntensityPerGram", "RelativeMetabolisationRate"]);
		return fields;
	}

	private void LoadMedicalChildren(object row)
	{
		if (row is Drug drug) _db.Entry(drug).Collection(x => x.DrugsIntensities).Load();
	}

	private static void MedicalFields(object row, Dictionary<string, string> fields)
	{
		if (row is Drug drug) fields["DrugEffects"] = JsonSerializer.Serialize(drug.DrugsIntensities.OrderBy(x => x.DrugType)
			.Select(x => new DrugEffect(x.DrugType, x.RelativeIntensity, x.AdditionalEffects)).ToArray());
	}

	private void ApplyMedicalChildren(object row, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> merged)
	{
		if (row is not Drug drug || current["DrugEffects"] == merged["DrugEffects"]) return;
		Replace(drug.DrugsIntensities, JsonSerializer.Deserialize<DrugEffect[]>(merged["DrugEffects"])!.Select(x =>
			new DrugIntensity { DrugId = drug.Id, DrugType = x.Type, RelativeIntensity = x.Intensity, AdditionalEffects = x.Additional }),
			x => x.DrugType, (actual, next) => { actual.RelativeIntensity = next.RelativeIntensity; actual.AdditionalEffects = next.AdditionalEffects; });
	}

	private sealed record DrugEffect(int Type, double Intensity, string Additional);
}
