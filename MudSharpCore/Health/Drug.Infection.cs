#nullable enable
using System;
using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Health;

public partial class Drug
{
	private bool BuildingCommandInfection(ICharacter actor, StringStack command)
	{
		if (!DrugTypeMulipliers.ContainsKey(DrugType.Infection))
		{
			actor.Send("First add an Infection effect intensity.");
			return false;
		}
		var info = AdditionalInfoFor<DrugInfectionAdditionalInfo>(DrugType.Infection);
		var option = command.PopForSwitch();
		var argument = command.SafeRemainingArgument;
		if (option == "name" && !string.IsNullOrWhiteSpace(argument)) info.Name = argument;
		else if (option == "symptoms")
		{
			var profile = Gameworld.Drugs.GetByIdOrName(argument);
			if (profile is null || profile.Id == Id || profile.DrugTypes.Contains(DrugType.Infection))
			{
				actor.Send("Choose a separate symptom drug profile without an Infection effect.");
				return false;
			}
			info.SymptomDrugId = profile.Id;
		}
		else if (option == "recovery" && Enum.TryParse<InfectionRecoveryMode>(argument, true, out var mode) && Enum.IsDefined(mode)) info.RecoveryMode = mode;
		else if (option == "difficulty" && argument.TryParseEnum<Difficulty>(out var difficulty)) info.VirulenceDifficulty = (int)difficulty;
		else if (option == "symptomspread" && bool.TryParse(argument, out var symptomSpread)) info.SpreadOnSymptomEpisode = symptomSpread;
		else if (option == "cure" && bool.TryParse(argument, out var cure)) info.CureWithDrug = cure;
		else if (double.TryParse(argument, NumberStyles.Float, actor.Account.Culture, out var number) && double.IsFinite(number))
		{
			switch (option)
			{
				case "duration" when number > 0 && number < TimeSpan.MaxValue.TotalSeconds: info.DurationSeconds = number; break;
				case "severity" when number > 0: info.InitialIntensity = number; break;
				case "symptomscale" when number >= 0: info.SymptomIntensityMultiplier = number; break;
				case "virulence" when number > 0: info.VirulenceMultiplier = number; break;
				case "interval" when number >= 1 && number < TimeSpan.MaxValue.TotalSeconds: info.SpreadIntervalSeconds = number; break;
				case "exposure" when number is >= 0 and <= 1: info.ExposureChance = number; break;
				case "spread" when number is >= 0 and <= 1: info.SpreadChance = number; break;
				case "episode" when number is >= 0 and <= 1: info.SpreadEpisodeChance = number; break;
				default: return InfectionHelp(actor);
			}
		}
		else return InfectionHelp(actor);
		Changed = true;
		actor.Send(DescribeInfection(actor));
		return true;
	}

	private static bool InfectionHelp(ICharacter actor)
	{
		actor.Send("Use type infection name|symptoms|recovery|difficulty|cure|duration|severity|symptomscale|virulence|interval|exposure|spread|episode|symptomspread <value>. Durations are real seconds; probabilities are from zero to one.");
		return false;
	}

	private static string DescribeInfectionDuration(double seconds, ICharacter voyeur) =>
		double.IsFinite(seconds) && seconds >= 0 && seconds < TimeSpan.MaxValue.TotalSeconds
			? TimeSpan.FromSeconds(seconds).Describe(voyeur) : "invalid".ColourError();

	private string DescribeInfection(ICharacter voyeur)
	{
		var info = AdditionalInfoFor<DrugInfectionAdditionalInfo>(DrugType.Infection);
		return $"Illness: {info.Name.ColourName()}; symptoms: {(Gameworld.Drugs.Get(info.SymptomDrugId)?.Name ?? "not configured").ColourName()}\n" +
			$"Recovery: {info.RecoveryMode.DescribeEnum().ColourValue()}; duration: {DescribeInfectionDuration(info.DurationSeconds, voyeur)}; severity: {info.InitialIntensity.ToString("N2", voyeur)}; symptom scale: {info.SymptomIntensityMultiplier.ToString("N2", voyeur)}\n" +
			$"Virulence: {((Difficulty)info.VirulenceDifficulty).DescribeEnum()} x {info.VirulenceMultiplier.ToString("N2", voyeur)}; exposure probability: {info.ExposureChance.ToString("P4", voyeur)}\n" +
			$"Spread: {info.SpreadChance.ToString("P4", voyeur)} per other room occupant; symptom-triggered: {info.SpreadOnSymptomEpisode.ToColouredString()}; episode probability {info.SpreadEpisodeChance.ToString("P4", voyeur)}, every {DescribeInfectionDuration(info.SpreadIntervalSeconds, voyeur)}; cure with initiating drug: {info.CureWithDrug.ToColouredString()}; valid: {info.IsValid.ToColouredString()}";
	}
}
