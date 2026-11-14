#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Database;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Links completed, tracked one-shot import output. Never pools existing builder/foundation
/// programs or changes function-call text. The caller owns the installation transaction.
/// </summary>
internal static class ArmageddonWorldfileProgConsolidation
{
	internal sealed record Alias(long OriginalId, string OriginalName, long CanonicalId, string CanonicalName);
	private static readonly Regex Identifiers = new(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.CultureInvariant);
	private static readonly Regex PoisonCalls = new(@"\bsetweaponpoison\s*\(", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	private static readonly Regex LiteralPoisonCalls = new(@"\bsetweaponpoison\s*\(\s*@[A-Za-z_][A-Za-z0-9_]*\s*,\s*(?<id>[0-9]+)\s*\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

	internal static IReadOnlyList<Alias> Apply(FuturemudDatabaseContext db, IReadOnlyList<FutureProg> generated)
	{
		// All generation, name-based lookups, and per-source mutations must finish first.
		db.ChangeTracker.DetectChanges();
		var autoDetect = db.ChangeTracker.AutoDetectChangesEnabled;
		db.ChangeTracker.AutoDetectChangesEnabled = false;
		try
		{
			var entries = db.ChangeTracker.Entries().Where(x => x.State != EntityState.Deleted).ToArray();
			var byId = generated.ToDictionary(x => x.Id);
			var names = generated.Select(x => x.FunctionName).ToHashSet(StringComparer.OrdinalIgnoreCase);
			var protectedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var protectedIds = new HashSet<long>();
			void ProtectNames(string? text)
			{
				if (string.IsNullOrEmpty(text)) return;
				foreach (Match token in Identifiers.Matches(text))
					if (names.Contains(token.Value)) protectedNames.Add(token.Value);
			}

			// Retain every named call target, including names in literals/comments. This is
			// deliberately conservative and avoids altering strings, recursion or overload lookup.
			foreach (var text in db.FutureProgs.AsNoTracking().Select(x => x.FunctionText))
			{
				ProtectNames(text);
				var literalCalls = LiteralPoisonCalls.Matches(text ?? "");
				if (PoisonCalls.Matches(text ?? "").Count != literalCalls.Count)
					protectedIds.UnionWith(byId.Keys); // A dynamic prog-ID expression cannot be safely relinked.
				else foreach (Match call in literalCalls)
					if (long.TryParse(call.Groups["id"].Value, out var id)) protectedIds.Add(id);
			}
			foreach (var entry in entries)
				foreach (var property in XmlProperties(entry.Metadata))
					ProtectNames(entry.Property(property.Name).CurrentValue as string);

			var progType = db.Model.FindEntityType(typeof(FutureProg))!;
			foreach (var entry in entries)
			{
				if (entry.Entity is FutureProgsParameter or GameItemProtosOnLoadProgs) continue;
				foreach (var fk in ProgForeignKeys(entry.Metadata, progType))
					if (fk.Properties.Any(x => x.IsPrimaryKey()) &&
						entry.Property(fk.Properties.Single().Name).CurrentValue is long id)
						protectedIds.Add(id); // Unknown keyed-link semantics require a dedicated adapter.
				foreach (var property in NumericProgProperties(entry.Metadata).Where(x => x.IsPrimaryKey()))
					if (entry.Property(property.Name).CurrentValue is { } value)
						protectedIds.Add(Convert.ToInt64(value, CultureInfo.InvariantCulture));
			}

			var replacements = new Dictionary<long, FutureProg>();
			foreach (var group in generated.Where(x => !x.Public && x.StaticType == 0).GroupBy(Payload))
			{
				var members = group.OrderBy(x => x.Id).ToArray();
				var canonical = members.FirstOrDefault(x => protectedNames.Contains(x.FunctionName) || protectedIds.Contains(x.Id))
					?? members[0];
				foreach (var prog in members)
					if (prog.Id != canonical.Id && !protectedNames.Contains(prog.FunctionName) && !protectedIds.Contains(prog.Id))
						replacements.Add(prog.Id, canonical);
			}

			// Two on-load rows on one prototype mean two invocations, even with equal code.
			// Its composite key cannot represent that after pooling; retain that whole payload group.
			var conflictingCanonicals = entries.Select(x => x.Entity).OfType<GameItemProtosOnLoadProgs>()
				.GroupBy(x => (x.GameItemProtoId, x.GameItemProtoRevisionNumber,
					Prog: replacements.TryGetValue(x.FutureProgId, out var p) ? p.Id : x.FutureProgId))
				.Where(x => x.Count() > 1).Select(x => x.Key.Prog).ToHashSet();
			foreach (var id in replacements.Where(x => conflictingCanonicals.Contains(x.Value.Id)).Select(x => x.Key).ToArray())
				replacements.Remove(id);

			if (replacements.Count == 0) return [];
			foreach (var entry in entries)
			{
				if (entry.Entity is FutureProgsParameter) continue; // Alias signatures are deleted, not copied onto the canonical.
				foreach (var fk in ProgForeignKeys(entry.Metadata, progType))
				{
					var property = fk.Properties.Single();
					if (entry.Property(property.Name).CurrentValue is not long id || !replacements.TryGetValue(id, out var canonical)) continue;
					if (entry.Entity is GameItemProtosOnLoadProgs link)
					{
						db.Remove(link);
						db.Add(new GameItemProtosOnLoadProgs
						{
							GameItemProtoId = link.GameItemProtoId, GameItemProtoRevisionNumber = link.GameItemProtoRevisionNumber,
							GameItemProto = link.GameItemProto, FutureProgId = canonical.Id, FutureProg = canonical
						});
						continue;
					}
					if (fk.DependentToPrincipal is { } navigation) entry.Reference(navigation.Name).CurrentValue = canonical;
					entry.Property(property.Name).CurrentValue = canonical.Id;
				}
				if (entry.State == EntityState.Deleted) continue; // A keyed on-load link has been replaced above.
				// Some legacy typed prog pointers intentionally have no EF foreign key (e.g. Liquid.DraughtProgId).
				foreach (var property in NumericProgProperties(entry.Metadata))
					if (entry.Property(property.Name).CurrentValue is { } value &&
						replacements.TryGetValue(Convert.ToInt64(value, CultureInfo.InvariantCulture), out var scalar))
						entry.Property(property.Name).CurrentValue = Convert.ChangeType(scalar.Id,
							Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType, CultureInfo.InvariantCulture);
				foreach (var property in XmlProperties(entry.Metadata))
					if (entry.Property(property.Name).CurrentValue is string xml)
						entry.Property(property.Name).CurrentValue = RewriteXml(xml, replacements);
				if (entry.Entity is StaticConfiguration setting && IsProgReference(setting.SettingName) &&
					long.TryParse(setting.Definition, out var settingId) && replacements.TryGetValue(settingId, out var configured))
					setting.Definition = configured.Id.ToString(CultureInfo.InvariantCulture);
				if (entry.Entity is SeederManagedRecord { EntityType: nameof(FutureProg) or "Prog", LogicalId: { } logicalId } record &&
					replacements.TryGetValue(logicalId, out var target)) record.LogicalId = target.Id;
			}

			var aliases = replacements.OrderBy(x => x.Key)
				.Select(x => new Alias(x.Key, byId[x.Key].FunctionName, x.Value.Id, x.Value.FunctionName)).ToArray();
			db.FutureProgs.RemoveRange(replacements.Keys.Select(x => byId[x]));
			db.ChangeTracker.DetectChanges();
			// Check the same typed reference surfaces after relinking, before the caller commits.
			foreach (var entry in db.ChangeTracker.Entries().Where(x => x.State != EntityState.Deleted))
			{
				foreach (var fk in ProgForeignKeys(entry.Metadata, progType))
					if (entry.Property(fk.Properties.Single().Name).CurrentValue is long id && replacements.ContainsKey(id))
						throw new InvalidOperationException($"Unresolved generated prog reference in {entry.Metadata.ClrType.Name}.");
				foreach (var property in NumericProgProperties(entry.Metadata))
					if (entry.Property(property.Name).CurrentValue is { } value &&
						replacements.ContainsKey(Convert.ToInt64(value, CultureInfo.InvariantCulture)))
						throw new InvalidOperationException($"Unresolved scalar prog reference in {entry.Metadata.ClrType.Name}.{property.Name}.");
				foreach (var property in XmlProperties(entry.Metadata))
					if (entry.Property(property.Name).CurrentValue is string xml && RewriteXml(xml, replacements) != xml)
						throw new InvalidOperationException($"Unresolved serialized prog reference in {entry.Metadata.ClrType.Name}.{property.Name}.");
				if (entry.Entity is StaticConfiguration setting && IsProgReference(setting.SettingName) &&
					long.TryParse(setting.Definition, out var configuredId) && replacements.ContainsKey(configuredId))
					throw new InvalidOperationException($"Unresolved configured prog reference in {setting.SettingName}.");
			}
			return aliases;
		}
		finally { db.ChangeTracker.AutoDetectChangesEnabled = autoDetect; }
	}

	private static IEnumerable<IForeignKey> ProgForeignKeys(IEntityType type, IEntityType progType) =>
		type.GetForeignKeys().Where(x => x.PrincipalEntityType == progType);

	private static IEnumerable<IProperty> XmlProperties(IEntityType type) => type.GetProperties()
		.Where(x => x.ClrType == typeof(string) && x.Name is "Definition" or "EffectData");

	private static IEnumerable<IProperty> NumericProgProperties(IEntityType type) => type.GetProperties()
		.Where(x => IsProgReference(x.Name) &&
			(Nullable.GetUnderlyingType(x.ClrType) ?? x.ClrType) is var numeric && (numeric == typeof(long) || numeric == typeof(int)));

	private static string Payload(FutureProg prog) => JsonSerializer.Serialize(new
	{
		prog.FunctionText, prog.ReturnTypeDefinition, prog.AcceptsAnyParameters, prog.StaticType, prog.Public,
		prog.Category, prog.Subcategory,
		Parameters = prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => new { x.ParameterIndex, x.ParameterTypeDefinition, x.ParameterName })
	});

	private static bool IsProgReference(string name) =>
		name.EndsWith("Prog", StringComparison.OrdinalIgnoreCase) || name.EndsWith("ProgId", StringComparison.OrdinalIgnoreCase);

	internal static string RewriteXml(string xml, IReadOnlyDictionary<long, FutureProg> replacements)
	{
		if (!xml.AsSpan().TrimStart().StartsWith("<", StringComparison.Ordinal)) return xml;
		XElement root;
		try { root = XElement.Parse(xml, LoadOptions.PreserveWhitespace); }
		catch (XmlException) { return xml; } // This field may contain source prose, not a serialized entity definition.
		var changed = false;
		foreach (var element in root.DescendantsAndSelf())
		{
			var trapParameter = element.Name.LocalName == "Parameter" &&
				(string?)element.Attribute("name") == "prog" &&
				element.Parent?.Name.LocalName == "Payload" && (string?)element.Parent.Attribute("type") == "ExecuteProg";
			if (!element.HasElements && (IsProgReference(element.Name.LocalName) || trapParameter) &&
				long.TryParse(element.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) &&
				replacements.TryGetValue(id, out var prog))
			{
				element.Value = prog.Id.ToString(CultureInfo.InvariantCulture); changed = true;
			}
			foreach (var attribute in element.Attributes())
				if ((IsProgReference(attribute.Name.LocalName) ||
					attribute.Name.LocalName.Equals("Id", StringComparison.OrdinalIgnoreCase) && IsProgReference(element.Name.LocalName)) &&
					long.TryParse(attribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) &&
					replacements.TryGetValue(value, out var target))
				{
					attribute.Value = target.Id.ToString(CultureInfo.InvariantCulture); changed = true;
				}
			if (!element.HasElements && element.Name.LocalName == "Policy" &&
				element.Value.AsSpan().TrimStart().StartsWith("{", StringComparison.Ordinal))
			{
				var policy = JsonNode.Parse(element.Value)!;
				if (RewritePolicy(policy, replacements))
				{
					element.ReplaceNodes(new XCData(policy.ToJsonString())); changed = true;
				}
			}
		}
		return changed ? root.ToString(SaveOptions.DisableFormatting) : xml;
	}

	private static bool RewritePolicy(JsonNode node, IReadOnlyDictionary<long, FutureProg> replacements)
	{
		var changed = false;
		if (node is JsonObject obj)
			foreach (var property in obj.ToArray())
			{
				if (IsProgReference(property.Key) && property.Value is JsonValue value && value.TryGetValue<long>(out var id) &&
					replacements.TryGetValue(id, out var prog))
				{
					obj[property.Key] = prog.Id; changed = true;
				}
				else if (property.Value is { } child) changed |= RewritePolicy(child, replacements);
			}
		else if (node is JsonArray array)
			foreach (var child in array.OfType<JsonNode>()) changed |= RewritePolicy(child, replacements);
		return changed;
	}
}
