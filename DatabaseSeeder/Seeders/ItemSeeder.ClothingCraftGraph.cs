#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record ClothingCraftGraphSnapshot(
		IReadOnlyCollection<GameItemProto> Items,
		IReadOnlyCollection<GameItemSkin> Skins,
		IReadOnlyCollection<Tag> Tags,
		IReadOnlyCollection<Material> Materials,
		IReadOnlyCollection<Liquid> Liquids,
		IReadOnlyCollection<CharacteristicDefinition> Definitions,
		IReadOnlyCollection<CharacteristicValue> Values,
		IReadOnlyCollection<TraitDefinition> Traits,
		IReadOnlyCollection<FutureProg> Progs);

	private static readonly Regex PersistedCraftEchoReference = new(
		@"\$(?<kind>[itpf])(?<id>\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

	private static void ValidatePreservedClothingCraftGraph(Craft craft, ClothingSourceLocation source,
		ClothingCraftGraphSnapshot snapshot)
	{
		if (string.IsNullOrWhiteSpace(craft.Name) || string.IsNullOrWhiteSpace(craft.Category) ||
			string.IsNullOrWhiteSpace(craft.Blurb) || string.IsNullOrWhiteSpace(craft.ActionDescription) ||
			string.IsNullOrWhiteSpace(craft.ActiveCraftItemSdesc))
			throw source.Error("Preserved clothing craft has incomplete authored presentation fields.");
		if (snapshot.Traits.Count(x => x.Id == craft.CheckTraitId) != 1)
			throw source.Error($"Preserved clothing craft has missing or ambiguous trait {craft.CheckTraitId}.");
		if (!double.IsFinite(craft.CheckQualityWeighting) || !double.IsFinite(craft.InputQualityWeighting) ||
			!double.IsFinite(craft.ToolQualityWeighting) || craft.CheckQualityWeighting < 0 ||
			craft.InputQualityWeighting < 0 || craft.ToolQualityWeighting < 0)
			throw source.Error("Preserved clothing craft has invalid quality weighting.");

		ValidateCraftProg(craft.AppearInCraftsListProgId, true, "appear", source, snapshot);
		ValidateCraftProg(craft.CanUseProgId, false, "can-use", source, snapshot);
		ValidateCraftProg(craft.WhyCannotUseProgId, false, "why-cannot-use", source, snapshot);
		ValidateCraftProg(craft.OnUseProgStartId, false, "on-start", source, snapshot, allowAnyReturn: true);
		ValidateCraftProg(craft.OnUseProgCompleteId, false, "on-finish", source, snapshot, allowAnyReturn: true);
		ValidateCraftProg(craft.OnUseProgCancelId, false, "on-cancel", source, snapshot, allowAnyReturn: true);

		var phases = craft.CraftPhases.OrderBy(x => x.PhaseNumber).ToArray();
		if (phases.Length == 0 || !phases.Select(x => x.PhaseNumber).SequenceEqual(Enumerable.Range(1, phases.Length)) ||
			craft.FailPhase < 1 || craft.FailPhase > phases.Length)
			throw source.Error("Preserved clothing craft requires consecutive one-based phases and a valid failure phase.");
		foreach (var phase in phases)
		{
			if (!double.IsFinite(phase.PhaseLengthInSeconds) || phase.PhaseLengthInSeconds <= 0 ||
				!double.IsFinite(phase.StaminaUsage) || phase.StaminaUsage < 0 ||
				string.IsNullOrWhiteSpace(phase.Echo) || string.IsNullOrWhiteSpace(phase.FailEcho))
				throw source.Error($"Preserved clothing craft phase {phase.PhaseNumber} has invalid timing, stamina or prose.");
			if (phase.PhaseNumber < craft.FailPhase && phase.Echo != phase.FailEcho)
				throw source.Error($"Preserved clothing craft phase {phase.PhaseNumber} changes its echo before failure is possible.");
		}

		var inputs = craft.CraftInputs.OrderBy(x => x.OriginalAdditionTime).ThenBy(x => x.Id).ToArray();
		var tools = craft.CraftTools.OrderBy(x => x.OriginalAdditionTime).ThenBy(x => x.Id).ToArray();
		var products = craft.CraftProducts.Where(x => !x.IsFailProduct)
			.OrderBy(x => x.OriginalAdditionTime).ThenBy(x => x.Id).ToArray();
		var failures = craft.CraftProducts.Where(x => x.IsFailProduct)
			.OrderBy(x => x.OriginalAdditionTime).ThenBy(x => x.Id).ToArray();
		if (inputs.Any(x => x.CraftId != craft.Id || x.CraftRevisionNumber != craft.RevisionNumber) ||
			 tools.Any(x => x.CraftId != craft.Id || x.CraftRevisionNumber != craft.RevisionNumber) ||
			 craft.CraftProducts.Any(x => x.CraftId != craft.Id || x.CraftRevisionNumber != craft.RevisionNumber))
			throw source.Error("Preserved clothing craft has a child row attached to another craft revision.");
		if (inputs.Select(x => x.Id).Distinct().Count() != inputs.Length || tools.Select(x => x.Id).Distinct().Count() != tools.Length ||
			craft.CraftProducts.Select(x => x.Id).Distinct().Count() != craft.CraftProducts.Count)
			throw source.Error("Preserved clothing craft repeats a child identity.");

		var inputVariables = inputs.Select(input => ValidatePersistedCraftInput(input, source, snapshot)).ToArray();
		foreach (var tool in tools) ValidatePersistedCraftTool(tool, source, snapshot);
		var productDependencies = products
			.Select(product => ValidatePersistedCraftProduct(product, inputs, inputVariables, source, snapshot))
			.ToArray();
		var failureDependencies = failures
			.Select(product => ValidatePersistedCraftProduct(product, inputs, inputVariables, source, snapshot))
			.ToArray();
		ValidatePersistedCraftPhaseGraph(craft, phases, inputs, tools, products, failures,
			productDependencies, failureDependencies, source);
	}

	private static void ValidateCraftProg(long? id, bool required, string label,
		ClothingSourceLocation source, ClothingCraftGraphSnapshot snapshot, bool allowAnyReturn = false)
	{
		if (id is null or 0)
		{
			if (required) throw source.Error($"Preserved clothing craft has no {label} program.");
			return;
		}
		var rows = snapshot.Progs.Where(x => x.Id == id).ToArray();
		if (rows.Length != 1) throw source.Error($"Preserved clothing craft has missing or ambiguous {label} program {id}.");
		if (!allowAnyReturn && label != "why-cannot-use" && rows[0].ReturnType != (long)ProgVariableTypes.Boolean)
			throw source.Error($"Preserved clothing craft {label} program must return Boolean.");
		if (!allowAnyReturn && label == "why-cannot-use" && rows[0].ReturnType != (long)ProgVariableTypes.Text)
			throw source.Error("Preserved clothing craft why-cannot-use program must return Text.");
		var prog = rows[0];
		if (!Enum.IsDefined(typeof(FutureProgStaticType), prog.StaticType))
			throw source.Error($"Preserved clothing craft {label} program has an invalid static execution mode.");
		if (prog.AcceptsAnyParameters) return;
		var parameters = prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex).ToArray();
		if (parameters.Length != 1 || parameters[0].ParameterIndex != 0 ||
			!ProgVariableTypes.Character.CompatibleWith((ProgVariableTypes)parameters[0].ParameterType))
			throw source.Error($"Preserved clothing craft {label} program must accept one character parameter.");
	}

	private static IReadOnlySet<long> ValidatePersistedCraftInput(CraftInput input, ClothingSourceLocation source,
		ClothingCraftGraphSnapshot snapshot)
	{
		if (!double.IsFinite(input.InputQualityWeight) || input.InputQualityWeight < 0)
			throw source.Error($"Preserved clothing craft input {input.Id} has invalid quality weighting.");
		var root = CraftXml(input.Definition, $"input {input.Id}", source);
		switch (input.InputType)
		{
			case "SimpleItem":
				RequireCurrentItem(CraftLong(root, "TargetItemId", source), snapshot, source);
				CraftPositiveInt(root, "Quantity", source);
				return new HashSet<long>();
			case "Tag":
				RequireTag(CraftLong(root, "TargetTagId", source), snapshot, source);
				CraftPositiveInt(root, "Quantity", source);
				return new HashSet<long>();
			case "TagVariable":
				RequireTag(CraftLong(root, "TargetTagId", source), snapshot, source);
				CraftPositiveInt(root, "Quantity", source);
				return ValidateVariableList(root, "Variable", snapshot, source, true);
			case "Commodity":
				RequireSolid(CraftLong(root, "Material", source), snapshot, source);
				RequireOptionalTag(CraftLong(root, "CommodityPileTag", source), snapshot, source);
				CraftPositiveDouble(root, "Weight", source);
				return ValidateCharacteristicRequirements(root.Element("Characteristics"), snapshot, source);
			case "CommodityTag":
				RequireTag(CraftLong(root, "MaterialTag", source), snapshot, source);
				RequireOptionalTag(CraftLong(root, "CommodityPileTag", source), snapshot, source);
				CraftPositiveDouble(root, "Weight", source);
				return ValidateCharacteristicRequirements(root.Element("Characteristics"), snapshot, source);
			case "LiquidUse":
				var liquid = CraftLong(root, "Liquid", source);
				if (snapshot.Liquids.Count(x => x.Id == liquid) != 1)
					throw source.Error($"Preserved clothing craft input {input.Id} has missing liquid {liquid}.");
				CraftPositiveDouble(root, "Amount", source);
				return new HashSet<long>();
			default:
				throw source.Error($"Preserved clothing craft uses unsupported input type {input.InputType}.");
		}
	}

	private static void ValidatePersistedCraftTool(CraftTool tool, ClothingSourceLocation source,
		ClothingCraftGraphSnapshot snapshot)
	{
		if (tool.ToolType != "TagTool") throw source.Error($"Preserved clothing craft uses unsupported tool type {tool.ToolType}.");
		if (!double.IsFinite(tool.ToolQualityWeight) || tool.ToolQualityWeight < 0 ||
			!Enum.IsDefined(typeof(DesiredItemState), tool.DesiredState) || tool.DesiredState == (int)DesiredItemState.Unknown)
			throw source.Error($"Preserved clothing craft tool {tool.Id} has invalid state or quality weighting.");
		var root = CraftXml(tool.Definition, $"tool {tool.Id}", source);
		RequireTag(CraftLong(root, "TargetItemTag", source), snapshot, source);
	}

	private static IReadOnlySet<int> ValidatePersistedCraftProduct(CraftProduct product, IReadOnlyList<CraftInput> inputs,
		IReadOnlyList<IReadOnlySet<long>> inputVariables, ClothingSourceLocation source,
		ClothingCraftGraphSnapshot snapshot)
	{
		var root = CraftXml(product.Definition, $"product {product.Id}", source);
		var dependencies = new HashSet<int>();
		if (product.MaterialDefiningInputIndex is { } materialIndex && (materialIndex < 0 || materialIndex >= inputs.Count))
			throw source.Error($"Preserved clothing craft product {product.Id} has an invalid material-defining input.");
		if (product.MaterialDefiningInputIndex is { } validMaterialIndex) dependencies.Add(validMaterialIndex + 1);
		switch (product.ProductType)
		{
			case "SimpleProduct":
			case "SimpleVariableProduct":
				var item = RequireCurrentItem(CraftLong(root, "ProductProducedId", source), snapshot, source);
				CraftPositiveInt(root, "Quantity", source);
				var skinId = CraftLong(root, "Skin", source);
				if (skinId != 0)
				{
					var skins = snapshot.Skins.Where(x => x.Id == skinId && x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
					if (skins.Length != 1 || skins[0].ItemProtoId != item.Id)
						throw source.Error($"Preserved clothing craft product {product.Id} has a missing, ambiguous or wrong-base skin.");
				}
				if (product.ProductType == "SimpleVariableProduct")
					dependencies.UnionWith(ValidateProductVariables(root, inputs, inputVariables, snapshot, source));
				else if (root.Elements("Variable").Any() || root.Elements("FixedVariable").Any())
					throw source.Error($"Preserved clothing craft product {product.Id} stores variables on a non-variable product.");
				break;
			case "CommodityProduct":
				RequireSolid(CraftLong(root, "Material", source), snapshot, source);
				CraftPositiveDouble(root, "Weight", source);
				RequireOptionalTag(CraftLong(root, "Tag", source), snapshot, source);
				dependencies.UnionWith(ValidateCommodityOutputs(root.Element("Characteristics"), inputs, inputVariables, snapshot, source));
				break;
			case "UnusedInput":
				var inputId = CraftLong(root, "WhichInputId", source);
				var inputIndex = inputs.Select((input, index) => (input, index)).SingleOrDefault(x => x.input.Id == inputId);
				if (inputIndex.input is null)
					throw source.Error($"Preserved clothing craft product {product.Id} has a missing recovered input {inputId}.");
				dependencies.Add(inputIndex.index + 1);
				var ratio = CraftDouble(root, "PercentageRecovered", source);
				if (ratio is <= 0 or > 1) throw source.Error($"Preserved clothing craft product {product.Id} has an invalid recovery fraction.");
				break;
			default:
				throw source.Error($"Preserved clothing craft uses unsupported product type {product.ProductType}.");
		}
		return dependencies;
	}

	private static IReadOnlySet<int> ValidateProductVariables(XElement root, IReadOnlyList<CraftInput> inputs,
		IReadOnlyList<IReadOnlySet<long>> inputVariables, ClothingCraftGraphSnapshot snapshot,
		ClothingSourceLocation source)
	{
		var seen = new HashSet<long>();
		var dependencies = new HashSet<int>();
		foreach (var element in root.Elements("Variable"))
		{
			var definition = CraftElementLong(element, source);
			RequireDefinition(definition, snapshot, source);
			if (!seen.Add(definition)) throw source.Error($"Preserved clothing craft product repeats characteristic {definition}.");
			var index = CraftAttributeInt(element, "inputindex", source);
			if (index < 0 || index >= inputs.Count || !inputVariables[index].Contains(definition))
				throw source.Error($"Preserved clothing craft product characteristic {definition} is not supplied by input {index + 1}.");
			dependencies.Add(index + 1);
		}
		foreach (var element in root.Elements("FixedVariable"))
		{
			var definition = CraftElementLong(element, source);
			RequireDefinition(definition, snapshot, source);
			if (!seen.Add(definition)) throw source.Error($"Preserved clothing craft product repeats characteristic {definition}.");
			RequireCompatibleValue(definition, CraftAttributeLong(element, "value", source), snapshot, source);
		}
		return dependencies;
	}

	private static IReadOnlySet<long> ValidateCharacteristicRequirements(XElement? element,
		ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		if (element is null) return new HashSet<long>();
		var mode = element.Attribute("mode")?.Value ?? "";
		if (mode is not ("" or "any" or "none" or "specific"))
			throw source.Error($"Preserved clothing craft has unsupported commodity characteristic mode {mode}.");
		var result = new HashSet<long>();
		foreach (var child in element.Elements("Characteristic"))
		{
			var definition = CraftAttributeLong(child, "definition", source);
			RequireDefinition(definition, snapshot, source);
			if (!result.Add(definition)) throw source.Error($"Preserved clothing craft repeats commodity characteristic {definition}.");
			var value = CraftAttributeLong(child, "value", source);
			if (value != 0) RequireCompatibleValue(definition, value, snapshot, source);
		}
		if (mode == "none" && result.Count > 0)
			throw source.Error("Preserved clothing craft combines no-characteristics mode with explicit requirements.");
		return result;
	}

	private static IReadOnlySet<int> ValidateCommodityOutputs(XElement? element, IReadOnlyList<CraftInput> inputs,
		IReadOnlyList<IReadOnlySet<long>> inputVariables, ClothingCraftGraphSnapshot snapshot,
		ClothingSourceLocation source)
	{
		if (element is null) return new HashSet<int>();
		var seen = new HashSet<long>();
		var dependencies = new HashSet<int>();
		foreach (var child in element.Elements("Characteristic"))
		{
			var definition = CraftAttributeLong(child, "definition", source);
			RequireDefinition(definition, snapshot, source);
			if (!seen.Add(definition)) throw source.Error($"Preserved clothing craft repeats commodity output characteristic {definition}.");
			var value = CraftAttributeLong(child, "value", source);
			var input = CraftAttributeInt(child, "input", source);
			if ((value > 0) == (input >= 0)) throw source.Error("Commodity output must select exactly one fixed value or source input.");
			if (value > 0) RequireCompatibleValue(definition, value, snapshot, source);
			if (input >= 0 && (input >= inputs.Count || !inputVariables[input].Contains(definition)))
				throw source.Error($"Commodity output characteristic {definition} is not supplied by input {input + 1}.");
			if (input >= 0) dependencies.Add(input + 1);
		}
		return dependencies;
	}

	private static void ValidatePersistedCraftPhaseGraph(Craft craft, IReadOnlyList<CraftPhase> phases,
		IReadOnlyList<CraftInput> inputs, IReadOnlyList<CraftTool> tools,
		IReadOnlyList<CraftProduct> products, IReadOnlyList<CraftProduct> failures,
		IReadOnlyList<IReadOnlySet<int>> productDependencies,
		IReadOnlyList<IReadOnlySet<int>> failureDependencies, ClothingSourceLocation source)
	{
		var consumed = new Dictionary<int, int>();
		var usedTools = new HashSet<int>();
		var produced = new Dictionary<int, int>();
		var failed = new Dictionary<int, int>();
		foreach (var phase in phases)
		{
			Read(phase.Echo, phase.PhaseNumber, false);
			Read(phase.FailEcho, phase.PhaseNumber, true);
		}
		if (Enumerable.Range(1, inputs.Count).Any(x => !consumed.ContainsKey(x)))
			throw source.Error("Preserved clothing craft does not consume every input in a success echo.");
		if (Enumerable.Range(1, tools.Count).Any(x => !usedTools.Contains(x)))
			throw source.Error("Preserved clothing craft does not use every tool in a success echo.");
		if (Enumerable.Range(1, products.Count).Any(x => !produced.ContainsKey(x)) ||
			Enumerable.Range(1, failures.Count).Any(x => !failed.ContainsKey(x)))
			throw source.Error("Preserved clothing craft does not expose every success and failure product in its phase prose.");
		ValidateDependencies(productDependencies, produced, "success");
		ValidateDependencies(failureDependencies, failed, "failure");

		void Read(string echo, int phase, bool failure)
		{
			foreach (Match match in PersistedCraftEchoReference.Matches(echo))
			{
				var index = int.Parse(match.Groups["id"].Value, CultureInfo.InvariantCulture);
				switch (match.Groups["kind"].Value.ToLowerInvariant())
				{
					case "i":
						if (index < 1 || index > inputs.Count) throw source.Error($"Preserved clothing craft phase {phase} refers to unknown input {match.Value}.");
						if (!failure) consumed.TryAdd(index, phase);
						else if (!consumed.TryGetValue(index, out var consumedAt) || consumedAt > phase)
							throw source.Error($"Preserved clothing craft failure phase {phase} refers to unconsumed input {match.Value}.");
						break;
					case "t":
						if (index < 1 || index > tools.Count) throw source.Error($"Preserved clothing craft phase {phase} refers to unknown tool {match.Value}.");
						if (!failure) usedTools.Add(index);
						break;
					case "p":
						if (index < 1 || index > products.Count) throw source.Error($"Preserved clothing craft phase {phase} refers to unknown product {match.Value}.");
						if (!failure) produced.TryAdd(index, phase);
						else if (!produced.TryGetValue(index, out var productAt) || productAt >= craft.FailPhase || productAt > phase)
							throw source.Error($"Preserved clothing craft failure phase {phase} refers to an unavailable success product {match.Value}.");
						break;
					case "f":
						if (!failure || phase < craft.FailPhase || index < 1 || index > failures.Count)
							throw source.Error($"Preserved clothing craft phase {phase} has invalid failure product {match.Value}.");
						failed.TryAdd(index, phase);
						break;
				}
			}
		}

		void ValidateDependencies(IReadOnlyList<IReadOnlySet<int>> dependencies,
			IReadOnlyDictionary<int, int> productionPhases, string label)
		{
			for (var index = 1; index <= dependencies.Count; index++)
			{
				var producedAt = productionPhases[index];
				foreach (var input in dependencies[index - 1])
					if (!consumed.TryGetValue(input, out var consumedAt) || consumedAt > producedAt)
						throw source.Error($"Preserved clothing craft {label} product {index} needs input $i{input} before that input is consumed.");
			}
		}
	}

	private static XElement CraftXml(string xml, string label, ClothingSourceLocation source)
	{
		try
		{
			var root = XElement.Parse(xml);
			return root.Name == "Definition" ? root : throw source.Error($"Preserved clothing craft {label} has a non-Definition XML root.");
		}
		catch (XmlException ex) { throw source.Error($"Preserved clothing craft {label} has invalid XML: {ex.Message}"); }
	}

	private static long CraftLong(XElement root, string name, ClothingSourceLocation source)
	{
		var rows = root.Elements(name).ToArray();
		return rows.Length == 1 && long.TryParse(rows[0].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
			? value : throw source.Error($"Preserved clothing craft requires one valid {name} value.");
	}

	private static double CraftDouble(XElement root, string name, ClothingSourceLocation source)
	{
		var rows = root.Elements(name).ToArray();
		return rows.Length == 1 && double.TryParse(rows[0].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && double.IsFinite(value)
			? value : throw source.Error($"Preserved clothing craft requires one finite {name} value.");
	}

	private static void CraftPositiveInt(XElement root, string name, ClothingSourceLocation source)
	{
		var value = CraftLong(root, name, source);
		if (value <= 0 || value > int.MaxValue) throw source.Error($"Preserved clothing craft requires a positive {name} value.");
	}

	private static void CraftPositiveDouble(XElement root, string name, ClothingSourceLocation source)
	{
		if (CraftDouble(root, name, source) <= 0) throw source.Error($"Preserved clothing craft requires a positive {name} value.");
	}

	private static long CraftElementLong(XElement element, ClothingSourceLocation source) =>
		long.TryParse(element.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
			? value : throw source.Error("Preserved clothing craft has an invalid characteristic definition ID.");

	private static long CraftAttributeLong(XElement element, string name, ClothingSourceLocation source) =>
		long.TryParse(element.Attribute(name)?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
			? value : throw source.Error($"Preserved clothing craft has an invalid {name} attribute.");

	private static int CraftAttributeInt(XElement element, string name, ClothingSourceLocation source) =>
		int.TryParse(element.Attribute(name)?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
			? value : throw source.Error($"Preserved clothing craft has an invalid {name} attribute.");

	private static GameItemProto RequireCurrentItem(long id, ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		var rows = snapshot.Items.Where(x => x.Id == id && x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
		return rows.Length == 1 ? rows[0] : throw source.Error($"Preserved clothing craft has missing or ambiguous current item {id}.");
	}

	private static void RequireTag(long id, ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		if (id <= 0 || snapshot.Tags.Count(x => x.Id == id) != 1) throw source.Error($"Preserved clothing craft has missing tag {id}.");
	}

	private static void RequireOptionalTag(long id, ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		if (id != 0) RequireTag(id, snapshot, source);
	}

	private static void RequireSolid(long id, ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		if (id <= 0 || snapshot.Materials.Count(x => x.Id == id && x.Type == (int)MaterialType.Solid) != 1)
			throw source.Error($"Preserved clothing craft has missing solid material {id}.");
	}

	private static void RequireDefinition(long id, ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		if (id <= 0 || snapshot.Definitions.Count(x => x.Id == id) != 1)
			throw source.Error($"Preserved clothing craft has missing characteristic definition {id}.");
	}

	private static IReadOnlySet<long> ValidateVariableList(XElement root, string name,
		ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source, bool required)
	{
		var ids = root.Elements(name).Select(x => CraftElementLong(x, source)).ToArray();
		if (required && ids.Length == 0) throw source.Error("Preserved variable-aware craft input declares no characteristics.");
		if (ids.Distinct().Count() != ids.Length) throw source.Error("Preserved clothing craft repeats a characteristic definition.");
		foreach (var id in ids) RequireDefinition(id, snapshot, source);
		return ids.ToHashSet();
	}

	private static void RequireCompatibleValue(long definitionId, long valueId,
		ClothingCraftGraphSnapshot snapshot, ClothingSourceLocation source)
	{
		var value = snapshot.Values.SingleOrDefault(x => x.Id == valueId) ??
			throw source.Error($"Preserved clothing craft has missing characteristic value {valueId}.");
		var current = snapshot.Definitions.Single(x => x.Id == definitionId);
		var visited = new HashSet<long>();
		while (true)
		{
			if (!visited.Add(current.Id)) throw source.Error("Preserved clothing craft characteristic ancestry is cyclic.");
			if (value.DefinitionId == current.Id) return;
			if (current.ParentId is not { } parent) break;
			current = snapshot.Definitions.SingleOrDefault(x => x.Id == parent) ??
				throw source.Error($"Preserved clothing craft has missing parent characteristic definition {parent}.");
		}
		throw source.Error($"Characteristic value {valueId} is incompatible with definition {definitionId}.");
	}
}
