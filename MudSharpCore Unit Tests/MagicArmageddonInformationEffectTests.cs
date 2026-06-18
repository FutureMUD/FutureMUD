#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicArmageddonInformationEffectTests
{
	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersArmageddonInformationEffects()
	{
		var effectTypes = SpellEffectFactory.MagicEffectTypes.ToList();

		CollectionAssert.Contains(effectTypes, "identify");
		CollectionAssert.Contains(effectTypes, "reciteproxy");
		CollectionAssert.Contains(effectTypes, "deadspeak");

		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("identify").MatchingTriggers, "self");
		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("identify").MatchingTriggers, "character");
		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("reciteproxy").MatchingTriggers, "self");
		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("reciteproxy").MatchingTriggers, "character");
		CollectionAssert.Contains(SpellEffectFactory.BuilderInfoForType("deadspeak").MatchingTriggers, "corpse");
	}

	[TestMethod]
	public void SpellEffectFactory_LoadEffect_RoundTripsArmageddonInformationXml()
	{
		var spell = new Mock<IMagicSpell>();
		foreach (var definition in Definitions())
		{
			var effect = SpellEffectFactory.LoadEffect(definition, spell.Object);
			var saved = effect.SaveToXml();

			Assert.AreEqual(definition.Attribute("type")?.Value, saved.Attribute("type")?.Value);
		}
	}

	[TestMethod]
	public void SpellIdentifyEffect_GetLookText_UsesStoredCasterInstanceForTwoParameterProg()
	{
		var fixture = CreateIdentifyFixture();
		fixture.Prog
		       .Setup(x => x.MatchesParameters(It.Is<IEnumerable<ProgVariableTypes>>(MatchesParameters(
			       ProgVariableTypes.Character, ProgVariableTypes.Character))))
		       .Returns(true);
		fixture.Prog
		       .Setup(x => x.ExecuteString(It.Is<object[]>(args =>
			       ReferenceEquals(args[0], fixture.Target.Object) &&
			       ReferenceEquals(args[1], fixture.SecondaryCaster.Object))))
		       .Returns("Their aura is bright.");

		var effect = new SpellIdentifyEffect(
			fixture.Target.Object,
			fixture.Parent.Object,
			fixture.IdentityId,
			fixture.SecondaryInstanceId,
			fixture.Prog.Object);

		var text = effect.GetLookText(fixture.Target.Object);

		Assert.AreEqual("Their aura is bright.", text);
		Assert.AreEqual(fixture.SecondaryInstanceId, effect.CasterInstanceId);
		fixture.Prog.Verify(x => x.ExecuteString(It.Is<object[]>(args =>
			ReferenceEquals(args[0], fixture.Target.Object) &&
			ReferenceEquals(args[1], fixture.SecondaryCaster.Object))), Times.Once);
	}

	[TestMethod]
	public void SpellIdentifyEffect_GetLookText_FallsBackToSingleParameterProgWhenCasterUnavailable()
	{
		var fixture = CreateIdentifyFixture();
		fixture.Gameworld
		       .Setup(x => x.TryGetCharacter(fixture.IdentityId, true))
		       .Returns((ICharacter)null!);
		fixture.Prog
		       .Setup(x => x.MatchesParameters(It.Is<IEnumerable<ProgVariableTypes>>(MatchesParameters(
			       ProgVariableTypes.Character, ProgVariableTypes.Character))))
		       .Returns(true);
		fixture.Prog
		       .Setup(x => x.MatchesParameters(It.Is<IEnumerable<ProgVariableTypes>>(MatchesParameters(
			       ProgVariableTypes.Character))))
		       .Returns(true);
		fixture.Prog
		       .Setup(x => x.ExecuteString(It.Is<object[]>(args =>
			       ReferenceEquals(args[0], fixture.Target.Object) && args.Length == 1)))
		       .Returns("Only the target matters.");

		var effect = new SpellIdentifyEffect(
			fixture.Target.Object,
			fixture.Parent.Object,
			fixture.IdentityId,
			fixture.SecondaryInstanceId,
			fixture.Prog.Object);

		var text = effect.GetLookText(fixture.Target.Object);

		Assert.AreEqual("Only the target matters.", text);
		fixture.Prog.Verify(x => x.ExecuteString(It.Is<object[]>(args =>
			ReferenceEquals(args[0], fixture.Target.Object) && args.Length == 1)), Times.Once);
		fixture.Prog.Verify(x => x.ExecuteString(It.Is<object[]>(args => args.Length == 2)), Times.Never);
	}

	[TestMethod]
	public void SpellIdentifyEffect_GetLookText_SuppressesBlankProgOutput()
	{
		var fixture = CreateIdentifyFixture();
		fixture.Prog
		       .Setup(x => x.MatchesParameters(It.Is<IEnumerable<ProgVariableTypes>>(MatchesParameters(
			       ProgVariableTypes.Character))))
		       .Returns(true);
		fixture.Prog
		       .Setup(x => x.ExecuteString(It.Is<object[]>(args =>
			       ReferenceEquals(args[0], fixture.Target.Object) && args.Length == 1)))
		       .Returns("  ");

		var effect = new SpellIdentifyEffect(
			fixture.Target.Object,
			fixture.Parent.Object,
			0,
			0,
			fixture.Prog.Object);

		Assert.IsNull(effect.GetLookText(fixture.Target.Object));
	}

	private static XElement[] Definitions()
	{
		return
		[
			new XElement("Effect", new XAttribute("type", "identify"),
				new XElement("IdentifyProgId", 1L)),
			new XElement("Effect", new XAttribute("type", "reciteproxy"),
				new XElement("LinkProgId", 1L),
				new XElement("RelayChance", 1.0),
				new XElement("TargetEcho", new XCData("")),
				new XElement("ReciteEcho", new XCData("@ recite|recites"))),
			new XElement("Effect", new XAttribute("type", "deadspeak"),
				new XElement("LinkProgId", 1L),
				new XElement("AllowPCs", true),
				new XElement("AllowNPCs", true),
				new XElement("AllowAdmins", false),
				new XElement("AllowFinal", true),
				new XElement("AllowSkeletal", false),
				new XElement("RelayChance", 1.0),
				new XElement("TargetEcho", new XCData("")),
				new XElement("RoomEcho", new XCData("@ rise|rises")),
				new XElement("CollapseEcho", new XCData("@ collapse|collapses")),
				new XElement("RestoreEcho", new XCData("@ settle|settles back into $1")),
				new XElement("ReciteEcho", new XCData("@ recite|recites")))
		];
	}

	private static Expression<Func<IEnumerable<ProgVariableTypes>, bool>> MatchesParameters(
		params ProgVariableTypes[] expected)
	{
		return actual => actual.SequenceEqual(expected);
	}

	private static IdentifyFixture CreateIdentifyFixture()
	{
		const long identityId = 100L;
		const long primaryInstanceId = 100L;
		const long secondaryInstanceId = 200L;
		const long progId = 10L;

		var futureProgs = new Mock<IUneditableAll<IFutureProg>>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);

		var target = new Mock<ICharacterInstance>();
		target.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var primaryCaster = new Mock<ICharacterInstance>();
		var secondaryCaster = new Mock<ICharacterInstance>();
		var identity = new Mock<ICharacterIdentity>();
		identity.SetupGet(x => x.Id).Returns(identityId);
		identity.SetupGet(x => x.Instances).Returns([primaryCaster.Object, secondaryCaster.Object]);
		primaryCaster.SetupGet(x => x.Id).Returns(identityId);
		primaryCaster.SetupGet(x => x.Identity).Returns(identity.Object);
		primaryCaster.SetupGet(x => x.InstanceId).Returns(primaryInstanceId);
		secondaryCaster.SetupGet(x => x.Id).Returns(identityId);
		secondaryCaster.SetupGet(x => x.Identity).Returns(identity.Object);
		secondaryCaster.SetupGet(x => x.InstanceId).Returns(secondaryInstanceId);
		gameworld.Setup(x => x.TryGetCharacter(identityId, true)).Returns(primaryCaster.Object);

		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(progId);
		futureProgs.Setup(x => x.Get(progId)).Returns(prog.Object);

		var parent = new Mock<IMagicSpellEffectParent>();

		return new IdentifyFixture(
			Gameworld: gameworld,
			FutureProgs: futureProgs,
			Prog: prog,
			Parent: parent,
			Target: target,
			PrimaryCaster: primaryCaster,
			SecondaryCaster: secondaryCaster,
			IdentityId: identityId,
			SecondaryInstanceId: secondaryInstanceId);
	}

	private sealed record IdentifyFixture(
		Mock<IFuturemud> Gameworld,
		Mock<IUneditableAll<IFutureProg>> FutureProgs,
		Mock<IFutureProg> Prog,
		Mock<IMagicSpellEffectParent> Parent,
		Mock<ICharacterInstance> Target,
		Mock<ICharacterInstance> PrimaryCaster,
		Mock<ICharacterInstance> SecondaryCaster,
		long IdentityId,
		long SecondaryInstanceId);
}
