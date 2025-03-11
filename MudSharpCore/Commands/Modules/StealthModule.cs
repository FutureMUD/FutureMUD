using System;
using System.Linq;
using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using Org.BouncyCastle.Asn1.X509;

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
	[HelpInfo("hide", @"The #3hide#0 command is used to perform two functions; one is to begin hiding, trying to keep out of sight of everyone. The second is to hide an item.

The syntax is as follows:

	#3hide#0 - begin hiding yourself
	#3hide <item>#0 - hide an item

Note - anyone in the room at the time you hide yourself or an item will be able to see through your stealth as long as you all remain there", AutoHelp.HelpArg)]
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

					actor.HandleEvent(EventType.CharacterHidden, actor);
					foreach (var witness in actor.Location.EventHandlers)
					{
						witness.HandleEvent(EventType.CharacterHidesWitness, actor, witness);
					}
				}, "looking for a hiding spot", new[] { "general", "movement" }, "looking for a hiding spot"),
				TimeSpan.FromSeconds(15));
			return;
		}

		// Hide an item
		var targetText = ss.SafeRemainingArgument;
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

	[PlayerCommand("Unhide", "unhide")]
	[DelayBlock("general", "You must first stop {0} before you can reveal yourself.")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("unhide", @"The #3unhide#0 command is used to stop hiding. This command simply toggles your hiding off.

The syntax is as follows:

	#3unhide#0 - reveal yourself to everyone in the room", AutoHelp.HelpArg)]
	protected static void Unhide(ICharacter actor, string command)
	{
		if (!actor.AffectedBy<IHideEffect>())
		{
			actor.OutputHandler.Send("You are not hidden, and so do not need to stop hiding.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		var emoteText = ss.PopParentheses();
		var output = new MixedEmoteOutput(new Emote("@ reveal|reveals %0", actor, actor),
			flags: OutputFlags.SuppressObscured);
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

		actor.RemoveAllEffects(x => x.IsEffectType<IHideEffect>());
		actor.OutputHandler.Handle(output);
	}

	[PlayerCommand("Reveal", "reveal")]
	[DelayBlock("general", "You must first stop {0} before you can reveal yourself.")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("reveal", @"The #3reveal#0 command is used to reveal yourself to people in your room when you are hiding, without stopping the fact that you are hiding.

People to whom you reveal yourself will be able to see you while you remain hidden and in this location.

The syntax is as follows:

	#3reveal#0 - reveal yourself to everyone in the room
	#3reveal <target>#0 - reveal yourself only to a particular person", AutoHelp.HelpArg)]
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

		if (ss.IsFinished)
		{
			successAction = () =>
			{
				foreach (var person in actor.Location.LayerCharacters(actor.RoomLayer).Except(actor).AsEnumerable())
				{
					person.AddEffect(new SawHider(person, actor), TimeSpan.FromSeconds(600));
				}
			};
			output = new MixedEmoteOutput(new Emote("@ reveal|reveals %0 to everyone present", actor, actor),
				flags: OutputFlags.SuppressObscured);
		}
		else
		{
			var targetText = ss.PopSpeech();
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
	[HelpInfo("sneak", @"The #3sneak#0 command is used to toggle whether you want to sneak when you move, and try to hide the fact that you have moved to others around you.

If someone notices you sneaking it is obvious that you were trying to sneak, unless you use the subtle sneak option; if they notice you with subtle sneaking, they will just see a regular movement message. However, subtle sneaking is easier to notice on the whole.

The syntax is as follows:

	#3sneak#0 - toggles sneaking on or off
	#3sneak subtle#0 - toggles subtle sneaking on or off", AutoHelp.HelpArg)]
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
	[HelpInfo("search", @"The #3search#0 command is used to search your surroundings for hidden things and people. This is a continuous action, and you will do it until you cancel it with the #3stop#0 command.

The syntax is simply #3search#0.", AutoHelp.HelpArg)]
	protected static void Search(ICharacter actor, string command)
	{
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins searching the area.", actor)));
		actor.AddEffect(new Searching(actor), Searching.EffectDuration);
	}
}