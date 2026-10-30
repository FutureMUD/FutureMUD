using System;
using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

#nullable enable
namespace MudSharp.Health;

public partial class Drug
{
	private bool BuildingCommandSymptomOutcome(ICharacter actor, StringStack command, DrugSymptomStage stage, string option)
	{
		bool Number(string text, out double value) => double.TryParse(text, NumberStyles.Float, actor.Account.Culture, out value) && double.IsFinite(value);
		if (option == "outcomeadd")
		{
			if (!Number(command.PopSpeech(), out var weight) || weight <= 0 || !command.PopSpeech().TryParseEnum<DrugSymptomAction>(out var action) || !Enum.IsDefined(action) || !command.IsFinished)
			{
				actor.Send("Use outcomeadd <stage> <positive weight> <None|Sit|DropReadiedItems|ReleaseGrapple|Babble|Residue>.");
				return false;
			}
			stage.Outcomes.Add(new DrugSymptomOutcome { Weight = weight, Action = action });
			if (action == DrugSymptomAction.Residue) actor.Send("Configure this outcome with outcomeresidue before it can deposit material.");
		}
		else
		{
			if (!int.TryParse(command.PopSpeech(), out var index) || index < 0 || index >= stage.Outcomes.Count)
			{
				actor.Send("Specify an existing zero-based outcome number after the stage number.");
				return false;
			}
			var outcome = stage.Outcomes[index];
			var text = command.SafeRemainingArgument;
			switch (option)
			{
				case "outcomeremove": stage.Outcomes.RemoveAt(index); break;
				case "outcomeecho": outcome.PrivateEcho = text.EqualTo("clear") ? "" : text; break;
				case "outcomeemote":
					if (!text.EqualTo("clear") && !new Emote(text, actor, actor).Valid) { actor.Send("Specify a valid emote using $0 for the affected character, or clear."); return false; }
					outcome.RoomEmote = text.EqualTo("clear") ? "" : text; break;
				case "outcomeseconds":
					if (!Number(text, out var seconds) || seconds <= 0 || seconds > 86400) return false;
					outcome.DurationSeconds = seconds; break;
				case "outcomeweight":
					if (!Number(text, out var weight) || weight <= 0) return false;
					outcome.Weight = weight; break;
				case "outcomerelease":
					if (!bool.TryParse(text, out var release)) return false;
					outcome.ReleaseGrapple = release; break;
				case "outcomeresidue":
					var material = Gameworld.Materials.GetByIdOrName(command.PopSpeech());
					if (material is null || !Number(command.PopSpeech(), out var kilograms) || kilograms <= 0) return false;
					outcome.ResidueMaterialId = material.Id; outcome.ResidueKilograms = kilograms;
					outcome.ClothingBodypart = command.SafeRemainingArgument; break;
				default:
					actor.Send("Use outcomeadd <stage> <weight> <action>, or outcomeremove|outcomeecho|outcomeemote|outcomeseconds|outcomeweight|outcomerelease|outcomeresidue <stage> <outcome> [value]. Residue takes <material> <kilograms> [clothing bodypart name fragment].");
					return false;
			}
		}
		Changed = true;
		actor.Send("The weighted symptom outcome was updated.");
		return true;
	}
}
