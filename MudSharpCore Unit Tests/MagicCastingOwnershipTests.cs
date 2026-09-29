using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Generators;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Effects;
using MudSharp.Body;
using MudSharp.Communication.Language;
using ConcreteCharacter = MudSharp.Character.Character;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingOwnershipTests
{
	[TestMethod]
	public void SecondarySkillAcquisition_UsesOneNativeTraitAndPropagatesLinkedLanguages()
	{
		var f = new MagicCastingFixture();
		var primary = Holder.Create(f.World.Object, 100); var secondary = Holder.Create(f.World.Object, 101); secondary.Canonical = primary;
		Set(primary, "_secondaryInstances", new List<ICharacterInstance> { secondary });
		var spoken = new Mock<ILanguage>(); spoken.SetupGet(x => x.LinkedTrait).Returns(f.Traits[0]); spoken.SetupGet(x => x.Accents).Returns([]);
		var signed = new Mock<ISignedLanguage>(); signed.SetupGet(x => x.LinkedTrait).Returns(f.Traits[0]);
		f.World.SetupGet(x => x.Languages).Returns(MagicCastingFixture.Collection<ILanguage>(() => [spoken.Object]));
		f.World.SetupGet(x => x.SignedLanguages).Returns(MagicCastingFixture.Collection<ISignedLanguage>(() => [signed.Object]));
		Mock.Get(f.Traits[0]).Setup(x => x.NewTrait(primary, 10)).Returns(f.NativeSkill.Object);
		f.NativeSkill.SetupGet(x => x.Definition).Returns(f.Traits[0]);
		Assert.IsTrue(secondary.AddTrait(f.Traits[0], 10));
		Assert.AreSame(f.NativeSkill.Object, primary.GetTrait(f.Traits[0]));
		Assert.AreSame(primary.GetTrait(f.Traits[0]), secondary.GetTrait(f.Traits[0]));
		Assert.IsTrue(primary.Languages.Contains(spoken.Object) && secondary.Languages.Contains(spoken.Object));
		Assert.IsTrue(primary.SignedLanguages.Contains(signed.Object) && secondary.SignedLanguages.Contains(signed.Object));
	}

	[TestMethod]
	public void NativeImprovementScope_SuppressesOnlyActingCastCheck_AndRestoresOtherChecks()
	{
		var f = new MagicCastingFixture();
		f.Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 33, Expression = "variable" }, f.World.Object));
		var template = new MudSharp.Models.CheckTemplate { Name = "ARM02 native check", ImproveTraits = true, CanBranchIfTraitMissing = true };
		foreach (var d in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)d });
		var model = new MudSharp.Models.Check { Type = (int)CheckType.CastSpellCheck, TraitExpressionId = 33, CheckTemplate = template,
			MaximumDifficultyForImprovement = (int)Difficulty.Impossible };
		var check = new NativeCheck(model, f.World.Object);
		using (new CheckImprovementScope(f.Actor.Object))
		{
			check.Resolve(f.Actor.Object, f.Traits[0]); Assert.AreEqual(0, f.SkillUses);
			model.Type = (int)CheckType.TraitBranchCheck;
			new NativeCheck(model, f.World.Object).Resolve(f.Actor.Object, f.Traits[0]); Assert.AreEqual(1, f.SkillUses);
		}
		check.Resolve(f.Actor.Object, f.Traits[0]); Assert.AreEqual(2, f.SkillUses);
	}

	[TestMethod]
	public void ConfiguredGeneration_UsesAuthoredSleepPolicy_AndPreservesIndependentLegacyGenerator()
	{
		var f = new MagicCastingFixture();
		var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
		var generator = new LinearTimeBasedGenerator(new MudSharp.Models.MagicGenerator { Id = 20, Name = "sleep allowed",
			Definition = "<Definition><WhichResource>11</WhichResource><AmountPerMinute>2</AmountPerMinute><ConsciousOnly>false</ConsciousOnly></Definition>" }, f.World.Object);
		Set(f.Earth, "_resourceRegenerators", new List<IMagicResourceRegenerator> { generator });
		actor.ReconcileCastingResources(); Set(actor, "_state", CharacterState.Sleeping);
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(2.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsTrue(generator.BuildingCommand(f.Actor.Object, new StringStack("conscious")));
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(2.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Set(actor, "_state", CharacterState.Awake);
		var legacy = new Mock<IMagicCapability>(); legacy.SetupGet(x => x.Regenerators).Returns([generator]);
		actor.Available.Clear(); actor.Available.Add(legacy.Object); actor.ReconcileCastingResources();
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(4.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.AreEqual(1, actor.MagicResourceGenerators.Count());
	}

	[TestMethod]
	public void GenerationFilter_DoesNotCaptureOtherIdentities_AndNpcSecondaryOwnsNoDuplicatePool()
	{
		var f = new MagicCastingFixture();
		var primary = Holder.Create(f.World.Object, 100); primary.Player = false;
		var secondary = Holder.Create(f.World.Object, 101); secondary.Player = false; secondary.Canonical = primary;
		Set(primary, "_secondaryInstances", new List<ICharacterInstance> { secondary });
		secondary.Available.Add(f.Earth);
		var other = Holder.Create(f.World.Object, 200);
		foreach (var resource in f.Resources) Mock.Get(resource).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
		var generator = new Mock<IMagicResourceRegenerator>(); generator.SetupGet(x => x.GeneratedResources).Returns([f.Resources[1], f.Resources[0]]);
		generator.Setup(x => x.GetOnMinuteDelegate(It.IsAny<IHaveMagicResource>())).Returns<IHaveMagicResource>(recipient => () =>
		{
			other.AddResource(f.Resources[0], 7); recipient.AddResource(f.Resources[1], 2); recipient.AddResource(f.Resources[0], 9);
		});
		Set(f.Earth, "_resourceRegenerators", new List<IMagicResourceRegenerator> { generator.Object });
		primary.ReconcileCastingResources(); primary.ReconcileCastingResources();
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(7.0, other.MagicResourceAmounts[f.Resources[0]]);
		Assert.AreEqual(2.0, primary.MagicResourceAmounts[f.Resources[1]]);
		Assert.AreEqual(2.0, secondary.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsFalse(secondary.MagicResourceAmounts.ContainsKey(f.Resources[0]));
		Assert.AreEqual(1, primary.MagicResourceGenerators.Count());
	}

	private sealed class NativeCheck(MudSharp.Models.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		public CheckOutcome Resolve(ICharacter actor, ITraitDefinition trait) => HandleStandardCheck(actor, actor, Outcome.Pass, Difficulty.Normal, trait);
	}

	private sealed class Holder : ConcreteCharacter
	{
		private Holder() : base(null!, null!, true) { }
		public List<IMagicCapability> Available = null!;
		public ICharacterIdentity? Canonical;
		public bool Player = true;
		public override ICharacterIdentity Identity => Canonical ?? this;
		public override bool IsPlayerCharacter => Player;
		public static Holder Create(IFuturemud world, long id)
		{
			var actor = (Holder)RuntimeHelpers.GetUninitializedObject(typeof(Holder)); actor.Available = []; actor.Player = true;
			Set(actor, "<Gameworld>k__BackingField", world); Set(actor, "_id", id); Set(actor, "_noSave", true);
			Set(actor, "_state", CharacterState.Awake); Set(actor, "_positionState", MudSharp.Body.Position.PositionStates.PositionStanding.Instance);
			Set(actor, "<Body>k__BackingField", new Mock<IBody> { DefaultValue = DefaultValue.Mock }.Object);
			Set(actor, "<EffectHandler>k__BackingField", new EffectHandler(actor));
			var merit = new Mock<IMagicCapabilityMerit>(); merit.Setup(x => x.Applies(actor)).Returns(true);
			merit.SetupGet(x => x.Capabilities).Returns(() => actor.Available);
			Set(actor, "_merits", new List<IMerit> { merit.Object });
			Set(actor, "_secondaryInstances", new List<ICharacterInstance>());
			Set(actor, "_characterTraits", new List<ITrait>());
			Set(actor, "_languages", new List<ILanguage>()); Set(actor, "_signedLanguages", new List<ISignedLanguage>());
			Set(actor, "_acquisitionAccents", new Dictionary<ILanguage, IAccent>());
			Set(actor, "_preferredAccents", new Dictionary<ILanguage, IAccent>());
			Set(actor, "_accents", new Dictionary<IAccent, Difficulty>());
			Set(actor, "_magicResourceAmounts", new DoubleCounter<IMagicResource>());
			Set(actor, "_magicResourceGenerators", new List<IMagicResourceRegenerator>());
			Set(actor, "_generatorDelegateDictionary", new Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate>());
			Set(actor, "_castingGenerators", new Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate>());
			return actor;
		}
	}

	private static void Set(object target, string field, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) is { } member) { member.SetValue(target, value); return; }
		throw new MissingFieldException(field);
	}
}
