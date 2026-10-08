#nullable enable

using MudSharp.Construction;

namespace MudSharp.NPC.AI;

public abstract partial class CreatureAIBase
{
	protected bool BuildingCommandHunting(ICharacter actor, StringStack command)
	{
		var section = command.PopForSwitch();
		var option = command.PopForSwitch();
		var value = command.SafeRemainingArgument;
		bool Bad(string message) { actor.OutputHandler.Send(message); return false; }
		bool Number(out double number) => double.TryParse(value, out number) && double.IsFinite(number);
		bool Positive(out double number) => Number(out number) && number > 0 && number <= 86400;
		if (section == "hunting")
		{
			switch (option)
			{
				case "on": Hunting.Enabled = true; break;
				case "off": Hunting.Enabled = false; break;
				case "opening":
					if (!Enum.TryParse<AnimalHuntOpening>(value, true, out var opening) || !Enum.IsDefined(opening)) return Bad("Use Direct, Ambush or TrapWait.");
					Hunting.Opening = opening; break;
				case "followup":
					if (!Enum.TryParse<AnimalHuntFollowup>(value, true, out var followup) || !Enum.IsDefined(followup)) return Bad("Use Fight, Extract or VenomWithdrawal.");
					Hunting.Followup = followup; break;
				case "layer":
					if (value.EqualTo("any")) { Hunting.PreferredLayer = null; break; }
					if (!value.TryParseEnum<RoomLayer>(out var layer)) return Bad("Specify a room layer, or any.");
					Hunting.PreferredLayer = layer; break;
				case "opportunity":
					if (!value.EqualTo("on") && !value.EqualTo("off")) return Bad("Use on or off.");
					Hunting.Opportunistic = value.EqualTo("on"); break;
				case "range":
					if (!int.TryParse(value, out var range) || range < 1 || range > 20) return Bad("Use a pursuit radius from 1 to 20 rooms.");
					Hunting.PursuitRange = range; break;
				case "timeout":
				case "lost":
					if (!Positive(out var seconds)) return Bad("Specify seconds greater than zero and at most 86400.");
					if (option == "lost") Hunting.LostTimeout = TimeSpan.FromSeconds(seconds);
					else Hunting.PursuitTimeout = TimeSpan.FromSeconds(seconds);
					break;
				default: return Bad("Use hunting on|off, opening <Direct|Ambush|TrapWait>, followup <Fight|Extract|VenomWithdrawal>, layer <layer|any>, opportunity <on|off>, range <rooms>, timeout <seconds>, or lost <seconds>.");
			}
		}
		else if (section == "assessment")
		{
			switch (option)
			{
				case "cautious": case "balanced": case "bold": Hunting.SetAssessmentProfile(option); break;
				case "engage": case "abandon": case "starvation": case "confidence":
					if (!Number(out var number) || number > 100 || number < (option == "confidence" ? -100 : 0)) return Bad("Use a value from 0 to 100 (confidence also accepts -100 to 0).");
					if (option == "engage") { Hunting.EngageThreshold = number; Hunting.AbandonThreshold = Math.Min(number, Hunting.AbandonThreshold); }
					if (option == "abandon")
					{
						if (number > Hunting.EngageThreshold) return Bad("The abandon threshold cannot exceed the engage threshold.");
						Hunting.AbandonThreshold = number;
					}
					if (option == "starvation") Hunting.StarvationAdjustment = number;
					if (option == "confidence") Hunting.ConfidenceBias = number;
					break;
				case "weight":
					var key = command.PopForSwitch();
					value = command.SafeRemainingArgument;
					if (!Hunting.Weights.ContainsKey(key) || !Number(out var weight) || Math.Abs(weight) > 1000) return Bad("Specify a weight from -1000 to 1000 for size, injury, vulnerability, tactic, support, weapons, owninjury or fatigue.");
					Hunting.Weights[key] = weight; break;
				default: return Bad("Use assessment cautious|balanced|bold, engage|abandon|starvation|confidence <number>, or weight <factor> <number>.");
			}
		}
		else
		{
			switch (option)
			{
				case "people":
					if (!Enum.TryParse<AnimalPeoplePreyPolicy>(value, true, out var policy) || !Enum.IsDefined(policy)) return Bad("Use Never, Desperate or Eligible. Desperate requires starvation.");
					Hunting.People = policy; break;
				case "selection":
					if (!Enum.TryParse<AnimalPreySelection>(value, true, out var selection) || !Enum.IsDefined(selection)) return Bad("Use Safest, Nearest or LargestManageable.");
					Hunting.Selection = selection; break;
				case "include": case "exclude": case "prefer":
					if (value.EqualTo("clear"))
					{
						if (option == "include") Hunting.IncludedRaces.Clear();
						if (option == "exclude") Hunting.ExcludedRaces.Clear();
						if (option == "prefer") Hunting.PreferredRaces.Clear();
						break;
					}
					var race = Gameworld.Races.GetByIdOrName(command.PopSpeech());
					if (race is null) return Bad("Specify a race (quote names containing spaces), or clear. Descendants are included.");
					if (option == "prefer")
					{
						value = command.SafeRemainingArgument;
						if (!Number(out var preference)) return Bad("Specify a finite preference score after the race.");
						Hunting.PreferredRaces[race.Id] = preference;
					}
					else
					{
						var set = option == "include" ? Hunting.IncludedRaces : Hunting.ExcludedRaces;
						if (!set.Remove(race.Id)) set.Add(race.Id);
					}
					break;
				case "sizes":
					if (value.EqualTo("any")) { Hunting.MinimumSizeDifference = null; Hunting.MaximumSizeDifference = null; break; }
					if (!int.TryParse(command.PopSpeech(), out var min) || !int.TryParse(command.PopSpeech(), out var max) || min > max) return Bad("Specify minimum and maximum apparent size difference (prey minus hunter), or any.");
					Hunting.MinimumSizeDifference = min; Hunting.MaximumSizeDifference = max; break;
				case "eligibility": case "classification": case "preference":
					long progId = 0;
					if (!value.EqualTo("clear"))
					{
						var prog = new ProgLookupFromBuilderInput(Gameworld, actor, value,
							option == "preference" ? ProgVariableTypes.Number : ProgVariableTypes.Boolean,
							[ProgVariableTypes.Character, ProgVariableTypes.Character]).LookupProg();
						if (prog is null) return false;
						progId = prog.Id;
					}
					if (option == "eligibility") Hunting.EligibilityProgId = progId;
					if (option == "classification") Hunting.ClassificationProgId = progId;
					if (option == "preference") Hunting.PreferenceProgId = progId;
					break;
				default: return Bad("Use prey people <Never|Desperate|Eligible>, selection <Safest|Nearest|LargestManageable>, include|exclude <race|clear>, prefer <race> <score>, sizes <min> <max>|any, or eligibility|classification|preference <prog|clear>. Progs take hunter and prey; classification returns true for people.");
			}
		}
		Changed = true;
		actor.OutputHandler.Send(ShowHunting(actor));
		return true;
	}
}
