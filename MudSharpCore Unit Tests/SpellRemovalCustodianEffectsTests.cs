#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellRemovalCustodianEffectsTests
{
	private sealed class Fixture
	{
		internal readonly List<IEffect> Effects = [];
		internal readonly Mock<IPerceivable> Owner = new();
		internal readonly MagicSpellParent Parent;
		internal Fixture()
		{
			Owner.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
			Owner.SetupGet(x => x.Effects).Returns(() => Effects);
			Parent = new MagicSpellParent(Owner.Object, Mock.Of<IMagicSpell>(), null!);
		}
		internal void Add(IMagicSpellEffect child)
		{
			if (!Effects.Contains(Parent)) Effects.Add(Parent);
			Parent.AddSpellEffect(child); Effects.Add(child);
		}
		internal bool Admitted => GameItem.SpellRemovalEffectsArePassive(Owner.Object);
	}

	[TestMethod]
	public void RemovalCustodian_NativeMetadata_AdmitsAndPreservesExactPlayerHistory()
	{
		var actor = new Mock<MudSharp.Character.ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		var hints = new NewPlayerHintsShown(actor.Object) { LastHintShown = DateTime.UnixEpoch };
		hints.ShownHintIds.UnionWith([17, 23]);
		var branching = new IncreasedBranchChance(actor.Object);
		var skill = Mock.Of<MudSharp.Body.Traits.Subtypes.ISkillDefinition>(x => x.Id == 31);
		var knowledge = Mock.Of<MudSharp.RPG.Knowledge.IKnowledge>(x => x.Id == 41 && x.LearnerSessionsRequired == 3);
		branching.UseSkill(skill); branching.UseSkill(skill);
		branching.KnowledgeLesson(knowledge, 2.5);
		IEffect[] effects = [new AdminSight(actor.Object), new Immwalk(actor.Object), new AdminTelepathy(actor.Object), hints, branching];
		actor.SetupGet(x => x.Effects).Returns(effects);
		Assert.IsTrue(GameItem.SpellRemovalEffectsArePassive(actor.Object));
		CollectionAssert.AreEquivalent(new long[] { 17, 23 }, hints.ShownHintIds.ToArray());
		Assert.AreEqual(DateTime.UnixEpoch, hints.LastHintShown);
		Assert.AreEqual(2, branching.GetAttemptsForSkill(skill));
		Assert.AreEqual(2.5, branching.GetKnowledges().Single().Lessons);
		actor.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
		foreach (var effect in effects)
		{
			effect.ApplicabilityProg = Mock.Of<IFutureProg>();
			Assert.IsFalse(GameItem.SpellRemovalEffectsArePassive(actor.Object));
			effect.ApplicabilityProg = null;
		}
		actor.SetupGet(x => x.Effects).Returns([new AdminSight(Mock.Of<IPerceivable>(x => x.Gameworld == actor.Object.Gameworld))]);
		Assert.IsFalse(GameItem.SpellRemovalEffectsArePassive(actor.Object));
		actor.SetupGet(x => x.Effects).Returns([new DerivedAdminSight(actor.Object)]);
		Assert.IsFalse(GameItem.SpellRemovalEffectsArePassive(actor.Object));
	}
	private sealed class DerivedAdminSight(IPerceivable owner) : AdminSight(owner);

	[TestMethod]
	public void RemovalCustodian_NoEffects_AdmitsOrdinaryHolder()
	{
		Assert.IsTrue(new Fixture().Admitted);
	}

	[DataTestMethod]
	[DataRow("sense")]
	[DataRow("invisible")]
	[DataRow("ethereal")]
	[DataRow("infrared")]
	public void RemovalCustodian_NativePassiveDetection_AdmitsWithoutRemovingEffects(string type)
	{
		var f = new Fixture();
		IMagicSpellEffect effect = type switch
		{
			"sense" => new SpellDetectMagickEffect(f.Owner.Object, f.Parent),
			"invisible" => new SpellDetectInvisibleEffect(f.Owner.Object, f.Parent),
			"ethereal" => new SpellDetectEtherealEffect(f.Owner.Object, f.Parent),
			_ => new SpellInfravisionEffect(f.Owner.Object, f.Parent)
		};
		f.Add(effect);
		Assert.IsTrue(f.Admitted); Assert.AreEqual(2, f.Effects.Count);
		Assert.AreSame(effect, f.Parent.SpellEffects.Single());
		f.Owner.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("unknown")]
	[DataRow("unsafe-sibling")]
	[DataRow("missing-wrapper")]
	[DataRow("missing-child")]
	[DataRow("foreign-owner")]
	[DataRow("foreign-parent")]
	[DataRow("applicability")]
	[DataRow("empty-wrapper")]
	[DataRow("derived-detection")]
	public void RemovalCustodian_UnsupportedOrIncompleteEffectGraph_RetainsRemovalGuard(string reason)
	{
		var f = new Fixture();
		var detection = new SpellDetectMagickEffect(f.Owner.Object, f.Parent);
		f.Add(detection);
		switch (reason)
		{
			case "unknown": f.Effects.Add(Mock.Of<IEffect>()); break;
			case "unsafe-sibling": f.Add(new SpellSleepEffect(f.Owner.Object, f.Parent)); break;
			case "missing-wrapper": f.Effects.Remove(f.Parent); break;
			case "missing-child": f.Effects.Remove(detection); break;
			case "foreign-owner": f.Add(new SpellDetectMagickEffect(Mock.Of<IPerceivable>(x => x.Gameworld == f.Owner.Object.Gameworld), f.Parent)); break;
			case "foreign-parent": detection.ParentEffect = Mock.Of<IMagicSpellEffectParent>(); break;
			case "applicability": detection.ApplicabilityProg = Mock.Of<IFutureProg>(); break;
			case "empty-wrapper": f = new Fixture(); f.Effects.Add(f.Parent); break;
			case "derived-detection": f.Add(new DerivedDetection(f.Owner.Object, f.Parent)); break;
		}
		Assert.IsFalse(f.Admitted);
	}

	private sealed class DerivedDetection(IPerceivable owner, IMagicSpellEffectParent parent)
		: SpellDetectMagickEffect(owner, parent);
}
