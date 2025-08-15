using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat.Moves;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.RPG.Law;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using static MudSharp.Effects.Concrete.Dragging;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Law;

namespace MudSharp.Commands.Modules;

internal class ManipulationModule : Module<ICharacter>
{
	private static readonly Regex EatRegex =
		new(
			@"^(?:(?:(?<all>all)|(?<bites>\d+))\s+)*(?<yield>yield\s+)*(?<food>[a-z0-9.]+)(?: from (?<container>[a-z0-9.]+))*(?: on (?<table>[a-z0-9.]+))*(?: \((?<emote>.+)\))*$",
			RegexOptions.IgnoreCase);

	private static readonly Regex DrinkCommandRegex =
		new(
			@"^(?<target>[\w]{0,}[a-z.-]{1,})[ ]{0,1}(?:(?<amount>[\w]{0,}[a-z0-9.\- ]{1,}?)[ ]{0,1}){0,1}(?: on (?<table>[a-z0-9.]+))*(?: \\((?<emote>.+)\\))*$",
			RegexOptions.IgnoreCase);

	private static readonly Regex FillCommandRegex =
		new(
			@"^(?<target>[\w]{0,}[a-z.-@]{1,}) (?<from>[\w]{0,}[a-z.-]{1,})[ ]{0,1}(?:(?<amount>[^(]+)[ ]{0,1}){0,1}(?<emote>\(.*\)){0,1}$",
			RegexOptions.IgnoreCase);

	private static readonly Regex PourCommandRegex =
		new(
			@"^(?:(?<owner>[\w]{0,}[a-zA-Z.-]{1,})\s+)?(?<target>[\w]{0,}[a-z.-@]{1,})[ ]{0,1}(?:(?<amount>[^(]+)[ ]{0,1}){0,1}(?<emote>\(.*\)){0,1}$",
			RegexOptions.IgnoreCase);

	private static readonly Regex SpillCommandRegex =
		new(
			@"^(?<vessel>(?:[0-9]+\.)?[a-z.-]{1,}) (?:(?<owner>[\w]{0,}[a-zA-Z.-]{1,})\s+)?(?<target>(?:[0-9]+\.)?[a-z.-@]{1,})[ ]{0,1}(?:(?<subtarget>(?:[0-9]+\.)?[a-z.-]{1,})[ ]{0,1}){0,1}(?:(?<amount>[^(]+)[ ]{0,1}){0,1}(?<emote>\(.*\)){0,1}$",
			RegexOptions.IgnoreCase);

	private static readonly Regex ArgumentsAndEmoteRegex =
		new("^(?<arguments>[^(]+)*\\s*(?:\\((?<emote>.+)\\))*$");

	private ManipulationModule()
		: base("Manipulation")
	{
		IsNecessary = true;
	}

	public override int CommandsDisplayOrder => 5;

	public static ManipulationModule Instance { get; } = new();

	[PlayerCommand("Haul", "haul")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "drag", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[HelpInfo("haul",
		@"This command is used to move something on the ground in your location to a container also in your location, for example if something is too heavy for you to lift. You must still be able to drag the item to do this; if you are currently dragging the item, your helpers will count towards this. The syntax is #3haul <item> <container>#0.

You can also use this command to remove similarly large items from containers. In this case the syntax is #3haul out <item> <container>#0.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Haul(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (ss.PeekSpeech().EqualTo("out"))
		{
			HaulOut(actor, ss);
			return;
		}

		var targetItem = actor.TargetLocalItem(ss.Pop());
		if (targetItem == null)
		{
			actor.Send("You don't see anything like that to haul anywhere.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("Which container do you want to haul that into?");
			return;
		}

		var targetContainer = actor.TargetLocalItem(ss.Pop());
		if (targetContainer == null)
		{
			actor.Send("You don't see any container like that to haul anything into.");
			return;
		}

		var container = targetContainer.GetItemType<IContainer>();
		if (container == null)
		{
			actor.Send($"{targetContainer.HowSeen(actor, true)} is not a container.");
			return;
		}

		if (!actor.Location.CanGet(targetItem, actor))
		{
			actor.Send(actor.Location.WhyCannotGet(targetItem, actor));
			return;
		}

		if (!actor.Location.CanGet(targetContainer, actor))
		{
			actor.Send(actor.Location.WhyCannotGet(targetContainer, actor));
			return;
		}

		if (!container.CanPut(targetItem))
		{
			switch (container.WhyCannotPut(targetItem))
			{
				case WhyCannotPutReason.NotContainer:
					actor.Send($"{targetContainer.HowSeen(actor, true)} is not a container.");
					return;
				case WhyCannotPutReason.ContainerClosed:
					actor.Send($"{targetContainer.HowSeen(actor, true)} is closed.");
					return;
				case WhyCannotPutReason.ContainerFull:
					actor.Send($"{targetItem.HowSeen(actor, true)} will not fit in {targetContainer.HowSeen(actor)}.");
					return;
				case WhyCannotPutReason.ItemTooLarge:
					actor.Send(
						$"{targetItem.HowSeen(actor, true)} is too large to fit in {targetContainer.HowSeen(actor)}.");
					return;
				case WhyCannotPutReason.NotCorrectItemType:
					actor.Send(
						$"{targetItem.HowSeen(actor, true)} is not the right item type to go into {targetContainer.HowSeen(actor)}.");
					return;
				case WhyCannotPutReason.CantPutContainerInItself:
					actor.Send("You cannot put a container inside itself.");
					return;
				case WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity:
					actor.Send($"{targetItem.HowSeen(actor, true)} will not fit in {targetContainer.HowSeen(actor)}, but could fit a lesser quantity of that item.");
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		var dragging = targetItem.EffectsOfType<DragTarget>().FirstOrDefault();
		if (dragging?.TheDrag.CharacterDraggers.Contains(actor) == true)
		{
			var dragCapacity = dragging.TheDrag.Draggers.Sum(x =>
				(x.Character.MaximumDragWeight - x.Character.Body.ExternalItems.Sum(y => y.Weight)) *
				(x.Aid?.EffortMultiplier ?? 1.0));
			if (dragCapacity < targetItem.Weight)
			{
				actor.Send(
					$"{(dragging.Drag.Draggers.Count() > 1 ? "You and your helpers" : "You")} are unable to drag {targetItem.HowSeen(actor)} into {targetContainer.HowSeen(actor)}.");
				return;
			}
		}
		else if (!actor.Body.CanGet(targetItem, 0))
		{
			actor.Send(actor.Body.WhyCannotGet(targetItem, 0));
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				$"@{((dragging?.Drag.Draggers.Count() ?? 0) > 1 ? " and &0's helpers" : "")} {((dragging?.Drag.Draggers.Count() ?? 0) > 1 ? "haul" : "haul|hauls")} $1 into $2.",
				actor, actor, targetItem, targetContainer), flags: OutputFlags.InnerWrap));
		container.Put(actor, targetItem);
		targetItem.Get(null);
	}

	private static void HaulOut(ICharacter actor, StringStack ss)
	{
		ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which item do you want to haul out from somewhere?");
			return;
		}

		var targetItemText = ss.PopSpeech();
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What container do you want to haul that out of?");
			return;
		}

		var targetContainer = actor.TargetLocalItem(ss.PopSpeech());
		if (targetContainer == null)
		{
			actor.OutputHandler.Send("You don't see anything like that here to haul things out of.");
			return;
		}

		var targetAsContainer = targetContainer.GetItemType<IContainer>();
		if (targetAsContainer == null)
		{
			actor.OutputHandler.Send($"{targetContainer.HowSeen(actor, true)} is not a container.");
			return;
		}

		if (!actor.Location.CanGetAccess(targetContainer, actor))
		{
			actor.OutputHandler.Send(actor.Location.WhyCannotGetAccess(targetContainer, actor));
			return;
		}

		if (targetContainer.GetItemType<IOpenable>()?.IsOpen == false)
		{
			actor.OutputHandler.Send(
				$"You can't haul anything out of {targetContainer.HowSeen(actor)} because it is closed.");
			return;
		}

		var targetItem = targetAsContainer.Contents.GetFromItemListByKeyword(targetItemText, actor);
		if (targetItem == null)
		{
			actor.OutputHandler.Send($"You don't see anything like that in {targetContainer.HowSeen(actor)}.");
			return;
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ haul|hauls $1 out of $2.", actor, actor, targetItem,
			targetContainer)));
		targetAsContainer.Take(targetItem);
		targetItem.RoomLayer = actor.RoomLayer;
		actor.Location.Insert(targetItem, true);
	}

	[PlayerCommand("Style", "style")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[HelpInfo("style", @"The #3style#0 command is used to change a characteristic of yourself or someone else, typically a hairstyle.

The options that you will have available to you will be limited by your relevant skill - you may need to have a higher skill to do certain options. Your target will also need to consider you an ally or consent to you changing their style. You may additionally need specific tools like scissors, combs, or more specialised equipment to do certain kinds of styles.

You should support the use of this command with roleplay showing how you go about doing the change.

The syntax is as follows:

	#3style <person>#0 - see what characteristics that person has available to style
	#3style <person> <characteristic>#0 - see the options for styling them
	#3style <person> <characteristic> <new value>#0 - style the person's characteristic.", AutoHelp.HelpArg)]
	protected static void Style(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("?") || ss.Peek().EqualTo("help"))
		{
			actor.Send($"The correct syntax is {"style <person> <characteristic> <value>".Colour(Telnet.Yellow)}.");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send(
				$"Which characteristic of theirs did you want to style? Available options are: {target.CharacteristicDefinitions.Where(x => x.Type == CharacteristicType.Growable).Select(x => $"{x.Name.Colour(Telnet.Green)} ({x.Pattern.ToString()})").ListToString()}.");
			return;
		}

		var text = ss.PopSpeech();
		var definition = target.CharacteristicDefinitions.Where(x => x.Type == CharacteristicType.Growable)
		                       .FirstOrDefault(x => x.Pattern.IsMatch(text) || x.Name.Equals(text));
		if (definition == null)
		{
			actor.Send("There is no characteristic like that possessed by them that can be styled.");
			return;
		}

		var possibleStyles = target.PossibleStyles(definition).ToList();
		if (ss.IsFinished)
		{
			if (!possibleStyles.Any())
			{
				actor.Send(
					$"{target.HowSeen(actor, true)} doesn't have any possible styles for {definition.Name.Colour(Telnet.Yellow)}.");
				return;
			}

			var current = target.Body.GetCharacteristic(definition, null) as IGrowableCharacteristicValue;
			var universal = actor.Gameworld.Tags.Get(actor.Gameworld.GetStaticLong("UniversalStyleToolTagId"));
			var different = actor.Gameworld.Tags.Get(actor.Gameworld.GetStaticLong("DifferentGrowthStyleToolTagId"));
			var sb = new StringBuilder(
				$"{target.HowSeen(actor, true)} could have the following possible styles for {definition.Name.Colour(Telnet.Yellow)}:");
			sb.AppendLine();
			sb.Append(
				StringUtilities.GetTextTable(
					from pstyle in possibleStyles
					orderby pstyle.GrowthStage, pstyle.Name
					let tool = pstyle.StyleToolTag ?? universal
					let dtool = current.GrowthStage != pstyle.GrowthStage ? different : null
					select new[]
					{
						pstyle.Name,
						pstyle.GetBasicValue,
						pstyle.StyleDifficulty.Describe(),
						tool == null && dtool == null
							? "None"
							: new[] { tool, dtool }.SelectNotNull(x => x?.Name.A_An_RespectPlurals(true).TitleCase())
							                       .ListToString(),
						pstyle.GrowthStage.ToString("N0", actor)
					},
					new[]
					{
						"Name",
						"Basic",
						"Difficulty",
						"Tools",
						"Length"
					},
					actor.LineFormatLength,
					colour: Telnet.Green,
					unicodeTable: actor.Account.UseUnicode
				)
			);
			actor.Send(sb.ToString());
			return;
		}

		text = ss.SafeRemainingArgument;
		var style = possibleStyles.FirstOrDefault(x => x.GetValue.EqualTo(text));
		if (style == null)
		{
			actor.Send(
				$"That is not a valid style. {"style <person> <characteristic>".Colour(Telnet.Yellow)} to see a list of possible styles.");
			return;
		}

		actor.Style(target, definition, style);
	}

	[PlayerCommand("Drag", "drag")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	[NoMovementCommand]
	[HelpInfo("drag", @"The #3drag#0 command is used to move an item that is too heavy for you to lift, or to move a person (either consenting or unable to resist).

The syntax is as follows:

	#3drag <target>#0 - start to drag a person or thing
	#3drag <target> by <lodged, worn or attached item>#0 - drag with the help of a drag aid
	#3drag help <who>#0 - begin helping someone else drag, combining your strength with theirs

Note - you can use the #3stop#0 command to stop dragging someone", AutoHelp.HelpArgOrNoArg)]
	protected static void Drag(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("?"))
		{
			actor.Send(
				$"The drag command is used to either begin dragging someone or something, or help another to drag. The syntax is:\n\t{"drag <thing> [by <aid>]".Colour(Telnet.Yellow)} - start to drag something, optionally specifying an attached dragging aid.\n{"drag help <dragger> [by <aid>]".Colour(Telnet.Yellow)} - help someone to drag something.");
			return;
		}

		IDragAid aid = null;
		ICharacter tChar = null;

		if (ss.Peek().EqualTo("aid") || ss.Peek().EqualTo("help"))
		{
			ss.Pop();
			if (ss.IsFinished)
			{
				actor.Send("Who would you like to help to drag something?");
				return;
			}

			var help = actor.TargetActor(ss.Pop());
			if (help == null)
			{
				actor.Send("There is nobody like that here who you can help to drag something.");
				return;
			}

			if (help == actor)
			{
				actor.Send("You cannot help yourself to drag something.");
				return;
			}

			if (!help.EffectsOfType<Dragging>().Any() && !help.EffectsOfType<DragHelper>().Any())
			{
				actor.Send($"{help.HowSeen(actor, true)} is not dragging anything.");
				return;
			}

			var drag = help.EffectsOfType<Dragging>().FirstOrDefault() ?? help.EffectsOfType<DragHelper>().First().Drag;

			if (ss.Peek().EqualTo("by"))
			{
				ss.Pop();
				if (ss.IsFinished)
				{
					actor.Send($"Help {help.HowSeen(actor)} to drag {drag.Target.HowSeen(actor)} by what?");
					return;
				}

				tChar = drag.Target as ICharacter;
				if (tChar != null)
				{
					var item = tChar.Body.WornItems.Concat(tChar.Body.Wounds.Select(x => x.Lodged))
					                .GetFromItemListByKeyword(ss.Pop(), actor);
					if (item == null)
					{
						actor.Send(
							$"{tChar.HowSeen(actor, true)} is not wearing anything like that for you to drag them by.");
						return;
					}

					aid = item.GetItemType<IDragAid>();
					if (aid == null)
					{
						actor.Send(
							$"{item.HowSeen(actor, true)} is not something that can be used to drag {tChar.HowSeen(actor)}.");
						return;
					}

					if (drag.Draggers.Count(x => x.Aid == aid) >= aid.MaximumUsers)
					{
						actor.Send(
							$"There is no space left on {item.HowSeen(actor)} for you to use as an aid. Use something else, or nothing.");
						return;
					}
				}
				else
				{
					var tItem = drag.Target as IGameItem;
					var item = tItem.AttachedAndConnectedItems.GetFromItemListByKeyword(ss.Pop(), actor);
					if (item == null)
					{
						actor.Send(
							$"{tItem.HowSeen(actor, true)} doesn't have anything attached or connected like that for you to drag it by.");
						return;
					}

					aid = item.GetItemType<IDragAid>();
					if (aid == null)
					{
						actor.Send(
							$"{item.HowSeen(actor, true)} is not something that can be used to drag {tItem.HowSeen(actor)}.");
						return;
					}

					if (drag.Draggers.Count(x => x.Aid == aid) >= aid.MaximumUsers)
					{
						actor.Send(
							$"There is no space left on {item.HowSeen(actor)} for you to use as an aid. Use something else, or nothing.");
						return;
					}
				}
			}

			drag.AddHelper(actor, aid);
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to help $1 to drag $2$?3| by $3||$",
				actor, actor, help, drag.Target, aid?.Parent)));
			return;
		}

		var target = actor.TargetLocal(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything or anyone like that to drag.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You cannot drag yourself.");
			return;
		}

		if (target.AffectedBy<DragTarget>())
		{
			actor.Send($"{target.HowSeen(actor, true)} is already being dragged.");
			return;
		}

		if (ss.Peek().EqualTo("by"))
		{
			ss.Pop();
			if (ss.IsFinished)
			{
				actor.Send($"Drag {target.HowSeen(actor)} by what?");
				return;
			}

			tChar = target as ICharacter;
			if (tChar != null)
			{
				var item = tChar.Body.WornItems.Concat(tChar.Body.Wounds.Select(x => x.Lodged))
				                .GetFromItemListByKeyword(ss.Pop(), actor);
				if (item == null)
				{
					actor.Send(
						$"{tChar.HowSeen(actor, true)} is not wearing anything like that for you to drag them by.");
					return;
				}

				aid = item.GetItemType<IDragAid>();
				if (aid == null)
				{
					actor.Send(
						$"{item.HowSeen(actor, true)} is not something that can be used to drag {tChar.HowSeen(actor)}.");
					return;
				}
			}
			else
			{
				var tItem = target as IGameItem;
				var item = tItem.AttachedAndConnectedItems.GetFromItemListByKeyword(ss.Pop(), actor);
				if (item == null)
				{
					actor.Send(
						$"{tItem.HowSeen(actor, true)} doesn't have anything attached or connected like that for you to drag it by.");
					return;
				}

				aid = item.GetItemType<IDragAid>();
				if (aid == null)
				{
					actor.Send(
						$"{item.HowSeen(actor, true)} is not something that can be used to drag {tItem.HowSeen(actor)}.");
					return;
				}
			}
		}

		if (target is IGameItem targetItem)
		{
			if (!targetItem.IsItemType<IHoldable>())
			{
				actor.Send($"{target.HowSeen(actor, true)} is not the kind of thing that can be moved about.");
				return;
			}

			if (!targetItem.GetItemType<IHoldable>().IsHoldable)
			{
				actor.Send($"{target.HowSeen(actor, true)} is not currently able to be moved about.");
				return;
			}

                actor.AddEffect(new Dragging(actor, aid, target));
                actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to drag $1$?2| by $2||$", actor, actor,
                        target, aid?.Parent)));
                        return;
		}

		tChar = target as ICharacter;
		var ally = tChar.IsAlly(actor);
		if (CharacterState.Able.HasFlag(tChar.State) && !ally && !tChar.IsHelpless)
		{
			actor.Send("You cannot drag someone who is conscious and unwilling."); // TODO - accept affect?
			return;
		}

		if (tChar.Effects.Any(x => x.Applies() && x.IsBlockingEffect("general")))
		{
			actor.Send(
				$"You cannot drag someone while they are {tChar.Effects.First(x => x.Applies() && x.IsBlockingEffect("general")).BlockingDescription("general", actor)}.");
			return;
		}

		if (tChar.Combat != null && tChar.MeleeRange)
		{
			actor.Send($"You cannot drag someone who is in melee combat.");
			return;
		}

		actor.AddEffect(new Dragging(actor, aid, target));
                actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to drag $1$?2| by $2||$", actor, actor,
                        target, aid?.Parent)));
                if (!ally)
                {
                        CrimeExtensions.CheckPossibleCrimeAllAuthorities(actor, CrimeTypes.Kidnapping, tChar, null, "");
                }

                if (CharacterState.Sleeping.HasFlag(tChar.State) && !CharacterState.Unconscious.HasFlag(tChar.State))
		{
			tChar.State = tChar.State & ~CharacterState.Sleeping;
			tChar.OutputHandler.Handle(new EmoteOutput(new Emote("@ wake|wakes up due to the disturbance.", tChar)));
		}
	}

	[PlayerCommand("Struggle", "struggle")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("struggle", @"The #3struggle#0 command is used when you want to try to break free of a grapple or being dragged.

The syntax is simply #3struggle#0, which can be used on a short delay and costs you stamina to attempt.", AutoHelp.HelpArg)]
	protected static void Struggle(ICharacter actor, string command)
	{
		if (!actor.CombinedEffectsOfType<IBeingGrappled>().Any() && !actor.EffectsOfType<DragTarget>().Any())
		{
			actor.Send("You are not being dragged or grappled, and so have no reason to struggle free.");
			return;
		}

		var drag = actor.EffectsOfType<DragTarget>().FirstOrDefault()?.Drag;
		if (drag?.CharacterOwner.IsAlly(actor) == true)
		{
			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					$"@ struggle|struggles free from $1{(drag.Helpers.Any() ? " and &1's helpers" : "")}, and #1 %1|let|lets &0 go without a fight.",
					actor, actor, drag.Owner), flags: OutputFlags.InnerWrap));
			drag.RemovalEffect();
			return;
		}

		if (drag != null && !actor.CanSpendStamina(actor.Gameworld.GetStaticDouble("StruggleFreeFromDragStaminaCost")))
		{
			actor.Send("You don't have the stamina to struggle free from your captors.");
			return;
		}

		var grapple = actor.CombinedEffectsOfType<IBeingGrappled>().FirstOrDefault();
		if (grapple is null)
		{
			actor.Send("You are not being dragged or grappled, and so have no reason to struggle free.");
			return;
		}

		if (!actor.CanSpendStamina(actor.Gameworld.GetStaticDouble("StruggleFreeFromGrappleStaminaCost")))
		{
			actor.OutputHandler.Send(
				$"You don't have the stamina to struggle free from {grapple.Grappling.CharacterOwner.HowSeen(actor)}.");
			return;
		}

		var move = new StruggleMove { Assailant = actor };
		if (actor.Combat == null)
		{
			move.ResolveMove(null);
		}
		else
		{
			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectStruggle(actor)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send($"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Struggling free from your impairments.");
			}
		}
	}

	[PlayerCommand("Breakout", "breakout")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("breakout", @"The #3breakout#0 command is used, typically in combat, to manually direct your combat strategy to try to break free of a grapple when it might not otherwise want to (like if you yourself were grappling your opponent).

The syntax is simply #3breakout#0.", AutoHelp.HelpArg)]
	protected static void Breakout(ICharacter actor, string command)
	{
		if (!actor.CombinedEffectsOfType<IBeingGrappled>().Any())
		{
			actor.Send("You are not being grappled, and so have no reason to breakout from any grapples.");
			return;
		}

		if (!actor.CanSpendStamina(actor.Gameworld.GetStaticDouble("BreakoutFromGrappleStaminaCost")))
		{
			actor.OutputHandler.Send($"You don't have the stamina to breakout of your grapples.");
			return;
		}

		var move = new BreakoutMove(actor);
		if (actor.Combat == null)
		{
			move.ResolveMove(null);
		}
		else
		{
			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectBreakout(actor)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send($"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Breaking out from your grapples.");
			}
		}
	}

	[PlayerCommand("Flip", "flip")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoMeleeCombatCommand]
	[HelpInfo("flip", @"The #3flip#0 command has two uses; one is to flip something over that can be interacted with in that way, such as furniture that can be flipped over to provide cover. The other use is to flip a coin.

The syntax is #3flip <item>#0 or #3flip coin#0. You must be holding a single coin to use the flip coin version of the command.", AutoHelp.HelpArg)]
	protected static void Flip(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("Flip what?");
			return;
		}

		if (ss.Peek().EqualTo("coin"))
		{
			ss.Pop();
			var currency = actor.Body.HeldOrWieldedItems
								.SelectNotNull(x => x.GetItemType<ICurrencyPile>())
								.FirstOrDefault(x => x.Coins.Count() == 1 && x.Coins.First().Item2 == 1 && x.Coins.First().Item1.GeneralForm.EqualTo("coin")) ??
						   actor.Body.HeldOrWieldedItems
								.SelectNotNull(x => x.GetItemType<ICurrencyPile>())
								.FirstOrDefault();
			if (currency is null)
			{
				actor.OutputHandler.Send("You must be holding a single coin on its own in order to flip a coin.");
				return;
			}

			if (currency.Coins.Count() != 1 || currency.Coins.First().Item2 != 1 && !currency.Coins.First().Item1.GeneralForm.EqualTo("coin"))
			{
				actor.OutputHandler.Send($"You must be holding a single coin on its own in order to flip a coin, and {currency.Parent.HowSeen(actor)} does not qualify for that.");
				return;
			}

			var cheat = false;
			var wantsHeads = false;
			if (!ss.IsFinished)
			{
				switch (ss.PopForSwitch())
				{
					case "heads":
					case "obverse":
					case "front":
						cheat = true;
						wantsHeads = true;
						break;
					case "tails":
					case "reverse":
					case "back":
						cheat = true;
						break;
				}
			}

			bool heads;
			Dictionary<Difficulty, CheckOutcome> result = [];
			if (cheat)
			{
				var check = actor.Gameworld.GetCheck(CheckType.CheatAtCoinFlip);
				result = check.CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
				if (result[Difficulty.Normal].IsPass())
				{
					heads = wantsHeads;
				}
				else
				{
					heads = Dice.Roll(1, 2) == 1;
				}
			}
			else
			{
				heads = Dice.Roll(1, 2) == 1;
			}

			actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ flip|flips $1. It lands on the {(heads ? "obverse (front/heads)" : "reverse (back/tails)")} side.", actor, actor, currency.Parent), flags: OutputFlags.PurelyVisual));
			var noticeCheck = actor.Gameworld.GetCheck(CheckType.NoticeCheck);
			if (cheat)
			{
				foreach (var tch in actor.Location.LayerCharacters(actor.RoomLayer).Except([actor]))
				{
					var outcome = new OpposedOutcome(result, noticeCheck.CheckAgainstAllDifficulties(tch, Difficulty.Hard, null), Difficulty.Normal, Difficulty.Hard);
					if (outcome.Outcome == OpposedOutcomeDirection.Opponent)
					{
						tch.OutputHandler.Send($"You are pretty sure that {actor.HowSeen(tch)} was cheating at the coin flip.");
					}
				}
			}
			return;
		}

		var target = actor.TargetLocalItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to flip.");
			return;
		}

		var flippable = target.GetItemType<IFlip>();
		if (flippable == null)
		{
			actor.Send("{0} is not something that can be flipped.", target.HowSeen(actor, true));
			return;
		}

		if (!actor.Location.CanGet(target, actor))
		{
			actor.Send(actor.Location.WhyCannotGet(target, actor));
			return;
		}

		var cover = target.GetItemType<IProvideCover>();
		if (flippable.Flipped && cover?.Cover != null && cover.IsProvidingCover &&
		    (actor.Location.LayerCharacters(actor.RoomLayer).Any(x => x.Cover?.Cover == cover.Cover) ||
		     actor.Combat != null))
		{
			actor.Send(
				"You cannot flip back over something that is providing cover while it is being used or while you are in combat.");
			return;
		}

		PlayerEmote emote = null;
		var text = ss.PopParentheses();
		if (!string.IsNullOrWhiteSpace(text))
		{
			emote = new PlayerEmote(text, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		flippable.Flip(actor, emote);
	}

	[PlayerCommand("Tear", "tear")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("tear", @"The #3tear#0 command is used to tear things or tera things out of other things. For example, tearing a page out of a book.

The syntax to use this command is #3tear <item>#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Tear(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("help", "?"))
		{
			actor.Send(
				"This command can be used to tear things off or out of items that can be torn. The syntax is tear <item>.");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("There is nothing like that to tear.");
			return;
		}

		var tearable = target.GetItemType<ITearable>();
		if (tearable == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not the kind of thing that can be torn.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		PlayerEmote emote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrWhiteSpace(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		var tornItem = tearable.Tear(actor, emote);
		if (tornItem != null)
		{
			if (actor.Body.CanGet(tornItem, 0))
			{
				actor.Body.Get(tornItem, silent: true);
			}
			else
			{
				actor.Send("Your hands are full, so you set it down on the ground.");
				tornItem.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(tornItem);
			}
		}
	}

	public const string TurnHelp = @"The turn command is used to turn things that can be turned. The turnable might be a book, a knob or dial, or some other thing. The specific page or setting to which the thing is being turned is called the 'extent'. 

There are a couple of ways you can use this command:

	#3turn#0 - turns the first turnable that you are holding by the default amount
	#3turn <turnable>#0 - turns a specific turnable being held or in the room by the default amount
	#3turn <turnable> <extent>#0 - turns the specified turnable to the specified extent
	#3turn <turnable> +<increment>#0 - turns the specified turnable by a certain amount from its current value
	#3turn <turnable> -<increment>#0 - turns the specified turnable back by a certain amount from its current value";

	[PlayerCommand("Turn", "turn")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("turn", TurnHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Turn(ICharacter actor, string command)
	{
		var turnable = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<ITurnable>()).FirstOrDefault();
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			if (turnable == null)
			{
				actor.OutputHandler.Send(TurnHelp.SubstituteANSIColour());
				return;
			}

			turnable.Turn(actor, turnable.CurrentExtent + turnable.DefaultExtentIncrement, null);
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item == null)
		{
			actor.Send("There is nothing like that for you to turn.");
			return;
		}

		turnable = item.GetItemType<ITurnable>();
		if (turnable == null)
		{
			actor.Send($"{item.HowSeen(actor, true)} is not something that can be turned.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(item);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		PlayerEmote emote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrWhiteSpace(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		if (ss.IsFinished)
		{
			turnable.Turn(actor, turnable.CurrentExtent + turnable.DefaultExtentIncrement, emote);
			return;
		}

		var extent = ss.PopSpeech();
		emoteText = ss.PopParentheses();
		if (!string.IsNullOrWhiteSpace(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		if (extent[0] == '+')
		{
			extent = extent.Substring(1);
			if (!double.TryParse(extent, out var value))
			{
				actor.Send("You must enter a valid amount to turn this item by.");
				return;
			}

			turnable.Turn(actor, turnable.CurrentExtent + value, emote);
			return;
		}

		if (extent[0] == '-')
		{
			extent = extent.Substring(1);
			if (!double.TryParse(extent, out var value))
			{
				actor.Send("You must enter a valid amount to turn this item by.");
				return;
			}

			turnable.Turn(actor, turnable.CurrentExtent - value, emote);
			return;
		}

		if (!double.TryParse(extent, out var dvalue))
		{
			actor.Send($"You must enter a valid {turnable.ExtentDescriptor} to turn this item to.");
			return;
		}

                turnable.Turn(actor, dvalue, emote);
        }

        [PlayerCommand("Apply", "apply")]
        [RequiredCharacterState(CharacterState.Able)]
        [DelayBlock("general", "You must first stop {0} before you can do that.")]
        [HelpInfo("apply", @"The #3apply#0 command is used to apply creams or similar items to a target bodypart.

You must be holding a suitable item and the bodypart you're trying to apply it to must be accessible. The target must be helpless or consent to the application.

If you don't specify an amount, you will use the whole amount of the cream.", AutoHelp.HelpArgOrNoArg)]
        protected static void Apply(ICharacter character, string command)
        {
                var ss = new StringStack(command.RemoveFirstWord());
                if (ss.IsFinished)
                {
                        character.Send("Who or what do you want to apply?");
                        return;
                }

                var item = character.TargetHeldItem(ss.PopSpeech());
                if (item == null)
                {
                        character.Send("You are not holding anything like that to apply.");
                        return;
                }

                var applicable = item.GetItemType<IApply>();
                if (applicable == null)
                {
                        character.Send($"{item.HowSeen(character, true)} is not something that can be applied.");
                        return;
                }

                var targetCharacter = character.TargetActor(ss.PopSpeech());
                if (targetCharacter == null)
                {
                        character.Send($"There is nobody like that for you to apply {item.HowSeen(character)} to.");
                        return;
                }

                if (ss.IsFinished)
                {
                        character.Send($"Which bodypart of {targetCharacter.HowSeen(character, type: DescriptionType.Possessive)} do you want to apply {item.HowSeen(character)} to?");
                        return;
                }

                var bodypart = targetCharacter.Body.GetTargetBodypart(ss.PopSpeech());
                if (bodypart == null)
                {
                        character.Send($"{targetCharacter.HowSeen(character)} does not have any such bodypart for you to apply {item.HowSeen(character)} to.");
                        return;
                }

                var emote = ss.PopParentheses();
                var amount = 0.0;
                if (!ss.IsFinished)
                {
                        amount = character.Gameworld.UnitManager.GetBaseUnits(ss.SafeRemainingArgument, UnitType.Mass, out var success);
                        if (!success)
                        {
                                character.OutputHandler.Send("That is not a valid amount to apply.");
                                return;
                        }
                        if (string.IsNullOrWhiteSpace(emote))
                        {
                                emote = ss.PopParentheses();
                        }
                }

                PlayerEmote pemote = null;
                if (!string.IsNullOrWhiteSpace(emote))
                {
                        pemote = new PlayerEmote(emote, character);
                        if (!pemote.Valid)
                        {
                                character.Send(pemote.ErrorMessage);
                                return;
                        }
                }

                switch (applicable.CanApply(targetCharacter.Body, bodypart))
                {
                        case WhyCannotApply.CannotApplyEmpty:
                                character.Send($"You cannot apply {item.HowSeen(character)} because it is empty.");
                                return;
                        case WhyCannotApply.CannotApplyNoAccessToPart:
                                character.Send($"You cannot apply {item.HowSeen(character)} to {targetCharacter.HowSeen(character, type: DescriptionType.Possessive)} {bodypart.FullDescription()} because the bodypart is obstructed by items.");
                                return;
                }

                if (character != targetCharacter && !targetCharacter.WillingToPermitMedicalIntervention(character))
                {
                        targetCharacter.AddEffect(new Accept(targetCharacter, new GenericProposal(
                                text =>
                                {
                                        if (targetCharacter.Location != character.Location)
                                        {
                                                targetCharacter.Send("You are no longer in the same location as the person who wanted to apply something to you.");
                                                return;
                                        }

                                        if (targetCharacter.Combat != null && targetCharacter.MeleeRange)
                                        {
                                                targetCharacter.Send("You can't be willingly applied while you are in melee combat!");
                                                return;
                                        }

                                        if (character.Combat != null && character.MeleeRange)
                                        {
                                                targetCharacter.Send($"{character.HowSeen(targetCharacter, true)} is no longer capable of applying that as they are in melee combat.");
                                                return;
                                        }

                                        if (!CharacterState.Able.HasFlag(character.State))
                                        {
                                                targetCharacter.Send($"{character.HowSeen(targetCharacter, true)} is no longer capable of applying that as they are {character.State.Describe()}.");
                                                return;
                                        }

                                        if (targetCharacter.Location != character.Location)
                                        {
                                                targetCharacter.Send("You are no longer in the same location as the person who wanted to apply something to you.");
                                                return;
                                        }

                                        if (!character.Body.HeldItems.Contains(item))
                                        {
                                                targetCharacter.Send($"{character.HowSeen(targetCharacter, true)} is no longer holding the item they wanted to apply to you.");
                                                return;
                                        }

                                        if (applicable.CanApply(targetCharacter.Body, bodypart) != WhyCannotApply.CanApply)
                                        {
                                                targetCharacter.Send($"{character.HowSeen(targetCharacter, true)} is no longer capable of applying that item to you.");
                                                return;
                                        }

                                        character.OutputHandler.Handle(character == targetCharacter
                                                ? new MixedEmoteOutput(new Emote($"@ apply|applies $1 to &0's {bodypart.FullDescription()}", character, item), flags: OutputFlags.SuppressObscured).Append(pemote)
                                                : new MixedEmoteOutput(new Emote($"@ apply|applies $1 to $2's {bodypart.FullDescription()}", character, item, targetCharacter), flags: OutputFlags.SuppressObscured).Append(pemote));
                                        applicable.Apply(targetCharacter.Body, bodypart, amount, character);
                                },
                                text =>
                                {
                                        targetCharacter.OutputHandler.Send(new EmoteOutput(new Emote("@ decline|declines to allow $1 to apply &0.", targetCharacter, targetCharacter, character)));
                                },
                                () =>
                                {
                                        targetCharacter.OutputHandler.Send(new EmoteOutput(new Emote("@ decline|declines to allow $1 to apply &0.", targetCharacter, targetCharacter, character)));
                                },
                                "proposing to be applied", "apply")), TimeSpan.FromSeconds(120));
                        character.OutputHandler.Handle(new EmoteOutput(new Emote($"@ are|is proposing to apply $1 to &0's {bodypart.FullDescription()}.", character, targetCharacter, item)));
                        return;
                }

                character.OutputHandler.Handle(character == targetCharacter
                        ? new MixedEmoteOutput(new Emote($"@ apply|applies $1 to &0's {bodypart.FullDescription()}", character, item), flags: OutputFlags.SuppressObscured).Append(pemote)
                        : new MixedEmoteOutput(new Emote($"@ apply|applies $1 to $2's {bodypart.FullDescription()}", character, item, targetCharacter), flags: OutputFlags.SuppressObscured).Append(pemote));
                applicable.Apply(targetCharacter.Body, bodypart, amount, character);
        }

        [PlayerCommand("Inject", "inject")]
        [RequiredCharacterState(CharacterState.Able)]
        [DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("inject", @"The #3inject#0 command is used to inject liquids into a person or thing using a syringe or similar. 

You must be holding a syringe full of liquid and the bodypart you're trying to inject into must be accessible. The target must be helpless or consent to the injection.

If you don't specify an amount, you will inject the whole volume of the syringe.

The syntax is #3inject <item> <target> <bodypart> [<amount>]#0.", AutoHelp.HelpArgOrNoArg)]
	protected static void Inject(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("Who or what do you want to inject?");
			return;
		}

		var target = character.TargetHeldItem(ss.PopSpeech());
		if (target == null)
		{
			character.Send("You are not holding anything like that to inject.");
			return;
		}

		var targetInjectable = target.GetItemType<IInject>();
		if (targetInjectable == null)
		{
			character.Send($"{target.HowSeen(character, true)} is not something that can be injected.");
			return;
		}

		var targetCharacter = character.TargetActor(ss.PopSpeech());
		if (targetCharacter == null)
		{
			character.Send($"There is nobody like that for you to inject with {target.HowSeen(character)}.");
			return;
		}

		if (ss.IsFinished)
		{
			character.Send(
				$"Which bodypart of {targetCharacter.HowSeen(character, type: DescriptionType.Possessive)} do you want to inject {target.HowSeen(character)} into?");
			return;
		}

		var bodypart = targetCharacter.Body.GetTargetBodypart(ss.PopSpeech());
		if (bodypart == null)
		{
			character.Send(
				$"{targetCharacter.HowSeen(character)} does not have any such bodypart for you to inject {target.HowSeen(character)} into.");
			return;
		}

		var emote = ss.PopParentheses();

		var amount = 0.0;
		if (!ss.IsFinished)
		{
			amount = character.Gameworld.UnitManager.GetBaseUnits(ss.SafeRemainingArgument, UnitType.FluidVolume,
				out var success);
			if (!success)
			{
				character.OutputHandler.Send("That is not a valid amount to inject.");
				return;
			}

			if (string.IsNullOrWhiteSpace(emote))
			{
				emote = ss.PopParentheses();
			}
		}

		PlayerEmote pemote = null;
		if (!string.IsNullOrWhiteSpace(emote))
		{
			pemote = new PlayerEmote(emote, character);
			if (!pemote.Valid)
			{
				character.Send(pemote.ErrorMessage);
				return;
			}
		}

		switch (targetInjectable.CanInject(targetCharacter.Body, bodypart))
		{
			case WhyCannotInject.CannotInjectEmpty:
				character.Send(
					$"You cannot inject of  {target.HowSeen(character)} into {targetCharacter.HowSeen(character, type: DescriptionType.Possessive)} {bodypart.FullDescription()} because it is empty.");
				return;
			case WhyCannotInject.CannotInjectNoAccessToPart:
				character.Send(
					$"You cannot inject of  {target.HowSeen(character)} into {targetCharacter.HowSeen(character, type: DescriptionType.Possessive)} {bodypart.FullDescription()} because the bodypart is obstructed by items.");
				return;
		}

		// If the target could potentially be unwilling, make them agree
		if (character != targetCharacter && !targetCharacter.WillingToPermitMedicalIntervention(character))
		{
			targetCharacter.AddEffect(new Accept(targetCharacter, new GenericProposal(
				text =>
				{
					if (targetCharacter.Location != character.Location)
					{
						targetCharacter.Send(
							"You are no longer in the same location as the person who wanted to inject you.");
						return;
					}

					if (targetCharacter.Combat != null && targetCharacter.MeleeRange)
					{
						targetCharacter.Send("You can't be willing injected while you are in melee combat!");
						return;
					}

					if (character.Combat != null && character.MeleeRange)
					{
						targetCharacter.Send(
							$"{character.HowSeen(targetCharacter, true)} is no longer capable of injecting you as they are in melee combat.");
						return;
					}

					if (!CharacterState.Able.HasFlag(character.State))
					{
						targetCharacter.Send(
							$"{character.HowSeen(targetCharacter, true)} is no longer capable of injecting you as they are {character.State.Describe()}.");
						return;
					}

					if (targetCharacter.Location != character.Location)
					{
						targetCharacter.Send(
							"You are no longer in the same location as the person who wanted to inject you.");
						return;
					}

					if (!character.Body.HeldItems.Contains(target))
					{
						targetCharacter.Send(
							$"{character.HowSeen(targetCharacter, true)} is no longer holding the item they wanted to inject you with.");
						return;
					}

					if (targetInjectable.CanInject(targetCharacter.Body, bodypart) != WhyCannotInject.CanInject)
					{
						targetCharacter.Send(
							$"{character.HowSeen(targetCharacter, true)} is no longer capable of injecting you with the same item.");
						return;
					}

					character.OutputHandler.Handle(
						new MixedEmoteOutput(
							new Emote($"@ inject|injects $1's {bodypart.FullDescription()} with $2", character,
								character, targetCharacter, target), flags: OutputFlags.SuppressObscured).Append(
							pemote));
					targetInjectable.Inject(targetCharacter.Body, bodypart, amount, character);
				},
				text =>
				{
					targetCharacter.OutputHandler.Send(
						new EmoteOutput(new Emote("@ decline|declines to allow $1 to inject &0.", targetCharacter,
							targetCharacter, character)));
				},
				() =>
				{
					targetCharacter.OutputHandler.Send(
						new EmoteOutput(new Emote("@ decline|declines to allow $1 to inject &0.", targetCharacter,
							targetCharacter, character)));
				},
				"proposing to be injected", "inject")), TimeSpan.FromSeconds(120));
			character.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"@ are|is proposing to inject $0 with $1 on &0's {bodypart.FullDescription()}.",
						character, targetCharacter, target)));
			return;
		}

		character.OutputHandler.Handle(character == targetCharacter
			? new MixedEmoteOutput(
				new Emote($"@ inject|injects &0's {bodypart.FullDescription()} with $1", character, character,
					target), flags: OutputFlags.SuppressObscured).Append(pemote)
			: new MixedEmoteOutput(
				new Emote($"@ inject|injects $1's {bodypart.FullDescription()} with $2", character, character,
					targetCharacter, target), flags: OutputFlags.SuppressObscured).Append(pemote));

		targetInjectable.Inject(targetCharacter.Body, bodypart, amount, character);
	}

	[PlayerCommand("Feed", "feed")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	[NoHideCommand]
	[HelpInfo("feed", @"The #3feed#0 command is used to give food, drink or medicine to someone else (either willing or unable to object). 

If the target is able to object, they will have to accept your help. You must be holding the food, medicine or liquid container.

You can use the following syntaxes with this command:

	#3feed <target> <food/medicine>#0 - give food or medicine to a target
	#3feed <target> from <liquid container>#0 - give a drink to the target", AutoHelp.HelpArgOrNoArg)]
	protected static void Feed(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		
		var target = character.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			character.Send("You don't see anyone like that to feed.");
			return;
		}

		if (target == character)
		{
			character.Send("You can't feed yourself. Use the eat, drink and swallow commands instead.");
			return;
		}

		PlayerEmote emote = null;
		if (ss.PeekSpeech().EqualTo("from"))
		{
			ss.PopSpeech();
			if (ss.IsFinished)
			{
				character.Send("Force them to drink from what?");
				return;
			}

			var container = character.TargetHeldItem(ss.PopSpeech());
			if (container == null)
			{
				character.Send("You aren't holding anything like that to force someone to drink from.");
				return;
			}

			emote = new PlayerEmote(ss.PopParentheses(), character);
			if (!emote.Valid)
			{
				character.OutputHandler.Send(emote.ErrorMessage);
				return;
			}

			var liquidContainer = container.GetItemType<ILiquidContainer>();
			if (liquidContainer == null)
			{
				character.Send($"{container.HowSeen(character, true)} is not a liquid container.");
				return;
			}

			if (container.GetItemType<IOpenable>()?.IsOpen == false)
			{
				character.Send($"{container.HowSeen(character, true)} is not open.");
				return;
			}

			if (liquidContainer.LiquidMixture?.IsEmpty != false)
			{
				character.Send($"{container.HowSeen(character, true)} is empty.");
				return;
			}

			void ExecuteForceLiquid()
			{
				if (target.Location != character.Location || target.RoomLayer != character.RoomLayer)
				{
					target.Send("You are no longer in the same location as the person who was going to feed you.");
					return;
				}

				if (!character.Body.HeldOrWieldedItems.Contains(container))
				{
					target.Send("Your feeder no longer has the item they were going to feed you from.");
					return;
				}

				if (!liquidContainer.IsOpen)
				{
					target.Send("The liquid container they were going to feed you from is no longer open.");
					return;
				}

				if (liquidContainer.LiquidMixture?.IsEmpty != false)
				{
					target.Send("The liquid container they were going to feed you from is bone dry.");
					return;
				}

				if (target.State.HasFlag(CharacterState.Dead))
				{
					character.OutputHandler.Send("Your target has died.");
					return;
				}

				if (character.State.HasFlag(CharacterState.Dead))
				{
					target.Send("Unfortunately the person who was going to feed you is dead.");
					return;
				}

				character.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote("@ force|forces $0 to drink from $1", character, target,
						container)).Append(emote));
				var liquid = liquidContainer.LiquidMixture;
				if (!target.SilentDrink(liquidContainer, Math.Min(0.03, liquidContainer.LiquidMixture.TotalVolume)))
				{
					target.OutputHandler.Handle(
						new EmoteOutput(
							new Emote($"{liquid.LiquidDescription.Proper()} dribbles out of $0's mouth.", target,
								target)));
				}
			}

			if (target.WillingToPermitMedicalIntervention(character))
			{
				ExecuteForceLiquid();
			}
			else
			{
				character.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ are|is proposing to give $0 a drink from $1.", character, target,
						container)));
				target.Send(
					$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
				target.AddEffect(new Accept(target, new GenericProposal
				{
					DescriptionString = "being forcefed",
					Keywords = new[] { "force", "feed" }.ToList(),
					AcceptAction = text => { ExecuteForceLiquid(); },
					RejectAction = text =>
					{
						target.OutputHandler.Handle(
							new EmoteOutput(new Emote("@ decline|declines to be force fed.", target)));
					},
					ExpireAction = () =>
					{
						target.OutputHandler.Handle(
							new EmoteOutput(new Emote("@ decline|declines to be force fed.", target)));
					}
				}), TimeSpan.FromSeconds(120));
			}

			return;
		}

		if (ss.IsFinished)
		{
			character.Send("What do you want to feed to them?");
			return;
		}

		var item = character.TargetHeldItem(ss.PopSpeech());
		if (item == null)
		{
			character.Send("You don't have anything like that to feed to them.");
			return;
		}

		var swallow = item.GetItemType<ISwallowable>();
		var edible = item.GetItemType<IEdible>();
		if (swallow == null && edible == null)
		{
			character.Send($"{item.HowSeen(character, true)} is not something that can be fed to people.");
			return;
		}

		emote = new PlayerEmote(ss.PopParentheses(), character);
		if (!emote.Valid)
		{
			character.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		void ExecuteForceFeed()
		{
			if (target.Location != character.Location)
			{
				target.Send("You are no longer in the same location as the person who was going to feed you.");
				return;
			}

			if (!character.Body.HeldOrWieldedItems.Contains(item))
			{
				target.Send("Your feeder no longer has the item they were going to feed you from.");
				return;
			}

			if (target.State.HasFlag(CharacterState.Dead))
			{
				character.OutputHandler.Send("Your target has died.");
				return;
			}

			if (character.State.HasFlag(CharacterState.Dead))
			{
				target.Send("Unfortunately the person who was going to feed you is dead.");
				return;
			}

			var newItem = item.DropsWhole(1) ? item : item.Get(target.Body, 1);
			swallow = newItem?.GetItemType<ISwallowable>();
			edible = newItem?.GetItemType<IEdible>();
			character.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote($"@ force|forces $0 to {(swallow != null ? "swallow" : "eat")} $1",
					character, target, newItem)).Append(emote));
			if (swallow != null)
			{
				if (!target.SilentSwallow(swallow))
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"$1 rolls out of $0's mouth.", target, target, newItem)));
					newItem.RoomLayer = target.RoomLayer;
					target.Location.Insert(newItem);
				}
			}
			else
			{
				if (newItem != item)
				{
					if (!character.Body.CanGet(newItem, 0))
					{
						newItem.RoomLayer = character.RoomLayer;
						character.Location.Insert(newItem);
					}
					else
					{
						character.Body.Get(newItem, silent: true);
					}
				}

				if (target.SilentEat(edible, 1))
				{
				}
				else
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote($"$1 falls out of $0's mouth.", target, target, newItem)));
					character.Body.Take(newItem);
					newItem.RoomLayer = target.RoomLayer;
					target.Location.Insert(newItem);
				}
			}
		}

		if (target.WillingToPermitMedicalIntervention(character))
		{
			ExecuteForceFeed();
		}
		else
		{
			character.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ are|is proposing to feed $0 $1.", character, target, item)));
			target.Send(
				$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
			target.AddEffect(new Accept(target, new GenericProposal
			{
				DescriptionString = "being forcefed",
				Keywords = new[] { "force", "feed" }.ToList(),
				AcceptAction = text => { ExecuteForceFeed(); },
				RejectAction = text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines to be force fed.", target)));
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines to be force fed.", target)));
				}
			}), TimeSpan.FromSeconds(120));
		}
	}

	[PlayerCommand("Swallow", "swallow")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("swallow", @"The #3swallow#0 command is used to swallow a pill or other solid medicine.

You can swallow pills directly out of containers, including containers on tables.

The syntax is as follows:

	#3swallow <pill>#0 - swallow a pill that you are holding
	#3swallow <pill> from <container>#0 - swallow a pill from a container you're holding
	#3swallow <pill> on <table>#0 - swallow a pill that is sitting on a table
	#3swallow <pill> from <container> on <table>#0 - swallow a pill from a container on a table", AutoHelp.HelpArgOrNoArg)]
	protected static void Swallow(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to swallow?");
			return;
		}

		var match = EatRegex.Match(command.RemoveFirstWord());
		if (!match.Success)
		{
			character.Send("The correct syntax is {0}.",
				"swallow <pill> [from <container>] [on <table>]".Colour(Telnet.Yellow));
			return;
		}

		ITable table = null;
		if (!string.IsNullOrEmpty(match.Groups["table"].Value))
		{
			var tableItem = character.TargetLocalItem(match.Groups["table"].Value);
			if (tableItem == null)
			{
				character.Send("You do not see any tables like that to swallow anything from.");
				return;
			}

			if (!tableItem.IsItemType<ITable>())
			{
				character.Send("{0} is not a table, and so you may not swallow anything from it.",
					tableItem.HowSeen(character, true));
				return;
			}

			if (!tableItem.IsItemType<IContainer>())
			{
				character.Send(
					"Although {0} is a table, it is not a container and so does not have anything you can swallow.",
					tableItem.HowSeen(character));
				return;
			}

			var (truth, error) = character.CanManipulateItem(tableItem);
			if (!truth)
			{
				character.Send(error);
				return;
			}

			table = tableItem.GetItemType<ITable>();
		}

		IContainer container = null;
		if (!string.IsNullOrEmpty(match.Groups["container"].Value))
		{
			IGameItem targetContainer = null;
			if (table == null)
			{
				targetContainer = character.TargetItem(match.Groups["container"].Value);
			}
			else
			{
				targetContainer =
					table.Parent.GetItemType<IContainer>()
					     .Contents.GetFromItemListByKeyword(match.Groups["container"].Value, character);
			}

			if (targetContainer == null)
			{
				character.Send("You do not see anything like that to swallow anything from.");
				return;
			}

			if (!targetContainer.IsItemType<IContainer>())
			{
				character.Send("{0} is not a container, and so you cannot swallow anything from it.",
					targetContainer.HowSeen(character, true));
				return;
			}

			var (truth, error) = character.CanManipulateItem(targetContainer);
			if (!truth)
			{
				character.Send(error);
				return;
			}

			container = targetContainer.GetItemType<IContainer>();
		}
		else if (table != null)
		{
			container = table.Parent.GetItemType<IContainer>();
			table = null;
		}

		if (container?.Parent.IsItemType<IOpenable>() == true && !container.Parent.GetItemType<IOpenable>().IsOpen)
		{
			character.Send("You cannot swallow anything from {0} if it is closed.",
				container.Parent.HowSeen(character));
			return;
		}

		var target = container == null
			? character.TargetHeldItem(match.Groups["food"].Value)
			: container.Contents.GetFromItemListByKeyword(match.Groups["food"].Value, character);
		if (target == null)
		{
			if (container == null)
			{
				character.Send("You do not have anything like that to swallow.");
				return;
			}

			character.Send("There is nothing like that in {0} for you to swallow.",
				container.Parent.HowSeen(character));
			return;
		}

		var targetAsSwallowable = target.GetItemType<ISwallowable>();
		if (targetAsSwallowable == null)
		{
			character.Send("{0} is not something that you can swallow.", target.HowSeen(character, true));
			return;
		}

		var emote = new PlayerEmote(match.Groups["emote"].Value, character);
		if (!emote.Valid)
		{
			character.Send(emote.ErrorMessage);
			return;
		}

		character.Swallow(targetAsSwallowable, container, table, emote);
	}

	private static void EatYield(ICharacter character, Match match)
	{
		var bites = match.Groups["bites"].Length > 0 ? double.Parse(match.Groups["bites"].Value) : 1;
		var emote = new PlayerEmote(match.Groups["emote"].Value, character);
		if (!emote.Valid)
		{
			character.Send(emote.ErrorMessage);
			return;
		}

		var yield = match.Groups["food"].Value.ToLowerInvariant();
		if (!character.Location.ForagableTypes.Any(x => x.EqualTo(yield)))
		{
			character.OutputHandler.Send(
				$"There is no foragable type {yield.Colour(Telnet.Green)} to eat at this location. You might need to look elsewhere.");
			character.OutputHandler.Send("Type FORAGE on its own to see a list of foragable types at this location."
				.ColourCommand());
			return;
		}

		character.Eat(yield, bites, emote);
	}

	public const string EatHelpText = @"The #3eat#0 command is used to make your character eat food.

You can eat food directly from containers, including containers on tables (e.g. a dinner plate). Some characters might be able to eat ""yield"" directly from the room, like vegetation.

The core syntax for this command is as follows:

	#3eat <food>#0 - eat a bite of some food
	#3eat all <food>#0 - eat all bites of a particular food item
	#3eat <##bites> <food>#0 - eat a certain number of bites of a food item
	#3eat [all|<##bites>] <food> from <container>#0 - eats food (as above) from a container
	#3eat [all|<##bites>] <food> on <table>#0 - eats food (as above) from a table
	#3eat [all|<##bites>] <food> from <container> on <table>#0 - eats food (as above) from a container on a table

Additionally, if you can eat foragable yields, the syntax is as per below:

	#3eat yield <which>#0 - eats a bite of a yield directly from a room (if permitted)
	#3eat all yield <which>#0 - eats all bites of a yield directly from a room
	#3eat <##bites> yield <which>#0 - eats a certain number of bites of yield from a room";

	[PlayerCommand("Eat", "eat")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("eat", EatHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Eat(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to eat?");
			return;
		}

		var match = EatRegex.Match(command.RemoveFirstWord());
		if (!match.Success)
		{
			character.OutputHandler.Send(EatHelpText.SubstituteANSIColour());
			return;
		}

		if (match.Groups["yield"].Length > 0)
		{
			EatYield(character, match);
			return;
		}

		ITable table = null;
		if (!string.IsNullOrEmpty(match.Groups["table"].Value))
		{
			var tableItem = character.TargetLocalItem(match.Groups["table"].Value);
			if (tableItem == null)
			{
				character.Send("You do not see any tables like that to eat from.");
				return;
			}

			if (!tableItem.IsItemType<ITable>())
			{
				character.Send("{0} is not a table, and so you may not eat from it.",
					tableItem.HowSeen(character, true));
				return;
			}

			if (!tableItem.IsItemType<IContainer>())
			{
				character.Send(
					"Although {0} is a table, it is not a container and so does not have anything you can eat.",
					tableItem.HowSeen(character));
				return;
			}

			var (truth, error) = character.CanManipulateItem(tableItem);
			if (!truth)
			{
				character.Send(error);
				return;
			}

			table = tableItem.GetItemType<ITable>();
		}

		IContainer container = null;
		if (!string.IsNullOrEmpty(match.Groups["container"].Value))
		{
			IGameItem targetContainer = null;
			if (table == null)
			{
				targetContainer = character.TargetItem(match.Groups["container"].Value);
			}
			else
			{
				targetContainer =
					table.Parent.GetItemType<IContainer>()
					     .Contents.GetFromItemListByKeyword(match.Groups["container"].Value, character);
			}

			if (targetContainer == null)
			{
				character.Send("You do not see anything like that to eat from.");
				return;
			}

			if (!targetContainer.IsItemType<IContainer>())
			{
				character.Send("{0} is not a container, and so you cannot eat anything from it.",
					targetContainer.HowSeen(character, true));
				return;
			}

			var (truth, error) = character.CanManipulateItem(targetContainer);
			if (!truth)
			{
				character.Send(error);
				return;
			}

			container = targetContainer.GetItemType<IContainer>();
		}
		else if (table != null)
		{
			container = table.Parent.GetItemType<IContainer>();
			table = null;
		}

		if (container?.Parent.IsItemType<IOpenable>() == true && !container.Parent.GetItemType<IOpenable>().IsOpen)
		{
			character.Send("You cannot eat anything from {0} if it is closed.",
				container.Parent.HowSeen(character));
			return;
		}

		var target = container == null
			? character.TargetHeldItem(match.Groups["food"].Value)
			: container.Contents.GetFromItemListByKeyword(match.Groups["food"].Value, character);
		if (target == null)
		{
			if (container == null)
			{
				character.Send("You do not have anything like that to eat.");
				return;
			}

			character.Send("There is nothing like that in {0} for you to eat.", container.Parent.HowSeen(character));
			return;
		}

		var targetAsFood = target.GetItemType<IEdible>();
		var targetAsCorpse = target.GetItemType<ICorpse>();
		var targetAsBodypart = target.GetItemType<ISeveredBodypart>();
		if (targetAsFood == null && targetAsCorpse == null && targetAsBodypart == null)
		{
			if (character.IsAdministrator())
			{
				character.OutputHandler.Handle(new EmoteOutput(new Emote("@ eat|eats $0.", character, target),
					flags: OutputFlags.SuppressObscured));
				character.Body.Take(target);
				target.Delete();
				return;
			}

			character.Send("{0} is not something that you can eat.", target.HowSeen(character, true));
			return;
		}

		var bites = match.Groups["all"].Length > 0 ? 0 :
			match.Groups["bites"].Length > 0 ? double.Parse(match.Groups["bites"].Value) : 1;

		var emote = new PlayerEmote(match.Groups["emote"].Value, character);
		if (!emote.Valid)
		{
			character.Send(emote.ErrorMessage);
			return;
		}

		if (targetAsBodypart != null)
		{
			character.Eat(targetAsBodypart, bites, emote);
		}
		else if (targetAsCorpse != null)
		{
			character.Eat(targetAsCorpse, bites, emote);
		}
		else
		{
			character.Eat(targetAsFood, container, table, bites, emote);
		}
	}

	public const string DrinkHelpText = @"The #3drink#0 command is used to make your character drink a liquid. You can drink directly from beverage containers that are on tables. If you don't specify a quantity to drink, you will drink approximately 30ml/1oz.

The core syntax for this command is as follows:

	#3drink <container>#0 - drink a sip from a liquid container
	#3drink <amount> <container>#0 - drink an amount from a liquid container
	#3drink <container> on <table>#0 - drink a sip from a liquid container on a table
	#3drink <amount> <container> on <table>#0 - drink an amount from a liquid container on a table

#6Note - for the amount, you can either use a quantity with units (e.g. 30ml/1oz) or if you use a number, it's a multiplier of the default sip amount.#0";

	[PlayerCommand("Drink", "drink")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("drink", DrinkHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Drink(ICharacter character, string command)
	{
		var text = command.RemoveFirstWord();
		if (string.IsNullOrEmpty(text))
		{
			character.Send("What is it that you want to drink?");
			return;
		}

		var match = DrinkCommandRegex.Match(text);
		if (!match.Success)
		{
			character.OutputHandler.Send(DrinkHelpText.SubstituteANSIColour());
			return;
		}

		IGameItem targetTable = null, target = null;
		if (match.Groups["table"].Length > 0)
		{
			targetTable = character.TargetLocalItem(match.Groups["table"].Value);
			if (targetTable == null)
			{
				character.Send("You do not see any table like that to find drinks on.");
				return;
			}

			if (!targetTable.IsItemType<ITable>() || !targetTable.IsItemType<IContainer>())
			{
				character.Send("{0} is not something that you can find drinks on.",
					targetTable.HowSeen(character, true));
				return;
			}

			target =
				targetTable.GetItemType<IContainer>()
				           .Contents.GetFromItemListByKeyword(match.Groups["target"].Value, character);

			if (target == null)
			{
				character.Send("There is nothing like that on {0} that you can drink.",
					targetTable.HowSeen(character));
				return;
			}

			var (truth, error) = character.CanManipulateItem(target);
			if (!truth)
			{
				character.Send(error);
				return;
			}
		}
		else
		{
			target = character.TargetItem(match.Groups["target"].Value);
			if (target == null)
			{
				character.Send("You do not have anything like that to drink from.");
				return;
			}
		}

		var targetAsDrink = target.GetItemType<ILiquidContainer>();
		if (targetAsDrink == null)
		{
			character.Send("That is not a liquid container.");
			return;
		}

		if (targetAsDrink.IsOpen && targetAsDrink.LiquidMixture?.IsEmpty != false)
		{
			character.Send("{0} is bone dry.", target.HowSeen(character, true));
			return;
		}

		var amount = character.Gameworld.GetStaticDouble("DefaultSipAmount");
		if (!string.IsNullOrEmpty(match.Groups["amount"].Value))
		{
			if (double.TryParse(match.Groups["amount"].Value, out var multiplier) && multiplier > 0.0)
			{
				amount *= multiplier;
			}
			else
			{
				amount = character.Gameworld.UnitManager.GetBaseUnits(match.Groups["amount"].Value.Trim(),
					UnitType.FluidVolume, out var success);
				if (!success || amount <= 0.0)
				{
					character.Send($"The text {match.Groups["amount"].Value.ColourCommand()} is not a valid amount to drink.");
					return;
				}
			}
		}

		if (targetAsDrink.LiquidMixture is not null && amount > targetAsDrink.LiquidMixture.TotalVolume)
		{
			amount = targetAsDrink.LiquidMixture.TotalVolume;
		}

		if (amount > 0.5)
		{
			character.Send("You couldn't possibly drink so much at once.");
			return;
		}

		PlayerEmote emote = null;
		if (match.Groups["emote"].Length > 0)
		{
			var emoteText = match.Groups["emote"].Value;
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		character.Drink(targetAsDrink, targetTable?.GetItemType<ITable>(), amount, emote);
	}

	public const string FillHelpText = @"The #3fill#0 command allows you fill up one liquid container from another, or some other source of liquid like a river or puddle.

The syntax to use this command is as follows:

	#3fill <vessel> <from vessel>#0 - fills as much liquid as possible into the vessel from another
	#3fill <vessel> <from vessel> <amount>#0 - fills a specific amount of liquid from one vessel to another
	#3fill <vessel> <character> <from vessel>#0 - fills as much liquid as possible into the vessel from another held by someone else
	#3fill <vessel> <character> <from vessel> <amount>#0 - fills a specific amount of liquid from one vessel held by someone else to another";

	[PlayerCommand("Fill", "fill")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	[NoCombatCommand]
	[HelpInfo("fill", FillHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Fill(ICharacter character, string command)
	{
		var text = command.RemoveFirstWord();
		if (string.IsNullOrEmpty(text))
		{
			character.Send("What vessel do you want to fill?");
			return;
		}

		var match = FillCommandRegex.Match(text);
		if (!match.Success)
		{
			character.OutputHandler.Send(FillHelpText.SubstituteANSIColour());
			return;
		}

		var target = character.TargetHeldItem(match.Groups["target"].Value);
		if (target == null)
		{
			character.Send("You do not have anything like that to fill.");
			return;
		}

		var targetAsContainer = target.GetItemType<ILiquidContainer>();
		if (targetAsContainer == null)
		{
			character.Send("{0} is not a liquid container.", target.HowSeen(character, true));
			return;
		}

		if (targetAsContainer.LiquidMixture?.TotalVolume >= targetAsContainer.LiquidCapacity)
		{
			character.Send($"{target.HowSeen(character, true)} is already full.");
			return;
		}

		IGameItem container;
		ICharacter containerOwner = null;
		if (match.Groups["owner"].Length > 0)
		{
			containerOwner = character.TargetActorOrCorpse(match.Groups["owner"].Value);
			if (containerOwner == null)
			{
				character.OutputHandler.Send(
					"You don't see anyone like that whose containers you can fill liquid from.");
				return;
			}

			if (containerOwner == character)
			{
				character.OutputHandler.Send(
					"You cannot use the version of this command with a specified container owner with yourself as the target.");
				return;
			}

			if (!containerOwner.WillingToPermitInventoryManipulation(character))
			{
				character.OutputHandler.Send(
					$"{containerOwner.HowSeen(character, true)} will not permit you to interact with {containerOwner.ApparentGender(character).Possessive()} containers.");
				return;
			}

			container = containerOwner.Body.ExternalItemsForOtherActors.Where(x => character.CanSee(x))
			                          .GetFromItemListByKeyword(match.Groups["from"].Value, character);
			if (container == null)
			{
				character.Send("{0} has nothing like that from which to fill {1}.",
					containerOwner.HowSeen(character, true), target.HowSeen(character));
				return;
			}
		}
		else
		{
			container = character.TargetItem(match.Groups["from"].Value);
			if (container == null)
			{
				character.Send("There is nothing like from which to fill {0}.", target.HowSeen(character));
				return;
			}
		}

		var containerAsContainer = container.GetItemType<ILiquidContainer>();
		if (containerAsContainer == null)
		{
			character.Send("{0} is not a liquid container.", container.HowSeen(character, true));
			return;
		}

		if (!targetAsContainer.IsOpen)
		{
			character.Send("{0} is not open.", target.HowSeen(character, true));
			return;
		}

		if (!containerAsContainer.IsOpen)
		{
			character.Send("{0} is not open.", container.HowSeen(character, true));
			return;
		}

		if (containerAsContainer.LiquidMixture?.IsEmpty != false)
		{
			character.Send("{0} is empty.", container.HowSeen(character, true));
			return;
		}

		PlayerEmote emote = null;
		if (match.Groups["emote"].Length > 0)
		{
			var emoteText = match.Groups["emote"].Value;
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		var amount = Math.Min(targetAsContainer.LiquidCapacity - (targetAsContainer.LiquidMixture?.TotalVolume ?? 0.0),
			containerAsContainer.LiquidMixture.TotalVolume);
		if (match.Groups["amount"].Length > 0)
		{
			amount = character.Gameworld.UnitManager.GetBaseUnits(match.Groups["amount"].Value, UnitType.FluidVolume,
				out var success);
			if (!success)
			{
				character.Send("That is not a valid amount of liquid.");
				return;
			}
		}

		amount = Math.Min(targetAsContainer.LiquidCapacity - (targetAsContainer.LiquidMixture?.TotalVolume ?? 0.0),
			Math.Min(containerAsContainer.LiquidMixture.TotalVolume, amount));

		if (amount <= 0.0)
		{
			character.Send("That is not a valid amount of liquid.");
			return;
		}

		var (truth, error) = character.CanManipulateItem(target);
		if (!truth)
		{
			character.Send(error);
			return;
		}

		if (containerOwner == null)
		{
			character.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
					$"@ fill|fills $0 from $1 with {containerAsContainer.LiquidMixture.ColouredLiquidDescription}",
					character, target,
					container), flags: OutputFlags.SuppressObscured, style: OutputStyle.IgnoreLiquidsAndFlags)
				.Append(emote));
		}
		else
		{
			character.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
					$"@ fill|fills $0 from $2's $1 with {containerAsContainer.LiquidMixture.ColouredLiquidDescription}",
					character, target,
					container, containerOwner), flags: OutputFlags.SuppressObscured,
				style: OutputStyle.IgnoreLiquidsAndFlags).Append(emote));
		}

		targetAsContainer.MergeLiquid(containerAsContainer.RemoveLiquidAmount(amount, character, "fill"), character,
			"fill");
	}

	[PlayerCommand("Empty", "empty")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	[HelpInfo("empty",
		@"The #3empty#0 command allows you to empty out the contents of a container, or pour liquid out of a liquid container. 

The syntax is as follows:

	#3empty <container> [<other container>]#0 - empty the contents of a container (optionally into another container or another person's container)
	#3empty <liquid container> [<amount>]#0 - tip liquid from a liquid container onto the ground.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Empty(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You do not see anything like that to empty.");
			return;
		}

		PlayerEmote emote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		if (!actor.Location.CanGetAccess(target, actor))
		{
			actor.OutputHandler.Send(actor.Location.WhyCannotGetAccess(target, actor));
			return;
		}

		var liquidContainer = target.GetItemType<ILiquidContainer>();
		if (liquidContainer != null)
		{
			if (!liquidContainer.IsOpen)
			{
				actor.Send("{0} is not open.", target.HowSeen(actor, true));
				return;
			}

			if (liquidContainer.LiquidMixture?.IsEmpty != false)
			{
				actor.Send("{0} is empty.", target.HowSeen(actor, true));
				return;
			}

			if (target.InInventoryOf == null && liquidContainer.CanBeEmptiedWhenInRoom)
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} cannot be emptied when it is on the ground.");
				return;
			}

			var amount = liquidContainer.LiquidMixture.TotalVolume;
			if (!ss.IsFinished)
			{
				amount = actor.Gameworld.UnitManager.GetBaseUnits(ss.PopSpeech(), UnitType.FluidVolume,
					out var success);
				if (!success)
				{
					actor.OutputHandler.Send("That is not a valid amount of liquid.");
					return;
				}

				emoteText = ss.PopParentheses();
				if (!string.IsNullOrEmpty(emoteText))
				{
					emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor);
					if (!emote.Valid)
					{
						actor.OutputHandler.Send(emote.ErrorMessage);
						return;
					}
				}
			}

			amount = Math.Min(liquidContainer.LiquidMixture.TotalVolume, amount);

			actor.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
					$"@ pour|pours {liquidContainer.LiquidMixture.ColouredLiquidDescription} out of $0", actor, target),
				flags: OutputFlags.SuppressObscured, style: OutputStyle.IgnoreLiquidsAndFlags).Append(emote));
			var mixture = new LiquidMixture(liquidContainer.LiquidMixture);
			mixture.SetLiquidVolume(amount);
			PuddleGameItemComponentProto.CreateNewPuddle(mixture, actor.Location, actor.RoomLayer, actor);
			liquidContainer.ReduceLiquidQuantity(amount, actor, "empty");
			return;
		}

		var container = target.GetItemType<IContainer>();
		if (container == null)
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is not any kind of container, and so cannot be emptied.");
			return;
		}

		if (target.GetItemType<IOpenable>()?.IsOpen == false)
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true)} is not open.");
			return;
		}

		if (!container.Contents.Any())
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} is already empty.");
			return;
		}

		if (!ss.IsFinished)
		{
			var secondTargetText = ss.PopSafe();
			var secondTarget = actor.TargetItem(secondTargetText);
			if (secondTarget == null)
			{
				actor.OutputHandler.Send($"You don't see anything like that to empty {target.HowSeen(actor)} into.");
				return;
			}

			var (truth, error) = actor.CanManipulateItem(secondTarget);
			if (!truth)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			var secondContainer = secondTarget.GetItemType<IContainer>();
			if (secondContainer == null)
			{
				actor.Send(
					$"{secondTarget.HowSeen(actor, true)} is not a container, so you can't empty anything into it.");
				return;
			}

			if (secondTarget.GetItemType<IOpenable>()?.IsOpen == false)
			{
				actor.OutputHandler.Send(
					$"{secondTarget.HowSeen(actor, true)} is not open.");
				return;
			}

			if (secondTarget == target)
			{
				actor.OutputHandler.Send($"You cannot empty a container into itself.");
				return;
			}

			container.Empty(actor, secondContainer, emote);
			return;
		}

		container.Empty(actor, null, emote);
	}

	[PlayerCommand("Pour", "pour")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	[HelpInfo("pour",
		@"The #3pour#0 command allows you to pour liquid from one vessel to another. You must be holding the container you want to pour from.

The syntax is as follows:

	#3pour [<amount>] <container> into <other>#0 - pour liquid from one container to another",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Pour(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var amountArg = ss.PopSpeech();
		var fromArg = ss.PopSafe();
		var intoArg = ss.PopSafe();
		if (intoArg.EqualTo("into"))
		{
			intoArg = ss.PopSafe();
		}

		if (string.IsNullOrEmpty(fromArg))
		{
			actor.OutputHandler.Send($"What do you want to pour liquid from?");
			return;
		}

		var fromItem = actor.TargetPersonalItem(fromArg);
		if (fromItem == null)
		{
			actor.OutputHandler.Send("You don't have anything like that to pour from.");
			return;
		}

		var fromItemContainer = fromItem.GetItemType<ILiquidContainer>();
		if (fromItemContainer == null)
		{
			actor.OutputHandler.Send($"{fromItem.HowSeen(actor)} is not a liquid container.");
			return;
		}

		if (!fromItemContainer.IsOpen)
		{
			actor.OutputHandler.Send($"{fromItem.HowSeen(actor)} is not open.");
			return;
		}

		if (fromItemContainer.LiquidMixture?.IsEmpty != false)
		{
			actor.OutputHandler.Send($"{fromItem.HowSeen(actor)} is empty.");
			return;
		}

		var amount = actor.Gameworld.UnitManager.GetBaseUnits(amountArg, UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid amount of liquid to pour.");
			return;
		}

		amount = Math.Min(amount, fromItemContainer.LiquidVolume);
		if (string.IsNullOrEmpty(intoArg))
		{
			actor.OutputHandler.Send($"What do you want to pour the liquid from {fromItem.HowSeen(actor)} into?");
			return;
		}

		var intoItem = actor.TargetItem(intoArg);
		if (intoItem == null)
		{
			actor.OutputHandler.Send(
				$"You don't see anything like that for you to pour the contents of {fromItem.HowSeen(actor)} into.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(intoItem);
		if (!truth)
		{
			actor.Send(error);
			return;
		}

		var intoItemContainer = intoItem.GetItemType<ILiquidContainer>();
		if (intoItemContainer == null)
		{
			actor.OutputHandler.Send($"{intoItem.HowSeen(actor, true)} is not a liquid container.");
			return;
		}

		if (!intoItemContainer.IsOpen)
		{
			actor.OutputHandler.Send($"{intoItem.HowSeen(actor, true)} is not open.");
			return;
		}

		if (intoItemContainer.LiquidMixture?.CanMerge(fromItemContainer.LiquidMixture) == false)
		{
			actor.OutputHandler.Send($"Unfortunately, the liquid contents of those two containers cannot mix.");
			return;
		}

		if (intoItemContainer.LiquidCapacity - intoItemContainer.LiquidVolume <= 0.0)
		{
			actor.OutputHandler.Send($"{intoItem.HowSeen(actor, true)} is full to the brim and can hold no more.");
			return;
		}

		PlayerEmote emote = null;
		if (!ss.IsFinished)
		{
			var emoteText = ss.PopParentheses();
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		amount = Math.Min(amount, intoItemContainer.LiquidCapacity - intoItemContainer.LiquidVolume);
		var intoCharacter = intoItem.InInventoryOf?.Actor;
		if (intoCharacter == null || intoCharacter == actor)
		{
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(
					new Emote(
						$"@ pour|pours {fromItemContainer.LiquidMixture.ColouredLiquidDescription} from $1 into $2",
						actor, actor, fromItem, intoItem), style: OutputStyle.IgnoreLiquidsAndFlags).Append(emote));
		}
		else
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
					new Emote(
						$"@ pour|pours {fromItemContainer.LiquidMixture.ColouredLiquidDescription} from $1 into $3's $2",
						actor, actor, fromItem, intoItem, intoCharacter), style: OutputStyle.IgnoreLiquidsAndFlags)
				.Append(emote));
		}

		intoItemContainer.MergeLiquid(fromItemContainer.RemoveLiquidAmount(amount, actor, "pour"), actor, "pour");
	}

	public const string SpillHelpText = @"The #3spill#0 command allows you to spill liquid from a container onto a person or object. You must be holding the container that you wish to spill.

The syntax is as follows:

	#3spill <vessel> <target> [<amount>]#0 - spill all (or some amount) of liquid onto a target";

	[PlayerCommand("Spill", "spill")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoMovementCommand]
	[NoCombatCommand]
	[HelpInfo("spill", SpillHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Spill(ICharacter character, string command)
	{
		var match = SpillCommandRegex.Match(command.RemoveFirstWord());
		if (!match.Success)
		{
			character.OutputHandler.Send(SpillHelpText.SubstituteANSIColour());
			return;
		}

		var vessel = character.TargetHeldItem(match.Groups["vessel"].Value);
		if (vessel == null)
		{
			character.Send("You are holding no such vessel from which you can spill anything.");
			return;
		}

		var vesselContainer = vessel.GetItemType<ILiquidContainer>();
		if (vesselContainer == null)
		{
			character.Send($"{vessel.HowSeen(character, true)} is not a liquid container.");
			return;
		}

		if (!vesselContainer.IsOpen)
		{
			character.Send($"{vessel.HowSeen(character, true)} is not open.");
			return;
		}

		if (vesselContainer.LiquidMixture?.IsEmpty != false)
		{
			character.Send($"{vessel.HowSeen(character, true)} is empty.");
			return;
		}

		IGameItem target;
		ICharacter charTarget = null;
		if (match.Groups["subtarget"].Length > 0)
		{
			charTarget = character.TargetActorOrCorpse(match.Groups["target"].Value);
			if (charTarget == null)
			{
				character.Send("You don't see anyone like that upon whom you can spill anything.");
				return;
			}

			target = charTarget.Inventory.GetFromItemListByKeyword(match.Groups["subtarget"].Value, character);
			if (target == null)
			{
				character.Send(
					$"{charTarget.HowSeen(character, true)} doesn't have any items like that upon which you can spill anything.");
				return;
			}
		}
		else
		{
			target = character.TargetItem(match.Groups["target"].Value);
			if (target == null)
			{
				character.Send("You don't see anything like that upon which you can spill anything.");
				return;
			}
		}

		var amount = vesselContainer.LiquidMixture.TotalVolume;
		if (match.Groups["amount"].Length > 0)
		{
			amount = character.Gameworld.UnitManager.GetBaseUnits(match.Groups["amount"].Value, UnitType.FluidVolume,
				out var success);
			if (!success)
			{
				character.Send("That is not a valid amount of liquid.");
				return;
			}
		}

		amount = Math.Min(vesselContainer.LiquidMixture.TotalVolume, amount);

		PlayerEmote emote = null;
		if (match.Groups["emote"].Length > 0)
		{
			var emoteText = match.Groups["emote"].Value;
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		if (charTarget != null && !charTarget.WillingToPermitInventoryManipulation(character) &&
		    charTarget.State.IsAble() && charTarget.CanSee(character))
		{
			var check1 = character.Gameworld.GetCheck(CheckType.SpillLiquidOnPerson);
			var proximityBonus = character.GetProximity(charTarget).In(Proximity.Intimate, Proximity.Immediate)
				? character.Gameworld.GetStaticDouble("SpillCharacterProximityBonus")
				: 0.0;
			var results1 =
				check1.CheckAgainstAllDifficulties(character, Difficulty.Normal, null, charTarget, proximityBonus);

			var check2 = character.Gameworld.GetCheck(CheckType.DodgeSpillLiquidOnPerson);
			var unawareBonus = charTarget.IsBlocked("general", "move").Truth
				? 0.0
				: character.Gameworld.GetStaticDouble("SpillCharacterUnawareBonus");
			var results2 =
				check2.CheckAgainstAllDifficulties(charTarget, Difficulty.Normal, null, character, unawareBonus);

			var outcome = new OpposedOutcome(results1, results2, Difficulty.Normal, Difficulty.Normal);
			if (outcome.Outcome == OpposedOutcomeDirection.Opponent)
			{
				character.OutputHandler.Handle(new MixedEmoteOutput(
						new Emote(
							$"@ try|tries to spill {vesselContainer.LiquidMixture.ColouredLiquidDescription} from $0 on $2's !1, but #2 %2|are|is able to avoid it.",
							character, vessel, target, charTarget), style: OutputStyle.IgnoreLiquidsAndFlags)
					.Append(emote));
				vesselContainer.LiquidMixture.RemoveLiquidVolume(amount);
				return;
			}
		}

		character.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
			$"@ spill|spills {vesselContainer.LiquidMixture.ColouredLiquidDescription} from $0 on {(charTarget != null ? "$2's !1" : "$1")}",
			character, vessel, target, charTarget), style: OutputStyle.IgnoreLiquidsAndFlags).Append(emote));
		target.ExposeToLiquid(new LiquidMixture(vesselContainer.LiquidMixture.RemoveLiquidVolume(amount)), null,
			LiquidExposureDirection.FromOnTop);
	}

	[PlayerCommand("Smoke", "smoke")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoHideCommand]
	protected static void Smoke(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to smoke?");
			return;
		}

		var target = character.TargetHeldItem(ss.Pop());
		if (target == null)
		{
			character.Send("You do not see anything like that to smoke.");
			return;
		}

		if (!target.IsItemType<ISmokeable>())
		{
			character.Send("{0} is not something that can be smoked.", target.HowSeen(character, true));
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		target.GetItemType<ISmokeable>().Smoke(character, emote);
	}

	[PlayerCommand("Light", "light")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	protected static void Light(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to light?");
			return;
		}

		var target = character.TargetItem(ss.Pop());
		if (target == null)
		{
			character.Send("You do not see that to light.");
			return;
		}

		var ignitionText = ss.Pop();
		var ignition = character.Target(ignitionText);
		if (ignition == null && !string.IsNullOrEmpty(ignitionText))
		{
			character.Send("You do not see that ignition source.");
			return;
		}

		var lightable = target.GetItemType<ILightable>();
		if (lightable == null)
		{
			character.Send("{0} is not something that can be lit.", target.HowSeen(character, true));
			return;
		}

		var (truth, error) = character.CanManipulateItem(target);
		if (!truth)
		{
			character.Send(error);
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		lightable.Light(character, ignition, emote);
	}

	[PlayerCommand("Extinguish", "extinguish")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	protected static void Extinguish(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to extinguish?");
			return;
		}

		var target = character.TargetItem(ss.Pop());
		if (target == null)
		{
			character.Send("You do not see that to extinguish.");
			return;
		}

		var lightable = target.GetItemType<ILightable>();
		if (lightable == null)
		{
			character.Send("{0} is not something that can be extinguished.", target.HowSeen(character, true));
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		lightable.Extinguish(character, emote);
	}

	[PlayerCommand("Knock", "knock")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	protected static void Knock(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to knock upon?");
			return;
		}

		var exits = character.Location.ExitsFor(character);

		IDoor openable = null;

		var targetExit = exits.GetFromItemListByKeyword(ss.Pop(), character);
		if (targetExit != null)
		{
			if (targetExit.Exit.Door == null)
			{
				character.Send("There is no door in that direction to knock upon.");
				return;
			}

			openable = targetExit.Exit.Door;
		}
		else
		{
			var doorItem =
				exits.SelectNotNull(x => x.Exit.Door)
				     .Select(x => x.Parent)
				     .GetFromItemListByKeyword(ss.Last, character);
			if (doorItem != null)
			{
				openable = doorItem.GetItemType<IDoor>();
				targetExit = openable.InstalledExit.CellExitFor(character.Location);
			}
		}

		if (openable == null)
		{
			character.Send("You do not see that to knock upon.");
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, character);
			if (!emote.Valid)
			{
				character.Send(emote.ErrorMessage);
				return;
			}
		}

		targetExit.Exit.Door.Knock(character, emote);
	}

	[PlayerCommand("Install", "install")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	protected static void Install(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What do you want to install?");
			return;
		}

		var installText = ss.Pop();
		if (ss.IsFinished)
		{
			character.Send("Where do you want to install that?");
			return;
		}

		var target = character.TargetHeldItem(installText);
		if (target == null)
		{
			character.Send("You do not see anything like that to install.");
			return;
		}

		var targetAsDoor = target.GetItemType<IDoor>();
		if (targetAsDoor != null)
		{
			InstallDoor(character, ss, target, targetAsDoor);
			return;
		}

		var targetAsLock = target.GetItemType<ILock>();
		if (targetAsLock != null)
		{
			InstallLock(character, ss, target, targetAsLock);
			return;
		}

		character.Send("{0} is not something that can be installed.", target.HowSeen(character, true));
	}

	private static void InstallLock(ICharacter character, StringStack ss, IGameItem lockItem, ILock theLock)
	{
		if (ss.IsFinished)
		{
			character.Send("Where do you want to install {0}?", lockItem.HowSeen(character));
			return;
		}

		var targetText = ss.PopSpeech();
		IGameItem targetItem = null;

		var exit = character.Location.ExitsFor(character).GetFromItemListByKeyword(targetText, character);
		if (exit == null)
		{
			targetItem = character.TargetItem(targetText);
		}
		else
		{
			targetItem = exit.Exit.Door != null ? exit.Exit.Door.Parent : character.TargetItem(targetText);
		}

		if (targetItem == null)
		{
			character.Send("You do not see that to install {0} in.", lockItem.HowSeen(character));
			return;
		}

		var targetAsLockable = targetItem.GetItemType<ILockable>();
		if (targetAsLockable == null)
		{
			character.Send("{0} is not something that can have locks installed in it.",
				targetItem.HowSeen(character, true));
			return;
		}

		if (theLock.IsLocked)
		{
			character.Send("You cannot install locks unless they are currently unlocked.");
			return;
		}

		void LockInstallCompletionAction(IPerceivable perceivable)
		{
			if (lockItem == null || !character.Body.HeldItems.Contains(lockItem))
			{
				character.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ stop|stops installing the lock as #0 no longer have|has it.", character, character)));
				return;
			}

			if (theLock.IsLocked)
			{
				character.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ stop|stops installing the lock as it is locked.", character, character)));
				return;
			}

			if (targetItem == null || !targetItem.TrueLocations.Contains(character.Location))
			{
				character.OutputHandler.Handle(new EmoteOutput(new Emote(
					"@ stop|stops installing the lock as the target is no longer there.", character, character)));
				return;
			}

			character.OutputHandler.Handle(new EmoteOutput(new Emote("@ finish|finishes installing $0 in $1.",
				character, lockItem, targetItem)));
			character.Body.Take(lockItem);
			targetAsLockable.InstallLock(theLock, character);
		}

		character.OutputHandler.Handle(
			new EmoteOutput(
				new Emote("@ begin|begins installing $0 in $1.", character, lockItem, targetItem)));

		if (character.IsAdministrator())
		{
			LockInstallCompletionAction(character);
		}
		else
		{
			character.AddEffect(
				new SimpleCharacterAction(character, LockInstallCompletionAction, "installing a lock",
					new[] { "general", "movement" }, "installing a lock"),
				TimeSpan.FromSeconds(30));
		}
	}

	private static void InstallDoor(ICharacter character, StringStack ss, IGameItem doorItem, IDoor door)
	{
		if (ss.IsFinished)
		{
			character.Send("What exit do you want to install {0} in?", doorItem.HowSeen(character));
			return;
		}

		var exitText = ss.PopSpeech();

		var exit = character.Location.ExitsFor(character).GetFromItemListByKeyword(exitText, character);
		if (exit == null)
		{
			character.Send("There is no exit in that direction.");
			return;
		}

		if (!exit.Exit.AcceptsDoor)
		{
			character.Send("That exit is not suitable for the installation of doors.");
			return;
		}

		if (exit.Exit.Door != null)
		{
			character.Send("There is already a door installed in that exit. You must remove it first.");
			return;
		}

		if (exit.Exit.DoorSize != doorItem.Size)
		{
			character.Send("That exit takes only {0} size doors, and {1} is {2}.",
				exit.Exit.DoorSize.Describe().Colour(Telnet.Green),
				doorItem.HowSeen(character),
				doorItem.Size.Describe().Colour(Telnet.Green)
			);
			return;
		}

		ICell openDirection = null;
		if (!ss.IsFinished)
		{
			switch (ss.Pop().ToLowerInvariant())
			{
				case "inwards":
					openDirection = exit.Origin;
					break;
				case "outwards":
					openDirection = exit.Destination;
					break;
				default:
					character.Send("Valid options are inwards or outwards as a direction to install this door.");
					return;
			}
		}

		void doorInstallCompletionAction(IPerceivable perceivable)
		{
			if (exit.Exit.Door == null)
			{
				door.State = DoorState.Open;
				door.OpenDirectionCell = openDirection;
				door.HingeCell = character.Location;
				door.Changed = true;
				door.InstalledExit = exit.Exit;
				character.Body.Take(doorItem);
				exit.Exit.Door = door;
				exit.Exit.Changed = true;
				var doorAsUnlockable = door.Parent.GetItemType<ILockable>();
				foreach (var lo in door.Locks.ToList())
				{
					doorAsUnlockable.RemoveLock(lo);
					doorAsUnlockable.InstallLock(lo);
				}

				character.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							$"@ install|installs $0 in the exit to {exit.OutboundDirectionDescription}{(openDirection != null ? ", opening " + ss.Last.ToLowerInvariant() : "")}.",
							character, doorItem)));
			}
			else
			{
				character.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops installing the door as one is already there.", character)));
			}
		}

		character.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					$"@ begin|begins installing $0 in the exit to {exit.OutboundDirectionDescription}.",
					character, doorItem)));

		if (character.IsAdministrator())
		{
			doorInstallCompletionAction(character);
		}
		else
		{
			character.AddEffect(
				new SimpleCharacterAction(character, doorInstallCompletionAction, "installing a door",
					new[] { "general", "movement" }, "installing a door"),
				TimeSpan.FromSeconds(30));
		}
	}

	[PlayerCommand("Uninstall", "uninstall")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	protected static void Uninstall(ICharacter character, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			character.Send("What is it that you want to uninstall?");
			return;
		}

		var targetText = ss.Pop();
		IGameItem targetItem = null;
		var exit = character.Location.ExitsFor(character).GetFromItemListByKeyword(targetText, character);
		if (exit == null)
		{
			targetItem = character.TargetItem(targetText);
		}
		else
		{
			targetItem = exit.Exit.Door == null ? character.TargetItem(targetText) : exit.Exit.Door.Parent;
		}

		if (targetItem == null)
		{
			character.Send("You do not see that to uninstall.");
			return;
		}

		if (CrimeExtensions.HandleCrimesAndLawfulActing(character, CrimeTypes.BreakAndEnter, null, targetItem))
		{
			return;
		}

		if (ss.IsFinished)
		{
			var targetAsDoor = targetItem.GetItemType<IDoor>();
			if (targetAsDoor != null)
			{
				UninstallDoor(character, targetItem, targetAsDoor, exit);
				return;
			}
		}

		var targetAsLockable = targetItem.GetItemType<ILockable>();
		if (targetAsLockable != null)
		{
			UninstallLock(character, ss, targetItem, targetAsLockable);
			return;
		}

		character.Send("{0} is not something that can be a target of an uninstallation.",
			targetItem.HowSeen(character, true));
	}

	private static void UninstallLock(ICharacter character, StringStack ss, IGameItem lockableItem,
		ILockable lockable)
	{
		ILock theLock;
		if (ss.IsFinished)
		{
			theLock = lockable.Locks.FirstOrDefault();

			if (theLock == null)
			{
				character.Send("{0} does not have any locks for you to uninstall.",
					lockableItem.HowSeen(character, true));
				return;
			}
		}
		else
		{
			var targetLock = lockable.Locks.Select(x => x.Parent).GetFromItemListByKeyword(ss.Pop(), character);
			if (targetLock == null)
			{
				character.Send("{0} does not have any such lock for you to uninstall.",
					lockableItem.HowSeen(character, true));
				return;
			}

			theLock = targetLock.GetItemType<ILock>();
		}

		if (!character.IsAdministrator())
		{
			if (theLock.IsLocked)
			{
				character.Send("You must first unlock {0} before you can remove it.",
					theLock.Parent.HowSeen(character));
				return;
			}

			if (lockableItem.IsItemType<IOpenable>() && !lockableItem.GetItemType<IOpenable>().IsOpen)
			{
				character.Send("You must first open {0} before you can remove {1} from it.",
					lockableItem.HowSeen(character), theLock.Parent.HowSeen(character));
				return;
			}
		}

		CrimeExtensions.CheckPossibleCrimeAllAuthorities(character, CrimeTypes.BreakAndEnter, null, lockableItem, "");

		void lockUninstallAction(IPerceivable perceivable)
		{
			if (!lockableItem.TrueLocations.Contains(character.Location))
			{
				character.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops removing the lock as it is no longer there.", character)));
				return;
			}

			if (!character.IsAdministrator())
			{
				if (theLock.IsLocked)
				{
					character.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ stop|stops removing the lock as it is now locked.", character)));
					return;
				}

				if (lockableItem.IsItemType<IOpenable>() && !lockableItem.GetItemType<IOpenable>().IsOpen)
				{
					character.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ stop|stops removing the lock as $0 is now closed.", character,
							lockableItem)));
					return;
				}
			}

			character.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ finish|finishes removing $0 from $1.", character, theLock.Parent,
					lockableItem)));
			lockable.RemoveLock(theLock);
			if (character.Body.CanGet(theLock.Parent, 0))
			{
				character.Body.Get(theLock.Parent);
			}
			else
			{
				theLock.Parent.RoomLayer = character.RoomLayer;
				character.Location.Insert(theLock.Parent);
				character.Send("You set the lock down on the ground as you cannot hold it.");
			}
		}

		character.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ begin|begins removing $0 from $1.", character, theLock.Parent, lockableItem)));

		if (character.IsAdministrator())
		{
			lockUninstallAction(character);
		}
		else
		{
			character.AddEffect(
				new SimpleCharacterAction(character, lockUninstallAction, "removing a lock",
					new[] { "general", "movement" }, "removing a lock"),
				TimeSpan.FromSeconds(30));
		}
	}

	private static void UninstallDoor(ICharacter character, IGameItem doorItem, IDoor door, ICellExit exit)
	{
		if (!character.IsAdministrator())
		{
			if (!door.CanPlayersUninstall)
			{
				character.Send($"You cannot think of a way to remove {doorItem.HowSeen(character)}.");
				return;
			}

			if ((door.UninstallDifficultyHingeSide == Difficulty.Impossible &&
			     door.HingeCell == character.Location) ||
			    (door.UninstallDifficultyNotHingeSide == Difficulty.Impossible &&
			     door.HingeCell != character.Location))
			{
				character.Send($"You cannot think of a way to remove {doorItem.HowSeen(character)} from this side.");
				return;
			}
		}

		if (character.AffectedBy<IHideEffect>() && !character.IsAdministrator())
		{
			character.Send("That is not something you can do while you are trying to remain hidden.");
			return;
		}

		if (character.Movement != null)
		{
			character.Send("You must stop moving first.");
			return;
		}

		if (door.InstalledExit == null)
		{
			character.Send("{0} is not installed anywhere.", doorItem.HowSeen(character, true));
			return;
		}

		CrimeExtensions.CheckPossibleCrimeAllAuthorities(character, CrimeTypes.BreakAndEnter, null, doorItem, "");

		var difficulty = door.HingeCell == character.Location
			? door.UninstallDifficultyHingeSide
			: door.UninstallDifficultyNotHingeSide;

		void DoorUninstallAction(IPerceivable perceivable)
		{
			if (exit.Exit.Door == door)
			{
				if (character.IsAdministrator() || character.Gameworld.GetCheck(CheckType.UninstallDoorCheck)
				                                            .Check(character, difficulty, door.UninstallTrait,
					                                            door.Parent)
				                                            .IsPass())
				{
					exit.Exit.Door = null;
					exit.Exit.Changed = true;
					door.OpenDirectionCell = null;
					door.HingeCell = null;
					door.State = DoorState.Uninstalled;
					door.InstalledExit = null;
					var doorAsUnlockable = door.Parent.GetItemType<ILockable>();
					foreach (var lo in door.Locks.ToList())
					{
						doorAsUnlockable.RemoveLock(lo);
						doorAsUnlockable.InstallLock(lo);
					}

					door.Changed = true;
					door.Parent.RoomLayer = character.RoomLayer;
					character.Location.Insert(door.Parent);
					character.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"@ remove|removes $0 from the exit to {exit.OutboundDirectionDescription}.", character,
						door.Parent)));
				}
				else
				{
					character.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"@ fail|fails to remove|removes $0 from the exit to {exit.OutboundDirectionDescription}.",
						character, door.Parent)));
				}
			}
			else
			{
				character.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops removing the door as it is no longer there.", character)));
			}
		}

		character.OutputHandler.Handle(new EmoteOutput(new Emote(
			$"@ begin|begins removing $0 from the exit to {exit.OutboundDirectionDescription}.", character,
			door.Parent)));

		if (character.IsAdministrator())
		{
			DoorUninstallAction(character);
		}
		else
		{
			character.AddEffect(
				new SimpleCharacterAction(character, DoorUninstallAction, "removing a door",
					new[] { "general", "movement" }, "removing a door"),
				TimeSpan.FromSeconds((int)difficulty * 20));
		}
	}

	[PlayerCommand("Junk", "junk")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Junk(ICharacter character, string command)
	{
		if (!character.Gameworld.GetStaticBool("PlayersCanJunk") && !character.IsAdministrator())
		{
			character.Send("You are not allowed to junk things.");
			return;
		}

		var ss = new StringStack(command);
		var junkCmd = ss.Pop();
		if (!junkCmd.EqualTo("junk"))
		{
			character.Send(
				"For your protection, you must type out the entirety of the command 'Junk' in order for it to work.");
			return;
		}

		var cmd = ss.PopSpeech().ToLowerInvariant();

		if (string.IsNullOrEmpty(cmd))
		{
			character.OutputHandler.Send("What is it that you wish to junk?");
			return;
		}

		var item = character.TargetHeldItem(cmd);
		if (item == null)
		{
			character.OutputHandler.Send("You do not see that to junk.");
			return;
		}

		if (item.IsItemType<ICorpse>())
		{
			character.OutputHandler.Send("You cannot junk corpses.");
			return;
		}

		if (item.IsItemType<ISeveredBodypart>())
		{
			character.OutputHandler.Send("You cannot junk severed body parts.");
			return;
		}

		// TODO - no junk component

		void JunkAction()
		{
			if (!character.Body.HeldOrWieldedItems.Contains(item))
			{
				character.Send("You no longer have the item that you were going to junk.");
				return;
			}

			character.OutputHandler.Handle(new EmoteOutput(new Emote("@ junk|junks $0.", character, item),
				flags: OutputFlags.SuppressObscured));
			character.Body.Take(item);
			item.Delete();
		}

		if (item.GetItemType<IContainer>()?.Contents.Any() == true)
		{
			character.OutputHandler.Send(
				$"{item.HowSeen(character, true)} is not empty. Are you sure you want to junk it? Type ACCEPT if so.");
			character.AddEffect(new Accept(character, new GenericProposal
			{
				AcceptAction = text => { JunkAction(); },
				DescriptionString = "junking an item",
				Keywords = new List<string> { "junk" },
				RejectAction = text => { character.Send($"You decide not to junk {item.HowSeen(character)}."); },
				ExpireAction = () => { character.Send($"You decide not to junk {item.HowSeen(character)}."); }
			}), TimeSpan.FromSeconds(90));
			return;
		}

		JunkAction();
	}

	[PlayerCommand("Open", "open")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Open(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var cmd = ss.PopSafe();
		var cmd2 = ss.PopSafe();

		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("What do you want to open?");
			return;
		}

		var exits = actor.Location.ExitsFor(actor);
		IOpenable openable = null;
		ICharacter openableOwner = null;
		var cellExits = exits as ICellExit[] ?? exits.ToArray();
		var targetExit = cellExits.GetFromItemListByKeyword(cmd, actor);
		if (targetExit != null)
		{
			if (targetExit.Exit.Door == null)
			{
				actor.OutputHandler.Send("There is no door in that direction to open.");
				return;
			}

			openable = targetExit.Exit.Door;
		}
		else if (!string.IsNullOrEmpty(cmd2))
		{
			if ((targetExit = cellExits.GetFromItemListByKeyword(cmd2, actor)) != null)
			{
				if (targetExit.Exit.Door == null)
				{
					actor.OutputHandler.Send("There is no door in that direction to open.");
					return;
				}

				if (new[] { targetExit.Exit.Door.Parent }.GetFromItemListByKeyword(cmd, actor) == null)
				{
					actor.OutputHandler.Send("There is no such door in that direction for you to open.");
					return;
				}

				openable = targetExit.Exit.Door;
			}
			else
			{
				openableOwner = actor.TargetActor(cmd);
				if (openableOwner == null)
				{
					actor.OutputHandler.Send("There is nobody here like that whose things you can open.");
					return;
				}

				if (openableOwner == actor)
				{
					Open(actor, $"{cmd2} {ss.RemainingArgument}");
					return;
				}

				if (!openableOwner.WillingToPermitInventoryManipulation(actor))
				{
					actor.OutputHandler.Send(
						$"{openableOwner.HowSeen(actor, true)} is not willing to permit you to interact with {openableOwner.ApparentGender(actor).Possessive()} inventory.");
					return;
				}

				var item = openableOwner.Body.ExternalItemsForOtherActors.Where(x => actor.CanSee(x))
				                        .GetFromItemListByKeyword(cmd2, actor);
				if (item == null)
				{
					actor.OutputHandler.Send(
						$"{openableOwner.HowSeen(actor, true)} does not have anything like that for you to open.");
					return;
				}

				openable = item.GetItemType<IOpenable>();
				if (openable == null)
				{
					actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not something that can be opened.");
					return;
				}
			}
		}
		else
		{
			var doorItem = cellExits.SelectNotNull(x => x.Exit.Door)
			                        .Select(x => x.Parent)
			                        .GetFromItemListByKeyword(cmd, actor);
			if (doorItem != null)
			{
				openable = doorItem.GetItemType<IOpenable>();
			}
			else
			{
				var item = actor.TargetItem(cmd);
				if (item != null)
				{
					openable = item.GetItemType<IOpenable>();
					if (openable == null)
					{
						actor.OutputHandler.Send(item.HowSeen(actor.Body, true) +
						                         " is not something that can be opened.");
						return;
					}
				}
			}
		}

		if (openable == null)
		{
			actor.OutputHandler.Send("You do not see that here to open.");
			return;
		}

		PlayerEmote playerEmote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			playerEmote = new PlayerEmote(emoteText, actor);
			if (!playerEmote.Valid)
			{
				actor.OutputHandler.Send(playerEmote.ErrorMessage);
				return;
			}
		}

		var (truth, error) = actor.CanManipulateItem(openable.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (actor.Body.CanOpen(openable))
		{
			actor.Body.Open(openable, openableOwner, playerEmote);
		}
		else
		{
			switch (openable.WhyCannotOpen(actor.Body))
			{
				case WhyCannotOpenReason.Jammed:
					actor.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote("@ try|tries to open $?1|$1's !0|$0|$, but it will not budge.",
							actor.Body,
							openable.Parent, openableOwner)).Append(playerEmote));
					break;
				case WhyCannotOpenReason.Locked:
					actor.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote(
							"@ try|tries to open $?1|$1's !0|$0|$, but it appears to be locked.",
							actor.Body, openable.Parent, openableOwner)).Append(playerEmote));
					break;
				case WhyCannotOpenReason.NotOpenable:
					actor.OutputHandler.Handle(
						new MixedEmoteOutput(
							new Emote(
								"@ try|tries to open $?1|$1's !0|$0|$, but it doesn't appear to be something that can opened.",
								actor.Body, openable.Parent, openableOwner)).Append(playerEmote));
					break;
				default:
					actor.OutputHandler.Send("You cannot open " + openable.Parent.HowSeen(actor.Body) + " because it" +
					                         actor.Body.WhyCannotOpen);
					break;
			}
		}
	}

	[PlayerCommand("Attach", "attach")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[HelpInfo("attach", @"The #3attach#0 command is used to attach things to other things, usually either belts or weapon attachments.

The syntax is as follows:

	#3attach <item> <target>#0 - attaches an item to a belt or weapon
	#3attach <prosthetic> [<target player>]#0 - attaches a prosthetic to a player (or yourself if not specified)",
		AutoHelp.HelpArgOrNoArg)]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Attach(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var cmd = ss.Pop();

		var targetItem = actor.TargetHeldItem(cmd);
		if (targetItem == null)
		{
			actor.OutputHandler.Send("You do not have anything like that to attach.");
			return;
		}

		var targetAsBeltable = targetItem.GetItemType<IBeltable>();
		if (targetAsBeltable == null)
		{
			if (targetItem.IsItemType<IProsthetic>())
			{
				AttachProsthetic(actor, targetItem, ss);
				return;
			}

			actor.Send("{0} is not something that can be attached to things.", targetItem.HowSeen(actor, true));
			return;
		}

		cmd = ss.Pop();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.Send("What is it that you want to attach {0} to?", targetItem.HowSeen(actor));
			return;
		}

		var targetBelt = actor.TargetItem(cmd);
		if (targetBelt == null)
		{
			actor.Send("You do not see anything like that to attach {0} to.", targetItem.HowSeen(actor));
			return;
		}

		var targetBeltAsBelt = targetBelt.GetItemType<IBelt>();
		if (targetBeltAsBelt == null)
		{
			actor.Send("{0} is not something that can have things attached to it.", targetBelt.HowSeen(actor, true));
			return;
		}

		if (!(targetBelt.Location?.CanGetAccess(targetBelt, actor) ?? true))
		{
			actor.Send(targetBelt.Location.WhyCannotGetAccess(targetBelt, actor));
			return;
		}

		PlayerEmote playerEmote = null;
		if (!ss.IsFinished)
		{
			playerEmote = new PlayerEmote(ss.PopParentheses(), actor);
			if (!playerEmote.Valid)
			{
				actor.OutputHandler.Send(playerEmote.ErrorMessage);
				return;
			}
		}

		switch (targetBeltAsBelt.CanAttachBeltable(targetAsBeltable))
		{
			case IBeltCanAttachBeltableResult.Success:
				actor.Body.Take(targetItem);
				targetBeltAsBelt.AddConnectedItem(targetAsBeltable);
				targetBelt.InInventoryOf?.RecalculateItemHelpers();
				actor.OutputHandler.Handle(
					new MixedEmoteOutput(new Emote("@ attach|attaches $0 to $1", actor, targetItem, targetBelt),
						flags: OutputFlags.SuppressObscured).Append(playerEmote)
				);
				break;
			case IBeltCanAttachBeltableResult.FailureTooLarge:
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("$0 is far too large for you to attach to $1.", actor, targetItem,
						targetBelt)), OutputRange.Personal);
				break;
			case IBeltCanAttachBeltableResult.FailureExceedMaximumNumber:
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("You cannot attached $0 because $1 has no spare room to which to attach things.",
							actor, targetItem, targetBelt)), OutputRange.Personal);
				break;
			case IBeltCanAttachBeltableResult.NotValidType:
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("You cannot attached $0 to $1 because it is not a valid type for that attachment.",
							actor, targetItem, targetBelt)), OutputRange.Personal);
				break;
		}
	}

	private static void AttachProsthetic(ICharacter actor, IGameItem targetItem, StringStack ss)
	{
		ICharacter target = null;
		target = ss.IsFinished ? actor : actor.TargetActor(ss.PopSpeech());

		if (target == null)
		{
			actor.Send($"There is nobody like that for you to attach {targetItem.HowSeen(actor)} to.");
			return;
		}

		var targetProsthetic = targetItem.GetItemType<IProsthetic>();
		if (!CanAttachProsthetic(actor, target, targetProsthetic, false))
		{
			return;
		}

		void onSuccessAction()
		{
			if (!CanAttachProsthetic(actor, target, targetProsthetic, true))
			{
				return;
			}

			void delayedAction(IPerceivable perceivable)
			{
				if (!CanAttachProsthetic(actor, target, targetProsthetic, true))
				{
					return;
				}

				actor.Body.Take(targetItem);
				target.Body.InstallProsthetic(targetProsthetic);
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							$"@ finish|finishes attaching $2 to $1's {targetProsthetic.TargetBodypart.FullDescription()}",
							actor, actor, target, targetItem)));
			}

			if (actor.IsAdministrator())
			{
				delayedAction(actor);
			}
			else
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ begin|begins attaching $2 to $1.", actor, actor, target, targetItem)));
				actor.AddEffect(
					new SimpleCharacterAction(actor, delayedAction,
						$"attaching a prosthetic to {target.HowSeen(actor, colour: false)}",
						new[] { "general", "movement" }, "attaching a prosthetic"),
					TimeSpan.FromSeconds(10));
			}
		}

		if (actor != target && !actor.IsAdministrator() && CharacterState.Able.HasFlag(target.State) &&
		    !target.IsAlly(actor))
		{
			target.AddEffect(new Accept(target, new GenericProposal(
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ accept|accepts $1's proposal to attach $2.", target, target,
							actor, targetProsthetic.Parent)));
					onSuccessAction();
				},
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines $1's proposal to attach $2.", target, target,
							actor, targetProsthetic.Parent)));
				},
				() =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines $1's proposal to attach $2.", target, target,
							actor, targetProsthetic.Parent)));
				},
				"proposal to attach a prosthetic limb", "attach", "prosthetic")));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ propose|proposes to $1 that #0 will attach $2.", actor, actor, target,
					targetProsthetic.Parent)));
			target.Send(
				$"You can use {"accept".Colour(Telnet.Yellow)} to allow {actor.ApparentGender(target).Objective()} to do this or {"decline".Colour(Telnet.Yellow)} to prevent it.");
			return;
		}

		onSuccessAction();
	}

	private static bool CanAttachProsthetic(ICharacter actor, ICharacter target, IProsthetic targetProsthetic,
		bool delayed)
	{
		if (delayed)
		{
			if (target.State.HasFlag(CharacterState.Dead))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops attaching $1 because &0's patient has died.", actor,
						actor, targetProsthetic.Parent)));
				return false;
			}

			if (!Equals(actor.Location, target.Location))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops attaching $1 because &0's patient is no longer there.",
						actor, actor, targetProsthetic.Parent)));
				return false;
			}

			if (actor.Combat != null && actor.MeleeRange)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ stop|stops trying to attach $2 to $1 because &0 is engaged in melee combat!",
							actor, actor, target,
							targetProsthetic.Parent)));
				return false;
			}

			if (target.Combat != null && target.MeleeRange)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							"@ stop|stops trying to attach $2 to $1 because &0's patient is engaged in melee combat!",
							actor, actor, target,
							targetProsthetic.Parent)));
				return false;
			}
		}

		if (!target.Body.Prototype.CountsAs(targetProsthetic.TargetBody))
		{
			if (delayed)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops trying to attach $1 to $0.", actor, target,
						targetProsthetic.Parent)));
			}

			actor.Send(
				$"{target.HowSeen(actor, true)} {(target == actor ? "are" : "is")} not the right body type to have {targetProsthetic.Parent.HowSeen(actor)} installed.");
			return false;
		}

		if (target.Body.SeveredRoots.All(x => x != targetProsthetic.TargetBodypart))
		{
			if (delayed)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops trying to attach $1 to $0.", actor, target,
						targetProsthetic.Parent)));
			}

			actor.Send(
				$"{target.HowSeen(actor, true)} {(target == actor ? "do" : "does")} not have the appropriate disability to have {targetProsthetic.Parent.HowSeen(actor)} installed. It is designed for individuals with a sever at the {targetProsthetic.TargetBodypart.FullDescription()}.");
			return false;
		}

		if (target.Body.Prosthetics.Any(x => x.TargetBodypart == targetProsthetic.TargetBodypart))
		{
			if (delayed)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops trying to attach $1 to $0.", actor, target,
						targetProsthetic.Parent)));
			}

			if (target.Body.Prosthetics.First(x => x.TargetBodypart == targetProsthetic.TargetBodypart).Obvious ||
			    actor == target || actor.IsAdministrator())
			{
				actor.Send(
					$"{target.HowSeen(actor, true)} already {(target == actor ? "have" : "has")} a prosthetic for that location. The existing one must first be removed.");
				return false;
			}

			actor.Send(
				$"{target.HowSeen(actor, true)} {(target == actor ? "do" : "does")} not have the appropriate disability to have {targetProsthetic.Parent.HowSeen(actor)} installed. It is designed for individuals with a sever at the {targetProsthetic.TargetBodypart.FullDescription()}.");
			return false;
		}

		var affectedParts =
			target.Body.Prototype.BodypartsFor(target.Race, target.Gender.Enum)
			      .Where(
				      x =>
					      x.DownstreamOfPart(targetProsthetic.TargetBodypart) ||
					      x == targetProsthetic.TargetBodypart)
			      .ToList();

		if (!actor.IsAdministrator() &&
		    target.Body.WornItems.Select(x => x.GetItemType<IWearable>())
		          .Any(
			          x =>
				          x != null && x.CurrentProfile.AllProfiles.Any(
					          y => affectedParts.Contains(y.Key) && y.Value.PreventsRemoval)))
		{
			if (delayed)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops trying to attach $1 to $0.", actor, target,
						targetProsthetic.Parent)));
			}

			actor.Send(
				$"{target.HowSeen(actor, true)} {(target == actor ? "have" : "has")} clothing obstructing the installation of {targetProsthetic.Parent.HowSeen(actor)}. You must first remove it.");
			return false;
		}

		return true;
	}

	[PlayerCommand("Detach", "detach", "detatch", "unattach")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("detatch", "Syntax: detatch <belt> <item>\n\tdetatch [<target>] <prosthetic>", AutoHelp.HelpArgOrNoArg)]
	protected static void Detach(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var cmd = ss.Pop();

		var targetBelt = actor.TargetItem(cmd);
		if (targetBelt?.GetItemType<IBelt>() == null)
		{
			targetBelt = actor.Body.Prosthetics.Select(x => x.Parent).GetFromItemListByKeyword(cmd, actor);
			if (targetBelt != null)
			{
				DetachProsthetic(actor, actor, targetBelt.GetItemType<IProsthetic>(), ss);
				return;
			}

			var targetActor = actor.TargetActor(cmd);
			if (targetActor != null)
			{
				targetBelt =
					targetActor.Body.Prosthetics.Where(x => x.Obvious || actor.IsAdministrator())
					           .Select(x => x.Parent)
					           .GetFromItemListByKeyword(ss.Peek(), actor);
				if (targetBelt != null)
				{
					ss.Pop();
					DetachProsthetic(actor, targetActor, targetBelt.GetItemType<IProsthetic>(), ss);
					return;
				}
			}

			actor.OutputHandler.Send("You do not see anything like that to detach things from.");
			return;
		}

		var targetBeltAsBelt = targetBelt.GetItemType<IBelt>();
		if (targetBeltAsBelt == null)
		{
			actor.Send("{0} is not something that can have things attached to it.", targetBelt.HowSeen(actor, true));
			return;
		}

		cmd = ss.Pop();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.Send("What is it that you want to detach from {0}?", targetBelt.HowSeen(actor));
			return;
		}

		var targetItem = targetBeltAsBelt.ConnectedItems.Select(x => x.Parent).GetFromItemListByKeyword(cmd, actor);
		if (targetItem == null)
		{
			actor.Send("{0} does not have anything like that attached to it.", targetBelt.HowSeen(actor, true));
			return;
		}

		if (!(targetBelt.Location?.CanGetAccess(targetBelt, actor) ?? true))
		{
			actor.Send(targetBelt.Location.WhyCannotGetAccess(targetBelt, actor));
			return;
		}

		PlayerEmote playerEmote = null;
		if (!ss.IsFinished)
		{
			playerEmote = new PlayerEmote(ss.PopParentheses(), actor);
			if (!playerEmote.Valid)
			{
				actor.OutputHandler.Send(playerEmote.ErrorMessage);
				return;
			}
		}

		if (!actor.Body.CanGet(targetItem, 0))
		{
			actor.Send("You do not have any free {0} with which to hold {1} once removed from {2}.",
				actor.Body.WielderDescriptionPlural, targetItem.HowSeen(actor), targetBelt.HowSeen(actor));
			return;
		}

		targetBeltAsBelt.RemoveConnectedItem(targetItem.GetItemType<IBeltable>());
		actor.Body.Get(targetItem, 0, silent: true);
		targetBelt.InInventoryOf?.RecalculateItemHelpers();
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ detach|detaches $0 from $1", actor, targetItem, targetBelt),
				flags: OutputFlags.SuppressObscured).Append(playerEmote)
		);
	}

	protected static void DetachProsthetic(ICharacter actor, ICharacter target, IProsthetic targetItem,
		StringStack ss)
	{
		bool canRemoveProsthethic()
		{
			if (actor.State.HasFlag(CharacterState.Dead))
			{
				return false;
			}

			if (!CharacterState.Able.HasFlag(actor.State))
			{
				return false;
			}

			if (!target.Body.Prosthetics.Contains(targetItem))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ stop|stops removing $2 from $1 because &1 no longer has it attached.", actor,
							actor, target, targetItem.Parent)));
				return false;
			}

			if (actor.Combat != null && actor.MeleeRange)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops removing $2 from $1 because &0 is in melee combat!",
						actor, actor, target, targetItem.Parent)));
				return false;
			}

			if (target.Combat != null && target.MeleeRange)
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ stop|stops removing $2 from $1 because &0's target is in melee combat!", actor,
							actor, target, targetItem.Parent)));
				return false;
			}

			if (target.State.HasFlag(CharacterState.Dead))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(new Emote("@ stop|stops removing $2 from $1 because &0's patient has died.",
						actor, actor, target, targetItem.Parent)));
				return false;
			}

			if (!Equals(actor.Location, target.Location))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							"@ stop|stops removing $2 from $1 because &0's patient is no longer in the same location.",
							actor, actor, target, targetItem.Parent)));
				return false;
			}

			var affectedParts =
				target.Body.Prototype.BodypartsFor(target.Race, target.Gender.Enum)
				      .Where(
					      x => x.DownstreamOfPart(targetItem.TargetBodypart) || x == targetItem.TargetBodypart)
				      .ToList();

			if (!actor.IsAdministrator() &&
			    target.Body.WornItems.Select(x => x.GetItemType<IWearable>())
			          .Any(
				          x =>
					          x != null && x.CurrentProfile.AllProfiles.Any(
						          y => affectedParts.Contains(y.Key) && y.Value.PreventsRemoval)))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote(
							"@ stop|stops removing $2 from $1 because there is clothing obstructing the procedure.",
							actor, actor, target, targetItem.Parent)));
				return false;
			}

			return true;
		}

		void onSuccessAction()
		{
			if (!canRemoveProsthethic())
			{
				return;
			}

			void delayedAction(IPerceivable perceivable)
			{
				if (!canRemoveProsthethic())
				{
					return;
				}

				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ finish|finishes removing $1 from $0", actor, target, targetItem.Parent)));

				target.Body.RemoveProsthetic(targetItem);
				if (actor.Body.CanGet(targetItem.Parent, 0))
				{
					actor.Body.Get(targetItem.Parent, 0, silent: true);
				}
				else
				{
					targetItem.Parent.RoomLayer = actor.RoomLayer;
					actor.Location.Insert(targetItem.Parent);
					actor.Send(
						$"Your {actor.Body.Prototype.WielderDescriptionPlural} are full, so you set {targetItem.Parent.HowSeen(actor)} down.");
				}
			}

			if (actor.IsAdministrator())
			{
				delayedAction(actor);
			}
			else
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ begin|begins detatching $2 from $1.", actor, actor, target, targetItem.Parent)));
				actor.AddEffect(
					new SimpleCharacterAction(actor, delayedAction,
						$"detatching a prosthetic from {target.HowSeen(actor, colour: false)}",
						new[] { "general", "movement" }, "detatching a prosthetic"),
					TimeSpan.FromSeconds(10));
			}
		}

		if (actor != target && !actor.IsAdministrator() && CharacterState.Able.HasFlag(target.State) &&
		    !target.IsAlly(actor))
		{
			target.AddEffect(new Accept(target, new GenericProposal(
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ accept|accepts $1's proposal to remove $2.", target, target,
							actor, targetItem.Parent)));
					onSuccessAction();
				},
				text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines $1's proposal to remove $2.", target, target,
							actor, targetItem.Parent)));
				},
				() =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines $1's proposal to remove $2.", target, target,
							actor, targetItem.Parent)));
				},
				"proposal to remove a prosthetic limb", "remove", "prosthetic")));
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ propose|proposes to $1 that #0 will remove $2.", actor, actor, target,
					targetItem.Parent)));
			target.Send(
				$"You can use {"accept".Colour(Telnet.Yellow)} to allow {actor.ApparentGender(target).Objective()} to do this or {"decline".Colour(Telnet.Yellow)} to prevent it.");
			return;
		}

		onSuccessAction();
	}

	[PlayerCommand("Close", "close", "cl", "clo")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Close(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var cmd = ss.PopSafe();
		var cmd2 = ss.PopSafe();

		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("What do you want to close?");
			return;
		}

		var exits = actor.Body.Location.ExitsFor(actor.Body);

		IOpenable openable = null;
		ICharacter openableOwner = null;
		var cellExits = exits as ICellExit[] ?? exits.ToArray();
		var targetExit = cellExits.GetFromItemListByKeyword(cmd, actor.Body);
		if (targetExit != null)
		{
			if (targetExit.Exit.Door == null)
			{
				actor.OutputHandler.Send("There is no door in that direction to close.");
				return;
			}

			openable = targetExit.Exit.Door;
		}
		else if (!string.IsNullOrEmpty(cmd2))
		{
			if ((targetExit = cellExits.GetFromItemListByKeyword(cmd2, actor)) != null)
			{
				if (targetExit.Exit.Door == null)
				{
					actor.OutputHandler.Send("There is no door in that direction to close.");
					return;
				}

				if (new[] { targetExit.Exit.Door.Parent }.GetFromItemListByKeyword(cmd, actor) == null)
				{
					actor.OutputHandler.Send("There is no such door in that direction for you to close.");
					return;
				}

				openable = targetExit.Exit.Door;
			}
			else
			{
				openableOwner = actor.TargetActor(cmd);
				if (openableOwner == null)
				{
					actor.OutputHandler.Send("There is nobody here like that whose things you can close.");
					return;
				}

				if (openableOwner == actor)
				{
					Close(actor, $"{cmd2} {ss.RemainingArgument}");
					return;
				}

				if (!openableOwner.WillingToPermitInventoryManipulation(actor))
				{
					actor.OutputHandler.Send(
						$"{openableOwner.HowSeen(actor, true)} is not willing to permit you to interact with {openableOwner.ApparentGender(actor).Possessive()} inventory.");
					return;
				}

				var item = openableOwner.Body.ExternalItemsForOtherActors.Where(x => actor.CanSee(x))
				                        .GetFromItemListByKeyword(cmd2, actor);
				if (item == null)
				{
					actor.OutputHandler.Send(
						$"{openableOwner.HowSeen(actor, true)} does not have anything like that for you to close.");
					return;
				}

				openable = item.GetItemType<IOpenable>();
				if (openable == null)
				{
					actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not something that can be closed.");
					return;
				}
			}
		}
		else
		{
			var doorItem =
				cellExits.SelectNotNull(x => x.Exit.Door)
				         .Select(x => x.Parent)
				         .GetFromItemListByKeyword(cmd, actor.Body);
			if (doorItem != null)
			{
				openable = doorItem.GetItemType<IOpenable>();
			}
			else
			{
				var item = actor.Body.TargetItem(cmd);
				if (item != null)
				{
					openable = item.GetItemType<IOpenable>();
					if (openable == null)
					{
						actor.OutputHandler.Send(item.HowSeen(actor.Body, true) +
						                         " is not something that can be closed.");
						return;
					}
				}
			}
		}

		if (openable == null)
		{
			actor.OutputHandler.Send("You do not see that here to close.");
			return;
		}

		PlayerEmote playerEmote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			playerEmote = new PlayerEmote(emoteText, actor.Body);
			if (!playerEmote.Valid)
			{
				actor.OutputHandler.Send(playerEmote.ErrorMessage);
				return;
			}
		}


		var (truth, error) = actor.CanManipulateItem(openable.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (actor.Body.CanClose(openable))
		{
			actor.Body.Close(openable, openableOwner, playerEmote);
		}
		else
		{
			if (openable.CanClose(actor.Body))
			{
				actor.Send(actor.Body.WhyCannotClose);
				return;
			}

			switch (openable.WhyCannotClose(actor.Body))
			{
				case WhyCannotCloseReason.Jammed:
					actor.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote("@ try|tries to close $?1|$1's !0|$0$, but it will not budge",
							actor,
							openable.Parent, openableOwner)).Append(playerEmote));
					break;
				case WhyCannotCloseReason.Locked:
					actor.OutputHandler.Handle(
						new MixedEmoteOutput(new Emote(
							"@ try|tries to close $?1|$1's !0|$0$, but it appears to be locked", actor,
							openable.Parent, openableOwner)).Append(playerEmote));
					break;
				case WhyCannotCloseReason.NotOpenable:
					actor.OutputHandler.Handle(
						new MixedEmoteOutput(
							new Emote(
								"@ try|tries to close $?1|$1's !0|$0|$, but it doesn't appear to be something that can close",
								actor, openable.Parent, openableOwner)).Append(playerEmote));
					break;
				case WhyCannotCloseReason.SingleUse:
					actor.OutputHandler.Send("You cannot close " + openable.Parent.HowSeen(actor) +
					                         " because it cannot be closed once it has been opened.");
					break;
				default:
					actor.OutputHandler.Send("You cannot close " + openable.Parent.HowSeen(actor) + " because" +
					                         actor.Body.WhyCannotClose);
					break;
			}
		}
	}

	[PlayerCommand("Lock", "lock")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[HelpInfo("lock",
		@"This command allows you to lock all of the locks that you can lock on a door or container, or optionally a specific lock. You must be holding the key to any locks that require them.

The syntax is as follows:

	#3lock <item>#0 - locks a lock item, or all locks on an item if it is lockable
	#3lock <item> <lock>#0 - locks a specific lock on an item", AutoHelp.HelpArgOrNoArg)]
	protected static void Lock(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to lock?");
			return;
		}

		var cmd = ss.PopSafe();
		var cmd2 = ss.PopSafe();

		var exits = actor.Location.ExitsFor(actor);
		IGameItem targetItem = null;
		ILockable lockable = null;
		ILock theLock = null;
		ICharacter lockableOwner = null;
		var cellExits = exits as ICellExit[] ?? exits.ToArray();
		var targetExit = cellExits.GetFromItemListByKeyword(cmd, actor);
		if (targetExit != null)
		{
			if (targetExit.Exit.Door == null)
			{
				actor.OutputHandler.Send("There is no door in that direction to lock.");
				return;
			}

			lockable = targetExit.Exit.Door;
			targetItem = targetExit.Exit.Door.Parent;
		}
		else
		{
			targetItem = actor.TargetItem(cmd);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not see that here to lock.");
				return;
			}

			lockable = targetItem.GetItemType<ILockable>();
			if (lockable == null)
			{
				theLock = targetItem.GetItemType<ILock>();
				if (theLock == null)
				{
					actor.OutputHandler.Send($"{targetItem.HowSeen(actor, true)} is not something that can be locked.");
					return;
				}
			}
		}

		if (theLock is null && !string.IsNullOrEmpty(cmd2))
		{
			if ((theLock = GetTargetLock(actor, lockable.Parent, cmd2)) == null)
			{
				return;
			}
		}

		var (truth, error) = actor.CanManipulateItem(targetItem);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		PlayerEmote playerEmote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			playerEmote = new PlayerEmote(emoteText, actor);
			if (!playerEmote.Valid)
			{
				actor.OutputHandler.Send(playerEmote.ErrorMessage);
				return;
			}
		}

		IKey whichKey = null;
		var heldKeys = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IKey>()).ToList();
		if (theLock is not null)
		{
			if (theLock.IsLocked)
			{
				actor.Send("{0} is already locked.", theLock.Parent.HowSeen(actor, true));
				return;
			}

			foreach (var key in heldKeys.Where(key => theLock.CanLock(actor, key)))
			{
				whichKey = key;
				break;
			}

			if (whichKey == null)
			{
				if (!theLock.CanLock(actor, null))
				{
					actor.Send("You cannot lock {0}.", theLock.Parent.HowSeen(actor));
					return;
				}
			}

			theLock.Lock(actor, whichKey, lockable?.Parent, playerEmote);
			return;
		}

		if (lockable is not ILock && !lockable.Locks.Any())
		{
			actor.Send("{0} does not have any locks for you to lock.", lockable.Parent.HowSeen(actor, true));
			return;
		}

		if (lockable.Locks.All(x => x.IsLocked) && (lockable as ILock)?.IsLocked != false)
		{
			actor.Send("{0} is already completely locked up.", lockable.Parent.HowSeen(actor, true));
			return;
		}

		var changedAny = false;
		foreach (var item in lockable.Locks.Where(x => !x.IsLocked).ToList())
		{
			if (item.CanLock(actor, null))
			{
				item.Lock(actor, null, lockable.Parent, playerEmote);
				changedAny = true;
				continue;
			}

			foreach (var key in heldKeys.Where(x => item.CanLock(actor, x)))
			{
				item.Lock(actor, key, lockable?.Parent, playerEmote);
				changedAny = true;
				break;
			}
		}

		if (lockable is ILock selfLock)
		{
			if (selfLock.CanLock(actor, null))
			{
				selfLock.Lock(actor, null, null, playerEmote);
				changedAny = true;
				return;
			}

			foreach (var key in heldKeys.Where(x => selfLock.CanLock(actor, x)))
			{
				selfLock.Lock(actor, key, null, playerEmote);
				changedAny = true;
				return;
			}
		}

		if (!changedAny)
		{
			actor.OutputHandler.Send($"{targetItem.HowSeen(actor, true)} does not have any locks that you can lock.");
			return;
		}
	}

	[PlayerCommand("Unlock", "unlock")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoMeleeCombatCommand]
	[HelpInfo("unlock",
		@"This command allows you to unlock all of the locks that you can unlock on a door or container, or optionally a specific lock. You must be holding the key to any locks that require them.

The syntax is as follows:

	#3unlock <item>#0 - unlocks a lock item, or all locks on an item if it is lockable
	#3unlock <item> <lock>#0 - unlocks a specific lock on an item", AutoHelp.HelpArgOrNoArg)]
	protected static void Unlock(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to unlock?");
			return;
		}

		var cmd = ss.PopSafe();
		var cmd2 = ss.PopSafe();

		var exits = actor.Location.ExitsFor(actor);
		IGameItem targetItem = null;
		ILockable lockable = null;
		ILock theLock = null;
		ICharacter lockableOwner = null;
		var cellExits = exits as ICellExit[] ?? exits.ToArray();
		var targetExit = cellExits.GetFromItemListByKeyword(cmd, actor);
		if (targetExit != null)
		{
			if (targetExit.Exit.Door == null)
			{
				actor.OutputHandler.Send("There is no door in that direction to unlock.");
				return;
			}

			lockable = targetExit.Exit.Door;
			targetItem = targetExit.Exit.Door.Parent;
		}
		else
		{
			targetItem = actor.TargetItem(cmd);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not see that here to unlock.");
				return;
			}

			lockable = targetItem.GetItemType<ILockable>();
			if (lockable == null)
			{
				theLock = targetItem.GetItemType<ILock>();
				if (theLock == null)
				{
					actor.OutputHandler.Send(
						$"{targetItem.HowSeen(actor, true)} is not something that can be unlocked.");
					return;
				}
			}
		}

		if (theLock is null && !string.IsNullOrEmpty(cmd2))
		{
			if ((theLock = GetTargetLock(actor, lockable.Parent, cmd2)) == null)
			{
				return;
			}
		}

		var (truth, error) = actor.CanManipulateItem(targetItem);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		PlayerEmote playerEmote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			playerEmote = new PlayerEmote(emoteText, actor);
			if (!playerEmote.Valid)
			{
				actor.OutputHandler.Send(playerEmote.ErrorMessage);
				return;
			}
		}

		IKey whichKey = null;
		var heldKeys = actor.Body.HeldOrWieldedItems.SelectNotNull(x => x.GetItemType<IKey>()).ToList();
		if (theLock is not null)
		{
			if (!theLock.IsLocked)
			{
				actor.Send("{0} is already unlocked.", theLock.Parent.HowSeen(actor, true));
				return;
			}

			foreach (var key in heldKeys.Where(key => theLock.CanUnlock(actor, key)))
			{
				whichKey = key;
				break;
			}

			if (whichKey == null)
			{
				if (!theLock.CanUnlock(actor, null))
				{
					actor.Send("You cannot unlock {0}.", theLock.Parent.HowSeen(actor));
					return;
				}
			}

			theLock.Unlock(actor, whichKey, lockable?.Parent, playerEmote);
			return;
		}

		if (lockable is not ILock && !lockable.Locks.Any())
		{
			actor.Send("{0} does not have any locks for you to unlock.", lockable.Parent.HowSeen(actor, true));
			return;
		}

		if (lockable.Locks.All(x => !x.IsLocked) && (lockable as ILock)?.IsLocked != true)
		{
			actor.Send("{0} is already completely unlocked.", lockable.Parent.HowSeen(actor, true));
			return;
		}

		var changedAny = false;
		foreach (var item in lockable.Locks.Where(x => x.IsLocked).ToList())
		{
			if (item.CanUnlock(actor, null))
			{
				item.Unlock(actor, null, lockable.Parent, playerEmote);
				changedAny = true;
				continue;
			}

			foreach (var key in heldKeys.Where(x => item.CanUnlock(actor, x)))
			{
				item.Unlock(actor, key, lockable?.Parent, playerEmote);
				changedAny = true;
				break;
			}
		}

		if (lockable is ILock selfLock)
		{
			if (selfLock.CanUnlock(actor, null))
			{
				selfLock.Unlock(actor, null, null, playerEmote);
				changedAny = true;
				return;
			}

			foreach (var key in heldKeys.Where(x => selfLock.CanUnlock(actor, x)))
			{
				selfLock.Unlock(actor, key, null, playerEmote);
				changedAny = true;
				return;
			}
		}

		if (!changedAny)
		{
			actor.OutputHandler.Send($"{targetItem.HowSeen(actor, true)} does not have any locks that you can unlock.");
			return;
		}
	}

	private const string LocksmithHelp =
		@"The #3locksmith#0 command is used for both legitimate and criminal manipulation of locking mechanisms. 

It may be illegal to do this command in a public place, consider using #3set lawful#0 if you're trying to do legal locksmith work to avoid any accidents.

You typically need a set of locksmithing tools to use this command.

The syntax for this command is as follows:

	#3locksmith inspect [<parent>] <lock/key>#0 - work out the key settings for a given lock or key. Can inspect a lock on a door or container.
	#3locksmith lock [<parent>] <lock>#0 - set a lock to locked without the key
	#3locksmith unlock [<parent>] <lock>#0  - set a lock to unlocked without the key
	#3locksmith set [<parent>] <lock/key> <combination>#0 - set a lock or key to use a specific numeric combination
	#3locksmith set [<parent>] <lock/key> random#0 - set a lock or key to use a random numeric combination
	#3locksmith pair [<parent>] <lock> <key>#0 - pairs a lock and key to the same numeric combination";

	[PlayerCommand("Locksmith", "locksmith")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[CommandPermission(PermissionLevel.NPC)]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("locksmith", LocksmithHelp,
		AutoHelp.HelpArgOrNoArg)]
	protected static void Locksmith(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (!actor.IsAdministrator() && !actor.Body.HeldOrWieldedItems.Any(x => x.IsItemType<ILocksmithingTool>()))
		{
			actor.Send("You must be holding a set of locksmithing tools to do any locksmithing action.");
			return;
		}

		switch (ss.Pop().ToLowerInvariant())
		{
			case "set":
				LockSmithSet(actor, ss);
				break;
			case "pair":
				LockSmithPair(actor, ss);
				break;
			case "lock":
				LockSmithLock(actor, ss);
				break;
			case "unlock":
				LockSmithUnlock(actor, ss);
				break;
			case "inspect":
				LockSmithInspect(actor, ss);
				break;
			default:
				actor.Send(LocksmithHelp.SubstituteANSIColour());
				return;
		}
	}

	[PlayerCommand("Connect", "connect")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Connect(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to connect?");
			return;
		}

		var target1 = actor.TargetItem(ss.Pop());
		if (target1 == null)
		{
			actor.Send("You do not see anything like that to connect.");
			return;
		}

		var target1Connectables = target1.GetItemTypes<IConnectable>().ToList();
		if (!target1Connectables.Any())
		{
			actor.Send($"{target1.HowSeen(actor, true)} is not something that can be connected to other things.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send($"What is it that you want to connect {target1.HowSeen(actor)} to?");
			return;
		}

		ICharacter targetActor = null;
		var target2 = actor.TargetItem(ss.Pop());
		if (target2 == null)
		{
			targetActor = actor.TargetActor(ss.Last);
			if (targetActor != null)
			{
				target2 = ss.IsFinished
					? targetActor.Body.Implants.FirstOrDefault(x => x.External && x.Parent.IsItemType<IConnectable>())
					             ?.Parent
					: targetActor.Body.Implants.Where(x => x.External && x.Parent.IsItemType<IConnectable>())
					             .Select(x => x.Parent).GetFromItemListByKeyword(ss.PopSpeech(), actor);
			}

			if (target2 == null)
			{
				actor.Send($"You do not see anything like that to connect {target1.HowSeen(actor)} to.");
				return;
			}
		}

		if (target2 == target1)
		{
			actor.Send($"You cannot connect {target1.HowSeen(actor)} to itself.");
			return;
		}

		var target2Connectables = target2.GetItemTypes<IConnectable>().ToList();
		if (!target2Connectables.Any())
		{
			actor.Send($"{target2.HowSeen(actor, true)} is not something that can be connected to other things.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target1);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		(truth, error) = actor.CanManipulateItem(target2);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		foreach (var connectable1 in target1Connectables)
		foreach (var connectable2 in target2Connectables)
		{
			if (connectable1.CanConnect(actor, connectable2))
			{
				actor.Body.Connect(connectable1, connectable2, null, targetActor, emote);
				return;
			}
		}

		actor.OutputHandler.Send(
			$"{target1.HowSeen(actor, true)} and {target2.HowSeen(actor)} cannot be connected because they have no free, compatible connections.");
	}

	[PlayerCommand("Disconnect", "disconnect")]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Disconnect(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to disconnect?");
			return;
		}

		var target1 = actor.TargetItem(ss.PopSafe());
		if (target1 == null)
		{
			actor.Send("You do not see anything like that to disconnect.");
			return;
		}

		var target1AsConnectable = target1.GetItemType<IConnectable>();
		if (target1AsConnectable == null)
		{
			actor.Send($"{target1.HowSeen(actor, true)} is not something that can be disconnect from other things.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send($"What is it that you want to disconnect {target1.HowSeen(actor)} from?");
			return;
		}

		ICharacter targetActor = null;
		var target2 = actor.TargetItem(ss.PopSafe());
		if (target2 == null)
		{
			targetActor = actor.TargetActor(ss.Last);
			if (targetActor != null)
			{
				target2 = ss.IsFinished
					? targetActor.Body.Implants.FirstOrDefault(x => x.External && x.Parent.IsItemType<IConnectable>())
					             ?.Parent
					: targetActor.Body.Implants.Where(x => x.External && x.Parent.IsItemType<IConnectable>())
					             .Select(x => x.Parent).GetFromItemListByKeyword(ss.PopSpeech(), actor);
			}

			if (target2 == null)
			{
				actor.Send($"You do not see anything like that to disconnect {target1.HowSeen(actor)} from.");
				return;
			}
		}

		if (target2 == target1)
		{
			actor.Send($"You cannot disconnect {target1.HowSeen(actor)} from itself.");
			return;
		}

		var target2AsConnectable = target2.GetItemType<IConnectable>();
		if (target2AsConnectable == null)
		{
			actor.Send(
				$"{target2.HowSeen(actor, true)} is not something that can be disconnected from other things.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target1);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		(truth, error) = actor.CanManipulateItem(target2);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		actor.Body.Disconnect(target1AsConnectable, target2AsConnectable, null, targetActor, emote);
	}

	[PlayerCommand("Select", "select")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Select(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());

		if (ss.IsFinished)
		{
			actor.Send("What option do you want to select, or what thing do you want to select an option from?");
			return;
		}

		PlayerEmote emote = null;
		var argumentText = "";
		var regexResult = ArgumentsAndEmoteRegex.Match(ss.SafeRemainingArgument ?? "");
		if (regexResult.Success)
		{
			emote = new PlayerEmote(regexResult.Groups["emote"].Value, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}

			argumentText = regexResult.Groups["arguments"].Value;
		}

		var argSS = new StringStack(argumentText);
		argSS.PopAll();

		IEnumerable<ISelectable> selectables;
		ISelectable selectable;
		if (argSS.Memory.Count() > 1)
		{
			var selectableItem =
				actor.ContextualItems.WhereNotNull(x => x.GetItemType<ISelectable>())
				     .GetFromItemListByKeyword(argSS.Memory.ElementAt(0), actor);
			if (selectableItem == null)
			{
				actor.Send("You do not see anything by that keyword that you can select things with.");
				return;
			}

			selectable = selectableItem.GetItemType<ISelectable>();
			selectables = new List<ISelectable> { selectable };
			argumentText = argumentText.RemoveFirstWord();
		}
		else
		{
			selectables = actor.ContextualItems.SelectNotNull(x => x.GetItemType<ISelectable>()).ToList();
			selectable = selectables.FirstOrDefault();
		}

		if (selectable == null)
		{
			actor.Send("There is nothing in your location or in your possession that has options to be selected.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(selectable.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!selectable.CanSelect(actor, argumentText) && selectables.Count() > 1)
		{
			foreach (var item in selectables)
			{
				if (item.CanSelect(actor, argumentText))
				{
					item.Select(actor, argumentText, emote);
					return;
				}
			}
		}

		selectable.Select(actor, argumentText, emote);
	}

	[PlayerCommand("Insert", "insert")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Insert(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to insert?");
			return;
		}

		var target = actor.TargetHeldItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You aren't holding anything like that which you can insert.");
			return;
		}

		IGameItem insertable;
		if (ss.IsFinished)
		{
			insertable = actor.ContextualItems.FirstOrDefault(x => x.IsItemType<IInsertable>());
			if (insertable == null)
			{
				actor.Send("There is nothing nearby that you can insert {0} into.", target.HowSeen(actor));
				return;
			}
		}
		else
		{
			insertable = actor.TargetItem(ss.PopSpeech());
			if (insertable == null)
			{
				actor.Send("You do not see anything like that to insert {0} into.", target.HowSeen(actor));
				return;
			}

			if (!insertable.IsItemType<IInsertable>())
			{
				actor.Send("{0} is not the sort of thing that can have anything inserted into it.",
					target.HowSeen(actor, true));
				return;
			}
		}

		var (truth, error) = actor.CanManipulateItem(insertable);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		PlayerEmote emote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		insertable.GetItemType<IInsertable>().Insert(actor, target, emote);
	}

	[PlayerCommand("Switch", "switch")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("switch", @"The #3switch#0 command allows you to activate a switch or dial on an item to change it to a particular setting.

The syntax is as follows:

	#3switch <item>#0 - see a list of possible switch options for an item
	#3switch <item> <option>#0 - switch an item to the specified option (e.g. on, off, safe, etc.)", AutoHelp.HelpArgOrNoArg)]
	protected static void Switch(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.Send("What is it that you want to switch?");
			return;
		}

		var target = actor.TargetItem(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You can't see anything like that which can be switched.");
			return;
		}

		var switchables = target
		                  .GetItemTypes<ISwitchable>()
		                  .Where(x => x.SwitchSettings.Any())
		                  .ToList();
		if (!switchables.Any())
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be switched.");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		PlayerEmote emote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		if (ss.IsFinished)
		{
			actor.Send(
				$"{target.HowSeen(actor, true)} has the following options for switch: {switchables.SelectMany(x => x.SwitchSettings.Select(y => y.ColourCommand())).Distinct().ListToString()}.");
			return;
		}

		var switchText = ss.PopSpeech();
		var canSwitchables = switchables.Where(x => x.CanSwitch(actor, switchText)).ToList();
		if (!canSwitchables.Any())
		{
			if (!switchables.SelectMany(x => x.SwitchSettings).Distinct().Any(x => x.EqualTo(switchText)))
			{
				actor.Send("");
				return;
			}

			var sb = new StringBuilder();
			foreach (var switchable in switchables.Where(x => !canSwitchables.Contains(x)))
			{
				if (!switchable.SwitchSettings.Any(x => x.EqualTo(switchText)))
				{
					continue;
				}

				sb.AppendLine(switchable.WhyCannotSwitch(actor, switchText));
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote($"@ switch|switches $0 to '{ss.Last.ToLowerInvariant()}'", actor,
				target)).Append(emote));
		foreach (var switchable in canSwitchables)
		{
			switchable.Switch(actor, switchText);
		}
	}

	#region Locksmithing Sub-commands

#nullable enable
	private static ILock? GetTargetLock(ICharacter actor, IGameItem target, string lockName)
	{
		if (target.IsItemType<ILockable>())
		{
			var lockable = target.GetItemType<ILockable>();
			IGameItem? targetLockItem;
			if (!string.IsNullOrEmpty(lockName))
			{
				targetLockItem = lockable.Locks.Select(x => x.Parent).GetFromItemListByKeyword(lockName, actor);
				if (targetLockItem == null)
				{
					actor.Send("You do not see any such lock on {0}.", target.HowSeen(actor));
					return null;
				}

				return targetLockItem.GetItemType<ILock>();
			}

			if (target.IsItemType<ILock>())
			{
				return target.GetItemType<ILock>();
			}

			// Otherwise, return the only lock on the target
			targetLockItem = lockable.Locks.FirstOrDefault()?.Parent;
			var theLock = targetLockItem?.GetItemType<ILock>();
			if (theLock is null)
			{
				actor.OutputHandler.Send($"{target.HowSeen(actor, true)} does not have any locks on it.");
			}
		}

		if (target.IsItemType<ILock>())
		{
			return target.GetItemType<ILock>();
		}

		actor.Send("{0} has no locking mechanism.", target.HowSeen(actor, true));
		return null;
	}
#nullable restore

	private static void LockSmithSet(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which lock do you want to set the combination for?");
			return;
		}

		var targetName = ss.PopSpeech();
		var target = actor.TargetItem(targetName);
		if (target == null)
		{
			actor.Send("You do not see anything like that here.");
			return;
		}

		var patternName = ss.PopSpeech();
		var lockName = string.Empty;

		if (!patternName.Equals("random", StringComparison.InvariantCultureIgnoreCase)
		    && !int.TryParse(patternName, out var newCombination))
		{
			lockName = patternName;
			patternName = ss.PopSpeech();
		}

		var targetLock = GetTargetLock(actor, target, lockName);
		if (targetLock == null)
		{
			return;
		}

		//Verify the lock is unlocked and that the container is open
		if (targetLock.IsLocked && !actor.IsAdministrator())
		{
			actor.Send("You cannot change the combination on a lock when it is locked.");
			return;
		}

		if (target.GetItemType<IOpenable>()?.IsOpen == false && !actor.IsAdministrator())
		{
			actor.Send("You must open {0} before changing locks on it.", target.HowSeen(actor));
			return;
		}

		if (patternName.Equals("random", StringComparison.InvariantCultureIgnoreCase))
		{
			newCombination = RandomUtilities.Random(0, 1000000);
		}
		else
		{
			if (!int.TryParse(patternName, out newCombination))
			{
				actor.Send("What combination do you want to set this lock to? Either specify {0} or a number.",
					"random".Colour(Telnet.Yellow));
				return;
			}
		}

		var (truth, error) = actor.CanManipulateItem(targetLock.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var timespan = TimeSpan.FromSeconds(30);

		void finalAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Any(x => x.IsItemType<ILocksmithingTool>()))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				return;
			}

			//Sanity check that everything is still valid. States could have changed while we were tinkering
			if (targetLock.IsLocked && !actor.IsAdministrator())
			{
				actor.Send("You cannot change the combination on a lock when it is locked.");
				return;
			}

			if (target.GetItemType<IOpenable>()?.IsOpen == false && !actor.IsAdministrator())
			{
				actor.Send("You must open {0} before changing locks on it.", target.HowSeen(actor));
				return;
			}

			var finalOutcome = actor.Gameworld.GetCheck(CheckType.LocksmithingCheck)
			                        .Check(actor, Difficulty.Easy, targetLock.Parent);
			if (finalOutcome.IsPass() || actor.IsAdministrator())
			{
				targetLock.Pattern = newCombination;
				actor.Send($"You set the combination to {newCombination:N0}.");
			}
			else
			{
				actor.Send($"You fail to manipulate the lock into a new combination.");
			}

			actor.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ finish|finishes working on the combination for $0.", actor, targetLock.Parent),
				flags: OutputFlags.SuppressObscured));
		}

		void intermediateAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Any(x => x.IsItemType<ILocksmithingTool>()))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ continue|continues to tinker with $0.", actor, targetLock.Parent),
					flags: OutputFlags.SuppressObscured));
		}

		if (!actor.IsAdministrator())
		{
			actor.AddEffect(new MultiStageBlockingDelayedAction(actor, new Action<IPerceivable>[]
			{
				intermediateAction,
				intermediateAction,
				intermediateAction,
				finalAction
			}, "setting a combination for a lock", new[] { "general", "movement" }, 4, timespan), timespan);
			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ begin|begins to tinker with $0.", actor, targetLock.Parent),
					flags: OutputFlags.SuppressObscured));
		}
		else
		{
			finalAction(actor);
		}
	}

	private static void LockSmithPair(ICharacter actor, StringStack ss)
	{
		if (ss.CountRemainingArguments() < 2)
		{
			actor.Send(StringUtilities.HMark + "Syntax: locksmith pair <lock> <key>");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which lock do you want to pair?");
			return;
		}

		var targetName = ss.PopSpeech();

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which key do you want to pair that lock with?");
			return;
		}

		var lockName = ss.PopSpeech();
		var keyName = string.Empty;

		if (ss.IsFinished)
		{
			//If we're out of arguments, then the last argument is treated as the key, not the lock, so shuffle
			//arguments around
			keyName = lockName;
			lockName = string.Empty;
		}
		else
		{
			keyName = ss.PopSpeech();
		}

		var target = actor.TargetItem(targetName);
		if (target == null)
		{
			actor.Send("You do not see anything like that here.");
			return;
		}

		var targetLock = GetTargetLock(actor, target, lockName);
		if (targetLock == null)
		{
			return;
		}

		var (truth, error) = actor.CanManipulateItem(targetLock.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		//Verify the lock is unlocked and that the container is open
		if (targetLock.IsLocked && !actor.IsAdministrator())
		{
			actor.Send("You cannot pair a key to a lock that is currently locked.");
			return;
		}

		if (targetLock.Pattern == 0)
		{
			actor.Send("{0} has no combination set so cannot have a key paired to it.",
				targetLock.Parent.HowSeen(actor, true));
			return;
		}

		if (target.GetItemType<IOpenable>()?.IsOpen == false && !actor.IsAdministrator())
		{
			actor.Send("You must open {0} before pairing keys to locks on it.", target.HowSeen(actor));
			return;
		}

		var targetKeyItem = actor.TargetHeldItem(keyName);
		if (targetKeyItem == null)
		{
			actor.Send("You are not holding any such key.");
			return;
		}

		if (!targetKeyItem.IsItemType<IKey>() || targetKeyItem.IsItemType<IKeyring>())
		{
			actor.Send("{0} is not a key.", targetKeyItem.HowSeen(actor, true));
			return;
		}

		var targetKey = targetKeyItem.GetItemType<IKey>();

		if (targetKey.LockType == null)
		{
			actor.Send("{0} is not configured to work with any type of lock. Please report this build error to staff.",
				targetKeyItem.HowSeen(actor, true));
			return;
		}

		if (!targetKey.LockType.Equals(targetLock.LockType, StringComparison.InvariantCultureIgnoreCase))
		{
			actor.Send("{0} will not work with {1}. It is designed to work with {2} style locks.",
				targetKeyItem.HowSeen(actor, true), targetLock.Parent.HowSeen(actor),
				targetKey.LockType);
			return;
		}

		if (targetKey.Pattern == targetLock.Pattern)
		{
			actor.Send("{0} is already paired with {1}.", targetKeyItem.HowSeen(actor, true),
				targetLock.Parent.HowSeen(actor));
			return;
		}

		var attemptingRekey = false;
		if (targetKey.Pattern != 0 && !actor.IsAdministrator())
		{
			//Attempting to re-pair a key that's already been paired is extremely hard and destroys the
			//key on failure.
			attemptingRekey = true;
			actor.Send("You are attempting to rekey {0}. This is a difficult process and may destroy the key...",
				targetKeyItem.HowSeen(actor));
		}

		var timespan = TimeSpan.FromSeconds(30);

		void finalAction(IPerceivable perceivable)
		{
			if (!actor.Body.HeldItems.Contains(targetKeyItem))
			{
				actor.Send("You stop tinkering as you seem to have lost the key.");
				return;
			}

			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Any(x => x.IsItemType<ILocksmithingTool>()))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				return;
			}

			//Sanity check that everything is still valid. States could have changed while we were tinkering
			if (targetLock.IsLocked && !actor.IsAdministrator())
			{
				actor.Send("You cannot pair a key to a lock that is currently locked.");
				return;
			}

			if (targetLock.Pattern == 0)
			{
				actor.Send("{0} has no combination set so cannot have a key paired to it.",
					targetLock.Parent.HowSeen(actor, true));
				return;
			}

			if (target.GetItemType<IOpenable>()?.IsOpen == false && !actor.IsAdministrator())
			{
				actor.Send("You must open {0} before pairing keys to locks on it.", target.HowSeen(actor));
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ finish|finishes trying to pair $0 with $1.", actor, targetLock.Parent, targetKeyItem),
					flags: OutputFlags.SuppressObscured));

			var checkOutcome = actor.Gameworld.GetCheck(CheckType.LocksmithingCheck)
			                        .Check(actor, attemptingRekey ? Difficulty.VeryHard : Difficulty.Easy,
				                        targetLock.Parent);

			if (checkOutcome.IsPass() || actor.IsAdministrator())
			{
				targetKey.Pattern = targetLock.Pattern;
				actor.Send("You pair {0} and {1} together, both now sharing pattern {2:N0}.",
					targetLock.Parent.HowSeen(actor), targetKeyItem.HowSeen(actor), targetLock.Pattern);
			}
			else
			{
				if (attemptingRekey && checkOutcome.FailureDegrees() > 1)
				{
					actor.Send("You fail to pair {0} and {1} together and ruin the key in the process.",
						targetLock.Parent.HowSeen(actor), targetKeyItem.HowSeen(actor));
					targetKeyItem.Die();
				}
				else
				{
					actor.Send("You fail to pair {0} and {1} together but the key is still usable.",
						targetLock.Parent.HowSeen(actor), targetKeyItem.HowSeen(actor));
				}
			}
		}

		void intermediateAction(IPerceivable perceivable)
		{
			if (!actor.Body.HeldItems.Contains(targetKeyItem))
			{
				actor.Send("You stop tinkering as you seem to have lost the key.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Any(x => x.IsItemType<ILocksmithingTool>()))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ continue|continues to tinker with $0 and $1.", actor, targetLock.Parent,
						targetKeyItem),
					flags: OutputFlags.SuppressObscured));
		}

		if (!actor.IsAdministrator())
		{
			actor.AddEffect(new MultiStageBlockingDelayedAction(actor, new Action<IPerceivable>[]
			{
				intermediateAction,
				intermediateAction,
				intermediateAction,
				finalAction
			}, "pairing a lock and key", new[] { "general", "movement" }, 4, timespan), timespan);
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ begin|begins to tinker with $0 and $1.", actor, targetLock.Parent, targetKeyItem),
					flags: OutputFlags.SuppressObscured));
		}
		else
		{
			finalAction(actor);
		}
	}

	private static void LockSmithUnlock(ICharacter actor, StringStack ss)
	{
		if (ss.CountRemainingArguments() < 1)
		{
			actor.Send(StringUtilities.HMark + "Syntax: locksmith unlock <lock>");
			return;
		}

		var targetName = ss.Pop();
		var lockName = ss.IsFinished ? string.Empty : ss.Pop();

		var target = actor.TargetItem(targetName);
		if (target == null)
		{
			actor.Send("You do not see anything like that here.");
			return;
		}

		var targetLock = GetTargetLock(actor, target, lockName);
		if (targetLock == null)
		{
			return;
		}

		var (truth, error) = actor.CanManipulateItem(targetLock.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!targetLock.IsLocked)
		{
			actor.Send("{0} is not locked.", targetLock.Parent.HowSeen(actor, true));
			return;
		}

		var lockTool = actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ILocksmithingTool>()).FirstOrDefault();
		var timespan = TimeSpan.FromSeconds(45);

		void finalAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Contains(lockTool?.Parent))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				return;
			}

			if (!targetLock.IsLocked)
			{
				actor.Send("{0} is not locked.", targetLock.Parent.HowSeen(actor, true));
				return;
			}

			var outcome = actor.Gameworld.GetCheck(CheckType.LocksmithingCheck)
			                   .Check(actor, targetLock.PickDifficulty.StageDown(lockTool?.DifficultyAdjustment ?? 0),
				                   targetLock.Parent);
			if (outcome.IsPass() || actor.IsAdministrator())
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ finish|finishes unlocking $0 with $1.", actor, targetLock.Parent,
							lockTool?.Parent), flags: OutputFlags.SuppressObscured));
				targetLock.SetLocked(false, true);
			}
			else if (outcome.Outcome == Outcome.MajorFail && (lockTool?.Breakable ?? false))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ fail|fails to unlock $0, and break|breaks $1 in the process.", actor,
							targetLock.Parent, lockTool?.Parent), flags: OutputFlags.SuppressObscured));
				lockTool?.Parent.Die();
			}
			else
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ fail|fails to unlock $0 with $1.", actor, targetLock.Parent,
							lockTool?.Parent), flags: OutputFlags.SuppressObscured));
			}
		}

		void intermediateAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Contains(lockTool?.Parent))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!targetLock.IsLocked)
			{
				actor.Send("{0} is not locked.", targetLock.Parent.HowSeen(actor, true));
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ continue|continues to unlock $0.", actor, targetLock.Parent),
					flags: OutputFlags.SuppressObscured));
		}

		if (!actor.IsAdministrator())
		{
			actor.AddEffect(new MultiStageBlockingDelayedAction(actor, new Action<IPerceivable>[]
			{
				intermediateAction,
				intermediateAction,
				finalAction
			}, "picking a lock", new[] { "general", "movement" }, 3, timespan), timespan);
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ begin|begins to unlock $0 with $1.", actor, targetLock.Parent,
						lockTool?.Parent), flags: OutputFlags.SuppressObscured));
		}
		else
		{
			finalAction(actor);
		}
	}

	private static void LockSmithLock(ICharacter actor, StringStack ss)
	{
		if (ss.CountRemainingArguments() < 1)
		{
			actor.Send(StringUtilities.HMark + "Syntax: locksmith lock <lock>");
			return;
		}

		var targetName = ss.PopSpeech();
		var lockName = ss.IsFinished ? string.Empty : ss.Pop();

		var target = actor.TargetItem(targetName);
		if (target == null)
		{
			actor.Send("You do not see anything like that here.");
			return;
		}

		var targetLock = GetTargetLock(actor, target, lockName);
		if (targetLock == null)
		{
			return;
		}

		var (truth, error) = actor.CanManipulateItem(targetLock.Parent);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (targetLock.IsLocked)
		{
			actor.Send("{0} is already locked.", targetLock.Parent.HowSeen(actor, true));
			return;
		}

		var lockTool = actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ILocksmithingTool>()).FirstOrDefault();
		var timespan = TimeSpan.FromSeconds(30);

		void finalAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Contains(lockTool?.Parent))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				return;
			}

			if (targetLock.IsLocked)
			{
				actor.Send("{0} is already locked.", targetLock.Parent.HowSeen(actor, true));
				return;
			}

			var outcome = actor.Gameworld.GetCheck(CheckType.LocksmithingCheck)
			                   .Check(actor, targetLock.PickDifficulty.StageDown(lockTool?.DifficultyAdjustment ?? 0),
				                   targetLock.Parent);

			if (outcome.IsPass() || actor.IsAdministrator())
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ finish|finishes locking $0 with $1.", actor, targetLock.Parent,
							lockTool?.Parent), flags: OutputFlags.SuppressObscured));
				targetLock.SetLocked(true, true);
			}
			else if (outcome.Outcome == Outcome.MajorFail && (lockTool?.Breakable ?? false))
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ fail|fails to lock $0, and break|breaks $1 in the process.", actor,
							targetLock.Parent, lockTool?.Parent), flags: OutputFlags.SuppressObscured));
				lockTool?.Parent.Die();
			}
			else
			{
				actor.OutputHandler.Handle(
					new EmoteOutput(
						new Emote("@ fail|fails to unlock $0 with $1.", actor, targetLock.Parent,
							lockTool?.Parent), flags: OutputFlags.SuppressObscured));
			}
		}

		void intermediateAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(targetLock.Parent) || !actor.CanSee(target))
			{
				actor.Send("You stop tinkering as you seem to have lost the lock.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Contains(lockTool?.Parent))
			{
				actor.Send("You stop tinkering as you are not holding a set of locksmithing tools.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (targetLock.IsLocked)
			{
				actor.Send("{0} is already locked.", targetLock.Parent.HowSeen(actor, true));
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ continue|continues to unlock $0.", actor, targetLock.Parent),
					flags: OutputFlags.SuppressObscured));
		}

		if (!actor.IsAdministrator())
		{
			actor.AddEffect(new MultiStageBlockingDelayedAction(actor, new Action<IPerceivable>[]
			{
				intermediateAction,
				intermediateAction,
				finalAction
			}, "picking a lock", new[] { "genera", "movement" }, 3, timespan), timespan);
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ begin|begins to lock $0 with $1.", actor, targetLock.Parent,
						lockTool?.Parent), flags: OutputFlags.SuppressObscured));
		}
		else
		{
			finalAction(actor);
		}
	}

	private static void LockSmithInspect(ICharacter actor, StringStack ss)
	{
		if (ss.CountRemainingArguments() < 1)
		{
			actor.Send(StringUtilities.HMark + "Syntax: locksmith inspect <lock or key>");
			return;
		}

		var targetName = ss.PopSpeech();
		var lockName = ss.IsFinished ? string.Empty : ss.PopSpeech();

		var target = actor.TargetItem(targetName);

		if (target == null)
		{
			actor.Send("You do not see anything like that here.");
			return;
		}

		if (target.IsItemType<IKeyring>())
		{
			actor.OutputHandler.Send($"You cannot use this command with keyrings.");
			return;
		}

		ILock targetLock;
		if (!target.IsItemType<IKey>())
		{
			//Item isn't a key, so assume it's a lock and try to find it
			targetLock = GetTargetLock(actor, target, lockName);

			if (targetLock == null)
			{
				return;
			}

			target = targetLock.Parent;
		}

		var (truth, error) = actor.CanManipulateItem(target);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var lockTool = actor.Body.HeldItems.SelectNotNull(x => x.GetItemType<ILocksmithingTool>()).FirstOrDefault();
		var timespan = TimeSpan.FromSeconds(20);

		void finalAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(target))
			{
				actor.Send("You stop inspecting as you seem to have lost track of what you were inspecting.");
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Contains(lockTool?.Parent))
			{
				actor.Send("You stop inspecting as you are not holding a set of locksmithing tools.");
				return;
			}

			//generate output here
			var output = string.Empty;
			if (target.IsItemType<IKey>())
			{
				output = target.GetItemType<IKey>().Inspect(actor, output);
				actor.Send(output);
			}
			else if (target.IsItemType<ILock>())
			{
				output = target.GetItemType<ILock>().Inspect(actor, output);
				actor.Send(output);
			}
			else
			{
				throw new ApplicationException("LockSmithInspect() - unexpected target.");
			}
		}

		void intermediateAction(IPerceivable perceivable)
		{
			if (!actor.CanSee(target))
			{
				actor.Send("You stop inspecting as you seem to have lost track of what you were inspecting.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			if (!actor.IsAdministrator() && !actor.Body.HeldItems.Contains(lockTool?.Parent))
			{
				actor.Send("You stop inspecting as you are not holding a set of locksmithing tools.");
				actor.RemoveAllEffects(
					x => x.IsEffectType<MultiStageBlockingDelayedAction>() && x.IsBlockingEffect("general"));
				return;
			}

			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ continue|continues to inspect $0.", actor, target),
					flags: OutputFlags.SuppressObscured));
		}

		if (!actor.IsAdministrator())
		{
			actor.AddEffect(new MultiStageBlockingDelayedAction(actor, new Action<IPerceivable>[]
			{
				intermediateAction,
				intermediateAction,
				finalAction
			}, "inspecting a lock", new[] { "general", "movement" }, 3, timespan), timespan);
			actor.OutputHandler.Handle(
				new EmoteOutput(
					new Emote("@ begin|begins to inspect $0.", actor, target),
					flags: OutputFlags.SuppressObscured));
		}
		else
		{
			finalAction(actor);
		}
	}

	#endregion

	[PlayerCommand("Sharpen", "sharpen")]
	protected static void Sharpen(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send(
				$"The syntax for the sharpen command is {"sharpen <thing to be sharpened> <thing doing the sharpening>".Colour(Telnet.Yellow)}.");
			return;
		}

		var target = actor.TargetHeldItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to sharpen.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What do you want to sharpen {0} with?", target.HowSeen(actor));
			return;
		}

		var sharpener = actor.TargetItem(ss.Pop());
		if (sharpener == null)
		{
			actor.Send("You don't see anything like that with which to sharpen {0}.", target.HowSeen(actor));
			return;
		}

		var (truth, error) = actor.CanManipulateItem(sharpener);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var sharpenerAsSharpen = sharpener.GetItemType<ISharpen>();
		if (sharpenerAsSharpen == null)
		{
			actor.Send("{0} is not something that can sharpen things.", target.HowSeen(actor));
			return;
		}

		if (!sharpenerAsSharpen.CanSharpen(actor, target))
		{
			actor.Send(sharpenerAsSharpen.WhyCannotSharpen(actor, target));
			return;
		}

		sharpenerAsSharpen.Sharpen(actor, target);
	}

	[PlayerCommand("Lob", "lob")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[HelpInfo("lob",
		"The lob command allows you to lob an item into another room or layer. You can lob the item into the room without regard to where it lands, or you can specify a target in that room to try to lob it near. The syntax is LOB <item> <direction> [<target>] or LOB <item> <layer> [<target>].",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Lob(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var tossable = actor.TargetHeldItem(ss.PopSpeech());
		if (tossable == null)
		{
			actor.OutputHandler.Send("You don't have anything like that to lob.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"In which layer or direction do you want to lob {tossable.HowSeen(actor)}?");
			return;
		}

		var whereText = ss.PopSpeech();

		ICellExit exit = null;
		if (!Enum.TryParse<RoomLayer>(whereText, true, out var layer))
		{
			exit = actor.Location.GetExitKeyword(whereText, actor);
			if (exit == null)
			{
				actor.Send("There is no exit in that direction that you can see.");
				return;
			}
		}

		if (exit?.Exit.Door is not null && !exit.Exit.Door.IsOpen && !exit.Exit.Door.CanFireThrough)
		{
			actor.OutputHandler.Send($"You can't lob anything through {exit.Exit.Door.Parent.HowSeen(actor)}.");
			return;
		}

		IPerceiver target = null;
		var targetText = ss.PopSafe();
		if (!string.IsNullOrEmpty(targetText))
		{
			if (exit is not null)
			{
				target = exit.Destination.Characters.OfType<IPerceiver>().Concat(exit.Destination.GameItems)
				             .Where(x => actor.CanSee(x)).GetFromItemListByKeyword(targetText, actor);
				if (target == null)
				{
					actor.OutputHandler.Send(
						$"You can't see anyone or anything like that to {exit.OutboundDirectionDescription.ColourName()}.");
					return;
				}
			}
			else
			{
				target = actor.Location
				              .LayerCharacters(layer)
				              .Cast<IPerceiver>()
				              .Concat(actor.Location.LayerGameItems(layer))
				              .Where(x => actor.CanSee(x))
				              .GetFromItemListByKeyword(targetText, actor);
				if (target == null)
				{
					actor.OutputHandler.Send(
						$"You can't see anyone or anything like that {layer.LocativeDescription().ColourName()}.");
					return;
				}
			}
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		var difficulty = actor.ArmourUseCheckDifficulty(ArmourUseDifficultyContext.LobItem);
		string tossEmote;
		var minimumSuccessThreshold = Outcome.MinorPass;
		if (exit is not null)
		{
			tossEmote = $"@ lob|lobs $1 {exit.OutboundMovementSuffix}$?2| at $2||$";
			difficulty = difficulty.StageUp(1);
		}
		else
		{
			if (layer.IsHigherThan(actor.RoomLayer))
			{
				minimumSuccessThreshold = minimumSuccessThreshold.StageUp(1);
			}

			if (actor.Location.IsUnderwaterLayer(actor.RoomLayer))
			{
				minimumSuccessThreshold = minimumSuccessThreshold.StageUp(1);
				difficulty = difficulty.StageUp(1);
				if (!actor.Location.IsSwimmingLayer(layer))
				{
					minimumSuccessThreshold = minimumSuccessThreshold.StageUp(1);
					difficulty = difficulty.StageUp(2);
				}
			}

			tossEmote = $"@ lob|lobs $1 {layer.DativeDescription()}$?2| at $2||$";
		}

		if (target is not null)
		{
			difficulty = difficulty.StageUp(1);
		}

		var check = actor.Gameworld.GetCheck(CheckType.TossItemCheck);
		var result = check.Check(actor, difficulty);

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(tossEmote, actor, actor, tossable, target)).Append(emote));

		if (result.Outcome < minimumSuccessThreshold)
		{
			if (result.Outcome == Outcome.MajorFail)
			{
				if (actor.Location.IsSwimmingLayer(actor.RoomLayer))
				{
					actor.OutputHandler.Handle(new EmoteOutput(new Emote(
						"&0 completely flub|flubs &0's lobbing of $1, and it lands in the water nearby!", actor, actor,
						tossable)));
				}
				else
				{
					actor.OutputHandler.Handle(new EmoteOutput(new Emote(
						"&0 completely flub|flubs &0's lobbing of $1, and it lands at &0's feet!", actor, actor,
						tossable)));
				}

				actor.Body.Take(tossable);
				tossable.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(tossable, true);
				tossable.SetTarget(actor);
				return;
			}

			if (exit == null)
			{
				var intermediates = actor.Location.Terrain(actor).TerrainLayers
				                         .IntermediateLayers(actor.RoomLayer, layer).ToList();
				if (intermediates.Any())
				{
					var random = intermediates.GetRandomElement();
					actor.OutputHandler.Handle(new EmoteOutput(new Emote(
						$"&0's lob is too short, and $1 lands {random.LocativeDescription()} instead.", actor, actor,
						tossable)));
					actor.Body.Take(tossable);
					tossable.RoomLayer = random;
					actor.Location.Insert(tossable, true);
					return;
				}
			}

			if (actor.Location.IsSwimmingLayer(actor.RoomLayer))
			{
				actor.OutputHandler.Handle(new EmoteOutput(new Emote(
					"&0's lob is too short, and $1 splashes into the water some distance away.", actor, actor,
					tossable)));
			}
			else
			{
				actor.OutputHandler.Handle(new EmoteOutput(
					new Emote("&0's lob is too short, and $1 lands on the ground here.", actor, actor, tossable)));
			}

			actor.Body.Take(tossable);
			tossable.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(tossable, true);
			if (exit != null)
			{
				tossable.AddEffect(new AdjacentToExit(tossable, exit), AdjacentToExit.DefaultEffectTimeSpan);
			}

			return;
		}

		actor.Body.Take(tossable);
		if (target != null && result.Outcome != Outcome.MajorPass)
		{
			var potentialTargets = (exit == null
				                       ? actor.Location.LayerCharacters(layer).OfType<IPerceiver>()
				                              .Concat(actor.Location.LayerGameItems(layer))
				                       : exit.Destination.LayerCharacters(target.RoomLayer).OfType<IPerceiver>()
				                             .Concat(exit.Destination.LayerGameItems(target.RoomLayer)))
			                       .Except(target)
			                       .ToList();
			if (potentialTargets.Any() && Dice.Roll(1, 2) == 1)
			{
				// Scatter to a new target
				target = potentialTargets.GetWeightedRandom(x =>
					x is ICharacter ch
						? (int)ch.CurrentContextualSize(SizeContext.RangedTarget)
						: (int)((IGameItem)x).Size);
				actor.OutputHandler.Send(
					$"Your lob landed a bit wide of your target and scattered to {target.HowSeen(actor)} instead!");
			}
			else
			{
				// Land vague
				target = null;
				actor.OutputHandler.Send("Your lob landed a bit wide of your target.");
			}
		}

		if (exit == null)
		{
			tossable.RoomLayer = layer;
			actor.Location.Insert(tossable);
			if (layer.IsLowerThan(actor.RoomLayer))
			{
				tossable.OutputHandler.Handle(new EmoteOutput(
					new Emote($"@ fly|flies in fom above$?1|, ending up right by $1||$.", tossable, tossable, target)));
			}
			else
			{
				tossable.OutputHandler.Handle(new EmoteOutput(new Emote(
					$"@ fly|flies in from below$?1|, ending up right by $1||$.", tossable, tossable, target)));
			}
		}
		else
		{
			tossable.RoomLayer = target?.RoomLayer ?? actor.RoomLayer;
			exit.Destination.Insert(tossable);
			tossable.SetTarget(target);
			tossable.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ fly|flies in {exit.InboundDirectionSuffix} $?1|and lands right by $1|and lands on the ground|$.",
				tossable, tossable, target)));
		}

		actor.AddEffect(
			new CommandDelay(actor, "Lob",
				onExpireAction: () => { actor.Send("You feel as if you could lob something again."); }),
			TimeSpan.FromSeconds(10));
	}

	[PlayerCommand("Bundle", "bundle")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	protected static void Bundle(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("You must specify items to bundle.");
			return;
		}

		var items = new List<IGameItem>();
		while (!ss.IsFinished)
		{
			var target = actor.TargetLocalOrHeldItem(ss.PopSpeech());
			if (target == null)
			{
				actor.OutputHandler.Send(
					$"You don't see anything with the keyword {ss.Last.ColourCommand()} to bundle.");
				return;
			}

			if (items.Contains(target))
			{
				actor.OutputHandler.Send($"You cannot include {target.HowSeen(actor)} more than once in your bundle.");
				return;
			}

			if (!target.CanBeBundled)
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} is not something that can be put into bundles.");
				return;
			}

			if (!actor.Location.CanGet(target, actor))
			{
				actor.OutputHandler.Send(actor.Location.WhyCannotGet(target, actor));
				return;
			}

			items.Add(target);
		}

		if (items.Count < 2)
		{
			actor.OutputHandler.Send("You must have at least two items in your bundle.");
			return;
		}

		foreach (var item in items)
		{
			item.InInventoryOf?.Take(item);
			item.ContainedIn?.Take(item);
			item.Location?.Extract(item);
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ bundle|bundles $1 into a single pile.", actor, actor,
			new PerceivableGroup(items))));
		var bundle = PileGameItemComponentProto.CreateNewBundle(items);
		actor.Gameworld.Add(bundle);
		if (actor.Body.CanGet(bundle, 0))
		{
			actor.Body.Get(bundle, silent: true);
		}
		else
		{
			bundle.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(bundle, true);
			actor.OutputHandler.Send("You were unable to carry the bundle, so you set it down.");
		}
	}

	[PlayerCommand("Roll", "roll")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	protected static void Roll(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to roll?");
			return;
		}

		var target = actor.TargetHeldItem(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You are not holding anything like that.");
			return;
		}

		var targetBundle = target.GetItemType<PileGameItemComponent>();
		if (targetBundle != null)
		{
			if (!targetBundle.Contents.All(x => x.IsItemType<IDice>()))
			{
				actor.OutputHandler.Send("You can only roll bundles when they contain nothing but dice.");
				return;
			}
		}

		var targetDice = target.GetItemType<IDice>();
		if (targetDice == null && targetBundle == null)
		{
			actor.OutputHandler.Send("You can only roll dice or bundles of dice.");
			return;
		}

		IGameItem surfaceTarget = null;
		var surfaceText = ss.PopSafe();
		if (!string.IsNullOrEmpty(surfaceText))
		{
			surfaceTarget = actor.TargetLocalItem(surfaceText);
			if (surfaceTarget == null)
			{
				actor.OutputHandler.Send("There is no such surface for you to roll something onto.");
				return;
			}

			if (!surfaceTarget.IsItemType<ITable>() || !surfaceTarget.IsItemType<IContainer>())
			{
				actor.OutputHandler.Send("You can only roll things onto tables that accept contents.");
				return;
			}

			if (!surfaceTarget.GetItemType<IContainer>().CanPut(target))
			{
				switch (surfaceTarget.GetItemType<IContainer>().WhyCannotPut(target))
				{
					case WhyCannotPutReason.NotContainer:
						actor.OutputHandler.Send(
							$"You cannot roll anything onto {surfaceTarget.HowSeen(actor)} because it is not a container.");
						return;
					case WhyCannotPutReason.ContainerClosed:
						actor.OutputHandler.Send(
							$"You cannot roll anything onto {surfaceTarget.HowSeen(actor)} because it is closed.");
						return;
					case WhyCannotPutReason.ContainerFull:
					case WhyCannotPutReason.ContainerFullButCouldAcceptLesserQuantity:
						actor.OutputHandler.Send(
							$"You cannot {target.HowSeen(actor)} onto {surfaceTarget.HowSeen(actor)} because it would not fit.");
						return;
					case WhyCannotPutReason.ItemTooLarge:
						actor.OutputHandler.Send(
							$"You cannot roll {target.HowSeen(actor)} onto {surfaceTarget.HowSeen(actor)} because it is too large.");
						return;
					case WhyCannotPutReason.NotCorrectItemType:
						actor.OutputHandler.Send(
							$"You cannot roll {target.HowSeen(actor)} onto {surfaceTarget.HowSeen(actor)} because it is not the correct item type to fit.");
						return;
					case WhyCannotPutReason.CantPutContainerInItself:
						throw new ApplicationException(
							"Got WhyCannotPutReason.CantPutContaineInItself when it cannot be true.");
				}

				return;
			}
		}

		PlayerEmote emote = null;
		var text = ss.PopParentheses();
		if (!string.IsNullOrWhiteSpace(text))
		{
			emote = new PlayerEmote(text, actor);
			if (!emote.Valid)
			{
				actor.Send(emote.ErrorMessage);
				return;
			}
		}

		var dice = new List<IDice>();
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ roll|rolls $1$?2| onto $2||$", actor, actor, target, surfaceTarget))
				.Append(emote));
		if (targetBundle != null)
		{
			dice.AddRange(targetBundle.Contents.Select(x => x.GetItemType<IDice>()));
		}
		else
		{
			dice.Add(targetDice);
		}

		actor.Body.Take(target);
		if (surfaceTarget != null)
		{
			surfaceTarget.GetItemType<IContainer>().Put(actor, target, false);
		}
		else
		{
			target.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(target, true);
		}

		var results = new List<string>();
		foreach (var die in dice)
		{
			for (var i = 0; i < die.Parent.Quantity; i++)
			{
				results.Add(die.Roll());
			}
		}

		actor.OutputHandler.Handle($"The results come up as {results.Select(x => x.ColourValue()).ListToString()}.");
	}
}