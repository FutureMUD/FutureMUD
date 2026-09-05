#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Community;
using MudSharp.Communication.Language;
using MudSharp.Economy;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgLookupAndCharacteristicTests
{
	[ClassInitialize]
	public static void Initialise(TestContext _) => FutureProgTestBootstrap.EnsureInitialised();

	[TestMethod]
	public void ToLookups_NewGlobalFamilies_RegisterIdAndNameWithMatchingReturnTypes()
	{
		foreach (var type in new[]
		         {
			         ProgVariableTypes.CharacteristicDefinition, ProgVariableTypes.CharacteristicValue,
			         ProgVariableTypes.Calendar, ProgVariableTypes.Clock, ProgVariableTypes.WeatherEvent,
			         ProgVariableTypes.Shop, ProgVariableTypes.Solid, ProgVariableTypes.Liquid, ProgVariableTypes.Gas,
			         ProgVariableTypes.MagicSpell, ProgVariableTypes.MagicSchool, ProgVariableTypes.MagicCapability,
			         ProgVariableTypes.Market, ProgVariableTypes.MarketCategory, ProgVariableTypes.NPCSkillPackage,
			         ProgVariableTypes.AgricultureField, ProgVariableTypes.Tag, ProgVariableTypes.ItemPrototype,
			         ProgVariableTypes.NPCTemplate, ProgVariableTypes.OutfitTemplate, ProgVariableTypes.Vehicle,
			         ProgVariableTypes.CelestialObject, ProgVariableTypes.Grid, ProgVariableTypes.Property,
			         ProgVariableTypes.EconomicZone, ProgVariableTypes.Channel
		         })
		foreach (var argument in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			var name = "to" + type.Describe().ToLowerInvariant();
			Assert.AreEqual(type, Registration(name, argument).ReturnType, name);
		}
	}

	[TestMethod]
	public void CharacteristicLookups_IdNameAndScopedName_ReturnCorrectObjectsAndTypedNulls()
	{
		var (world, definition, value) = Characteristics();
		Assert.AreSame(definition, Run(world, "tocharacteristicdefinition", new NumberVariable(definition.Id)).GetObject);
		Assert.AreSame(definition, Run(world, "tocharacteristicdefinition", new TextVariable("EYECOLOUR")).GetObject);
		Assert.AreSame(value, Run(world, "tocharacteristicvalue", definition, new TextVariable("BLUE")).GetObject);
		Assert.AreSame(value, Run(world, "tocharacteristicvalue", new NumberVariable(value.Id)).GetObject);
		var missing = Run(world, "tocharacteristicvalue", definition, new NumberVariable(999));
		Assert.AreEqual(ProgVariableTypes.CharacteristicValue, missing.Type);
		Assert.IsNull(missing.GetObject);
		Assert.IsNull(Run(world, "tocharacteristicvalue", new NullVariable(ProgVariableTypes.CharacteristicDefinition), new TextVariable("blue")).GetObject);
	}

	[TestMethod]
	public void SetCharacteristic_AllMixedOverloads_UseDefinitionScopedNames()
	{
		var (world, definition, value) = Characteristics();
		var character = Item<ICharacter>(10, "character", ProgVariableTypes.Character);
		var item = Item<IGameItem>(11, "item", ProgVariableTypes.Item);
		foreach (var target in new IProgVariable[] { character.Object, item.Object })
		foreach (var definitionArgument in new IProgVariable[] { definition, new NumberVariable(definition.Id), new TextVariable(definition.Name) })
		foreach (var valueArgument in new IProgVariable[] { value, new NumberVariable(value.Id), new TextVariable(value.Name) })
		{
			Assert.AreEqual(true, Run(world, "setcharacteristic", target, definitionArgument, valueArgument).GetObject);
		}
		character.Verify(x => x.SetCharacteristic(definition, value), Times.Exactly(9));
		item.Verify(x => x.SetCharacteristic(definition, value), Times.Exactly(9));
		Assert.AreEqual(false, Run(world, "setcharacteristic", character.Object, definition,
			new NullVariable(ProgVariableTypes.CharacteristicValue)).GetObject);
		Assert.AreEqual(false, Run(world, "setcharacteristic", character.Object, definition, new NumberVariable(1)).GetObject);
	}

	[TestMethod]
	public void CharacteristicReads_TypedAndLegacyArguments_PreserveResultContracts()
	{
		var (world, definition, value) = Characteristics();
		var target = Item<ICharacter>(10, "character", ProgVariableTypes.Character);
		target.Setup(x => x.GetCharacteristic(definition, null!)).Returns(value);
		foreach (var argument in new IProgVariable[] { definition, new NumberVariable(definition.Id), new TextVariable(definition.Name) })
		{
			Assert.AreSame(value, Run(world, "getcharacteristicvalue", target.Object, argument).GetObject);
			Assert.AreEqual(value.Name, Run(world, "characteristicvalue", target.Object, argument).GetObject);
			Assert.AreEqual((decimal)value.Id, Run(world, "characteristicid", target.Object, argument).GetObject);
		}
		Assert.IsNull(Run(world, "getcharacteristicvalue", target.Object, new NullVariable(ProgVariableTypes.CharacteristicDefinition)).GetObject);
	}

	[TestMethod]
	public void CharacteristicDescription_TypedDefinition_PreservesViewerAndRealVariants()
	{
		var (world, definition, _) = Characteristics();
		var target = Item<ICharacter>(10, "character", ProgVariableTypes.Character);
		var viewer = Item<ICharacter>(11, "viewer", ProgVariableTypes.Character);
		target.Setup(x => x.DescribeCharacteristic(definition, viewer.Object, CharacteristicDescriptionType.Normal)).Returns("disguised");
		target.Setup(x => x.DescribeCharacteristic(definition, null!, CharacteristicDescriptionType.Normal)).Returns("blue");
		Assert.AreEqual("disguised", Run(world, "getcharacteristic", target.Object, definition, viewer.Object).GetObject);
		Assert.AreEqual("blue", Run(world, "getrealcharacteristic", target.Object, definition).GetObject);
	}

	[TestMethod]
	public void ScopedLookup_DuplicateBankAccountTypeNames_RespectsOwnerAndNull()
	{
		var first = Item<IBankAccountType>(10, "Savings", ProgVariableTypes.BankAccountType);
		var second = Item<IBankAccountType>(11, "Savings", ProgVariableTypes.BankAccountType);
		var bank1 = Item<IBank>(1, "First", ProgVariableTypes.Bank);
		var bank2 = Item<IBank>(2, "Second", ProgVariableTypes.Bank);
		bank1.SetupGet(x => x.BankAccountTypes).Returns([first.Object]);
		bank2.SetupGet(x => x.BankAccountTypes).Returns([second.Object]);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Banks).Returns(Registry(bank1.Object, bank2.Object));
		Assert.AreSame(second.Object, Run(world.Object, "tobankaccounttype", bank2.Object, new TextVariable("savings")).GetObject);
		Assert.AreSame(first.Object, Run(world.Object, "tobankaccounttype", new NumberVariable(10)).GetObject);
		Assert.IsNull(Run(world.Object, "tobankaccounttype", bank2.Object, new NumberVariable(10)).GetObject);
		Assert.IsNull(Run(world.Object, "tobankaccounttype", new NullVariable(ProgVariableTypes.Bank), new TextVariable("savings")).GetObject);
	}

	[TestMethod]
	public void DedicatedScopedConversions_PreserveGlobalIdsScopedNamesAndAliases()
	{
		var world = new Mock<IFuturemud>();
		var merchandise = Item<IMerchandise>(21, "shared", ProgVariableTypes.Merchandise);
		var shop = Item<IShop>(1, "shop", ProgVariableTypes.Shop);
		shop.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);
		world.SetupGet(x => x.Shops).Returns(Registry(shop.Object));
		var law = Item<ILaw>(22, "shared", ProgVariableTypes.Law);
		var authority = Item<ILegalAuthority>(2, "authority", ProgVariableTypes.LegalAuthority);
		authority.SetupGet(x => x.Laws).Returns([law.Object]);
		world.SetupGet(x => x.LegalAuthorities).Returns(Registry(authority.Object));
		var variety = Item<ISignedLanguageVariety>(23, "shared", ProgVariableTypes.SignedVariety);
		var language = Item<ISignedLanguage>(3, "language", ProgVariableTypes.SignedLanguage);
		language.SetupGet(x => x.Varieties).Returns([variety.Object]);
		world.SetupGet(x => x.SignedLanguages).Returns(Registry(language.Object));

		foreach (var (name, owner, expected, id) in new (string, IProgVariable, IProgVariable, long)[]
		         {
			         ("tomerchandise", shop.Object, merchandise.Object, 21),
			         ("tolaw", authority.Object, law.Object, 22),
			         ("tosignedvariety", language.Object, variety.Object, 23),
			         ("tosignedlanguagevariety", language.Object, variety.Object, 23)
		         })
		{
			Assert.AreSame(expected, Run(world.Object, name, new NumberVariable(id)).GetObject);
			Assert.AreSame(expected, Run(world.Object, name, owner, new NumberVariable(id)).GetObject);
			Assert.AreSame(expected, Run(world.Object, name, owner, new TextVariable("SHARED")).GetObject);
			var missing = Run(world.Object, name, owner, new NumberVariable(999));
			Assert.AreEqual(expected.Type, missing.Type);
			Assert.IsNull(missing.GetObject);
			Assert.IsNull(Run(world.Object, name, new NullVariable(owner.Type), new TextVariable("shared")).GetObject);
		}
	}

	[TestMethod]
	public void ClanLookups_HaveUniqueSignaturesAndCorrectNullType()
	{
		foreach (var name in new[] { "toclan", "torank", "toappointment", "topaygrade" })
		{
			var registrations = FutureProg.GetFunctionCompilerInformations().Where(x => x.FunctionName == name);
			Assert.IsTrue(registrations.GroupBy(x => string.Join(",", x.Parameters.Select(y => y.ToStorageString())))
				.All(x => x.Count() == 1), name);
		}
		var result = Run(Mock.Of<IFuturemud>(), "topaygrade", new NullVariable(ProgVariableTypes.Clan), new TextVariable("missing"));
		Assert.AreEqual(ProgVariableTypes.ClanPaygrade, result.Type);
		Assert.IsNull(result.GetObject);
		var emptyWorld = new Mock<IFuturemud>();
		emptyWorld.SetupGet(x => x.Clans).Returns(Registry<IClan>());
		foreach (var name in new[] { "toclan", "torank", "toappointment", "topaygrade" })
		{
			Assert.IsNull(Run(emptyWorld.Object, name, new NullVariable(ProgVariableTypes.Number)).GetObject);
		}
		Assert.IsNull(Run(Mock.Of<IFuturemud>(), "torank", new NullVariable(ProgVariableTypes.Clan),
			new NullVariable(ProgVariableTypes.Number)).GetObject);

	}

	[TestMethod]
	public void CharacteristicReads_Chargen_ReturnSelectedValueAndDescription()
	{
		var (world, definition, value) = Characteristics();
		Mock.Get(value).SetupGet(x => x.GetValue).Returns("blue eyes");
		var chargen = Item<IChargen>(10, "chargen", ProgVariableTypes.Chargen);
		chargen.SetupGet(x => x.SelectedCharacteristics).Returns([(definition, value)]);
		Assert.AreSame(value, Run(world, "getcharacteristicvalue", chargen.Object, definition).GetObject);
		Assert.AreEqual("blue eyes", Run(world, "getrealcharacteristic", chargen.Object, definition).GetObject);
		Assert.AreEqual("blue eyes", Run(world, "getrealcharacteristic", chargen.Object, new TextVariable("eyecolour")).GetObject);
	}

	[TestMethod]
	public void RandomCharacteristic_TypedDefinition_UsesProfileAndBoundsForcedRerolls()
	{
		var (world, definition, value) = Characteristics();
		var target = Item<ICharacter>(10, "character", ProgVariableTypes.Character);
		var other = Item<ICharacteristicValue>(20, "other", ProgVariableTypes.CharacteristicValue);
		var profile = new Mock<ICharacteristicProfile>();
		profile.SetupGet(x => x.Id).Returns(100);
		profile.SetupGet(x => x.Name).Returns("eyes");
		profile.SetupGet(x => x.Values).Returns([value, other.Object]);
		profile.Setup(x => x.IsProfileFor(definition)).Returns(true);
		profile.Setup(x => x.GetRandomCharacteristic(target.Object)).Returns(value);
		Mock.Get(world).SetupGet(x => x.CharacteristicProfiles).Returns(Registry(profile.Object));
		target.Setup(x => x.GetCharacteristic(definition, null!)).Returns(value);
		Assert.AreEqual(true, Run(world, "setcharacteristicrandom", target.Object, definition,
			new TextVariable("eyes"), new BooleanVariable(false)).GetObject);
		Assert.AreEqual(false, Run(world, "setcharacteristicrandom", target.Object, definition,
			new NumberVariable(100), new BooleanVariable(true)).GetObject);
		target.Verify(x => x.SetCharacteristic(definition, value), Times.Once);
		Assert.AreEqual(false, Run(world, "setcharacteristicrandom", target.Object,
			new NullVariable(ProgVariableTypes.CharacteristicDefinition), new NumberVariable(100), new BooleanVariable(false)).GetObject);
	}

	[TestMethod]
	public void ClanRank_NumericAndTextLookups_DistinguishIdentityFromRankNumber()
	{
		var rank = Item<IRank>(100, "Captain", ProgVariableTypes.ClanRank);
		rank.SetupGet(x => x.RankNumber).Returns(3);
		rank.SetupGet(x => x.Titles).Returns(["Commander"]);
		rank.SetupGet(x => x.Abbreviations).Returns(["Cpt"]);
		var clan = Item<IClan>(5, "Guard", ProgVariableTypes.Clan);
		clan.SetupGet(x => x.Ranks).Returns([rank.Object]);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Clans).Returns(Registry(clan.Object));
		Assert.AreSame(rank.Object, Run(world.Object, "torank", new NumberVariable(100)).GetObject);
		Assert.AreSame(rank.Object, Run(world.Object, "torank", clan.Object, new NumberVariable(3)).GetObject);
		Assert.AreSame(rank.Object, Run(world.Object, "torank", clan.Object, new TextVariable("commander")).GetObject);
		Assert.IsNull(Run(world.Object, "torank", clan.Object, new NumberVariable(100)).GetObject);
	}

	[TestMethod]
	public void TypedCharacteristicChain_CompilesInRealProg()
	{
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "TypedCharacteristics", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "target")],
			"return setcharacteristic(@target, tocharacteristicdefinition(1), tocharacteristicvalue(tocharacteristicdefinition(1), \"blue\"))");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
	}

	private static (IFuturemud World, ICharacteristicDefinition Definition, ICharacteristicValue Value) Characteristics()
	{
		var definition = Item<ICharacteristicDefinition>(5, "eyecolour", ProgVariableTypes.CharacteristicDefinition);
		var wrong = Item<ICharacteristicValue>(1, "blue", ProgVariableTypes.CharacteristicValue);
		var value = Item<ICharacteristicValue>(2, "blue", ProgVariableTypes.CharacteristicValue);
		definition.Setup(x => x.IsValue(value.Object)).Returns(true);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Characteristics).Returns(Registry(definition.Object));
		world.SetupGet(x => x.CharacteristicValues).Returns(Registry(wrong.Object, value.Object));
		return (world.Object, definition.Object, value.Object);
	}

	private static Mock<T> Item<T>(long id, string name, ProgVariableTypes type) where T : class, IFrameworkItem, IProgVariable
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.As<IProgVariable>().SetupGet(x => x.Type).Returns(type);
		mock.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(mock.Object);
		return mock;
	}

	private static All<T> Registry<T>(params T[] items) where T : class, IFrameworkItem
	{
		var result = new All<T>();
		foreach (var item in items) result.Add(item);
		return result;
	}

	private static FunctionCompilerInformation Registration(string name, params ProgVariableTypes[] types)
	{
		return FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == name &&
			x.Parameters.Count() == types.Length && x.Parameters.Zip(types).All(pair => pair.Second.CompatibleWith(pair.First)));
	}

	private static IProgVariable Run(IFuturemud world, string name, params IProgVariable[] arguments)
	{
		var registration = Registration(name, arguments.Select(x => x.Type).ToArray());
		var functions = arguments.Select(argument =>
		{
			var function = new Mock<IFunction>();
			function.SetupGet(x => x.ReturnType).Returns(argument.Type);
			function.SetupGet(x => x.Result).Returns(argument);
			function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
			return function.Object;
		}).ToList();
		var compiled = registration.CompilerFunction(functions, world);
		Assert.AreEqual(StatementResult.Normal, compiled.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(registration.ReturnType, compiled.ReturnType);
		return compiled.Result;
	}
}
