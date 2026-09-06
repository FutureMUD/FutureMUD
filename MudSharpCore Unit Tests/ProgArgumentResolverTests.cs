#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine;
using MudSharp.Traps;
using MudSharp.Vehicles;
using MudSharp.Work.Agriculture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProgArgumentResolverTests
{
	private Mock<IFuturemud> _world = null!;
	private Mock<ICharacter> _actor = null!;
	private Mock<ICell> _cell = null!;
	private List<string> _messages = null!;

	[TestInitialize]
	public void Initialise()
	{
		_world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		_actor = new Mock<ICharacter>();
		_cell = Reference<ICell>(ProgVariableTypes.Location);
		_messages = new List<string>();
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => _messages.Add(text));
		_actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		_actor.SetupGet(x => x.Gameworld).Returns(_world.Object);
		_actor.SetupGet(x => x.Location).Returns(_cell.Object);
		_actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		_actor.SetupGet(x => x.GetObject).Returns(_actor.Object);
		_actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns((Type type) => CultureInfo.InvariantCulture.GetFormat(type));
		_cell.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
	}

	public static IEnumerable<object[]> ConcreteTypes => ProgVariableTypes.CollectionItem.GetAllFlags()
		.Where(x => x.IsExactType && x != ProgVariableTypes.Void)
		.Select(x => new object[] { x.Describe() });

	[DataTestMethod]
	[DynamicData(nameof(ConcreteTypes), DynamicDataSourceType.Property)]
	public void GetArgument_EveryCurrentConcreteValueType_HasResolver(string name)
	{
		var type = FutureProg.GetTypeByName(name);
		ProgModule.GetArgument(type, "missing", 1, _actor.Object);
		Assert.IsFalse(_messages.Any(x => x.Contains("not yet supported", StringComparison.OrdinalIgnoreCase)), name);
	}

	[DataTestMethod]
	[DataRow("void")]
	[DataRow("error")]
	[DataRow("anything")]
	[DataRow("literal")]
	[DataRow("collection")]
	[DataRow("dictionary")]
	[DataRow("collectiondictionary")]
	public void GetArgument_CompilerMasksAndBareModifiers_AreNotInputValues(string name)
	{
		Assert.IsFalse(ProgModule.GetArgument(FutureProg.GetTypeByName(name), "number 5", 1, _actor.Object).success);
	}

	[DataTestMethod]
	[DataRow("location")]
	[DataRow("zone")]
	[DataRow("shard")]
	public void GetArgument_MissingSpatialId_FailsButExplicitNullSucceeds(string name)
	{
		_world.SetupGet(x => x.Cells).Returns(new Mock<IUneditableAll<ICell>>().Object);
		_world.SetupGet(x => x.Zones).Returns(new Mock<IUneditableAll<IZone>>().Object);
		_world.SetupGet(x => x.Shards).Returns(new Mock<IUneditableAll<IShard>>().Object);
		var type = FutureProg.GetTypeByName(name);
		Assert.IsFalse(ProgModule.GetArgument(type, "999999", 1, _actor.Object).success);
		var result = ProgModule.GetArgument(type, "null", 1, _actor.Object);
		Assert.IsTrue(result.success);
		Assert.IsNull(result.result);
	}

	[DataTestMethod]
	[DynamicData(nameof(ConcreteTypes), DynamicDataSourceType.Property)]
	public void GetArgument_ExplicitNull_HasDocumentedTypeSemantics(string name)
	{
		var type = FutureProg.GetTypeByName(name);
		if (!type.CompatibleWith(ProgVariableTypes.ReferenceType) && type != ProgVariableTypes.PersonalName) return;
		var result = ProgModule.GetArgument(type, "null", 1, _actor.Object);
		Assert.IsTrue(result.success, name);
		Assert.IsNull(result.result, name);
		Assert.AreEqual(type, FutureProg.GetVariable(type, result.result).Type);
	}

	[TestMethod]
	public void GetArgument_AddedWorldReferences_ResolveIdsAndNames()
	{
		var field = Reference<IAgricultureField>(ProgVariableTypes.AgricultureField);
		var route = Reference<IVehicleRoute>(ProgVariableTypes.VehicleRoute);
		var service = Reference<IVehicleService>(ProgVariableTypes.VehicleService);
		var journey = Reference<IVehicleJourney>(ProgVariableTypes.VehicleJourney);
		var package = Reference<INPCSkillPackage>(ProgVariableTypes.NPCSkillPackage);
		var language = Reference<ISignedLanguage>(ProgVariableTypes.SignedLanguage);
		_world.SetupGet(x => x.AgricultureFields).Returns(Registry(field.Object));
		var routes = new Mock<IUneditableRevisableAll<IVehicleRoute>>();
		routes.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns(route.Object);
		_world.SetupGet(x => x.VehicleRoutes).Returns(routes.Object);
		_world.SetupGet(x => x.VehicleServices).Returns(Registry(service.Object));
		_world.SetupGet(x => x.VehicleJourneys).Returns(Registry(journey.Object));
		_world.SetupGet(x => x.NpcSkillPackages).Returns(Registry(package.Object));
		_world.SetupGet(x => x.SignedLanguages).Returns(Registry(language.Object));
		foreach (var item in new IProgVariable[] { field.Object, route.Object, service.Object, journey.Object, package.Object, language.Object })
		{
			foreach (var input in new[] { "7", "Example Name" })
			{
				var result = ProgModule.GetArgument(item.Type, input, 1, _actor.Object);
				Assert.IsTrue(result.success, item.Type.Describe());
				Assert.AreSame(item, result.result);
				Assert.AreSame(item, FutureProg.GetVariable(item.Type, result.result));
				Assert.IsFalse(ProgModule.DescribeProgVariable(_actor.Object, item.Type, result.result!).Contains("undisplayable"));
			}
		}
	}

	[DataTestMethod]
	[DataRow("\"\" next", "", "next")]
	[DataRow("() next", "", "next")]
	[DataRow("(\"a)b\" (c d)) next", "\"a)b\" (c d)", "next")]
	[DataRow("“two words” next", "two words", "next")]
	public void TryPopArgument_ValidGroups_PreservesContentAndNextArgument(string text, string expected, string next)
	{
		var input = new StringStack(text);
		Assert.IsTrue(ProgModule.TryPopArgument(ref input, out var argument, out var error), error);
		Assert.AreEqual(expected, argument);
		Assert.AreEqual(next, input.RemainingArgument);
	}

	[DataTestMethod]
	[DataRow("(one two")]
	[DataRow("\"unclosed")]
	[DataRow("(\"unclosed)")]
	[DataRow("(value)garbage")]
	[DataRow("\"value\"garbage")]
	public void TryPopArgument_MalformedGroup_FailsWithoutConsuming(string text)
	{
		var input = new StringStack(text);
		Assert.IsFalse(ProgModule.TryPopArgument(ref input, out _, out _));
		Assert.AreEqual(text, input.RemainingArgument);
	}

	[TestMethod]
	public void GetArgument_DictionariesAndCollectionDictionaries_RoundTripAndDisplay()
	{
		var type = ProgVariableTypes.Text | ProgVariableTypes.Dictionary;
		var result = ProgModule.GetArgumentFromRemainingInput(type, new StringStack("(\"first key\" \"first value\" second \"\")"), 1, _actor.Object);
		Assert.IsTrue(result.success);
		var variable = FutureProg.GetVariable(type, result.result);
		var values = (IDictionary)variable.GetObject;
		Assert.AreEqual("first value", ((IProgVariable)values["first key"]!).GetObject);
		Assert.AreEqual("", ((IProgVariable)values["second"]!).GetObject);
		StringAssert.Contains(ProgModule.DescribeProgVariable(_actor.Object, type, variable), "first value");

		type = ProgVariableTypes.Text | ProgVariableTypes.CollectionDictionary;
		result = ProgModule.GetArgumentFromRemainingInput(type, new StringStack("(group (\"first value\" second) empty ())"), 1, _actor.Object);
		Assert.IsTrue(result.success);
		variable = FutureProg.GetVariable(type, result.result);
		var groups = (CollectionDictionary<string, IProgVariable>)variable.GetObject;
		Assert.AreEqual(2, groups["group"].Count);
		Assert.IsTrue(groups.ContainsKey("empty"));
		Assert.AreEqual(0, groups["empty"].Count);
		StringAssert.Contains(ProgModule.DescribeProgVariable(_actor.Object, type, variable), "group:");
	}

	[DataTestMethod]
	[DataRow("a 1 a 2")]
	[DataRow("a")]
	[DataRow("a invalid")]
	[DataRow("a (1")]
	public void GetArgument_InvalidDictionary_DoesNotReturnPartialValue(string text)
	{
		var result = ProgModule.GetArgument(ProgVariableTypes.Number | ProgVariableTypes.Dictionary, text, 1, _actor.Object);
		Assert.IsFalse(result.success);
		Assert.IsNull(result.result);
	}

	[TestMethod]
	public void GetArgument_Union_RequiresCompatibleTypeAndPreservesValue()
	{
		var result = ProgModule.GetArgument(ProgVariableTypes.ValueType, "number 25", 1, _actor.Object);
		Assert.IsTrue(result.success);
		Assert.AreEqual(25m, ((IProgVariable)result.result!).GetObject);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.Material, "number 25", 1, _actor.Object).success);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.Perceiver, "here", 1, _actor.Object).success);
		Assert.AreSame(_cell.Object, ProgModule.GetArgument(ProgVariableTypes.Perceivable, "here", 1, _actor.Object).result);
		Assert.AreSame(_actor.Object, ProgModule.GetArgument(ProgVariableTypes.Toon, "self", 1, _actor.Object).result);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.Toon, "", 1, _actor.Object).success);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.Toon, "*", 1, _actor.Object).success);
	}

	[TestMethod]
	public void GetArgument_CultureAndGender_SurviveConversion()
	{
		_actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns((Type type) => CultureInfo.GetCultureInfo("de-DE").GetFormat(type));
		Assert.AreEqual(1.25m, ProgModule.GetArgument(ProgVariableTypes.Number, "1,25", 1, _actor.Object).result);
		Assert.AreEqual(new DateTime(2026, 12, 31), ProgModule.GetArgument(ProgVariableTypes.DateTime, "31.12.2026", 1, _actor.Object).result);
		var gender = ProgModule.GetArgument(ProgVariableTypes.Gender, "female", 1, _actor.Object);
		Assert.AreEqual(Gender.Female, FutureProg.GetVariable(ProgVariableTypes.Gender, gender.result).GetObject);
		Assert.AreEqual(Gender.Female, FutureProg.GetVariable(ProgVariableTypes.Gender, (short)Gender.Female).GetObject);
	}

	[TestMethod]
	public void GetArgument_EffectAndOutfit_ResolveWithinOwner()
	{
		var effect = Reference<IEffect>(ProgVariableTypes.Effect);
		_cell.SetupGet(x => x.Effects).Returns(new[] { effect.Object });
		Assert.AreSame(effect.Object, ProgModule.GetArgument(ProgVariableTypes.Effect, "here 1", 1, _actor.Object).result);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.Effect, "here 2", 1, _actor.Object).success);
		var outfit = new Mock<IOutfit>();
		outfit.SetupGet(x => x.Name).Returns("Dress Uniform");
		var item = new Mock<IOutfitItem>();
		item.SetupGet(x => x.Id).Returns(42);
		outfit.SetupGet(x => x.Items).Returns(new[] { item.Object });
		_actor.SetupGet(x => x.Outfits).Returns(new[] { outfit.Object });
		Assert.AreSame(outfit.Object, ProgModule.GetArgument(ProgVariableTypes.Outfit, "self \"Dress Uniform\"", 1, _actor.Object).result);
		Assert.AreSame(item.Object, ProgModule.GetArgument(ProgVariableTypes.OutfitItem, "self \"Dress Uniform\" 42", 1, _actor.Object).result);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.OutfitItem, "self \"Dress Uniform\" 43", 1, _actor.Object).success);
	}

	[TestMethod]
	public void GetArgument_Trap_IndexesOnlyTrapsAndPreservesTypedWrapper()
	{
		_cell.SetupGet(x => x.Gameworld).Returns(_world.Object);
		var trap = new TrapEffect(_cell.Object, new Mock<ITrapTemplate>().Object);
		_cell.SetupGet(x => x.Effects).Returns(new IEffect[] { new Mock<IEffect>().Object, trap });
		var result = ProgModule.GetArgument(ProgVariableTypes.Trap, "here 1", 1, _actor.Object);
		Assert.IsTrue(result.success);
		Assert.AreEqual(ProgVariableTypes.Trap, ((IProgVariable)result.result!).Type);
		Assert.AreSame(trap, FutureProg.GetVariable(ProgVariableTypes.Trap, result.result).GetObject);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.Trap, "here 2", 1, _actor.Object).success);
	}

	[TestMethod]
	public void GetArgument_SignedVariety_ResolvesGlobalIdOrOwnerQualifiedName()
	{
		var language = Reference<ISignedLanguage>(ProgVariableTypes.SignedLanguage);
		var variety = Reference<ISignedLanguageVariety>(ProgVariableTypes.SignedVariety);
		language.SetupGet(x => x.Varieties).Returns(new[] { variety.Object });
		var languages = new Mock<IUneditableAll<ISignedLanguage>>();
		languages.Setup(x => x.GetEnumerator()).Returns(() => new List<ISignedLanguage> { language.Object }.GetEnumerator());
		languages.Setup(x => x.GetByIdOrName("Example Name", It.IsAny<bool>())).Returns(language.Object);
		_world.SetupGet(x => x.SignedLanguages).Returns(languages.Object);
		Assert.AreSame(variety.Object, ProgModule.GetArgument(ProgVariableTypes.SignedVariety, "7", 1, _actor.Object).result);
		Assert.AreSame(variety.Object, ProgModule.GetArgumentFromRemainingInput(ProgVariableTypes.SignedVariety,
			new StringStack("(\"Example Name\" \"Example Name\")"), 1, _actor.Object).result);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.SignedVariety, "\"Example Name\" unknown", 1, _actor.Object).success);
	}

	[TestMethod]
	public void GetArgument_LiquidMixture_CreatesMeasuredValueWithoutChangingWorld()
	{
		var liquid = Reference<ILiquid>(ProgVariableTypes.Liquid);
		liquid.SetupGet(x => x.Density).Returns(1.0);
		_world.SetupGet(x => x.Liquids).Returns(Registry(liquid.Object));
		var units = new Mock<IUnitManager>();
		var amount = 0.25;
		units.Setup(x => x.TryGetBaseUnits("250ml", UnitType.FluidVolume, _actor.Object, out amount)).Returns(true);
		_world.SetupGet(x => x.UnitManager).Returns(units.Object);
		var result = ProgModule.GetArgument(ProgVariableTypes.LiquidMixture, "\"Example Name\" 250ml", 1, _actor.Object);
		Assert.IsTrue(result.success);
		Assert.AreEqual(0.25, ((LiquidMixture)result.result!).TotalVolume);
		Assert.IsTrue(((LiquidMixture)ProgModule.GetArgument(ProgVariableTypes.LiquidMixture, "empty", 1, _actor.Object).result!).IsEmpty);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.LiquidMixture, "\"Example Name\" invalid", 1, _actor.Object).success);
	}

	[DataTestMethod]
	[DataRow("\"\" 5", true, false)]
	[DataRow("\"\" invalid", false, false)]
	[DataRow("\"\" 5 extra", false, false)]
	[DataRow("\"unterminated 5", false, false)]
	[DataRow("\"\" 5 ignored", true, true)]
	public void ProgExecute_EmptyAndInvalidArguments_OnlyExecutesCompleteValidInput(string text, bool expected, bool anyParameters)
	{
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.AcceptsAnyParameters).Returns(anyParameters);
		prog.SetupGet(x => x.NamedParameters).Returns(new List<Tuple<ProgVariableTypes, string>>
		{
			Tuple.Create(ProgVariableTypes.Text, "text"), Tuple.Create(ProgVariableTypes.Number, "number")
		});
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Void);
		prog.Setup(x => x.MXPClickableFunctionNameWithId()).Returns("example");
		var progs = new Mock<IUneditableAll<IFutureProg>>();
		progs.Setup(x => x.GetByIdOrName("example", It.IsAny<bool>())).Returns(prog.Object);
		_world.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		typeof(ProgModule).GetMethod("ProgExecute", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, new object[] { _actor.Object, new StringStack($"example {text}") });
		prog.Verify(x => x.Execute(It.IsAny<object[]>()), expected ? Times.Once() : Times.Never());
	}

	[TestMethod]
	public void GetArgument_BankAccount_UsesActorsWorldAndRejectsMissingAccounts()
	{
		var account = Reference<IBankAccount>(ProgVariableTypes.BankAccount);
		account.SetupGet(x => x.AccountNumber).Returns(123);
		var bank = Reference<IBank>(ProgVariableTypes.Bank);
		bank.SetupGet(x => x.BankAccounts).Returns(new[] { account.Object });
		_world.SetupGet(x => x.Banks).Returns(Registry(bank.Object));
		Assert.AreSame(account.Object, ProgModule.GetArgument(ProgVariableTypes.BankAccount, "Example Name:123", 1, _actor.Object).result);
		Assert.IsFalse(ProgModule.GetArgument(ProgVariableTypes.BankAccount, "Example Name:124", 1, _actor.Object).success);
	}

	private static Mock<T> Reference<T>(ProgVariableTypes type) where T : class, IProgVariable, IFrameworkItem
	{
		var item = new Mock<T>();
		item.SetupGet(x => x.Id).Returns(7);
		item.SetupGet(x => x.Name).Returns("Example Name");
		item.SetupGet(x => x.Type).Returns(type);
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		return item;
	}

	private static IUneditableAll<T> Registry<T>(T item) where T : class, IFrameworkItem
	{
		var registry = new Mock<IUneditableAll<T>>();
		registry.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
			.Returns((string text, bool _) => text == "7" || text == "Example Name" ? item : null);
		registry.Setup(x => x.Get(7)).Returns(item);
		return registry.Object;
	}
}
