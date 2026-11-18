#nullable enable

using System.Xml;
using System.Xml.Linq;
using ItemRow = MudSharp.Models.GameItem;

namespace MudSharp.GameItems;

internal static class LegacyItemCustody
{
	internal static IReadOnlySet<long> FindAmbiguousChildren(IEnumerable<ItemRow> items,
		IReadOnlyDictionary<(long Id, int Revision), string> componentTypes, Action<string> report)
	{
		var rows = items.ToArray();
		var withoutDirectCustody = rows.Where(x => x.RoomsGameItems.Count == 0 &&
		                                           x.BodiesGameItems.Count == 0 && x.BodiesImplants.Count == 0 &&
		                                           x.BodiesProsthetics.Count == 0 && x.WoundsLodgedItem.Count == 0)
			.ToArray();
		var candidates = withoutDirectCustody.Where(x => x.ContainerId is null)
			.Select(x => x.Id).ToHashSet();
		var claims = new Dictionary<long, HashSet<(long ParentId, long ComponentId)>>();
		foreach (var item in rows)
		{
			foreach (var component in item.GameItemComponents)
			{
				if (!componentTypes.TryGetValue((component.GameItemComponentProtoId,
					    component.GameItemComponentProtoRevision), out var type)) continue;

				XElement definition;
				try
				{
					definition = XElement.Parse(component.Definition);
				}
				catch (XmlException ex)
				{
					report($"Could not index custody for component #{component.Id} on item #{item.Id}: {ex.Message}");
					continue;
				}

				var references = type switch
				{
					"AmmoClip" => definition.Element("Contents")?.Elements("Item") ?? [],
					"Bodypart" => (definition.Element("Contents")?.Elements("Item") ?? [])
						.Concat(definition.Element("Implants")?.Elements("Implant") ?? []),
					"Gun" or "BoltAction" => definition.Elements("Magazine").Concat(definition.Elements("ChamberedRound"))
						.Concat(definition.Element("Attachments")?.Elements("Attachment") ?? []),
					"InternalMagazineGun" => (definition.Element("RoundsInMagazine")?.Elements("Round") ?? [])
						.Concat(definition.Elements("ChamberedRound")).Concat(definition.Elements("ChamberedCasing"))
						.Concat(definition.Element("Attachments")?.Elements("Attachment") ?? []),
					"Sheath" => definition.Element("Contents") is { } sheathContents
						? sheathContents.Elements("Content")
						: definition.Elements("Content"),
					"Belt" => definition.Elements("Connected"),
					"Table" => definition.Elements("Chair"),
					"Container" or "LockingContainer" or "Automation Housing" or "BatteryPowered" or "BatteryCharger" or
						"ImplantContainer" or "ImplantRefrigerator" or "Refrigerator" or "Dryer" or "FoldedPocket" or
						"CashRegister" or "LockingCashRegister" or "WashingMachine" or "FaxMachine" or "Photocopier" or "ShopStall" => definition.Elements("Contained")
						.Concat(definition.Element("Locks")?.Elements("Lock") ?? []),
					"Fuel Generator" => definition.Element("Locks")?.Elements("Lock") ?? [],
					"Pile" => definition.Elements("Contained"),
					_ => []
				};
				foreach (var reference in references)
				{
					if (!long.TryParse(reference.Value, out var childId) || childId == item.Id ||
					    !candidates.Contains(childId)) continue;

					if (!claims.TryGetValue(childId, out var owners))
					{
						claims[childId] = owners = new HashSet<(long, long)>();
					}

					owners.Add((item.Id, component.Id));
				}
			}
		}

		var ambiguous = new HashSet<long>();
		var parents = withoutDirectCustody.Where(x => x.ContainerId is not null)
			.ToDictionary(x => x.Id, x => x.ContainerId!.Value);
		foreach (var (childId, owners) in claims.OrderBy(x => x.Key))
		{
			if (owners.Count == 1)
			{
				parents.Add(childId, owners.Single().ParentId);
				continue;
			}
			ambiguous.Add(childId);
			var identifiers = owners.OrderBy(x => x).Select(x => $"item #{x.ParentId}, component #{x.ComponentId}");
			report($"Ambiguous legacy custody of item #{childId}: {string.Join(" | ", identifiers)}.");
		}

		var visited = new HashSet<long>();
		var ancestry = new Dictionary<long, int>();
		foreach (var start in parents.Keys.Order())
		{
			if (visited.Contains(start)) continue;
			var current = start;
			while (!visited.Contains(current) && parents.TryGetValue(current, out var parent))
			{
				if (!ancestry.TryAdd(current, ancestry.Count))
				{
					var cycle = ancestry.Where(x => x.Value >= ancestry[current]).Select(x => x.Key).Order().ToArray();
					ambiguous.UnionWith(cycle);
					report($"Cyclic item custody: {string.Join(", ", cycle.Select(id => $"item #{id}"))}.");
					break;
				}

				current = parent;
			}

			visited.UnionWith(ancestry.Keys);
			ancestry.Clear();
		}

		return ambiguous;
	}
}
