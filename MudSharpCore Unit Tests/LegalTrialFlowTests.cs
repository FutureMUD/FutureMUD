#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DbCrime = MudSharp.Models.Crime;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LegalTrialFlowTests
{
	[TestMethod]
	public void OnTrial_LoadEffect_ShouldPreserveSavedPhase()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> crime = new();
		crime.SetupGet(x => x.Id).Returns(10L);

		All<ILegalAuthority> legalAuthorities = new();
		legalAuthorities.Add(authority.Object);

		All<ICrime> crimes = new();
		crimes.Add(crime.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(legalAuthorities);
		gameworld.SetupGet(x => x.Crimes).Returns(crimes);
		gameworld.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());

		Mock<ICharacter> owner = new();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		OnTrial.InitialiseEffectType();
		OnTrial trial = new(owner.Object, authority.Object, DateTime.UtcNow, [crime.Object])
		{
			Phase = TrialPhase.Verdict,
			ManualTrial = true
		};

		OnTrial loaded = (OnTrial)Effect.LoadEffect(trial.SaveToXml(new Dictionary<IEffect, TimeSpan>()), owner.Object);

		Assert.AreEqual(TrialPhase.Verdict, loaded.Phase);
		Assert.IsTrue(loaded.ManualTrial);
	}

	[TestMethod]
	public void OnTrial_LoadEffect_ShouldPreserveCrimeQueueAndPunishments()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> firstCrime = new();
		firstCrime.SetupGet(x => x.Id).Returns(10L);

		Mock<ICrime> secondCrime = new();
		secondCrime.SetupGet(x => x.Id).Returns(11L);

		All<ILegalAuthority> legalAuthorities = new();
		legalAuthorities.Add(authority.Object);

		All<ICrime> crimes = new();
		crimes.Add(firstCrime.Object);
		crimes.Add(secondCrime.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(legalAuthorities);
		gameworld.SetupGet(x => x.Crimes).Returns(crimes);
		gameworld.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());

		Mock<ICharacter> owner = new();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		OnTrial.InitialiseEffectType();
		OnTrial trial = new(owner.Object, authority.Object, DateTime.UtcNow, [firstCrime.Object, secondCrime.Object])
		{
			Phase = TrialPhase.Sentencing
		};
		trial.Punishments[firstCrime.Object] = new PunishmentResult
		{
			Fine = 12.5M,
			CustodialSentence = MudTimeSpan.FromDays(3.0),
			GoodBehaviourBondLength = MudTimeSpan.FromDays(30.0),
			Execution = true
		};
		trial.Punishments[secondCrime.Object] = new PunishmentResult();
		Assert.AreSame(firstCrime.Object, trial.NextCrime());

		OnTrial loaded = (OnTrial)Effect.LoadEffect(trial.SaveToXml(new Dictionary<IEffect, TimeSpan>()), owner.Object);

		Assert.AreEqual(TrialPhase.Sentencing, loaded.Phase);
		Assert.IsTrue(loaded.Punishments.ContainsKey(firstCrime.Object));
		Assert.IsTrue(loaded.Punishments.ContainsKey(secondCrime.Object));
		Assert.AreEqual(12.5M, loaded.Punishments[firstCrime.Object].Fine);
		Assert.AreEqual(MudTimeSpan.FromDays(3.0), loaded.Punishments[firstCrime.Object].CustodialSentence);
		Assert.AreEqual(MudTimeSpan.FromDays(30.0), loaded.Punishments[firstCrime.Object].GoodBehaviourBondLength);
		Assert.IsTrue(loaded.Punishments[firstCrime.Object].Execution);
		Assert.IsTrue(loaded.HasSentenceBeenAnnounced(firstCrime.Object));
		Assert.IsFalse(loaded.HasSentenceBeenAnnounced(secondCrime.Object));
		Assert.AreSame(secondCrime.Object, loaded.NextCrime());
	}

	[TestMethod]
	public void OnTrial_Applies_ShouldFilterByLegalAuthority()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ILegalAuthority> otherAuthority = new();
		otherAuthority.SetupGet(x => x.Id).Returns(2L);
		otherAuthority.SetupGet(x => x.Name).Returns("Other Authority");

		Mock<ICrime> crime = new();
		crime.SetupGet(x => x.Id).Returns(10L);

		Mock<ICharacter> owner = new();

		OnTrial trial = new(owner.Object, authority.Object, DateTime.UtcNow, [crime.Object]);

		Assert.IsTrue(trial.Applies(authority.Object));
		Assert.IsFalse(trial.Applies(otherAuthority.Object));
	}

	[TestMethod]
	public void OnTrial_ShouldPreventQuitAndVoluntaryMovement()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> crime = new();
		crime.SetupGet(x => x.Id).Returns(10L);

		Mock<ICharacter> owner = new();

		OnTrial trial = new(owner.Object, authority.Object, DateTime.UtcNow, [crime.Object]);
		Assert.IsInstanceOfType(trial, typeof(INoQuitEffect));
		Assert.AreEqual("You cannot quit while you are on trial.", ((INoQuitEffect)trial).NoQuitReason);

		PerceivableRejectionResponse response = new();
		owner.Raise(x => x.OnWantsToMove += null, owner.Object, response);

		Assert.IsTrue(response.Rejected);
		StringAssert.Contains(response.Reason, "trial is in progress");
	}

	[TestMethod]
	public void OnTrial_HasPleaBeenEntered_ShouldFollowPleaQueue()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> firstCrime = new();
		firstCrime.SetupGet(x => x.Id).Returns(10L);

		Mock<ICrime> secondCrime = new();
		secondCrime.SetupGet(x => x.Id).Returns(11L);

		Mock<ICharacter> owner = new();
		owner.Setup(x => x.EffectsOfType<ConsideringPlea>(It.IsAny<Predicate<ConsideringPlea>>()))
		     .Returns(Array.Empty<ConsideringPlea>());

		OnTrial trial = new(owner.Object, authority.Object, DateTime.UtcNow, [firstCrime.Object, secondCrime.Object])
		{
			Phase = TrialPhase.Plea
		};

		Assert.IsFalse(trial.HasPleaBeenEntered(firstCrime.Object));
		Assert.IsFalse(trial.HasPleaBeenEntered(secondCrime.Object));

		ICrime? askedCrime = trial.NextCrime();
		trial.Pleas[askedCrime!] = false;

		Assert.IsTrue(trial.HasPleaBeenEntered(firstCrime.Object));
		Assert.IsFalse(trial.HasPleaBeenEntered(secondCrime.Object));
	}

	[TestMethod]
	public void OnTrial_RevealState_ShouldNotRevealSentencesBeforeSentencingAnnouncement()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> firstCrime = new();
		firstCrime.SetupGet(x => x.Id).Returns(10L);

		Mock<ICrime> secondCrime = new();
		secondCrime.SetupGet(x => x.Id).Returns(11L);

		Mock<ICharacter> owner = new();
		OnTrial trial = new(owner.Object, authority.Object, DateTime.UtcNow, [firstCrime.Object, secondCrime.Object])
		{
			Phase = TrialPhase.Verdict
		};

		Assert.IsFalse(trial.HasVerdictBeenAnnounced(firstCrime.Object));
		Assert.IsFalse(trial.HasSentenceBeenAnnounced(firstCrime.Object));

		ICrime? verdictCrime = trial.NextCrime();
		trial.Punishments[verdictCrime!] = new PunishmentResult { Fine = 10.0M };

		Assert.IsTrue(trial.HasVerdictBeenAnnounced(firstCrime.Object));
		Assert.IsFalse(trial.HasSentenceBeenAnnounced(firstCrime.Object));
		Assert.IsFalse(trial.HasVerdictBeenAnnounced(secondCrime.Object));

		trial.ResetCrimeQueue();
		trial.Phase = TrialPhase.Sentencing;

		Assert.IsTrue(trial.HasVerdictBeenAnnounced(firstCrime.Object));
		Assert.IsFalse(trial.HasSentenceBeenAnnounced(firstCrime.Object));

		Assert.AreSame(firstCrime.Object, trial.NextCrime());
		Assert.IsTrue(trial.HasSentenceBeenAnnounced(firstCrime.Object));
	}

	[TestMethod]
	public void LegalCustodyEffects_ShouldShowInScore()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICharacter> owner = new();

		var effects = new IScoreAddendumEffect[]
		{
			new AwaitingSentencing(owner.Object, authority.Object, MudDateTime.Never),
			new OnBail(owner.Object, authority.Object, MudDateTime.Never),
			new ServingCustodialSentence(owner.Object, authority.Object, TimeSpan.FromDays(5.0), MudDateTime.Never),
			new AwaitingExecution(owner.Object, authority.Object, MudDateTime.Never)
		};

		foreach (IScoreAddendumEffect effect in effects)
		{
			Assert.IsTrue(effect.ShowInScore);
			Assert.IsFalse(effect.ShowInHealth);
			StringAssert.Contains(effect.ScoreAddendum.StripANSIColour(), "Test Authority");
		}

		StringAssert.Contains(effects[0].ScoreAddendum.StripANSIColour(), "on remand");
		StringAssert.Contains(effects[1].ScoreAddendum.StripANSIColour(), "on bail");
		StringAssert.Contains(effects[2].ScoreAddendum.StripANSIColour(), "custodial sentence");
		StringAssert.Contains(effects[3].ScoreAddendum.StripANSIColour(), "awaiting execution");
		StringAssert.Contains(effects[3].ScoreAddendum.StripANSIColour(), MudDateTime.Never.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal));
	}

	[TestMethod]
	public void LegalSurrenderRoomDescriptionAddenda_ShouldPromptForReturnBailAtPrison()
	{
		Mock<ICell> prison = new();
		Mock<ICell> elsewhere = new();

		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Name).Returns("Test Authority");
		authority.SetupGet(x => x.PrisonLocation).Returns(prison.Object);

		Mock<ICharacter> actor = new();
		actor.SetupGet(x => x.Location).Returns(prison.Object);
		OnBail bail = new(actor.Object, authority.Object, MudDateTime.Never);
		OnBail[] bailEffects = [bail];
		actor.Setup(x => x.EffectsOfType<OnBail>(It.IsAny<Predicate<OnBail>>()))
		     .Returns((Predicate<OnBail>? predicate) => bailEffects.Where(x => predicate?.Invoke(x) ?? true));
		actor.Setup(x => x.EffectsOfType<WarnedByEnforcer>(It.IsAny<Predicate<WarnedByEnforcer>>()))
		     .Returns(Array.Empty<WarnedByEnforcer>());

		List<string> prisonAddenda = Cell.LegalSurrenderRoomDescriptionAddenda(actor.Object, prison.Object)
		                                  .Select(x => x.StripANSIColour())
		                                  .ToList();
		List<string> elsewhereAddenda = Cell.LegalSurrenderRoomDescriptionAddenda(actor.Object, elsewhere.Object)
		                                      .Select(x => x.StripANSIColour())
		                                      .ToList();

		Assert.AreEqual(1, prisonAddenda.Count);
		StringAssert.Contains(prisonAddenda[0], "on bail");
		StringAssert.Contains(prisonAddenda[0], "RETURNBAIL");
		StringAssert.Contains(prisonAddenda[0], "Test Authority");
		Assert.AreEqual(0, elsewhereAddenda.Count);
	}

	[TestMethod]
	public void LegalSurrenderRoomDescriptionAddenda_ShouldPromptForWantedSurrenderWhenEnforcersArePresent()
	{
		Mock<ICell> cell = new();

		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> crime = new();
		Mock<ICharacter> actor = new();
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		Mock<ICharacter> enforcer = new();
		enforcer.Setup(x => x.ColocatedWith(actor.Object)).Returns(true);

		Mock<IPatrol> patrol = new();
		patrol.SetupGet(x => x.PatrolMembers).Returns([enforcer.Object]);

		WarnedByEnforcer warned = new(actor.Object, authority.Object, crime.Object, patrol.Object);
		WarnedByEnforcer[] warningEffects = [warned];
		actor.Setup(x => x.EffectsOfType<OnBail>(It.IsAny<Predicate<OnBail>>()))
		     .Returns(Array.Empty<OnBail>());
		actor.Setup(x => x.EffectsOfType<WarnedByEnforcer>(It.IsAny<Predicate<WarnedByEnforcer>>()))
		     .Returns((Predicate<WarnedByEnforcer>? predicate) => warningEffects.Where(x => predicate?.Invoke(x) ?? true));

		List<string> addenda = Cell.LegalSurrenderRoomDescriptionAddenda(actor.Object, cell.Object)
		                           .Select(x => x.StripANSIColour())
		                           .ToList();

		Assert.AreEqual(1, addenda.Count);
		StringAssert.Contains(addenda[0], "wanted");
		StringAssert.Contains(addenda[0], "SURRENDER");
		StringAssert.Contains(addenda[0], "Test Authority");
	}

	[TestMethod]
	public void LegalSurrenderRoomDescriptionAddenda_ShouldNotPromptForWantedSurrenderWithoutPresentEnforcers()
	{
		Mock<ICell> cell = new();

		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		Mock<ICrime> crime = new();
		Mock<ICharacter> actor = new();
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		Mock<ICharacter> enforcer = new();
		enforcer.Setup(x => x.ColocatedWith(actor.Object)).Returns(false);

		Mock<IPatrol> patrol = new();
		patrol.SetupGet(x => x.PatrolMembers).Returns([enforcer.Object]);

		WarnedByEnforcer warned = new(actor.Object, authority.Object, crime.Object, patrol.Object);
		WarnedByEnforcer[] warningEffects = [warned];
		actor.Setup(x => x.EffectsOfType<OnBail>(It.IsAny<Predicate<OnBail>>()))
		     .Returns(Array.Empty<OnBail>());
		actor.Setup(x => x.EffectsOfType<WarnedByEnforcer>(It.IsAny<Predicate<WarnedByEnforcer>>()))
		     .Returns((Predicate<WarnedByEnforcer>? predicate) => warningEffects.Where(x => predicate?.Invoke(x) ?? true));

		Assert.AreEqual(0, Cell.LegalSurrenderRoomDescriptionAddenda(actor.Object, cell.Object).Count());
	}

	[TestMethod]
	public void OnBail_LoadEffect_ShouldPreserveReturnDueDate()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		All<ILegalAuthority> legalAuthorities = new();
		legalAuthorities.Add(authority.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(legalAuthorities);
		gameworld.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());

		Mock<ICharacter> owner = new();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		OnBail.InitialiseEffectType();
		OnBail bail = new(owner.Object, authority.Object, MudDateTime.Never);

		OnBail loaded = (OnBail)Effect.LoadEffect(bail.SaveToXml(new Dictionary<IEffect, TimeSpan>()), owner.Object);

		Assert.AreEqual(MudDateTime.Never.GetDateTimeString(), loaded.ReturnDueDate.GetDateTimeString());
	}

	[TestMethod]
	public void OnBail_LoadEffect_ShouldReadLegacyArrestTimeAsReturnDueDate()
	{
		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.Id).Returns(1L);
		authority.SetupGet(x => x.Name).Returns("Test Authority");

		All<ILegalAuthority> legalAuthorities = new();
		legalAuthorities.Add(authority.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.LegalAuthorities).Returns(legalAuthorities);
		gameworld.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());

		Mock<ICharacter> owner = new();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		OnBail.InitialiseEffectType();
		XElement legacy = new(
			"Effect",
			new XElement("ApplicabilityProg", 0),
			new XElement("Type", "OnBail"),
			new XElement("Original", 0),
			new XElement("Remaining", 0),
			new XElement("Effect",
				new XElement("LegalAuthority", 1L),
				new XElement("ArrestTime", new XCData(MudDateTime.Never.GetDateTimeString()))
			)
		);

		OnBail loaded = (OnBail)Effect.LoadEffect(legacy, owner.Object);

		Assert.AreEqual(MudDateTime.Never.GetDateTimeString(), loaded.ReturnDueDate.GetDateTimeString());
	}

	[TestMethod]
	public void IsInRemandCell_ShouldRequirePhysicalCellPresence()
	{
		Mock<ICharacter> character = new();
		Mock<ICharacter> otherCharacter = new();

		Mock<ICell> cell = new();
		character.SetupGet(x => x.Location).Returns(cell.Object);

		Mock<ILegalAuthority> authority = new();
		authority.SetupGet(x => x.CellLocations).Returns([cell.Object]);

		Assert.IsTrue(authority.Object.IsInRemandCell(character.Object));
		Assert.IsFalse(authority.Object.IsInRemandCell(otherCharacter.Object));
	}

	[TestMethod]
	public void CrimeDescribeCrimeAtTrial_MissingVictim_ShouldUseUnnamedVictim()
	{
		Mock<ICellOverlay> overlay = new();
		overlay.SetupGet(x => x.CellName).Returns("The Entrance to Easy Street");

		Mock<ICell> location = new();
		location.SetupGet(x => x.Id).Returns(1L);
		location.SetupGet(x => x.CurrentOverlay).Returns(overlay.Object);

		All<ICell> cells = new();
		cells.Add(location.Object);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);

		Mock<ILaw> law = new();
		law.SetupGet(x => x.Id).Returns(10L);
		law.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		law.SetupGet(x => x.CrimeType).Returns(CrimeTypes.GreviousBodilyHarm);

		Crime crime = new(new DbCrime
		{
			Id = 20L,
			LawId = 10L,
			CriminalId = 30L,
			VictimId = 40L,
			LocationId = 1L,
			TimeOfCrime = MudDateTime.Never.GetDateTimeString(),
			RealTimeOfCrime = DateTime.UtcNow,
			CriminalShortDescription = "a defendant",
			CriminalFullDescription = "A defendant is here.",
			CriminalCharacteristics = string.Empty,
			WitnessIds = string.Empty
		}, law.Object, gameworld.Object);

		string description = crime.DescribeCrimeAtTrial(new Mock<IPerceiver>().Object);

		StringAssert.Contains(description,
			"caused grievous bodily harm to an unnamed victim at The Entrance to Easy Street");
		Assert.IsFalse(description.Contains("to at", StringComparison.OrdinalIgnoreCase));
	}
}
