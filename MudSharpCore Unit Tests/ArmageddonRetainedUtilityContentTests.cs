#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonRetainedUtilityContentTests
{
	private static ArmageddonRetainedUtilityBindings Bindings => new(9, 10, 11);

	[TestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void AllDefinitions_GradeLifetimeCostAndFactoryRoundTripAreExact(int grade)
	{
		foreach (var entry in ArmageddonRetainedUtilityContent.Create(Bindings))
		{
			var f = new MagicCastingFixture(); var spell = Load(f, entry.Content);
			var effect = spell.SpellEffects.Single();
			Assert.IsTrue(effect.IsCompatibleWithTrigger(spell.Trigger));
			Assert.IsTrue(XNode.DeepEquals(effect.SaveToXml(), effect.Clone().SaveToXml()));
			var reloaded = new MagicSpell(spell.SnapshotModel(), f.World.Object);
			Assert.IsTrue(XNode.DeepEquals(effect.SaveToXml(), reloaded.SpellEffects.Single().SaveToXml()));
			var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyStrong, Difficulty.Normal);
			Assert.AreEqual((entry.Content.Key.EndsWith("turn_the_elements") ? 30.0 : 60.0) * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object));
			Assert.AreEqual(entry.Content.Key.EndsWith("voice_of_remains") ? 33.0 : 7.0, entry.Content.MinimumEnergy);
			Assert.AreEqual(entry.Content.MinimumEnergy * grade, new TraitExpression(entry.Content.CostRow(1), f.World.Object)
				.EvaluateWith(f.Actor.Object, values: [("grade", grade)]));
			Assert.IsTrue(entry.Content.SpellRow(1, 1, 1).AppliedEffectsAreExclusive);
			Assert.IsFalse(entry.Content.SpellRow(1, 1, 1).ScrollInscriptionAllowed);
		}
	}

	[TestMethod]
	[DataRow(1, SpellPower.ExtremelyStrong)]
	[DataRow(7, SpellPower.ExtremelyWeak)]
	public void Trail_NativeTrackingHookAndColdChildUseSelectedGrade(int grade, SpellPower power)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, ArmageddonRetainedUtilityContent.Create(Bindings)[0].Content);
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, power, Difficulty.Normal);
		var child = Apply(f, copy);
		Assert.IsInstanceOfType(child, typeof(SpellTrackMarkEffect));
		var hook = (ITrackIntensityEffect)child;
		Assert.AreEqual(1.0 + 0.25 * grade, hook.VisualTrackIntensityMultiplier);
		Assert.AreEqual(1.0, hook.OlfactoryTrackIntensityMultiplier);
		Assert.AreEqual(0.0, hook.VisualTrackIntensityBonus);
		Assert.AreEqual(0.0, hook.OlfactoryTrackIntensityBonus);
		SpellTrackMarkEffect.InitialiseEffectType();
		var loaded = (ITrackIntensityEffect)Effect.LoadEffect(((Effect)child).SaveToXml(new()), f.Actor.Object);
		Assert.AreEqual(hook.VisualTrackIntensityMultiplier, loaded.VisualTrackIntensityMultiplier);
		Assert.AreEqual(hook.AdditionalTrackCircumstances, loaded.AdditionalTrackCircumstances);
		var row = spell.SnapshotModel(); var xml = XElement.Parse(row.Definition);
		xml.Element("ControlledPower")!.Element("ScalarBindings")!.RemoveNodes(); row.Definition = xml.ToString();
		Assert.AreEqual(1.0, ((TrackMarkEffect)new MagicSpell(row, f.World.Object).CastingCopy(f.Actor.Object,
			f.Traits[0], grade, power, Difficulty.Normal).SpellEffects.Single()).VisualTrackIntensityMultiplier);
	}

	[TestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void ReflectionCandidate_NativeLoopFenceExistsButCandidateIsExcludedFromCanonicalBatch(int grade)
	{
		Assert.IsFalse(ArmageddonRetainedUtilityContent.Create(Bindings).Any(x => x.Content.Key == "arm.spell.turn_the_elements"));
		var f = new MagicCastingFixture(); var spell = Load(f, ArmageddonRetainedUtilityContent.CreateReflectionCandidate(1).Content);
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal);
		var child = (SpellPersonalWardEffect)Apply(f, copy);
		Assert.AreSame(f.School, child.School); Assert.AreEqual(MagicInterdictionMode.Reflect, child.Mode);
		Assert.AreEqual(MagicInterdictionCoverage.Incoming, child.Coverage); Assert.IsFalse(child.IncludesSubschools);
		Assert.IsTrue(child.ShouldInterdict(f.Actor.Object, f.School));
		Assert.IsFalse(child.ShouldInterdict(f.Actor.Object, Mock.Of<IMagicSchool>()));
		SpellPersonalWardEffect.InitialiseEffectType();
		var loaded = (SpellPersonalWardEffect)Effect.LoadEffect(child.SaveToXml(new()), f.Actor.Object);
		Assert.AreSame(f.School, loaded.School); Assert.AreEqual(MagicInterdictionMode.Reflect, loaded.Mode);
		Assert.AreEqual(MagicInterdictionCoverage.Incoming, loaded.Coverage);
		f.Actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([child]);
		var source = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		Assert.AreEqual(MagicInterdictionMode.Reflect,
			MagicInterdictionHelper.GetInterdiction(source.Object, f.Actor.Object, f.School, true)!.Mode);
		Assert.AreEqual(MagicInterdictionMode.Fail,
			MagicInterdictionHelper.GetInterdiction(source.Object, f.Actor.Object, f.School, false)!.Mode,
			"The native second-reflection fence converts another ward to rejection, preventing loops.");
	}

	[TestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void Identify_RecipientRetainedPermissionAndBlankInformationColdReload(int grade)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, ArmageddonRetainedUtilityContent.Create(Bindings)[1].Content);
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal);
		var child = (SpellIdentifyEffect)Apply(f, copy);
		Assert.AreSame(f.Actor.Object, child.Owner); Assert.AreEqual(9L, child.IdentifyProgId);
		Assert.AreEqual("An authored permitted aura.", child.GetLookText(f.Actor.Object));
		var hidden = new Mock<ICharacter>(); hidden.SetupGet(x => x.Id).Returns(999);
		hidden.SetupGet(x => x.GetObject).Returns(hidden.Object);
		hidden.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(999));
		Assert.IsNull(child.GetLookText(hidden.Object));
		SpellIdentifyEffect.InitialiseEffectType();
		var loaded = (SpellIdentifyEffect)Effect.LoadEffect(child.SaveToXml(new()), f.Actor.Object);
		Assert.AreEqual("An authored permitted aura.", loaded.GetLookText(f.Actor.Object));
		Assert.AreEqual(child.CasterInstanceId, loaded.CasterInstanceId);
	}

	[TestMethod]
	[DataRow(1)]
	[DataRow(7)]
	public void Echo_ActualLanguageOutputUsesAuthorisedSpeakerAndNeverDispatchesCommands(int grade)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, ArmageddonRetainedUtilityContent.Create(Bindings)[2].Content);
		var proxy = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		proxy.SetupGet(x => x.Id).Returns(200); proxy.SetupGet(x => x.InstanceId).Returns(200);
		proxy.SetupGet(x => x.Gameworld).Returns(f.World.Object); proxy.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
		proxy.SetupGet(x => x.Corpse).Returns((MudSharp.GameItems.Interfaces.ICorpse)null!);
		Mock.Get(proxy.Object.OutputHandler).SetupGet(x => x.Perceiver).Returns(proxy.Object);
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Characters).Returns([proxy.Object]);
		Mock.Get(f.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns([proxy.Object]);
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Normal);
		var child = (SpellReciteProxyEffect)copy.SpellEffects.Single().GetOrApplyEffect(f.Actor.Object, proxy.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, new MagicSpellParent(proxy.Object, copy, f.Actor.Object, SpellPower.ExtremelyWeak), [])!;
		Assert.AreEqual(f.Actor.Object.InstanceId, child.LinkedInstanceId);
		SpellReciteProxyEffect.InitialiseEffectType();
		var loaded = (SpellReciteProxyEffect)Effect.LoadEffect(child.SaveToXml(new()), proxy.Object);
		Assert.AreEqual(child.LinkedInstanceId, loaded.LinkedInstanceId);
		var language = new Mock<ILanguage> { DefaultValue = DefaultValue.Mock }; var accent = Mock.Of<IAccent>();
		LanguageOutput? output = null;
		Mock.Get(proxy.Object.OutputHandler).Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<IOutput, bool, bool>((value, _, _) => output = value as LanguageOutput);
		Assert.IsFalse(loaded.HandleEvent(EventType.CharacterSpeaksWitness, f.Actor.Object, null!, AudioVolume.Decent,
			language.Object, accent, "authorised words"));
		Assert.IsNotNull(output); Assert.AreSame(language.Object, output.LanguageInfo.Language);
		Assert.AreSame(f.Actor.Object, output.LanguageInfo.Origin); Assert.AreSame(proxy.Object, output.LanguageInfo.Proxy);
		Assert.AreEqual("Authorised words", output.LanguageInfo.RawText);
		output = null;
		loaded.HandleEvent(EventType.CharacterSpeaksWitness, proxy.Object, null!, AudioVolume.Decent, language.Object, accent, "recursive words");
		Assert.IsNull(output);
		proxy.Verify(x => x.ExecuteCommand(It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void Voice_NativeRefusesNonCorpseAndColdModelRetainsExactOriginalBodyOwnership()
	{
		var f = new MagicCastingFixture(); var spell = Load(f, ArmageddonRetainedUtilityContent.Create(Bindings)[3].Content);
		Assert.IsNull(spell.SpellEffects.Single().GetOrApplyEffect(f.Actor.Object, Mock.Of<IGameItem>(), OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []));
		SpellDeadSpeakEffect.InitialiseEffectType();
		var item = Mock.Of<IGameItem>(x => x.Gameworld == f.World.Object);
		var parent = new MagicSpellParent(item, spell, f.Actor.Object, SpellPower.ExtremelyWeak);
		var model = new SpellDeadSpeakEffect(item, parent, 100, 100, 300, 400, 500, 600, 700, 0, 1, 100, 100, 1.0,
			CharacterInstancePersistencePolicy.TemporaryEffectBound, "", "", "", "@ recite|recites");
		var loaded = (SpellDeadSpeakEffect)Effect.LoadEffect(model.SaveToXml(new()), item);
		Assert.AreEqual(300L, loaded.CorpseItemId); Assert.AreEqual(500L, loaded.OriginalBodyId);
		Assert.AreEqual(600L, loaded.AnimatedInstanceId); Assert.AreEqual(700L, loaded.OriginalLocationId);
		Assert.AreEqual(CharacterInstancePersistencePolicy.TemporaryEffectBound, loaded.PersistencePolicy);
	}

	[TestMethod]
	public void Bindings_RejectMissingTypedDependenciesAndTrailInfinityBeforePayment()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonRetainedUtilityContent.CreateReflectionCandidate(0));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonRetainedUtilityContent.Create(Bindings with { IdentifyTextProgId = 0 }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonRetainedUtilityContent.Create(Bindings with { ReciteLinkProgId = -1 }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonRetainedUtilityContent.Create(Bindings with { DeadSpeakLinkProgId = 0 }));
		var f = new MagicCastingFixture(); f.Acquire(); var content = ArmageddonRetainedUtilityContent.Create(Bindings)[0].Content;
		var row = Load(f, content).SnapshotModel(); var xml = XElement.Parse(row.Definition);
		xml.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single().Value = "1/0";
		row.Definition = xml.ToString(); f.Spells[0] = new MagicSpell(row, f.World.Object);
		f.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		var result = f.Service.Cast(f.Intent(1, false) with { Targets = "self" });
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status); StringAssert.Contains(result.Message, "target[0]");
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls);
	}

	private static IMagicSpellEffect Apply(MagicCastingFixture f, MagicSpell spell) => spell.SpellEffects.Single().GetOrApplyEffect(
		f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak,
		new MagicSpellParent(f.Actor.Object, spell, f.Actor.Object, SpellPower.ExtremelyWeak), [])!;

	private static MagicSpell Load(MagicCastingFixture f, ArmageddonUtilitySpellContent content)
	{
		FutureProgTestBootstrap.EnsureInitialised(); Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.RouteDefinition).Returns((RouteRoomDefinition)null!);
		f.Actor.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(100));
		f.World.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>())).Returns((ICharacter)null!);
		var progs = new All<IFutureProg>(); foreach (var old in f.World.Object.FutureProgs) progs.Add(old);
		foreach (var spec in new[] { (9L, ProgVariableTypes.Text, "if (@target.id == 999)\nreturn \"\"\nend if\nreturn \"An authored permitted aura.\"", "target"),
			(10L, ProgVariableTypes.Character, "return @caster", "caster"), (11L, ProgVariableTypes.Character, "return @caster", "caster") })
		{
			var row = new MudSharp.Models.FutureProg { Id = spec.Item1, FunctionName = "retained" + spec.Item1,
				FunctionText = spec.Item3, ReturnTypeDefinition = spec.Item2.ToStorageString() };
			row.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = spec.Item4, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			var prog = new FutureProg(row, f.World.Object); Assert.IsTrue(prog.Compile(), prog.CompileError); progs.Add(prog);
		}
		f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.Expressions.Clear(); var cost = content.CostRow(1); cost.Id = 1; var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object)); f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var model = content.SpellRow(1, 1, 1); model.Id = 1; model.EffectDurationExpressionId = 2;
		model.Definition = content.BuildDefinition(10, 1, 0).ToString(); return new MagicSpell(model, f.World.Object);
	}
}
