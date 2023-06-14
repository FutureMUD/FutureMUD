using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Commands.Modules;

internal class InventoryModule : Module<ICharacter>
{
	protected static Regex PutCommandRegex =
		new(
			@"^(?:(?<quantity>\d{0,})\s+){0,1}(?<item>[\w]{0,}[a-zA-Z.-]{1,})(?:\s+(?<owner>[\w]{0,}[a-zA-Z.-]{1,}))?(?:\s+(?<container>[\w]{0,}[a-zA-Z.-@]{1,}))\s*?(?<emote>\(.*\)){0,1}$");

	protected static Regex PutCurrencyRegex =
		new(
			"^(?<exactly>exactly ){0,1}(?<currency>\\\"{0,1}(?:([^\\d\\s]){0,1}(?:\\d{1,}(?:\\.\\d+){0,})[ ]{0,1}(?:[a-zA-Z]+)*\\s*)+\\\"|[\\S]+){0,1}(?:\\s+(?<owner>[\\w]{0,}[a-zA-Z.-]{1,}))?(?: (?<container>[\\w]{0,}[a-zA-Z.-@]{0,}))\\s*(?<emote>\\(.*\\)){0,1}$");

	protected static Regex PutWeightRegex =
		new(
			@"^(?<weight>[0-9]+(?:\.[0-9]+)?[a-zA-Z]+)\s+(?<item>[\w]{0,}[a-zA-Z.-]{1,})(?:\s+(?<owner>[\w]{0,}[a-zA-Z.-]{1,}))?(?:\s+(?<container>[\w]{0,}[a-zA-Z.-@]{1,}))\s*?(?<emote>\(.*\)){0,1}$");

	protected static Regex TakeCommandRegex =
		new(
			@"^(?:(?<quantity>\d{0,})\s+){0,1}(?<item>[\w]{0,}[a-zA-Z.-]{1,})\s+(?<target>[\w]{0,}[a-zA-Z.-]{1,})(?:\s*(?<targetcontainer>[\w]{0,}[a-zA-Z.-@]{1,}))?\s*?(?<emote>\(.*\)){0,1}$");

	protected static Regex GetCommandRegex =
		new(
			@"^(?:(\d{0,})[ ]){0,1}([\w]{0,}[a-zA-Z.-]{1,})[ ]{0,1}([\w]{0,}[a-zA-Z.-@]{0,})[ ]{0,1}(\(.*\)){0,1}$");

	protected static Regex GetCurrencyRegex =
		new(
			"^(?<exactly>exactly ){0,1}(?<currency>\\\"{0,1}(?:([^\\d\\s]){0,1}(?:\\d{1,}(?:\\.\\d+){0,})[ ]{0,1}(?:[a-zA-Z]+)*\\s*)+\\\"|[\\S]+){0,1}(?: (?<container>[\\w]{0,}[a-zA-Z.-@]{0,})){0,}\\s*(?<emote>\\(.*\\)){0,1}$");

	protected static Regex GetWeightRegex =
		new(
			@"^(?<weight>[0-9]+(?:\.[0-9]+)?[a-zA-Z]+)\s+(?<item>[\w]{0,}[a-zA-Z.-]{1,})(?:\s+(?<container>[\w]{0,}[a-zA-Z.-@]{1,}))?\s*?(?<emote>\(.*\)){0,1}$");

	protected static Regex DropCommandRegex =
		new(@"^(new |)(?:(\d{0,})[ ]){0,1}([\w]{0,}[a-zA-Z.-]{1,})[ ]{0,1}(\(.*\)){0,1}$");

	protected static Regex DropCurrencyRegex =
		new(
			"^(?<new>new |)(?<exactly>exactly ){0,1}(?<currency>\\\"{0,1}(?:([^\\d\\s]){0,1}(?:\\d{1,}(?:\\.\\d+){0,})[ ]{0,1}(?:[a-zA-Z]+)*\\s*)+\\\"|[\\S]+){0,1}\\s*(?<emote>\\(.*\\)){0,1}$");

	protected static Regex GiveCommandRegex =
		new(
			@"^(?:(\d{0,})[ ]){0,1}([\w]{0,}[a-zA-Z.-]{0,})[ ]{0,1}([\w]{0,}[a-zA-Z.-]{0,})[ ]{0,1}(\(.*\)){0,1}$");

	protected static Regex GiveCurrencyRegex =
		new(
			"^(?<exactly>exactly ){0,1}(?<currency>\\\"{0,1}(?:([^\\d\\s]){0,1}(?:\\d{1,}(?:\\.\\d+){0,})[ ]{0,1}(?:[a-zA-Z]+)*\\s*)+\\\"|[\\S]+){0,1}(?: (?<target>[\\w]{0,}[a-zA-Z.-]{0,})){0,}\\s*(?<emote>\\(.*\\)){0,1}$");

	protected static Regex GiveWeightRegex =
		new(
			@"^(?<weight>[0-9]+(?:\.[0-9]+)?[a-zA-Z]+)\s+(?<item>[\w]{0,}[a-zA-Z.-]{1,})(?:\s+(?<target>[\w]{0,}[a-zA-Z.-]{1,}))\s*?(?<emote>\(.*\)){0,1}$");

	protected static Regex DropWeightRegex =
		new(
			@"^(?<new>new |)(?<weight>[0-9]+(?:\.[0-9]+)?[a-zA-Z]+)\s+(?<item>[\w]{0,}[a-zA-Z.-]{1,})\s*?(?<emote>\(.*\)){0,1}$");

	protected static Regex WearCommandRegex =
		new(@"^([\w]{0,}[a-zA-Z.-]{0,})[ ]{0,1}([\w]{0,}[a-zA-Z]{0,})[ ]{0,1}(\(.*\)){0,1}$");

	private InventoryModule()
		: base("Inventory")
	{
		IsNecessary = true;
	}

	public static InventoryModule Instance { get; } = new();

	public override int CommandsDisplayOrder => 3;

	[PlayerCommand("Inventory", "inventory", "inv", "i", "equipment")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Inventory(ICharacter actor, string input)
	{
		actor.DisplayInventory();
	}

	[PlayerCommand("Swap", "swap")]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Swap(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			if (!actor.Body.HeldOrWieldedItems.Any())
			{
				actor.Send(
					$"You're not holding anything in your {actor.Body.Prototype.WielderDescriptionPlural}, so there's nothing to swap.");
				return;
			}

			if (actor.Body.HeldOrWieldedItems.Count() > 2)
			{
				actor.Send(
					"You're holding and wielding more than two items, so you'll need to specify which ones you want to swap.");
				return;
			}

			actor.Body.Swap(actor.Body.HeldOrWieldedItems.First(),
				actor.Body.HeldOrWieldedItems.Count() == 1
					? null
					: actor.Body.HeldOrWieldedItems.Skip(1).Single());
			return;
		}

		var target = actor.TargetHeldItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You're not holding anything like that to swap.");
			return;
		}

		var targetTwo = actor.TargetHeldItem(ss.Pop());
		if (targetTwo == null && !string.IsNullOrEmpty(ss.Last))
		{
			actor.Send($"You don't see anything like that to swap with {target.HowSeen(actor)}.");
			return;
		}

		actor.Body.Swap(target, targetTwo);
	}

	[PlayerCommand("Restrain", "restrain")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	protected static void Restrain(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send($"The correct syntax is {"restrain <person> <item> <how> [<target>]".Colour(Telnet.Yellow)}.");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to restrain.");
			return;
		}

		if (target == actor)
		{
			actor.Send("You can't do a proper job of restraining yourself. Find someone else to do it.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What do you want to restrain them with?");
			return;
		}

		var item = actor.TargetHeldItem(ss.Pop());
		if (item == null)
		{
			actor.Send("You aren't holding anything like that to restrain them with.");
			return;
		}

		var restraint = item.GetItemType<IRestraint>();
		if (restraint == null)
		{
			actor.Send($"{item.HowSeen(actor, true)} is not a restraint.");
			return;
		}

		if (restraint.RestraintType == RestraintType.Shackle)
		{
			actor.Send("Coming soon.");
			return;
		}

		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			actor.Send("That restraint has not been properly configured. Please report it to staff.");
			return;
		}

		var profiles = wearable.Profiles
		                       .Where(x => target.Body.Prototype.CountsAs(x.DesignedBody) &&
		                                   x.Profile(target.Body) != null).ToList();
		if (!profiles.Any())
		{
			actor.OutputHandler.Send(
				$"There are no valid ways for you to restrain {(target == actor ? "yourself" : target.HowSeen(actor))} with {item.HowSeen(actor)}.");
		}

		IWearProfile profile = null;
		if (ss.IsFinished && profiles.Count > 1)
		{
			actor.Send(
				$"There is more than one way to use that restraint, you must specify {profiles.Select(x => x.Name.Colour(Telnet.Yellow)).ListToString(conjunction: "or ")}.");
			return;
		}
		else if (ss.IsFinished)
		{
			profile = profiles.FirstOrDefault(x => x == wearable.DefaultProfile) ??
			          profiles.First();
		}
		else
		{
			var text = ss.Pop();
			profile = profiles.FirstOrDefault(x => x.Name.EqualTo(text));
		}

		if (profile == null)
		{
			actor.Send("That is not a valid way to use that restraint.");
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		IGameItem targetItem = null;
		if (profile.AllProfiles.Select(x => target.Body.GetLimbFor(x.Key)?.LimbType)
		           .Any(x => x == LimbType.Head || x == LimbType.Torso))
		{
			if (ss.IsFinished)
			{
				actor.Send("You need to specify an item to restrain them to when you restrain their head or torso.");
				return;
			}

			targetItem = actor.TargetLocalItem(ss.Pop());
			if (targetItem == null)
			{
				actor.Send("There is nothing like that here to restrain them to.");
				return;
			}

			if (targetItem.IsItemType<IHoldable>() && targetItem.Size < target.SizeSitting)
			{
				actor.Send("The item you restrain them to must be either immovable or at least the same size as them.");
				return;
			}

			emoteText = ss.PopParentheses();
			if (!string.IsNullOrEmpty(emoteText))
			{
				emote = new PlayerEmote(emoteText, actor);
				if (!emote.Valid)
				{
					actor.OutputHandler.Send(emote.ErrorMessage);
					return;
				}
			}
		}

		if (target.WillingToPermitInventoryManipulation(actor))
		{
			target.Body.Restrain(item, profile, actor, targetItem, emote, false);
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is proposing to restrain $0 with $1.", actor,
				target, item)));
			target.OutputHandler.Send(
				$"Use {"accept".Colour(Telnet.Yellow)} to consent, or {"decline".Colour(Telnet.Yellow)} to dismiss this request.");
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = text =>
				{
					if (!actor.ColocatedWith(target))
					{
						actor.Send("You are no longer in the same location as your target.");
						return;
					}

					if (actor.Combat != null && actor.MeleeRange)
					{
						actor.Send("You are now engaged in melee and can't restrain anyone!");
						return;
					}

					if (target.Combat != null && target.MeleeRange)
					{
						actor.Send("You can no longer restrain your target as they are engaged in melee.");
						return;
					}

					if (item.Destroyed || !actor.Body.HeldOrWieldedItems.Contains(item))
					{
						actor.Send("You no longer have your restraint.");
						return;
					}

					if (actor.IsHelpless)
					{
						actor.Send("You aren't able to do anything right now, let alone restrain people!");
						return;
					}

					if (targetItem != null && targetItem.Location != actor.Location)
					{
						actor.Send("The item you were going to restrain them to is no longer there.");
						return;
					}

					target.Body.Restrain(item, profile, actor, targetItem, emote, false);
				},
				DescriptionString = "being restrained",
				ExpireAction = () => { },
				Keywords = { "restrain" },
				RejectAction = text =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines to be restrained by $0.", target, actor)));
				}
			}));
		}
	}

	[PlayerCommand("Undo", "undo")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoMeleeCombatCommand]
	protected static void Undo(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("help") || ss.Peek().EqualTo("?"))
		{
			actor.Send($"The correct syntax is {"undo <person> <item>".Colour(Telnet.Yellow)}.");
			return;
		}

		if (actor.Body.Limbs.Where(x => x.LimbType == LimbType.Arm)
		         .All(x => actor.Body.CanUseLimb(x) != CanUseLimbResult.CanUse))
		{
			actor.Send("You require at least one working arm to undo someone's bindings.");
			return;
		}

		var target = actor.TargetActor(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anyone like that to undo the bindings of.");
			return;
		}

		IGameItem item = null;
		if (ss.IsFinished)
		{
			item = target.Body.WornItems.FirstOrDefault(x => x.GetItemType<IRestraint>()?.RestraintType ==
			                                                 RestraintType.Binding &&
			                                                 target.Body.CoverInformation(x).All(y =>
				                                                 y.Item1 == WearableItemCoverStatus.Uncovered));
		}
		else
		{
			item = target.Body.WornItems.GetFromItemListByKeyword(ss.Pop(), actor);
		}

		if (item == null)
		{
			actor.Send("They do not have any bindings like that to undo.");
			return;
		}

		if (target.Body.CoverInformation(item).Any(y => y.Item1 != WearableItemCoverStatus.Uncovered))
		{
			actor.Send("That item is covered with another item, which must be removed first.");
			return;
		}

		var restraint = item.GetItemType<IRestraint>();
		if (restraint == null)
		{
			actor.Send($"{item.HowSeen(actor, true)} is not a binding that can be undone.");
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		target.Body.Take(item);
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ undo|undoes $1, releasing $0 from &0's restraint", actor, target, item))
				.Append(emote));
		if (actor.Body.CanGet(item, 0))
		{
			actor.Body.Get(item);
		}
		else
		{
			item.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(item);
		}
	}

	[PlayerCommand("Unwield", "unwield")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Unwield(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What is it that you want to stop wield?");
			return;
		}

		var targetText = ss.Pop();
		var targetItem = actor.Body.WieldedItems.GetFromItemListByKeyword(targetText, actor);
		if (targetItem == null)
		{
			actor.OutputHandler.Send("You are not wielding any such item.");
			return;
		}

		var emoteText = ss.PopParentheses();
		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		actor.Body.Unwield(targetItem, emote);
	}

	[PlayerCommand("Wield", "wield")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Wield(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What is it that you want to wield?");
			return;
		}

		var targetText = ss.Pop();
		var targetItem = actor.TargetHeldItem(targetText);
		if (targetItem == null)
		{
			actor.OutputHandler.Send("You are not holding any such item.");
			return;
		}

		var emoteText = ss.PopParentheses();
		var manual2hand = false;
		IWield specificHand = null;
		if (!ss.IsFinished)
		{
			var shandText = ss.PopSpeech();
			if (shandText.EqualToAny(MUDConstants.TwoHandedStrings))
			{
				manual2hand = true;
				emoteText = ss.PopParentheses();
				shandText = ss.PopSpeech();
			}

			if (!string.IsNullOrEmpty(shandText))
			{
				specificHand =
					actor.Body.WieldLocs.FirstOrDefault(
						x => x.FullDescription().Equals(shandText, StringComparison.InvariantCultureIgnoreCase)) ??
					actor.Body.WieldLocs.FirstOrDefault(
						x => x.ShortDescription().Equals(shandText, StringComparison.InvariantCultureIgnoreCase));
				if (specificHand == null)
				{
					actor.Send("You do not have any such {0} with which to wield.",
						actor.Body.WielderDescriptionSingular);
					return;
				}

				if (!ss.IsFinished && ss.Peek().EqualToAny(MUDConstants.TwoHandedStrings))
				{
					manual2hand = true;
				}

				emoteText = ss.PopParentheses();
			}
		}

		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		if (actor.Combat != null)
		{
			if (specificHand == null && !actor.Body.CanWield(targetItem,
				    manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None))
			{
				actor.Send(actor.Body.WhyCannotWield(targetItem,
					manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None));
				return;
			}

			if (specificHand != null && !actor.Body.CanWield(targetItem, specificHand,
				    manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None))
			{
				actor.Send(actor.Body.WhyCannotWield(targetItem, specificHand,
					manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None));
				return;
			}

			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectWieldItem(actor,
				    targetItem.GetItemType<IWieldable>(), emote, specificHand,
				    manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Wielding {targetItem.HowSeen(actor)}{(manual2hand ? " 2-handed" : "")}.");
			}

			return;
		}

		if (specificHand == null)
		{
			actor.Body.Wield(targetItem, emote,
				flags: manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None);
		}
		else
		{
			actor.Body.Wield(targetItem, specificHand, emote,
				flags: manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None);
		}
	}

	[PlayerCommand("Put", "put", "pu")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Put(ICharacter actor, string input)
	{
		var match = PutCommandRegex.Match(input.RemoveFirstWord());
		var currencyMatch = PutCurrencyRegex.Match(input.RemoveFirstWord());
		var weightMatch = PutWeightRegex.Match(input.RemoveFirstWord());

		if (!match.Success && !currencyMatch.Success && !weightMatch.Success)
		{
			if (input.RemoveFirstWord().Length == 0)
			{
				actor.OutputHandler.Send("What do you want to put?");
				return;
			}

			actor.OutputHandler.Send(
				$"That is not a valid use of the put command.\nUsage: {"put [quantity] <item> [<person>] <container>".Colour(Telnet.Yellow)} or {"put [exactly] <currency> [<person>] <container>".Colour(Telnet.Yellow)} or {"put <weight> <commodity> [<person>] <container>".Colour(Telnet.Yellow)}");
			return;
		}

		var emoteText = weightMatch.Success ? weightMatch.Groups["emote"].Value :
			match.Success ? match.Groups[4].Value : currencyMatch.Groups["emote"].Value;
		PlayerEmote emote = null;
		if (emoteText.Length > 0)
		{
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor.Body);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		var containerOwnerText = weightMatch.Success ? weightMatch.Groups["owner"].Value :
			match.Success ? match.Groups["owner"].Value :
			currencyMatch.Groups["owner"].Value;

		var containerText = weightMatch.Success
			? weightMatch.Groups["container"].Value
			: match.Success
				? match.Groups["container"].Value
				: currencyMatch.Groups["container"].Value;
		IGameItem containerItem;
		ICharacter containerOwner = null;
		if (!string.IsNullOrEmpty(containerOwnerText))
		{
			containerOwner = actor.TargetActorOrCorpse(containerOwnerText);
			if (containerOwner == null)
			{
				actor.OutputHandler.Send("You don't see anyone like that whose containers you can put things in.");
				return;
			}

			if (containerOwner == actor)
			{
				actor.OutputHandler.Send(
					"You do not need to specify yourself when interacting with your own containers. Use the version of this command without a container owner specified instead.");
				return;
			}

			if (!containerOwner.WillingToPermitInventoryManipulation(actor))
			{
				actor.OutputHandler.Send(
					$"{containerOwner.HowSeen(actor, true)} will not permit you to interact with {containerOwner.ApparentGender(actor).Possessive()} containers.");
				return;
			}

			containerItem = containerOwner.Body.ExternalItemsForOtherActors.Where(x => actor.CanSee(x))
			                              .GetFromItemListByKeyword(containerText, actor);
		}
		else
		{
			containerItem = actor.TargetItem(containerText);
		}

		if (containerItem == null)
		{
			actor.OutputHandler.Send("What is it that you want to put that in?");
			return;
		}

		var (truth, error) = actor.CanManipulateItem(containerItem);
		if (!truth)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (weightMatch.Success)
		{
			var amount = actor.Gameworld.UnitManager.GetBaseUnits(weightMatch.Groups["weight"].Value,
				Framework.Units.UnitType.Mass, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid weight of commodity to put.");
				return;
			}

			var targetItem = actor.Body.TargetHeldItem(weightMatch.Groups["item"].Value);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not have anything like that to put in " +
				                         containerItem.HowSeen(actor.Body) + ".");
				return;
			}

			if (!(targetItem.GetItemType<ICommodity>() is ICommodity ic))
			{
				var quantity = (int)(targetItem.Weight > 0.0 && targetItem.Quantity > 0
					? Math.Min(targetItem.Quantity, Math.Ceiling(amount / (targetItem.Weight / targetItem.Quantity)))
					: targetItem.Quantity);
				actor.Body.Put(targetItem, containerItem, containerOwner, quantity, emote);
				return;
			}

			var item = targetItem.PeekSplitByWeight(amount);
			if (actor.Body.CanPut(item, containerItem, containerOwner, 0, false))
			{
				actor.Body.Put(targetItem.GetByWeight(actor.Body, amount), containerItem, containerOwner, 0, emote,
					allowLesserAmounts: false);
			}
			else if (actor.Body.CanPut(item, containerItem, containerOwner, 0, true))
			{
				var lesserAmount = containerItem.GetItemType<IContainer>().CanPutAmount(item);
				actor.Body.Put(targetItem.Get(actor.Body, lesserAmount), containerItem, containerOwner, 0, emote,
					allowLesserAmounts: false);
			}
			else
			{
				actor.OutputHandler.Send(actor.Body.WhyCannotPut(item, containerItem, containerOwner, 0, true));
			}
		}
		else if (match.Success)
		{
			var quantity = 0;
			if (match.Groups["quantity"].Length > 0)
			{
				quantity = Convert.ToInt32(match.Groups["quantity"].Value);
			}

			var targetItem = actor.Body.TargetHeldItem(match.Groups["item"].Value);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not have anything like that to put in " +
				                         containerItem.HowSeen(actor.Body) + ".");
				return;
			}

			actor.Body.Put(targetItem, containerItem, containerOwner, quantity, emote);
		}
		else
		{
			var currencyText = currencyMatch.Groups["currency"].Value.Strip(x => x == '\"');
			var targetCurrency = actor.Currency.GetBaseCurrency(currencyText, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid amount of money for your current currency.");
				return;
			}

			actor.Body.Put(actor.Currency, containerItem, containerOwner, targetCurrency,
				currencyMatch.Groups["exactly"].Value.Length > 0, emote);
		}
	}

	[PlayerCommand("Take", "take")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoMeleeCombatCommand]
	protected static void Take(ICharacter actor, string input)
	{
		var text = input.RemoveFirstWord();
		if (string.IsNullOrEmpty(text))
		{
			actor.Send("What do you wish to take, and from whom do you wish to take it?");
			return;
		}

		var match = TakeCommandRegex.Match(text);
		if (!match.Success)
		{
			actor.Send("The correct syntax is {0}",
				"take [<quantity>] <item> <target> [<targetcontainer>]".Colour(Telnet.Yellow));
			return;
		}

		var quantity = 0;
		if (match.Groups["quantity"].Length > 0)
		{
			quantity = int.Parse(match.Groups["quantity"].Value);
		}

		var target = actor.TargetActorOrCorpse(match.Groups["target"].Value);
		if (target == null)
		{
			actor.Send("You do not see them to take anything from.");
			return;
		}

		IGameItem targetItem = null, targetContainerItem = null;
		if (match.Groups["targetcontainer"].Length > 0)
		{
			targetContainerItem =
				target.Body.ExternalItemsForOtherActors.Where(x => actor.CanSee(x))
				      .GetFromItemListByKeyword(match.Groups["targetcontainer"].Value, actor);
			if (targetContainerItem == null)
			{
				actor.Send("{0} does not have any container like that from which you can take things.",
					target.HowSeen(actor, true));
				return;
			}

			var targetContainer = targetContainerItem.GetItemType<IContainer>();
			if (targetContainer == null)
			{
				actor.Send("{0} is not a container.", targetContainerItem.HowSeen(actor, true));
				return;
			}

			if (targetContainer is IOpenable && !(targetContainer as IOpenable).IsOpen)
			{
				actor.Send("{0} is closed, you must open it before you can take anything from it.",
					targetContainerItem.HowSeen(actor, true));
			}

			targetItem = targetContainer.Contents.Where(x => actor.CanSee(x))
			                            .GetFromItemListByKeyword(match.Groups["item"].Value, actor);
		}
		else
		{
			targetItem = target.Body.DirectItems.Where(x => actor.CanSee(x))
			                   .GetFromItemListByKeyword(match.Groups["item"].Value, actor);
		}

		if (targetItem == null)
		{
			actor.Send("You do not see anything like that to take.");
			return;
		}

		PlayerEmote emote = null;
		if (match.Groups["emote"].Length > 0)
		{
			emote = new PlayerEmote(new StringStack(match.Groups["emote"].Value).PopParentheses(), actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		if (targetContainerItem == null)
		{
			if (!target.Body.CanBeRemoved(targetItem, actor))
			{
				actor.Send(target.Body.WhyCannotBeRemoved(targetItem, actor));
				return;
			}

			if (!actor.Body.CanGet(targetItem, quantity))
			{
				actor.Send(actor.Body.WhyCannotGet(targetItem, quantity));
				return;
			}

			var newItem = target.Body.Take(targetItem, quantity);
			actor.Body.Get(newItem, silent: true);
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ take|takes $0 from $1", actor, targetItem, target),
						flags: OutputFlags.SuppressObscured)
					.Append(emote));
		}
		else
		{
			if (!target.WillingToPermitInventoryManipulation(actor))
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} will not allow you to interact with their inventory.");
				return;
			}

			if (!actor.Body.CanGet(targetItem, targetContainerItem, quantity))
			{
				actor.Send(actor.Body.WhyCannotGet(targetItem, targetContainerItem, quantity));
				return;
			}

			actor.Body.Get(targetItem, targetContainerItem, quantity, null, true);
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote(
						$"@ take|takes $0 from $1's {targetContainerItem.Name.Colour(Telnet.Green)}", actor,
						targetItem, target), flags: OutputFlags.SuppressObscured)
					.Append(emote));
		}
	}

	[PlayerCommand("Get", "get", "ge", "g")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Get(ICharacter actor, string input)
	{
		if (new StringStack(input.RemoveFirstWord()).IsFinished)
		{
			actor.Send("What is it that you want to get?");
			return;
		}

		var match = GetCommandRegex.Match(input.RemoveFirstWord());
		var currencyMatch = GetCurrencyRegex.Match(input.RemoveFirstWord());
		var weightMatch = GetWeightRegex.Match(input.RemoveFirstWord());

		if (!match.Success && !currencyMatch.Success && !weightMatch.Success)
		{
			if (input.RemoveFirstWord().Length == 0)
			{
				actor.OutputHandler.Send("What do you want to get?");
				return;
			}

			actor.OutputHandler.Send(
				$" That is not a valid use of the get command.\nUsage: {"get [quantity] <item> [container]".Colour(Telnet.Yellow)} or {"get <currency> [<container>]".Colour(Telnet.Yellow)} or  {"get <weight> <item> [<container>]".Colour(Telnet.Yellow)}");
			return;
		}

		var emoteText = weightMatch.Success ? weightMatch.Groups["emote"].Value :
			match.Success ? match.Groups[4].Value : currencyMatch.Groups["emote"].Value;

		PlayerEmote emote = null;
		if (emoteText.Length > 0)
		{
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor.Body);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		var quantity = 0;
		if (match.Success && match.Groups[1].Length > 0)
		{
			quantity = Convert.ToInt32(match.Groups[1].Value);
		}

		var containerText = weightMatch.Success ? weightMatch.Groups["container"].Value :
			match.Success ? match.Groups[3].Value : currencyMatch.Groups["container"].Value;
		if (containerText.Length > 0)
		{
			var containerItem = actor.TargetItem(containerText);
			if (containerItem == null)
			{
				actor.OutputHandler.Send("What is it that you want to get that out of?");
				return;
			}

			if (containerItem.IsItemType<ICorpse>())
			{
				Take(actor, input);
				return;
			}

			var container = containerItem.GetItemType<IContainer>();
			if (container == null)
			{
				actor.OutputHandler.Send(containerItem.HowSeen(actor, true) + " is not a container.");
				return;
			}

			var (truth, error) = actor.CanManipulateItem(containerItem);
			if (!truth)
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (weightMatch.Success)
			{
				var amount = actor.Gameworld.UnitManager.GetBaseUnits(weightMatch.Groups["weight"].Value,
					Framework.Units.UnitType.Mass, out var success);
				if (!success)
				{
					actor.OutputHandler.Send("That is not a valid weight of commodity to get.");
					return;
				}

				var targetItem = container.Contents.GetFromItemListByKeyword(weightMatch.Groups["item"].Value, actor);
				if (targetItem == null)
				{
					actor.OutputHandler.Send("You do not see anything like that to get in " +
					                         containerItem.HowSeen(actor.Body) + ".");
					return;
				}

				if (!(targetItem.GetItemType<ICommodity>() is ICommodity ic))
				{
					quantity = (int)(targetItem.Weight > 0.0 && targetItem.Quantity > 0
						? Math.Min(targetItem.Quantity,
							Math.Ceiling(amount / (targetItem.Weight / targetItem.Quantity)))
						: targetItem.Quantity);
					actor.Body.Get(targetItem, containerItem, quantity, emote);
					return;
				}

				if (targetItem.DropsWholeByWeight(amount))
				{
					actor.Body.Get(targetItem, containerItem, 0, emote);
					return;
				}

				var item = targetItem.GetByWeight(actor.Body, amount);
				actor.Body.Get(item, containerItem, 0, emote, false, ItemCanGetIgnore.IgnoreInContainer);
			}
			else if (match.Success)
			{
				var targetItem = container.Contents.GetFromItemListByKeyword(match.Groups[2].Value, actor);
				actor.Body.Get(targetItem, containerItem, quantity, emote);
			}
			else
			{
				var currencyText = currencyMatch.Groups["currency"].Value.Strip(x => x == '\"');
				var targetCurrency = actor.Currency.GetBaseCurrency(currencyText, out var success);
				if (!success)
				{
					actor.OutputHandler.Send("That is not a valid amount of money for your current currency.");
					return;
				}

				actor.Body.Get(actor.Currency, containerItem, targetCurrency,
					currencyMatch.Groups["exactly"].Value.Length > 0, emote);
			}
		}
		else
		{
			if (weightMatch.Success)
			{
				var amount = actor.Gameworld.UnitManager.GetBaseUnits(weightMatch.Groups["weight"].Value,
					Framework.Units.UnitType.Mass, out var success);
				if (!success)
				{
					actor.OutputHandler.Send("That is not a valid weight of commodity to get.");
					return;
				}

				var targetItem = actor.TargetLocalItem(weightMatch.Groups["item"].Value);
				if (targetItem == null)
				{
					actor.OutputHandler.Send("You do not see anything like that to get.");
					return;
				}

				if (!(targetItem.GetItemType<ICommodity>() is ICommodity ic))
				{
					quantity = (int)(targetItem.Weight > 0.0 && targetItem.Quantity > 0
						? Math.Min(targetItem.Quantity,
							Math.Ceiling(amount / (targetItem.Weight / targetItem.Quantity)))
						: targetItem.Quantity);
					actor.Body.Get(targetItem, quantity, emote);
					return;
				}

				if (targetItem.DropsWholeByWeight(amount))
				{
					actor.Body.Get(targetItem, 0, emote);
					return;
				}

				var item = targetItem.GetByWeight(actor.Body, amount);
				actor.Body.Get(item, 0, emote);
			}
			else if (match.Success)
			{
				var targetItem = actor.TargetLocalItem(match.Groups[2].Value);
				if (targetItem == null)
				{
					actor.OutputHandler.Send("You do not see that here to get.");
					return;
				}

				if (actor.Combat != null)
				{
					if (!actor.Body.CanGet(targetItem, quantity))
					{
						actor.Send(actor.Body.WhyCannotGet(targetItem, quantity));
						return;
					}

					if (actor.TakeOrQueueCombatAction(
						    SelectedCombatAction.GetEffectGetItem(actor, targetItem, emote)) &&
					    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
					{
						actor.Send(
							$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Getting {targetItem.HowSeen(actor)}.");
					}

					return;
				}

				actor.Body.Get(targetItem, quantity, emote);
			}

			else
			{
				var currencyText = currencyMatch.Groups["currency"].Value.Strip(x => x == '\"');
				var targetCurrency = actor.Currency.GetBaseCurrency(currencyText, out var success);
				if (!success)
				{
					actor.OutputHandler.Send("That is not a valid amount of money for your current currency.");
					return;
				}

				if (actor.Combat != null)
				{
					actor.Send("You can't get money while you're in combat.");
					return;
				}

				actor.Body.Get(actor.Currency, targetCurrency, currencyMatch.Groups["exactly"].Value.Length > 0,
					emote);
			}
		}
	}

	[PlayerCommand("Drop", "drop", "dro", "dr")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Drop(ICharacter actor, string input)
	{
		if (string.IsNullOrWhiteSpace(input.RemoveFirstWord()))
		{
			actor.OutputHandler.Send("What do you want to drop?");
			return;
		}

		var match = DropCommandRegex.Match(input.RemoveFirstWord());
		var currencyMatch = DropCurrencyRegex.Match(input.RemoveFirstWord());
		var weightMatch = DropWeightRegex.Match(input.RemoveFirstWord());
		if (!match.Success && !currencyMatch.Success && !weightMatch.Success)
		{
			actor.OutputHandler.Send(
				$"That is not a valid use of the drop command.\nUsage: {"drop [new] [quantity] <item>".Colour(Telnet.Yellow)} or {"drop [new] [exactly] <currency>".Colour(Telnet.Yellow)} or {"drop [new] <weight> <item>".Colour(Telnet.Yellow)}");
			return;
		}

		var emoteText = weightMatch.Success ? weightMatch.Groups["emote"].Value :
			match.Success ? match.Groups[4].Value : currencyMatch.Groups["emote"].Value;
		PlayerEmote emote = null;
		if (emoteText.Length > 0)
		{
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		if (match.Groups[2].Length == 0 &&
		    match.Groups[3].Value.Equals("all", StringComparison.InvariantCultureIgnoreCase))
		{
			var items = actor.Body.HeldItems.ToList();
			foreach (var item in items.Where(item => !actor.Body.CanDrop(item, 0)))
			{
				actor.Send(actor.Body.WhyCannotDrop(item, 0));
				return;
			}

			foreach (var item in items)
			{
				actor.Body.Drop(item);
			}

			return;
		}

		if (weightMatch.Success)
		{
			var amount = actor.Gameworld.UnitManager.GetBaseUnits(weightMatch.Groups["weight"].Value,
				Framework.Units.UnitType.Mass, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid weight of commodity to drop.");
				return;
			}

			var targetItem = actor.TargetHeldItem(weightMatch.Groups["item"].Value);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not have anything like that to drop.");
				return;
			}

			if (!(targetItem.GetItemType<ICommodity>() is ICommodity ic))
			{
				var quantity = (int)(targetItem.Weight > 0.0 && targetItem.Quantity > 0
					? Math.Min(targetItem.Quantity, Math.Ceiling(amount / (targetItem.Weight / targetItem.Quantity)))
					: targetItem.Quantity);
				actor.Body.Drop(targetItem, quantity, match.Groups["new"].Length > 0, emote);
				return;
			}

			if (targetItem.DropsWholeByWeight(amount))
			{
				actor.Body.Drop(targetItem, 0, match.Groups["new"].Length > 0, emote);
				return;
			}

			var item = targetItem.DropByWeight(actor.Location, amount);
			actor.Body.Drop(item, 0, match.Groups["new"].Length > 0, emote);
		}
		else if (match.Success)
		{
			var targetItem = actor.TargetHeldItem(match.Groups[3].Value);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not have that to drop.");
				return;
			}

			var quantity = 0;
			if (match.Groups[2].Length > 0)
			{
				quantity = Convert.ToInt32(match.Groups[2].Value);
			}

			actor.Body.Drop(targetItem, quantity, match.Groups[1].Length > 0, emote);
		}
		else
		{
			var currencyText = currencyMatch.Groups["currency"].Value.Strip(x => x == '\"');
			var targetCurrency = actor.Currency.GetBaseCurrency(currencyText, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid amount of money for your current currency.");
				return;
			}

			actor.Body.Drop(actor.Currency, targetCurrency, currencyMatch.Groups["exactly"].Value.Length > 0,
				currencyMatch.Groups["new"].Value.Length > 0, emote);
		}
	}

	[PlayerCommand("Give", "give")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Give(ICharacter actor, string input)
	{
		var match = GiveCommandRegex.Match(input.RemoveFirstWord());
		var currencyMatch = GiveCurrencyRegex.Match(input.RemoveFirstWord());
		var weightMatch = GiveWeightRegex.Match(input.RemoveFirstWord());
		if (!match.Success && !currencyMatch.Success && !weightMatch.Success)
		{
			actor.OutputHandler.Send(
				$"That is not a valid use of the give command.\nUsage: {"give [quantity] <item> <target>".Colour(Telnet.Yellow)} or {"give [exactly] <currency> <target>".Colour(Telnet.Yellow)} or {"give <weight> <item> <target>".Colour(Telnet.Yellow)}");
			return;
		}

		var targetText = weightMatch.Success ? weightMatch.Groups["target"].Value :
			match.Success ? match.Groups[3].Value : currencyMatch.Groups["target"].Value;
		if (targetText.Length == 0)
		{
			actor.OutputHandler.Send("Who do you wish give that to?");
			return;
		}

		var target = actor.Target(targetText);
		if (target == null)
		{
			actor.OutputHandler.Send("You do not see them to give that to.");
			return;
		}

		ICorpse targetCorpse = null;
		ICharacter targetActor = null;
		if (target is ICharacter)
		{
			targetActor = target as ICharacter;
		}
		else
		{
			if (!(target is IGameItem && (target as IGameItem).IsItemType<ICorpse>()))
			{
				actor.Send("You can only give things to characters and corpses.");
				return;
			}

			targetCorpse = (target as IGameItem).GetItemType<ICorpse>();
			targetActor = (target as IGameItem).GetItemType<ICorpse>().OriginalCharacter;
		}

		var emoteText = weightMatch.Success ? weightMatch.Groups["emote"].Value :
			match.Success ? match.Groups[4].Value : currencyMatch.Groups["emote"].Value;
		PlayerEmote emote = null;
		if (emoteText.Length > 0)
		{
			emote = new PlayerEmote(new StringStack(emoteText).PopParentheses(), actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		if (!targetActor.IsAlly(actor) && (actor.Combat != null || targetActor.Combat != null))
		{
			actor.Send(
				"You cannot give things to people who aren't your allies while you or the recipient are in combat."
					.Wrap(actor.InnerLineFormatLength));
			return;
		}

		if (weightMatch.Success)
		{
			var amount = actor.Gameworld.UnitManager.GetBaseUnits(weightMatch.Groups["weight"].Value,
				Framework.Units.UnitType.Mass, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid weight of commodity to give.");
				return;
			}

			var targetItem = actor.TargetHeldItem(weightMatch.Groups["item"].Value);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not have anything like that to give.");
				return;
			}

			if (!(targetItem.GetItemType<ICommodity>() is ICommodity ic))
			{
				var quantity = (int)(targetItem.Weight > 0.0 && targetItem.Quantity > 0
					? Math.Min(targetItem.Quantity, Math.Ceiling(amount / (targetItem.Weight / targetItem.Quantity)))
					: targetItem.Quantity);
				if (targetCorpse != null)
				{
					actor.Body.Give(targetItem, targetCorpse, quantity, emote);
				}
				else
				{
					actor.Body.Give(targetItem, targetActor.Body, quantity, emote);
				}

				return;
			}

			if (targetItem.DropsWholeByWeight(amount))
			{
				if (targetCorpse != null)
				{
					actor.Body.Give(targetItem, targetCorpse, 0, emote);
				}
				else
				{
					actor.Body.Give(targetItem, targetActor.Body, 0, emote);
				}

				return;
			}

			var item = targetItem.GetByWeight(actor.Body, amount);
			if (targetCorpse != null)
			{
				actor.Body.Give(item, targetCorpse, 0, emote);
			}
			else
			{
				actor.Body.Give(item, targetActor.Body, 0, emote);
			}
		}
		else if (match.Success)
		{
			if (match.Groups[2].Length == 0)
			{
				actor.OutputHandler.Send("What do you wish to give?");
				return;
			}

			var quantity = 0;
			if (match.Groups[1].Length > 0)
			{
				quantity = Convert.ToInt32(match.Groups[1].Value);
			}

			var targetItem = actor.TargetHeldItem(match.Groups[2].Value);
			if (targetItem == null)
			{
				actor.OutputHandler.Send("You do not have that to give.");
				return;
			}

			if (targetCorpse != null)
			{
				actor.Body.Give(targetItem, targetCorpse, quantity, emote);
			}
			else
			{
				actor.Body.Give(targetItem, targetActor.Body, quantity, emote);
			}
		}
		else
		{
			var currencyText = currencyMatch.Groups["currency"].Value.Strip(x => x == '\"');
			var targetCurrency = actor.Currency.GetBaseCurrency(currencyText, out var success);
			if (!success)
			{
				actor.OutputHandler.Send("That is not a valid amount of money for your current currency.");
				return;
			}

			if (targetCorpse != null)
			{
				actor.Body.Give(actor.Currency, targetCorpse, targetCurrency,
					currencyMatch.Groups["exactly"].Value.Length > 0, emote);
			}
			else
			{
				actor.Body.Give(actor.Currency, targetActor.Body, targetCurrency,
					currencyMatch.Groups["exactly"].Value.Length > 0, emote);
			}
		}
	}

	[PlayerCommand("Wear", "wear", "wea")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Wear(ICharacter actor, string input)
	{
		var match = WearCommandRegex.Match(input.RemoveFirstWord());
		if (!match.Success)
		{
			actor.OutputHandler.Send("That is not a valid use of the wear command.\nUsage: " +
			                         "wear <item> [<profile>]".Colour(Telnet.Yellow));
			return;
		}

		if (match.Groups[1].Length == 0)
		{
			actor.OutputHandler.Send("What do you wish to wear?");
			return;
		}

		PlayerEmote emote = null;
		if (match.Groups[3].Value.Length > 0)
		{
			emote = new PlayerEmote(new StringStack(match.Groups[3].Value).PopParentheses(), actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		var targetItem = actor.TargetHeldItem(match.Groups[1].Value);
		if (targetItem == null)
		{
			actor.OutputHandler.Send("You do not see that to wear.");
			return;
		}

		var config = match.Groups[2].Value;
		if (actor.Combat != null)
		{
			if (!actor.Body.CanWear(targetItem, config))
			{
				actor.Send(actor.Body.WhyCannotWear(targetItem, config));
				return;
			}

			if (actor.TakeOrQueueCombatAction(
				    SelectedCombatAction.GetEffectWearItem(actor, targetItem, emote, config)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send($"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Wearing {targetItem.HowSeen(actor)}.");
			}

			return;
		}

		if (config.Length > 0)
		{
			actor.Body.Wear(targetItem, config, emote);
		}
		else
		{
			actor.Body.Wear(targetItem, emote);
		}
	}

	[PlayerCommand("Remove", "remove", "rem", "rm")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Remove(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var target = ss.Pop();

		var emote = new PlayerEmote(ss.PopParentheses(), actor);

		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return;
		}

		var targetItem = actor.TargetTopLevelWornItem(target);
		if (targetItem == null)
		{
			actor.OutputHandler.Send("You are not wearing anything like that.");
			return;
		}

		if (actor.Combat != null)
		{
			if (!actor.Body.CanRemoveItem(targetItem, ItemCanGetIgnore.IgnoreWeight))
			{
				actor.Send(actor.Body.WhyCannotRemove(targetItem, ignoreFlags: ItemCanGetIgnore.IgnoreWeight));
				return;
			}

			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectRemoveItem(actor, targetItem, emote)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send($"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Removing {targetItem.HowSeen(actor)}.");
			}

			return;
		}

		actor.Body.RemoveItem(targetItem, emote, ignoreFlags: ItemCanGetIgnore.IgnoreWeight);
	}

	[PlayerCommand("Dress", "dress")]
	[NoMeleeCombatCommand]
	[RequiredCharacterState(CharacterState.Able)]
	protected static void Dress(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualTo("?") || ss.Peek().EqualTo("help"))
		{
			actor.Send(
				$"This command is used to dress someone else, or give temporary consent to someone else dressing you.\nThe syntax is {"dress <person> <item>".Colour(Telnet.Yellow)} or {"dress me <person>".Colour(Telnet.Yellow)} to request someone dress you.");
			return;
		}

		ICharacter target;
		var cmd = ss.Pop();
		if (cmd.EqualTo("me"))
		{
			if (ss.IsFinished)
			{
				actor.Send("Who do you want to give permission to dress (or undress) you?");
				return;
			}

			if (ss.Peek().EqualTo("none"))
			{
				actor.Send("You end all of your consent to be dressed or undressed.");
				actor.RemoveAllEffects(x => x.IsEffectType<BeDressedEffect>());
				return;
			}

			target = actor.TargetActor(ss.Pop());
			if (target == null)
			{
				actor.Send("You don't see anyone like that to give consent to dress and undress you.");
				return;
			}

			if (target == actor)
			{
				actor.Send("You don't need to give yourself permission to dress and undress yourself. Just do it.");
				return;
			}

			if (actor.EffectsOfType<BeDressedEffect>().Any(x => x.Dresser == target))
			{
				actor.Reschedule(actor.EffectsOfType<BeDressedEffect>().First(x => x.Dresser == target),
					TimeSpan.FromMinutes(10));
				actor.Send("You renew the duration on your consent to be dressed and undressed by {0}.",
					target.HowSeen(actor));
				target.Send("{0} renews the duration on {1} consent to be dressed and undressed by you.",
					actor.HowSeen(target), actor.ApparentGender(target).Possessive());
				return;
			}

			actor.AddEffect(new BeDressedEffect(actor, target), TimeSpan.FromMinutes(10));
			actor.Send("You give your automatic consent to be dressed and undressed by {0} for the next 10 minutes.",
				target.HowSeen(actor));
			actor.Send(
				$"Note: You can end your consent early if you like by typing {"dress me none".Colour(Telnet.Yellow)}.");
			target.Send("{0} gives {1} consent to be dressed and undressed by you for the next 10 minutes.",
				actor.HowSeen(target), actor.ApparentGender(target).Possessive());
			return;
		}

		target = actor.TargetActor(cmd);
		if (target == null)
		{
			var citem = actor.TargetItem(cmd);
			if (citem?.IsItemType<ICorpse>() == true)
			{
				target = citem.GetItemType<ICorpse>().OriginalCharacter;
			}
			else
			{
				actor.Send("You don't see anyone like that to dress.");
				return;
			}
		}

		if (target == actor)
		{
			actor.Send("You can't use the dress command on yourself, use the regular inventory commands instead.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What do you want to dress them with?");
			return;
		}

		var item = actor.TargetHeldItem(ss.Pop());
		if (item == null)
		{
			actor.Send("You aren't holding anything like that.");
			return;
		}

		PlayerEmote emote = null;
		if (!ss.IsFinished)
		{
			var text = ss.PopParentheses();
			if (!string.IsNullOrEmpty(text))
			{
				emote = new PlayerEmote(text, actor);
				if (!emote.Valid)
				{
					actor.Send(emote.ErrorMessage);
					return;
				}
			}
		}

		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			actor.Send(item.HowSeen(actor, true) + " is not something that can be worn.");
			return;
		}

		IWearProfile wprofile = null;
		if (!ss.IsFinished)
		{
			var profile = ss.PopSpeech();

			wprofile = wearable.Profiles.FirstOrDefault(x =>
				x.Name.ToLowerInvariant().StartsWith(profile, StringComparison.Ordinal));
			if (wprofile == null)
			{
				actor.Send("That is not a valid way to wear " + item.HowSeen(actor) + ".");
				return;
			}

			if (!ss.IsFinished)
			{
				var text = ss.PopParentheses();
				if (!string.IsNullOrEmpty(text))
				{
					emote = new PlayerEmote(text, actor);
					if (!emote.Valid)
					{
						actor.Send(emote.ErrorMessage);
						return;
					}
				}
			}
		}

		target.Body.Dress(item, actor, wprofile, emote);
	}

	[PlayerCommand("Sheathe", "sheathe")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Sheath(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var emoteText = ss.PopParentheses();
		IGameItem item = null, sheath = null;
		if (!ss.IsFinished)
		{
			item = actor.TargetHeldItem(ss.Pop());
			if (item == null)
			{
				actor.OutputHandler.Send("You do not see anything like that to sheathe.");
				return;
			}

			emoteText = ss.PopParentheses();
			if (!ss.IsFinished)
			{
				sheath = actor.TargetPersonalItem(ss.Pop());
				if (sheath == null)
				{
					actor.OutputHandler.Send("You do not have any item like that to sheathe something in.");
					return;
				}

				emoteText = ss.PopParentheses();
			}
		}

		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		actor.Body.Sheathe(item, sheath, emote);
	}

	[PlayerCommand("Draw", "draw", "dra", "dr")]
	[RequiredCharacterState(CharacterState.Conscious)]
	protected static void Draw(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		var emoteText = ss.PopParentheses();
		IGameItem item = null;
		if (!ss.IsFinished && !ss.Peek().EqualToAny(MUDConstants.TwoHandedStrings))
		{
			var targetText = ss.Pop();
			emoteText = ss.PopParentheses();
			item =
				actor.Body.ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
				     .SelectNotNull(x => x.Content)
				     .Select(x => x.Parent)
				     .GetFromItemListByKeyword(targetText, actor);
			if (item == null)
			{
				actor.OutputHandler.Send("You do not have anything like that which you can draw.");
				return;
			}
		}

		IWield specificHand = null;
		var manual2hand = false;
		if (!ss.IsFinished)
		{
			var shandText = ss.PopSpeech();
			if (shandText.EqualToAny(MUDConstants.TwoHandedStrings))
			{
				manual2hand = true;
				emoteText = ss.PopParentheses();
				shandText = ss.PopSpeech();
			}

			if (!string.IsNullOrEmpty(shandText))
			{
				specificHand =
					actor.Body.WieldLocs.FirstOrDefault(
						x => x.FullDescription().Equals(shandText, StringComparison.InvariantCultureIgnoreCase)) ??
					actor.Body.WieldLocs.FirstOrDefault(
						x => x.ShortDescription().Equals(shandText, StringComparison.InvariantCultureIgnoreCase));
				if (specificHand == null)
				{
					actor.Send("You do not have any such {0} with which to wield.",
						actor.Body.WielderDescriptionSingular);
					return;
				}

				if (!ss.IsFinished && ss.Peek().EqualToAny(MUDConstants.TwoHandedStrings))
				{
					manual2hand = true;
				}

				emoteText = ss.PopParentheses();
			}
		}

		PlayerEmote emote = null;
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		if (actor.Combat != null)
		{
			if (!actor.Body.CanDraw(item, specificHand,
				    manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None))
			{
				actor.Send(actor.Body.WhyCannotDraw(item, specificHand,
					manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None));
				return;
			}

			if (actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectDrawItem(actor,
				    item?.GetItemType<IWieldable>(), specificHand, emote,
				    manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Drawing {item?.HowSeen(actor) ?? "first available"}{(manual2hand ? " 2-handed" : "")}.");
			}

			return;
		}

		actor.Body.Draw(item, specificHand, emote,
			flags: manual2hand ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None);
	}

	[PlayerCommand("Strip", "strip")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMeleeCombatCommand]
	[HelpInfo("strip",
		@"This command can be used to quickly remove items of clothing on yourself or others. 

When used on yourself or someone else with no argument, it will attempt to remove all items of clothing. If you specify an additional argument, you can either specify a bodypart you want to fully expose (and it will only remove items necessary to expose that bodypart), or you can add COVERING <item> to remove all articles covering the specified item (though this syntax is generally only useful when using strip on yourself, because you probably won't be able to see the covered items you want on your target).

The possible syntaxes for this command are:

	#3strip#0 - strips all worn items off yourself
	#3strip <target>#0 - strips all worn items off a target
	#3strip me|<target> <part>#0 - strips items to expose the skin of a bodypart
	#3strip me covering <item>#0 - strips all items covering the specified item
	#3strip me|<target> into <container>#0 - strips all items into a container for storage
	#3strip me|<target> <part> <container>#0 - strips all items into a container to expose the skin of a bodypart",
		AutoHelp.HelpArg)]
	protected static void Strip(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			StripSelf(actor, ss, null);
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

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.Send("You dont see anyone like that to strip.");
			return;
		}

		if (target == actor)
		{
			StripSelf(actor, ss, emote);
			return;
		}

		StripOther(actor, target, ss, emote);
	}

	public static bool RecursiveRemoveItem(ICharacter actor, IGameItem item, IContainer intoContainer,
		out IGameItem problemItem)
	{
		if (item == null)
		{
			problemItem = null;
			return true;
		}

		if (!actor.Body.WornItems.Contains(item))
		{
			problemItem = null;
			return true;
		}

		if (actor.Body.CanRemoveItem(item, ItemCanGetIgnore.IgnoreFreeHands))
		{
			actor.Body.RemoveItem(item, null, ignoreFlags: ItemCanGetIgnore.IgnoreFreeHands);
			if (intoContainer?.CanPut(item) == true)
			{
				intoContainer.Put(actor, item);
			}
			else
			{
				item.RoomLayer = actor.RoomLayer;
				actor.Location.Insert(item);
			}

			problemItem = null;
			return true;
		}

		var itemsCoveringItem = actor.Body.CoverInformation(item)
		                             .Where(x => x.Item1 != WearableItemCoverStatus.Uncovered).ToList();
		if (!itemsCoveringItem.Any())
		{
			problemItem = item;
			return false;
		}

		foreach (var otherItem in itemsCoveringItem)
		{
			if (!RecursiveRemoveItem(actor, otherItem.Item2, intoContainer, out problemItem))
			{
				return false;
			}
		}

		return RecursiveRemoveItem(actor, item, intoContainer, out problemItem);
	}

	private static void StripSelf(ICharacter actor, StringStack ss, PlayerEmote emote)
	{
		TimeSpan TimeSpanForStrip(List<IGameItem> strippedItems)
		{
			var seconds = 0.0;
			var considered = new HashSet<IGameItem>();

			void ProcessSecondsForItem(IGameItem item)
			{
				if (item == null)
				{
					return;
				}

				if (considered.Contains(item))
				{
					return;
				}

				considered.Add(item);
				var wear = item.GetItemType<IWearable>();
				if (wear == null)
				{
					return;
				}

				seconds += wear.Bulky ? 2.0 : 0.33;
				foreach (var part in wear.CurrentProfile.AllProfiles)
				{
					var items = actor.Body.WornItemsFor(part.Key).SkipWhile(x => x != item).Skip(1);
					foreach (var otherItem in items)
					{
						ProcessSecondsForItem(otherItem);
					}
				}
			}

			foreach (var item in strippedItems)
			{
				ProcessSecondsForItem(item);
			}

			return TimeSpan.FromSeconds(seconds);
		}

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

		List<IGameItem> coveringItems = null;
		IContainer container = null;
		if (ss.Peek().EqualTo("covering"))
		{
			ss.Pop();
			if (ss.IsFinished)
			{
				actor.Send("Which item do you want to strip the covering items from?");
				return;
			}

			var coverItem = actor.Body.TargetWornItem(ss.PopSpeech());
			if (coverItem == null)
			{
				actor.Send("You are not wearing any such item.");
				return;
			}

			var coverItemInfo = actor.Body.CoverInformation(coverItem).ToList();
			if (!coverItemInfo.Any())
			{
				actor.Send($"{coverItem.HowSeen(actor, true)} is not covered by anything.");
				return;
			}

			coveringItems = coverItemInfo.Select(x => x.Item2).ToList();
			emoteText = ss.PopParentheses();
			if (!string.IsNullOrEmpty(emoteText))
			{
				emote = new PlayerEmote(emoteText, actor);
				if (!emote.Valid)
				{
					actor.Send(emote.ErrorMessage);
					return;
				}
			}
		}
		else if (ss.Peek().EqualTo("into"))
		{
			ss.Pop();
			var targetContainerItem = actor.TargetLocalOrHeldItem(ss.PopSpeech());
			if (targetContainerItem == null)
			{
				actor.Send("You do not see anything like that to put your stripped gear into.");
				return;
			}

			container = targetContainerItem.GetItemType<IContainer>();
			if (container == null)
			{
				actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not a container.");
				return;
			}

			if (targetContainerItem.GetItemType<IOpenable>()?.IsOpen == false)
			{
				actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not open.");
				return;
			}
		}
		else
		{
			IBodypart targetPart;
			if (!ss.IsFinished)
			{
				targetPart = actor.Body.GetTargetBodypart(ss.Pop());
				if (targetPart == null)
				{
					actor.Send("You don't have any such bodypart to strip and expose.");
					return;
				}

				emoteText = ss.PopParentheses();
				if (!string.IsNullOrEmpty(emoteText))
				{
					emote = new PlayerEmote(emoteText, actor);
					if (!emote.Valid)
					{
						actor.Send(emote.ErrorMessage);
						return;
					}
				}

				if (!ss.IsFinished)
				{
					var targetContainerItem = actor.TargetLocalOrHeldItem(ss.PopSpeech());
					if (targetContainerItem == null)
					{
						actor.Send("You do not see anything like that to put your stripped gear into.");
						return;
					}

					container = targetContainerItem.GetItemType<IContainer>();
					if (container == null)
					{
						actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not a container.");
						return;
					}

					if (targetContainerItem.GetItemType<IOpenable>()?.IsOpen == false)
					{
						actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not open.");
						return;
					}

					emoteText = ss.PopParentheses();
					if (!string.IsNullOrEmpty(emoteText))
					{
						emote = new PlayerEmote(emoteText, actor);
						if (!emote.Valid)
						{
							actor.Send(emote.ErrorMessage);
							return;
						}
					}
				}

				coveringItems = actor.Body.WornItemsFullInfo
				                     .Where(x => x.Wearloc == targetPart && x.Profile.PreventsRemoval)
				                     .Select(x => x.Item).ToList();
				if (!coveringItems.Any())
				{
					actor.Send("You are not wearing any items that are covering that bodypart.");
					return;
				}
			}
			else
			{
				coveringItems = actor.Body.DirectWornItems.ToList();
			}
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote($"@ begin|begins stripping off &0's gear", actor, actor),
				flags: OutputFlags.SuppressObscured).Append(emote));
		coveringItems.Reverse();

		actor.AddEffect(new SimpleCharacterAction(actor, perc =>
		{
			foreach (var item in coveringItems)
			{
				if (!RecursiveRemoveItem(actor, item, container, out var problem))
				{
					actor.Send($"You were unable to remove {problem.HowSeen(actor)} and anything below it.");
					return;
				}
			}
		}, "stripping off gear", "general", "stripping off gear"), TimeSpanForStrip(coveringItems));
	}

	private static void StripOther(ICharacter actor, ICharacter target, StringStack ss, PlayerEmote emote)
	{
		bool StripOtherRecursiveRemoveItem(IGameItem item, IContainer intoContainer, out IGameItem problemItem)
		{
			if (item == null)
			{
				problemItem = null;
				return true;
			}

			if (!target.Body.DirectWornItems.Contains(item))
			{
				problemItem = null;
				return true;
			}

			if (target.Body.CanRemoveItem(item, ItemCanGetIgnore.IgnoreFreeHands))
			{
				target.Body.RemoveItem(item, null, actor);
				if (intoContainer?.CanPut(item) == true)
				{
					intoContainer.Put(actor, item);
				}
				else
				{
					item.RoomLayer = actor.RoomLayer;
					actor.Location.Insert(item);
				}

				problemItem = null;
				return true;
			}

			var itemsCoveringItem = target.Body.CoverInformation(item)
			                              .Where(x => x.Item1 != WearableItemCoverStatus.Uncovered).ToList();
			if (!itemsCoveringItem.Any())
			{
				problemItem = item;
				return false;
			}

			foreach (var otherItem in itemsCoveringItem)
			{
				if (!StripOtherRecursiveRemoveItem(otherItem.Item2, intoContainer, out problemItem))
				{
					return false;
				}
			}

			return StripOtherRecursiveRemoveItem(item, intoContainer, out problemItem);
		}

		TimeSpan TimeSpanForStrip(List<IGameItem> strippedItems)
		{
			var seconds = 0.0;
			var considered = new HashSet<IGameItem>();

			void ProcessSecondsForItem(IGameItem item)
			{
				if (item == null)
				{
					return;
				}

				if (considered.Contains(item))
				{
					return;
				}

				considered.Add(item);
				var wear = item.GetItemType<IWearable>();
				if (wear == null)
				{
					return;
				}

				seconds += wear.Bulky ? 4.0 : 0.5;
				foreach (var part in wear.CurrentProfile.AllProfiles)
				{
					var items = target.Body.WornItemsFor(part.Key).SkipWhile(x => x != item).Skip(1).ToList();
					foreach (var otherItem in items)
					{
						ProcessSecondsForItem(otherItem);
					}
				}
			}

			foreach (var item in strippedItems)
			{
				ProcessSecondsForItem(item);
			}

			return TimeSpan.FromSeconds(seconds);
		}

		List<IGameItem> coveringItems = null;
		IContainer container = null;
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

		IBodypart targetPart;
		if (!ss.IsFinished && ss.Peek().EqualTo("into"))
		{
			ss.Pop();
			var targetContainerItem = actor.TargetLocalOrHeldItem(ss.PopSpeech());
			if (targetContainerItem == null)
			{
				actor.Send("You do not see anything like that to put your stripped gear into.");
				return;
			}

			container = targetContainerItem.GetItemType<IContainer>();
			if (container == null)
			{
				actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not a container.");
				return;
			}

			if (targetContainerItem.GetItemType<IOpenable>()?.IsOpen == false)
			{
				actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not open.");
				return;
			}
		}
		else if (!ss.IsFinished)
		{
			targetPart = target.Body.GetTargetBodypart(ss.Pop());
			if (targetPart == null)
			{
				actor.Send($"{target.HowSeen(actor, true)} doesn't have any such bodypart to strip and expose.");
				return;
			}

			emoteText = ss.PopParentheses();
			if (!string.IsNullOrEmpty(emoteText))
			{
				emote = new PlayerEmote(emoteText, actor);
				if (!emote.Valid)
				{
					actor.Send(emote.ErrorMessage);
					return;
				}
			}

			if (!ss.IsFinished)
			{
				var targetContainerItem = actor.TargetLocalOrHeldItem(ss.PopSpeech());
				if (targetContainerItem == null)
				{
					actor.Send("You do not see anything like that to put the stripped gear into.");
					return;
				}

				container = targetContainerItem.GetItemType<IContainer>();
				if (container == null)
				{
					actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not a container.");
					return;
				}

				if (targetContainerItem.GetItemType<IOpenable>()?.IsOpen == false)
				{
					actor.Send($"{targetContainerItem.HowSeen(actor, true)} is not open.");
					return;
				}

				emoteText = ss.PopParentheses();
				if (!string.IsNullOrEmpty(emoteText))
				{
					emote = new PlayerEmote(emoteText, actor);
					if (!emote.Valid)
					{
						actor.Send(emote.ErrorMessage);
						return;
					}
				}
			}

			coveringItems = target.Body.WornItemsFullInfo
			                      .Where(x => x.Wearloc == targetPart && x.Profile.PreventsRemoval).Select(x => x.Item)
			                      .ToList();
			if (!coveringItems.Any())
			{
				actor.Send($"{target.HowSeen(actor, true)} is not wearing any items that are covering that bodypart.");
				return;
			}
		}
		else
		{
			coveringItems = target.Body.DirectWornItems.ToList();
		}

		void BeginStrip()
		{
			actor.OutputHandler.Handle(
				new MixedEmoteOutput(new Emote($"@ begin|begins stripping off $0's gear", actor, target),
					flags: OutputFlags.SuppressObscured).Append(emote));
			coveringItems.Reverse();

			actor.AddEffect(new SimpleCharacterAction(actor, perc =>
			{
				foreach (var item in coveringItems)
				{
					if (!StripOtherRecursiveRemoveItem(item, container, out var problem))
					{
						actor.Send($"You were unable to remove {problem.HowSeen(actor)} and anything below it.");
						return;
					}
				}
			}, "stripping off gear", "general", "stripping off gear"), TimeSpanForStrip(coveringItems));
		}

		if (target.WillingToPermitInventoryManipulation(actor))
		{
			BeginStrip();
		}
		else
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(@"@ are|is proposing to strip $0's gear.", actor,
				target)));
			target.Send(
				$"You must type {"accept".Colour(Telnet.Yellow)} to permit them to do so, or {"decline".Colour(Telnet.Yellow)} to decline.");
			target.AddEffect(new Accept(target, new GenericProposal(
				perc => { BeginStrip(); },
				perc =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines to be stripped.", target)));
				},
				() =>
				{
					target.OutputHandler.Handle(
						new EmoteOutput(new Emote("@ decline|declines to be stripped.", target)));
				},
				"stripping off gear",
				"strip"
			)), TimeSpan.FromSeconds(120));
		}
	}

	[PlayerCommand("Outfit", "outfit")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("outfit",
		"The outfit command is used to create, edit, view and manage outfits, which are collections of worn items. Outfits are specific to a character, but can be taught to other characters as well. The valid syntaxes are:\n\n\tOUTFIT EDIT <outfit> - opens an outfit for editing\n\tOUTFIT EDIT NEW <name> - creates a new outfit for editing\n\tOUTFIT CLOSE - closes an open outfit\n\tOUTFIT DELETE - deletes the open outfit\n\tOUTFIT SHOW - shows the open outfit\n\tOUTFIT SHOW <name> - shows the named outfit\n\tOUTFIT SET <parameters> - See OUTFIT SET HELP for more info\n\tOUTFIT CLONE <newname> - clones the open outfit to a new outfit\n\tOUTFIT TEACH <outfit> <person> - teaches the named outfit to someone else\n\tOUTFIT WEAR <name> - wears the specified outfit\n\tOUTFIT REMOVE <name> - removes the specified outfit ",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Outfit(ICharacter actor, string command)
	{
		// TODO - finish help command
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				OutfitList(actor, ss);
				return;
			case "show":
				OutfitShow(actor, ss);
				return;
			case "edit":
				OutfitEdit(actor, ss);
				return;
			case "set":
				OutfitSet(actor, ss);
				return;
			case "teach":
				OutfitTeach(actor, ss);
				return;
			case "close":
				OutfitClose(actor, ss);
				return;
			case "wear":
				OutfitWear(actor, ss);
				return;
			case "remove":
				OutfitRemove(actor, ss);
				return;
			case "delete":
				OutfitDelete(actor, ss);
				return;
			case "clone":
				OutfitClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid option for use with this command. Please see {"OUTFIT HELP".FluentTagMXP("send", "href='outfit help' hint='Show the helpfile for outfit'")} for more info.");
				return;
		}
	}

	private static void OutfitClone(ICharacter actor, StringStack ss)
	{
		if (!actor.AffectedBy<BuilderEditingEffect<IOutfit>>())
		{
			actor.OutputHandler.Send("You are not editing any outfits at the moment.");
			return;
		}

		var outfit = actor.EffectsOfType<BuilderEditingEffect<IOutfit>>().First().EditingItem;

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the clone of the outfit?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		if (actor.Outfits.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"You already have an outfit with the name {name.TitleCase().Colour(Telnet.Cyan)}. Names must be unique.");
			return;
		}

		var newOutfit = outfit.CopyOutfit(actor, name);
		actor.AddOutfit(newOutfit);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IOutfit>>());
		actor.AddEffect(new BuilderEditingEffect<IOutfit>(actor) { EditingItem = newOutfit });
		actor.OutputHandler.Send(
			$"You clone your outfit {outfit.Name.TitleCase().Colour(Telnet.Cyan)} to a new outfit called {newOutfit.Name.TitleCase().Colour(Telnet.Cyan)}. You are now editing the new outfit.");
	}

	private static void OutfitDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.AffectedBy<BuilderEditingEffect<IOutfit>>())
		{
			actor.OutputHandler.Send("You are not editing any outfits at the moment.");
			return;
		}

		var outfit = actor.EffectsOfType<BuilderEditingEffect<IOutfit>>().First().EditingItem;
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = dummy =>
			{
				actor.OutputHandler.Send($"You delete the {outfit.Name.TitleCase().Colour(Telnet.Cyan)} outfit.");
				actor.RemoveOutfit(outfit);
			},
			RejectAction = dummy =>
			{
				actor.OutputHandler.Send(
					$"You decide not to delete the {outfit.Name.TitleCase().Colour(Telnet.Cyan)} outfit.");
			},
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to delete the {outfit.Name.TitleCase().Colour(Telnet.Cyan)} outfit.");
			},
			DescriptionString = "deleting an outfit",
			Keywords = new List<string> { "delete", "outfit" }
		}), TimeSpan.FromSeconds(120));
		actor.OutputHandler.Send(
			$"Are you sure you want to delete your outfit called {outfit.Name.TitleCase().Colour(Telnet.Cyan)}? This is permanent and irreversible. You must type {"ACCEPT".ColourCommand()} to confirm, or {"REJECT".ColourCommand()} to cancel.");
	}

	private static void OutfitRemove(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which outfit do you want to remove?");
			return;
		}

		var name = ss.PopSpeech();
		var outfit = actor.Outfits.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		             actor.Outfits.FirstOrDefault(
			             x => x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
		if (outfit == null)
		{
			actor.OutputHandler.Send("You have no such outfit.");
			return;
		}

		PlayerEmote emote = null;
		var emoteText = ss.PopParentheses();
		if (!string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, actor);
			if (!emote.Valid)
			{
				actor.OutputHandler.Send(emote.ErrorMessage);
				return;
			}
		}

		var items = actor.Body.DirectWornItems.Where(x => outfit.Items.Any(y => y.Id == x.Id)).Reverse().ToList();
		if (!items.Any())
		{
			actor.OutputHandler.Send("You are not wearing any items from that outfit.");
			return;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote($"@ begin|begins stripping off &0's gear", actor, actor),
				flags: OutputFlags.SuppressObscured).Append(emote));

		actor.AddEffect(new SimpleCharacterAction(actor, perc =>
		{
			foreach (var outfitItem in items)
			{
				var coverItemInfo = actor.Body.CoverInformation(outfitItem).Where(x => x.Item2 != null).ToList();
				var coveringItems = new List<IGameItem>();
				if (coverItemInfo.Any())
				{
					coveringItems.AddRange(coverItemInfo.SelectNotNull(x => x.Item2).Reverse());
				}

				coveringItems.Add(outfitItem);
				foreach (var item in coveringItems)
				{
					var containerId = outfit.Items.FirstOrDefault(x => x.Id == item.Id)?.PreferredContainerId;
					var container = actor.ContextualItems.FirstOrDefault(
						                     x => x.Id == containerId &&
						                          x.GetItemType<IContainer>()?.CanPut(item) == true)
					                     ?.GetItemType<IContainer>();
					if (!RecursiveRemoveItem(actor, item, container, out var problem))
					{
						actor.Send($"You were unable to remove {problem.HowSeen(actor)} and anything below it.");
						return;
					}
				}
			}
		}, "stripping off gear", "general", "stripping off gear"), TimeSpan.FromSeconds(5));
	}

	private static void OutfitWear(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which outfit were you wanting to wear?");
			return;
		}

		var name = ss.PopSpeech();
		var outfit = actor.Outfits.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		             actor.Outfits.FirstOrDefault(
			             x => x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
		if (outfit == null)
		{
			actor.OutputHandler.Send("You don't have any outfit like that to wear.");
			return;
		}

		var force = !ss.IsFinished && ss.Pop().EqualTo("!");
		var i = 1;
		var plan = new InventoryPlanTemplate(actor.Gameworld,
			from item in outfit.Items
			select new InventoryPlanPhaseTemplate(i++,
				new[]
				{
					new InventoryPlanActionWear(actor.Gameworld, 0, 0, x => x.Id == item.Id, null)
						{ DesiredProfile = item.DesiredProfile, OriginalReference = item.ItemDescription }
				})
		);

		var charPlan = plan.CreatePlan(actor);
		var feasible = charPlan.PlanIsFeasible();
		var infeasible = charPlan.InfeasibleActions().ToList();
		if (force && feasible != InventoryPlanFeasibility.Feasible)
		{
			if (infeasible.Count == outfit.Items.Count())
			{
				actor.OutputHandler.Send("None of the items in that outfit are available to wear.");
				return;
			}
		}

		if (feasible == InventoryPlanFeasibility.Feasible ||
		    (feasible == InventoryPlanFeasibility.NotFeasibleMissingItems && force))
		{
			var results = charPlan.ExecuteWholePlan();
			charPlan.FinalisePlanWithExemptions(results.Select(x => x.PrimaryTarget).ToList());
			return;
		}

		var missingText = infeasible.Select(x =>
			                            $"{x.Action.OriginalReference.ToString().Colour(Telnet.Green)} {(x.Reason == InventoryPlanFeasibility.NotFeasibleMissingItems ? "[missing]" : x.Reason == InventoryPlanFeasibility.NotFeasibleNotEnoughHands ? "[nohands]" : "[nowield]").Colour(Telnet.Red)}")
		                            .ListToCompactString();
		actor.OutputHandler.Send(
			$"You are missing the following items from the outfit that you cannot locate or wear: {missingText}\n{"You can wear whatever is actually available for the outfit by appending a ! to the end of your command.".ColourCommand()}");
	}

	private static void OutfitClose(ICharacter actor, StringStack ss)
	{
		if (!actor.AffectedBy<BuilderEditingEffect<IOutfit>>())
		{
			actor.OutputHandler.Send("You are not editing any outfits at the moment.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IOutfit>>());
		actor.OutputHandler.Send("You are no longer editing any outfits.");
	}

	private static void OutfitTeach(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which outfit do you want to teach to another person?");
			return;
		}

		var name = ss.PopSpeech();
		var outfit = actor.Outfits.FirstOrDefault(x => x.Name.EqualTo(name));
		if (outfit == null)
		{
			actor.OutputHandler.Send("You do not have any outfit with that name to teach someone.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to teach that outfit to?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that to teach that outfit to.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send(
				"You cannot teach yourself an outfit. Use CLONE if you really want to make a copy of it.");
			return;
		}

		target.AddEffect(new Accept(target, new GenericProposal
		{
			AcceptAction = text =>
			{
				var newName = outfit.Name;
				if (!string.IsNullOrWhiteSpace(text))
				{
					newName = text;
				}

				while (target.Outfits.Any(x => x.Name.EqualTo(newName)))
				{
					newName = newName.IncrementNumberOrAddNumber();
				}

				var newOutfit = outfit.CopyOutfit(target, newName);
				target.AddOutfit(newOutfit);
				target.OutputHandler.Send(
					$"You have learned the outfit from {actor.HowSeen(target)}, called {newName.TitleCase().Colour(Telnet.Cyan)}.");
				actor.OutputHandler.Send(
					$"You teach your outfit {outfit.Name.TitleCase().Colour(Telnet.Cyan)} to {target.HowSeen(actor)}.");
			},
			RejectAction = dummy =>
			{
				target.OutputHandler.Send($"You decide against learning a new outfit from {actor.HowSeen(target)}.");
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} decides against learning a new outfit from you.");
			},
			ExpireAction = () =>
			{
				target.OutputHandler.Send($"You decide against learning a new outfit from {actor.HowSeen(target)}.");
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} decides against learning a new outfit from you.");
			},
			DescriptionString = "being taught a new outfit",
			Keywords = new List<string> { "outfit", "learn" }
		}), TimeSpan.FromSeconds(120));

		actor.OutputHandler.Send(
			$"You propose to teach your outfit {outfit.Name.TitleCase().Colour(Telnet.Cyan)} to {target.HowSeen(actor)}.");
		target.OutputHandler.Send(
			$"{actor.HowSeen(target, true)} is proposing to teach your their outfit called {outfit.Name.TitleCase().Colour(Telnet.Cyan)}.\nTyping {"ACCEPT".ColourCommand()} will add this to your list of outfits with the same name. You can specify your own name with the syntax {"ACCEPT <new name>".ColourCommand()}, or you can simply {"REJECT".ColourCommand()} to not learn it.");
	}

	private static void OutfitSet(ICharacter actor, StringStack ss)
	{
		if (!actor.AffectedBy<BuilderEditingEffect<IOutfit>>())
		{
			actor.OutputHandler.Send("You are not editing any outfits at the moment.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to set about your edited outfit? See OUTFIT SET HELP for more info.");
			return;
		}

		actor.EffectsOfType<BuilderEditingEffect<IOutfit>>().First().EditingItem.BuildingCommand(actor, ss);
	}

	private static void OutfitEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.AffectedBy<BuilderEditingEffect<IOutfit>>())
			{
				OutfitShow(actor, ss);
				return;
			}

			actor.OutputHandler.Send(
				"You are not currently editing any outfits. Use OUTFIT EDIT <name> to begin editing one.");
			return;
		}

		var name = ss.PopSpeech();
		if (name.EqualTo("new"))
		{
			name = ss.PopSpeech();
			if (string.IsNullOrWhiteSpace(name))
			{
				actor.OutputHandler.Send("What name do you want to give to your new outfit?");
				return;
			}

			if (actor.Outfits.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("You already have an outfit with that name. Outfit names must be unique.");
				return;
			}

			var newOutfit = new Outfit(actor, name);
			actor.AddOutfit(newOutfit);
			actor.OutfitsChanged = true;
			actor.OutputHandler.Send(
				$"You create a new outfit called {name.TitleCase().Colour(Telnet.Cyan)}, which you are now editing.");
			actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IOutfit>>());
			actor.AddEffect(new BuilderEditingEffect<IOutfit>(actor) { EditingItem = newOutfit });
			return;
		}

		var outfit = actor.Outfits.FirstOrDefault(x => x.Name.EqualTo(name));
		if (outfit == null)
		{
			actor.OutputHandler.Send("You do not have any outfit with that name.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IOutfit>>());
		actor.AddEffect(new BuilderEditingEffect<IOutfit>(actor) { EditingItem = outfit });
		actor.OutputHandler.Send($"You open the {outfit.Name.TitleCase().Colour(Telnet.Cyan)} outfit for editing.");
	}

	private static void OutfitShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.AffectedBy<BuilderEditingEffect<IOutfit>>())
			{
				actor.OutputHandler.Send(actor.EffectsOfType<BuilderEditingEffect<IOutfit>>().First().EditingItem
				                              .Show(actor));
				return;
			}

			actor.OutputHandler.Send(
				"You are not currently editing any outfits. Use OUTFIT EDIT <name> to begin editing one.");
			return;
		}

		var name = ss.PopSpeech();
		var outfit = actor.Outfits.FirstOrDefault(x => x.Name.EqualTo(name));
		if (outfit == null)
		{
			actor.OutputHandler.Send("You do not have any outfit with that name.");
			return;
		}

		actor.OutputHandler.Send(outfit.Show(actor));
	}

	private static void OutfitList(ICharacter actor, StringStack ss)
	{
		if (!actor.Outfits.Any())
		{
			actor.OutputHandler.Send("You do not currently have any outfits.");
			return;
		}

		var sb = new StringBuilder();
		actor.OutputHandler.Send("You have the following outfits:");
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from outfit in actor.Outfits
				let count = outfit.Items.Count()
				let worn = new HashSet<long>(actor.Body.WornItems.Select(x => x.Id))
				let here = new HashSet<long>(actor.DeepContextualItems.Select(x => x.Id).Except(worn))
				select new[]
				{
					outfit.Name,
					count.ToString("N0", actor),
					((double)outfit.Items.Count(x => worn.Contains(x.Id)) / (count > 0 ? count : 1)).ToString("P2",
						actor),
					((double)outfit.Items.Count(x => here.Contains(x.Id)) / (count > 0 ? count : 1)).ToString("P2",
						actor)
				},
				new[] { "Name", "Garments", "Worn?", "Here?" },
				actor.LineFormatLength,
				colour: Telnet.Green,
				unicodeTable: actor.Account.UseUnicode
			));
	}

	[PlayerCommand("Outfits", "outfits")]
	protected static void Outfits(ICharacter actor, string command)
	{
		OutfitList(actor, new StringStack(command.RemoveFirstWord()));
	}
}