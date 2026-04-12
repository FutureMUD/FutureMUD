#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LegalClassFutureProgTests
{
	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void LegalClassDotReference_ShouldExposeExpectedProperties()
	{
		FutureProgVariableCompileInfo compileInfo = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.LegalClass];

		Assert.AreEqual(ProgVariableTypes.Number, compileInfo.PropertyTypeMap["id"]);
		Assert.AreEqual(ProgVariableTypes.Text, compileInfo.PropertyTypeMap["name"]);
		Assert.AreEqual(ProgVariableTypes.LegalAuthority, compileInfo.PropertyTypeMap["legalauthority"]);
		Assert.AreEqual(ProgVariableTypes.Number, compileInfo.PropertyTypeMap["priority"]);
		Assert.AreEqual(ProgVariableTypes.Boolean, compileInfo.PropertyTypeMap["canbedetaineduntilfinespaid"]);
	}

	[TestMethod]
	public void ToLegalClass_NumberLookup_ShouldResolveLegalClass()
	{
		var authority = CreateLegalAuthorityMock(7L, "Test Authority");
		var legalClass = CreateLegalClassMock(101L, "Citizen", authority.Object, 10, false);
		var gameworld = CreateGameworld([authority.Object], [legalClass.Object]);

		FutureProg prog = CompileProg(
			gameworld.Object,
			"LegalClassNumberLookup",
			ProgVariableTypes.Number,
			Array.Empty<Tuple<ProgVariableTypes, string>>(),
			"return ToLegalClass(101).Priority");

		Assert.AreEqual(10.0m, prog.Execute<decimal>());
	}

	[TestMethod]
	public void ToLegalClass_AuthorityScopedNameLookup_ShouldResolveLegalClass()
	{
		var authority = CreateLegalAuthorityMock(7L, "Test Authority");
		var legalClass = CreateLegalClassMock(101L, "Citizen", authority.Object, 10, false);
		authority.SetupGet(x => x.LegalClasses).Returns([legalClass.Object]);
		var gameworld = CreateGameworld([authority.Object], [legalClass.Object]);

		FutureProg prog = CompileProg(
			gameworld.Object,
			"LegalClassNameLookup",
			ProgVariableTypes.Text,
			[Tuple.Create(ProgVariableTypes.LegalAuthority, "authority")],
			"""return ToLegalClass("Citizen", @authority).Name""");

		Assert.AreEqual("Citizen", prog.ExecuteString(authority.Object));
	}

	[TestMethod]
	public void GetLegalClass_AndPriority_ShouldResolveFromAuthority()
	{
		var authority = CreateLegalAuthorityMock(7L, "Test Authority");
		var legalClass = CreateLegalClassMock(101L, "Citizen", authority.Object, 10, false);
		var character = CreateCharacterMock(1L, "Test Citizen");
		authority.Setup(x => x.GetLegalClass(character.Object)).Returns(legalClass.Object);
		var gameworld = CreateGameworld([authority.Object], [legalClass.Object]);

		FutureProg prog = CompileProg(
			gameworld.Object,
			"LegalClassCharacterLookup",
			ProgVariableTypes.Number,
			[
				Tuple.Create(ProgVariableTypes.Character, "ch"),
				Tuple.Create(ProgVariableTypes.LegalAuthority, "authority")
			],
			"return GetLegalClass(@ch, @authority).Priority");

		Assert.AreEqual(10.0m, prog.Execute<decimal>(character.Object, authority.Object));
	}

	[TestMethod]
	public void LegalClassOutranks_ShouldCompareResolvedCharacterPriorities()
	{
		var authority = CreateLegalAuthorityMock(7L, "Test Authority");
		var sovereign = CreateLegalClassMock(100L, "Sovereign", authority.Object, 100, false);
		var citizen = CreateLegalClassMock(101L, "Citizen", authority.Object, 10, false);
		var criminal = CreateCharacterMock(1L, "Criminal");
		var victim = CreateCharacterMock(2L, "Victim");
		authority.Setup(x => x.GetLegalClass(criminal.Object)).Returns(sovereign.Object);
		authority.Setup(x => x.GetLegalClass(victim.Object)).Returns(citizen.Object);
		var gameworld = CreateGameworld([authority.Object], [sovereign.Object, citizen.Object]);

		FutureProg prog = CompileProg(
			gameworld.Object,
			"LegalClassOutranksCharacterLookup",
			ProgVariableTypes.Boolean,
			[
				Tuple.Create(ProgVariableTypes.Character, "criminal"),
				Tuple.Create(ProgVariableTypes.Character, "victim"),
				Tuple.Create(ProgVariableTypes.LegalAuthority, "authority")
			],
			"return LegalClassOutranks(@criminal, @victim, @authority)");

		Assert.IsTrue(prog.ExecuteBool(criminal.Object, victim.Object, authority.Object));
	}

	[TestMethod]
	public void ProgModuleGetArgument_ShouldResolveLegalAuthorityAndLegalClass()
	{
		var authority = CreateLegalAuthorityMock(7L, "Test Authority");
		var legalClass = CreateLegalClassMock(101L, "Citizen", authority.Object, 10, false);
		var gameworld = CreateGameworld([authority.Object], [legalClass.Object]);
		var actor = CreateActorForProgModule(gameworld.Object);

		(object authorityResult, bool authoritySuccess) =
			ProgModule.GetArgument(ProgVariableTypes.LegalAuthority, "Test Authority", 1, actor.Object);
		(object legalClassResult, bool legalClassSuccess) =
			ProgModule.GetArgument(ProgVariableTypes.LegalClass, "Citizen", 2, actor.Object);

		Assert.IsTrue(authoritySuccess);
		Assert.IsTrue(legalClassSuccess);
		Assert.AreSame(authority.Object, authorityResult);
		Assert.AreSame(legalClass.Object, legalClassResult);
	}

	private static FutureProg CompileProg(
		IFuturemud gameworld,
		string name,
		ProgVariableTypes returnType,
		IEnumerable<Tuple<ProgVariableTypes, string>> parameters,
		string functionText)
	{
		FutureProg prog = new(gameworld, name, returnType, parameters, functionText);
		prog.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog.CompileError), prog.CompileError);
		return prog;
	}

	private static Mock<IFuturemud> CreateGameworld(
		IEnumerable<ILegalAuthority> legalAuthorities,
		IEnumerable<ILegalClass> legalClasses)
	{
		All<ILegalAuthority> authorityCollection = new();
		foreach (var authority in legalAuthorities)
		{
			authorityCollection.Add(authority);
		}

		All<ILegalClass> legalClassCollection = new();
		foreach (var legalClass in legalClasses)
		{
			legalClassCollection.Add(legalClass);
		}

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(authorityCollection);
		gameworld.SetupGet(x => x.LegalClasses).Returns(legalClassCollection);
		return gameworld;
	}

	private static Mock<ILegalAuthority> CreateLegalAuthorityMock(long id, string name)
	{
		Mock<ILegalAuthority> mock = new();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns("LegalAuthority");
		mock.SetupGet(x => x.LegalClasses).Returns(Array.Empty<ILegalClass>());
		mock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((string property) => property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(id),
			"name" => new TextVariable(name),
			_ => throw new ApplicationException($"Unexpected legal authority property {property}")
		});
		mock.SetupGet(x => x.Type).Returns(ProgVariableTypes.LegalAuthority);
		mock.SetupGet(x => x.GetObject).Returns(() => mock.Object);
		return mock;
	}

	private static Mock<ILegalClass> CreateLegalClassMock(
		long id,
		string name,
		ILegalAuthority authority,
		int priority,
		bool canBeDetainedUntilFinesPaid)
	{
		Mock<ILegalClass> mock = new();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns("LegalClass");
		mock.SetupGet(x => x.Authority).Returns(authority);
		mock.SetupGet(x => x.LegalClassPriority).Returns(priority);
		mock.SetupGet(x => x.CanBeDetainedUntilFinesPaid).Returns(canBeDetainedUntilFinesPaid);
		mock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((string property) => property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(id),
			"name" => new TextVariable(name),
			"legalauthority" => authority,
			"priority" => new NumberVariable(priority),
			"canbedetaineduntilfinespaid" => new BooleanVariable(canBeDetainedUntilFinesPaid),
			_ => throw new ApplicationException($"Unexpected legal class property {property}")
		});
		mock.SetupGet(x => x.Type).Returns(ProgVariableTypes.LegalClass);
		mock.SetupGet(x => x.GetObject).Returns(() => mock.Object);
		return mock;
	}

	private static Mock<ICharacter> CreateCharacterMock(long id, string name)
	{
		Mock<ICharacter> mock = new();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns("Character");
		mock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);
		mock.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		mock.SetupGet(x => x.GetObject).Returns(() => mock.Object);
		return mock;
	}

	private static Mock<ICharacter> CreateActorForProgModule(IFuturemud gameworld)
	{
		Mock<IOutputHandler> output = new();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);

		Mock<ICharacter> actor = new();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.Id).Returns(500L);
		actor.SetupGet(x => x.Name).Returns("Builder");
		actor.SetupGet(x => x.FrameworkItemType).Returns("Character");
		actor.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((IProgVariable?)null);
		actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		actor.SetupGet(x => x.GetObject).Returns(() => actor.Object);
		return actor;
	}
}
