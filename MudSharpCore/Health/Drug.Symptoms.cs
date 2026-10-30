using System;
using System.Globalization;
using System.Linq;
using System.Text;
using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Framework;

#nullable enable
namespace MudSharp.Health;

public partial class Drug
{
	private bool BuildingCommandSymptoms(ICharacter actor, StringStack command)
	{
		if (!DrugTypeMulipliers.ContainsKey(DrugType.Symptoms))
		{
			actor.OutputHandler.Send("Set a Symptoms intensity before configuring its stages.");
			return false;
		}
		var info = AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		var option = command.PopForSwitch();
		if (option == "immune")
		{
			var race = Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
			if (race is null) return false;
			if (!info.ImmuneRaceIds.Remove(race.Id)) info.ImmuneRaceIds.Add(race.Id);
			Changed = true;
			actor.Send("The symptom racial immunity list was updated.");
			return true;
		}
		if (option is "onsetclear" or "expiryclear")
		{
			if (option == "onsetclear") info.OnsetTraitChanges.Clear(); else info.ExpiryTraitChanges.Clear();
		}
		else if (option == "perdose")
		{
			if (!bool.TryParse(command.SafeRemainingArgument, out var enabled))
			{
				actor.OutputHandler.Send("Specify true for onset consequences on each administered dose, or false for once per active exposure.");
				return false;
			}
			info.OnsetPerDose = enabled;
		}
		else if (option == "interval")
		{
			if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor.Account.Culture, out var interval) ||
				!double.IsFinite(interval) || interval < 1.0 || interval > 86400.0)
			{
				actor.OutputHandler.Send("Specify a symptom trial interval from one to 86,400 real seconds.");
				return false;
			}
			info.TickIntervalSeconds = interval;
		}
		else if (option is "onset" or "expiry")
		{
			var trait = Gameworld.Traits.GetByIdOrName(command.PopSpeech());
			var chance = command.PopSpeech();
			var floor = 0.0;
			if (trait is null || !IsValidSymptomFormula(chance, info.InitialStage) ||
				!double.TryParse(command.PopSpeech(), NumberStyles.Float, actor.Account.Culture, out var amount) || !double.IsFinite(amount) ||
				(!command.IsFinished && (!double.TryParse(command.PopSpeech(), NumberStyles.Float, actor.Account.Culture, out floor) || !double.IsFinite(floor))) ||
				!command.IsFinished)
			{
				actor.OutputHandler.Send("Use onset|expiry <trait> <quoted chance formula> <permanent amount> [minimum raw value].");
				return false;
			}
			var changes = option == "onset" ? info.OnsetTraitChanges : info.ExpiryTraitChanges;
			changes.RemoveAll(x => x.TraitId == trait.Id);
			if (amount != 0.0) changes.Add(new DrugPermanentTraitChange
			{ TraitId = trait.Id, Chance = chance, Amount = amount, MinimumValue = floor });
		}
		else if (option == "trait")
		{
			if (command.IsFinished) return false;
			var text = command.SafeRemainingArgument;
			var trait = text.EqualTo("none") ? null : Gameworld.Traits.GetByIdOrName(text);
			if (trait is null && !text.EqualTo("none"))
			{
				actor.OutputHandler.Send("Specify a resistance trait or none.");
				return false;
			}
			info.ResistanceTraitId = trait?.Id ?? 0;
		}
		else if (option == "addstage")
		{
			info.Stages.Add(new DrugSymptomStage());
		}
		else
		{
			if (!int.TryParse(command.PopSpeech(), NumberStyles.Integer, actor.Account.Culture, out var index) ||
				index < 0 || index >= info.Stages.Count)
			{
				actor.OutputHandler.Send($"Specify a stage from 0 to {(info.Stages.Count - 1).ToString("N0", actor)}. Use addstage to append one.");
				return false;
			}
			var stage = info.Stages[index];
			var text = command.SafeRemainingArgument;
			var weightDestination = -1;
			if (option == "weight")
			{
				if (!int.TryParse(command.PopSpeech(), out weightDestination) || !stage.NextStages.Contains(weightDestination))
				{
					actor.OutputHandler.Send("Specify one of this stage's transition destinations, then a weight formula.");
					return false;
				}
				text = command.SafeRemainingArgument;
			}
			if (option.StartsWith("outcome", StringComparison.Ordinal)) return BuildingCommandSymptomOutcome(actor, command, stage, option);
			switch (option)
			{
				case "initial":
					if (!command.IsFinished) return false;
					info.InitialStage = index;
					break;
				case "weight": case "chance": case "damage": case "pain": case "stun": case "stamina": case "transition": case "maxhp":
					if (!IsValidSymptomFormula(text, index))
					{
						actor.OutputHandler.Send("Use a valid formula with intensity, resistance, fullness, stamina, sleeping, unconscious, damage and stage as inputs. Chances are 0–1 probabilities per configured symptom trial.");
						return false;
					}
					switch (option)
					{
						case "chance": stage.Chance = text; break;
						case "damage": stage.Damage = text; break;
						case "pain": stage.Pain = text; break;
						case "stun": stage.Stun = text; break;
						case "stamina": stage.Stamina = text; break;
						case "transition": stage.TransitionChance = text; break;
						case "weight": stage.NextStageWeights[weightDestination] = text; break;
						case "maxhp": stage.MaximumHitPointsBonus = text; break;
					}
					break;
				case "next":
					var next = new System.Collections.Generic.List<int>();
					while (!command.IsFinished)
					{
						if (!int.TryParse(command.PopSpeech(), out var destination) || destination < 0 || destination >= info.Stages.Count)
						{
							actor.OutputHandler.Send("Every transition destination must be an existing zero-based stage.");
							return false;
						}
						next.Add(destination);
					}
					stage.NextStages = next;
					foreach (var key in stage.NextStageWeights.Keys.Where(x => !next.Contains(x)).ToArray()) stage.NextStageWeights.Remove(key);
					break;
				case "awake": case "sleepfatal": case "sleepatexhaustion": case "blockrecovery": case "unconsciousrecoveryonly": case "ceasecombat": case "capacityonly":
					if (!bool.TryParse(text, out var enabled))
					{
						actor.OutputHandler.Send("Specify true or false.");
						return false;
					}
					switch (option)
					{
						case "awake": stage.AwakeOnly = enabled; break;
						case "sleepfatal": stage.KillSleeping = enabled; break;
						case "sleepatexhaustion": stage.SleepAtZeroStamina = enabled; break;
						case "blockrecovery": stage.PreventNaturalStunRecovery = enabled; break;
						case "unconsciousrecoveryonly": stage.RecoveryBlockOnlyWhenUnconscious = enabled; break;
						case "ceasecombat": stage.CeaseCombatOnParalysis = enabled; break;
						case "capacityonly": stage.MaximumHitPointsCapacityOnly = enabled; break;
					}
					break;
				case "maxhpseconds":
					if (!double.TryParse(text, NumberStyles.Float, actor.Account.Culture, out var seconds) || !double.IsFinite(seconds) || seconds <= 0 || seconds > TimeSpan.MaxValue.TotalSeconds) return false;
					stage.MaximumHitPointsDurationSeconds = seconds;
					break;
				case "damagetype":
					if (!text.TryParseEnum<DamageType>(out var damageType) || !Enum.IsDefined(damageType))
					{
						actor.OutputHandler.Send("Specify a valid native damage type.");
						return false;
					}
					stage.DamageType = damageType;
					break;
				case "echo":
					if (string.IsNullOrWhiteSpace(text)) return false;
					if (text.EqualTo("clear")) stage.Echoes.Clear(); else stage.Echoes.Add(text);
					break;
				default:
					actor.OutputHandler.Send("Use symptoms trait <trait|none>, immune <race>, interval <seconds>, perdose <true|false>, onset|expiry <trait> <quoted chance> <permanent amount> [minimum], onsetclear|expiryclear, initial <stage>, addstage, weight <stage> <destination> <formula>, or <chance|damage|pain|stun|stamina|transition|next|awake|sleepfatal|damagetype|echo|maxhp|maxhpseconds|capacityonly|sleepatexhaustion|blockrecovery|unconsciousrecoveryonly|ceasecombat> <stage> <value>.");
					return false;
			}
		}
		Changed = true;
		actor.OutputHandler.Send("The drug's symptom profile was updated.");
		return true;
	}

	private static bool IsValidSymptomFormula(string text, int stage)
	{
		var expression = new Expression(text);
		return !expression.HasErrors() && !expression.ParameterNames.Except(
			new[] { "intensity", "resistance", "fullness", "stamina", "sleeping", "unconscious", "stage", "damage" }, StringComparer.OrdinalIgnoreCase).Any() &&
			expression.TryEvaluateDoubleWith(new System.Collections.Generic.Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
			{ ["intensity"] = 1.0, ["resistance"] = 10.0, ["fullness"] = 1.0, ["stamina"] = 100.0, ["sleeping"] = 0.0, ["unconscious"] = 0.0, ["stage"] = stage, ["damage"] = 1.0 }, out _, out _);
	}

	private string DescribeSymptomStages(IPerceiver voyeur)
	{
		var info = AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		var sb = new StringBuilder($"Resistance: {Gameworld.Traits.Get(info.ResistanceTraitId)?.Name ?? "none"}; initial stage {info.InitialStage.ToString("N0", voyeur)}\n");
		sb.AppendLine($"Trial interval: {TimeSpan.FromSeconds(info.TickIntervalSeconds).Describe(voyeur)}; onset per dose: {info.OnsetPerDose.ToColouredString()}");
		foreach (var (phase, changes) in new[] { ("Onset", info.OnsetTraitChanges), ("Natural expiry", info.ExpiryTraitChanges) })
		foreach (var change in changes)
			sb.AppendLine($"  {phase}: {Gameworld.Traits.Get(change.TraitId)?.Name ?? "unknown trait"}, chance {change.Chance.ColourValue()}, permanent {change.Amount.ToString("N2", voyeur).ColourValue()}, minimum {change.MinimumValue.ToString("N2", voyeur).ColourValue()}");
		for (var i = 0; i < info.Stages.Count; i++)
		{
			var stage = info.Stages[i];
			sb.AppendLine($"  Stage {i.ToString("N0", voyeur)}: chance {stage.Chance.ColourValue()}, awake only {stage.AwakeOnly.ToColouredString()}, fatal while sleeping {stage.KillSleeping.ToColouredString()}");
			sb.AppendLine($"    {stage.DamageType.DescribeEnum().ColourName()} damage {stage.Damage.ColourValue()}, pain {stage.Pain.ColourValue()}, stun {stage.Stun.ColourValue()}, stamina loss {stage.Stamina.ColourValue()}");
			sb.AppendLine($"    Timed maximum HP {stage.MaximumHitPointsBonus.ColourValue()} for {stage.MaximumHitPointsDurationSeconds.ToString("N0", voyeur)} seconds; capacity only {stage.MaximumHitPointsCapacityOnly.ToColouredString()}");
			sb.AppendLine($"    Exhaustion sleep {stage.SleepAtZeroStamina.ToColouredString()}, recovery block {stage.PreventNaturalStunRecovery.ToColouredString()}, unconscious only {stage.RecoveryBlockOnlyWhenUnconscious.ToColouredString()}, paralysis combat cessation {stage.CeaseCombatOnParalysis.ToColouredString()}");
			sb.AppendLine($"    Transition {stage.TransitionChance.ColourValue()} to {stage.NextStages.Select(x => x.ToString("N0", voyeur)).ListToCommaSeparatedValues()}; private echoes: {stage.Echoes.Count.ToString("N0", voyeur)}");
			foreach (var (destination, weight) in stage.NextStageWeights) sb.AppendLine($"      Destination {destination.ToString("N0", voyeur)} weight: {weight.ColourValue()}");
			for (var j = 0; j < stage.Outcomes.Count; j++)
			{
				var outcome = stage.Outcomes[j];
				sb.AppendLine($"      Outcome {j.ToString("N0", voyeur)}: weight {outcome.Weight.ToString("N2", voyeur)}, {outcome.Action.DescribeEnum()}, duration {outcome.DurationSeconds.ToString("N0", voyeur)}s, release grapple {outcome.ReleaseGrapple.ToColouredString()}, residue #{outcome.ResidueMaterialId} {outcome.ResidueKilograms.ToString("N3", voyeur)}kg on body/{outcome.ClothingBodypart}; private: {outcome.PrivateEcho}; public: {outcome.RoomEmote}");
			}
			foreach (var echo in stage.Echoes) sb.AppendLine($"      {echo}");
		}
		return sb.ToString();
	}
}
