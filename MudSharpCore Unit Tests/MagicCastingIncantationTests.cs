using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Vancian;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingIncantationTests
{
	internal sealed class SpeechFixture
	{
		public MagicCastingFixture F { get; } = new();
		public Mock<ILanguage> Language { get; } = new();
		public Mock<IAccent> Accent { get; } = new();
		public List<SpokenLanguageInfo> Utterances { get; } = [];
		public Action<SpokenLanguageInfo>? OnOutput { get; set; }
		public double SynthFunction { get; set; } = 1;
		public SpeechFixture()
		{
			var model = new Mock<ILanguageDifficultyModel>();
			model.Setup(x => x.RateDifficulty(It.IsAny<ExplodedString>())).Returns(Difficulty.Automatic);
			Language.SetupGet(x => x.Id).Returns(1); Language.SetupGet(x => x.Name).Returns("ARM Common");
			Language.SetupGet(x => x.Model).Returns(model.Object); Language.SetupGet(x => x.LinkedTrait).Returns(F.Traits[0]);
			Language.SetupGet(x => x.UnknownLanguageSpokenDescription).Returns("an unknown tongue");
			Accent.SetupGet(x => x.Id).Returns(1); Accent.SetupGet(x => x.Language).Returns(Language.Object);
			F.World.SetupGet(x => x.Languages).Returns(MagicCastingFixture.Collection(() => new[] { Language.Object }));
			F.World.SetupGet(x => x.Accents).Returns(MagicCastingFixture.Collection(() => new[] { Accent.Object }));
			F.World.Setup(x => x.GetStaticInt("MaximumSayLength")).Returns(4096);
			F.Actor.SetupGet(x => x.CurrentLanguage).Returns(Language.Object); F.Actor.SetupGet(x => x.CurrentAccent).Returns(Accent.Object);
			F.Actor.SetupGet(x => x.Languages).Returns(new[] { Language.Object }); F.Actor.SetupGet(x => x.Accents).Returns(new[] { Accent.Object });
			F.Actor.Setup(x => x.IsSelf(F.Actor.Object)).Returns(true);
			F.Actor.SetupGet(x => x.Corpse).Returns(() => null!);
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object); F.Body.SetupGet(x => x.CurrentLanguage).Returns(() => F.Actor.Object.CurrentLanguage);
			F.Body.SetupGet(x => x.CurrentAccent).Returns(Accent.Object); F.Body.SetupGet(x => x.Location).Returns(F.Actor.Object.Location);
			F.Body.SetupGet(x => x.OutputHandler).Returns(F.Actor.Object.OutputHandler);
			F.Body.SetupGet(x => x.Communications).Returns(RobotCommunicationStrategy.Instance);
			F.Body.Setup(x => x.OrganFunction<SpeechSynthesizer>()).Returns(() => SynthFunction);
			F.Body.Setup(x => x.Say(It.IsAny<IPerceivable>(), It.IsAny<string>(), It.IsAny<IEmote>()))
				.Callback<IPerceivable, string, IEmote>((target, message, emote) => RobotCommunicationStrategy.Instance.Say(F.Body.Object, target, message, emote));
			F.Body.Setup(x => x.Whisper(It.IsAny<IPerceivable>(), It.IsAny<string>(), It.IsAny<IEmote>()))
				.Callback<IPerceivable, string, IEmote>((target, message, emote) => RobotCommunicationStrategy.Instance.Whisper(F.Body.Object, target, message, emote));
			var output = Mock.Get(F.Actor.Object.OutputHandler); output.SetupGet(x => x.Perceiver).Returns(F.Actor.Object);
			output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<IOutput, bool, bool>((o, _, _) => { if (o is LanguageOutput { LanguageInfo: SpokenLanguageInfo s }) { Utterances.Add(s); OnOutput?.Invoke(s); } }).Returns(true);
			Mock.Get(F.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns(new[] { F.Actor.Object });
			Mock.Get(F.Actor.Object.Location).SetupGet(x => x.RouteDefinition).Returns(() => null!);
			Mock.Get(F.Actor.Object.Location).SetupGet(x => x.Id).Returns(1);
			var speak = new Mock<ICheck>();
			speak.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<MudSharp.Body.Traits.ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(CheckOutcome.SimpleOutcome(CheckType.SpokenLanguageSpeakCheck, Outcome.Pass));
			F.World.Setup(x => x.GetCheck(CheckType.SpokenLanguageSpeakCheck)).Returns(speak.Object);
			Assert.IsTrue(F.Spell.BuildingCommand(F.Actor.Object, new StringStack("grades incantation fixture 1 fm-near fm-earth fm-protect fm-shape stone")), string.Join(";", F.Messages));
			F.Acquire(3);
		}
		public MagicCastingIntent Intent(string method = "Say", Guid? origin = null) => new(F.Actor.Object, F.Earth.Id, F.Spell.Id, 3, false, "self", Method: method, OriginId: origin);
		public const string Formula = "kral fm-near fm-earth fm-protect fm-shape on self via Earth";
		public void Speak(string text = Formula, string method = "Say", Guid? origin = null)
		{
			using var input = MagicSpeechContext.PlayerInput(F.Actor.Object, origin);
			using var command = MagicSpeechContext.CharacterCommand(F.Actor.Object);
			PlayerSpeechCommand(text, method);
		}
		public void PlayerSpeechCommand(string text, string method = "Say") =>
			typeof(CommunicationsModule).GetMethod(method, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [F.Actor.Object, method.ToLowerInvariant() + " " + text]);
	}

	[DataTestMethod]
	[DataRow(1, "wek")][DataRow(2, "yuqa")][DataRow(3, "kral")][DataRow(4, "een")]
	[DataRow(5, "pav")][DataRow(6, "sul")][DataRow(7, "mon")]
	public void PowerWords_ExplicitExternalGradeCoordinate(int grade, string word)
	{
		Assert.AreEqual(word, ArmageddonPowerWords.ForGrade(grade)); Assert.AreEqual(grade, ArmageddonPowerWords.Grade(word.ToUpperInvariant()));
		var s = new SpeechFixture(); s.F.Acquire(7);
		var result = s.F.Service.CastFormula(s.F.Actor.Object, word + " stone on self via Earth");
		Assert.AreEqual(MagicCastingStatus.Succeeded, result!.Status, result.Message);
		var expected = new[] { SpellPower.ExtremelyWeak, SpellPower.VeryWeak, SpellPower.Weak, SpellPower.Standard,
			SpellPower.Strong, SpellPower.VeryStrong, SpellPower.ExtremelyStrong };
		Assert.AreEqual(expected[grade - 1], s.F.Service.Quote(s.Intent() with { Grade = grade }).Invocation!.Power);
		Assert.AreEqual(100.0 - 5 * grade, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(1, s.F.Rolls);
	}

	[TestMethod]
	public void FullGrammar_All120CategoryOrders_AndAlias_ResolveTheSameGradeAndPayment()
	{
		var words = new[] { "kral", "fm-near", "fm-earth", "fm-protect", "fm-shape" };
		var orders = Permute(words).ToArray(); Assert.AreEqual(120, orders.Length);
		foreach (var order in orders)
		{
			var s = new SpeechFixture(); var result = s.F.Service.CastFormula(s.F.Actor.Object, string.Join(" ", order) + " on self via Earth");
			Assert.AreEqual(MagicCastingStatus.Succeeded, result!.Status, string.Join(" ", order) + ": " + result.Message);
			Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(1, s.F.Rolls); Assert.AreEqual(1, s.Utterances.Count);
		}
		static IEnumerable<string[]> Permute(string[] entries) => entries.Length == 0 ? new[] { Array.Empty<string>() } :
			entries.SelectMany((x, i) => Permute(entries.Where((_, j) => i != j).ToArray()).Select(rest => new[] { x }.Concat(rest).ToArray()));
	}

	[DataTestMethod]
	[DataRow("kral unknown on self via Earth")][DataRow("kral fm-near fm-earth fm-protect fm-protect on self via Earth")]
	[DataRow("kral yuqa stone on self via Earth")][DataRow("kral stone")][DataRow("kral stone on self area via Earth")]
	[DataRow("kral stone on self via Earth extra")][DataRow("kral stone on self")]
	[DataRow("kral stone on nonexistent via Earth")][DataRow("kral stone on self please do not cast via Earth")]
	public void InvalidOrAmbiguousRouteFormula_NoCastingMutations(string formula)
	{
		var s = new SpeechFixture(); var writes = s.F.Store.Writes;
		var result = s.F.Service.CastFormula(s.F.Actor.Object, formula);
		Assert.AreEqual(MagicCastingStatus.Refused, result!.Status); Assert.AreEqual(writes, s.F.Store.Writes);
		Assert.AreEqual(100.0, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(0, s.F.Rolls); Assert.AreEqual(0, s.Utterances.Count);
	}

	[DataTestMethod]
	[DataRow("character")][DataRow("item")]
	public void CompleteTargetText_NativeCharacterAndItemParsersRemainUsable(string trigger)
	{
		var s = new SpeechFixture(); var item = new Mock<IGameItem>();
		s.F.Actor.Setup(x => x.TargetActorOrCorpse("Bob the guard")).Returns(s.F.Actor.Object);
		s.F.Actor.Setup(x => x.TargetItem("red stone")).Returns(item.Object);
		Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("trigger new " + trigger)));
		var text = trigger == "character" ? "Bob the guard" : "red stone";
		var resolved = SpellTargetCapture.Resolve(s.F.Actor.Object, s.F.Spell, SpellPower.Strong, new StringStack(text), true);
		Assert.IsNotNull(resolved); Assert.AreSame(trigger == "character" ? (IPerceivable)s.F.Actor.Object : item.Object, resolved.Target);
		Assert.AreEqual(0, s.F.Rolls); Assert.AreEqual(100.0, s.F.Balances[s.F.Resources[1]]);
	}

	[TestMethod]
	public void CompleteCompositeTarget_RejectsTrailingWordsWithoutRejectingValidExit()
	{
		var s = new SpeechFixture(); var exit = new Mock<IRoomExit>();
		s.F.Actor.Setup(x => x.TargetActorOrCorpse("Bob")).Returns(s.F.Actor.Object);
		Mock.Get(s.F.Actor.Object.Location).Setup(x => x.GetExitKeyword("north", s.F.Actor.Object)).Returns(exit.Object);
		Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("trigger new characterexit")));
		Assert.IsTrue(s.F.Spell.Trigger.BuildingCommand(s.F.Actor.Object, new StringStack("self")));
		Assert.IsNotNull(SpellTargetCapture.Resolve(s.F.Actor.Object, s.F.Spell, SpellPower.Strong, new StringStack("Bob north"), true));
		Assert.IsNull(SpellTargetCapture.Resolve(s.F.Actor.Object, s.F.Spell, SpellPower.Strong, new StringStack("Bob north extra"), true));
		Assert.AreEqual(0, s.F.Rolls); Assert.AreEqual(100.0, s.F.Balances[s.F.Resources[1]]);
	}

	[TestMethod]
	public void UnmappedTargetParser_IsAnExplicitIncantationReadinessError()
	{
		var s = new SpeechFixture(); Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("trigger new progcharacter")));
		Assert.IsTrue(s.F.Spell.GradeConfigurationErrors().Any(x => x.Contains("no complete target adapter")));
		Assert.IsFalse(s.F.Service.Quote(s.Intent()).Allowed); Assert.AreEqual(0, s.F.Rolls);
	}

	[TestMethod]
	public void Vocabulary_DoesNotGrantKnowledgeOrSelectBetweenAmbiguousSpells()
	{
		var s = new SpeechFixture(); s.F.Store.Acquired.Clear();
		Assert.AreEqual(MagicCastingStatus.Refused, s.F.Service.CastFormula(s.F.Actor.Object, SpeechFixture.Formula)!.Status);
		Assert.AreEqual(0, s.F.Store.Acquired.Count); Assert.AreEqual(0, s.F.Rolls);
		s.F.Acquire(3); var other = s.F.NewSpell(2, "Other Stone", "<Effect type='boost' trait='3' bonus='0' context='0' />");
		Assert.IsTrue(other.BuildingCommand(s.F.Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(other.BuildingCommand(s.F.Actor.Object, new StringStack("grades incantation fixture 1 fm-near fm-earth fm-protect fm-shape stone")));
		Assert.IsTrue(s.F.Service.CastFormula(s.F.Actor.Object, SpeechFixture.Formula)!.Message.Contains("ambiguous"));
		Assert.AreEqual(0, s.F.Rolls);
	}

	[TestMethod]
	public void NativeSpeech_MultiwordCapabilitySurvivesNativeQuoteRemoval()
	{
		var s = new SpeechFixture(); Assert.IsTrue(s.F.Earth.BuildingCommand(s.F.Actor.Object, new StringStack("name Earth Adept")));
		s.Speak("kral stone on self via \"Earth Adept\"");
		Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[1]], string.Join(" | ", s.F.Messages));
		Assert.AreEqual(1, s.F.Rolls); Assert.AreEqual(1, s.Utterances.Count);
		Assert.IsTrue(s.Utterances[0].RawText.Contains("Earth Adept", StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void NativeSpeech_QuotedCompositeTargetMatchesNamedAndFormulaAdapters()
	{
		foreach (var adapter in new[] { "named", "formula", "speech" })
		{
			var s = new SpeechFixture(); var exit = new Mock<IRoomExit>();
			s.F.Actor.Setup(x => x.TargetActorOrCorpse("Bob the guard")).Returns(s.F.Actor.Object);
			Mock.Get(s.F.Actor.Object.Location).Setup(x => x.GetExitKeyword("north", s.F.Actor.Object)).Returns(exit.Object);
			Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("trigger new characterexit")));
			Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("trigger set self")));
			Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("grades scalar remove target 0")));
			Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("effect remove 1")));
			Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("effect add personaltagward")));
			Assert.IsTrue(s.F.Earth.BuildingCommand(s.F.Actor.Object, new StringStack("name Earth Adept")));
			if (adapter == "named") MagicModule.MagicGeneric(s.F.Actor.Object, "earth cast \"Stone Skin\" grade 3 on \"Bob the guard\" north via \"Earth Adept\"");
			else if (adapter == "formula") MagicModule.MagicGeneric(s.F.Actor.Object, "earth formula kral stone on \"Bob the guard\" north via \"Earth Adept\"");
			else s.Speak("kral stone on \"Bob the guard\" north via \"Earth Adept\"");
			Assert.AreEqual(1, s.F.Rolls, adapter + ": " + string.Join(" | ", s.F.Messages));
			Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(1, s.Utterances.Count);
		}
	}

	[DataTestMethod]
	[DataRow("named")][DataRow("formula")][DataRow("speech")][DataRow("alias")]
	public void NativeAdapters_OneLanguageAwareUtterance_OnePaidOperation(string adapter)
	{
		var s = new SpeechFixture();
		switch (adapter)
		{
			case "named": MagicModule.MagicGeneric(s.F.Actor.Object, "earth cast \"Stone Skin\" grade 3 on self via Earth"); break;
			case "formula": MagicModule.MagicGeneric(s.F.Actor.Object, "earth formula " + SpeechFixture.Formula); break;
			case "speech": s.Speak(); break;
			case "alias": s.Speak("kral stone on self via Earth"); break;
		}
		Assert.AreEqual(1, s.F.Rolls, string.Join(";", s.F.Messages)); Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[1]]);
		Assert.AreEqual(1, s.Utterances.Count); Assert.AreEqual(1, s.F.Store.Operations.Count);
		Assert.AreEqual(LanguageForm.Spoken, s.Utterances[0].Form); Assert.AreSame(s.Language.Object, s.Utterances[0].Language);
		Assert.AreEqual(adapter is "speech" or "alias" ? SpeechOriginKind.PlayerInput : SpeechOriginKind.GeneratedCasting, s.Utterances[0].Provenance.Kind);
		var op = s.F.Store.Operations.Values.Single(); Assert.AreEqual(op.Id, s.Utterances[0].Provenance.Id);
		var receipt = XElement.Parse(op.Definition).Element("Speech")!; Assert.AreEqual("Say", (string)receipt.Attribute("method")!);
		Assert.AreEqual("Completed", op.Stage);
	}

	[DataTestMethod]
	[DataRow("named")][DataRow("formula")][DataRow("speech")]
	public void Quiet_IsNativeWhisper_AndModifiersApplyOnceToDesignatedEnergy(string adapter)
	{
		var s = new SpeechFixture();
		Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("cost 12 1")));
		var quote = s.F.Service.Quote(s.Intent("Whisper")); Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(Difficulty.Hard, quote.Invocation!.Difficulty); Assert.AreEqual(AudioVolume.Quiet, quote.Invocation.Delivery!.Volume);
		if (adapter == "named") MagicModule.MagicGeneric(s.F.Actor.Object, "earth quiet \"Stone Skin\" grade 3 on self via Earth");
		else if (adapter == "formula") MagicModule.MagicGeneric(s.F.Actor.Object, "earth quiet formula " + SpeechFixture.Formula);
		else s.Speak(method: "Whisper");
		Assert.AreEqual(70.0, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[2]]);
		Assert.AreEqual(1, s.F.Rolls, string.Join(";", s.F.Messages)); Assert.AreEqual(1, s.Utterances.Count);
		Assert.AreEqual(AudioVolume.Quiet, s.Utterances[0].Volume);
	}

	[TestMethod]
	public void MethodTuning_IsAuthored_AndUnmappedCombinationsRefuse()
	{
		var s = new SpeechFixture();
		Assert.IsFalse(s.F.Service.Quote(s.Intent("Shout")).Allowed);
		Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("grades incantation method Shout 0.5 -1")));
		var quote = s.F.Service.Quote(s.Intent("Shout")); Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(Difficulty.Easy, quote.Invocation!.Difficulty); Assert.AreEqual(7.5, quote.Invocation.Costs.Single().Amount);
		Assert.AreEqual(AudioVolume.ExtremelyLoud, quote.Invocation.Delivery!.Volume);
		Assert.IsTrue(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack("grades incantation method Whisper 1.25 2")));
		quote = s.F.Service.Quote(s.Intent("Whisper")); Assert.AreEqual(18.75, quote.Invocation!.Costs.Single().Amount); Assert.AreEqual(Difficulty.VeryHard, quote.Invocation.Difficulty);
		Assert.IsFalse(s.F.Service.Quote(s.Intent("Whisper") with { Mode = MagicCastingMode.Practice, Targets = "" }).Allowed);
		Assert.AreEqual(0, s.F.Rolls); Assert.AreEqual(0, s.Utterances.Count);
	}

	[TestMethod]
	public void NativeRobotVolumeEligibility_QuietDoesNotBypassMuteSilenceOrManipulation()
	{
		var s = new SpeechFixture { SynthFunction = 0.4 };
		Assert.IsFalse(s.F.Service.Quote(s.Intent()).Allowed); Assert.IsTrue(s.F.Service.Quote(s.Intent("Whisper")).Allowed);
		s.SynthFunction = 0.2; Assert.IsFalse(s.F.Service.Quote(s.Intent("Whisper")).Allowed);
		s.SynthFunction = 1; var mute = new Mock<IMuteMerit>(); mute.Setup(x => x.Applies(s.F.Actor.Object)).Returns(true);
		s.F.Actor.SetupGet(x => x.Merits).Returns(new[] { mute.Object }); Assert.IsFalse(s.F.Service.Quote(s.Intent("Whisper")).Allowed);
		s.F.Actor.SetupGet(x => x.Merits).Returns([]);
		var parent = new MagicSpellParent(s.F.Actor.Object, s.F.Spell, s.F.Actor.Object);
		var silence = new SpellSilenceEffect(s.F.Actor.Object, parent); parent.AddSpellEffect(silence);
		s.F.Body.Setup(x => x.CombinedEffectsOfType<ISilencedEffect>()).Returns([silence]); Assert.IsFalse(s.F.Service.Quote(s.Intent("Whisper")).Allowed);
		s.F.Body.Setup(x => x.CombinedEffectsOfType<ISilencedEffect>()).Returns([]); s.F.Body.SetupGet(x => x.FunctioningFreeHands).Returns([]);
		Assert.IsFalse(s.F.Service.Quote(s.Intent("Whisper")).Allowed); Assert.AreEqual(0, s.F.Rolls);
	}

	[TestMethod]
	public void NativeGagPredicate_RefusesNamedQuietAndOriginalSpeechBeforeCosts()
	{
		var s = new SpeechFixture(); var mouth = new MouthProto(new MudSharp.Models.BodypartProto { Id = 900, Name = "mouth", Description = "mouth" }, s.F.World.Object);
		var gag = new Mock<IGameItem>(); gag.Setup(x => x.IsItemType<IGag>()).Returns(true);
		s.F.Body.SetupGet(x => x.Bodyparts).Returns([mouth]); s.F.Body.Setup(x => x.WornItemsFor(mouth)).Returns([gag.Object]);
		Assert.IsFalse(s.F.Service.Quote(s.Intent()).Allowed); Assert.IsFalse(s.F.Service.Quote(s.Intent("Whisper")).Allowed);
		s.Speak(); Assert.AreEqual(0, s.F.Rolls); Assert.AreEqual(100.0, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(0, s.Utterances.Count);
	}

	[TestMethod]
	public void OriginReceipt_DuplicateCallbackAndRestart_CannotDebitOrCheckAgain()
	{
		var s = new SpeechFixture(); var id = Guid.NewGuid(); s.Speak(origin: id);
		Assert.AreEqual(id, s.F.Store.Operations.Values.Single().Id);
		s.Speak(origin: id); s.F.Restart(); s.Speak(origin: id);
		Assert.AreEqual(1, s.F.Rolls); Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[1]]); Assert.AreEqual(1, s.F.Store.Operations.Count);
		Assert.AreEqual(3, s.Utterances.Count, "Each actual player command still speaks; replayed casting origin cannot pay again.");
	}

	[TestMethod]
	public void GeneratedOutputCallbacks_AndNestedForcedSpeech_CannotAuthorizeAnotherCast()
	{
		var s = new SpeechFixture(); var calls = 0;
		s.OnOutput = _ =>
		{
			if (++calls != 1) return;
			using var nested = MagicSpeechContext.CharacterCommand(s.F.Actor.Object);
			s.PlayerSpeechCommand(SpeechFixture.Formula);
		};
		Assert.AreEqual(MagicCastingStatus.Succeeded, s.F.Service.Cast(s.Intent()).Status);
		Assert.AreEqual(1, s.F.Rolls); Assert.AreEqual(2, s.Utterances.Count); Assert.IsNull(s.Utterances[1].Provenance);
		Assert.AreEqual(85.0, s.F.Balances[s.F.Resources[1]]);
	}

	[TestMethod]
	public void PreCaptureEligibilityCallback_CannotPromoteSpeechAuthorityToNetworkInput()
	{
		var s = new SpeechFixture(); var entered = false;
		s.F.Body.Setup(x => x.OrganFunction<SpeechSynthesizer>()).Returns(() =>
		{
			if (!entered)
			{
				entered = true;
				using var nested = MagicSpeechContext.CharacterCommand(s.F.Actor.Object);
				s.PlayerSpeechCommand(SpeechFixture.Formula);
			}
			return 1;
		});
		s.Speak(); Assert.AreEqual(1, s.F.Rolls); Assert.AreEqual(2, s.Utterances.Count);
		Assert.IsNull(s.Utterances[0].Provenance); Assert.AreEqual(SpeechOriginKind.PlayerInput, s.Utterances[1].Provenance.Kind);
	}

	[TestMethod]
	public void ScriptedAndEmoteSpeech_WithoutOriginalNativeCommand_CannotCast()
	{
		var s = new SpeechFixture(); s.PlayerSpeechCommand(SpeechFixture.Formula);
		using (MagicSpeechContext.PlayerInput(s.F.Actor.Object))
		using (MagicSpeechContext.CharacterCommand(s.F.Actor.Object))
		using (MagicSpeechContext.Suppress()) s.PlayerSpeechCommand(SpeechFixture.Formula);
		using (MagicSpeechContext.PlayerInput(s.F.Actor.Object))
		using (MagicSpeechContext.CharacterCommand(s.F.Actor.Object))
			s.F.Body.Object.Communications.Emote(s.F.Body.Object, "@ chants \"" + SpeechFixture.Formula + "\"");
		Assert.AreEqual(0, s.F.Rolls); Assert.AreEqual(100.0, s.F.Balances[s.F.Resources[1]]);
		s.Speak("ordinary chatter about a formula"); Assert.AreEqual(0, s.F.Rolls);
	}

	[TestMethod]
	public void IncantationBuilder_RoundtripAndAtomicInvalidInput_PreservesMalformedProfile()
	{
		var s = new SpeechFixture(); var before = s.F.Spell.SnapshotModel();
		foreach (var command in new[] { "grades incantation word reach kral", "grades incantation method Whisper NaN 0",
			"grades incantation fixture 1 same same same same stone", "grades incantation method Unknown 1 0", "grades incantation method Whisper 1 0 extra" })
		{
			Assert.IsFalse(s.F.Spell.BuildingCommand(s.F.Actor.Object, new StringStack(command)), command);
			Assert.AreEqual(before.Definition, s.F.Spell.SnapshotModel().Definition);
		}
		var loaded = new MagicSpell(before, s.F.World.Object); Assert.AreEqual(0, loaded.GradeConfigurationErrors().Count);
		Assert.AreEqual(before.Definition, loaded.SnapshotModel().Definition);
		var xml = XElement.Parse(before.Definition); xml.Descendants("Incantation").Single().SetAttributeValue("schema", 999);
		before.Definition = xml.ToString();
		var broken = new MagicSpell(before, s.F.World.Object);
		Assert.IsTrue(broken.GradeConfigurationErrors().Any()); Assert.AreEqual("999", (string)XElement.Parse(broken.SnapshotModel().Definition).Descendants("Incantation").Single().Attribute("schema")!);
	}

	[DataTestMethod]
	[DataRow("reach")][DataRow("element")][DataRow("sphere")][DataRow("mood")][DataRow("provenance")][DataRow("method-name")]
	public void MissingRequiredIncantationAttribute_PreservesUnreadableXmlAndReportsError(string attribute)
	{
		var s = new SpeechFixture(); var model = s.F.Spell.SnapshotModel(); var xml = XElement.Parse(model.Definition);
		var incantation = xml.Descendants("Incantation").Single();
		if (attribute == "method-name") incantation.Element("Method")!.Attribute("name")!.Remove();
		else incantation.Attribute(attribute)!.Remove();
		model.Definition = xml.ToString(); var loaded = new MagicSpell(model, s.F.World.Object);
		Assert.IsTrue(loaded.GradeConfigurationErrors().Any());
		Assert.IsTrue(XNode.DeepEquals(xml.Element("ControlledPower"), XElement.Parse(loaded.SnapshotModel().Definition).Element("ControlledPower")));
	}
}
