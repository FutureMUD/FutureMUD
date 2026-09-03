#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private IReadOnlyCollection<FutureProg>? _traitGateProgRows;

	private Craft? ResolveOwnedCraftTarget(string stableKey, string name, string category, IReadOnlyCollection<Craft> rows)
	{
		var managed = FindManagedRecord("craft", stableKey);
		var key = CraftLookupKey(name, category);
		var named = rows.Where(x => CraftLookupKey(x.Name, x.Category).Equals(key, StringComparison.OrdinalIgnoreCase)).ToArray();
		var currentNamed = named.Where(x => x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
		if (currentNamed.Length > 1)
			throw new InvalidOperationException($"Missing or ambiguous current craft {stableKey} for name/category {name} / {category}.");
		var existing = currentNamed.SingleOrDefault();
		if (managed?.LogicalId is { } id)
		{
			var owned = rows.Where(x => x.Id == id).ToArray();
			var current = owned.Where(x => x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
			if (current.Length > 1)
				throw new InvalidOperationException($"Missing or ambiguous current craft {stableKey} on owned ID {id}.");
			if (owned.Length > 0 && current.Length == 0)
				throw new InvalidOperationException($"Owned craft {stableKey} has no current revision; resolve its revision state before installing clothing.");
			if (existing is not null && existing.Id != id)
				throw new InvalidOperationException($"ItemSeeder ownership conflict for craft:{stableKey}: provenance names ID {id}, but the name/category resolves to {existing.Id}.");
			existing = current.SingleOrDefault() ?? existing;
		}
		if (existing is null)
		{
			if (named.Length > 0)
				throw new InvalidOperationException($"Craft {stableKey} has no current revision; resolve its revision state before installing clothing.");
			return null;
		}
		var otherOwners = _managedRecordsByIdentity.Values.Where(x => x.EntityType.Equals("craft", StringComparison.OrdinalIgnoreCase) &&
			x.LogicalId == existing.Id && !x.StableKey.Equals(stableKey, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (otherOwners.Length > 0)
			throw new InvalidOperationException($"ItemSeeder ownership conflict for craft:{stableKey}: ID {existing.Id} is claimed by {string.Join(", ", otherOwners.Select(x => x.StableKey).OrderBy(x => x, StringComparer.Ordinal))}.");
		return existing;
	}

	private void ValidateUnmanagedCraftSignature(string stableKey, Craft? existing, CraftDefinitionSpec spec)
	{
		if (existing is null || FindManagedRecord("craft", stableKey) is not null) return;
		try
		{
			var live = ItemSeederManifestCatalogue.Fingerprint(BuildLiveCraftManifestDefinition(existing));
			var expected = ItemSeederManifestCatalogue.Fingerprint(BuildExpectedLiveCraftDefinition(existing, NormaliseCraftDefinition(spec)));
			if (!live.Equals(expected, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException("The complete stock signature does not match.");
		}
		catch (Exception ex) when (ex is InvalidOperationException or ApplicationException)
		{
			throw new InvalidOperationException($"Unmanaged craft conflict for '{stableKey}'. The complete stock signature cannot be proven; it will not be claimed or overwritten. {ex.Message}", ex);
		}
	}

	private static ProgManifestDefinition TraitGateLiveDefinition(FutureProg prog) => new(
		prog.FunctionName, prog.Category, prog.Subcategory, prog.ReturnType, prog.FunctionComment, prog.FunctionText,
		prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => new ProgParameterManifestDefinition(x.ParameterType, x.ParameterName)).ToArray());

	private FutureProg? ResolveTraitGateProgOwnership(ProgManifestDefinition definition, IReadOnlyCollection<FutureProg> rows)
	{
		var managed = FindManagedRecord("prog", definition.Name);
		var named = rows.Where(x => x.FunctionName.Equals(definition.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (named.Length > 1)
			throw new InvalidOperationException($"Ambiguous craft access prog {definition.Name}.");
		var existing = named.SingleOrDefault();
		if (managed?.LogicalId is { } id)
		{
			if (existing is not null && existing.Id != id)
				throw new InvalidOperationException($"ItemSeeder ownership conflict for prog:{definition.Name}: provenance names ID {id}, but the name resolves to {existing.Id}.");
			existing = rows.SingleOrDefault(x => x.Id == id) ?? existing;
		}
		if (existing is null) return null;
		var otherOwners = _managedRecordsByIdentity.Values.Where(x => x.EntityType.Equals("prog", StringComparison.OrdinalIgnoreCase) &&
			x.LogicalId == existing.Id && !x.StableKey.Equals(definition.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (otherOwners.Length > 0)
			throw new InvalidOperationException($"ItemSeeder ownership conflict for prog:{definition.Name}: ID {existing.Id} is claimed by another stable key.");
		if (managed is null && !ItemSeederManifestCatalogue.Fingerprint(TraitGateLiveDefinition(existing)).Equals(
			ItemSeederManifestCatalogue.Fingerprint(definition), StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException($"Unmanaged craft access prog conflict for '{definition.Name}': the complete stock signature does not match.");
		var parameters = existing.FutureProgsParameters.ToArray();
		if (existing.ReturnType != definition.ReturnType || existing.AcceptsAnyParameters ||
			existing.StaticType != (int)FutureProgStaticType.NotStatic || parameters.Length != 1 ||
			parameters[0].ParameterIndex != 0 || parameters[0].ParameterType != (long)ProgVariableTypes.Character)
			throw new InvalidOperationException($"Craft access prog {definition.Name} has an incompatible callable signature or static execution mode.");
		return existing;
	}

	private void ValidateIndustrialisedClothingCraftOwnership(IndustrialisedClothingCatalogueDocument document, IReadOnlySet<string> selected)
	{
		if (_manifestCaptureOnly || document.Crafts.Count == 0) return;
		var crafts = _context!.Crafts.Include(x => x.EditableItem).Include(x => x.CraftPhases).Include(x => x.CraftInputs)
			.Include(x => x.CraftTools).Include(x => x.CraftProducts).AsSplitQuery().AsNoTracking().ToArray();
		var progs = _context.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
		var graph = new ClothingCraftGraphSnapshot(
			_context.GameItemProtos.Include(x => x.EditableItem).AsNoTracking().ToArray(),
			_context.GameItemSkins.Include(x => x.EditableItem).AsNoTracking().ToArray(),
			_context.Tags.AsNoTracking().ToArray(), _context.Materials.AsNoTracking().ToArray(),
			_context.Liquids.AsNoTracking().ToArray(), _context.CharacteristicDefinitions.AsNoTracking().ToArray(),
			_context.CharacteristicValues.AsNoTracking().ToArray(), _context.TraitDefinitions.AsNoTracking().ToArray(), progs);
		var plannedGates = new Dictionary<string, ProgManifestDefinition>(StringComparer.OrdinalIgnoreCase);
		foreach (var row in document.Crafts.Where(x => x.EraAdmissions.Any(selected.Contains)))
		{
			try
			{
				var spec = _clothingCraftPlans[row.StableReference];
				var existing = ResolveOwnedCraftTarget(row.StableReference, row.Name, row.Category, crafts);
				var definitions = TraitGateDefinitions(spec.Trait!, row.MinimumTraitValue);
				foreach (var definition in definitions)
				{
					if (plannedGates.TryGetValue(definition.Name, out var planned) &&
						!ItemSeederManifestCatalogue.Fingerprint(planned).Equals(ItemSeederManifestCatalogue.Fingerprint(definition), StringComparison.OrdinalIgnoreCase))
						throw new InvalidOperationException($"Conflicting planned craft access prog {definition.Name}; distinct trait gates cannot share a stock identity.");
					plannedGates[definition.Name] = definition;
				}
				var gates = definitions.Select(x => ResolveTraitGateProgOwnership(x, progs)).ToArray();
				if (existing is not null && FindManagedRecord("craft", row.StableReference) is null && gates.Any(x => x is null))
					throw new InvalidOperationException($"Unmanaged craft conflict for '{row.StableReference}': expected access progs are missing, so its complete stock signature cannot match.");
				ValidateUnmanagedCraftSignature(row.StableReference, existing, spec with
				{
					AppearProg = gates[0], CanUseProg = gates[1], WhyCannotUseProg = gates[2]
				});
				if (existing is not null) ValidatePreservedClothingCraftGraph(existing, row.Source, graph);
			}
			catch (InvalidOperationException ex)
			{
				throw row.Source.Error(ex.Message);
			}
		}
	}
}
