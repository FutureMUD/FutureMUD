using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Magic;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingIntegrationTests
{
	[TestMethod]
	public void PlayerCommands_RefuseAmbiguityAndLaterModes_ExplicitRoutePays()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		MagicModule.MagicGeneric(f.Actor.Object, "earth cast \"Stone Skin\" grade 3 overreach on self");
		Assert.IsTrue(f.Messages.Last().Contains("Multiple routes")); Assert.AreEqual(0, f.Rolls);
		foreach (var mode in new[] { "formula", "quiet", "area" })
		{
			MagicModule.MagicGeneric(f.Actor.Object, "earth " + mode);
			Assert.IsTrue(f.Messages.Last().Contains("unavailable"));
		}
		MagicModule.MagicGeneric(f.Actor.Object, "earth practice \"Stone Skin\" grade 3 overreach via Earth");
		Assert.IsTrue(f.Messages.Last().Contains("Practice is not enabled"));
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
		MagicModule.MagicGeneric(f.Actor.Object, "earth cast \"Stone Skin\" standard");
		Assert.AreEqual(0, f.Rolls);
		MagicModule.MagicGeneric(f.Actor.Object, "earth cast \"Stone Skin\" grade 3 overreach on self via Earth");
		Assert.AreEqual(1, f.Rolls, string.Join(";", f.Messages)); Assert.AreEqual(77.5, f.Balances[f.Resources[1]]);
		f.ActiveCapabilities.Clear();
		Assert.IsTrue(MagicModule.MagicFilterFunction(f.Actor.Object, "earth"));
		MagicModule.MagicGeneric(f.Actor.Object, "earth spells");
		Assert.IsTrue(f.Messages.Last().Contains("acquired, controlled grade 3"));
	}

	[TestMethod]
	public void FutureProgs_CompileTypedContractsAndUseSamePhysicalPaymentBoundary()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var f = new MagicCastingFixture(); f.Acquire();
		foreach (var c in MagicCastingFunction.Contracts)
		{
			var args = c.Parameters.Select((x, i) => Tuple.Create(x, $"arg{i}")).ToArray();
			var prog = new FutureProg(f.World.Object, "test" + c.Name, c.Return, args,
				$"return {c.Name}({string.Join(", ", args.Select(x => "@" + x.Item2))})");
			Assert.IsTrue(prog.Compile(), prog.CompileError);
			if (c.Name == "hasacquiredspell") Assert.IsTrue(prog.ExecuteBool(f.Actor.Object, f.Spell));
			if (c.Name != "channelspell") continue;
			f.Body.Setup(x => x.Communications.CanVocalise(f.Body.Object)).Returns(false);
			Assert.IsTrue(prog.ExecuteString(f.Actor.Object, f.Earth, f.Spell, 3M, true, "self").StartsWith("Refused"));
			Assert.AreEqual(0, f.Rolls);
			f.Body.Setup(x => x.Communications.CanVocalise(f.Body.Object)).Returns(true);
			Assert.IsTrue(prog.ExecuteString(f.Actor.Object, f.Earth, f.Spell, 3M, true, "self").StartsWith("Succeeded"));
			Assert.AreEqual(1, f.Rolls); Assert.AreEqual(77.5, f.Balances[f.Resources[1]]);
		}
	}

	[TestMethod]
	public void UnknownLegacyEffect_AppliesOnceWithoutProvingMastery()
	{
		var f = new MagicCastingFixture(); f.Acquire(); SetEffect(f, "<Effect type='blindness'/>");
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<SpellBlindnessEffect>()), Times.Once);
		Assert.AreEqual(0, f.Samples); Assert.AreEqual(1, f.SkillUses);
	}

	[TestMethod]
	public void NativeGroup_MultipleAppliedTargets_UsesOneSkillAndMasteryOpportunityAndOneCasterEffect()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		((List<IMagicSpellEffectTemplate>)f.Spell.CasterSpellEffects).Add(f.Spell.SpellEffects.Single().Clone());
		CaptureTarget(f, new PerceivableGroup([f.Actor.Object, f.Actor.Object]));
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(1, f.Samples); Assert.AreEqual(1, f.Rolls);
		// Two targets and one caster effect; the caster effect's authored fixed zero scalar is retained.
		f.Actor.Verify(x => x.AddEffect(It.IsAny<SpellTraitBoostEffect>()), Times.Exactly(3));
	}

	[TestMethod]
	public void Group_EmptyPreservesNativeSuccess_ButCasterEffectCannotEarnMastery()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		((List<IMagicSpellEffectTemplate>)f.Spell.CasterSpellEffects).Add(f.Spell.SpellEffects.Single().Clone());
		CaptureTarget(f, new PerceivableGroup([]));
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(0, f.Samples);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
	}

	[TestMethod]
	public void Group_AllTargetsWardBlocked_PaysOnceAndCannotEarnMastery()
	{
		var f = new MagicCastingFixture(); f.Acquire();
		var ward = new Mock<IMagicInterdictionEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail);
		ward.Setup(x => x.ShouldInterdict(f.Actor.Object, f.School)).Returns(true);
		f.Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		CaptureTarget(f, new PerceivableGroup([f.Actor.Object, f.Actor.Object]));
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(77.5, f.Balances[f.Resources[1]]); Assert.AreEqual(1, f.SkillUses); Assert.AreEqual(0, f.Samples);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void InstantaneousDamage_RequiresDeliveredWoundMagnitude_AndRunsOnlyOnce(bool delivered)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		SetEffect(f, "<Effect type='damage'><DamageType>0</DamageType><DamageExpression>grade*5</DamageExpression><Bodypart>0</Bodypart><Limb>-1</Limb></Effect>");
		var wound = new Mock<IWound>(); wound.SetupGet(x => x.CurrentDamage).Returns(delivered ? 15 : 0);
		f.Actor.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns([wound.Object]);
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		f.Actor.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Once);
		Assert.AreEqual(delivered ? 1 : 0, f.Samples);
		Assert.AreEqual(delivered ? 3 : 2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RemoveBlindness_NoOpCannotMaster_ActualRemovalQualifiesOnce(bool present)
	{
		var f = new MagicCastingFixture(); f.Acquire(); SetEffect(f, "<Effect type='removeblindness'/>");
		var effect = new SpellBlindnessEffect(f.Actor.Object, new Mock<IMagicSpellEffectParent>().Object, null!);
		var effects = present ? new List<SpellBlindnessEffect> { effect } : [];
		f.Actor.Setup(x => x.EffectsOfType<SpellBlindnessEffect>(It.IsAny<Predicate<SpellBlindnessEffect>>())).Returns(() => effects);
		f.Actor.Setup(x => x.RemoveAllEffects<SpellBlindnessEffect>(It.IsAny<Predicate<SpellBlindnessEffect>>(), true)).Callback(() => effects.Clear());
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		f.Actor.Verify(x => x.RemoveAllEffects<SpellBlindnessEffect>(It.IsAny<Predicate<SpellBlindnessEffect>>(), true), Times.Once);
		Assert.AreEqual(present ? 1 : 0, f.Samples);
	}

	[TestMethod]
	public void SharedTraitAndPrerequisites_AcquireOnlyTheEnrolledPermanentRouteEdges()
	{
		var f = new MagicCastingFixture();
		var ward = f.NewSpell(2, "Wardcraft", "<Effect type='personaltagward'/>");
		ward.BuildingCommand(f.Actor.Object, new StringStack("grades fixture"));
		var voidWard = f.NewSpell(3, "Void Wardcraft", "<Effect type='personaltagward'/>");
		voidWard.BuildingCommand(f.Actor.Object, new StringStack("grades fixture"));
		Assert.IsTrue(f.Sorcerer.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 3")));
		Assert.IsTrue(f.Sorcerer.BuildingCommand(f.Actor.Object, new StringStack("casting prerequisite add 3 1 2 40")));
		foreach (var cap in new[] { f.Earth, f.Sorcerer })
		{
			Assert.IsTrue(cap.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
			Assert.IsTrue(cap.BuildingCommand(f.Actor.Object, new StringStack("casting prerequisite add 2 1 2 40")));
		}
		var merit = new Mock<IMagicCapabilityMerit>(); merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.SetupGet(x => x.Capabilities).Returns(new[] { f.Earth });
		f.Actor.SetupGet(x => x.Merits).Returns(new IMerit[] { merit.Object });
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "permanent Earth route").Allowed);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 2));
		f.Acquire(2); f.Service.NotifyProgress(f.Actor.Object, spellId: 1);
		Assert.IsNotNull(f.Service.Acquisition(f.Actor.Object, 2));
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 3));
		Assert.IsNull(f.Store.Enrolment(100, f.Sorcerer.CastingPolicy!.Identity));
		Assert.AreEqual(2, f.Store.Acquired.Count);
	}

	[TestMethod]
	public void OldAndVancianDefinitions_DoNotImplicitlyEnableConfiguredCasting()
	{
		var f = new MagicCastingFixture();
		var root = XElement.Parse(f.Earth.SaveToXml()); root.Element("Casting")!.Remove();
		var model = new MudSharp.Models.MagicCapability { Id = 90, Name = "Old", MagicSchoolId = 1, CapabilityModel = "skilllevel", Definition = root.ToString() };
		var old = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(model, f.World.Object);
		Assert.IsFalse(old.HasCastingPolicy); Assert.IsNull(XElement.Parse(old.SaveToXml()).Element("Casting"));
		root.Add(XElement.Parse(f.Earth.SaveToXml()).Element("Casting")); model.CapabilityModel = "vancian"; model.Definition = root.ToString();
		var vancian = new VancianMagicCapability(model, f.World.Object);
		Assert.IsTrue(vancian.CastingConfigurationErrors().Any(x => x.Contains("Vancian")));
		Assert.IsFalse(vancian.BuildingCommand(f.Actor.Object, new StringStack("casting enable on")));
	}

	private static void SetEffect(MagicCastingFixture f, string effect)
	{
		f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades scalar remove target 0"));
		var effects = (List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects; effects.Clear();
		effects.Add(SpellEffectFactory.LoadEffect(XElement.Parse(effect), f.Spell));
	}
	private static void CaptureTarget(MagicCastingFixture f, IPerceivable target)
	{
		var trigger = new Mock<ICastMagicTrigger>(); var xml = f.Spell.Trigger.SaveToXml();
		trigger.Setup(x => x.SaveToXml()).Returns(xml); trigger.SetupGet(x => x.TargetTypes).Returns("characters");
		trigger.SetupGet(x => x.MinimumPower).Returns(SpellPower.Insignificant); trigger.SetupGet(x => x.MaximumPower).Returns(SpellPower.RecklesslyPowerful);
		trigger.SetupGet(x => x.TriggerYieldsTarget).Returns(true);
		trigger.Setup(x => x.DoTriggerCast(f.Actor.Object, It.IsAny<StringStack>())).Callback<ICharacter, StringStack>((a, s) =>
		{ var power = Enum.Parse<SpellPower>(s.PopSpeech()); f.Spell.CastSpell(a, target, power); });
		f.Spell.Trigger = trigger.Object;
	}
}
