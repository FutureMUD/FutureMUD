#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseSeeder.Seeders;

/// <summary>Compiles complete authored recipes to the existing craft contract without rewriting prose.</summary>
internal static class IndustrialisedClothingCraftPlan
{
	private static readonly Regex EchoReference = new(@"\$(?<kind>[itpf])(?<id>\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

	internal static ItemSeeder.CraftDefinitionSpec Compile(IndustrialisedClothingCatalogueDocument document, ClothingCraftRow craft)
	{
		var phases = document.CraftPhases.Where(x => x.CraftReference == craft.StableReference).OrderBy(x => x.Order).ToArray();
		var inputs = document.CraftInputs.Where(x => x.CraftReference == craft.StableReference).OrderBy(x => x.Order).ToArray();
		var tools = document.CraftTools.Where(x => x.CraftReference == craft.StableReference).OrderBy(x => x.Order).ToArray();
		var products = document.CraftProducts.Where(x => x.CraftReference == craft.StableReference)
			.OrderBy(x => x.FailureProduct).ThenBy(x => x.Order).ToArray();
		ValidatePhaseGraph(craft, phases, inputs, tools, products, document.CraftColours);
		var inherited = products.Where(x => x.Kind == ClothingProductKind.Item)
			.SelectMany(product => IndustrialisedClothingColourPlan.CraftValues(document, product).Values
				.Where(x => x.InputOrder.HasValue)
				.Select(colour => (Order: colour.InputOrder!.Value,
					Definition: IndustrialisedClothingColourPlan.Channels(document, product.Reference, product.SkinReference)[colour.Variable].Definition)))
			.Distinct().ToLookup(x => x.Order, x => x.Definition);

		var inputSpecs = inputs.Select(input =>
		{
			var definitions = inherited[input.Order].OrderBy(x => x, StringComparer.Ordinal).ToArray();
			if (definitions.Length > 0 && input.Kind is ClothingInputKind.Item or ClothingInputKind.Liquid)
				throw input.Source.Error("This input kind cannot transmit characteristics. Use a variable-aware tagged item or characterised commodity input.");
			var quantity = Quantity(input.Quantity, input.Source, input.Kind is ClothingInputKind.Item or ClothingInputKind.Tag);
			var options = input.Kind is ClothingInputKind.Commodity or ClothingInputKind.CommodityTag
				? definitions.Select(x => $"characteristic {x} any").ToArray() : [];
			var (type, details) = input.Kind switch
			{
				ClothingInputKind.Item => ("SimpleItem", $"{quantity}x @{input.Reference}"),
				ClothingInputKind.Tag when definitions.Length > 0 => ("TagVariable", $"{quantity}x an item with the {input.Reference} tag with variables {string.Join(",", definitions)}"),
				ClothingInputKind.Tag => ("Tag", $"{quantity}x an item with the {input.Reference} tag"),
				ClothingInputKind.Commodity => ("Commodity", $"{quantity} grams of {input.Reference}"),
				ClothingInputKind.CommodityTag => ("CommodityTag", $"{quantity} grams of a material tagged as {input.Reference}"),
				ClothingInputKind.Liquid => ("LiquidUse", $"{quantity} millilitres of {input.Reference}"),
				_ => throw input.Source.Error("Unknown craft input kind.")
			};
			return new ItemSeeder.CraftInputSpec(Import(type, details, options), type, details, Array.AsReadOnly(options), input.QualityWeight);
		}).ToArray();

		var toolSpecs = tools.Select(tool =>
		{
			var details = $"{tool.Placement} - an item with the {tool.Tag} tag";
			return new ItemSeeder.CraftToolSpec(Import("TagTool", details, []), "TagTool", details, [], tool.QualityWeight, tool.UseToolDuration);
		}).ToArray();

		var productSpecs = products.Select(product =>
		{
			var options = new List<string>();
			string type;
			string details;
			switch (product.Kind)
			{
				case ClothingProductKind.Item:
					type = "SimpleVariableProduct";
					details = $"{Quantity(product.Quantity, product.Source, true)}x @{product.Reference}";
					if (product.SkinReference.Length > 0) options.Add($"skin @{product.SkinReference}");
					var channels = IndustrialisedClothingColourPlan.Channels(document, product.Reference, product.SkinReference);
					foreach (var (variable, colour) in IndustrialisedClothingColourPlan.CraftValues(document, product).OrderBy(x => x.Key, StringComparer.Ordinal))
					{
						var definition = channels[variable].Definition;
						options.Add(colour.InputOrder is { } input
							? $"variable {definition}=$i{input.ToString(CultureInfo.InvariantCulture)}"
							: $"fixedvariable {definition}={colour.Value}");
					}
					break;
				case ClothingProductKind.Commodity:
					type = "CommodityProduct";
					details = $"{Quantity(product.Quantity, product.Source)} grams of {product.Reference} commodity";
					break;
				case ClothingProductKind.UnusedInput:
					var index = int.Parse(product.Reference, CultureInfo.InvariantCulture);
					var inputRow = inputs.Single(x => x.Order == index);
					if (inputRow.Kind is not (ClothingInputKind.Item or ClothingInputKind.Tag) ||
						inputRow.Kind == ClothingInputKind.Tag && inputRow.Quantity > 1)
						throw product.Source.Error("UnusedInput recovers item counts, not liquid/commodity mass or heterogeneous tagged batches. Declare an honest commodity product or an exact item input.");
					type = "UnusedInput";
					// Scale the authored decimal fraction, not its binary floating-point approximation.
					var percentage = decimal.Parse(Quantity(product.Quantity, product.Source), CultureInfo.InvariantCulture) * 100m;
					details = $"{percentage.ToString("0.################", CultureInfo.InvariantCulture)}% of {inputSpecs[index - 1].Details} ($i{index.ToString(CultureInfo.InvariantCulture)})";
					break;
				default: throw product.Source.Error("Unknown craft product kind.");
			}
			return new ItemSeeder.CraftProductSpec(Import(type, details, options), type, details, options.AsReadOnly(),
				product.FailureProduct, product.MaterialInputOrder - 1);
		}).ToArray();

		return new ItemSeeder.CraftDefinitionSpec
		{
			StableReference = craft.StableReference,
			PreserveAuthoredText = true,
			Name = craft.Name,
			Category = craft.Category,
			Blurb = craft.Blurb,
			Action = craft.Action,
			ActiveCraftItemSdesc = craft.ActiveItemDescription,
			Difficulty = craft.Difficulty,
			Threshold = craft.FailureThreshold,
			FreeChecks = craft.FreeChecks,
			FailPhase = craft.FailPhase,
			Interruptable = craft.Interruptable,
			Phases = Array.AsReadOnly(phases.Select(x => new ItemSeeder.CraftPhaseSpec { Seconds = x.Seconds, Echo = x.Echo, FailEcho = x.FailEcho }).ToArray()),
			Inputs = Array.AsReadOnly(inputSpecs),
			Tools = Array.AsReadOnly(toolSpecs),
			Products = Array.AsReadOnly(productSpecs.Where(x => !x.IsFailProduct).ToArray()),
			FailProducts = Array.AsReadOnly(productSpecs.Where(x => x.IsFailProduct).ToArray())
		};
	}

	private static string Import(string type, string details, IEnumerable<string> options) => string.Join("; ", new[] { $"{type} - {details}" }.Concat(options));

	private static string Quantity(double value, ClothingSourceLocation source, bool count = false)
	{
		if (!double.IsFinite(value) || value <= 0 || count && (value != Math.Truncate(value) || value > int.MaxValue))
			throw source.Error("Craft quantities must be finite and positive; item counts must fit a positive integer.");
		var result = value.ToString("0.################", CultureInfo.InvariantCulture);
		if (double.Parse(result, CultureInfo.InvariantCulture) != value)
			throw source.Error("Craft quantity exceeds the precision of the existing physical-unit import grammar.");
		return result;
	}

	private static void ValidatePhaseGraph(ClothingCraftRow craft, IReadOnlyList<ClothingCraftPhaseRow> phases,
		IReadOnlyList<ClothingCraftInputRow> inputs, IReadOnlyList<ClothingCraftToolRow> tools,
		IReadOnlyList<ClothingCraftProductRow> products, IReadOnlyList<ClothingCraftColourRow> colours)
	{
		var consumed = new Dictionary<int, int>();
		var produced = new Dictionary<(bool Failure, int Order), int>();
		var usedTools = new HashSet<int>();
		foreach (var phase in phases)
		{
			if (phase.Order < craft.FailPhase && phase.Echo != phase.FailEcho)
				throw phase.Source.Error("Pre-failure success and failure echoes must be identical authored text.");
			ReadEcho(phase, phase.Echo, false);
		}
		foreach (var phase in phases) ReadEcho(phase, phase.FailEcho, true);
		foreach (var input in inputs)
			if (!consumed.ContainsKey(input.Order)) throw input.Source.Error("Every craft input must appear explicitly in a success echo to declare consumption timing.");
		foreach (var tool in tools)
			if (!usedTools.Contains(tool.Order)) throw tool.Source.Error("Every craft tool must appear naturally in an authored success echo; text is never amended by the adapter.");
		foreach (var product in products)
		{
			if (!produced.TryGetValue((product.FailureProduct, product.Order), out var phase))
				throw product.Source.Error("Every product must appear explicitly in its success/failure echoes; implicit production phases are not accepted.");
			var dependencies = colours.Where(x => x.CraftReference == craft.StableReference && x.ProductOrder == product.Order &&
				x.FailureProduct == product.FailureProduct && x.InputOrder.HasValue).Select(x => x.InputOrder!.Value)
				.Concat(product.MaterialInputOrder.HasValue ? new[] { product.MaterialInputOrder.Value } : [])
				.Concat(product.Kind == ClothingProductKind.UnusedInput ? new[] { int.Parse(product.Reference, CultureInfo.InvariantCulture) } : []);
			foreach (var input in dependencies.Distinct())
				if (!consumed.TryGetValue(input, out var inputPhase) || inputPhase > phase)
					throw product.Source.Error($"Product needs input $i{input} before that input is consumed.");
		}

		void ReadEcho(ClothingCraftPhaseRow phase, string echo, bool failure)
		{
			foreach (Match match in EchoReference.Matches(echo))
			{
				if (!int.TryParse(match.Groups["id"].Value, out var index) || index <= 0)
					throw phase.Source.Error($"Invalid craft echo reference {match.Value}.");
				switch (match.Groups["kind"].Value.ToLowerInvariant())
				{
					case "i":
						if (index > inputs.Count) throw phase.Source.Error($"Unknown input {match.Value}.");
						if (!failure) consumed.TryAdd(index, phase.Order);
						else if (!consumed.TryGetValue(index, out var at) || at > phase.Order)
							throw phase.Source.Error($"Failure echo refers to unconsumed input {match.Value}.");
						break;
					case "t":
						if (index > tools.Count) throw phase.Source.Error($"Unknown tool {match.Value}.");
						if (!failure) usedTools.Add(index);
						break;
					case "p":
						if (!products.Any(x => !x.FailureProduct && x.Order == index)) throw phase.Source.Error($"Unknown product {match.Value}.");
						if (!failure) produced.TryAdd((false, index), phase.Order);
						else if (!produced.TryGetValue((false, index), out var productPhase) || productPhase >= craft.FailPhase || productPhase > phase.Order)
							throw phase.Source.Error($"Failure echo cannot refer to an unproduced success product {match.Value}.");
						break;
					case "f":
						if (!failure || phase.Order < craft.FailPhase || !products.Any(x => x.FailureProduct && x.Order == index))
							throw phase.Source.Error($"Invalid failure product reference {match.Value}.");
						produced.TryAdd((true, index), phase.Order);
						break;
				}
			}
		}
	}
}
