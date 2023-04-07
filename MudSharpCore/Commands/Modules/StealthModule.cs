using System;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Commands.Modules;

public class StealthModule : Module<ICharacter>
{
	private StealthModule()
		: base("Stealth")
	{
		IsNecessary = true;
	}

	public static StealthModule Instance { get; } = new();

	[PlayerCommand("Hide", "hide")]
	[DelayBlock("general", "You must first stop {0} before you can hide anywhere.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoMovementCommand]
	protected static void Hide(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		// Standard Hide
		if (ss.IsFinished)
		{
			if (actor.AffectedBy<IHideEffect>())
			{
				actor.OutputHandler.Send("You are already hidden.");
				return;
			}

			if (!actor.CanMove())
			{
				actor.OutputHandler.Send(actor.WhyCannotMove());
				return;
			}

			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ begin|begins looking for a hiding spot.", actor), flags: OutputFlags.SuppressObscured));
			actor.AddEffect(new SimpleCharacterAction(actor, character =>
				{
					actor.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ settle|settles down into a hiding spot.", actor),
							flags: OutputFlags.SuppressObscured));
					var result = actor.Gameworld.GetCheck(CheckType.HideCheck)
					                  .Check(actor, actor.Location.Terrain(actor).HideDifficulty);
					actor.AddEffect(new HideInvis(actor, result.TargetNumber));
					foreach (var witness in actor.Location.Characters.Except(actor))
					{
						witness.AddEffect(new SawHider(witness, actor), TimeSpan.FromSeconds(300));
					}
				}, "looking for a hiding spot", new[] { "general", "movement" }, "looking for a hiding spot"),
				TimeSpan.FromSeconds(15));
			return;
		}

		// Hide from a specific target
		if (ss.Pop().Equals("from", StringComparison.InvariantCultureIgnoreCase))
		{
			actor.OutputHandler.Send("Coming soon.");
			return;
		}

		// Hide an item
		var targetText = ss.Last;
		var target = actor.TargetLocalOrHeldItem(targetText);
		if (target == null)
		{
			actor.OutputHandler.Send("You do not have or see anything like that to hide.");
			return;
		}

		if (target.AffectedBy<IItemHiddenEffect>())
		{
			actor.Send("{0} is already hidden.\n{1}", target.HowSeen(actor, true),
				"Hint: If you pick up the item, it will clear the hidden flag".Colour(Telnet.Yellow));
			return;
		}

		if (!target.IsItemType<IHoldable>() && !actor.IsAdministrator())
		{
			actor.Send($"You can only hide items that are holdable, and {target.HowSeen(actor)} is not holdable.");
			return;
		}

		if (target.InInventoryOf == actor.Body)
		{
			actor.Body.Take(target);
			target.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(target, true);
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote("@ begin|begins hiding $0.", actor, target), flags: OutputFlags.SuppressObscured));
		actor.AddEffect(new CharacterActionWithTarget(actor, target,
			character =>
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ finish|finishes hiding $0.", actor, target),
					flags: OutputFlags.SuppressObscured));
				var result = actor.Gameworld.GetCheck(CheckType.HideItemCheck)
				                  .Check(actor, actor.Location.Terrain(actor).HideDifficulty);
				var effect = new ItemHidden(target, result.TargetNumber);
				target.AddEffect(effect);
				foreach (var witness in actor.Location.Characters)
				{
					witness.AddEffect(new SawHiddenItem(witness, target), TimeSpan.FromSeconds(300));
					effect.OriginalWitnesses.Add(witness.Id);
				}
			},
			"hiding an item",
			"@ stop|stops hiding $1",
			"@ cannot move because #0 are|is hiding $1",
			new string[] { "general", "movement" },
			"hiding an item"
		), TimeSpan.FromSeconds(10));
	}

	[PlayerCommand("Reveal", "reveal", "unhide")]
	[DelayBlock("general", "You must first stop {0} before you can reveal yourself.")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Reveal(ICharacter actor, string command)
	{
		if (!actor.AffectedBy<IHideEffect>())
		{
			actor.OutputHandler.Send("You are not hidden, and so do not need to reveal yourself.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());

		var emoteText = ss.PopParentheses();
		MixedEmoteOutput output;
		Action successAction;

		if (ss.IsFinished &&
		    "reveal".StartsWith(new StringStack(command).Pop(), StringComparison.InvariantCultureIgnoreCase))
		{
			successAction = () => actor.RemoveAllEffects(x => x.IsEffectType<IHideEffect>());
			output = new MixedEmoteOutput(new Emote("@ reveal|reveals %0", actor, actor),
				flags: OutputFlags.SuppressObscured);
		}
		else
		{
			var targetText = ss.Pop();
			var target = actor.TargetActor(targetText);
			if (target == null)
			{
				actor.OutputHandler.Send("You do not see any such person to whom to reveal yourself.");
				return;
			}

			successAction = () => { target.AddEffect(new SawHider(target, actor), TimeSpan.FromSeconds(600)); };

			output = new MixedEmoteOutput(new Emote("@ reveal|reveals %0 to $1", actor, actor, target),
				flags: OutputFlags.SuppressObscured);

			emoteText = ss.PopParentheses();
		}

		if (!string.IsNullOrWhiteSpace(emoteText))
		{
			var emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}

			output.Append(emote);
		}

		successAction();
		actor.OutputHandler.Handle(output);
	}

	[PlayerCommand("Sneak", "sneak")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoMovementCommand]
	protected static void Sneak(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var subtleSneak = false;
		if (!ss.IsFinished)
		{
			if (ss.Pop().EqualTo("subtle"))
			{
				subtleSneak = true;
			}
			else
			{
				actor.Send("You can either use the syntax SNEAK or SNEAK SUBTLE.");
				return;
			}
		}

		if (actor.AffectedBy<ISneakEffect>())
		{
			if (subtleSneak && !actor.EffectsOfType<ISneakEffect>().First().Subtle)
			{
				actor.RemoveAllEffects(x => x.IsEffectType<ISneakEffect>());
				actor.AddEffect(new SneakSubtle(actor));
				actor.OutputHandler.Send("You will now try to subtly sneak when you move about.");
				return;
			}

			actor.RemoveAllEffects(x => x.IsEffectType<ISneakEffect>());
			actor.OutputHandler.Send("You will no longer try to sneak when you move about.");
			return;
		}

		if (subtleSneak)
		{
			actor.AddEffect(new SneakSubtle(actor));
			actor.OutputHandler.Send("You will now try to subtly sneak when you move about.");
		}
		else
		{
			actor.AddEffect(new Sneak(actor));
			actor.OutputHandler.Send("You will now try to sneak when you move about.");
		}
	}

	[PlayerCommand("Palm", "palm")]
	[DelayBlock("general", "You must first stop {0} before you can palm anything.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Palm(ICharacter actor, string command)
	{
		if (actor.Combat != null)
		{
			actor.Send("You are too busy fighting to worry about that!");
			return;
		}

		actor.Send("Coming soon.");
	}

	[PlayerCommand("Search", "search")]
	[DelayBlock("general", "You must first stop {0} before you can search.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	protected static void Search(ICharacter actor, string command)
	{
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins searching the area.", actor)));
		actor.AddEffect(new Searching(actor), Searching.EffectDuration);
	}
}