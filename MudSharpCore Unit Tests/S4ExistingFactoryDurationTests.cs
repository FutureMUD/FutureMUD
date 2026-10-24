#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using NativeExpression = ExpressionEngine.Expression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4ExistingFactoryDurationTests
{
	[ClassInitialize]
	public static void Initialise(TestContext _) => FutureProgTestBootstrap.EnsureInitialised();
	private sealed class Fixture
	{
		internal readonly MagicCastingFixture F = new();
		internal readonly Dictionary<int, long> Items = Enumerable.Range(1, 7).ToDictionary(x => x, x => 100L + x);
		internal Fixture()
		{
			var attribute = Mock.Of<ITraitDefinition>(x => x.Id == 4 && x.TraitType == TraitType.Attribute); F.Traits.Add(attribute);
			var eligibility = new Mock<IFutureProg>(); eligibility.SetupGet(x => x.Id).Returns(52);
			eligibility.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			eligibility.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			eligibility.Setup(x => x.Compile()).Returns(true);
			F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection<IFutureProg>(() => [eligibility.Object]));
			F.World.SetupGet(x => x.SpellOwnedItems).Returns(Mock.Of<ISpellOwnedItemService>());
			F.World.SetupGet(x => x.DefaultHooks).Returns([]);
			var prototypes = Items.Values.Select(id => WeaponPrototype(id, F.World.Object)).ToArray();
			var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
			catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => prototypes.SingleOrDefault(x => x.Id == id)!);
			F.World.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
			var hand = Mock.Of<IWield>(x => x.Id == 1 && x.Alignment == Alignment.Right);
			Mock.Get(hand).Setup(x => x.Hands(It.IsAny<IGameItem>())).Returns(1);
			F.Body.SetupGet(x => x.Handedness).Returns(Alignment.Right); F.Body.SetupGet(x => x.WieldLocs).Returns([hand]);
			F.Body.Setup(x => x.CanGet(It.IsAny<IGameItem>(), 0, ItemCanGetIgnore.None)).Returns(true);
			F.Body.Setup(x => x.CanWield(It.IsAny<IGameItem>(), hand, ItemCanWieldFlags.None)).Returns(true);
		}
		internal ArmageddonUtilitySpellContent Content(string kind) => kind switch
		{
			"fury" => ArmageddonExistingFactorySpellContent.RousedFury(new(4, 52)),
			"calm" => ArmageddonExistingFactorySpellContent.StillAnger(new(1, 52, Enumerable.Range(1, 7).ToDictionary(x => x, _ => Difficulty.Normal))),
			_ => ArmageddonExistingFactorySpellContent.StormSpear(Items)
		};
		internal MagicSpell Load(ArmageddonUtilitySpellContent content)
		{
			var costRow = content.CostRow(1); costRow.Id = 54;
			var durationRow = content.DurationRow(1); durationRow.Id = 55;
			F.Expressions.Add(new TraitExpression(costRow, F.World.Object)); F.Expressions.Add(new TraitExpression(durationRow, F.World.Object));
			var model = content.SpellRow(1, 1, 52); model.Id = 1; model.EffectDurationExpressionId = 55;
			model.Definition = content.BuildDefinition(10, 54, 0).ToString(); return new(model, F.World.Object);
		}
	}
	private static GameItemProto WeaponPrototype(long id, IFuturemud world)
	{
		var prototype = TestObjectFactory.CreateUninitialized<GameItemProto>();
		typeof(FrameworkItem).GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(prototype, id);
		typeof(GameItemProto).GetProperty(nameof(GameItemProto.Gameworld))!.SetValue(prototype, world);
		typeof(GameItemProto).GetProperty(nameof(GameItemProto.Status))!.SetValue(prototype, RevisionStatus.Current);
		var holdable = TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>();
		var weapon = TestObjectFactory.CreateUninitialized<MeleeWeaponGameItemComponentProto>();
		typeof(MeleeWeaponGameItemComponentProto).GetProperty(nameof(MeleeWeaponGameItemComponentProto.WeaponType))!.SetValue(weapon, Mock.Of<IWeaponType>());
		typeof(GameItemProto).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(prototype, new List<IGameItemComponentProto> { holdable, weapon });
		prototype.OnLoadProgs = []; return prototype;
	}
	[TestMethod]
	[DataRow("fury")] [DataRow("calm")] [DataRow("storm")]
	public void CanonicalWholeSpell_OrdinaryDurationIsZero_ReadyAndColdRowsRetainProfileTiming(string kind)
	{
		var f = new Fixture(); var content = f.Content(kind); var spell = f.Load(content);
		Assert.AreEqual("0", content.DurationRow(1).Expression);
		Assert.IsTrue(spell.ReadyForGame, spell.ReadyForGame ? "" : spell.WhyNotReadyForGame(f.F.Actor.Object));
		var saved = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(spell, [])!;
		var cold = new MagicSpell(saved, f.F.World.Object);
		Assert.IsTrue(cold.ReadyForGame, cold.ReadyForGame ? "" : cold.WhyNotReadyForGame(f.F.Actor.Object));
		Assert.AreEqual("0", cold.EffectDurationExpression.OriginalFormulaText);
		Assert.AreEqual(55L, saved.EffectDurationExpressionId);
		if (kind == "storm")
		{
			Assert.AreEqual(ArmageddonExistingFactorySpellContent.StormLifetime, ((CreateItemEffect)cold.SpellEffects.Single()).LifetimeExpression!.OriginalFormulaText);
			Assert.AreEqual(SpellLifecycleMode.TemporaryCleanup, ((CreateItemEffect)cold.SpellEffects.Single()).LifecycleMode);
		}
		else
		{
			var profile = ((SourceEmotionalEffect)cold.SpellEffects.Single()).Profile!; profile.ValidateBindings(f.F.World.Object);
			Assert.IsTrue(profile.CanonicalGradePolicy); Assert.AreEqual(60, profile.UnitSeconds); Assert.AreEqual(7, profile.CapUnits);
			spell.EffectDurationExpression = new TraitExpression("60*grade", f.F.World.Object);
			Assert.IsFalse(spell.ReadyForGame, "The old ordinary duration would violate native source-emotion readiness.");
		}
	}
	private sealed class CountingRandom(bool upper) : Random
	{
		internal int Calls;
		public override int Next(int minValue, int maxValue)
		{ Calls++; Assert.AreEqual(1, minValue); Assert.AreEqual(7, maxValue); return upper ? maxValue - 1 : minValue; }
	}
	[TestMethod]
	[DataRow(1, false)] [DataRow(1, true)] [DataRow(7, false)] [DataRow(7, true)]
	public void StormNativeAdmission_ZeroOrdinaryDurationAndCachedOwnedLifetime_DrawExactlyOnce(int grade, bool upper)
	{
		var f = new Fixture(); var source = f.Load(f.Content("storm"));
		var copy = source.CastingCopy(f.F.Actor.Object, f.F.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal, grade);
		var random = new CountingRandom(upper); using var rng = NativeExpression.PushRandom(random);
		Assert.AreEqual(0, copy.EffectDurationExpression.Evaluate(f.F.Actor.Object)); Assert.AreEqual(0, random.Calls);
		var effect = (CreateItemEffect)copy.SpellEffects.Single();
		Assert.IsTrue(effect.ValidateRecipientInvocation(f.F.Actor.Object, f.F.Actor.Object, out var error), error);
		Assert.AreEqual(1, random.Calls);
		Assert.IsTrue(effect.ValidateRecipientInvocation(f.F.Actor.Object, f.F.Actor.Object, out error), error);
		Assert.AreEqual(1, random.Calls, "Repeated native admission retains the same owned-item lifetime draw.");
		var seconds = (double?)typeof(CreateItemEffect).GetField("_preparedLifetimeSeconds", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(effect);
		Assert.AreEqual((24 + (upper ? 6 : 1)) * 90.0 * grade * 0.75, seconds);
		Assert.AreEqual(0, copy.EffectDurationExpression.Evaluate(f.F.Actor.Object)); Assert.AreEqual(1, random.Calls);
	}
	[TestMethod]
	[DataRow(1)] [DataRow(7)]
	public void CanonicalEmotion_ZeroOrdinaryDuration_ProfileOwnsSixtySecondsPerGrade(int grade)
	{
		foreach (var kind in new[] { "fury", "calm" })
		{
			var f = new Fixture(); var source = f.Load(f.Content(kind));
			var copy = source.CastingCopy(f.F.Actor.Object, f.F.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal, grade);
			Assert.AreEqual(0, copy.EffectDurationExpression.Evaluate(f.F.Actor.Object));
			var profile = ((SourceEmotionalEffect)copy.SpellEffects.Single()).Profile!;
			Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), profile.ResolveLifetime(grade, 1, SpellPower.ExtremelyWeak,
				(_, _) => throw new AssertFailedException("Canonical emotions cannot draw ordinary or source random duration.")).Duration);
		}
	}
}
