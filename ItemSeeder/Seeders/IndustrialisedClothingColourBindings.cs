#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

internal sealed record ClothingBoundColour(string Variable, long DefinitionId, long ProfileId,
	IReadOnlyDictionary<string, long> Values, string DefaultValue);

/// <summary>
/// Read-only, scalar snapshot of the runtime characteristic registry. Exact bindings are checked
/// before item creation; runtime's permissive prefix matching and random fallback are not preflight.
/// </summary>
internal sealed class IndustrialisedClothingColourBindings
{
	private sealed record Definition(long Id, string Name, string Pattern, long? ParentId, string Model);
	private sealed record Profile(long Id, string Name, long TargetId, string Type, string Xml);
	private sealed record Value(long Id, string Name, long DefinitionId, long? OngoingProgId);
	private readonly Definition[] _definitions;
	private readonly Profile[] _profiles;
	private readonly Value[] _values;

	internal IndustrialisedClothingColourBindings(IEnumerable<CharacteristicDefinition> definitions,
		IEnumerable<CharacteristicProfile> profiles, IEnumerable<CharacteristicValue> values)
	{
		_definitions = definitions.Select(x => new Definition(x.Id, x.Name, x.Pattern, x.ParentId, x.Model)).ToArray();
		_profiles = profiles.Select(x => new Profile(x.Id, x.Name, x.TargetDefinitionId, x.Type, x.Definition)).ToArray();
		_values = values.Select(x => new Value(x.Id, x.Name, x.DefinitionId, x.OngoingValidityProgId)).ToArray();
	}

	internal static IndustrialisedClothingColourBindings Read(FuturemudDatabaseContext context) => new(
		context.CharacteristicDefinitions.AsNoTracking().ToArray(),
		context.CharacteristicProfiles.AsNoTracking().ToArray(),
		context.CharacteristicValues.AsNoTracking().ToArray());

	internal IReadOnlyDictionary<string, ClothingBoundColour> Bind(
		IReadOnlyDictionary<string, ClothingColourRow> channels,
		IEnumerable<GameItemComponentProto> components, ClothingSourceLocation source, bool requireStandaloneProfile = true)
	{
		var variables = components.Where(x => x.Type == "Variable").ToArray();
		if (variables.Length != 1)
		{
			throw source.Error($"A clothing base requires exactly one Variable component; found {variables.Length}.");
		}

		var root = ParseXml(variables[0].Definition, source);
		var bindings = root.Elements("Characteristic")
			.Select(x => (Definition: ReadId(x.Attribute("Value")?.Value, source),
				Profile: ReadId(x.Attribute("Profile")?.Value, source))).ToArray();
		if (bindings.Length == 0 || bindings.Select(x => x.Definition).Distinct().Count() != bindings.Length)
		{
			throw source.Error("The Variable component has no characteristics or repeats a characteristic definition.");
		}

		var componentDefinitions = bindings.Select(x => DefinitionById(x.Definition, source)).ToArray();
		var result = new Dictionary<string, ClothingBoundColour>(StringComparer.Ordinal);
		foreach (var (variable, channel) in channels.OrderBy(x => x.Key, StringComparer.Ordinal))
		{
			var definition = Exact(_definitions, x => x.Name, channel.Definition, channel.Source, "characteristic definition");
			var profile = Exact(_profiles, x => x.Name, channel.Profile, channel.Source, "characteristic profile");
			if (!bindings.Contains((definition.Id, profile.Id)))
			{
				throw channel.Source.Error($"Variable component does not bind {channel.Definition} to {channel.Profile}.");
			}

			// Runtime uses the first matching pattern, so accepting overlaps would make order observable.
			var matchingDefinitions = componentDefinitions.Where(x => Matches(x.Pattern, variable, channel.Source)).ToArray();
			if (matchingDefinitions.Length != 1 || matchingDefinitions[0].Id != definition.Id)
			{
				throw channel.Source.Error($"Colour variable {variable} must match exactly its declared runtime definition.");
			}

			var profileValues = ProfileValues(profile, channel.Source, []).ToArray();
			if (profileValues.Length == 0 || profileValues.Any(x => !IsValue(definition, x, channel.Source)))
			{
				throw channel.Source.Error($"Characteristic profile {profile.Name} is empty or contains incompatible values.");
			}

			var permitted = new Dictionary<string, long>(StringComparer.Ordinal);
			foreach (var name in channel.AllowedValues)
			{
				// Resolve against the definition as runtime does, not just the palette. Two same-name values
				// outside/inside the palette would otherwise make builder overrides ambiguous.
				var value = Exact(_values.Where(x => IsValue(definition, x, channel.Source)), x => x.Name,
					name, channel.Source, "characteristic value");
				if (!profileValues.Any(x => x.Id == value.Id) || value.OngoingProgId is not null)
				{
					throw channel.Source.Error($"Colour {name} is outside {profile.Name} or has unresolved ongoing validity behaviour.");
				}

				permitted.Add(name, value.Id);
			}

			if (!permitted.ContainsKey(channel.DefaultValue))
			{
				throw channel.Source.Error($"Standalone colour default {channel.DefaultValue} is not permitted.");
			}
			if (requireStandaloneProfile)
			{
				// A bare item can be created without outfit/craft load arguments. Its Variable component
				// randomly samples this profile; TSV defaults do not replace that runtime distribution.
				var outside = profileValues.Where(x => !permitted.Values.Contains(x.Id))
					.Select(x => x.Name).OrderBy(x => x, StringComparer.Ordinal).ToArray();
				if (outside.Length > 0)
					throw channel.Source.Error($"Standalone variable {variable} can randomly select values outside its authored palette: {string.Join(", ", outside)}. Bind an exact compatible stock profile before admission.");
			}
			result.Add(variable, new(variable, definition.Id, profile.Id, permitted.AsReadOnly(), channel.DefaultValue));
		}

		if (result.Values.Select(x => x.DefinitionId).Distinct().Count() != result.Count ||
			!componentDefinitions.Select(x => x.Id).OrderBy(x => x).SequenceEqual(result.Values.Select(x => x.DefinitionId).OrderBy(x => x)))
		{
			throw source.Error("Every runtime variable needs exactly one authored colour channel; undeclared channels would randomise.");
		}

		return result.AsReadOnly();
	}

	internal static string LoadArguments(IReadOnlyDictionary<string, ClothingBoundColour> bindings,
		IReadOnlyDictionary<string, string> choices, ClothingSourceLocation source)
	{
		if (!bindings.Keys.OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(choices.Keys.OrderBy(x => x, StringComparer.Ordinal)))
		{
			throw source.Error("Colour selections must name every bound variable exactly once.");
		}

		return string.Join(" ", bindings.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x =>
		{
			if (!x.Value.Values.TryGetValue(choices[x.Key], out var id))
			{
				throw source.Error($"Unsupported colour selection {choices[x.Key]} for {x.Key}.");
			}
			return $"{x.Key}={id.ToString(CultureInfo.InvariantCulture)}";
		}));
	}

	internal void ValidateUnrestrictedCraftInheritance(ClothingBoundColour binding, ClothingSourceLocation source)
	{
		var definition = DefinitionById(binding.DefinitionId, source);
		var possible = _values.Where(x => IsValue(definition, x, source)).ToArray();
		if (possible.Any(x => x.OngoingProgId is not null || !binding.Values.Values.Contains(x.Id)))
		{
			throw source.Error($"Inherited {definition.Name} can supply values outside this product palette. The existing input accepts any value for the definition; use an explicit product selection or implement a supported input palette filter before admission.");
		}
	}

	/// <summary>Check persisted outfit arguments without runtime's prefix ambiguity or random fallback.</summary>
	internal void ValidatePersistedLoadArguments(string? arguments, IEnumerable<GameItemComponentProto> components,
		ClothingSourceLocation source, IReadOnlyDictionary<string, ClothingBoundColour>? authored = null)
	{
		var variables = components.Where(x => x.Type == "Variable").ToArray();
		if (variables.Length > 1) throw source.Error("Outfit entry has multiple Variable components.");
		(Definition Definition, Profile Profile)[] bindings = variables.Length == 0 ? [] : ParseXml(variables[0].Definition, source).Elements("Characteristic")
			.Select(x => (Definition: DefinitionById(ReadId(x.Attribute("Value")?.Value, source), source),
				Profile: ById(_profiles, x => x.Id, ReadId(x.Attribute("Profile")?.Value, source), source, "characteristic profile"))).ToArray();
		if (variables.Length == 1 && bindings.Length == 0)
			throw source.Error("Outfit Variable component has no characteristic bindings.");
		if (bindings.Select(x => x.Definition.Id).Distinct().Count() != bindings.Length)
			throw source.Error("Outfit entry repeats a runtime characteristic definition.");
		var text = arguments ?? "";
		var expression = new Regex("\\G\\s*(?<variable>\\w+)[=:](?:\"(?<quoted>[^\"\\r\\n]+)\"|(?<bare>[^\\s\"]+))", RegexOptions.CultureInvariant);
		var selected = new HashSet<long>();
		var offset = 0;
		while (offset < text.Length && !string.IsNullOrWhiteSpace(text[offset..]))
		{
			var match = expression.Match(text, offset);
			if (!match.Success) throw source.Error($"Invalid outfit load arguments near {text[offset..]}.");
			offset = match.Index + match.Length;
			var variable = match.Groups["variable"].Value;
			var targets = bindings.Where(x => Matches(x.Definition.Pattern, variable, source)).ToArray();
			if (targets.Length != 1) throw source.Error($"Outfit variable {variable} has no unique runtime characteristic binding.");
			var binding = targets[0];
			if (!selected.Add(binding.Definition.Id)) throw source.Error($"Outfit repeats the colour selection for {variable}.");
			var choice = match.Groups["quoted"].Success ? match.Groups["quoted"].Value : match.Groups["bare"].Value;
			if (choice.StartsWith(':')) throw source.Error($"Outfit {variable} must select a value, not a random characteristic profile.");
			var candidates = long.TryParse(choice, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
				? _values.Where(x => x.Id == id).ToArray()
				: _values.Where(x => IsValue(binding.Definition, x, source) && x.Name.StartsWith(choice, StringComparison.InvariantCultureIgnoreCase)).ToArray();
			if (candidates.Length != 1 || !IsValue(binding.Definition, candidates[0], source))
				throw source.Error($"Outfit colour {choice} is missing, ambiguous or incompatible with {variable}.");
			var value = candidates[0];
			var permitted = authored is null
				? ProfileValues(binding.Profile, source, []).Select(x => x.Id).ToHashSet()
				: authored.Values.Where(x => x.DefinitionId == binding.Definition.Id).SelectMany(x => x.Values.Values).ToHashSet();
			if (!permitted.Contains(value.Id) || value.OngoingProgId is not null)
				throw source.Error($"Outfit colour {choice} is outside the permitted palette or has unresolved ongoing validity.");
		}
		if (selected.Count != bindings.Length)
			throw source.Error("Outfit must explicitly select every runtime colour channel; omitted values would randomise.");
	}

	private IEnumerable<Value> ProfileValues(Profile profile, ClothingSourceLocation source, HashSet<long> visiting)
	{
		if (!visiting.Add(profile.Id))
		{
			throw source.Error($"Cyclic characteristic profile dependency at {profile.Name}.");
		}

		var definition = DefinitionById(profile.TargetId, source);
		var result = new List<Value>();
		var root = ParseXml(profile.Xml, source);
		switch (profile.Type.ToLowerInvariant())
		{
			case "all":
				result.AddRange(_values.Where(x => IsValue(definition, x, source)));
				break;
			case "standard":
			case "weighted":
				foreach (var element in root.Elements("Value"))
				{
					var value = long.TryParse(element.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
						? ById(_values, x => x.Id, id, source, "characteristic value")
						: Exact(_values.Where(x => IsValue(definition, x, source)), x => x.Name, element.Value, source, "characteristic value");
					if (!IsValue(definition, value, source))
					{
						throw source.Error($"Profile {profile.Name} contains a value incompatible with its target definition.");
					}
					if (profile.Type.Equals("weighted", StringComparison.OrdinalIgnoreCase) &&
						(!double.TryParse(element.Attribute("weight")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var weight) ||
						 !double.IsFinite(weight) || weight <= 0))
					{
						throw source.Error($"Profile {profile.Name} has a non-positive or invalid characteristic weight.");
					}
					result.Add(value);
				}
				break;
			case "compound":
				foreach (var element in root.Elements("Profile"))
				{
					var child = long.TryParse(element.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
						? ById(_profiles, x => x.Id, id, source, "characteristic profile")
						: Exact(_profiles, x => x.Name, element.Value, source, "characteristic profile");
					result.AddRange(ProfileValues(child, source, visiting));
				}
				break;
			default:
				throw source.Error($"Unsupported characteristic profile type {profile.Type}.");
		}

		visiting.Remove(profile.Id);
		return result.DistinctBy(x => x.Id).ToArray();
	}

	private bool IsValue(Definition definition, Value value, ClothingSourceLocation source)
	{
		var visited = new HashSet<long>();
		var current = definition;
		var matches = false;
		while (true)
		{
			if (!visited.Add(current.Id))
			{
				throw source.Error($"Cyclic characteristic definition ancestry at {current.Name}.");
			}
			if (current.Model is not ("standard" or "bodypart"))
			{
				throw source.Error($"Unsupported characteristic definition model {current.Model}.");
			}
			matches |= current.Id == value.DefinitionId;
			if (current.ParentId is null)
			{
				return matches;
			}
			current = DefinitionById(current.ParentId.Value, source);
		}
	}

	private Definition DefinitionById(long id, ClothingSourceLocation source) => ById(_definitions, x => x.Id, id, source, "characteristic definition");
	private static T ById<T>(IEnumerable<T> values, Func<T, long> id, long expected, ClothingSourceLocation source, string kind) =>
		Single(values.Where(x => id(x) == expected), source, $"{kind} ID {expected}");
	private static T Exact<T>(IEnumerable<T> values, Func<T, string> name, string expected, ClothingSourceLocation source, string kind) =>
		Single(values.Where(x => name(x).Equals(expected, StringComparison.OrdinalIgnoreCase)), source, $"{kind} '{expected}'");
	private static T Single<T>(IEnumerable<T> values, ClothingSourceLocation source, string description)
	{
		var matches = values.Take(2).ToArray();
		return matches.Length == 1 ? matches[0] : throw source.Error($"Missing or ambiguous {description}.");
	}

	private static long ReadId(string? text, ClothingSourceLocation source) =>
		long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var value) && value > 0
			? value : throw source.Error($"Invalid characteristic binding ID '{text}'.");

	private static XElement ParseXml(string text, ClothingSourceLocation source)
	{
		try { return XElement.Parse(text); }
		catch (Exception ex) when (ex is XmlException or ArgumentException)
		{
			throw source.Error($"Invalid characteristic XML: {ex.Message}");
		}
	}

	private static bool Matches(string pattern, string variable, ClothingSourceLocation source)
	{
		try { return Regex.IsMatch(variable, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)); }
		catch (Exception ex) when (ex is ArgumentException or RegexMatchTimeoutException)
		{
			throw source.Error($"Invalid or excessive characteristic pattern: {ex.Message}");
		}
	}
}
