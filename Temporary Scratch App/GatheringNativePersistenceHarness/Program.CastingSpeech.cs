using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.Implementations;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Network;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;
using Db = MudSharp.Models;
using RuntimeBody = MudSharp.Body.Implementations.Body;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record SpeechReader(string Database, FixtureIds Fixture, long Spell, long Capability, long Trait,
		Guid Operation, long Language, double Balance, int Operations);

	private static int RunSpeechAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm_speech", false);
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true, vocalAnatomy: true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var world = native.World; var actor = native.Actor; var body = native.Body;
		Mock.Get(body.Race).SetupGet(x => x.CommunicationStrategy).Returns(HumanoidCommunicationStrategy.Instance);
		body.CalculateOrganFunctions(true);
		Require(body.Organs.OfType<TracheaProto>().Any() && body.Bodyparts.OfType<MouthProto>().Any() && body.Bodyparts.OfType<TongueProto>().Any() &&
			body.Communications.CanVocalise(body, AudioVolume.Decent) && body.CanHear(actor), "Concrete native vocal/hearing anatomy is not functional.");
		var language = ConfigureSpeechLanguage(native, database.ConnectionString, true);
		PrepareSpeechActor(actor, language);
		var cap = (SkillLevelBasedMagicCapability)native.Capability;
		var trait = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var spell = new MagicSpell("ARM Speech Stone", cap.School); ((All<IMagicSpell>)world.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new self", $"trait {trait.Id}", "difficulty easy", "threshold minorpass", "duration ARM02 Duration",
			$"cost {native.Resource.Id} ARM02 Cost", "prog rejuvenation_known", "castemote The fixture gestures as its ward forms.",
			"failcastemote The fixture gestures without success.", "effect add boost", "effect 1 trait ARM02 Agility", "effect 1 bonus 0",
			"grades fixture", "grades scalar add target 0 boost Bonus -grade", $"grades incantation fixture {language.Id} fm-near fm-earth fm-protect fm-shape stone" })
			Require(spell.BuildingCommand(actor, new StringStack(command)), "Speech spell builder refused: " + command);
		if (!spell.ReadyForGame) throw new InvalidOperationException(spell.WhyNotReadyForGame(actor));
		foreach (var command in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(command)), "Speech route refused: " + command);
		world.SaveManager.Flush();
		var now = new DateTime(2026, 10, 2, 12, 0, 0, DateTimeKind.Utc);
		var service = new MagicCastingService(world, clock: () => now, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		Require(service.Enrol(staff.Object, actor, cap.Id, "Native speech fixture").Allowed, "Speech enrolment failed.");
		actor.SetTraitValue(trait, 62); actor.AddTrait(world.Traits.GetByName("ARM02 Agility"), 10);
		var store = new MagicCastingStateStore(); store.Write(acquired: service.Acquisition(actor, spell.Id)! with { ControlledGrade = 3 });
		var outputs = new List<SpokenLanguageInfo>(); var outputText = new List<string>();
		var listeners = new List<NativeHarnessCharacter> { actor };
		ConfigureSpeechOutput(actor, outputs, outputText);
		Mock.Get(actor.Location).SetupGet(x => x.Characters).Returns(() => listeners);
		Mock.Get(actor.Location).Setup(x => x.LayerCharacters(It.IsAny<MudSharp.Construction.RoomLayer>())).Returns(() => listeners);
		Mock.Get(actor.Location).SetupGet(x => x.EventHandlers).Returns(() => listeners.Cast<IHandleEvents>().ToArray());
		native.WorldMock.SetupGet(x => x.Characters).Returns(new SpeechCharacterCatalogue(listeners).Items);
		ConfigureSpeechCommands(actor);
		int Checks() => Mock.Get(world.GetCheck(CheckType.CastSpellCheck)).Invocations.Count(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties));
		int Operations() { using var db = NewIndependentContext(database.ConnectionString); return db.MagicCastingOperations.Count(x => x.CharacterId == actor.Id); }
		MagicCastingIntent Intent(string method = "Say") => new(actor, cap.Id, spell.Id, 3, false, "self", Method: method);
		var formula = $"kral fm-near fm-earth fm-protect fm-shape on self via {cap.Name.DoubleQuotes()}";
		void Reset()
		{
			actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
			actor.AddResource(native.Resource, 100); outputs.Clear(); outputText.Clear(); now += TimeSpan.FromSeconds(61);
		}
		body.Say(null!, "ordinary native speech probe");
		Reset(); var beforeChecks = Checks(); var beforeOps = Operations(); var beforeEffects = actor.Effects.Count(); var beforeBalance = actor.MagicResourceAmounts[native.Resource];
		var quote = service.Quote(Intent());
		Require(quote.Allowed && outputs.Count == 0 && Checks() == beforeChecks && Operations() == beforeOps &&
			actor.Effects.Count() == beforeEffects && actor.MagicResourceAmounts[native.Resource] == beforeBalance, "Speech quote mutated gameplay or emitted words.");
		MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} cast \"{spell.Name}\" grade 3 on self via {cap.Name.DoubleQuotes()}");
		Require(Checks() == beforeChecks + 1 && Operations() == beforeOps + 1 && actor.MagicResourceAmounts[native.Resource] == 85 &&
			outputs.Count == 1 && outputs[0].Provenance.Kind == SpeechOriginKind.GeneratedCasting && outputs[0].Volume == AudioVolume.Decent &&
			actor.EffectsOfType<SpellTraitBoostEffect>().Single().Bonus == -3,
			$"Named native casting parity: checks={Checks()-beforeChecks}, operations={Operations()-beforeOps}, balance={actor.MagicResourceAmounts[native.Resource]}, utterances={outputs.Count}, messages={string.Join(" | ", outputText)}");
		Console.WriteLine("ARM-SPEECH-named=passed kral:grade3 native-language-output one-utterance one-check one-operation balance:85 ward-bonus:-3");
		Reset(); beforeChecks = Checks(); beforeOps = Operations();
		MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} formula fm-shape fm-earth kral fm-near fm-protect on self via {cap.Name.DoubleQuotes()}");
		Require(Checks() == beforeChecks + 1 && Operations() == beforeOps + 1 && outputs.Count == 1 && actor.MagicResourceAmounts[native.Resource] == 85,
			"Command full formula did not use the same pipeline.");
		Console.WriteLine("ARM-SPEECH-formula=passed unordered-five-category-command same-grade/payment/effect single-generated-utterance");
		Reset(); beforeChecks = Checks(); beforeOps = Operations();
		RunSpeechNetworkCommand(actor, "say " + formula, repeatDispatch: true);
		Require(Checks() == beforeChecks + 1 && Operations() == beforeOps + 1 && outputs.Count == 1 &&
			outputs[0].Provenance.Kind == SpeechOriginKind.PlayerInput && actor.MagicResourceAmounts[native.Resource] == 85,
			$"Network/native speech parity: checks={Checks()-beforeChecks}, operations={Operations()-beforeOps}, balance={actor.MagicResourceAmounts[native.Resource]}, utterances={outputs.Count}, messages={string.Join(" | ", outputText)}");
		var speechOp = outputs[0].Provenance.Id;
		Require(service.CastFormula(actor, formula, speech: new(speechOp, formula, "Say", AudioVolume.Decent, language.Id))!.Status == MagicCastingStatus.Refused &&
			Checks() == beforeChecks + 1 && actor.MagicResourceAmounts[native.Resource] == 85, "Repeated speech origin paid again.");
		Console.WriteLine("ARM-SPEECH-native=passed loopback-TCP queue original-utterance empty-second-dispatch duplicate-origin-refused one-check one-operation");
		Reset(); beforeChecks = Checks(); RunSpeechNetworkCommand(actor, ". kral stone on self via " + cap.Name.DoubleQuotes());
		Require(Checks() == beforeChecks + 1 && outputs.Count == 1 && actor.MagicResourceAmounts[native.Resource] == 85,
			"Native say alias and concise spell alias were not eligible.");
		Console.WriteLine("ARM-SPEECH-alias=passed native-dot-command concise-POWER/spell-token single-original-utterance");
		Reset(); beforeChecks = Checks(); MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} quiet \"{spell.Name}\" grade 3 on self via {cap.Name.DoubleQuotes()}");
		Require(Checks() == beforeChecks + 1 && outputs.Count == 1 && outputs[0].Volume == AudioVolume.Quiet && actor.MagicResourceAmounts[native.Resource] == 70 &&
			(Difficulty)Mock.Get(world.GetCheck(CheckType.CastSpellCheck)).Invocations.Last(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties)).Arguments[1] == Difficulty.Hard &&
			actor.EffectsOfType<SpellTraitBoostEffect>().Single().Bonus == -3 && outputText.Any(x => x.Contains("gestures")), "Quiet lost native whisper, modifiers or visible casting.");
		Console.WriteLine("ARM-SPEECH-quiet=passed native-whisper energy:x2 difficulty:+1 balance:70 visible-gesture ward-bonus:-3");
		Reset(); beforeChecks = Checks(); RunSpeechNetworkCommand(actor, "whisper " + formula);
		Require(Checks() == beforeChecks + 1 && outputs.Count == 1 && outputs[0].Volume == AudioVolume.Quiet &&
			outputs[0].Provenance.Kind == SpeechOriginKind.PlayerInput && actor.MagicResourceAmounts[native.Resource] == 70,
			"Real whisper did not retain quiet parity.");
		Console.WriteLine("ARM-SPEECH-whisper=passed player-origin quiet parity no-second-utterance");
		Reset(); beforeChecks = Checks(); Require(spell.BuildingCommand(actor, new StringStack("grades incantation method Whisper 1.25 2")), "Custom native volume policy refused.");
		RunSpeechNetworkCommand(actor, "whisper " + formula);
		Require(Checks() == beforeChecks + 1 && actor.MagicResourceAmounts[native.Resource] == 81.25 && outputs.Count == 1 && outputs[0].Volume == AudioVolume.Quiet &&
			(Difficulty)Mock.Get(world.GetCheck(CheckType.CastSpellCheck)).Invocations.Last(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties)).Arguments[1] == Difficulty.VeryHard,
			"Custom native method energy/difficulty did not apply exactly once.");
		Require(spell.BuildingCommand(actor, new StringStack("grades incantation method Whisper 2 1")), "Quiet fixture policy restore refused.");
		Console.WriteLine("ARM-SPEECH-configured-volume=passed native-Whisper energy:x1.25 difficulty:+2 actual-check:VeryHard balance:81.25 restored-fixture");

		Reset(); beforeChecks = Checks(); beforeOps = Operations();
		RunSpeechNetworkCommand(actor, "say kral stone on self please do not cast via " + cap.Name.DoubleQuotes());
		RunSpeechNetworkCommand(actor, "say kral unknown on self via " + cap.Name.DoubleQuotes());
		Require(Checks() == beforeChecks && Operations() == beforeOps && actor.MagicResourceAmounts[native.Resource] == 100 && outputs.Count == 2,
			"Invalid complete formula suppressed ordinary speech or paid casting.");
		Console.WriteLine("ARM-SPEECH-invalid=passed ordinary-native-utterances unknown-vocabulary target-tail-refused no-payment/check/operation");
		Reset(); beforeChecks = Checks(); actor.OutOfContextExecuteCommand("say " + formula);
		Require(outputs.Count == 1 && outputs[0].Provenance is null && Checks() == beforeChecks, "Scripted player speech acquired input authority.");
		Console.WriteLine("ARM-SPEECH-scripted=passed native-character-out-of-context speech remains ordinary no-cast");
		foreach (var effectOwner in new IPerceivable[] { actor, body })
		{
			Reset(); beforeChecks = Checks(); var parent = new MagicSpellParent(effectOwner, spell, actor); var silence = new SpellSilenceEffect(effectOwner, parent);
			parent.AddSpellEffect(silence); effectOwner.AddEffect(parent); effectOwner.AddEffect(silence);
			Require(!service.Quote(Intent("Whisper")).Allowed, "Native silence permitted quiet casting.");
			RunSpeechNetworkCommand(actor, "whisper " + formula);
			Require(Checks() == beforeChecks && actor.MagicResourceAmounts[native.Resource] == 100 && outputs.Count == 0, "Silenced native body emitted executable words.");
			effectOwner.RemoveEffect(parent, true);
			Console.WriteLine($"ARM-SPEECH-silence=passed owner:{(ReferenceEquals(effectOwner, body) ? "body" : "actor")} quiet-refused no-utterance/payment/check");
		}
		Reset(); var tongue = body.Bodyparts.OfType<TongueProto>().Single(); var damagedTongue = new BodypartExcessivelyDamaged(body, tongue);
		body.AddEffect(damagedTongue); Require(!service.Quote(Intent()).Allowed && !service.Quote(Intent("Whisper")).Allowed, "Native tongue damage permitted words.");
		body.RemoveEffect(damagedTongue); Require(service.Quote(Intent()).Allowed, "Native tongue removal did not restore eligibility.");
		Console.WriteLine("ARM-SPEECH-anatomy=passed actual-mouth/tongue/trachea native-part-ineffective both-volumes-refused restoration");
		Reset(); beforeChecks = Checks(); var restrictions = body.FunctioningFreeHands.Select(x => new BodypartExcessivelyDamaged(body, x)).ToArray();
		Require(restrictions.Length > 0, "Speech fixture has no native hands."); foreach (var restriction in restrictions) body.AddEffect(restriction);
		Require(!service.Quote(Intent("Whisper")).Allowed, "Quiet bypassed native manipulation."); RunSpeechNetworkCommand(actor, "whisper " + formula);
		Require(Checks() == beforeChecks && actor.MagicResourceAmounts[native.Resource] == 100 && outputs.Count == 1, "Handless native whisper paid a spell.");
		foreach (var restriction in restrictions) body.RemoveEffect(restriction);
		Console.WriteLine("ARM-SPEECH-manipulation=passed native-hand-restrictions ordinary-whisper remains audible no-cast/payment/check restoration");
		MuteMerit.RegisterMeritInitialiser(); var mute = MeritFactory.LoadMerit(new Db.Merit { Id = 1000000001, Name = "Native fixture mute", Type = "Mute",
			MeritType = (int)MeritType.Flaw, MeritScope = (int)MeritScope.Character, Definition = "<Definition><PermitLanguageOption>2</PermitLanguageOption></Definition>" }, world);
		Reset(); beforeChecks = Checks(); actor.SetMerits(actor.Merits.Append(mute));
		Require(!service.Quote(Intent()).Allowed && !service.Quote(Intent("Whisper")).Allowed, "Native mute merit permitted incantations.");
		RunSpeechNetworkCommand(actor, "whisper " + formula);
		Require(Checks() == beforeChecks && actor.MagicResourceAmounts[native.Resource] == 100 && outputs.Count == 0, "Native mute emitted incantation words.");
		actor.SetMerits(actor.Merits.Where(x => !ReferenceEquals(x, mute)));
		Console.WriteLine("ARM-SPEECH-mute=passed concrete-MuteMerit both-volumes-refused no-utterance/payment/check");
		Mock.Get(body.Race).SetupGet(x => x.CommunicationStrategy).Returns(RobotCommunicationStrategy.Instance);
		var synth = body.Organs.OfType<SpeechSynthesizer>().Single(); var modifier = new Mock<IOrganFunctionBonusMerit>();
		modifier.Setup(x => x.Applies(actor)).Returns(true); modifier.Setup(x => x.OrganFunctionBonuses(body)).Returns(new[] { ((IOrganProto)synth, -0.6) });
		actor.SetMerits(actor.Merits.Append(modifier.Object)); body.CalculateOrganFunctions(true);
		Require(Math.Abs(body.OrganFunction<SpeechSynthesizer>() - 0.4) < 0.000001 && !service.Quote(Intent()).Allowed && service.Quote(Intent("Whisper")).Allowed,
			"Native synthesizer volume eligibility was flattened.");
		actor.SetMerits(actor.Merits.Where(x => !ReferenceEquals(x, modifier.Object))); body.CalculateOrganFunctions(true);
		Console.WriteLine("ARM-SPEECH-volume=passed actual-synthesizer cached-native-function:0.4 normal-refused whisper-eligible");
		Mock.Get(body.Race).SetupGet(x => x.CommunicationStrategy).Returns(HumanoidCommunicationStrategy.Instance);

		var firstListener = NewSpeechListener(database, native, "arm_speech_listener1", language, true);
		var secondListener = NewSpeechListener(database, native, "arm_speech_listener2", language, true);
		var unknown = NewSpeechListener(database, native, "arm_speech_unknown", language, false);
		listeners.AddRange([firstListener, secondListener, unknown]);
		foreach (var listener in new[] { firstListener, secondListener })
		{
			Require(service.Enrol(staff.Object, listener, cap.Id, "Eligible native listener").Allowed, "Listener admission failed.");
			listener.SetTraitValue(trait, 62); listener.AddTrait(world.Traits.GetByName("ARM02 Agility"), 10); listener.AddResource(native.Resource, 100);
			store.Write(acquired: service.Acquisition(listener, spell.Id)! with { ControlledGrade = 3 });
			ConfigureSpeechCommands(listener);
		}
		var heard1 = new List<SpokenLanguageInfo>(); var heard2 = new List<SpokenLanguageInfo>(); var unknownText = new List<string>();
		ConfigureSpeechOutput(firstListener, heard1, []); ConfigureSpeechOutput(secondListener, heard2, []); ConfigureSpeechOutput(unknown, [], unknownText);
		Reset(); beforeChecks = Checks(); RunSpeechNetworkCommand(actor, "say " + formula);
		Require(Checks() == beforeChecks + 1 && heard1.Count == 1 && heard2.Count == 1 && firstListener.MagicResourceAmounts[native.Resource] == 100 &&
			secondListener.MagicResourceAmounts[native.Resource] == 100 && service.Acquisition(unknown, spell.Id) is null &&
			unknownText.Any(x => x.Contains("unknown tongue")) && unknownText.All(x => !x.Contains("kral", StringComparison.OrdinalIgnoreCase)),
			"Native hearing cast for eligible listeners, granted unknown knowledge or leaked formula words.");
		RunSpeechNetworkCommand(firstListener, "say " + formula);
		Require(Checks() == beforeChecks + 2 && firstListener.MagicResourceAmounts[native.Resource] == 85 && secondListener.MagicResourceAmounts[native.Resource] == 100,
			"A listener's own repeat did not require a separate paid invocation.");
		Console.WriteLine("ARM-SPEECH-listeners=passed two-eligible-observers no-observer-cast unknown-language-no-formula/knowledge own-repeat-separate-payment");
		var unheard = new List<string>(); ConfigureSpeechOutput(firstListener, [], unheard);
		var deafParent = new MagicSpellParent(firstListener.Body, spell, actor); var deaf = new SpellDeafnessEffect(firstListener.Body, deafParent, null!);
		deafParent.AddSpellEffect(deaf); firstListener.Body.AddEffect(deafParent); firstListener.Body.AddEffect(deaf);
		Require(!firstListener.CanHear(actor), "Native deafness did not inhibit hearing."); Reset(); beforeChecks = Checks(); RunSpeechNetworkCommand(actor, "say " + formula);
		Require(Checks() == beforeChecks + 1 && unheard.Count > 0 && unheard.All(x => !x.Contains("kral", StringComparison.OrdinalIgnoreCase)), "A deaf native listener perceived formula words.");
		firstListener.Body.RemoveEffect(deafParent, true); ConfigureSpeechOutput(firstListener, heard1, []);
		Console.WriteLine("ARM-SPEECH-hearing=passed native-deafness receiver-language-parser no-formula-words no-listener-cast");
		foreach (var commandAdapter in new[] { true, false })
		{
			Reset(); beforeChecks = Checks(); beforeOps = Operations(); var callback = false;
			var witness = new SpeechWitnessCallback(firstListener, formula); firstListener.AddEffect(witness);
			ConfigureSpeechOutput(actor, outputs, outputText, speech =>
			{
				if (callback || speech.Provenance is null) return; callback = true; ((IControllable)actor).ExecuteCommand("say " + formula);
			});
			var listenerBalance = firstListener.MagicResourceAmounts[native.Resource];
			if (commandAdapter) MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} cast \"{spell.Name}\" grade 3 on self via {cap.Name.DoubleQuotes()}");
			else RunSpeechNetworkCommand(actor, "say " + formula);
			Require(callback && witness.Fired && Checks() == beforeChecks + 1 && Operations() == beforeOps + 1 &&
				actor.MagicResourceAmounts[native.Resource] == 85 && firstListener.MagicResourceAmounts[native.Resource] == listenerBalance &&
				outputs.Count >= 3, "Self-output or native effect-generated words became a paid invocation.");
			firstListener.RemoveEffect(witness); ConfigureSpeechOutput(actor, outputs, outputText);
			Console.WriteLine($"ARM-SPEECH-callbacks=passed adapter:{(commandAdapter ? "named" : "speech")} self-output-nested-command native-witness-effect-Body.Say no-recast one-payment/check/operation");
		}
		Reset(); var relayParent = new MagicSpellParent(secondListener, spell, actor);
		var relay = new SpellReciteProxyEffect(secondListener, relayParent, actor.Id, actor.InstanceId,
			actor.Id, actor.InstanceId, 1.0, "@ recite|recites");
		relayParent.AddSpellEffect(relay); secondListener.AddEffect(relayParent); secondListener.AddEffect(relay);
		beforeChecks = Checks(); var beforeHeard = heard2.Count; RunSpeechNetworkCommand(actor, "say " + formula);
		Require(Checks() == beforeChecks + 1 && heard2.Count == beforeHeard + 2 && heard2.Skip(beforeHeard).Any(x => ReferenceEquals(x.Proxy, secondListener)) &&
			secondListener.MagicResourceAmounts[native.Resource] == 100,
			"Native recite relay failed to echo or caused a second cast.");
		secondListener.RemoveEffect(relayParent, true);
		Console.WriteLine("ARM-SPEECH-relay=passed native-recitation-effect witness/output callback eligible-proxy no-recast/payment");
		Reset(); var deadParent = new MagicSpellParent(secondListener, spell, actor);
		native.WorldMock.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, _) => listeners.FirstOrDefault(x => x.Id == id)!);
		// Exercise concrete DeadSpeak forwarding only. No corpse or owned temporary instance is created by this fixture.
		var deadSpeak = new SpellDeadSpeakEffect(secondListener, deadParent, actor.Id, actor.InstanceId, 0,
			secondListener.Id, secondListener.Body.Id, 0, actor.Location.Id, (int)actor.RoomLayer, spell.Id,
			actor.Id, actor.InstanceId, 1.0, CharacterInstancePersistencePolicy.TemporaryEffectBound, "", "", "", "@ recite|recites");
		deadSpeak.InitialEffect(); Require(secondListener.EffectsOfType<SpellReciteProxyEffect>().Any(), "Native DeadSpeak did not install forwarding.");
		beforeChecks = Checks(); beforeOps = Operations(); beforeHeard = heard2.Count; RunSpeechNetworkCommand(actor, "say " + formula);
		Require(Checks() == beforeChecks + 1 && Operations() == beforeOps + 1 && heard2.Count == beforeHeard + 2 &&
			heard2.Skip(beforeHeard).Any(x => ReferenceEquals(x.Proxy, secondListener)) && secondListener.MagicResourceAmounts[native.Resource] == 100,
			"DeadSpeak forwarding failed to echo or cast for its eligible proxy.");
		secondListener.RemoveAllEffects<SpellReciteProxyEffect>(x => ReferenceEquals(x.ParentEffect, deadParent), true);
		Console.WriteLine("ARM-SPEECH-deadspeak-forwarding=passed concrete-DeadSpeak-InitialEffect recite-installation no-proxy-cast/payment corpse-lifecycle-not-qualified");
		FlushCasting(native);
		var operation = outputs.Last(x => x.Provenance?.Kind == SpeechOriginKind.PlayerInput).Provenance.Id;
		RunSpeechReaderProcess(new(database.Name, fixture, spell.Id, cap.Id, trait.Id, operation, language.Id, actor.MagicResourceAmounts[native.Resource], Operations()));
		Console.WriteLine("ARM-SPEECH-acceptance=passed native-anatomy language communication commands quiet guarded-casting persistence-reader");
		return 0;
	}

	private sealed class SpeechCharacterCatalogue(List<NativeHarnessCharacter> characters)
	{
		public IUneditableAll<ICharacter> Items { get; } = Create(characters);
		private static IUneditableAll<ICharacter> Create(List<NativeHarnessCharacter> characters)
		{
			var mock = new Mock<IUneditableAll<ICharacter>>(); mock.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => characters.FirstOrDefault(x => x.Id == id)!);
			mock.Setup(x => x.GetEnumerator()).Returns(() => characters.Cast<ICharacter>().GetEnumerator());
			mock.As<System.Collections.IEnumerable>().Setup(x => x.GetEnumerator()).Returns(() => characters.GetEnumerator()); return mock.Object;
		}
	}

	private sealed class SpeechWitnessCallback(ICharacter actor, string formula) : Effect(actor), IHandleEventsEffect
	{
		public bool Fired { get; private set; }
		protected override string SpecificEffectType => "NativeSpeechFixtureCallback";
		public override string Describe(IPerceiver voyeur) => "A one-shot native fixture speech witness.";
		public bool HandlesEvent(params EventType[] types) => types.Contains(EventType.CharacterSpeaksWitness);
		public bool HandleEvent(EventType type, params dynamic[] arguments)
		{
			if (Fired || type != EventType.CharacterSpeaksWitness) return false;
			Fired = true; actor.Body.Say(null!, formula); return false;
		}
	}

	private static Language ConfigureSpeechLanguage(NativeRuntime native, string connection, bool create)
	{
		using var db = NewIndependentContext(connection); Db.Language row;
		if (create)
		{
			var model = new Db.LanguageDifficultyModels { Name = "ARM Speech Model", Type = "WordList", Definition = "<Definition><Name>ARM Speech Model</Name><DefaultDifficulty>0</DefaultDifficulty><Words/><Sentences><Length Difficulty='0' Min='0' Max='4096'/></Sentences></Definition>" };
			db.LanguageDifficultyModels.Add(model); db.SaveChanges();
			row = new Db.Language { Name = "ARM Native Incantation", LinkedTraitId = CastingRequired(native.World.Traits.GetByName("ARM02 Agility")).Id,
				DifficultyModel = model.Id, UnknownLanguageDescription = "an unknown tongue", LanguageObfuscationFactor = 1.0 };
			row.Accents.Add(new Db.Accent { Name = "ARM Native Accent", Description = "A controlled fixture accent.", Suffix = "", VagueSuffix = "", Group = "ARM", Role = (int)AccentRole.Native, Difficulty = 0 });
			db.Languages.Add(row); db.SaveChanges();
		}
		row = db.Languages.Include(x => x.Accents).Single(x => x.Name == "ARM Native Incantation");
		var models = new All<ILanguageDifficultyModel>(); models.Add(new WordListDifficultyModel(db.LanguageDifficultyModels.Single(x => x.Id == row.DifficultyModel)));
		var languages = new All<ILanguage>(); var accents = new All<IAccent>();
		native.WorldMock.SetupGet(x => x.LanguageDifficultyModels).Returns(models); native.WorldMock.SetupGet(x => x.Languages).Returns(languages);
		native.WorldMock.SetupGet(x => x.Accents).Returns(accents); native.WorldMock.Setup(x => x.Add(It.IsAny<IAccent>())).Callback<IAccent>(x => accents.Add(x));
		var result = new Language(row, native.World); languages.Add(result);
		native.WorldMock.Setup(x => x.GetStaticInt("MaximumSayLength")).Returns(4096);
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((speaker, _, _, _, _, _, _) =>
				CheckOutcome.SimpleOutcome(CheckType.SpokenLanguageHearCheck, speaker is ICharacter c && c.Languages.Contains(result) ? Outcome.Pass : Outcome.MajorFail));
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(),
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.LanguageListenCheck, Outcome.Pass));
		native.WorldMock.Setup(x => x.GetCheck(CheckType.SpokenLanguageSpeakCheck)).Returns(check.Object);
		native.WorldMock.Setup(x => x.GetCheck(CheckType.SpokenLanguageHearCheck)).Returns(check.Object);
		native.WorldMock.Setup(x => x.GetCheck(CheckType.LanguageListenCheck)).Returns(check.Object);
		native.WorldMock.Setup(x => x.GetCheck(CheckType.AccentImproveCheck)).Returns(check.Object);
		native.WorldMock.Setup(x => x.GetCheck(CheckType.AccentAcquireCheck)).Returns(check.Object);
		return result;
	}

	private static void PrepareSpeechActor(NativeHarnessCharacter actor, Language language, bool knowLanguage = true)
	{
		var name = new Mock<IPersonalName>(); name.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns($"Native speech participant {actor.Id}");
		SetPrivateField(actor, "_personalName", name.Object); SetPrivateField(actor, "_currentName", name.Object);
		SetPrivateField(actor, "_languages", new List<ILanguage>()); SetPrivateField(actor, "_accents", new Dictionary<IAccent, Difficulty>());
		SetPrivateField(actor, "_preferredAccents", new Dictionary<ILanguage, IAccent>()); SetPrivateField(actor, "_acquisitionAccents", new Dictionary<ILanguage, IAccent>());
		SetPrivateField(actor, "_installedHooks", new List<IHook>());
		var dummy = new Mock<IHook>(); dummy.SetupGet(x => x.Type).Returns(EventType.CharacterSpeaks);
		dummy.SetupGet(x => x.Function).Returns((_, _) => false); actor.InstallHook(dummy.Object); actor.RemoveHook(dummy.Object);
		actor.NativeSpeechEvents = true;
		if (knowLanguage) { actor.LearnLanguage(language); actor.LearnAccent(language.Accents.Single(), Difficulty.Automatic); actor.CurrentLanguage = language; actor.CurrentAccent = language.Accents.Single(); }
	}

	private static NativeHarnessCharacter NewSpeechListener(TestDatabase database, NativeRuntime original, string prefix, Language language, bool known)
	{
		var fixture = FixtureSeed.Create(database, prefix, false); using var db = NewIndependentContext(database.ConnectionString);
		var model = db.Bodies.Find(fixture.BodyId)!; model.BodyPrototypeId = original.Body.Prototype.Id; model.RaceId = original.Body.Race.Id; model.EthnicityId = original.Body.Ethnicity.Id;
		var actor = NativeHarnessCharacter.Create(original.World, fixture.CharacterId, original.Actor.Location, original.Actor.Culture);
		var body = new RuntimeBody(model, original.World, actor); actor.AttachBody(body); SetPrivateField(body, "_healthTickActive", true);
		SetPrivateField(body, "_currentBloodVolumeLitres", 5.0); body.TotalBloodVolumeLitres = 5.0; body.CalculateOrganFunctions(true);
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(original.Capability)]); PrepareSpeechActor(actor, language, known);
		return actor;
	}

	private static void ConfigureSpeechOutput(NativeHarnessCharacter actor, List<SpokenLanguageInfo> utterances, List<string> text,
		Action<SpokenLanguageInfo>? callback = null)
	{
		var output = new Mock<IOutputHandler>(); output.SetupGet(x => x.Perceiver).Returns(actor);
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((s, _, _) => text.Add(s)).Returns(true);
		output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<IOutput, bool, bool>((o, _, _) =>
		{
			if (o is LanguageOutput { LanguageInfo: SpokenLanguageInfo speech }) { utterances.Add(speech); text.Add(speech.ParseFor(actor)); callback?.Invoke(speech); }
			else text.Add(o.ParseFor(actor));
		}).Returns(true);
		SetPrivateMember(actor, "OutputHandler", output.Object);
	}

	private static void ConfigureSpeechCommands(NativeHarnessCharacter actor)
	{
		var communicationsType = typeof(MagicModule).Assembly.GetType("MudSharp.Commands.Modules.CommunicationsModule")!;
		var module = communicationsType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;
		var moduleCommands = (CommandManager<ICharacter>)communicationsType.GetProperty("Commands")!.GetValue(module)!;
		var commands = new CharacterCommandManager("Unknown fixture command.", PermissionLevel.Player); commands.AddFrom(moduleCommands);
		var tree = new Mock<ICharacterCommandTree>(); tree.SetupGet(x => x.Commands).Returns(commands); SetPrivateMember(actor, "CommandTree", tree.Object);
	}

	private static void RunSpeechNetworkCommand(NativeHarnessCharacter actor, string command, bool repeatDispatch = false)
	{
		var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
		try
		{
			using var client = new TcpClient(); client.Connect((IPEndPoint)listener.LocalEndpoint); using var accepted = listener.AcceptTcpClient();
			using var connection = new PlayerConnection(accepted); var controller = new Mock<IFuturemudControlContext>();
			controller.SetupGet(x => x.Actor).Returns(actor); controller.Setup(x => x.HandleCommand(It.IsAny<string>())).Callback<string>(s => ((IControllable)actor).ExecuteCommand(s));
			connection.Bind(controller.Object); connection.StartTransport();
			client.GetStream().Write(Encoding.ASCII.GetBytes(command + "\n"));
			var wait = Stopwatch.StartNew(); while (!connection.HasIncomingCommands && wait.Elapsed < TimeSpan.FromSeconds(5)) Thread.Sleep(10);
			Require(connection.HasIncomingCommands, "Loopback command did not enter the native queue."); connection.AttemptCommand();
			if (repeatDispatch) connection.AttemptCommand();
			connection.RequestClose(ConnectionCloseMode.Abort);
		}
		finally { listener.Stop(); }
	}

	private static void RunSpeechReaderProcess(SpeechReader input)
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--speech-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input)))); using var process = Process.Start(start)!;
		var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Speech reader exceeded 60 seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); if (process.ExitCode != 0) throw new InvalidOperationException(error.GetAwaiter().GetResult());
	}

	private static int RunSpeechReader(string[] arguments)
	{
		var input = JsonSerializer.Deserialize<SpeechReader>(Encoding.UTF8.GetString(Convert.FromBase64String(arguments.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true, vocalAnatomy: true); ConfigureCastingWorld(native, database.ConnectionString, false);
		var language = ConfigureSpeechLanguage(native, database.ConnectionString, false); PrepareSpeechActor(native.Actor, language);
		Mock.Get(native.Body.Race).SetupGet(x => x.CommunicationStrategy).Returns(HumanoidCommunicationStrategy.Instance); native.Body.CalculateOrganFunctions(true);
		var service = new MagicCastingService(native.World, flush: () => throw new InvalidOperationException("Duplicate receipt must not flush.")); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var store = new MagicCastingStateStore(); var operation = CastingRequired(store.Operation(input.Operation)); var before = operation.Definition;
		var spell = (MagicSpell)CastingRequired(native.World.MagicSpells.Get(input.Spell)); Require(spell.GradeProfile!.Incantation!.LanguageId == input.Language && spell.GradeConfigurationErrors().Count == 0, "Native incantation profile did not reload.");
		Require(native.Actor.MagicResourceAmounts[native.Resource] == input.Balance && service.Acquisition(native.Actor, spell.Id)!.ControlledGrade == 3, "Native speech balance/knowledge changed on restart.");
		Require(service.Cast(new(native.Actor, input.Capability, input.Spell, 3, false, "self", OriginId: input.Operation)).Status == MagicCastingStatus.Refused &&
			store.Operation(input.Operation)!.Definition == before, "Restart replay changed the immutable operation.");
		using var db = NewIndependentContext(database.ConnectionString); Require(db.MagicCastingOperations.Count(x => x.CharacterId == native.Actor.Id) == input.Operations, "Reader created another operation.");
		Console.WriteLine($"ARM-SPEECH-reader=passed process:{Environment.ProcessId} native-XML profile/language costs knowledge immutable-origin:{input.Operation} no-replay"); return 0;
	}
}
