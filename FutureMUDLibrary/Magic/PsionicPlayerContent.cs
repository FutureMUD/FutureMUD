#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Magic;

public sealed record PsionicPlayerPower(string Name, string Description, string[] Syntax, string Example);

/// <summary>Player-facing names and command help; installation guidance belongs in seeder documentation.</summary>
public static class PsionicPlayerContent
{
	public static IReadOnlyDictionary<string, PsionicPlayerPower> Powers { get; } = new Dictionary<string, PsionicPlayerPower>
	{
		["contact"] = new("Mind Contact", "Establish a mental link with a familiar person in the same {range}. Use a dub to reach someone who is not in sight. Other powers can use this link until you release it or can no longer sustain it.",
			["contact <person or dub>", "contact last", "disconnect [person]"], "contact friend"),
		["connectback"] = new("Connect Back", "Follow an incoming mental presence back to its source, even if you do not know who it is. This lets you open your own link and reply using Mind Speech.",
			["connectback last", "disconnectback [person]"], "connectback last"),
		["say"] = new("Mind Speech", "Send words through a mental link. Say uses your first outgoing link; tell selects a person when you have more than one. Your message is heard in the mind, not spoken aloud.",
			["say <message>", "tell <person> <message>"], "tell friend The path is clear."),
		["barrier"] = new("Mind Barrier", "Draw a boundary around your mind to resist intrusion. Maintaining it requires concentration and Focus; it ends if you can no longer sustain it.",
			["barrier", "endbarrier"], "barrier"),
		["audit"] = new("Mind Audit", "Search your own thoughts for foreign presences. A concealed presence may be difficult to detect or identify.",
			["audit"], "audit"),
		["expel"] = new("Mind Expulsion", "Push against foreign presences in your mind, attempting to break their connections. Stronger intruders may withstand the attempt.",
			["expel"], "expel"),
		["psychometry"] = new("Psychometry", "Read lingering impressions from your surroundings or an object. Clearer readings may reveal more about past activity or those who carried an object; some histories leave no readable impression.",
			["psychometry here", "psychometry <item>"], "psychometry pendant"),
		["somaticsense"] = new("Somatic Sense", "Reach through a mental link to sense another person's bodily condition. A clearer reading reveals signs of exhaustion, pain and injury.",
			["somaticsense <person>"], "somaticsense friend"),
		["dreamsend"] = new("Dream Sending", "Weave a brief image or experience into a sleeping person's dreams through a mental link. An awake or protected mind cannot receive it.",
			["dreamsend <person> <dream text>"], "dreamsend friend You stand beside a quiet sea."),
		["guardmind"] = new("Guard Mind", "Extend your protection around a linked person who trusts you. You maintain the concentration and Focus, and can help push an intruder from their mind.",
			["guardmind <person>", "guardmind <person> expel", "guardmind end"], "guardmind friend"),
		["transferfocus"] = new("Transfer Focus", "Lend strength to a trusted linked person, or try to draw it from another mind. Some strength is lost in the transfer; you cannot take more than the donor has or receive more than you can hold.",
			["transferfocus <person> lend", "transferfocus <person> siphon"], "transferfocus friend lend"),
		["disruptconcentration"] = new("Disrupt Concentration", "Send a sharp pulse of distraction through a mental link, challenging one of the other person's sustained powers. A successful intrusion does not guarantee that their concentration will break.",
			["disruptconcentration <person>"], "disruptconcentration rival"),
		["forgetting"] = new("Selective Forgetting", "Place a temporary veil over access to a skill, knowledge, recognition of a person, or recall of a witnessed incident. Use incidents to choose an incident you know; reaching its unseen bystanders requires being at its scene. Reports already delivered are not erased.",
			["forgetting incidents", "forgetting <person> skill <skill>", "forgetting <person> knowledge <knowledge>", "forgetting <person> recognition <recognised person>", "forgetting <person> witness <incident number>", "forgetting virtual <incident number>"], "forgetting rival witness 1"),
		["psychiccircle"] = new("Psychic Circle", "Gather up to eight minds, including your own, into a shared conversation. Invitees must accept. You sustain the circle; membership does not reveal private thoughts.",
			["psychiccircle begin", "psychiccircle invite <person>", "psychiccircle dismiss <person>", "psychiccircle end", "!psicircle accept", "!psicircle decline", "!psicircle say <message>", "!psicircle leave"], "psychiccircle invite friend"),
		["psychicfeedback"] = new("Psychic Feedback", "Gather a defence that reacts to hostile mental intrusion, warning you and striking back at the intruder's Focus. It requires concentration and Focus to maintain.",
			["psychicfeedback begin", "psychicfeedback end"], "psychicfeedback begin"),
		["telekinesis"] = new("Telekinesis", "Manipulate visible, unattended objects nearby with your mind. Both source and destination must be within reach and light enough to affect. Closed or locked containers and fixed objects retain their ordinary restrictions. Liquid amounts may include volume units; omit the amount to transfer as much as fits.",
			["telekinesis <item> get|move|open|close", "telekinesis <item> switch <setting>", "telekinesis <item> select <option>", "telekinesis <item> empty [destination or liquid amount]", "telekinesis <item> pour <destination> [amount]", "telekinesis <item> fill <source> [amount]", "telekinesis <item> put <container>"], "telekinesis cup pour bowl"),
		["emotion"] = new("Emotional Attunement", "Sense feelings in a linked mind or attempt to encourage an emotion. Affinity and aversion concern a person you can identify nearby. Influencing feelings does not compel obedience.",
			["emotion <person> read", "emotion <person> fear|calm|courage|agitation", "emotion <person> affinity|aversion <subject>"], "emotion friend calm"),
		["attentionsuppression"] = new("Attention Suppression", "Soften your presence in other minds so that their attention tends to pass over you. Deliberate observation can overcome it, and hostile action ends the effect.",
			["attentionsuppression"], "attentionsuppression"),
		["delayedsuggestion"] = new("Delayed Suggestion", "Leave a thought or emotion waiting in a linked mind. It stirs once, after a delay or when the person enters this place, encounters someone you identify, or enters combat. The mind can still resist when the moment arrives.",
			["delayedsuggestion <person> delay <seconds> thought <text>", "delayedsuggestion <person> cell here thought <text>", "delayedsuggestion <person> encounter <subject> thought <text>", "delayedsuggestion <person> combat thought <text>", "delayedsuggestion <person> <trigger> emotion <fear|calm|courage|agitation|affinity|aversion>"], "delayedsuggestion friend delay 30 thought Remember the eastern gate."),
		["clairvoyance"] = new("Clairvoyance", "Briefly borrow a linked person's view of their surroundings. This gives a present glimpse rather than a view of the past or future.",
			["clairvoyance <person>"], "clairvoyance friend"),
		["suggest"] = new("Suggestion", "Place a thought in a linked mind. You may wrap it in an emotion using square brackets. The recipient receives a thought, not an order they must obey.",
			["suggest <person> <thought>", "suggest <person> [emotion] <thought>"], "suggest friend [reassurance] We can find another way."),
		["empathy"] = new("Empathic Healing", "Take another person's wounds into your own body, one at a time. This can injure you. The transfer stops at its limit or when taking more wounds would endanger you.",
			["empathy <person>"], "empathy friend"),
		["hex"] = new("Hex", "Lay a temporary hindrance on a linked person, making their efforts more difficult. The other mind may resist your influence.",
			["hex <person>"], "hex rival"),
		["psychicbolt"] = new("Psychic Bolt", "Strike a linked mind with a stunning pulse of psychic force. Mental defences can resist the attack.",
			["psychicbolt <person>"], "psychicbolt rival"),
		["trace"] = new("Psychic Trace", "Examine the mental connections and lingering psychic traces around a linked person. Concealment can obscure what you discover and whose presence left a trace.",
			["trace <person>"], "trace friend"),
		["prescience"] = new("Prescience", "Open yourself to a possible vision. After invoking the power, use the text editor to describe the question or vision you seek. A response may come later; no immediate answer or certain prediction is promised.",
			["prescience"], "prescience"),
		["hear"] = new("Hear Thoughts", "Listen for thoughts and feelings passing through minds linked with yours. This does not hear ordinary sounds in their surroundings. Maintaining the reception requires concentration and Focus.",
			["hear", "endhear"], "hear"),
		["clairaudience"] = new("Clairaudience", "Listen to sounds reaching a linked person. Maintaining the borrowed hearing requires concentration and Focus.",
			["clairaudience <person>", "endclairaudience"], "clairaudience friend"),
		["allspeak"] = new("Allspeak", "Attune your mind to meaning so you can understand unfamiliar languages while the power lasts. Maintaining the attunement requires concentration and Focus.",
			["allspeak", "endallspeak"], "allspeak"),
		["magicksense"] = new("Magic Sense", "Open your awareness to the presence of magic. Maintaining this sense requires concentration and Focus.",
			["magicksense", "endmagicksense"], "magicksense"),
		["dangersense"] = new("Danger Sense", "Remain alert to nearby danger through a heightened sense of warning. A warning can briefly sharpen your reactions. Maintaining the sense requires concentration and Focus.",
			["dangersense", "enddangersense"], "dangersense"),
		["sensitivity"] = new("Psychic Sensitivity", "Feel nearby magical and psychic activity as ripples in your awareness. Sustain the sense for ongoing warnings, or use senscan to examine a person nearby while sustaining the sense.",
			["sensitivity", "endsensitivity", "senscan <person>"], "sensitivity"),
		["babble"] = new("Babble", "Tangle the words forming in a linked mind, temporarily interfering with the person's speech.",
			["babble <person>"], "babble rival"),
		["coerce"] = new("Somatic Influence", "Alter a linked person's fatigue, hunger or thirst, or press a thought into their mind. This affects bodily sensations or delivers a thought; it does not make the person obey you.",
			["coerce <person> fatigue|refresh|thirst|quench|hunger|full", "coerce <person> thought <text>"], "coerce friend refresh"),
		["projectemotion"] = new("Project Emotion", "Communicate a feeling through a mental link. The recipient senses the feeling you send; this does not force them to adopt it as their own.",
			["projectemotion <person> <feeling>"], "projectemotion friend A quiet sense of reassurance."),
		["project"] = new("Astral Projection", "Let your awareness take astral form, leaving your body behind for a short time. Take care where you leave your body while your attention is elsewhere.",
			["project insignificant"], "project insignificant"),
		["possess"] = new("Possession", "Attempt to take control of another person's body for a short time. The person can resist your intrusion.",
			["possess insignificant <person>"], "possess insignificant rival"),
		["levitate"] = new("Levitation", "Support yourself with unseen force, lifting free of the ground for a short time.",
			["levitate insignificant"], "levitate insignificant"),
		["illusion"] = new("Personal Illusion", "Place a translucent shimmer over your own appearance as you perceive it. This changes your own view; it does not make others see the shimmer.",
			["illusion insignificant"], "illusion insignificant"),
	};

	public static string Help(string verb, string schoolVerb, bool advanced = false)
	{
		var entry = Powers[verb];
		var description = entry.Description.Replace("{range}", advanced ? "shard" : "zone");
		var syntax = entry.Syntax.Select(x => "#3" + (x.StartsWith("!") ? x[1..] : $"{schoolVerb} {x}") + "#0");
		return $"{description}\n\nSyntax:\n{string.Join("\n", syntax)}\n\nFor example: #3{schoolVerb} {entry.Example}#0.";
	}
}
