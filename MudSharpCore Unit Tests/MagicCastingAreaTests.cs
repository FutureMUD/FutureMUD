using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public partial class MagicCastingAreaTests
{
	private sealed class AreaFixture
	{
		public MagicCastingFixture F { get; }
		public List<ICharacter> Members { get; } = [];
		public List<(ICharacter Target, double Amount)> Damage { get; } = [];
		public Queue<int> Picks { get; } = [];
		public Action<ICharacter>? AfterDamage { get; set; }
		public int AreaDraws { get; private set; }
		public int MasterySamples { get; private set; }
		public MagicCastingService Service { get; private set; } = null!;
		public AreaFixture(string policy = "earthquake", MagicCastingFixture? fixture = null)
		{
			F = fixture ?? new(); F.Acquire();
			var cell = Mock.Get(F.Actor.Object.Location);
			cell.SetupGet(x => x.Id).Returns(1);
			cell.SetupGet(x => x.RouteDefinition).Returns(() => null!);
			cell.SetupGet(x => x.Characters).Returns(() => Members);
			cell.SetupGet(x => x.Perceivables).Returns(() => Members.Cast<IPerceivable>());
			cell.SetupGet(x => x.GameItems).Returns([]);
			cell.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns<RoomLayer>(layer => Members.Where(x => x.RoomLayer == layer));
			F.Actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			Members.Add(F.Actor.Object); TrackDamage(F.Actor);
			Build("trigger new character"); Build("grades scalar remove target 0");
			var effects = (List<IMagicSpellEffectTemplate>)F.Spell.SpellEffects; effects.Clear();
			effects.Add(SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='damage'><DamageType>0</DamageType><DamageExpression>grade*4</DamageExpression><Bodypart>0</Bodypart><Limb>-1</Limb></Effect>"), F.Spell));
			Build("grades area fixture " + policy); Restart();
		}
		public void Build(string command) => Assert.IsTrue(F.Spell.BuildingCommand(F.Actor.Object, new StringStack(command)), string.Join(" | ", F.Messages));
		public void Restart()
		{
			Service = new(F.World.Object, F.Store, () => F.Now, () => { MasterySamples++; return 0.1; },
				x => F.Checkpoint?.Invoke(x), () => { }, areaRandom: bound => { AreaDraws++; return Picks.Count > 0 ? Picks.Dequeue() : 0; });
			F.World.SetupGet(x => x.MagicCasting).Returns(Service);
		}
		public MagicCastingIntent Intent(Guid? id = null) => new(F.Actor.Object, F.Earth.Id, F.Spell.Id, 3, true, "here", MagicCastingMode.Area, OriginId: id);
		public Mock<ICharacter> Target(long id = 101, long? identity = null, long? bodyId = null, long? instance = null)
		{
			var target = new Mock<ICharacter>() { DefaultValue = DefaultValue.Mock };
			target.SetupGet(x => x.Id).Returns(id); target.SetupGet(x => x.InstanceId).Returns(instance ?? id);
			target.SetupGet(x => x.Identity).Returns(() => null!);
			if (identity is { } owner)
			{
				var canonical = new Mock<ICharacterIdentity>(); canonical.SetupGet(x => x.Id).Returns(owner);
				target.SetupGet(x => x.Identity).Returns(canonical.Object);
			}
			target.SetupGet(x => x.GetObject).Returns(target.Object); target.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			target.SetupGet(x => x.Location).Returns(F.Actor.Object.Location); target.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			var body = Mock.Get(target.Object.Body); body.SetupGet(x => x.Id).Returns(bodyId ?? id + 200);
			body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([]);
			Members.Add(target.Object); TrackDamage(target); return target;
		}
		private void TrackDamage(Mock<ICharacter> target)
		{
			target.SetupGet(x => x.Wounds).Returns([]);
			target.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(d =>
			{
				Damage.Add((target.Object, d.DamageAmount)); AfterDamage?.Invoke(target.Object);
				var wound = new Mock<IWound>(); wound.SetupGet(x => x.CurrentDamage).Returns(d.DamageAmount);
				return [wound.Object];
			});
		}
		public void Ward(Mock<ICharacter> target, MagicInterdictionMode mode = MagicInterdictionMode.Fail)
		{
			var ward = new Mock<IMagicInterdictionEffect>(); ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
			ward.SetupGet(x => x.Mode).Returns(mode); ward.Setup(x => x.ShouldInterdict(F.Actor.Object, F.School)).Returns(true);
			target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		}
	}

	[TestMethod]
	public void Earthquake_CasterThird_AlliesFull_DeduplicatesPhysicalBody_OnePaymentAndProgress()
	{
		var a = new AreaFixture(); var ally = a.Target(); var stranger = a.Target(102);
		a.F.Actor.Setup(x => x.IsAlly(ally.Object)).Returns(true); a.Members.Add(ally.Object);
		var quote = a.Service.Quote(a.Intent()); Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(3, quote.Invocation!.Area!.Candidates.Count); Assert.AreEqual(0, a.AreaDraws);
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		CollectionAssert.AreEqual(new[] { 4.0, 12.0, 12.0 }, a.Damage.Select(x => x.Amount).ToArray());
		Assert.AreEqual(1, a.F.Rolls); Assert.AreEqual(1, a.F.SkillUses); Assert.AreEqual(1, a.MasterySamples);
		Assert.AreEqual(77.5, a.F.Balances[a.F.Resources[1]]);
		a.F.Actor.Verify(x => x.UseResource(a.F.Resources[1], 22.5), Times.Once);
		var receipt = XElement.Parse(a.F.Store.Operations[result.OperationId!.Value].Definition).Element("Area")!;
		Assert.AreEqual(3, receipt.Elements("Candidate").Count()); Assert.AreEqual(3, receipt.Elements("Application").Count());
		Assert.AreEqual(1.0 / 3, (double)receipt.Elements("Application").First().Attribute("damage")!);
		Assert.AreSame(stranger.Object, a.Damage.Last().Target);
	}

	[TestMethod]
	public void ChainLightning_RepeatedRandomCasterQuarter_QuotesDoNotDraw_RestartDoesNotReplay()
	{
		var a = new AreaFixture("chainlightning"); var ally = a.Target();
		a.F.Actor.Setup(x => x.IsAlly(ally.Object)).Returns(true);
		a.Picks.Enqueue(0); a.Picks.Enqueue(1); a.Picks.Enqueue(0);
		for (var i = 0; i < 3; i++) Assert.IsTrue(a.Service.Quote(a.Intent()).Allowed);
		Assert.AreEqual(0, a.AreaDraws);
		var id = Guid.NewGuid(); var result = a.Service.Cast(a.Intent(id));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		CollectionAssert.AreEqual(new[] { 3.0, 12.0, 3.0 }, a.Damage.Select(x => x.Amount).ToArray());
		Assert.AreEqual(3, a.AreaDraws); Assert.AreEqual(1, a.F.Rolls); Assert.AreEqual(1, a.MasterySamples);
		var receipt = XElement.Parse(a.F.Store.Operations[id].Definition).Element("Area")!;
		CollectionAssert.AreEqual(new long[] { 200, 301, 200 }, receipt.Elements("Application").Select(x => (long)x.Attribute("body")!).ToArray());
		a.Restart(); Assert.AreEqual(MagicCastingStatus.Refused, a.Service.Cast(a.Intent(id)).Status);
		Assert.AreEqual(3, a.AreaDraws); Assert.AreEqual(3, a.Damage.Count);
	}

	[TestMethod]
	public void RoomFireball_ExcludesCaster_IncludesAlly_PreservesDistinctBodiesOfOneIdentity()
	{
		var a = new AreaFixture("roomfireball"); var first = a.Target(101, identity: 500); var second = a.Target(102, identity: 500);
		a.F.Actor.Setup(x => x.IsAlly(first.Object)).Returns(true); a.Members.Add(second.Object);
		Assert.AreEqual(MagicCastingStatus.Succeeded, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(2, a.Damage.Count); Assert.IsTrue(a.Damage.All(x => x.Amount == 12 && !ReferenceEquals(x.Target, a.F.Actor.Object)));
	}

	[TestMethod]
	public void ExplicitCanonicalSelection_CollapsesIdentity_ExplicitAllyExclusionDoesNotProtectOthers()
	{
		var a = new AreaFixture("roomfireball"); var first = a.Target(101, identity: 500); a.Target(102, identity: 500);
		var ally = a.Target(103); a.F.Actor.Setup(x => x.IsAlly(ally.Object)).Returns(true);
		a.Build("grades area identity CanonicalCharacter"); a.Build("grades area include allies false");
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, a.Damage.Count); Assert.AreSame(first.Object, a.Damage[0].Target);
	}

	[TestMethod]
	public void SourceExclusions_StaffFlyingLayerAndPlane_AreIndividual_NotWholeCastFailures()
	{
		var a = new AreaFixture(); a.Target();
		a.Target(102).Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		a.Target(103).SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		a.Target(104).SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		Mock.Get(a.Target(105).Object.Body).SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2));
		Assert.AreEqual(MagicCastingStatus.Succeeded, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(2, a.Damage.Count); Assert.AreEqual(1, a.F.Rolls);
	}

	[TestMethod]
	public void NativeAndAreaFiltersBothApply_ProtectedTargetCannotSilentlyRejoinSnapshot()
	{
		var a = new AreaFixture(); var rejected = a.Target(); var accepted = a.Target(102);
		var native = new Mock<IFutureProg>(); native.SetupGet(x => x.Id).Returns(8); native.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		native.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		native.Setup(x => x.Compile()).Returns(true);
		native.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny,It.IsAny<object[]>()))
			.Returns(new TargetPolicyExecutor((out object result,object[] args)=>{result=!ReferenceEquals(args[0],rejected.Object);return true;}));
		native.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns<object[]>(args => !ReferenceEquals(args[0], rejected.Object));
		var area = new Mock<IFutureProg>(); area.SetupGet(x => x.Id).Returns(9); area.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		area.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		area.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns<object[]>(args => !ReferenceEquals(args[0], a.F.Actor.Object));
		a.F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { native.Object, area.Object }));
		a.Build("trigger set filterprog 8"); a.Build("grades area filter 9");
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, a.Damage.Count); Assert.AreSame(accepted.Object, a.Damage[0].Target);
	}

	[TestMethod]
	public void PaidResolution_MutationsCannotAddTargets_AndRemovedOrMovedTargetsAreSkipped()
	{
		var a = new AreaFixture(); var removed = a.Target(); var moved = a.Target(102); var unaffected = a.Target(103);
		var added = a.Target(104); a.Members.RemoveAll(x => ReferenceEquals(x, added.Object));
		a.AfterDamage = target =>
		{
			if (!ReferenceEquals(target, a.F.Actor.Object)) return;
			a.Members.RemoveAll(x => ReferenceEquals(x, removed.Object)); a.Members.Add(added.Object);
			moved.SetupGet(x => x.Location).Returns(new Mock<ICell>().Object);
		};
		Assert.AreEqual(MagicCastingStatus.Succeeded, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(2, a.Damage.Count); Assert.AreSame(unaffected.Object, a.Damage.Last().Target);
		Assert.AreEqual(1, a.F.Rolls); Assert.AreEqual(1, a.MasterySamples);
	}

	[DataTestMethod]
	[DataRow("body")][DataRow("layer")][DataRow("ground")][DataRow("plane")][DataRow("ally")]
	public void RepeatedHit_RevalidatesEligibilityAfterFirstHit(string mutation)
	{
		var a = new AreaFixture("chainlightning"); a.Build("grades area include caster false"); var target = a.Target();
		if (mutation == "ground") a.Build("grades area grounded true");
		if (mutation == "ally") a.Build("grades area include allies false");
		a.AfterDamage = _ =>
		{
			switch (mutation)
			{
				case "body": target.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object); break;
				case "layer": target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir); break;
				case "ground": target.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance); break;
				case "plane": Mock.Get(target.Object.Body).SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2)); break;
				case "ally": a.F.Actor.Setup(x => x.IsAlly(target.Object)).Returns(true); break;
			}
		};
		Assert.AreEqual(MagicCastingStatus.Succeeded, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(1, a.Damage.Count); Assert.AreEqual(3, a.AreaDraws); Assert.AreEqual(1, a.MasterySamples);
	}

	[TestMethod]
	public void AllTargetsDisappearAfterPayment_FailsWithCostAndLockout_NoMastery()
	{
		var a = new AreaFixture(); a.F.Checkpoint = stage => { if (stage == "Committed") a.Members.Clear(); };
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(77.5, a.F.Balances[a.F.Resources[1]]); Assert.AreEqual(0, a.Damage.Count); Assert.AreEqual(0, a.MasterySamples);
		a.F.Actor.Verify(x => x.AddEffect(It.IsAny<MagicSpellLockout>(), TimeSpan.FromSeconds(5)), Times.Once);
	}

	[TestMethod]
	public void WardsAndPerTargetResistanceContinueLaterTargets_NoAreaReflectionRecursion()
	{
		var a = new AreaFixture("roomfireball"); var warded = a.Target(); var resistant = a.Target(102); var passed = a.Target(103);
		a.Ward(warded, MagicInterdictionMode.Reflect);
		a.Build("resist 3 normal");
		var checks = new Mock<ICheck>();
		checks.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((target, _, _, _, _, _, _) =>
				Enum.GetValues<Difficulty>().ToDictionary(d => d, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck,
					ReferenceEquals(target, resistant.Object) ? Outcome.MajorPass : Outcome.MajorFail)));
		a.F.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(checks.Object);
		Assert.AreEqual(MagicCastingStatus.Succeeded, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(1, a.Damage.Count); Assert.AreSame(passed.Object, a.Damage[0].Target);
		checks.Verify(x => x.CheckAgainstAllDifficulties(warded.Object, It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Never);
		Assert.AreEqual(1, a.F.Rolls); Assert.AreEqual(1, a.MasterySamples);
	}

	[TestMethod]
	public void ResistanceCallbackMovesTarget_NoEffectDeliveredAfterResistance()
	{
		var a = new AreaFixture("roomfireball"); var target = a.Target(); a.Build("resist 3 normal");
		var checks = new Mock<ICheck>();
		checks.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(() => { target.SetupGet(x => x.Location).Returns(new Mock<ICell>().Object); return Enum.GetValues<Difficulty>().ToDictionary(d => d,
				_ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorFail)); });
		a.F.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(checks.Object);
		Assert.AreEqual(MagicCastingStatus.Failed, a.Service.Cast(a.Intent()).Status); Assert.AreEqual(0, a.Damage.Count);
		Assert.AreEqual(77.5, a.F.Balances[a.F.Resources[1]]); Assert.AreEqual(0, a.MasterySamples);
	}

	[DataTestMethod]
	[DataRow("here tail")][DataRow("self")][DataRow("party")][DataRow("")]
	public void AreaSelectorMustBeExplicitAndComplete(string target)
	{
		var a = new AreaFixture(); Assert.IsFalse(a.Service.Quote(a.Intent() with { Targets = target }).Allowed);
		Assert.AreEqual(MagicCastingStatus.Refused, a.Service.Cast(a.Intent() with { Targets = target }).Status);
		Assert.AreEqual(100.0, a.F.Balances[a.F.Resources[1]]); Assert.AreEqual(0, a.F.Rolls);
	}

	[TestMethod]
	public void EmptyAndTargetOverflowRefuseBeforePayment_PreCommitMutationAlsoRefuses()
	{
		var a = new AreaFixture("roomfireball"); Assert.AreEqual(MagicCastingStatus.Refused, a.Service.Cast(a.Intent()).Status);
		a.Target(); a.Target(102); a.Build("grades area targets 1");
		Assert.AreEqual(MagicCastingStatus.Refused, a.Service.Cast(a.Intent()).Status);
		a.Build("grades area targets 2"); a.F.Checkpoint = stage => { if (stage == "BeforePayment") a.Members.RemoveAt(a.Members.Count - 1); };
		Assert.AreEqual(MagicCastingStatus.Refused, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(100.0, a.F.Balances[a.F.Resources[1]]); Assert.AreEqual(0, a.F.Rolls); Assert.AreEqual(0, a.AreaDraws);
	}

	[TestMethod]
	public void PhysicalInputOverflowIsBounded_EvenIfAllAreDuplicates()
	{
		var a = new AreaFixture(); for (var i = 0; i < 512; i++) a.Members.Add(a.F.Actor.Object);
		var quote = a.Service.Quote(a.Intent()); Assert.IsFalse(quote.Allowed); StringAssert.Contains(quote.Reason, "512");
		Assert.AreEqual(0, a.F.Rolls);
	}

	[TestMethod]
	public void RandomDistinct_IsExplicitAlternative_AndInvalidDrawCannotPay()
	{
		var a = new AreaFixture("chainlightning"); a.Target(); a.Build("grades area selection RandomDistinct");
		Assert.AreEqual(MagicCastingStatus.Succeeded, a.Service.Cast(a.Intent()).Status);
		Assert.AreEqual(2, a.Damage.Count); Assert.AreEqual(2, a.AreaDraws);
		var invalid = new AreaFixture("chainlightning"); invalid.Picks.Enqueue(999);
		Assert.AreEqual(MagicCastingStatus.Refused, invalid.Service.Cast(invalid.Intent()).Status);
		Assert.AreEqual(100.0, invalid.F.Balances[invalid.F.Resources[1]]); Assert.AreEqual(0, invalid.F.Rolls);
	}

	[TestMethod]
	public void AreaDoesNotTransformNativeRoomOrExitEffects_AndXmlClonesPolicyWithoutAliasing()
	{
		var a = new AreaFixture();
		var copy = new MagicSpell(a.F.Spell.SnapshotModel(), a.F.World.Object);
		Assert.AreEqual(1.0 / 3, copy.GradeProfile!.Area!.CasterDamageMultiplier);
		a.Build("grades area damage caster 0.5"); Assert.AreEqual(1.0 / 3, copy.GradeProfile.Area.CasterDamageMultiplier);
		a.Build("trigger new room"); Assert.IsFalse(a.F.Spell.ReadyForGame);
		Assert.IsFalse(a.F.Spell.BuildingCommand(a.F.Actor.Object, new StringStack("grades area fixture earthquake")));
		Assert.IsTrue(a.F.Spell.GradeConfigurationErrors().Any(x => x.Contains("native character trigger")));
	}

	[TestMethod]
	public void NamedFormulaAndOriginalSpeechUseAreaMode_QuietRequiresOwnTotalMethodModifiers()
	{
		var speech = new MagicCastingIncantationTests.SpeechFixture(); var a = new AreaFixture("earthquake", speech.F);
		a.F.Acquire(3);
		Assert.IsFalse(a.Service.Quote(a.Intent() with { Grade = 3, Overreach = false, Method = "Whisper" }).Allowed);
		a.Build("grades incantation method Whisper 99 9"); a.Build("grades area method Whisper 1.25 2");
		var quote = a.Service.Quote(a.Intent() with { Overreach = false, Method = "Whisper" }); Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(18.75, quote.Invocation!.Costs.Single().Amount); Assert.AreEqual(Difficulty.VeryHard, quote.Invocation.Difficulty);
		MagicModule.MagicGeneric(a.F.Actor.Object, "earth quiet area \"Stone Skin\" grade 3 on here via Earth");
		Assert.AreEqual(1, a.F.Rolls); Assert.AreEqual(1, speech.Utterances.Count); Assert.IsTrue(speech.Utterances[0].RawText.StartsWith("area ", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual(81.25, a.F.Balances[a.F.Resources[1]]);
		var f = new MagicCastingIncantationTests.SpeechFixture(); var original = new AreaFixture("earthquake", f.F); original.F.Acquire(3);
		f.Speak("area kral stone on here via Earth"); Assert.AreEqual(1, original.F.Rolls); Assert.AreEqual(1, f.Utterances.Count);
		Assert.AreEqual(85.0, original.F.Balances[original.F.Resources[1]]);
	}

	[TestMethod]
	public void TrueReturningFilterCannotMoveOrReplaceCapturedTargetAfterPayment()
	{
		var a = new AreaFixture("roomfireball"); var target = a.Target(); var paid = false;
		var filter = new Mock<IFutureProg>(); filter.SetupGet(x => x.Id).Returns(9); filter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		filter.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		filter.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns<object[]>(args =>
		{
			if (paid && ReferenceEquals(args[0], target.Object))
			{
				var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(900);
				body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
				target.SetupGet(x => x.Body).Returns(body.Object);
			}
			return true;
		});
		a.F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { filter.Object }));
		a.Build("grades area filter 9"); a.F.Checkpoint = stage => { if (stage == "Committed") paid = true; };
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(0, a.Damage.Count); Assert.AreEqual(1, a.F.Rolls); Assert.AreEqual(77.5, a.F.Balances[a.F.Resources[1]]);
		Assert.AreEqual(0, a.MasterySamples);
	}

	[TestMethod]
	public void PartialPersistentApplication_StillRegistersParentLifetimeAndRemovesAppliedChild()
	{
		var a = new AreaFixture("roomfireball"); var target = a.Target();
		var effects = (List<IMagicSpellEffectTemplate>)a.F.Spell.SpellEffects;
		effects.Insert(0, SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='boost' trait='3' bonus='1' context='0'/>"), a.F.Spell));
		var children = new List<IEffect>(); MagicSpellParent? parent = null; TimeSpan? duration = null;
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
		{
			children.Add(effect); target.SetupGet(x => x.Location).Returns(new Mock<ICell>().Object);
		});
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, delay) =>
		{
			parent = effect as MagicSpellParent; duration = delay;
		});
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, a.Damage.Count); Assert.AreEqual(1, children.Count); Assert.IsInstanceOfType(children.Single(), typeof(SpellTraitBoostEffect));
		Assert.IsNotNull(parent); Assert.IsTrue(duration > TimeSpan.Zero); Assert.AreSame(children[0], parent.SpellEffects.Single());
		Assert.IsNotNull(parent.SaveToXml(new Dictionary<IEffect, TimeSpan>()).Descendants("Children").Single());
		parent.RemovalEffect(); target.Verify(x => x.RemoveEffect(children.Single(), true), Times.Once);
	}

	[TestMethod]
	public void ExceptionAfterPersistentChild_QuarantinesButPreservesParentLifetimeAndCleanup()
	{
		var a = new AreaFixture("roomfireball"); var target = a.Target();
		var effects = (List<IMagicSpellEffectTemplate>)a.F.Spell.SpellEffects; effects.Clear();
		effects.Add(SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='boost' trait='3' bonus='1' context='0'/>"), a.F.Spell));
		effects.Add(SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='damage'><DamageType>0</DamageType><DamageExpression>grade-10</DamageExpression><Bodypart>0</Bodypart><Limb>-1</Limb></Effect>"), a.F.Spell));
		MagicSpellParent? parent = null;
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, duration) =>
		{
			Assert.IsTrue(duration > TimeSpan.Zero); parent = effect as MagicSpellParent;
		});
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		StringAssert.Contains(result.Message, "finite and non-negative"); Assert.AreEqual(77.5, a.F.Balances[a.F.Resources[1]]); Assert.AreEqual(1, a.F.Rolls);
		Assert.IsNotNull(parent); Assert.AreEqual(1, parent.SpellEffects.Count()); Assert.AreEqual(0, a.Damage.Count); Assert.AreEqual(0, a.MasterySamples);
		var child = parent.SpellEffects.Single(); parent.RemovalEffect(); target.Verify(x => x.RemoveEffect(child, true), Times.Once);
		Assert.AreEqual("NeedsReview", a.F.Store.Operations[result.OperationId!.Value].Stage);
	}

	[TestMethod]
	public void ExplicitAreaProfile_DoesNotAlterOrdinarySelectedCharacterDamage()
	{
		var a = new AreaFixture(); a.Target();
		a.F.Actor.Setup(x => x.TargetActorOrCorpse("self")).Returns(a.F.Actor.Object);
		var quote = a.Service.Quote(a.F.Intent());
		Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.IsNull(quote.Invocation!.Area);
		var result = a.Service.Cast(a.F.Intent()); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, a.Damage.Count); Assert.AreSame(a.F.Actor.Object, a.Damage[0].Target); Assert.AreEqual(12.0, a.Damage[0].Amount);
	}

	[TestMethod]
	public void WholeRoomScopeRefusesSpatialRoute_ImmediateScopeIsSeparatelyAuthored()
	{
		var a = new AreaFixture(); var cell = Mock.Get(a.F.Actor.Object.Location);
		cell.SetupGet(x => x.RouteDefinition).Returns(new Mock<IRouteCellDefinition>().Object);
		var quote = a.Service.Quote(a.Intent()); Assert.IsFalse(quote.Allowed); StringAssert.Contains(quote.Reason, "RouteCell");
		cell.SetupGet(x => x.RouteDefinition).Returns(() => null!); a.Build("grades area scope ImmediateCharacters"); a.Target();
		var result = a.Service.Cast(a.Intent()); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(2, a.Damage.Count); Assert.AreEqual(SpellAreaScope.ImmediateCharacters, a.Service.Quote(a.Intent() with { Overreach = false }).Invocation!.Area!.Scope);
	}
}
