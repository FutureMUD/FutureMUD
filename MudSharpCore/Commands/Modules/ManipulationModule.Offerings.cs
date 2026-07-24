using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class ManipulationModule
{
	[PlayerCommand("Libate", "libate")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[HelpInfo("libate", @"The #3libate#0 command ceremonially pours liquid from an open held container onto an offering focus. The liquid is consumed, and the focus may record the offering, run FutureProgs, fire offering events, or return a configured oracle response.

The syntax is:

	#3libate <amount> from <container> at <focus>#0
	#3libate all from <container> at <focus> (<emote>)#0

Use quotes around multi-word container or focus names.", AutoHelp.HelpArg)]
	protected static void Libate(ICharacter actor, string command)
	{
		if (!TryPopArgumentsAndEmote(command.RemoveFirstWord(), out var arguments, out var emoteText))
		{
			actor.OutputHandler.Send($"See {"help libate".ColourCommand()} for the command syntax.");
			return;
		}

		var fromIndex = arguments.FindIndex(x => x.EqualTo("from"));
		var atIndex = arguments.FindIndex(x => x.EqualTo("at"));
		if (fromIndex < 1 || atIndex != fromIndex + 2 || atIndex != arguments.Count - 2)
		{
			actor.OutputHandler.Send(
				$"Use the syntax {"libate <amount> from <container> at <focus>".ColourCommand()}.");
			return;
		}

		var amountText = JoinArguments(arguments.Take(fromIndex));
		var source = actor.TargetHeldItem(arguments[fromIndex + 1]);
		if (source is null)
		{
			actor.OutputHandler.Send("You are not holding any liquid container like that.");
			return;
		}

		var sourceContainer = source.GetItemType<ILiquidContainer>();
		if (sourceContainer is null)
		{
			actor.OutputHandler.Send($"{source.HowSeen(actor, true)} is not a liquid container.");
			return;
		}

		var focusItem = actor.TargetItem(arguments[atIndex + 1]);
		if (focusItem is null)
		{
			actor.OutputHandler.Send("You do not see any offering focus like that.");
			return;
		}

		var receiver = focusItem.GetItemType<IOfferingReceiver>();
		if (receiver is null)
		{
			actor.OutputHandler.Send($"{focusItem.HowSeen(actor, true)} is not something that can receive offerings.");
			return;
		}

		double amount;
		if (amountText.EqualTo("all"))
		{
			amount = sourceContainer.LiquidVolume;
		}
		else
		{
			amount = actor.Gameworld.UnitManager.GetBaseUnits(amountText, UnitType.FluidVolume, out var success);
			if (!success || amount <= 0.0)
			{
				actor.OutputHandler.Send("You must specify a positive fluid volume, or use all.");
				return;
			}
		}

		var (canManipulateSource, sourceError) = actor.CanManipulateItem(source);
		if (!canManipulateSource)
		{
			actor.OutputHandler.Send(sourceError);
			return;
		}

		var (canManipulateFocus, focusError) = actor.CanManipulateItem(focusItem);
		if (!canManipulateFocus)
		{
			actor.OutputHandler.Send(focusError);
			return;
		}

		PlayerEmote? emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		receiver.OfferLiquid(actor, source, amount, emote);
	}
}
