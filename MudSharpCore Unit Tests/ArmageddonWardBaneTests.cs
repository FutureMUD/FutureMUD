#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWardBaneTests
{
	[DataTestMethod]
	[DataRow(MagicInterdictionCoverage.Incoming, true, false)]
	[DataRow(MagicInterdictionCoverage.Outgoing, false, true)]
	[DataRow(MagicInterdictionCoverage.Both, true, true)]
	public void RefugeWard_UsesNativeBoundaryCoverageAndExactSelectors(MagicInterdictionCoverage coverage, bool incoming, bool outgoing)
	{
		var f = new MagicCastingFixture(); var room = new Mock<IRoom>(); room.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var configuration = new SpellShelterWardConfiguration([1], ["elemental-fire"], coverage);
		var effect = new SpellShelterWard(SpellShelterWard.Envelope(Guid.NewGuid(), 1, configuration), room.Object);
		room.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([effect]);
		Assert.AreEqual(incoming, MagicInterdictionHelper.GetInterdiction(f.Actor.Object, room.Object, f.School, false) is not null);
		var outside = new Mock<IRoom>(); f.Actor.SetupGet(x => x.Location).Returns(room.Object);
		Assert.AreEqual(outgoing, MagicInterdictionHelper.GetInterdiction(f.Actor.Object, outside.Object, f.School, false) is not null);
		var other = Mock.Of<IMagicSchool>(x => x.Id == 9);
		Assert.IsNull(MagicInterdictionHelper.GetInterdiction(f.Actor.Object, outside.Object, other, false));
		Assert.AreEqual(outgoing, MagicInterdictionHelper.GetInterdiction(f.Actor.Object, outside.Object, other, false,
			[new MagicInterdictionTag("ELEMENTAL-FIRE", "authored")]) is not null);
	}

	[TestMethod]
	public void WardAuthority_ReloadsExactJournalWard_RejectsForeignChangesAndAllowsMissingWardRetirement()
	{
		var f = new MagicCastingFixture(); var room = new Mock<IRoom>(); room.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		room.SetupGet(x => x.Hooks).Returns([]);
		var configuration = new SpellShelterWardConfiguration([1], [], MagicInterdictionCoverage.Both);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 1, 100, SpellShelterAnchor.Family,
			SpellLifecycleMode.TemporaryCleanup, f.Now, f.Now.AddSeconds(60), "typed fixture");
		var envelope = SpellShelterWard.Envelope(origin.Id, origin.SpellId, configuration);
		var effect = new SpellShelterWard(envelope, room.Object);
		room.SetupGet(x => x.Effects).Returns([effect]);
		SpellOwnedShelterService.RequireWardAuthority(room.Object, origin, configuration, true);
		SpellOwnedShelterService.RequireRetirementEffects(room.Object, origin, configuration);
		var saved = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		Assert.IsTrue(XNode.DeepEquals(envelope, saved));
		SpellOwnedShelterService.RequirePersistedWardAuthority(new XElement("Effects", saved).ToString(), origin, configuration);
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireWardAuthority(room.Object,
			origin with { Id = Guid.NewGuid() }, configuration, true));
		room.SetupGet(x => x.Effects).Returns([]);
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireWardAuthority(room.Object, origin, configuration, true));
		SpellOwnedShelterService.RequireRetirementEffects(room.Object, origin, configuration);
		room.SetupGet(x => x.Effects).Returns([effect, Mock.Of<IEffect>()]);
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireRetirementEffects(room.Object, origin, configuration));
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequirePersistedWardAuthority(
			new XElement("Effects", envelope, new XElement("Effect", new XElement("Type", "Foreign"))).ToString(), origin, configuration));
	}

	private delegate bool ExecutePolicy(out object value, object[] parameters);
	[DataTestMethod]
	[DataRow("hit")][DataRow("ineligible")][DataRow("drift")][DataRow("ward")][DataRow("resist")]
	public void BaneCasting_EligibilityFencesPayment_EligibleWardAndResistanceStayPaid(string scenario)
	{
		var f = new MagicCastingFixture(); var eligible = scenario != "ineligible";
		var target = f.Actor.Object;
		if (scenario == "resist")
		{
			var other = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
			other.SetupGet(x => x.Id).Returns(101); other.SetupGet(x => x.InstanceId).Returns(101);
			other.SetupGet(x => x.GetObject).Returns(other.Object); other.SetupGet(x => x.Gameworld).Returns(f.World.Object);
			other.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
			Mock.Get(other.Object.Body).SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
			other.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([]);
			target = other.Object;
		}
		var prog = CastingTargetFilterTests.Policy(72, "healthy");
		object value = true;
		prog.Setup(x => x.ExecuteWithStatus(out value, It.IsAny<object[]>())).Returns(new ExecutePolicy((out object result, object[] args) =>
		{
			Assert.AreSame(target, args[0]); Assert.AreSame(f.Actor.Object, args[1]);
			result = eligible; return true;
		}));
		var known = f.World.Object.FutureProgs.Get(1);
		f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { known, prog.Object }));
		var content = ArmageddonBaneContent.Create(1, Difficulty.Normal, 8, 40, DamageType.Arcane);
		var spell = f.NewSpell(2, "Apex Bane", content.BuildDefinition(11, 1, 72).Element("Effects")!.Elements().Single().ToString());
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		CastingTargetFilterTests.SetFilter(spell, "character", 72);
		spell.OpposedTrait = f.Traits[0]; spell.OpposedDifficulty = Difficulty.Normal;
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
		f.Store.Write(acquired: new(100, 2, 7, 1, f.Now, "fixture", DateTime.UnixEpoch, 0));
		f.Actor.Setup(x => x.TargetActorOrCorpse("target", It.IsAny<PerceiveIgnoreFlags>())).Returns(target);
		var resistance = new Mock<ICheck>();
		resistance.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<MudSharp.Body.Traits.ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck,
				scenario == "resist" ? Outcome.MajorPass : Outcome.MajorFail)));
		f.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(resistance.Object);
		if (scenario == "ward")
		{
			var ward = new Mock<IMagicInterdictionEffect>(); ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
			ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail); ward.Setup(x => x.ShouldInterdict(f.Actor.Object, f.School)).Returns(true);
			f.Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		}
		var amount = 0.0;
		f.Actor.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{ amount += damage.DamageAmount; var wound = new Mock<IWound>(); wound.SetupGet(x => x.CurrentDamage).Returns(damage.DamageAmount); return [wound.Object]; });
		f.Checkpoint = stage => { if (scenario == "drift" && stage == "BeforePayment") eligible = false; };
		var intent = new MagicCastingIntent(f.Actor.Object, f.Earth.Id, 2, 1, false, "target", OriginId: Guid.NewGuid());
		var result = f.Service.Cast(intent);
		var refused = scenario is "ineligible" or "drift";
		Assert.AreEqual(refused ? MagicCastingStatus.Refused : scenario == "hit" ? MagicCastingStatus.Succeeded : MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(refused ? 100.0 : 95.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(scenario == "hit" ? 8.0 : 0.0, amount);
		if (refused) Assert.IsNull(result.OperationId);
		else { Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(intent).Status); Assert.AreEqual(95.0, f.Balances[f.Resources[1]]); }
	}

	[DataTestMethod]
	[DataRow(1, 8.0)][DataRow(2, 16.0)][DataRow(3, 24.0)][DataRow(4, 32.0)]
	[DataRow(5, 40.0)][DataRow(6, 40.0)][DataRow(7, 40.0)]
	public void BaneDamage_BindsSelectedGradeAndHonoursAuthoredCap(int grade, double expected)
	{
		var f = new MagicCastingFixture();
		var content = ArmageddonBaneContent.Create(1, Difficulty.Normal, 8, 40, DamageType.Arcane);
		var spell = f.NewSpell(2, "Apex Bane", content.BuildDefinition(11, 1, 0).Element("Effects")!.Elements().Single().ToString());
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		var detached = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyStrong, Difficulty.Normal, 7);
		var effect = (MudSharp.Magic.SpellEffects.DamageEffect)detached.SpellEffects.Single();
		Assert.AreEqual(expected, effect.DamageExpression.EvaluateWith(f.Actor.Object, values: [("outcome", 0)]));
	}
}
