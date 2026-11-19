#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PunishmentStrategies;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class LegalAuthorityBuilderTests
{
	[TestMethod]
	public void NewAuthority_CreatesNamedClassAndPersistsNewLawWithNoPunishment()
	{
		using var database = new InMemoryDatabaseTestScope();
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(7);
		prog.SetupGet(x => x.Name).Returns("AlderEveryone");
		prog.SetupGet(x => x.FunctionName).Returns("AlderEveryone");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		var world = CreateWorld(prog.Object);
		var laws = new All<ILaw>();
		var legalClasses = new All<ILegalClass>();
		world.SetupGet(x => x.Laws).Returns(laws);
		world.SetupGet(x => x.LegalClasses).Returns(legalClasses);
		world.Setup(x => x.Add(It.IsAny<ILaw>())).Callback<ILaw>(law => laws.Add(law));
		world.Setup(x => x.Add(It.IsAny<ILegalClass>())).Callback<ILegalClass>(legalClass => legalClasses.Add(legalClass));
		world.Setup(x => x.Destroy(It.IsAny<ILaw>())).Callback<ILaw>(law => laws.Remove(law));
		world.Setup(x => x.Destroy(It.IsAny<ILegalClass>())).Callback<ILegalClass>(legalClass => legalClasses.Remove(legalClass));
		Accept? confirmation = null;
		var builder = new Mock<ICharacter>();
		builder.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		builder.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => confirmation = (Accept)effect);
		var actor = builder.Object;
		var authority = CreateAuthority(world.Object);

		Assert.IsTrue(authority.BuildingCommand(actor, new StringStack("class add \"Town Residents\" 7")));
		Assert.IsTrue(authority.BuildingCommand(actor, new StringStack("law add \"Workshop Trespass\" Trespassing")));

		var legalClass = database.Context.LegalClasses.Single();
		Assert.AreEqual("Town Residents", legalClass.Name);
		Assert.AreEqual(7L, legalClass.MembershipProgId);
		Assert.AreEqual(authority.LegalClasses.Single().LegalClassPriority, legalClass.LegalClassPriority);
		var law = database.Context.Laws.Single();
		Assert.AreEqual("Workshop Trespass", law.Name);
		Assert.AreEqual(legalClass.Id, law.LawsOffenderClasses.Single().LegalClassId);
		var punishment = PunishmentStrategyFactory.LoadStrategy(world.Object, law.PunishmentStrategy, authority);
		Assert.AreEqual("no punishment", punishment.Describe(actor));
		Assert.AreSame(authority.Laws.Single(), world.Object.Laws.Get(law.Id));
		Assert.AreSame(authority.LegalClasses.Single(), world.Object.LegalClasses.Get(legalClass.Id));

		Assert.IsTrue(authority.BuildingCommand(actor, new StringStack($"law delete {law.Id}")));
		Assert.IsNotNull(confirmation);
		confirmation.Proposal.Accept();
		Assert.AreEqual(0, authority.Laws.Count());
		Assert.IsFalse(world.Object.Laws.Has(law.Id));

		confirmation = null;
		Assert.IsTrue(authority.BuildingCommand(actor, new StringStack($"class delete {legalClass.Id}")));
		Assert.IsNotNull(confirmation);
		confirmation.Proposal.Accept();
		Assert.AreEqual(0, authority.LegalClasses.Count());
		Assert.IsFalse(world.Object.LegalClasses.Has(legalClass.Id));
	}

	[TestMethod]
	public void GetLegalClass_OverlappingMembershipUsesPriorityThenLowestId()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = CreateWorld();
		var authority = CreateAuthority(world.Object);
		var character = Mock.Of<ICharacter>();

		var catchAll = Mock.Of<ILegalClass>(x => x.Id == 4 && x.LegalClassPriority == 0 &&
		                                         x.IsMemberOfClass(character));
		var firstMatch = Mock.Of<ILegalClass>(x => x.Id == 3 && x.LegalClassPriority == 10 &&
		                                           x.IsMemberOfClass(character));
		var earlierClass = Mock.Of<ILegalClass>(x => x.Id == 2 && x.LegalClassPriority == 10 &&
		                                             x.IsMemberOfClass(character));
		var nonMember = Mock.Of<ILegalClass>(x => x.Id == 1 && x.LegalClassPriority == 20);
		authority.AddLegalClass(catchAll);
		authority.AddLegalClass(firstMatch);
		authority.AddLegalClass(earlierClass);
		authority.AddLegalClass(nonMember);

		Assert.AreSame(earlierClass, authority.GetLegalClass(character));
	}

	[TestMethod]
	public void WouldBeACrime_LawTypeEditChangesMatchingImmediately()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = CreateWorld();
		var zone = Mock.Of<IZone>(x => x.Id == 5 && x.Name == "Alderbridge");
		world.SetupGet(x => x.Zones).Returns(new All<IZone> { zone });
		var builder = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>() &&
		                                      x.Location == Mock.Of<IRoom>(room => room.Zone == zone));
		var authority = CreateAuthority(world.Object);
		var legalClass = Mock.Of<ILegalClass>(x => x.Id == 7 && x.IsMemberOfClass(builder));
		authority.AddLegalClass(legalClass);
		Assert.IsTrue(authority.BuildingCommand(builder, new StringStack("zone 5")));
		Assert.IsTrue(authority.BuildingCommand(builder, new StringStack("law add Trespass Trespassing")));
		var law = authority.Laws.Single();
		Assert.IsTrue(law.BuildingCommand(builder, new StringStack("auto")));
		Assert.IsTrue(authority.WouldBeACrime(builder, CrimeTypes.Trespassing, null!, null!, ""));
		Assert.IsFalse(authority.WouldBeACrime(builder, CrimeTypes.Vandalism, null!, null!, ""));

		Assert.IsTrue(law.BuildingCommand(builder, new StringStack("crime Vandalism")));

		Assert.IsFalse(authority.WouldBeACrime(builder, CrimeTypes.Trespassing, null!, null!, ""));
		Assert.IsTrue(authority.WouldBeACrime(builder, CrimeTypes.Vandalism, null!, null!, ""));
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void SendCharacterToPrison_UsesJailEntryWhenConfigured(bool hasJailEntry)
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = CreateWorld();
		world.Setup(x => x.GetStaticString(It.IsAny<string>())).Returns("The prisoner is transferred.");
		var authority = CreateAuthority(world.Object);
		var prison = new Mock<IRoom>();
		var jailEntry = new Mock<IRoom>();
		authority.PrisonLocation = prison.Object;
		authority.JailLocation = hasJailEntry ? jailEntry.Object : null!;
		var prisoner = new Mock<ICharacter>();
		prisoner.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		prisoner.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		prisoner.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		prisoner.Setup(x => x.EffectsOfType<ServingCustodialSentence>(null!))
			.Returns([new ServingCustodialSentence(prisoner.Object, authority, TimeSpan.FromDays(1), MudDateTime.Never)]);

		authority.SendCharacterToPrison(prisoner.Object);

		prison.Verify(x => x.Enter(prisoner.Object, null, false, RoomLayer.GroundLevel),
			hasJailEntry ? Times.Never() : Times.Once());
		jailEntry.Verify(x => x.Enter(prisoner.Object, null, false, RoomLayer.GroundLevel),
			hasJailEntry ? Times.Once() : Times.Never());
	}

	private static Mock<IFuturemud> CreateWorld(params IFutureProg[] progs)
	{
		var world = new Mock<IFuturemud>();
		var futureProgs = new All<IFutureProg>();
		foreach (var prog in progs)
		{
			futureProgs.Add(prog);
		}

		world.SetupGet(x => x.FutureProgs).Returns(futureProgs);
		world.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.Setup(x => x.GetStaticBool("ApplyAllLegalClassesAsOffendersByDefault")).Returns(true);
		return world;
	}

	private static LegalAuthority CreateAuthority(IFuturemud world)
	{
		return new LegalAuthority("Alderbridge Bench", Mock.Of<ICurrency>(x => x.Id == 3), world);
	}
}
