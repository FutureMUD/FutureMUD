#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonStockCapability(string Key, string Name, string MeritKey, string RoleKey,
	string MainTraitKey, string ResourceKey, IReadOnlyList<string> SpellKeys);

public sealed record ArmageddonStockBinding(string Key, string CapabilityKey, string ContentKey, string TraitKey,
	bool Support, bool Starting, double Opening, double RawCap, string? ParentKey, double? ParentThreshold,
	double? MainThreshold = null, bool RequiresCrafting80 = false);

/// <summary>The approved graph projected through declared fields, without searching definitions for IDs.</summary>
public sealed class ArmageddonStockProgression
{
	public const string CraftKey = "arm.support.component_crafting";
	public const string CraftTraitKey = CraftKey + ".skill";
	public IReadOnlyList<ArmageddonStockCapability> Capabilities { get; }
	public IReadOnlyList<ArmageddonStockBinding> Bindings { get; }
	public IReadOnlyDictionary<string, string> SpellNames { get; }
	public ArmageddonProgressionMode Mode { get; }

	public ArmageddonStockProgression(ArmageddonProgressionMode mode, string? json = null)
	{
		if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
		json ??= ArmageddonStockManifest.Read();
		var errors = ArmageddonStockManifest.Validate(json);
		if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
		using var document = JsonDocument.Parse(json);
		var root = document.RootElement;
		Mode = mode;
		Capabilities = Array.AsReadOnly(root.GetProperty("capabilities").EnumerateArray().Select(x => new ArmageddonStockCapability(
			Text(x, "key"), Text(x, "name"), Text(x, "merit_key"), Text(x, "class_role_key"), Text(x, "simple_main_trait_key"),
			Text(x, "reserve_key"), Array.AsReadOnly(x.GetProperty("required_spell_keys").EnumerateArray().Select(x => x.GetString()!).ToArray()))).ToArray());
		SpellNames = root.GetProperty("entries").EnumerateArray().ToDictionary(x => Text(x, "key"), x => Text(x, "name"), StringComparer.Ordinal);
		var bindings = new List<ArmageddonStockBinding>();
		if (mode == ArmageddonProgressionMode.Classic)
		{
			var policy = root.GetProperty("classic_policy");
			foreach (var row in policy.GetProperty("elemental_rows").EnumerateArray())
				bindings.Add(Classic(row, Text(row, "capability"), Text(row, "binding_key")));
			foreach (var cap in Capabilities.Where(IsSorcerer))
				foreach (var row in policy.GetProperty("sorcerer_install_rows").EnumerateArray())
					bindings.Add(Classic(row, cap.Key, cap.Key + ":" + Text(row, "key")));
		}
		else
		{
			foreach (var row in root.GetProperty("simple_progression").EnumerateArray())
			{
				var cap = Text(row, "capability");
				var spell = Text(row, "spell");
				bindings.Add(new(cap + ":" + spell, cap, spell, Text(row, "main_trait"), false,
					row.GetProperty("starting").GetBoolean(), 10, 100, null, null,
					row.GetProperty("unlock_at").GetDouble(), IsSorcererKey(cap) && spell == "arm.spell.read_enchantment"));
			}
			foreach (var cap in Capabilities)
			{
				bindings.Add(new(cap.Key + ":main", cap.Key, cap.MainTraitKey, cap.MainTraitKey, true, true, 10, 100, null, null));
				bindings.Add(new(cap.Key + ":craft", cap.Key, CraftKey, CraftTraitKey, true, true, 10, 90, null, null));
			}
		}
		Bindings = bindings.AsReadOnly();
		foreach (var cap in Capabilities)
		{
			var own = Bindings.Where(x => x.CapabilityKey == cap.Key).ToArray();
			if (!cap.SpellKeys.ToHashSet(StringComparer.Ordinal).SetEquals(own.Where(x => !x.Support).Select(x => x.ContentKey)))
				throw new InvalidOperationException($"{cap.Key}: progression roster differs from the approved capability.");
			if (own.Select(x => x.ContentKey).Distinct().Count() != own.Length)
				throw new InvalidOperationException($"{cap.Key}: duplicate progression content.");
		}
	}

	private static ArmageddonStockBinding Classic(JsonElement row, string cap, string bindingKey)
	{
		var key = Text(row, "key");
		var parent = row.GetProperty("parent_key").GetString();
		return new(bindingKey, cap, key, key == CraftKey ? CraftTraitKey : key + ".skill", Text(row, "kind") == "support",
			parent is null, row.GetProperty("opening").GetDouble(), row.GetProperty("raw_cap").GetDouble(), parent,
			parent is null ? null : row.GetProperty("parent_threshold").GetDouble());
	}

	private static string Text(JsonElement row, string field) => row.GetProperty(field).GetString()!;
	public static bool IsSorcerer(ArmageddonStockCapability cap) => IsSorcererKey(cap.Key);
	private static bool IsSorcererKey(string key) => key is "arm.capability.sorcerer" or "arm.capability.preserver" or "arm.capability.defiler";

	// Retain the original three identities and admission keys, including historical player receipts.
	public static Guid Identity(string capKey, string? contentKey = null)
	{
		var module = IsSorcererKey(capKey) ? ArmageddonTraditionInstaller.Module : ArmageddonProgressionInstaller.Module;
		var variant = capKey["arm.capability.".Length..];
		return new Guid(SHA256.HashData(Encoding.UTF8.GetBytes(ArmageddonMagicInstaller.Package + ":" + module + ":" +
			variant + (contentKey is null ? "" : ":" + contentKey))).AsSpan(0, 16));
	}

	/// <summary>Only the complete roster can be hydrated. Missing spells produce an inert, valid policy.</summary>
	public XElement Casting(ArmageddonStockCapability cap, IReadOnlyDictionary<string, long> traits, long resource,
		IReadOnlyDictionary<string, long>? qualifiedSpells = null, long? sourceResource = null)
	{
		var own = Bindings.Where(x => x.CapabilityKey == cap.Key).ToArray();
		var ready = qualifiedSpells is not null && cap.SpellKeys.All(x => qualifiedSpells.TryGetValue(x, out var id) && id > 0);
		var defaultTrait = Mode == ArmageddonProgressionMode.Simple ? cap.MainTraitKey : own.First(x => !x.Support && x.Starting).TraitKey;
		var casting = new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Identity(cap.Key)),
			new XAttribute("enabled", ready), new XAttribute("trait", traits[defaultTrait]), new XAttribute("source", sourceResource ?? resource),
			new XAttribute("reserve", resource), new XAttribute("passive", !IsSorcerer(cap)), new XAttribute("startingVersion", 1),
			new XAttribute("enrolOnClassSelection", true));
		if (!ready) return casting;
		if (cap.SpellKeys.Select(x => qualifiedSpells![x]).Distinct().Count() != cap.SpellKeys.Count)
			throw new InvalidOperationException($"{cap.Key}: canonical spell identities must remain distinct.");
		foreach (var row in own)
		{
			var node = new XElement(row.Support ? "SupportGrant" : "Admission", new XAttribute("key", Identity(cap.Key, row.ContentKey)),
				new XAttribute("trait", traits[row.TraitKey]), new XAttribute("starting", row.Starting),
				new XAttribute("opening", row.Opening), new XAttribute("rawCap", row.RawCap));
			if (!row.Support) node.Add(new XAttribute("spell", qualifiedSpells![row.ContentKey]),
				new XAttribute("min", 1), new XAttribute("max", 7), new XAttribute("capRelative", true));
			if (row.ParentKey is { } parent)
			{
				var prior = own.Single(x => x.ContentKey == parent);
				node.Add(Edge(cap.Key, row.ContentKey + ":parent", row.ParentThreshold!.Value,
					prior.Support ? traits[prior.TraitKey] : qualifiedSpells![parent], prior.Support));
			}
			if (row.MainThreshold is { } threshold)
				node.Add(Edge(cap.Key, row.ContentKey + ":main", threshold, traits[cap.MainTraitKey], true));
			if (row.RequiresCrafting80)
				node.Add(Edge(cap.Key, row.ContentKey + ":craft", 80, traits[CraftTraitKey], true));
			casting.Add(node);
		}
		return casting;
	}

	private static XElement Edge(string cap, string key, double threshold, long id, bool support) => new("Prerequisite",
		new XAttribute("key", Identity(cap, "edge:" + key)), new XAttribute("proficiency", threshold),
		support ? new XAttribute("kind", "trait") : null, new XAttribute(support ? "trait" : "spell", id),
		support ? null : new XAttribute("grade", 1));
}
