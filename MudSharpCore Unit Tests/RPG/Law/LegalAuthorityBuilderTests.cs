#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PunishmentStrategies;

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
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog.Object });
		world.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.Setup(x => x.GetStaticBool("ApplyAllLegalClassesAsOffendersByDefault")).Returns(true);
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		var currency = Mock.Of<ICurrency>(x => x.Id == 3);
		var authority = new LegalAuthority("Alderbridge Bench", currency, world.Object);

		Assert.IsTrue(authority.BuildingCommand(actor, new StringStack("class add \"Town Residents\" 7")));
		Assert.IsTrue(authority.BuildingCommand(actor, new StringStack("law add \"Workshop Trespass\" Trespassing")));

		var legalClass = database.Context.LegalClasses.Single();
		Assert.AreEqual("Town Residents", legalClass.Name);
		Assert.AreEqual(7L, legalClass.MembershipProgId);
		var law = database.Context.Laws.Single();
		Assert.AreEqual("Workshop Trespass", law.Name);
		Assert.AreEqual(legalClass.Id, law.LawsOffenderClasses.Single().LegalClassId);
		var punishment = PunishmentStrategyFactory.LoadStrategy(world.Object, law.PunishmentStrategy, authority);
		Assert.AreEqual("no punishment", punishment.Describe(actor));
	}
}
