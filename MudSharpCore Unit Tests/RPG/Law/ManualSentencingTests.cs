#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class ManualSentencingTests
{
	[TestMethod]
	[DataRow("confirm")]
	[DataRow("reject")]
	[DataRow("leave court")]
	[DataRow("options change")]
	public void Convict_BelowMinimumRequiresConfirmationAndRechecksCase(string response)
	{
		var world = new Mock<IFuturemud>();
		var court = Mock.Of<IRoom>();
		var judge = new Mock<ICharacter>();
		var defendant = new Mock<ICharacter>();
		var crime = new Mock<ICrime>();
		var law = new Mock<ILaw>();
		var authority = new Mock<ILegalAuthority>();
		var currency = new Mock<ICurrency>();
		var output = new Mock<IOutputHandler>();
		var punishmentStrategy = new Mock<IPunishmentStrategy>();
		var legalClass = Mock.Of<ILegalClass>();
		var enforcement = Mock.Of<IEnforcementAuthority>(x => x.CanConvict &&
			x.AccusableClasses == new[] { legalClass });
		var options = new PunishmentOptions { MinimumFine = 10m, MaximumFine = 100m, OnPunishHooks = [(31, PunishmentKind.Fine)] };
		authority.SetupGet(x => x.Id).Returns(1);
		authority.SetupGet(x => x.CourtLocation).Returns(court);
		authority.SetupGet(x => x.Currency).Returns(currency.Object);
		authority.Setup(x => x.GetEnforcementAuthority(judge.Object)).Returns(enforcement);
		authority.Setup(x => x.GetLegalClass(defendant.Object)).Returns(legalClass);
		authority.Setup(x => x.KnownCrimesForIndividual(defendant.Object)).Returns([crime.Object]);
		world.SetupGet(x => x.LegalAuthorities).Returns(new All<ILegalAuthority> { authority.Object });
		judge.SetupGet(x => x.Gameworld).Returns(world.Object);
		judge.SetupGet(x => x.Location).Returns(court);
		judge.SetupGet(x => x.State).Returns(CharacterState.Able);
		judge.SetupGet(x => x.OutputHandler).Returns(output.Object);
		judge.Setup(x => x.TargetActor("defendant", PerceiveIgnoreFlags.None)).Returns(defendant.Object);
		defendant.Setup(x => x.ColocatedWith(judge.Object)).Returns(true);
		crime.SetupGet(x => x.Id).Returns(1);
		crime.SetupGet(x => x.Name).Returns("Workshop trespass");
		crime.SetupGet(x => x.Law).Returns(law.Object);
		law.SetupGet(x => x.PunishmentStrategy).Returns(punishmentStrategy.Object);
		punishmentStrategy.Setup(x => x.GetOptions(defendant.Object, crime.Object)).Returns(() => options);
		decimal amount = 5m;
		currency.Setup(x => x.TryGetBaseCurrency("5cr", out amount)).Returns(true);
		PunishmentResult? applied = null;
		crime.Setup(x => x.Convict(judge.Object, It.IsAny<PunishmentResult>(), "Manually convicted"))
			.Callback<ICharacter, PunishmentResult, string>((_, result, _) => applied = result);
		Accept? pending = null;
		judge.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => pending = (Accept)effect);

		typeof(CrimeModule).GetMethod("Convict", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [judge.Object, "convict defendant 1 fine 5cr"]);

		Assert.IsNull(applied);
		Assert.IsNotNull(pending);
		Assert.IsTrue(pending.Keywords.Contains("sentence"));

		switch (response)
		{
			case "reject":
				pending.Proposal.Reject();
				break;
			case "leave court":
				judge.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
				pending.Proposal.Accept();
				break;
			case "options change":
				options = new PunishmentOptions { MinimumFine = 8m, MaximumFine = 100m };
				pending.Proposal.Accept();
				break;
			default:
				pending.Proposal.Accept();
				break;
		}

		if (response == "confirm")
		{
			Assert.IsNotNull(applied);
			Assert.AreEqual(5m, applied.Fine);
			Assert.AreEqual(PunishmentKind.Fine, applied.BelowMinimumOverride);
			CollectionAssert.AreEqual(new long[] { 31 }, applied.OnPunishProgIds);
		}
		else
		{
			Assert.IsNull(applied);
		}
	}

	[TestMethod]
	public void Convict_RecordsOverrideAndRunsPreparedHooksOnceAfterApplyingSentence()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud>();
		var authority = new Mock<ILegalAuthority>();
		var law = new Mock<ILaw>();
		var defendant = Mock.Of<ICharacter>(x => x.Id == 7);
		var hook = new Mock<IFutureProg>();
		hook.SetupGet(x => x.Id).Returns(30);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { hook.Object });
		world.SetupGet(x => x.Rooms).Returns(new All<IRoom>());
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.Setup(x => x.TryGetCharacter(7L, It.IsAny<bool>())).Returns(defendant);
		law.SetupGet(x => x.Gameworld).Returns(world.Object);
		law.SetupGet(x => x.Name).Returns("Workshop trespass");
		law.SetupGet(x => x.Authority).Returns(authority.Object);
		var row = new MudSharp.Models.Crime
		{
			Id = 20, LawId = 1, CriminalId = 7, TimeOfCrime = "Never", RealTimeOfCrime = DateTime.UtcNow,
			CriminalShortDescription = "a defendant", CriminalFullDescription = "A defendant is here.",
			CriminalCharacteristics = "", WitnessIds = ""
		};
		database.Context.Crimes.Add(row);
		database.Context.SaveChanges();
		var crime = new Crime(row, law.Object, world.Object);
		var applied = false;
		authority.Setup(x => x.ConvictCrime(defendant, crime, It.IsAny<PunishmentResult>()))
			.Callback(() => applied = true);
		hook.Setup(x => x.Execute(defendant, "Workshop trespass")).Callback(() => Assert.IsTrue(applied));
		var punishment = new PunishmentResult
		{
			Fine = 5m, BelowMinimumOverride = PunishmentKind.Fine, OnPunishProgIds = [30]
		};

		crime.Convict(null, punishment, "Manually convicted");
		crime.Convict(null, punishment, "Duplicate");
		crime.Save();
		var reloaded = new Crime(row, law.Object, world.Object);

		hook.Verify(x => x.Execute(defendant, "Workshop trespass"), Times.Once);
		Assert.AreEqual(PunishmentKind.Fine, reloaded.ManualSentenceMinimumOverride);
		Assert.AreEqual(5m, reloaded.FineRecorded);
	}
}
