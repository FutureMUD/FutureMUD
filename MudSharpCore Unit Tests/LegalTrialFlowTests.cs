#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
}
