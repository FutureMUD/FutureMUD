#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonTurnElementsTests
{
	[DataTestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void Content_NativeFactoryGradeLifetimeAndOptionalLegacySchema(int grade)
	{
		var x = new Fixture(grade);
		Assert.AreEqual(30.0 * grade, x.WardSpell.EffectDurationExpression.Evaluate(x.F.Actor.Object));
		Assert.IsTrue(x.Ward.OneUse); Assert.IsFalse(x.Ward.Used);
		var template = (PersonalWardEffect)x.WardSpell.SpellEffects.Single();
		Assert.IsTrue(XNode.DeepEquals(template.SaveToXml(), template.Clone().SaveToXml()));
		var legacy = template.SaveToXml(); legacy.Element("OneUse")!.Remove();
		var oldTemplate = (PersonalWardEffect)SpellEffectFactory.LoadEffect(legacy, x.WardSpell);
		Assert.IsFalse(oldTemplate.OneUse); Assert.IsNull(oldTemplate.SaveToXml().Element("OneUse"));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonTurnElementsContent.Create(0));
		Assert.AreEqual(7.0, ArmageddonTurnElementsContent.Create(1).Content.MinimumEnergy);
	}

	[TestMethod]
	public void Query_DoesNotSpendNoneligibleOrRecursionGuardAndCommitsOnlyCapturedWard()
	{
		var x = new Fixture(1);
		Assert.IsNull(x.Query(Mock.Of<IMagicSchool>()));
		var query = x.Query(); Assert.IsNotNull(query); Assert.IsFalse(x.Ward.Used);
		Assert.IsNotNull(x.Query()); Assert.IsFalse(x.Ward.Used);
		Assert.AreEqual(MagicInterdictionMode.Fail, x.Query(allowReflection: false)!.Mode);
		Assert.IsFalse(MagicInterdictionHelper.TryCommitReflection(x.Query(allowReflection: false)!));
		Assert.IsFalse(x.Ward.Used);
		Assert.IsTrue(MagicInterdictionHelper.TryCommitReflection(query)); Assert.IsTrue(x.Ward.Used);
		Assert.IsNull(x.Query()); Assert.IsFalse(MagicInterdictionHelper.TryCommitReflection(query));
		x.Target.VerifySet(a => a.EffectsChanged = true, Times.Once);
	}

	[TestMethod]
	public void Spent_NativeParentColdReloadKeepsUsedChildAndExpiryRemovesIt()
	{
		var x = new Fixture(1); var clock = new TestClock(x.F.Now); using var scope = RuntimeClock.Push(clock);
		var scheduler = new EffectScheduler(x.F.World.Object, clock); x.F.World.SetupGet(a => a.EffectScheduler).Returns(scheduler);
		Assert.IsTrue(MagicInterdictionHelper.TryCommitReflection(x.Query()!));
		MagicSpellParent.InitialiseEffectType(); SpellPersonalWardEffect.InitialiseEffectType();
		var saved = x.Parent.SaveToXml(new() { [x.Parent] = TimeSpan.FromSeconds(23) });
		var cold = (MagicSpellParent)Effect.LoadEffect(saved, x.Target.Object);
		var child = (SpellPersonalWardEffect)cold.SpellEffects.Single();
		Assert.IsTrue(child.OneUse); Assert.IsTrue(child.Used);
		Assert.IsFalse(child.ShouldInterdict(x.F.Actor.Object, x.F.School));
		x.TargetEffects.Add(cold);
		scheduler.AddSchedule(new EffectSchedule(cold, TimeSpan.FromSeconds(23)));
		clock.Advance(TimeSpan.FromSeconds(23)); scheduler.CheckSchedules();
		Assert.IsFalse(x.TargetEffects.Contains(cold)); Assert.IsFalse(x.TargetEffects.Contains(child));
	}

	[DataTestMethod]
	[DataRow("detach")]
	[DataRow("body")]
	[DataRow("source-body")]
	[DataRow("target-location")]
	[DataRow("source-location")]
	public void ApplicabilityCallback_StaleUseHoldsPaidRecipientWithoutSpendingOrRetargeting(string change)
	{
		var x = new Fixture(1, true);
		x.Prog!.Setup(a => a.ExecuteBool(It.IsAny<object[]>())).Returns(() =>
		{
			if (change == "detach") x.TargetEffects.Remove(x.Ward);
			else if (change == "body") x.Target.SetupGet(a => a.Body).Returns(Mock.Of<IBody>());
			else if (change == "target-location") x.Target.SetupGet(a => a.Location).Returns(Mock.Of<IRoom>());
			else if (change == "source-location") x.F.Actor.SetupGet(a => a.Location).Returns(Mock.Of<IRoom>());
			else x.F.Actor.SetupGet(a => a.Body).Returns(Mock.Of<IBody>());
			return true;
		});
		var query = x.Query(); Assert.IsNotNull(query);
		Assert.IsFalse(MagicInterdictionHelper.TryCommitReflection(query)); Assert.IsFalse(x.Ward.Used);
		Assert.AreEqual(0, x.SourceEffects.OfType<SpellTrackMarkEffect>().Count());
		Assert.AreEqual(0, x.TargetEffects.OfType<SpellTrackMarkEffect>().Count());
	}

	[TestMethod]
	public void PostWriteCallback_UsedStateSurvivesUncertaintyAndCannotReplay()
	{
		var x = new Fixture(1);
		x.Target.SetupSet(a => a.EffectsChanged = true).Callback(() => x.TargetEffects.Remove(x.Parent));
		Assert.ThrowsException<InvalidOperationException>(() => MagicInterdictionHelper.TryCommitReflection(x.Query()!));
		Assert.IsTrue(x.Ward.Used); Assert.IsNull(x.Query());
		Assert.AreEqual("true", x.Ward.SaveToXml(new()).Element("Effect")!.Element("Used")!.Value);
	}

	[TestMethod]
	public void PaidMagicSpell_FirstInvocationReflectsThenSecondReachesOriginalRecipient()
	{
		var x = new Fixture(1); x.F.Acquire();
		x.F.Actor.Setup(a => a.TargetActorOrCorpse("warded", It.IsAny<PerceiveIgnoreFlags>())).Returns(x.Target.Object);
		var first = x.F.Service.Cast(x.F.Intent(1, false) with { Targets = "warded" });
		Assert.IsTrue(x.Ward.Used, first.Message);
		Assert.AreEqual(1, x.SourceEffects.OfType<SpellTrackMarkEffect>().Count(), first.Message);
		Assert.AreEqual(0, x.TargetEffects.OfType<SpellTrackMarkEffect>().Count());
		Assert.IsTrue(x.F.Balances[x.F.Resources[1]] < 100);
		var balance = x.F.Balances[x.F.Resources[1]];
		var second = x.F.Service.Cast(x.F.Intent(1, false) with { Targets = "warded" });
		Assert.AreEqual(1, x.TargetEffects.OfType<SpellTrackMarkEffect>().Count(), second.Message);
		Assert.IsTrue(x.F.Balances[x.F.Resources[1]] < balance);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PaidCallback_StaleAuthorityHoldsRecipientAndPostWriteUncertaintyRetainsPayment(bool afterWrite)
	{
		var x = new Fixture(1, !afterWrite); x.F.Acquire();
		x.F.Actor.Setup(a => a.TargetActorOrCorpse("warded", It.IsAny<PerceiveIgnoreFlags>())).Returns(x.Target.Object);
		if (afterWrite)
			x.Target.SetupSet(a => a.EffectsChanged = true).Callback(() => x.TargetEffects.Remove(x.Parent));
		else
			x.Prog!.Setup(a => a.ExecuteBool(It.IsAny<object[]>())).Returns(() => { x.TargetEffects.Remove(x.Ward); return true; });
		var result = x.F.Service.Cast(x.F.Intent(1, false) with { Targets = "warded" });
		Assert.AreEqual(afterWrite, x.Ward.Used, result.Message);
		Assert.AreEqual(0, x.SourceEffects.OfType<SpellTrackMarkEffect>().Count(), result.Message);
		Assert.AreEqual(0, x.TargetEffects.OfType<SpellTrackMarkEffect>().Count(), result.Message);
		Assert.IsTrue(x.F.Balances[x.F.Resources[1]] < 100, result.Message);
	}

	[TestMethod]
	public void MalformedOneUse_ThrowsDuringNativeLoadBeforeAnyPayment()
	{
		var x = new Fixture(1);
		foreach (var value in new[] { "maybe", "-1" })
		{
			var definition = x.WardSpell.SpellEffects.Single().SaveToXml(); definition.SetElementValue("OneUse", value);
			Assert.ThrowsException<FormatException>(() => SpellEffectFactory.LoadEffect(definition, x.WardSpell));
		}
		var invalid = x.WardSpell.SpellEffects.Single().SaveToXml(); invalid.SetElementValue("Mode", "Fail");
		Assert.ThrowsException<FormatException>(() => SpellEffectFactory.LoadEffect(invalid, x.WardSpell));
		Assert.AreEqual(100.0, x.F.Balances[x.F.Resources[1]]); Assert.AreEqual(0, x.F.Rolls);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PaidReflectionOutputCallback_HoldsPayloadAfterAuthorityChangesAndRetainsSpentUse(bool bodyChange)
	{
		var x = new Fixture(1); x.F.Acquire();
		x.F.Actor.Setup(a => a.TargetActorOrCorpse("warded", It.IsAny<PerceiveIgnoreFlags>())).Returns(x.Target.Object);
		Mock.Get(x.Target.Object.OutputHandler).Setup(a => a.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) =>
			{
				if (!text.Contains("Your ward reflects")) return;
				if (bodyChange) x.Target.SetupGet(a => a.Body).Returns(Mock.Of<IBody>());
				else x.TargetEffects.Remove(x.Parent);
			});
		var result = x.F.Service.Cast(x.F.Intent(1, false) with { Targets = "warded" });
		Assert.IsTrue(x.Ward.Used, result.Message);
		Assert.AreEqual(0, x.SourceEffects.OfType<SpellTrackMarkEffect>().Count(), result.Message);
		Assert.AreEqual(0, x.TargetEffects.OfType<SpellTrackMarkEffect>().Count(), result.Message);
		Assert.IsTrue(x.F.Balances[x.F.Resources[1]] < 100, result.Message);
	}

	[TestMethod]
	public void PaidPayloadEmoteCallback_RechecksSameCommittedUseBeforeApplyingPayload()
	{
		var x = new Fixture(1); x.F.Acquire();
		x.F.Actor.Setup(a => a.TargetActorOrCorpse("warded", It.IsAny<PerceiveIgnoreFlags>())).Returns(x.Target.Object);
		((MagicSpell)x.F.Spells[0]).TargetEmote = "$0 shimmer|shimmers with incoming magic.";
		x.F.Actor.SetupGet(a => a.Corpse).Returns((MudSharp.GameItems.Interfaces.ICorpse)null!);
		Mock.Get(x.F.Actor.Object.Location).SetupGet(a => a.RouteDefinition).Returns((RouteRoomDefinition)null!);
		Mock.Get(x.F.Actor.Object.Location).Setup(a => a.LayerCharacters(It.IsAny<RoomLayer>())).Returns([x.F.Actor.Object]);
		Mock.Get(x.F.Actor.Object.OutputHandler).SetupGet(a => a.Perceiver).Returns(x.F.Actor.Object);
		Mock.Get(x.F.Actor.Object.OutputHandler).Setup(a => a.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<IOutput, bool, bool>((_, _, _) =>
			{
				if (x.Ward.Used) x.F.Actor.SetupGet(a => a.Body).Returns(Mock.Of<IBody>());
			});
		var result = x.F.Service.Cast(x.F.Intent(1, false) with { Targets = "warded" });
		Assert.IsTrue(x.Ward.Used, result.Message);
		Assert.AreEqual(0, x.SourceEffects.OfType<SpellTrackMarkEffect>().Count(), result.Message);
		Assert.AreEqual(0, x.TargetEffects.OfType<SpellTrackMarkEffect>().Count(), result.Message);
		Assert.IsTrue(x.F.Balances[x.F.Resources[1]] < 100, result.Message);
	}

	private sealed class Fixture
	{
		public MagicCastingFixture F { get; } = new();
		public Mock<ICharacter> Target { get; } = new() { DefaultValue = DefaultValue.Mock };
		public List<IEffect> TargetEffects { get; } = [];
		public List<IEffect> SourceEffects { get; } = [];
		public SpellPersonalWardEffect Ward { get; }
		public MagicSpellParent Parent { get; }
		public MagicSpell WardSpell { get; }
		public Mock<IFutureProg>? Prog { get; }
		public Fixture(int grade, bool prog = false)
		{
			Mock.Get(F.Actor.Object.Location).SetupGet(a => a.Gameworld).Returns(F.World.Object);
			Target.SetupGet(a => a.Id).Returns(300); Target.SetupGet(a => a.InstanceId).Returns(300);
			Target.SetupGet(a => a.Gameworld).Returns(F.World.Object); Target.SetupGet(a => a.Location).Returns(F.Actor.Object.Location);
			Target.SetupGet(a => a.Body).Returns(Mock.Of<IBody>(a => a.BasePlanarPresence == PlanarPresenceDefinition.DefaultMaterial(1)));
			Target.SetupGet(a => a.Effects).Returns(TargetEffects); F.Actor.SetupGet(a => a.Effects).Returns(SourceEffects);
			Target.Setup(a => a.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(TargetEffects.Add);
			Target.Setup(a => a.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => TargetEffects.Add(effect));
			Target.Setup(a => a.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { if (fire) effect.RemovalEffect(); TargetEffects.Remove(effect); });
			F.Actor.Setup(a => a.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(SourceEffects.Add);
			F.Actor.Setup(a => a.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => SourceEffects.Add(effect));
			Target.Setup(a => a.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns(() => TargetEffects.OfType<IMagicInterdictionEffect>());
			var attack = Load(F, ArmageddonRetainedUtilityContent.Create(new(9, 10, 11))[0].Content, 1, grade: 1);
			F.Spells[0] = attack;
			WardSpell = Load(F, ArmageddonTurnElementsContent.Create(1).Content, 2, grade);
			F.Spells.Add(WardSpell);
			Parent = new MagicSpellParent(Target.Object, WardSpell, F.Actor.Object, SpellPower.ExtremelyWeak);
			if (prog)
			{
				Prog = new(); Prog.Setup(a => a.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			}
			Ward = new SpellPersonalWardEffect(Target.Object, Parent, F.School, MagicInterdictionMode.Reflect,
				MagicInterdictionCoverage.Incoming, false, Prog?.Object, true);
			Parent.AddSpellEffect(Ward); TargetEffects.Add(Parent); TargetEffects.Add(Ward);
		}
		public MagicInterdictionResult? Query(IMagicSchool? school = null, bool allowReflection = true) =>
			MagicInterdictionHelper.GetInterdiction(F.Actor.Object, Target.Object, school ?? F.School, allowReflection);
	}

	private static MagicSpell Load(MagicCastingFixture f, ArmageddonUtilitySpellContent content, long id, int grade)
	{
		f.Expressions.Clear(); var cost = content.CostRow(id); cost.Id = 1; var duration = content.DurationRow(id); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object)); f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var model = content.SpellRow(1, 1, 1); model.Id = id; model.EffectDurationExpressionId = 2;
		model.Definition = content.BuildDefinition(10, 1, 0).ToString();
		var spell = new MagicSpell(model, f.World.Object);
		return id == 1 ? spell : spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal);
	}
	private sealed class TestClock(DateTime time) : TimeProvider
	{
		private DateTime _time = time;
		public override DateTimeOffset GetUtcNow() => new(_time);
		public void Advance(TimeSpan span) => _time += span;
	}
}
