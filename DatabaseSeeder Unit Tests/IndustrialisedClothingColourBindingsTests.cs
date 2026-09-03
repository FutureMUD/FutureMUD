#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingColourBindingsTests
{
	private static readonly ClothingSourceLocation Source = new("Clothing/colours.tsv", 2);
	private static readonly CharacteristicDefinition Definition = new()
	{
		Id = 11, Name = "Garment Colour", Pattern = "^colour$", Model = "standard", Description = "Garment dye"
	};
	private static CharacteristicValue[] Values() =>
	[
		new() { Id = 21, Name = "blue", DefinitionId = 11, Value = "blue" },
		new() { Id = 22, Name = "cream", DefinitionId = 11, Value = "cream" },
		new() { Id = 23, Name = "black", DefinitionId = 11, Value = "black" }
	];
	private static CharacteristicProfile Profile(string type = "all", string xml = "<Values/>") => new()
	{
		Id = 31, Name = "All Colours", Type = type, TargetDefinitionId = 11, Definition = xml, Description = "Permitted garment dyes"
	};
	private static GameItemComponentProto Component(string xml = "<Definition><Characteristic Value=\"11\" Profile=\"31\"/></Definition>") =>
		new() { Id = 41, Name = "Variable_Garment", Type = "Variable", Definition = xml };
	private static IReadOnlyDictionary<string, ClothingColourRow> Channels() => IndustrialisedClothingColourPlan.Channels(
		IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture()), "coat", "");

	[TestMethod]
	public void ExactBinding_ProducesStableNumericArgumentsAndDoesNotMutateDatabase()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		context.CharacteristicDefinitions.Add(Definition);
		context.CharacteristicProfiles.Add(Profile());
		context.CharacteristicValues.AddRange(Values());
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var snapshot = IndustrialisedClothingColourBindings.Read(context);
		var bindings = snapshot.Bind(Channels(), [Component()], Source);
		Assert.AreEqual("colour=22", IndustrialisedClothingColourBindings.LoadArguments(bindings,
			new Dictionary<string, string> { ["colour"] = "cream" }, Source));
		Assert.AreEqual("blue", bindings["colour"].DefaultValue);
		Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
		Assert.ThrowsException<InvalidDataException>(() => snapshot.Bind(Channels(), [], Source));
		Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
	}

	[TestMethod]
	public void CraftInheritance_RequiresTheOutputToAdmitEveryValueAcceptedByTheInput()
	{
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile()], Values());
		var complete = snapshot.Bind(Channels(), [Component()], Source)["colour"];
		snapshot.ValidateUnrestrictedCraftInheritance(complete, Source);
		var narrowed = complete with { Values = complete.Values.Where(x => x.Key != "black").ToDictionary(x => x.Key, x => x.Value) };
		var error = Assert.ThrowsException<InvalidDataException>(() => snapshot.ValidateUnrestrictedCraftInheritance(narrowed, Source));
		StringAssert.Contains(error.Message, "outside this product palette");
		StringAssert.Contains(error.Message, "Clothing/colours.tsv:2");
	}

	[TestMethod]
	public void StandaloneBase_RejectsProfileValuesOutsideItsAuthoredPalette()
	{
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile()], Values());
		var channels = Channels().ToDictionary(x => x.Key, x => x.Value with { AllowedValues = ["blue", "cream"] });
		var error = Assert.ThrowsException<InvalidDataException>(() => snapshot.Bind(channels, [Component()], Source));
		StringAssert.Contains(error.Message, "Standalone");
		StringAssert.Contains(error.Message, "black");
		StringAssert.Contains(error.Message, "Clothing/colours.tsv:2");
	}

	[TestMethod]
	public void StandaloneBase_AcceptsAnExactRestrictedStockProfile()
	{
		var snapshot = new IndustrialisedClothingColourBindings([Definition],
			[Profile("standard", "<Values><Value>21</Value><Value>22</Value></Values>")], Values());
		var channels = Channels().ToDictionary(x => x.Key, x => x.Value with { AllowedValues = ["blue", "cream"] });
		CollectionAssert.AreEquivalent(new[] { "blue", "cream" }, snapshot.Bind(channels, [Component()], Source)["colour"].Values.Keys.ToArray());
	}

	[TestMethod]
	public void ControlledSkinSelection_CanNarrowThePaletteWithoutChangingTheBaseProfile()
	{
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile()], Values());
		var channels = Channels().ToDictionary(x => x.Key, x => x.Value with { AllowedValues = ["blue", "cream"] });
		var skin = snapshot.Bind(channels, [Component()], Source, requireStandaloneProfile: false);
		Assert.AreEqual("colour=22", IndustrialisedClothingColourBindings.LoadArguments(skin,
			new Dictionary<string, string> { ["colour"] = "cream" }, Source));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourBindings.LoadArguments(skin,
			new Dictionary<string, string> { ["colour"] = "black" }, Source));
		Assert.AreEqual(3, snapshot.Bind(Channels(), [Component()], Source)["colour"].Values.Count);
	}

	[DataTestMethod]
	[DataRow("", "explicitly select")]
	[DataRow("colour=:31", "not a random")]
	[DataRow("colour=999", "missing, ambiguous or incompatible")]
	[DataRow("colour=21 colour=22", "repeats")]
	[DataRow("unknown=21", "no unique runtime")]
	[DataRow("colour=21 stray", "Invalid outfit load arguments")]
	public void PersistedOutfitArguments_RejectMissingRandomMalformedOrIncompatibleValues(string arguments, string diagnostic)
	{
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile()], Values());
		var binding = snapshot.Bind(Channels(), [Component()], Source);
		var error = Assert.ThrowsException<InvalidDataException>(() =>
			snapshot.ValidatePersistedLoadArguments(arguments, [Component()], Source, binding));
		StringAssert.Contains(error.Message, diagnostic);
		StringAssert.Contains(error.Message, Source.ToString());
	}

	[TestMethod]
	public void PersistedOutfitArguments_AcceptsAnExactPermittedValueByIdOrUniqueName()
	{
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile()], Values());
		var binding = snapshot.Bind(Channels(), [Component()], Source);
		snapshot.ValidatePersistedLoadArguments("colour=21", [Component()], Source, binding);
		snapshot.ValidatePersistedLoadArguments("colour=cream", [Component()], Source, binding);
	}

	[DataTestMethod]
	[DataRow("standard", "<Values><Value>21</Value><Value>cream</Value><Value>23</Value></Values>")]
	[DataRow("weighted", "<Values><Value weight=\"1\">21</Value><Value weight=\"2\">22</Value><Value weight=\"1\">23</Value></Values>")]
	[DataRow("compound", "<Definition><Profile>32</Profile></Definition>")]
	public void RuntimeProfileFamilies_ResolveAllPermittedValues(string type, string xml)
	{
		var child = Profile();
		child.Id = 32;
		child.Name = "Underlying colours";
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile(type, xml), child], Values());
		Assert.AreEqual(3, snapshot.Bind(Channels(), [Component()], Source)["colour"].Values.Count);
	}

	[DataTestMethod]
	[DataRow("missing-definition")]
	[DataRow("ambiguous-definition")]
	[DataRow("ambiguous-value")]
	[DataRow("ambiguous-profile")]
	[DataRow("mismatched-component")]
	[DataRow("duplicate-component-definition")]
	[DataRow("undeclared-variable")]
	[DataRow("wrong-pattern")]
	[DataRow("bad-pattern")]
	[DataRow("overlapping-patterns")]
	[DataRow("bad-xml")]
	[DataRow("empty-profile")]
	[DataRow("profile-cycle")]
	[DataRow("ancestry-cycle")]
	[DataRow("missing-parent")]
	[DataRow("incompatible-value")]
	[DataRow("outside-profile")]
	[DataRow("invalid-weight")]
	[DataRow("ongoing-prog")]
	public void InvalidDependencies_FailClosedWithSourceLine(string defect)
	{
		var definition = new CharacteristicDefinition { Id = 11, Name = "Garment Colour", Pattern = "^colour$", Model = "standard" };
		var definitions = new List<CharacteristicDefinition> { definition };
		var profiles = new List<CharacteristicProfile> { Profile() };
		var values = Values().ToList();
		var component = Component();
		switch (defect)
		{
			case "missing-definition": definitions.Clear(); break;
			case "ambiguous-definition": definitions.Add(new() { Id = 12, Name = "garment colour" }); break;
			case "ambiguous-value": values.Add(new() { Id = 24, Name = "BLUE", DefinitionId = 11 }); break;
			case "ambiguous-profile": profiles.Add(Profile()); break;
			case "mismatched-component": component = Component("<Definition><Characteristic Value=\"11\" Profile=\"32\"/></Definition>"); break;
			case "duplicate-component-definition": component.Definition = component.Definition.Replace("</Definition>", "<Characteristic Value=\"11\" Profile=\"31\"/></Definition>"); break;
			case "undeclared-variable":
			case "overlapping-patterns":
				definitions.Add(new() { Id = 12, Name = "Accent", Pattern = defect == "overlapping-patterns" ? ".*" : "^accent$", Model = "standard" });
				component.Definition = component.Definition.Replace("</Definition>", "<Characteristic Value=\"12\" Profile=\"31\"/></Definition>");
				break;
			case "wrong-pattern": definition.Pattern = "^accent$"; break;
			case "bad-pattern": definition.Pattern = "["; break;
			case "bad-xml": component.Definition = "<broken"; break;
			case "empty-profile": profiles[0] = Profile("standard"); break;
			case "profile-cycle": profiles[0] = Profile("compound", "<Definition><Profile>31</Profile></Definition>"); break;
			case "ancestry-cycle": definition.ParentId = 11; break;
			case "missing-parent": definition.ParentId = 99; break;
			case "incompatible-value":
				values[0].DefinitionId = 99;
				profiles[0] = Profile("standard", "<Values><Value>21</Value><Value>22</Value><Value>23</Value></Values>");
				break;
			case "outside-profile": profiles[0] = Profile("standard", "<Values><Value>21</Value><Value>22</Value></Values>"); break;
			case "invalid-weight": profiles[0] = Profile("weighted", "<Values><Value weight=\"NaN\">21</Value></Values>"); break;
			case "ongoing-prog": values[0].OngoingValidityProgId = 1; break;
		}
		var snapshot = new IndustrialisedClothingColourBindings(definitions, profiles, values);
		var ex = Assert.ThrowsException<InvalidDataException>(() => snapshot.Bind(Channels(), [component], Source));
		StringAssert.Contains(ex.Message, "Clothing/colours.tsv:2");
	}

	[TestMethod]
	public void AuthoredOutfitEntry_BindsPaletteAndExactGarmentWearProfile()
	{
		var document = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture());
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [Profile()], Values());
		var colours = snapshot.Bind(Channels(), [Component()], Source);
		var wearable = new GameItemComponentProto
		{
			Type = "Wearable", Definition = "<Definition><Profiles Default=\"51\"><Profile>51</Profile></Profiles></Definition>"
		};
		var wearProfile = new WearProfile { Id = 51, Name = "Coat" };
		var entry = document.OutfitEntries.Single() with { SkinReference = "" };
		var bound = ItemSeeder.BindClothingOutfitEntry(document, entry, [Component(), wearable], colours, [wearProfile]);
		Assert.AreEqual("colour=22", bound.LoadArguments);
		Assert.AreEqual("Coat", bound.WearProfile);
		Assert.AreEqual("coat_entry", bound.EntryKey);
		Assert.IsNull(bound.SkinStableReference);
		wearProfile.Id = 52;
		StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() =>
			ItemSeeder.BindClothingOutfitEntry(document, entry, [Component(), wearable], colours, [wearProfile])).Message,
			"not supported by this garment");
	}

	[TestMethod]
	public void SnapshotIsImmutableAndExplicitInvalidChoicesNeverFallBack()
	{
		var values = Values();
		var profile = Profile();
		var snapshot = new IndustrialisedClothingColourBindings([Definition], [profile], values);
		values[0].Name = "changed";
		profile.Definition = "<broken";
		var bindings = snapshot.Bind(Channels(), [Component()], Source);
		Assert.AreEqual(21, bindings["colour"].Values["blue"]);
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourBindings.LoadArguments(bindings,
			new Dictionary<string, string> { ["colour"] = "red" }, Source));
		Assert.ThrowsException<InvalidDataException>(() => IndustrialisedClothingColourBindings.LoadArguments(bindings,
			new Dictionary<string, string>(), Source));
	}
}
