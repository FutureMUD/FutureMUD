#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

// Independent original-binary capture, not regenerated from the implementation under test.
internal static class FrozenCatalogueContract
{
	internal static void AssertTypedRecipes(IEnumerable<CatalogueRecipeDefinition> recipes)
	{
		using var expected = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
			"Fixtures", "item-catalogue-typed-recipe-digest.json")));
		using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		var count = 0;
		foreach (var recipe in recipes)
		{
			hash.AppendData(JsonSerializer.SerializeToUtf8Bytes(ProjectConcrete(recipe))); hash.AppendData([10]); count++;
		}
		Assert.AreEqual(expected.RootElement.GetProperty("Count").GetInt32(), count);
		Assert.AreEqual(expected.RootElement.GetProperty("Sha256").GetString(), Convert.ToHexString(hash.GetHashAndReset()),
			"The concrete typed graph differs from the stage 5 artifact independently verified against every original serializer output.");
	}

	private static JsonNode? ProjectConcrete(object? value)
	{
		if (value is null) return null;
		var type = value.GetType();
		if (value is string || type.IsPrimitive || value is decimal) return JsonSerializer.SerializeToNode(value, type);
		if (type.IsEnum) return JsonValue.Create(value.ToString());
		if (value is IEnumerable sequence)
		{
			var array = new JsonArray();
			foreach (var item in sequence) array.Add(ProjectConcrete(item));
			return array;
		}
		var result = new JsonObject { ["$type"] = type.FullName };
		foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(property => property.GetIndexParameters().Length == 0).OrderBy(property => property.Name, StringComparer.Ordinal))
			result[property.Name] = ProjectConcrete(property.GetValue(value));
		return result;
	}

	private static readonly JsonDocument Expected = JsonDocument.Parse(
		File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "item-catalogue-original-digests.json")));

	internal static void AssertScenario(string name, ItemSeeder seeder, CapturedCatalogueDocument actual)
	{
		using var reference = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
			"Fixtures", "item-catalogue-native-validated-digests.json")));
		var expected = reference.RootElement.GetProperty("Scenarios").EnumerateArray()
			.Single(row => row.GetProperty("Name").GetString() == name);
		var baseRows = actual.Definitions.Where(entry => entry.Identity.EntityType is not ("craft" or "knowledge") &&
			!(entry.Identity.EntityType == "prog" && Regex.IsMatch(entry.Identity.StableKey, "^Cr(Ap|Use|Why)")))
			.Select(entry => Project(entry, false)).ToArray();
		var knowledge = actual.Definitions.Where(entry => entry.Identity.EntityType == "knowledge")
			.Select(entry => Project(entry, false)).ToArray();
		var recipes = actual.Definitions.Where(entry => entry.Identity.EntityType == "craft")
			.Select(entry => Project(entry, true)).ToArray();
		Check("Base", baseRows); Check("Knowledge", knowledge); Check("Recipe", recipes);
		Assert.AreEqual(expected.GetProperty("GateProgCount").GetInt32(), actual.Definitions.Count(entry =>
			entry.Identity.EntityType == "prog" && Regex.IsMatch(entry.Identity.StableKey, "^Cr(Ap|Use|Why)")), name + " access gate programs");

		JsonArray Project(CapturedCatalogueDefinition entry, bool recipe)
		{
			var identity = JsonSerializer.SerializeToNode(entry.Identity, CatalogueDefinitionCapture.JsonOptions)!.AsObject();
			var definition = JsonNode.Parse(CatalogueDefinitionCapture.SymbolicDefinition(entry, seeder.CapturedItemKeys).GetRawText())!.AsObject();
			var supplement = JsonNode.Parse(entry.Supplement.GetRawText());
			if (!recipe) return new(identity, definition, supplement);
			identity.Remove("fingerprint");
			var gate = seeder.CapturedRecipeGates.TryGetValue(entry.Identity.StableKey, out var contract)
				? JsonSerializer.SerializeToNode(contract, contract.GetType(), CatalogueDefinitionCapture.JsonOptions) : null;
			if (gate is not null)
				foreach (var field in new[] { "trait", "appearProg", "canUseProg", "whyCannotUseProg" }) definition[field] = null;
			return new(identity, definition, supplement, gate);
		}

		void Check(string kind, JsonArray[] rows)
		{
			Assert.AreEqual(expected.GetProperty(kind + "Count").GetInt32(), rows.Length, name + " " + kind);
			using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
			foreach (var row in rows)
			{
				using var stream = new MemoryStream();
				using (var writer = new Utf8JsonWriter(stream)) WriteCanonical(writer, JsonSerializer.SerializeToElement(row));
				hash.AppendData(stream.ToArray()); hash.AppendData([10]);
			}
			Assert.AreEqual(expected.GetProperty(kind + "Sha256").GetString(), Convert.ToHexString(hash.GetHashAndReset()),
				$"{name}: {kind} differs from independent original data or the native-qualified corrected recipe oracle.");
		}
	}

	internal static void AssertSourceGraphs()
	{
		var actual = JsonSerializer.SerializeToElement(new ItemSeeder().CaptureSourceGraphs(), CatalogueDefinitionCapture.JsonOptions);
		var expected = Expected.RootElement.GetProperty("SourceGraphs");
		Assert.AreEqual(expected.GetArrayLength(), actual.EnumerateObject().Count());
		foreach (var graph in expected.EnumerateArray())
			AssertGraph(graph.GetProperty("Name").GetString()!, actual);
	}

	internal static void AssertGraph(string name, JsonElement? graphs = null)
	{
		var actual = graphs ?? JsonSerializer.SerializeToElement(new ItemSeeder().CaptureSourceGraphs(), CatalogueDefinitionCapture.JsonOptions);
		var expected = Expected.RootElement.GetProperty("SourceGraphs").EnumerateArray()
			.Single(row => row.GetProperty("Name").GetString() == name);
		using var stream = new MemoryStream();
		using (var writer = new Utf8JsonWriter(stream)) WriteCanonical(writer, actual.GetProperty(name));
		Assert.AreEqual(expected.GetProperty("Sha256").GetString(), Convert.ToHexString(SHA256.HashData(stream.ToArray())), name);
	}

	private static void WriteCanonical(Utf8JsonWriter writer, JsonElement element)
	{
		if (element.ValueKind == JsonValueKind.Object)
		{
			writer.WriteStartObject();
			foreach (var property in element.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
			{
				writer.WritePropertyName(property.Name); WriteCanonical(writer, property.Value);
			}
			writer.WriteEndObject();
		}
		else if (element.ValueKind == JsonValueKind.Array)
		{
			writer.WriteStartArray();
			foreach (var child in element.EnumerateArray()) WriteCanonical(writer, child);
			writer.WriteEndArray();
		}
		else element.WriteTo(writer);
	}
}
