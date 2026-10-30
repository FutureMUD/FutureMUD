using System;
using System.Globalization;
using MudSharp.Character;
using MudSharp.Framework;

#nullable enable
namespace MudSharp.Health;

public partial class Drug
{
	private bool BuildingCommandFixedDose(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor.Account.Culture, out var grams) ||
			!double.IsFinite(grams) || grams <= 0.0)
		{
			actor.OutputHandler.Send("Specify a positive finite fixed dose in grams.");
			return false;
		}
		FixedDoseGrams = grams;
		Changed = true;
		actor.OutputHandler.Send($"The configured fixed administration mass is now {grams.ToString("N6", actor).ColourValue()} grams.");
		return true;
	}

	private bool BuildingCommandDoseBehaviour(ICharacter actor, StringStack command)
	{
		if (!Enum.TryParse<DrugDoseBehaviour>(command.SafeRemainingArgument, true, out var behaviour) ||
			(behaviour & ~(DrugDoseBehaviour.FixedDose | DrugDoseBehaviour.IgnoreAdditionalExposure)) != 0 ||
			behaviour.HasFlag(DrugDoseBehaviour.FixedDose) && FixedDoseGrams <= 0.0)
		{
			actor.OutputHandler.Send("Use Normal, FixedDose, IgnoreAdditionalExposure or comma-separated flags. Configure a positive fixeddose before enabling FixedDose.");
			return false;
		}
		DoseBehaviour = behaviour;
		Changed = true;
		actor.OutputHandler.Send($"This drug now uses {behaviour.DescribeEnum().ColourName()} administration behaviour.");
		return true;
	}
}
