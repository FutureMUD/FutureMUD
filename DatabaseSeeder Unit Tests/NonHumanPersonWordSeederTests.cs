#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using ModelDefinition = MudSharp.Models.CharacteristicDefinition;
using ModelValue = MudSharp.Models.CharacteristicValue;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NonHumanPersonWordSeederTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

	private static ModelDefinition AddDefinition(FuturemudDatabaseContext context)
	{
		var definition = new ModelDefinition
		{
			Name = "Person Word", Pattern = "person", Description = "Person word",
			Model = "standard", Definition = string.Empty
		};
		context.CharacteristicDefinitions.Add(definition);
		context.SaveChanges();
		return definition;
	}

	[TestMethod]
	[DataRow("Mythical")]
	[DataRow("Supernatural")]
	public void FreshWords_CompiledGatesAllowOnlyTheirRacesAndDescendants(string package)
	{
		using var context = Context();
		var definition = AddDefinition(context);
		var catalogue = package == "Mythical"
			? MythicalAnimalSeeder.PersonWordsByRace
			: SupernaturalSeeder.PersonWordsByRace;
		NonHumanPersonWordSeederHelper.Ensure(context, definition, package, catalogue);
		Assert.IsFalse(NonHumanPersonWordSeederHelper.HasMissing(context, catalogue));

		var races = new All<IRace>();
		var mocks = new Dictionary<string, Mock<IRace>>();
		foreach (var name in catalogue.Keys.Append("Human").Append("Unrelated Race"))
		{
			var race = new Mock<IRace>();
			race.SetupGet(x => x.Id).Returns(mocks.Count + 1);
			race.SetupGet(x => x.Name).Returns(name);
			race.SetupGet(x => x.Type).Returns(ProgVariableTypes.Race);
			race.SetupGet(x => x.GetObject).Returns(race.Object);
			race.Setup(x => x.SameRace(It.IsAny<IRace>())).Returns((IRace other) => other == race.Object);
			mocks.Add(name, race);
			races.Add(race.Object);
		}

		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Races).Returns(races);
		var progs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		MudSharp.FutureProg.FutureProg.Initialise();
		foreach (var row in context.FutureProgs.Include(x => x.FutureProgsParameters))
		{
			var prog = FutureProgFactory.CreateNew(row, world.Object);
			Assert.IsTrue(prog.Compile(), $"{row.FunctionName}: {prog.CompileError}");
			progs.Add(prog);
		}

		var runtimeDefinition = new MudSharp.Form.Characteristics.CharacteristicDefinition(definition, world.Object);
		var definitions = new All<ICharacteristicDefinition>();
		definitions.Add(runtimeDefinition);
		world.SetupGet(x => x.Characteristics).Returns(definitions);
		var values = new All<ICharacteristicValue>();
		foreach (var row in context.CharacteristicValues)
		{
			Assert.IsNotNull(row.FutureProgId, row.Name);
			values.Add(new MudSharp.Form.Characteristics.CharacteristicValue(row, world.Object));
		}
		world.SetupGet(x => x.CharacteristicValues).Returns(values);
		var allProfile = new CharacterProfileAll(new MudSharp.Models.CharacteristicProfile
		{
			Name = "All Person Words", TargetDefinitionId = definition.Id, Type = "All",
			Description = "All values", Definition = "<Definition/>"
		}, world.Object);

		foreach (var (name, race) in mocks)
		{
			var chargen = new Mock<IChargen>();
			chargen.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
			chargen.SetupGet(x => x.GetObject).Returns(chargen.Object);
			chargen.Setup(x => x.GetProperty("race")).Returns(race.Object);
			foreach (var value in allProfile.Values)
			{
				var expected = catalogue.TryGetValue(name, out var words) && words.Contains(value.Name);
				Assert.AreEqual(expected, value.ChargenApplicabilityProg.Execute<bool?>(chargen.Object),
					$"{name} / {value.Name}");
			}

			var random = allProfile.GetRandomCharacteristic(chargen.Object);
			if (catalogue.TryGetValue(name, out var allowed))
			{
				Assert.IsNotNull(random);
				CollectionAssert.Contains(allowed.ToArray(), random.Name);
			}
			else
			{
				Assert.IsNull(random, "An All profile must not leak racial words into human or unrelated NPC/chargen choices.");
			}
		}

		var parent = mocks[catalogue.Keys.First()].Object;
		var child = new Mock<IRace>();
		child.SetupGet(x => x.GetObject).Returns(child.Object);
		child.SetupGet(x => x.Type).Returns(ProgVariableTypes.Race);
		child.Setup(x => x.SameRace(It.IsAny<IRace>())).Returns((IRace other) => other == parent);
		var descendant = new Mock<IChargen>();
		descendant.SetupGet(x => x.Type).Returns(ProgVariableTypes.Chargen);
		descendant.SetupGet(x => x.GetObject).Returns(descendant.Object);
		descendant.Setup(x => x.GetProperty("race")).Returns(child.Object);
		Assert.IsNotNull(allProfile.GetRandomCharacteristic(descendant.Object));
	}

	[TestMethod]
	[DataRow("Mythical")]
	[DataRow("Supernatural")]
	public void LegacyWords_RepairInPlaceAndRerunWithoutDuplicates(string package)
	{
		using var context = Context();
		var definition = AddDefinition(context);
		var catalogue = package == "Mythical" ? MythicalAnimalSeeder.PersonWordsByRace : SupernaturalSeeder.PersonWordsByRace;
		foreach (var word in catalogue.Values.SelectMany(x => x).Distinct())
		{
			context.CharacteristicValues.Add(new ModelValue
			{
				Name = word, Value = word, Definition = definition, AdditionalValue = string.Empty
			});
		}
		foreach (var (race, words) in catalogue)
		{
			context.CharacteristicProfiles.Add(new MudSharp.Models.CharacteristicProfile
			{
				Name = $"{race} Person Words", TargetDefinition = definition, Type = "Standard",
				Description = $"Auto-generated profile for {race} Person Words",
				Definition = "<Values> " + string.Join(" ", words.Select(x => $"<Value>{x}</Value>")) + " </Values>"
			});
		}
		context.SaveChanges();
		var ids = context.CharacteristicValues.ToDictionary(x => x.Name, x => x.Id);
		Assert.IsTrue(NonHumanPersonWordSeederHelper.HasMissing(context, catalogue));
		NonHumanPersonWordSeederHelper.Ensure(context, definition, package, catalogue);
		var gateIds = context.CharacteristicValues.ToDictionary(x => x.Name, x => x.FutureProgId);
		Assert.IsTrue(gateIds.Values.All(x => x.HasValue));
		context.ChangeTracker.Clear();
		definition = context.CharacteristicDefinitions.Single();
		NonHumanPersonWordSeederHelper.Ensure(context, definition, package, catalogue);
		Assert.IsFalse(NonHumanPersonWordSeederHelper.HasMissing(context, catalogue));
		Assert.AreEqual(ids.Count, context.CharacteristicValues.Count());
		Assert.AreEqual(ids.Count, context.FutureProgs.Count());
		foreach (var value in context.CharacteristicValues)
		{
			Assert.AreEqual(ids[value.Name], value.Id);
			Assert.AreEqual(gateIds[value.Name], value.FutureProgId);
		}
	}

	[TestMethod]
	public void Repair_PreservesCustomRulesValuesAndUnownedNameCollisions()
	{
		using var context = Context();
		var definition = AddDefinition(context);
		var catalogue = new Dictionary<string, IReadOnlyList<string>>
		{
			["Test Race"] = ["custom rule", "custom text", "custom validity"],
			["Unowned Race"] = ["plain"],
			["Custom Profile Race"] = ["custom profile"]
		};
		NonHumanPersonWordSeederHelper.Ensure(context, definition, "Test", catalogue);
		foreach (var race in new[] { "Test Race", "Custom Profile Race" })
		{
			context.CharacteristicProfiles.Add(new MudSharp.Models.CharacteristicProfile
			{
				Name = $"{race} Person Words", TargetDefinition = definition, Type = "Standard",
				Description = $"Auto-generated profile for {race} Person Words",
				Definition = new XElement("Values", catalogue[race]
					.Select(word => new XElement("Value", word))).ToString()
			});
		}
		context.SaveChanges();
		context.CharacteristicProfiles.Single(x => x.Name == "Custom Profile Race Person Words").Definition =
			"<Values><Value>custom profile</Value><Value>builder addition</Value></Values>";
		var values = context.CharacteristicValues.ToDictionary(x => x.Name);
		var originalGate = values["custom rule"].FutureProgId;
		values["custom rule"].FutureProg.FunctionText = "return false";
		values["custom text"].FutureProg = null!;
		values["custom text"].FutureProgId = null;
		values["custom text"].Value = "builder wording";
		values["custom validity"].OngoingValidityProgId = originalGate;
		values["custom validity"].FutureProg = null!;
		values["custom validity"].FutureProgId = null;
		values["plain"].FutureProg = null!;
		values["plain"].FutureProgId = null;
		values["custom profile"].FutureProg = null!;
		values["custom profile"].FutureProgId = null;
		context.SaveChanges();
		context.ChangeTracker.Clear();
		definition = context.CharacteristicDefinitions.Single();
		values = context.CharacteristicValues.Include(x => x.FutureProg).ToDictionary(x => x.Name);
		NonHumanPersonWordSeederHelper.Ensure(context, definition, "Test", catalogue);
		Assert.AreEqual(originalGate, values["custom rule"].FutureProgId);
		Assert.AreEqual("return false", values["custom rule"].FutureProg.FunctionText);
		Assert.IsNull(values["custom text"].FutureProgId);
		Assert.AreEqual("builder wording", values["custom text"].Value);
		Assert.IsNull(values["custom validity"].FutureProgId);
		Assert.IsNull(values["plain"].FutureProgId, "A plain value without the legacy stock profile is not owned.");
		Assert.IsNull(values["custom profile"].FutureProgId, "A modified profile must not establish legacy ownership.");
		Assert.IsFalse(NonHumanPersonWordSeederHelper.HasMissing(context, catalogue));
	}
}
