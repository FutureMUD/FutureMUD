#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.Magic;

/// <summary>Authored stock echoes shared by seed XML and newly built powers. Persisted custom text is not replaced.</summary>
public static class PsionicPowerEmotes
{
	public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> All { get; } =
		new Dictionary<string, IReadOnlyDictionary<string, string>>
		{
			["allspeak"] = new Dictionary<string, string>
			{
				["BeginEmote"] = "Unfamiliar words begin to carry meaning as you attune your mind to language.",
				["EndEmote"] = "The borrowed understanding fades; unfamiliar words become strange again.",
				["FailEmote"] = "You search for meaning beneath the words, but it remains out of reach.",
			},
			["babble"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for the words forming in $1's thoughts, but cannot unsettle them.",
				["SuccessEcho"] = "You knot the words forming in $1's thoughts into a confused jumble.",
			},
			["clairaudience"] = new Dictionary<string, string>
			{
				["BeginEmote"] = "You turn your attention to the sounds reaching $1.",
				["EndEmote"] = "The borrowed sounds fade, leaving only your own hearing.",
				["FailEmote"] = "You cannot distinguish the sounds reaching $1 from your own surroundings.",
			},
			["clairvoyance"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach out for $1's surroundings, but the image collapses.",
				["SuccessEcho"] = "@ grow|grows still, &0's gaze losing focus.",
			},
			["coerce"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You press a wave of discomfort toward $1, but cannot make it take hold.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
			},
			["connectmind"] = new Dictionary<string, string>
			{
				["EmoteForConnect"] = "You sense {0} settle at the edge of your thoughts.",
				["SelfEmoteForConnect"] = "You reach out and establish a thread of contact with $1's mind.",
				["EmoteForFailConnect"] = "You sense {0} brush against your thoughts, then slip away.",
				["SelfEmoteForFailConnect"] = "You reach for $1's mind, but the thread of contact will not hold.",
				["EmoteForDisconnect"] = "The presence of {0} recedes from your thoughts.",
				["SelfEmoteForDisconnect"] = "Your thread of contact with $1's mind falls away.",
			},
			["dangersense"] = new Dictionary<string, string>
			{
				["BeginEmote"] = "You let your awareness hover at the edge of each moment, alert for danger.",
				["EndEmote"] = "The watchful tension in your thoughts eases.",
				["FailEmote"] = "You reach for a warning of danger, but your attention will not settle.",
				["ThreatEcho"] = "A prickling warning crawls across your thoughts: danger is nearby.",
				["DefenseEcho"] = "A flash of warning sharpens your reactions.",
			},
			["empathy"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for $1's pain, but cannot take hold of it.",
				["StartEcho"] = "@ grow|grows still, &0's attention fixed on $1.",
				["TransferEcho"] = "@ shudder|shudders as a wound passes from $1 into &0.",
				["StopEcho"] = "Your empathic link to $1 fades.",
				["SafetyEcho"] = "You recoil from taking another wound before the pain overwhelms you.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
			},
			["hear"] = new Dictionary<string, string>
			{
				["BeginEmote"] = "You quiet your own thoughts and listen along your mental links.",
				["EndEmote"] = "You draw your attention back from the thoughts of others.",
				["FailEmote"] = "Your own thoughts drown out the distant mental voices.",
			},
			["hex"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for $1's fortune, but your malice slips away.",
				["SuccessEcho"] = "@ fix|fixes &0's attention on $1, &0's expression tightening.",
				["TargetEcho"] = "A hostile psychic pressure settles over you.",
			},
			["magicksense"] = new Dictionary<string, string>
			{
				["BeginEmote"] = "You open your awareness to the presence of magic.",
				["EndEmote"] = "Your awareness of magic recedes into the background.",
				["FailEmote"] = "You cannot distinguish magic from the noise of your ordinary senses.",
			},
			["mindaudit"] = new Dictionary<string, string>
			{
				["EmoteText"] = "",
				["EmoteTextSelf"] = "You turn your attention inward, searching the edges of your thoughts for foreign presences.",
				["EchoToDetectedTarget"] = "A searching awareness brushes against your foothold in $0's mind.",
			},
			["mindbarrier"] = new Dictionary<string, string>
			{
				["EmoteForBegin"] = "",
				["EmoteForEnd"] = "",
				["EmoteForBeginSelf"] = "You draw your thoughts inward, setting a firm boundary against other minds.",
				["EmoteForEndSelf"] = "You relax the guarded boundary of your mind.",
				["BlockEmoteSelf"] = "An outside presence presses against your mental barrier, but finds no purchase.",
				["BlockEmoteTarget"] = "Your reaching thoughts meet a firm barrier around $1's mind.",
				["OvercomeEmoteSelf"] = "An outside presence breaks through the boundary of your mind.",
				["OvercomeEmoteTarget"] = "You force a path through the barrier around $1's mind.",
				["EndWhenNotSustainingError"] = "You have no mental barrier to release.",
				["BeginWhenAlreadySustainingError"] = "You are already maintaining a mental barrier.",
			},
			["mindexpel"] = new Dictionary<string, string>
			{
				["EmoteText"] = "",
				["EmoteTextSelf"] = "You gather your will and push outward against the foreign presences in your mind.",
				["EchoToExpelledTarget"] = "A surge of will tears your connection to $0's mind free.",
				["EchoToNonExpelledTarget"] = "A surge of will strains your connection to $0's mind, but it holds.",
			},
			["mindsay"] = new Dictionary<string, string>
			{
				["EmoteText"] = "You send to $1: {0}",
				["FailEmoteText"] = "The words scatter before you can send them to $1.",
				["TargetEmoteText"] = "{0} speaks into your mind: {1}",
			},
			["prescience"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You open yourself to the future, but the vision remains silent.",
				["SuccessEcho"] = "You open yourself to the future and shape a question in your mind.",
			},
			["projectemotion"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for $1's feelings, but cannot touch them.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
			},
			["psychicbolt"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You hurl force at $1's mind, but it scatters before impact.",
				["SuccessEcho"] = "@ tense|tenses, &0's attention snapping toward $1.",
				["TargetEcho"] = "Invisible psychic force crashes through your mind.",
			},
			["sensitivity"] = new Dictionary<string, string>
			{
				["BeginEmote"] = "You still your thoughts, feeling for ripples of magic and psychic activity.",
				["EndEmote"] = "The unseen currents fade from your awareness.",
				["FailEmote"] = "Your thoughts remain too restless to sense the unseen currents.",
				["ActivityEcho"] = "A ripple of {kind} activity touches your sensitivity: {description}.",
			},
			["suggest"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for $1's thoughts, but cannot plant your suggestion.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
			},
			["trace"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You trace the surface of $1's mind, but the connections elude you.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
			},
			["psychometry"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You sift through the lingering impressions, but no clear image takes shape.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
				["NoImpressionsEcho"] = "You discern no readable impressions.",
				["MixedHistoryEcho"] = "The object's history is mixed and indistinct.",
				["ImpressionEcho"] = "{0} ({1} ago)",
				["CustodyEcho"] = "Carried by {0} for {1}{2}.",
			},
			["somaticsense"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for $1's bodily sensations, but cannot separate them from your own.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
				["ConditionEcho"] = "You sense {0} as {1}, {2}.",
				["PainEcho"] = "There is pain within that body.",
				["NoPainEcho"] = "You sense no wound pain.",
				["WoundEcho"] = "The worst wound feels {0}.",
				["NoWoundsEcho"] = "You sense no wounds.",
			},
			["dreamsend"] = new Dictionary<string, string>
			{
				["FailEcho"] = "Your dream-image unravels before it can reach $1.",
				["SuccessEcho"] = "You weave a brief image into $1's dreams.",
				["DreamEcho"] = "Within a dream, you experience:\n{0}",
			},
			["guardmind"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You cannot steady your awareness around $1's mind.",
				["SuccessEcho"] = "You extend a watchful boundary around $1's mind.",
				["TargetEcho"] = "A watchful presence settles around your thoughts, ready to meet an intrusion.",
				["EndEcho"] = "You release your watch over $1's mind.",
				["TargetEndEcho"] = "The watchful presence around your thoughts recedes.",
				["IntrusionEcho"] = "You sense an intrusion pressing against $1's protected mind.",
				["ExpelEcho"] = "You help the willing mind push against foreign presences.",
			},
			["disruptconcentration"] = new Dictionary<string, string>
			{
				["FailEcho"] = "Your mental thrust fails to disturb $1's concentration.",
				["SuccessEcho"] = "You drive a sharp pulse of distraction into $1's thoughts.",
			},
			["transferfocus"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You cannot establish a steady flow of strength with $1.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
				["LendEcho"] = "You lend {0} {1} to the other mind.",
				["SiphonEcho"] = "You draw {0} {1} from the other mind.",
			},
			["forgetting"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You reach for the memory, but cannot loosen its hold.",
				["SuccessEcho"] = "You place a veil over that part of the mind.",
				["WitnessEcho"] = "You press the incident out of that mind's reach.",
				["VirtualEcho"] = "You reach into the remembered impressions of the incident's bystanders.",
			},
			["psychiccircle"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You cannot hold the circle of minds together.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
				["InviteEcho"] = "You extend an invitation to your psychic circle.",
				["TargetInviteEcho"] = "You are invited to a psychic circle. Use PSICIRCLE ACCEPT or PSICIRCLE DECLINE.",
				["JoinEcho"] = "You join the psychic circle. Use PSICIRCLE SAY <message> or PSICIRCLE LEAVE.",
				["LeaveEcho"] = "The shared voices fade as your link to the psychic circle falls away.",
				["SelfSpeechEcho"] = "Within the psychic circle, you say: {0}",
				["SpeechEcho"] = "Within the psychic circle, {0} says: {1}",
			},
			["psychicfeedback"] = new Dictionary<string, string>
			{
				["FailEcho"] = "The tension you gather around your mind dissipates.",
				["SuccessEcho"] = "You gather a taut, answering pressure at the boundary of your thoughts.",
				["EndEcho"] = "The answering pressure around your thoughts dissipates.",
				["IntrusionEcho"] = "An intrusion tugs at the answering pressure around your thoughts.",
				["ResponseEcho"] = "Psychic feedback lashes back against your intrusion.",
			},
			["telekinesis"] = new Dictionary<string, string>
			{
				["FailEcho"] = "Your grasp on the object slips before you can move it.",
				["SuccessEcho"] = "An unseen force manipulates $1.",
				["UnresponsiveEcho"] = "The mechanism does not respond to your manipulation.",
			},
			["emotionalinfluence"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You cannot find purchase among $1's feelings.",
				// No extra success preamble: named result, lifecycle or shared traffic echoes provide the output.
				["SuccessEcho"] = "",
				["ReadingEcho"] = "You sense {0} within that mind.",
				["NoEmotionsEcho"] = "No distinct emotional influence emerges.",
			},
			["attentionsuppression"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You try to slip from notice, but your presence remains distinct.",
				["SuccessEcho"] = "You soften the impression of your presence, inviting attention to slide past you.",
			},
			["delayedsuggestion"] = new Dictionary<string, string>
			{
				["FailEcho"] = "You cannot anchor the suggestion in $1's thoughts.",
				["SuccessEcho"] = "You tuck the suggestion into $1's thoughts, poised to stir when its moment arrives.",
			},
		};

	/// <summary>Shared traffic templates: formatted text, never reinterpreted as emotes or commands.</summary>
	public static IReadOnlyDictionary<string, string> Shared { get; } = new Dictionary<string, string>
	{
		["EmotionSourceEcho"] = "You push a feeling into {0}'s mind: {1}",
		["EmotionTargetEcho"] = "A feeling that is not your own settles into your mind: {0}",
		["EmotionListenerEcho"] = "{0} feels {1}",
		["ThoughtSourceEcho"] = "You press a thought into {0}'s mind:\n\t\"{1}\"",
		["ThoughtTargetEcho"] = "A thought that is not your own surfaces in your mind:\n\t\"{0}\"",
		["ThoughtListenerEcho"] = "{0} thinks,\n\t\"{1}\"",
		["CoerceSourceEcho"] = "You coerce {0} through {1}.",
		["CoerceTargetEcho"] = "A psionic pressure forces {0} through your body.",
	};

	public static string FormatShared(string field, params object[] arguments) =>
		string.Format(Shared[field], arguments);

	public static int FormatArgumentCount(string type, string field) =>
		Regex.Matches(Get(type, field), @"\{(\d+)\}")
			.Select(x => int.Parse(x.Groups[1].Value) + 1).DefaultIfEmpty(0).Max();

	/// <summary>Defaults for the five echo fields on each seeded backing spell.</summary>
	public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Spells { get; } =
		new Dictionary<string, IReadOnlyDictionary<string, string>>
		{
			["project"] = new Dictionary<string, string>
			{
				["CastingEmote"] = "@ grow|grows motionless, &0's gaze turning inward.",
				["FailCastingEmote"] = "@ falter|falters, &0's concentration breaking.",
				["TargetEmote"] = "Your awareness loosens from the weight of your body.",
				["TargetResistedEmote"] = "You hold fast to your body as the invading will loses its grip.",
				["TargetNullEmote"] = "Your intended subject offers no foothold for this power.",
			},
			["possess"] = new Dictionary<string, string>
			{
				["CastingEmote"] = "@ fix|fixes &0's gaze on $1 with unbroken concentration.",
				["FailCastingEmote"] = "@ falter|falters, &0's concentration breaking.",
				["TargetEmote"] = "Another will presses into your awareness, reaching for control of your body.",
				["TargetResistedEmote"] = "You hold fast to your body as the invading will loses its grip.",
				["TargetNullEmote"] = "Your intended subject offers no foothold for this power.",
			},
			["levitate"] = new Dictionary<string, string>
			{
				["CastingEmote"] = "@ steady|steadies %0, &0's attention fixed on the space beneath &0.",
				["FailCastingEmote"] = "@ falter|falters, &0's concentration breaking.",
				["TargetEmote"] = "An unseen support takes your weight, freeing you from the ground.",
				["TargetResistedEmote"] = "You hold fast to your body as the invading will loses its grip.",
				["TargetNullEmote"] = "Your intended subject offers no foothold for this power.",
			},
			["illusion"] = new Dictionary<string, string>
			{
				["CastingEmote"] = "@ study|studies &0's outline, concentrating on its edges.",
				["FailCastingEmote"] = "@ falter|falters, &0's concentration breaking.",
				["TargetEmote"] = "A translucent shimmer softens the outline you perceive around yourself.",
				["TargetResistedEmote"] = "You hold fast to your body as the invading will loses its grip.",
				["TargetNullEmote"] = "Your intended subject offers no foothold for this power.",
			},
		};

	public static string Get(string type, string field, string fallback = "") =>
		All.TryGetValue(type, out var fields) && fields.TryGetValue(field, out var value) ? value : fallback;
}
