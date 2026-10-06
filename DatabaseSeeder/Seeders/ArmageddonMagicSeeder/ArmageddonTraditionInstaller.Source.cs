#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Magic;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTraditionInstaller
{
	private static readonly Lazy<IReadOnlyList<ArmageddonSourceRow>> Source = new(() =>
	{
		using var stream = typeof(ArmageddonTraditionInstaller).Assembly.GetManifestResourceStream("ArmageddonSorcererSourceTree")
			?? throw new InvalidOperationException("Missing authoritative source tree.");
		using var document = JsonDocument.Parse(stream);
		var rows = document.RootElement.GetProperty("rows").Deserialize<ArmageddonSourceRow[]>(new JsonSerializerOptions
		{ PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }) ?? throw new InvalidOperationException("Missing source rows.");
		if (rows.Length != 94 || rows.Count(x => x.Kind == "spell") != 82 || rows.Count(x => x.Kind == "support") != 12 ||
			rows.Select(x => x.Key).Distinct().Count() != 94 || !rows.Select(x => x.Order).SequenceEqual(Enumerable.Range(1, 94)))
			throw new InvalidOperationException("Source inventory drift.");
		var byKey = rows.ToDictionary(x => x.Key);
		foreach (var row in rows)
		{
			if (row.Opening < 0 || row.RawCap < row.Opening || row.BranchesAt < 0 || row.BranchesAt > row.RawCap ||
				row.ParentKey is not null && (!byKey.TryGetValue(row.ParentKey, out var parent) || row.ParentThreshold != parent.BranchesAt))
				throw new InvalidOperationException($"Invalid source row {row.Key}.");
			var seen = new HashSet<string>(); var current = row;
			while (current.ParentKey is { } key)
			{
				if (!seen.Add(key)) throw new InvalidOperationException("Source cycle.");
				current = byKey[key];
			}
		}
		return Array.AsReadOnly(rows);
	});
	public static IReadOnlyList<ArmageddonSourceRow> SourceRows => Source.Value;

	private static Guid Identity(string key) => new(SHA256.HashData(Encoding.UTF8.GetBytes(Package + ":" + Module + ":" + key)).AsSpan(0, 16));
	private static IReadOnlyList<MagicGatheringMethodKind> Methods(ArmageddonTraditionInstallPlan plan, string variant) =>
		plan.AllowedMethods is { } selected ? selected[variant] : variant switch
		{
			"sorcerer" => [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Gentle, MagicGatheringMethodKind.Land],
			"preserver" => [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Gentle],
			"defiler" => [MagicGatheringMethodKind.Self, MagicGatheringMethodKind.Land],
			_ => throw new ArgumentOutOfRangeException(nameof(variant))
		};

	// Only full source paths may enter the live policy. Definition availability alone never grants a bypass.
	private static HashSet<string> Reachable(ArmageddonTraditionInstallPlan plan)
	{
		var available = new HashSet<string>();
		foreach (var row in SourceRows)
		{
			if (row.Kind == "spell" && !plan.ImplementedSpells.ContainsKey(row.Key)) continue;
			if (row.Kind == "support" && (!plan.SupportSkills.ContainsKey(row.Key) ||
				row.Key is not ("arm.support.component_crafting" or "arm.support.vloran" or "arm.support.gather"))) continue;
			if (row.ParentKey is null || available.Contains(row.ParentKey)) available.Add(row.Key);
		}
		return available;
	}

	private static XElement Capability(ArmageddonTraditionInstallPlan plan, string variant, XElement template,
		IReadOnlyDictionary<string, long> skills, HashSet<string> available)
	{
		var source = SourceRows.ToDictionary(x => x.Key);
		var gathering = new XElement(template.Element("Gathering")!);
		foreach (var method in gathering.Elements("Method").ToArray())
		{
			if (!Methods(plan, variant).Contains(Enum.Parse<MagicGatheringMethodKind>((string)method.Attribute("kind")!)))
			{ method.Remove(); continue; }
			var old = (string)method.Attribute("key")!;
			method.SetAttributeValue("key", Identity(variant + ":gather:" + old));
			foreach (var land in method.Element("Land")?.Elements("Source") ?? [])
				land.SetAttributeValue("key", Identity(variant + ":land:" + old + ":" + (string?)land.Attribute("key")));
		}
		var root = new XElement("Definition", new XElement("ConcentrationTrait", plan.SupportSkills["arm.support.gather"]),
			new XElement(template.Element("ConcentrationCapabilityExpression")!), new XElement(template.Element("ConcentrationDifficultyExpression")!),
			new XElement("Regenerators"), gathering);
		var casting = new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Identity(variant)),
			new XAttribute("enabled", true), new XAttribute("trait", skills["arm.spell.sense_enchantment"]),
			new XAttribute("source", plan.SourceResource), new XAttribute("reserve", plan.ReserveResource),
			new XAttribute("passive", false), new XAttribute("startingVersion", 1));
		foreach (var row in SourceRows.Where(x => available.Contains(x.Key)))
		{
			var node = new XElement(row.Kind == "spell" ? "Admission" : "SupportGrant",
				new XAttribute("key", Identity(variant + ":" + row.Key)), new XAttribute("trait", skills[row.Key]),
				new XAttribute("starting", row.ParentKey is null), new XAttribute("opening", row.Opening), new XAttribute("rawCap", row.RawCap));
			if (row.Kind == "spell") node.Add(new XAttribute("spell", plan.ImplementedSpells[row.Key]),
				new XAttribute("min", 1), new XAttribute("max", 7), new XAttribute("capRelative", true));
			if (row.ParentKey is { } parent)
			{
				var edge = new XElement("Prerequisite", new XAttribute("key", Identity(variant + ":edge:" + row.Key)),
					new XAttribute("proficiency", row.ParentThreshold!.Value));
				if (source[parent].Kind == "spell") edge.Add(new XAttribute("spell", plan.ImplementedSpells[parent]), new XAttribute("grade", 1));
				else edge.Add(new XAttribute("kind", "trait"), new XAttribute("trait", skills[parent]));
				node.Add(edge);
			}
			casting.Add(node);
		}
		root.Add(casting); return root;
	}
}
