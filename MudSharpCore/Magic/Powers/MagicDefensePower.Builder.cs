#nullable enable

using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed partial class MagicDefensePower
{
	protected override string SubtypeHelpText => @"
	#3mode <Opposed|Charged|Absorption>#0 - choose the defense mechanism
	#3threat <Weapon|Natural|Ranged|Magic|Control>#0 - toggle an eligible threat flag
	#3damagetype <type>#0 - toggle damage eligibility; an empty set allows all
	#3trait <id|name>#0 / #3difficulty <difficulty>#0 - defense skill and check
	#3charges <number>#0 / #3capacity <amount>#0 / #3stamina <amount>#0
	#3vision <true|false>#0 / #3attackervision <true|false>#0 / #3upright <true|false>#0
	#3hands <number>#0 / #3eligibility <prog|none>#0 - additional reaction requirements
	#3beginverb <verb>#0 / #3endverb <verb>#0
	#3echo <begin|end|success|fail> <text>#0
	Reaction resource costs use the #3reaction#0 verb in the ordinary cost commands.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PeekSpeech().ToLowerInvariant();
		if (option is not ("mode" or "threat" or "damagetype" or "trait" or "difficulty" or "charges" or "capacity" or "stamina" or "vision" or "attackervision" or "upright" or "hands" or "eligibility" or "beginverb" or "endverb" or "echo")) return base.BuildingCommand(actor, command);
		command.PopSpeech();
		var value = command.SafeRemainingArgument;
		switch (option)
		{
			case "mode":
				if (!Enum.TryParse<MagicDefenseMode>(value, true, out var mode) || !Enum.IsDefined(mode)) return Invalid(actor);
				DefenseMode = mode; break;
			case "threat":
				if (!Enum.TryParse<MagicDefenseThreat>(value, true, out var threat) || !Enum.IsDefined(threat) || threat == MagicDefenseThreat.None) return Invalid(actor);
				Threats ^= threat; break;
			case "damagetype":
				if (!Enum.TryParse<DamageType>(value, true, out var damage) || !Enum.IsDefined(damage)) return Invalid(actor);
				if (!DamageTypes.Remove(damage)) DamageTypes.Add(damage); break;
			case "trait":
				var trait = Gameworld.Traits.GetByIdOrName(value);
				if (trait is null) return Invalid(actor);
				DefenseTrait = trait; break;
			case "difficulty":
				if (!Enum.TryParse<Difficulty>(value, true, out var difficulty) || !Enum.IsDefined(difficulty)) return Invalid(actor);
				DefenseDifficulty = difficulty; break;
			case "charges": case "hands":
				if (!int.TryParse(value, out var count) || count < (option == "charges" ? 1 : 0)) return Invalid(actor);
				if (option == "charges") MaximumCharges = count; else FreeHands = count;
				break;
			case "capacity": case "stamina":
				if (!double.TryParse(value, out var amount) || !double.IsFinite(amount) || amount < 0 || option == "capacity" && amount == 0) return Invalid(actor);
				if (option == "capacity") MaximumCapacity = amount; else ReactionStamina = amount;
				break;
			case "vision": case "attackervision": case "upright":
				if (!bool.TryParse(value, out var enabled)) return Invalid(actor);
				if (option == "vision") RequiresVision = enabled;
				else if (option == "attackervision") RequiresAttackerVision = enabled;
				else RequiresUpright = enabled;
				break;
			case "eligibility":
				if (value.EqualTo("none")) { EligibilityProg = null; break; }
				var prog = new ProgLookupFromBuilderInput(actor, value, ProgVariableTypes.Boolean,
					[ProgVariableTypes.Character, ProgVariableTypes.Character]).LookupProg();
				if (prog is null) return false;
				EligibilityProg = prog; break;
			case "beginverb": case "endverb":
				if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsWhiteSpace) || value.EqualTo("reaction") ||
				    value.EqualTo(option == "beginverb" ? EndVerb : BeginVerb)) return Invalid(actor);
				if (option == "beginverb") { InvocationCosts[value] = InvocationCosts[BeginVerb]; InvocationCosts.Remove(BeginVerb); BeginVerb = value.ToLowerInvariant(); }
				else EndVerb = value.ToLowerInvariant();
				break;
			case "echo":
				var field = command.PopSpeech().ToLowerInvariant();
				var text = command.SafeRemainingArgument;
				var participantCount = field is "begin" or "end" ? 1 : 2;
				if (System.Text.RegularExpressions.Regex.Matches(text, @"(?<!!!)(?<!\$)(?<!&)(?<!#)(?<!%)[!$&#%](?<index>\d+)")
					.Any(match => !int.TryParse(match.Groups["index"].Value, out var index) || index >= participantCount))
				{
					actor.Send("That echo refers to a participant unavailable in this event.");
					return false;
				}
				Emote emote;
				try
				{
					emote = field is "begin" or "end" ? new Emote(text, actor, actor) : new Emote(text, actor, actor, actor);
				}
				catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
				{
					actor.Send("That echo refers to a participant unavailable in this event.");
					return false;
				}
				if (!emote.Valid) { actor.Send(emote.ErrorMessage); return false; }
				switch (field)
				{
					case "begin": BeginEmote = text; break;
					case "end": EndEmote = text; break;
					case "success": SuccessEmote = text; break;
					case "fail": FailEmote = text; break;
					default: return Invalid(actor);
				}
				break;
		}
		Changed = true;
		actor.Send("The defense power has been updated.");
		return true;
	}

	private static bool Invalid(ICharacter actor) { actor.Send("That is not a valid value. See the power's builder help for syntax."); return false; }
}
