#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DatabaseSeeder.Seeders;

namespace DatabaseSeeder;

/// <summary>One derived audit row per authored clothing source row, including graph children.</summary>
internal static class IndustrialisedClothingAudit
{
	internal const string RelativePath = "Design Documents/Seeding/Industrialised_Clothing_Catalogue_Audit.tsv";
	internal const string Header = "RecordType\tRecordKey\tSourceFile\tSourceLine\tEraAdmissions\tItemReference\tSkinReference\tCraftReference\tOutfitReference\tDependencies\tSourceRecord\tResolvedColourSelections\tValidation\tCatalogueSourceSha256";
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		Converters = { new JsonStringEnumConverter() }
	};

	internal static string Generate(IndustrialisedClothingCatalogueDocument document, string sourceHash)
	{
		IndustrialisedClothingCatalogue.ValidateStructure(document);
		foreach (var craft in document.Crafts) IndustrialisedClothingCraftPlan.Compile(document, craft);
		var bases = document.Bases.ToDictionary(x => x.ItemReference, StringComparer.Ordinal);
		var skins = document.Skins.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var crafts = document.Crafts.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var outfits = document.Outfits.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var rows = new List<(ClothingSourceLocation Source, string Key, string Text)>();

		foreach (var row in document.Bases)
			Add("base", row.ItemReference, row.Source, row, row.EraAdmissions, row.ItemReference,
				dependencies: [$"item:{row.ItemReference}"], selections: Defaults(row.ItemReference, ""));
		foreach (var row in document.Skins)
			Add("skin", row.StableReference, row.Source, row, row.EraAdmissions, row.BaseItemReference, row.StableReference,
				dependencies: [$"item:{row.BaseItemReference}"], selections: Defaults(row.BaseItemReference, row.StableReference));
		foreach (var row in document.Colours)
		{
			var skin = skins.GetValueOrDefault(row.PresentationReference);
			var item = skin?.BaseItemReference ?? row.PresentationReference;
			Add("colour", $"{row.PresentationReference}/{row.Variable}", row.Source, row,
				skin?.EraAdmissions ?? bases[item].EraAdmissions, item, skin?.StableReference ?? "",
				dependencies: [$"{(skin is null ? "item" : "item-skin")}:{row.PresentationReference}",
					$"characteristic:{row.Definition}", $"characteristic-profile:{row.Profile}"],
				selections: new SortedDictionary<string, string>(StringComparer.Ordinal) { [row.Variable] = row.DefaultValue });
		}
		foreach (var row in document.Palettes)
		{
			var consumers = document.OutfitEntries.Where(x => x.Palette == row.Palette).ToArray();
			var admittedEras = consumers.SelectMany(x => outfits[x.OutfitReference].EraAdmissions).ToHashSet(StringComparer.Ordinal);
			Add("palette", $"{row.Palette}/{row.Variable}", row.Source, row,
				new[] { "industrial", "modern", "nuclear", "information" }.Where(admittedEras.Contains),
				selections: new SortedDictionary<string, string>(StringComparer.Ordinal) { [row.Variable] = row.Value });
		}
		foreach (var row in document.Outfits)
			Add("outfit", row.StableReference, row.Source, row, row.EraAdmissions, outfit: row.StableReference,
				dependencies: document.OutfitEntries.Where(x => x.OutfitReference == row.StableReference)
					.Select(x => $"outfit-entry:{row.StableReference}/{x.EntryKey}"));
		foreach (var row in document.OutfitEntries)
			Add("outfit-entry", $"{row.OutfitReference}/{row.EntryKey}", row.Source, row,
				outfits[row.OutfitReference].EraAdmissions, row.ItemReference, row.SkinReference, outfit: row.OutfitReference,
				dependencies: new[] { $"outfit:{row.OutfitReference}", $"item:{row.ItemReference}" }
					.Concat(Optional("item-skin", row.SkinReference)).Concat(Optional("wear-profile", row.WearProfile))
					.Concat(Optional("palette", row.Palette)).Concat(row.ContainerKey.Length == 0 ? [] :
						new[] { $"outfit-entry:{row.OutfitReference}/{row.ContainerKey}" }),
				selections: IndustrialisedClothingColourPlan.OutfitValues(document, row));
		foreach (var row in document.OutfitColours)
			Add("outfit-colour", $"{row.OutfitReference}/{row.EntryKey}/{row.Variable}", row.Source, row,
				outfits[row.OutfitReference].EraAdmissions, outfit: row.OutfitReference,
				dependencies: [$"outfit-entry:{row.OutfitReference}/{row.EntryKey}"],
				selections: new SortedDictionary<string, string>(StringComparer.Ordinal) { [row.Variable] = row.Value });
		foreach (var row in document.Crafts)
			Add("craft", row.StableReference, row.Source, row, row.EraAdmissions, craft: row.StableReference,
				dependencies: new[] { $"trait:{row.Trait}" }
					.Concat(document.CraftInputs.Where(x => x.CraftReference == row.StableReference).Select(x => $"craft-input:{row.StableReference}/{Number(x.Order)}"))
					.Concat(document.CraftTools.Where(x => x.CraftReference == row.StableReference).Select(x => $"craft-tool:{row.StableReference}/{Number(x.Order)}"))
					.Concat(document.CraftProducts.Where(x => x.CraftReference == row.StableReference).Select(x => $"craft-product:{ProductKey(x.CraftReference, x.FailureProduct, x.Order)}")));
		foreach (var row in document.CraftPhases)
			Add("craft-phase", $"{row.CraftReference}/{Number(row.Order)}", row.Source, row, crafts[row.CraftReference].EraAdmissions,
				craft: row.CraftReference, dependencies: [$"craft:{row.CraftReference}"]);
		foreach (var row in document.CraftInputs)
		{
			var kind = row.Kind switch
			{
				ClothingInputKind.Item => "item", ClothingInputKind.Commodity => "material",
				ClothingInputKind.Liquid => "liquid", _ => "tag"
			};
			Add("craft-input", $"{row.CraftReference}/{Number(row.Order)}", row.Source, row, crafts[row.CraftReference].EraAdmissions,
				item: row.Kind == ClothingInputKind.Item ? row.Reference : "", craft: row.CraftReference,
				dependencies: [$"craft:{row.CraftReference}", $"{kind}:{row.Reference}"]);
		}
		foreach (var row in document.CraftTools)
			Add("craft-tool", $"{row.CraftReference}/{Number(row.Order)}", row.Source, row, crafts[row.CraftReference].EraAdmissions,
				craft: row.CraftReference, dependencies: [$"craft:{row.CraftReference}", $"tag:{row.Tag}"]);
		foreach (var row in document.CraftProducts)
		{
			var target = row.Kind switch
			{
				ClothingProductKind.Item => $"item:{row.Reference}", ClothingProductKind.Commodity => $"material:{row.Reference}",
				_ => $"craft-input:{row.CraftReference}/{row.Reference}"
			};
			Add("craft-product", ProductKey(row.CraftReference, row.FailureProduct, row.Order), row.Source, row,
				crafts[row.CraftReference].EraAdmissions, row.Kind == ClothingProductKind.Item ? row.Reference : "", row.SkinReference,
				row.CraftReference, dependencies: new[] { $"craft:{row.CraftReference}", target }
					.Concat(Optional("item-skin", row.SkinReference))
					.Concat(row.MaterialInputOrder.HasValue ? new[] { $"craft-input:{row.CraftReference}/{Number(row.MaterialInputOrder.Value)}" } : [])
					.Concat(document.CraftColours.Where(x => x.CraftReference == row.CraftReference && x.ProductOrder == row.Order &&
						x.FailureProduct == row.FailureProduct && x.InputOrder.HasValue).Select(x => $"craft-input:{row.CraftReference}/{Number(x.InputOrder!.Value)}")),
				selections: row.Kind == ClothingProductKind.Item ? IndustrialisedClothingColourPlan.CraftValues(document, row)
					.OrderBy(x => x.Key, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal) : null);
		}
		foreach (var row in document.CraftColours)
			Add("craft-colour", $"{ProductKey(row.CraftReference, row.FailureProduct, row.ProductOrder)}/{row.Variable}",
				row.Source, row, crafts[row.CraftReference].EraAdmissions, craft: row.CraftReference,
				dependencies: new[] { $"craft-product:{ProductKey(row.CraftReference, row.FailureProduct, row.ProductOrder)}" }
					.Concat(row.InputOrder.HasValue ? new[] { $"craft-input:{row.CraftReference}/{Number(row.InputOrder.Value)}" } : []));

		var output = new StringBuilder(Header).Append('\n');
		foreach (var row in rows.OrderBy(x => x.Source.File, StringComparer.Ordinal).ThenBy(x => x.Source.Line).ThenBy(x => x.Key, StringComparer.Ordinal))
			output.Append(row.Text).Append('\n');
		return output.ToString();

		SortedDictionary<string, string> Defaults(string item, string skin) => new(
			IndustrialisedClothingColourPlan.Channels(document, item, skin).ToDictionary(x => x.Key, x => x.Value.DefaultValue), StringComparer.Ordinal);

		void Add<T>(string kind, string key, ClothingSourceLocation source, T record, IEnumerable<string> admissions,
			string item = "", string skin = "", string craft = "", string outfit = "", IEnumerable<string>? dependencies = null, object? selections = null)
		{
			rows.Add((source, $"{kind}:{key}", string.Join('\t', new[]
			{
				kind, key, source.File, Number(source.Line), string.Join(';', admissions), item, skin, craft, outfit,
				JsonSerializer.Serialize((dependencies ?? []).Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal), JsonOptions),
				JsonSerializer.Serialize(record, JsonOptions), selections is null ? "" : JsonSerializer.Serialize(selections, JsonOptions),
				"structure-validated;database-unverified;production-unreviewed", sourceHash
			})));
		}
	}

	private static IEnumerable<string> Optional(string kind, string reference) => reference.Length == 0 ? [] : new[] { $"{kind}:{reference}" };
	private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);
	private static string ProductKey(string craft, bool failure, int order) => $"{craft}/{(failure ? "failure" : "success")}/{Number(order)}";
}
