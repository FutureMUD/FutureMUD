#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public sealed record PsionicStockPower(string Type, string Verb, int Band, bool Basic, double Cost, int Seconds, string Help);

/// <summary>Canonical finite stock tuning shared by installation and definition-loading tests.</summary>
public static class PsionicStockContent
{
	public const double FocusCap = 100;
	public const double FocusPerMinute = 5;
	public static IReadOnlyList<(string Verb, string EffectType, int Band, int Cost, int Seconds)> SpellPowers { get; } =
	[
		("project", "astralprojection", 80, 15, 120),
		("possess", "seizebody", 80, 20, 60),
		("levitate", "levitate", 60, 8, 120),
		("illusion", "subjectivedesc", 60, 8, 120)
	];
	public static IReadOnlyList<PsionicStockPower> Powers { get; } =
	[
		new("connectmind", "contact", 0, true, 2, 300, "Contact a familiar mind in the same zone (same shard in Advanced Psionics). Use a dub for distant targets; disconnect ends the link."),
		new("connectmind", "connectback", 0, true, 2, 300, "Connect to an incoming presence with connectback last, even if its identity is unknown. End with disconnectback."),
		new("mindsay", "say", 0, true, 1, 0, "Send directed speech along your mind link. This does not read private thoughts."),
		new("mindbarrier", "barrier", 20, true, 2, 300, "Maintain a barrier against mental intrusion; endbarrier releases it."),
		new("mindaudit", "audit", 20, true, 2, 0, "Audit your own mind for incoming presences. Concealment can obscure their identities."),
		new("mindexpel", "expel", 40, true, 3, 0, "Attempt to expel incoming presences from your own mind."),
		new("psychometry", "psychometry", 0, false, 3, 300, "Read here or an item. Requires EnablePsychometricImpressions; never invents unrecorded history."),
		new("somaticsense", "somaticsense", 0, false, 2, 300, "Read a linked mind's broad bodily condition, with detail determined by success."),
		new("dreamsend", "dreamsend", 20, false, 3, 300, "Send a short dream to an eligible sleeping linked mind."),
		new("guardmind", "guardmind", 20, false, 2, 300, "Guard a willing linked mind; the protector maintains concentration and upkeep."),
		new("transferfocus", "transferfocus", 40, false, 2, 300, "Transfer Focus with <target> lend or siphon. Loss is 10 percent; both pools remain bounded."),
		new("disruptconcentration", "disruptconcentration", 60, false, 5, 300, "Challenge one maintained effect on a linked target."),
		new("forgetting", "forgetting", 80, false, 10, 300, "Suppress skill, knowledge, recognition or witness recall. Use incidents to select a known incident; virtual requires its scene. Stock forgetting is temporary."),
		new("psychiccircle", "psychiccircle", 40, false, 1, 300, "Begin or invite a target. Invitees use psicircle accept, say or leave. Maximum eight members."),
		new("psychicfeedback", "psychicfeedback", 60, false, 3, 300, "Maintain a reactive defence that warns and drains hostile intruders' Focus."),
		new("telekinesis", "telekinesis", 40, false, 3, 300, "Use <item> get|move|open|close, switch <setting>, select <option>, empty [destination or amount], pour <destination> [amount], fill <source> [amount], or put <container>. All targets must be local and unattended."),
		new("emotionalinfluence", "emotion", 40, false, 4, 300, "Read or influence represented emotions. Affinity does not grant obedience, ownership or legal authority."),
		new("attentionsuppression", "attentionsuppression", 60, false, 5, 300, "Make yourself difficult to notice until deliberately observed or you act hostilely."),
		new("delayedsuggestion", "delayedsuggestion", 80, false, 8, 300, "Plant one delayed thought or emotion. Triggers: delay, cell, encounter, combat. Never executes commands."),
		new("clairvoyance", "clairvoyance", 40, false, 5, 300, "Observe through a linked mind within the configured limits."),
		new("suggest", "suggest", 80, false, 5, 300, "Suggest a thought; this is a mental message, not a command."),
		new("empathy", "empathy", 40, false, 4, 300, "Transfer wounds empathically within safety limits; this is not emotion reading."),
		new("hex", "hex", 60, false, 5, 300, "Impose a temporary bounded check penalty."),
		new("psychicbolt", "psychicbolt", 60, false, 5, 300, "Project a limited stunning psychic attack."),
		new("trace", "trace", 20, false, 3, 300, "Investigate permitted psychic traces. Character traces remain available when cell impressions are disabled."),
		new("prescience", "prescience", 40, false, 5, 300, "Request a staff-mediated vision; this does not automatically predict future events."),
		new("hear", "hear", 60, false, 3, 300, "Maintain reception of permitted thoughts and feelings on a mind link."),
		new("clairaudience", "clairaudience", 40, false, 3, 300, "Maintain remote hearing through a linked mind."),
		new("allspeak", "allspeak", 40, false, 3, 300, "Maintain supernatural language comprehension."),
		new("magicksense", "magicksense", 20, false, 2, 300, "Maintain awareness of magic."),
		new("dangersense", "dangersense", 20, false, 2, 300, "Sense nearby threats within configured bounds."),
		new("sensitivity", "sensitivity", 20, false, 2, 300, "Maintain sensitivity to nearby magical and psychic activity."),
		new("babble", "babble", 60, false, 4, 120, "Temporarily interfere with speech."),
		new("coerce", "coerce", 80, false, 6, 120, "Apply bounded bodily distress rather than arbitrary commands."),
		new("projectemotion", "projectemotion", 20, false, 2, 120, "Communicate a sanitised feeling to a linked mind.")
	];

	public static XElement Definition(PsionicStockPower stock, long trait, long resource, long yes, long no, long error, long normal, long? identity = null, long? eligibility = null, bool advanced = false)
	{
		var root = new XElement("Definition", new XElement("IsPsionic", true), new XElement("CanInvokePowerProg", yes),
			new XElement("WhyCantInvokePowerProg", error), new XElement("Verb", stock.Verb),
			new XElement("PowerDistance", stock.Basic ? (int)MagicPowerDistance.SameLocationOnly : (int)MagicPowerDistance.AnyConnectedMindOrConnectedTo),
			new XElement("SkillCheckDifficulty", (int)Difficulty.Normal), new XElement("SkillCheckTrait", trait),
			new XElement("MinimumSuccessThreshold", (int)Outcome.MinorPass), new XElement("DetectableWithDetectMagic", (int)Difficulty.Normal),
			new XElement("FailEcho", "You cannot shape the mental impulse."), new XElement("SuccessEcho", ""),
			new XElement("Duration", stock.Seconds), new XElement("DurationSeconds", stock.Seconds), new XElement("Resource", resource),
			new XElement("Amount", 10), new XElement("Loss", 0.1), new XElement("Permanent", false),
			new XElement("PsionicTrace", new XElement("Enabled", true), new XElement("DurationSeconds", 900),
				new XElement("ReadDifficulty", (int)Difficulty.Hard), new XElement("Description", "a lingering psychic disturbance")),
			new XElement("InvocationCosts", new XElement("Verbs", new XElement("Verb", new XAttribute("verb", stock.Verb),
				new XElement("Cost", new XAttribute("resource", resource), stock.Cost)))));
		void Set(string name, object value) => root.SetElementValue(name, value);
		void Sustain()
		{
			Set("ConcentrationPointsToSustain", 1); Set("SustainPenalty", -2); Set("Duration", stock.Seconds * 1000);
			root.Add(new XElement("SustainResourceCosts", new XElement("Cost", new XAttribute("resource", resource), 1)));
			Set("BeginVerb", stock.Verb); Set("EndVerb", "end" + stock.Verb);
		}
		switch (stock.Type)
		{
			case "connectmind":
				Sustain(); Set("ConnectVerb", stock.Verb); Set("DisconnectVerb", stock.Verb == "connectback" ? "disconnectback" : "disconnect");
				Set("PowerDistance", (int)(stock.Verb == "connectback" ? MagicPowerDistance.AnyConnectedMindOrConnectedTo : advanced ? MagicPowerDistance.SameShardOnly : MagicPowerDistance.SameZoneOnly));
				Set("TargetCanSeeIdentityProg", identity ?? no); Set("TargetEligibilityProg", eligibility ?? yes); Set("ExclusiveConnection", false);
				Set("UnknownIdentityDescription", "an unfamiliar mind");
				Set("SkillCheckDifficulty", (int)Difficulty.VeryEasy);
				root.Add(new XElement("OutcomeEchoes", Enum.GetValues<Outcome>().Select(x => new XElement("Outcome", new XAttribute("outcome", (int)x), new XAttribute("shouldecho", x >= Outcome.MinorPass)))));
				break;
			case "mindsay":
				Set("SayVerb", stock.Verb); Set("TellVerb", "tell"); Set("EmoteText", "You project the words: {0}");
				Set("FailEmoteText", "Your words fade unformed."); Set("TargetEmoteText", "{0} speaks into your mind: {1}");
				Set("UnknownIdentityDescription", "an unfamiliar mind"); Set("UseAccent", false); Set("UseLanguage", false); Set("TargetCanSeeIdentityProg", identity ?? no);
				root.Element("InvocationCosts")!.Element("Verbs")!.Add(new XElement("Verb", new XAttribute("verb", "tell"), new XElement("Cost", new XAttribute("resource", resource), stock.Cost)));
				break;
			case "mindbarrier":
				Sustain(); Set("AppliesToCharacterProg", yes); Set("PermitAllies", false); Set("PermitTrustedAllies", true); Set("FailIfOvercome", false);
				root.Add(new XElement("Bonuses", Enum.GetValues<Outcome>().Where(x => x >= Outcome.MajorFail).Select(x => new XElement("Bonus", new XAttribute("outcome", (int)x), new XAttribute("bonus", x >= Outcome.MinorPass ? -15 : 0)))));
				break;
			case "mindaudit": case "mindexpel":
				Set("EmoteText", ""); Set("EmoteTextSelf", "You examine the boundaries of your own mind."); Set("SkillCheckDifficultyProg", normal);
				Set("ShouldEchoDetectionProg", yes); Set("EchoToDetectedTarget", "Your mental presence has been noticed.");
				Set("EchoToExpelledTarget", "Your mental connection is expelled."); Set("EchoToNonExpelledTarget", "You feel pressure against your connection.");
				break;
			case "empathy": Set("MaxWounds", 3); Set("SafetyHealthPercent", 0.75); Set("TransferIntervalSeconds", 10); break;
			case "hex": Set("Penalty", 10); Set("Categories", "All"); Set("ReplaceExisting", true); break;
			case "psychicbolt": Set("StunAmount", 10); Set("DamageType", "Eldritch"); break;
			case "prescience": Set("BoardIdOrName", "Staff"); Set("PromptText", "Describe the question or vision you seek."); Set("SubjectTemplate", "Prescience: {character}"); Set("AuthorTemplate", "{character}"); break;
			case "hear": case "clairaudience": case "allspeak": case "magicksense": case "dangersense": case "sensitivity":
				Sustain();
				Set("ShowThinks", true); Set("ShowFeels", true); Set("ShowName", false); Set("ShowEmotes", true); Set("ShowDescriptionProg", no);
				Set("ThreatRange", 1); Set("DefenseBonus", 10); Set("DefenseDurationSeconds", 15); Set("ThreatWarningIntervalSeconds", 30);
				Set("ScanVerb", "senscan"); Set("ScanDistance", MagicPowerDistance.SameLocationOnly); Set("ActivityKinds", "Magical,Psychic");
				Set("ActivityRange", 1); Set("PermitCapabilityRead", false); Set("NotifySelf", false);
				break;
		}
		if (PsionicPowerEmotes.All.TryGetValue(stock.Type, out var emotes))
			foreach (var (field, text) in emotes) Set(field, text);
		return root;
	}
}
