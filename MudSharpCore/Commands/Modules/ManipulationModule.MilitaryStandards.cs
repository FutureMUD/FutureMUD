#nullable enable

using MudSharp.Commands.Trees;

namespace MudSharp.Commands.Modules;

internal partial class ManipulationModule
{
	private const string StandardHelp = @"The #3standard#0 command interacts with a military standard.

	#3standard inspect <standard>#0
	#3standard recognise <standard>#0
	#3standard plant <standard> [(emote)]#0
	#3standard takeup <standard> [(emote)]#0
	#3standard signal <standard> <pattern> [(emote)]#0

Administrators may use #3standard set#0 and #3standard reset#0 to configure individual copies.";

	[PlayerCommand("Standard", "standard")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("standard", StandardHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void MilitaryStandard(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var subcommand = ss.PopSpeech().ToLowerInvariant();
		switch (subcommand)
		{
			case "inspect":
				StandardInspect(actor, ss);
				return;
			case "recognise":
			case "recognize":
				StandardRecognise(actor, ss);
				return;
			case "plant":
				StandardPlant(actor, ss);
				return;
			case "takeup":
			case "take-up":
			case "raise":
				StandardTakeUp(actor, ss);
				return;
			case "signal":
				StandardSignal(actor, ss);
				return;
			case "set":
				StandardSet(actor, ss);
				return;
			case "reset":
				StandardReset(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(StandardHelp.SubstituteANSIColour());
				return;
		}
	}

	private static IMilitaryStandard? GetStandard(ICharacter actor, StringStack ss)
	{
		var item = actor.TargetItem(ss.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("You do not see any military standard like that.");
			return null;
		}

		var standard = item.GetItemType<IMilitaryStandard>();
		if (standard is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a military standard.");
			return null;
		}

		return standard;
	}

	private static void StandardInspect(ICharacter actor, StringStack ss)
	{
		var standard = GetStandard(actor, ss);
		if (standard is null)
		{
			return;
		}

		var knowsIdentity = actor.IsAdministrator() || standard.IsRecognisedBy(actor);
		var identity = knowsIdentity
			? $"{standard.IdentityName.ColourName()} ({standard.IdentityKey})\nDesign: {standard.Design}\n"
			: "Identity: Unknown\n";
		var association = knowsIdentity && standard.AssociationType != MilitaryStandardAssociationType.None
			? $"Association: {standard.AssociationType.DescribeEnum().ColourName()} - {standard.AssociationName.ColourName()} ({standard.AssociationKey})\n"
			: string.Empty;
		actor.OutputHandler.Send(
			$"{identity}{association}Family: {standard.Family.DescribeEnum().ColourName()}\nCustody: {standard.CustodyState.DescribeEnum().ColourName()}\nCaptures: {standard.CaptureCount.ToString("N0", actor).ColourValue()}\nPlanted: {standard.IsPlanted.ToColouredString()}\nSignals: {(standard.SignalPatterns.Any() ? standard.SignalPatterns.ListToString() : "None")}");
	}

	private static void StandardRecognise(ICharacter actor, StringStack ss)
	{
		GetStandard(actor, ss)?.Recognise(actor);
	}

	private static PlayerEmote? ParseOptionalStandardEmote(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			return null;
		}

		var text = ss.PopParentheses();
		if (string.IsNullOrEmpty(text) || !ss.IsFinished)
		{
			actor.OutputHandler.Send(StandardHelp.SubstituteANSIColour());
			return null;
		}

		var emote = new PlayerEmote(text, actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return null;
		}

		return emote;
	}

	private static void StandardPlant(ICharacter actor, StringStack ss)
	{
		var standard = GetStandard(actor, ss);
		if (standard is null)
		{
			return;
		}

		var hadEmote = !ss.IsFinished;
		var emote = ParseOptionalStandardEmote(actor, ss);
		if (hadEmote && emote is null)
		{
			return;
		}

		standard.Plant(actor, emote);
	}

	private static void StandardTakeUp(ICharacter actor, StringStack ss)
	{
		var standard = GetStandard(actor, ss);
		if (standard is null)
		{
			return;
		}

		var hadEmote = !ss.IsFinished;
		var emote = ParseOptionalStandardEmote(actor, ss);
		if (hadEmote && emote is null)
		{
			return;
		}

		standard.TakeUp(actor, emote);
	}

	private static void StandardSignal(ICharacter actor, StringStack ss)
	{
		var standard = GetStandard(actor, ss);
		if (standard is null)
		{
			return;
		}

		var pattern = ss.PopSpeech();
		if (string.IsNullOrWhiteSpace(pattern))
		{
			actor.OutputHandler.Send(
				$"Which signal? Available signals are {standard.SignalPatterns.ListToString()}.");
			return;
		}

		var hadEmote = !ss.IsFinished;
		var emote = ParseOptionalStandardEmote(actor, ss);
		if (hadEmote && emote is null)
		{
			return;
		}

		standard.Signal(actor, pattern, emote);
	}

	private static void StandardSet(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators may configure individual standards.");
			return;
		}

		var standard = GetStandard(actor, ss);
		if (standard is null)
		{
			return;
		}

		var setting = ss.PopSpeech().ToLowerInvariant();
		switch (setting)
		{
			case "identity":
				var key = ss.PopSpeech();
				if (string.IsNullOrWhiteSpace(key) || ss.IsFinished)
				{
					actor.OutputHandler.Send("Specify an identity key and display name.");
					return;
				}
				standard.SetIdentityOverride(key, ss.SafeRemainingArgument);
				break;
			case "design":
				standard.SetDesignOverride(ss.SafeRemainingArgument);
				break;
			case "association":
				if (!ss.PopSpeech().TryParseEnum<MilitaryStandardAssociationType>(out var association))
				{
					actor.OutputHandler.Send("Specify none, unit or ship.");
					return;
				}
				standard.SetAssociationOverride(association, ss.PopSpeech(), ss.SafeRemainingArgument);
				break;
			case "custody":
				if (!ss.SafeRemainingArgument.TryParseEnum<MilitaryStandardCustodyState>(out var custody))
				{
					actor.OutputHandler.Send("Specify unclaimed, friendly or captured.");
					return;
				}
				standard.SetCustody(custody);
				break;
			case "captures":
			case "capturecount":
				if (!int.TryParse(ss.SafeRemainingArgument, out var count) || count < 0)
				{
					actor.OutputHandler.Send("Specify a non-negative capture count.");
					return;
				}
				standard.SetCaptureCount(count);
				break;
			default:
				actor.OutputHandler.Send(
					"Set identity, design, association, custody or captures.");
				return;
		}

		actor.OutputHandler.Send("The standard's instance configuration has been updated.");
	}

	private static void StandardReset(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators may reset individual standards.");
			return;
		}

		var standard = GetStandard(actor, ss);
		if (standard is null)
		{
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "identity":
			case "association":
			case "design":
			case "overrides":
				standard.ResetOverrides();
				break;
			case "custody":
				standard.SetCustody(MilitaryStandardCustodyState.Unclaimed);
				break;
			case "captures":
			case "capturecount":
				standard.ResetCaptureCount();
				break;
			case "all":
				standard.ResetOverrides();
				standard.SetCustody(MilitaryStandardCustodyState.Unclaimed);
				standard.ResetCaptureCount();
				break;
			default:
				actor.OutputHandler.Send("Reset overrides, custody, captures or all.");
				return;
		}

		actor.OutputHandler.Send("The standard's selected instance state has been reset.");
	}
}
