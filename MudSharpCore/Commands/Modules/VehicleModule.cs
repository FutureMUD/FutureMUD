using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DB = MudSharp.Models;

namespace MudSharp.Commands.Modules;

internal class VehicleModule : Module<ICharacter>
{
	private VehicleModule() : base("Vehicles")
	{
		IsNecessary = true;
	}

	public static VehicleModule Instance { get; } = new();
	private static readonly IVehicleTowService TowService = new VehicleTowService();
	private static readonly IVehicleHitchService HitchService = new VehicleHitchService();
	private static readonly IVehicleHitchGraphService HitchGraphService = new VehicleHitchGraphService();

	private const string EmbarkHelp = @"The #3embark#0 command lets you board a vehicle exterior item.

Syntax:
	#3embark <vehicle>#0
	#3embark <vehicle> <slot id|driver|passenger|crew>#0
	#3embark <vehicle> [slot] via <access id|name>#0";

	[PlayerCommand("Embark", "embark")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoHideCommand]
	[HelpInfo("embark", EmbarkHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Embark(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			var vehicles = actor.Location.LayerGameItems(actor.RoomLayer)
			                    .SelectNotNull(x => x.GetItemType<IVehicleExterior>())
			                    .Where(x => x.Vehicle is not null)
			                    .ToList();
			if (!vehicles.Any())
			{
				actor.OutputHandler.Send("There are no vehicles here that you can board.");
				return;
			}

			var sb = new StringBuilder();
			sb.AppendLine("You can board the following vehicles:");
			foreach (var vehicle in vehicles)
			{
				sb.AppendLine($"\t{vehicle.Parent.HowSeen(actor)} - {vehicle.Vehicle.Name.ColourName()}");
			}

			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var item = actor.TargetLocalItem(ss.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("You don't see any vehicle like that here.");
			return;
		}

		var exterior = item.GetItemType<IVehicleExterior>();
		if (exterior?.Vehicle is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not a linked vehicle.");
			return;
		}

		var (slotText, accessText) = ParseEmbarkRemainder(ss.SafeRemainingArgument);
		var slot = ResolveSlot(actor, exterior.Vehicle, slotText);
		if (!string.IsNullOrWhiteSpace(slotText) && slot is null)
		{
			actor.OutputHandler.Send("There is no such occupant slot on that vehicle.");
			return;
		}

		var access = ResolveAccess(exterior.Vehicle, accessText);
		if (!string.IsNullOrWhiteSpace(accessText) && access is null)
		{
			actor.OutputHandler.Send("There is no such access point on that vehicle.");
			return;
		}

		if (!exterior.Vehicle.CanBoard(actor, slot, access, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		exterior.Vehicle.Board(actor, slot, access);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ board|boards $1.", actor, actor, item)));
	}

	[PlayerCommand("Disembark", "disembark", "unboard")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[NoHideCommand]
	[HelpInfo("disembark", "Use #3disembark#0 to leave the vehicle you are currently aboard.", AutoHelp.HelpArg)]
	protected static void Disembark(ICharacter actor, string input)
	{
		var vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x => x.IsOccupant(actor));
		if (vehicle is null)
		{
			actor.OutputHandler.Send("You are not aboard a vehicle.");
			return;
		}

		if (!vehicle.CanLeave(actor, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		vehicle.Leave(actor);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ disembark|disembarks from $1.", actor, actor, vehicle.ExteriorItem)));
	}

	[PlayerCommand("Drive", "drive")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("movement", "You cannot move until you stop {0}.")]
	[NoHideCommand]
	[HelpInfo("drive", "Use #3drive <direction>#0 while in a driver slot to move your vehicle through a normal exit. Ordinary movement commands like #3north#0 and #3east#0 do the same thing while you are controlling a vehicle.", AutoHelp.HelpArgOrNoArg)]
	protected static void Drive(ICharacter actor, string input)
	{
		VehicleMovementCommand.TryMoveControlledVehicle(actor, input.RemoveFirstWord(), true);
	}

	[PlayerCommand("Hitch", "hitch")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("hitch", "Use #3hitch <towpoint>@<vehicle> <towpoint>@<target> [with <hitch item>]#0 to link vehicles for towing, or #3hitch <character> <character|towpoint@vehicle> [with <hitch item>]#0 to hitch a character or mount to pull something. Non-direct tow point types require compatible hitch gear or a legacy drag aid; direct hand/manual/pull-style points do not.", AutoHelp.HelpArgOrNoArg)]
	protected static void Hitch(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tow point do you want to hitch from?");
			return;
		}

		var sourceText = ss.PopSpeech();
		if (!sourceText.Contains('@'))
		{
			HitchCharacter(actor, ss, sourceText);
			return;
		}

		if (!ResolveTowPoint(actor, sourceText, true, out var sourceVehicle, out var sourcePoint))
		{
			return;
		}

		if (!ResolveTowPoint(actor, ss.PopSpeech(), false, out var targetVehicle, out var targetPoint))
		{
			return;
		}

		if (sourceVehicle.ExteriorItem is null || targetVehicle.ExteriorItem is null)
		{
			actor.OutputHandler.Send("Both vehicles must have linked exterior items to be hitched.");
			return;
		}

		if (sourceVehicle.ExteriorItem?.AffectedBy<Dragging.DragTarget>() == true)
		{
			actor.OutputHandler.Send($"{sourceVehicle.ExteriorItem.HowSeen(actor, true)} is already being pulled or dragged.");
			return;
		}

		if (targetVehicle.ExteriorItem?.AffectedBy<Dragging.DragTarget>() == true)
		{
			actor.OutputHandler.Send($"{targetVehicle.ExteriorItem.HowSeen(actor, true)} is already being pulled or dragged.");
			return;
		}

		IGameItem hitchItem = null;
		if (!ss.IsFinished)
		{
			if (!ss.PopSpeech().EqualTo("with"))
			{
				actor.OutputHandler.Send("Use #3with <item>#0 to specify a hitch item.".SubstituteANSIColour());
				return;
			}

			hitchItem = actor.TargetHeldItem(ss.SafeRemainingArgument);
			if (hitchItem is null)
			{
				actor.OutputHandler.Send("You are not holding any such hitch item.");
				return;
			}
		}

		if (!TowService.CanHitch(actor, sourceVehicle, sourcePoint, targetVehicle, targetPoint, hitchItem, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (hitchItem is not null)
		{
			actor.Body.Drop(hitchItem, silent: true);
			if (hitchItem.Location != sourceVehicle.Location || hitchItem.RoomLayer != sourceVehicle.RoomLayer)
			{
				hitchItem.Location?.Extract(hitchItem);
				hitchItem.RoomLayer = sourceVehicle.RoomLayer;
				sourceVehicle.Location.Insert(hitchItem, true);
			}
		}

		DB.VehicleTowLink dbitem;
		using (new FMDB())
		{
			dbitem = new DB.VehicleTowLink
			{
				SourceVehicleId = sourceVehicle.Id,
				TargetVehicleId = targetVehicle.Id,
				SourceTowPointProtoId = sourcePoint.Id,
				TargetTowPointProtoId = targetPoint.Id,
				HitchItemId = hitchItem?.Id,
				IsDisabled = false,
				CreatedDateTime = DateTime.UtcNow
			};
			FMDB.Context.VehicleTowLinks.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var link = new VehicleTowLink(actor.Gameworld, dbitem);
		if (sourceVehicle is Vehicle source)
		{
			source.AddTowLink(link);
		}

		if (targetVehicle is Vehicle target)
		{
			target.AddTowLink(link);
		}

		HitchGearRules.Reserve(hitchItem, vehicleTowLinkId: link.Id);
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ hitch|hitches $1 to $2.", actor, actor,
			sourceVehicle.ExteriorItem, targetVehicle.ExteriorItem)));
	}

	private static void HitchCharacter(ICharacter actor, StringStack ss, string sourceText)
	{
		var source = ResolveHitchCharacter(actor, sourceText);
		if (source is null)
		{
			actor.OutputHandler.Send("You do not see any character or mount like that to hitch.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What do you want to hitch {source.HowSeen(actor)} to?");
			return;
		}

		var targetText = ss.PopSpeech();
		IPerceivable target;
		IVehicle targetVehicle = null;
		IVehicleTowPointPrototype targetTowPoint = null;
		if (targetText.Contains('@'))
		{
			if (!ResolveTowPoint(actor, targetText, false, out targetVehicle, out targetTowPoint))
			{
				return;
			}

			if (targetVehicle.ExteriorItem is null)
			{
				actor.OutputHandler.Send("That vehicle does not have a linked exterior item.");
				return;
			}

			target = targetVehicle.ExteriorItem;
		}
		else
		{
			target = ResolveHitchCharacter(actor, targetText);
			if (target is null)
			{
				actor.OutputHandler.Send("You do not see any character, mount, or vehicle tow point like that to hitch to.");
				return;
			}
		}

		IGameItem hitchItem = null;
		IDragAid dragAid = null;
		if (!ss.IsFinished)
		{
			if (!ss.PopSpeech().EqualTo("with"))
			{
				actor.OutputHandler.Send("Use #3with <item>#0 to specify a hitch item.".SubstituteANSIColour());
				return;
			}

			hitchItem = ResolveCharacterHitchItem(actor, source, target as ICharacter, ss.SafeRemainingArgument);
			if (hitchItem is null)
			{
				actor.OutputHandler.Send("You do not see any available hitch item like that.");
				return;
			}

			dragAid = HitchGearRules.DragAidFor(hitchItem);
			if (dragAid is null)
			{
				actor.OutputHandler.Send($"{hitchItem.HowSeen(actor, true)} is not usable hitch gear.");
				return;
			}
		}

		if (targetVehicle is not null && HitchGearRules.TowPointRequiresHitchItem(targetTowPoint) && dragAid is null)
		{
			actor.OutputHandler.Send($"That {targetTowPoint.TowType.ColourCommand()} tow point requires a hitch item. Use #3with <item>#0.".SubstituteANSIColour());
			return;
		}

		if (!CanCharacterHitch(actor, source, target, targetVehicle, targetTowPoint, hitchItem, dragAid, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (CanActorDirectlyHitchSource(actor, source))
		{
			ApplyCharacterHitch(actor, source, target, targetVehicle, targetTowPoint, hitchItem, dragAid);
			return;
		}

		OfferCharacterHitch(actor, source, target, targetVehicle, targetTowPoint, hitchItem, dragAid);
	}

	[PlayerCommand("Unhitch", "unhitch")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("unhitch", "Use #3unhitch <vehicle>#0, #3unhitch <towpoint>@<vehicle>#0, or #3unhitch <character>#0 to remove tow or character hitches and release any reserved hitch gear.", AutoHelp.HelpArgOrNoArg)]
	protected static void Unhitch(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which vehicle or tow point do you want to unhitch?");
			return;
		}

		var text = ss.SafeRemainingArgument;
		IGameItem item;
		IVehicle vehicle;
		List<IVehicleTowLink> links;
		if (text.Contains('@'))
		{
			if (!ResolveTowPoint(actor, text, out vehicle, out var towPoint))
			{
				return;
			}

			item = vehicle.ExteriorItem;
			if (item is not null && RemoveCharacterHitchesTargeting(actor, item, towPoint.Id))
			{
				return;
			}

			links = vehicle.TowLinks.Where(x =>
				(x.SourceVehicle == vehicle && x.SourceTowPoint?.Id == towPoint.Id) ||
				(x.TargetVehicle == vehicle && x.TargetTowPoint?.Id == towPoint.Id)).ToList();
		}
		else
		{
			var character = ResolveHitchCharacter(actor, text);
			if (character is not null && RemoveCharacterHitches(actor, character))
			{
				return;
			}

			item = actor.TargetLocalItem(text);
			vehicle = item?.GetItemType<IVehicleExterior>()?.Vehicle;
			if (vehicle is null)
			{
				actor.OutputHandler.Send("You do not see any linked vehicle like that here.");
				return;
			}

			if (RemoveCharacterHitchesTargeting(actor, item))
			{
				return;
			}

			links = vehicle.TowLinks.ToList();
		}

		if (!links.Any())
		{
			actor.OutputHandler.Send("There are no tow links like that to remove.");
			return;
		}

		using (new FMDB())
		{
			foreach (var link in links)
			{
				var dbitem = FMDB.Context.VehicleTowLinks.Find(link.Id);
				if (dbitem is not null)
				{
					FMDB.Context.VehicleTowLinks.Remove(dbitem);
				}

				if (link.SourceVehicle is Vehicle source)
				{
					source.RemoveTowLink(link.Id);
				}

				if (link.TargetVehicle is Vehicle target)
				{
					target.RemoveTowLink(link.Id);
				}

				HitchGearRules.Release(link.HitchItem, vehicleTowLinkId: link.Id);
			}

			FMDB.Context.SaveChanges();
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ unhitch|unhitches $1.", actor, actor, item)));
	}

	private static bool RemoveCharacterHitches(ICharacter actor, ICharacter character)
	{
		var persistentLinks = HitchService.LinksInvolving(actor.Gameworld, character).ToList();
		var hitches = character.EffectsOfType<Dragging>().ToList();
		var targetHitches = actor.Location.LayerCharacters(actor.RoomLayer)
		                         .SelectMany(x => x.EffectsOfType<Dragging>())
		                         .Where(x => x.Target == character)
		                         .ToList();
		hitches.AddRange(targetHitches);
		hitches = hitches.Distinct().ToList();
		if (!hitches.Any() && !persistentLinks.Any())
		{
			return false;
		}

		foreach (var link in persistentLinks)
		{
			HitchService.DeletePersistentLink(actor.Gameworld, link.Id);
		}

		foreach (var hitch in hitches)
		{
			hitch.CharacterOwner.RemoveAllEffects(x => x == hitch, true);
			hitch.CharacterOwner.RemoveAllEffects<CharacterHitch>(x => x.Target == hitch.Target, true);
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ unhitch|unhitches $1.", actor, actor, character)));
		return true;
	}

	private static bool RemoveCharacterHitchesTargeting(ICharacter actor, IPerceivable target, long? targetTowPointId = null)
	{
		var persistentLinks = HitchService.LinksInvolving(actor.Gameworld, target)
		                                .Where(x => targetTowPointId is null ||
		                                            x.TargetTowPointPrototypeId == targetTowPointId ||
		                                            x.SourceTowPointPrototypeId == targetTowPointId)
		                                .ToList();
		var hitches = actor.Location.LayerCharacters(actor.RoomLayer)
		                  .SelectMany(x => x.EffectsOfType<CharacterHitch>().Select(y => (Character: x, Hitch: y)))
		                  .Where(x => x.Hitch.Target == target &&
		                              (targetTowPointId is null || x.Hitch.TargetTowPointId == targetTowPointId))
		                  .Distinct()
		                  .ToList();
		if (!hitches.Any() && !persistentLinks.Any())
		{
			return false;
		}

		foreach (var link in persistentLinks)
		{
			HitchService.DeletePersistentLink(actor.Gameworld, link.Id);
		}

		foreach (var hitch in hitches)
		{
			hitch.Character.RemoveAllEffects<Dragging>(x => x.Target == hitch.Hitch.Target, true);
			hitch.Character.RemoveAllEffects(x => x == hitch.Hitch, true);
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ unhitch|unhitches $1.", actor, actor, target)));
		return true;
	}

	private static ICharacter ResolveHitchCharacter(ICharacter actor, string text)
	{
		if (text.EqualToAny("me", "self"))
		{
			return actor;
		}

		return actor.TargetActor(text);
	}

	private static IGameItem ResolveCharacterHitchItem(ICharacter actor, ICharacter source, ICharacter target, string text)
	{
		var candidates = actor.Body.ExternalItems
		                      .Concat(source.Body.ExternalItemsForOtherActors)
		                      .Concat(target?.Body.ExternalItemsForOtherActors ?? Enumerable.Empty<IGameItem>())
		                      .Concat(actor.Location.LayerGameItems(actor.RoomLayer))
		                      .Where(x => actor.CanSee(x))
		                      .Distinct()
		                      .ToList();
		return candidates.GetFromItemListByKeyword(text, actor);
	}

	private static bool TowPointRequiresCharacterHitchItem(IVehicleTowPointPrototype towPoint)
	{
		return !towPoint.TowType.EqualToAny("hand", "manual", "direct", "none", "pull");
	}

	private static bool CanActorDirectlyHitchSource(ICharacter actor, ICharacter source)
	{
		return source == actor ||
		       source.IsTrustedAlly(actor) ||
		       source.IsHelpless ||
		       source.IsPrimaryRider(actor) && source.PermitControl(actor) ||
		       source.CanBeMountedBy(actor) && source.PermitControl(actor);
	}

	private static bool CharacterHitchItemAvailable(ICharacter actor, ICharacter source, ICharacter target,
		IGameItem hitchItem, out string reason)
	{
		if (hitchItem is null)
		{
			reason = string.Empty;
			return true;
		}

		if (hitchItem.Deleted || hitchItem.Destroyed)
		{
			reason = $"{hitchItem.HowSeen(actor, true)} is not usable.";
			return false;
		}

		var candidates = actor.Body.ExternalItems
		                      .Concat(source.Body.ExternalItemsForOtherActors)
		                      .Concat(target?.Body.ExternalItemsForOtherActors ?? Enumerable.Empty<IGameItem>())
		                      .Concat(actor.Location.LayerGameItems(actor.RoomLayer))
		                      .Where(x => actor.CanSee(x))
		                      .Distinct()
		                      .ToList();
		if (candidates.Contains(hitchItem))
		{
			if (HitchGearRules.IsReserved(hitchItem))
			{
				reason = $"{hitchItem.HowSeen(actor, true)} is already being used as hitch gear.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		reason = $"{hitchItem.HowSeen(actor, true)} is no longer available to use as a hitch item.";
		return false;
	}

	private static bool CharacterHitchItemIsOnEndpoint(ICharacter source, IPerceivable target, IGameItem hitchItem)
	{
		return source.Body.ExternalItems.Any(x => x.Id == hitchItem.Id) ||
		       (target as ICharacter)?.Body.ExternalItems.Any(x => x.Id == hitchItem.Id) == true;
	}

	private static bool PrepareCharacterHitchItem(ICharacter actor, ICharacter source, IPerceivable target,
		IGameItem hitchItem, out string reason)
	{
		if (hitchItem is null)
		{
			reason = string.Empty;
			return true;
		}

		if (HitchGearRules.IsReserved(hitchItem))
		{
			reason = $"{hitchItem.HowSeen(actor, true)} is already being used as hitch gear.";
			return false;
		}

		if (CharacterHitchItemIsOnEndpoint(source, target, hitchItem))
		{
			reason = string.Empty;
			return true;
		}

		if (hitchItem.Location == source.Location && hitchItem.RoomLayer == source.RoomLayer &&
		    hitchItem.ContainedIn is null && hitchItem.InInventoryOf is null)
		{
			reason = string.Empty;
			return true;
		}

		if (hitchItem.InInventoryOf == actor.Body)
		{
			if (!actor.Body.CanDrop(hitchItem, 0))
			{
				reason = actor.Body.WhyCannotDrop(hitchItem, 0);
				return false;
			}

			actor.Body.Drop(hitchItem, silent: true);
			if (hitchItem.Location != source.Location || hitchItem.RoomLayer != source.RoomLayer)
			{
				hitchItem.Location?.Extract(hitchItem);
				hitchItem.RoomLayer = source.RoomLayer;
				source.Location.Insert(hitchItem, true);
			}

			reason = string.Empty;
			return true;
		}

		reason = $"{hitchItem.HowSeen(actor, true)} must be with the hitch chain or worn or carried by one of the hitch endpoints.";
		return false;
	}

	private static void ApplyCharacterHitch(ICharacter actor, ICharacter source, IPerceivable target,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem hitchItem, IDragAid dragAid)
	{
		if (!PrepareCharacterHitchItem(actor, source, target, hitchItem, out var hitchItemReason))
		{
			actor.OutputHandler.Send(hitchItemReason);
			return;
		}

		long? persistentLinkId = null;
		if (HitchService.CanPersistCharacterHitch(source, target, out _))
		{
			var link = HitchService.CreatePersistentCharacterHitch(actor, source, target, targetVehicle, targetTowPoint,
				hitchItem, out var persistentReason);
			if (link is null)
			{
				actor.OutputHandler.Send(persistentReason);
				return;
			}

			persistentLinkId = link.Id;
		}

		var pullMultiplier = targetTowPoint?.CharacterPullMultiplier ?? 1.0;
		HitchGearRules.Reserve(hitchItem, vehicleHitchLinkId: persistentLinkId,
			sourceCharacterId: CharacterInstanceIdentityComparer.IdentityId(source),
			targetId: CharacterInstanceIdentityComparer.FrameworkItemId(target));
		source.AddEffect(new CharacterHitch(source, target, pullMultiplier, targetTowPoint?.Id, persistentLinkId,
			hitchItem?.Id));
		source.AddEffect(new Dragging(source, dragAid, target));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ hitch|hitches $1 to $2$?3| with $3||.", actor, actor,
			source, target, hitchItem)));
	}

	private static void OfferCharacterHitch(ICharacter actor, ICharacter source, IPerceivable target,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem hitchItem, IDragAid dragAid)
	{
		source.AddEffect(new Accept(source, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (!CharacterHitchItemAvailable(actor, source, target as ICharacter, hitchItem, out var itemReason))
				{
					actor.OutputHandler.Send(itemReason);
					source.OutputHandler.Send(itemReason);
					return;
				}

				var currentDragAid = HitchGearRules.DragAidFor(hitchItem) ?? dragAid;
				if (hitchItem is not null && currentDragAid is null)
				{
					var dragAidReason = $"{hitchItem.HowSeen(actor, true)} is no longer usable hitch gear.";
					actor.OutputHandler.Send(dragAidReason);
					source.OutputHandler.Send(dragAidReason);
					return;
				}

				if (!CanCharacterHitch(actor, source, target, targetVehicle, targetTowPoint, hitchItem, currentDragAid,
					    out var reason))
				{
					actor.OutputHandler.Send(reason);
					source.OutputHandler.Send(reason);
					return;
				}

				source.OutputHandler.Handle(new EmoteOutput(new Emote("@ accept|accepts $1's proposal to be hitched to $2$?3| with $3||.",
					source, source, actor, target, hitchItem)));
				ApplyCharacterHitch(actor, source, target, targetVehicle, targetTowPoint, hitchItem, currentDragAid);
			},
			RejectAction = text =>
			{
				source.OutputHandler.Handle(new EmoteOutput(new Emote("@ decline|declines $1's proposal to be hitched to $2.",
					source, source, actor, target)));
			},
			ExpireAction = () =>
			{
				source.OutputHandler.Handle(new EmoteOutput(new Emote("@ decline|declines $1's proposal to be hitched to $2.",
					source, source, actor, target)));
			},
			DescriptionString = $"proposal to hitch {source.HowSeen(source)} to {target.HowSeen(source)}",
			Keywords = new List<string> { "hitch", target.Name }
		}), TimeSpan.FromSeconds(60));

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ propose|proposes to hitch $1 to $2$?3| with $3||.",
			actor, actor, source, target, hitchItem)));
		source.OutputHandler.Send(Accept.StandardAcceptPhrasing);
	}

	private static bool CanCharacterHitch(ICharacter actor, ICharacter source, IPerceivable target,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem hitchItem, IDragAid dragAid,
		out string reason)
	{
		if (target is null)
		{
			reason = "That is not a valid hitch target.";
			return false;
		}

		if (source == target)
		{
			reason = "You cannot hitch someone to themselves.";
			return false;
		}

		if (source.Location != actor.Location || source.RoomLayer != actor.RoomLayer)
		{
			reason = $"{source.HowSeen(actor, true)} must be here to hitch them.";
			return false;
		}

		if (source.EffectsOfType<Dragging>().Any())
		{
			reason = $"{source.HowSeen(actor, true)} is already hitched to pull something.";
			return false;
		}

		if (HitchService.LinksFrom(actor.Gameworld, source)
		                .Any(x => x.SourceType == VehicleHitchEndpointType.Character))
		{
			reason = $"{source.HowSeen(actor, true)} already has a persistent hitch link.";
			return false;
		}

		if (source.Movement is not null)
		{
			reason = $"{source.HowSeen(actor, true)} is already moving.";
			return false;
		}

		if (source.Combat is not null && source.IsEngagedInMelee)
		{
			reason = $"{source.HowSeen(actor, true)} cannot be hitched while engaged in melee.";
			return false;
		}

		if (target.AffectedBy<Dragging.DragTarget>())
		{
			reason = $"{target.HowSeen(actor, true)} is already being pulled or dragged.";
			return false;
		}

		if (target is ICharacter targetCharacter)
		{
			if (HitchService.LinksInvolving(actor.Gameworld, targetCharacter)
			                .Any(x => x.TargetType == VehicleHitchEndpointType.Character))
			{
				reason = $"{targetCharacter.HowSeen(actor, true)} already has a persistent hitch link.";
				return false;
			}

			if (targetCharacter.Location != source.Location || targetCharacter.RoomLayer != source.RoomLayer)
			{
				reason = $"{targetCharacter.HowSeen(actor, true)} must be in the same location and layer.";
				return false;
			}

			var ally = targetCharacter.IsAlly(actor);
			if (CharacterState.Able.HasFlag(targetCharacter.State) && !ally && !targetCharacter.IsHelpless)
			{
				reason = "You cannot hitch someone who is conscious and unwilling.";
				return false;
			}

			if (targetCharacter.Effects.Any(x => x.Applies() && x.IsBlockingEffect("general")))
			{
				reason = $"You cannot hitch someone while they are {targetCharacter.Effects.First(x => x.Applies() && x.IsBlockingEffect("general")).BlockingDescription("general", actor)}.";
				return false;
			}

			if (targetCharacter.Combat is not null && targetCharacter.MeleeRange)
			{
				reason = $"{targetCharacter.HowSeen(actor, true)} cannot be hitched while in melee combat.";
				return false;
			}

			if (hitchItem is not null &&
			    !HitchGearRules.GearCompatible(hitchItem, targetCharacter.Weight, out reason))
			{
				return false;
			}

			return CanPullWeight(actor, source, targetCharacter, dragAid, 1.0, out reason);
		}

		if (targetVehicle is null || targetTowPoint is null)
		{
			reason = "That is not a valid hitch target.";
			return false;
		}

		return HitchGraphService.CanAddCharacterVehicleHitch(actor, source, targetVehicle, targetTowPoint, hitchItem,
			dragAid, out reason);
	}

	private static bool RequiredTowPointAccessAvailable(ICharacter actor, IVehicle vehicle,
		IVehicleTowPointPrototype towPoint, out string reason)
	{
		reason = string.Empty;
		var required = towPoint.RequiredAccessPoint;
		if (required is null)
		{
			return true;
		}

		var access = vehicle.AccessPoints.FirstOrDefault(x => x.Prototype.Id == required.Id);
		if (access is null)
		{
			reason = $"{towPoint.Name} requires an access point that is missing on {vehicle.Name}.";
			return false;
		}

		if (access.CanUse(actor, out reason))
		{
			return true;
		}

		reason = $"{towPoint.Name} is unavailable: {reason}";
		return false;
	}

	private static bool CanPullWeight(ICharacter actor, ICharacter source, IHaveWeight target, IDragAid dragAid,
		double pullMultiplier, out string reason)
	{
		pullMultiplier = Math.Max(1.0, pullMultiplier);
		var capacity = (source.MaximumDragWeight - source.Body.ExternalItems.Sum(x => x.Weight)) *
		               (dragAid?.EffortMultiplier ?? 1.0);
		var effectiveWeight = target.Weight / pullMultiplier;
		if (capacity >= effectiveWeight)
		{
			reason = string.Empty;
			return true;
		}

		var targetDescription = target is IPerceivable perceivable ? perceivable.HowSeen(actor) : "the target";
		reason = $"{source.HowSeen(actor, true)} can only pull {capacity.ToString("N2", actor)} effective weight, but {targetDescription} needs {effectiveWeight.ToString("N2", actor)}.";
		return false;
	}

	private static IVehicleOccupantSlotPrototype ResolveSlot(ICharacter actor, IVehicle vehicle, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		if (long.TryParse(text, out var id))
		{
			return vehicle.Prototype.OccupantSlots.FirstOrDefault(x => x.Id == id);
		}

		return vehicle.Prototype.OccupantSlots.FirstOrDefault(x => x.Name.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase)) ??
		       vehicle.Prototype.OccupantSlots.FirstOrDefault(x => x.SlotType.DescribeEnum().StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase));
	}

	private static (string SlotText, string AccessText) ParseEmbarkRemainder(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return (string.Empty, string.Empty);
		}

		if (text.StartsWith("via ", System.StringComparison.InvariantCultureIgnoreCase))
		{
			return (string.Empty, text[4..].Trim());
		}

		var viaIndex = text.IndexOf(" via ", System.StringComparison.InvariantCultureIgnoreCase);
		return viaIndex < 0
			? (text.Trim(), string.Empty)
			: (text[..viaIndex].Trim(), text[(viaIndex + 5)..].Trim());
	}

	private static IVehicleAccessPoint ResolveAccess(IVehicle vehicle, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		if (long.TryParse(text, out var id))
		{
			return vehicle.AccessPoints.FirstOrDefault(x => x.Id == id);
		}

		return vehicle.AccessPoints.FirstOrDefault(x => x.Name.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase)) ??
		       vehicle.AccessPoints.FirstOrDefault(x => x.Prototype.AccessPointType.DescribeEnum().StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase));
	}

	private static bool ResolveTowPoint(ICharacter actor, string text, bool source, out IVehicle vehicle,
		out IVehicleTowPointPrototype towPoint)
	{
		vehicle = null;
		towPoint = null;
		var split = text.Split('@', 2);
		if (split.Length != 2)
		{
			actor.OutputHandler.Send("Tow points must be specified as #3<towpoint>@<vehicle>#0.".SubstituteANSIColour());
			return false;
		}

		var item = actor.TargetLocalItem(split[1]);
		vehicle = item?.GetItemType<IVehicleExterior>()?.Vehicle;
		if (vehicle is null)
		{
			actor.OutputHandler.Send("You do not see any linked vehicle like that here.");
			return false;
		}

		towPoint = ResolveTowPoint(vehicle, split[0], source);
		if (towPoint is null)
		{
			actor.OutputHandler.Send(source
				? "There is no towing-capable tow point like that on that vehicle."
				: "There is no towable tow point like that on that vehicle.");
			return false;
		}

		if (vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, towPoint.Id))
		{
			actor.OutputHandler.Send($"{towPoint.Name.ColourName()} is disabled because {vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, towPoint.Id)}.");
			return false;
		}

		return true;
	}

	private static bool ResolveTowPoint(ICharacter actor, string text, out IVehicle vehicle,
		out IVehicleTowPointPrototype towPoint)
	{
		vehicle = null;
		towPoint = null;
		var split = text.Split('@', 2);
		if (split.Length != 2)
		{
			actor.OutputHandler.Send("Tow points must be specified as #3<towpoint>@<vehicle>#0.".SubstituteANSIColour());
			return false;
		}

		var item = actor.TargetLocalItem(split[1]);
		vehicle = item?.GetItemType<IVehicleExterior>()?.Vehicle;
		if (vehicle is null)
		{
			actor.OutputHandler.Send("You do not see any linked vehicle like that here.");
			return false;
		}

		towPoint = ResolveTowPoint(vehicle, split[0]);
		if (towPoint is null)
		{
			actor.OutputHandler.Send("There is no tow point like that on that vehicle.");
			return false;
		}

		return true;
	}

	private static IVehicleTowPointPrototype ResolveTowPoint(IVehicle vehicle, string text, bool source)
	{
		var candidates = vehicle.Prototype.TowPoints
		                        .Where(x => source ? x.CanTow : x.CanBeTowed)
		                        .ToList();
		if (long.TryParse(text, out var id))
		{
			return candidates.FirstOrDefault(x => x.Id == id);
		}

		return candidates.FirstOrDefault(x => x.Name.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase)) ??
		       candidates.FirstOrDefault(x => x.TowType.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase));
	}

	private static IVehicleTowPointPrototype ResolveTowPoint(IVehicle vehicle, string text)
	{
		var candidates = vehicle.Prototype.TowPoints.ToList();
		if (long.TryParse(text, out var id))
		{
			return candidates.FirstOrDefault(x => x.Id == id);
		}

		return candidates.FirstOrDefault(x => x.Name.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase)) ??
		       candidates.FirstOrDefault(x => x.TowType.StartsWith(text, System.StringComparison.InvariantCultureIgnoreCase));
	}

	private const string VehicleAdminHelp = @"The #3vehicle#0 command manages live vehicle instances.

Syntax:
	#3vehicle list#0
	#3vehicle show <id|name>#0
	#3vehicle repair <id|name>#0
	#3vehicle repair <id|name> damage <zone|all>#0
	#3vehicle relink <id|name> <item id|local item>#0";

	[PlayerCommand("Vehicle", "vehicle")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("vehicle", VehicleAdminHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void VehicleAdmin(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "":
			case "list":
				VehicleList(actor);
				return;
			case "show":
			case "view":
				VehicleShow(actor, ss);
				return;
			case "repair":
			case "diagnose":
				VehicleRepair(actor, ss);
				return;
			case "relink":
				VehicleRelink(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(VehicleAdminHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleList(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Vehicles:");
		foreach (var vehicle in actor.Gameworld.Vehicles.OrderBy(x => x.Id))
		{
			sb.AppendLine($"\t#{vehicle.Id.ToString("N0", actor)} {vehicle.Name.ColourName()} [{vehicle.Prototype.Scale.DescribeEnum().ColourValue()}] at {(vehicle.Location is null ? "nowhere".ColourError() : vehicle.Location.HowSeen(actor))}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void VehicleShow(ICharacter actor, StringStack ss)
	{
		var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(ss.SafeRemainingArgument);
		if (vehicle is null)
		{
			actor.OutputHandler.Send("There is no such vehicle.");
			return;
		}

		actor.OutputHandler.Send(ShowVehicle(actor, vehicle));
	}

	private static string ShowVehicle(ICharacter actor, IVehicle vehicle)
	{
		var exterior = vehicle.ExteriorItem?.GetItemType<IVehicleExterior>();
		var movement = vehicle.MovementState;
		var sb = new StringBuilder();
		sb.AppendLine($"Vehicle #{vehicle.Id.ToString("N0", actor)} - {vehicle.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Prototype: {vehicle.Prototype.Name.ColourName()} (#{vehicle.Prototype.Id.ToString("N0", actor)}r{vehicle.Prototype.RevisionNumber.ToString("N0", actor)})");
		sb.AppendLine($"Scale: {vehicle.Prototype.Scale.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Canonical Location: {movement.LocationType.DescribeEnum().ColourValue()} {(movement.Location is null ? "nowhere".ColourError() : movement.Location.HowSeen(actor))} [{movement.RoomLayer.DescribeEnum().ColourValue()}]");
		sb.AppendLine($"Movement: {movement.MovementStatus.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Transit: Exit #{movement.CurrentExitId?.ToString("N0", actor) ?? "none"}, Destination #{movement.DestinationCellId?.ToString("N0", actor) ?? "none"}");
		sb.AppendLine();
		sb.AppendLine("Item Projection:");
		sb.AppendLine($"\tExterior Item Id: {vehicle.ExteriorItemId?.ToString("N0", actor) ?? "None".ColourError()}");
		sb.AppendLine($"\tLoaded Item: {(vehicle.ExteriorItem is null ? "None".ColourError() : vehicle.ExteriorItem.HowSeen(actor))}");
		sb.AppendLine($"\tComponent Link: {(exterior is null ? "Missing".ColourError() : exterior.VehicleId?.ToString("N0", actor) ?? "Unlinked".ColourError())}");
		sb.AppendLine($"\tProjection In Sync: {(vehicle.ExteriorItem is not null && vehicle.ExteriorItem.Location == movement.Location && vehicle.ExteriorItem.RoomLayer == movement.RoomLayer).ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Occupants:");
		sb.AppendLine(vehicle.Occupancies.Any()
			? vehicle.Occupancies.Select(x => $"\t{x.Occupant?.RenderStaffActorReference() ?? "missing".ColourError()} in {x.Slot.Name.ColourName()}{(x.IsController ? " controlling".Colour(Telnet.Green) : "")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Access Points:");
		sb.AppendLine(vehicle.AccessPoints.Any()
			? vehicle.AccessPoints.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} [{x.Prototype.AccessPointType.DescribeEnum().ColourValue()}] {(x.IsOpen ? "open".Colour(Telnet.Green) : "closed".Colour(Telnet.Yellow))}{(x.IsLocked ? " locked".Colour(Telnet.Red) : "")}{DisabledCause(actor, x.IsManuallyDisabled, vehicle, VehicleDamageEffectTargetType.AccessPoint, x.Prototype.Id)} projection {x.ProjectionItemId?.ToString("N0", actor) ?? "none".ColourError()}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Cargo Spaces:");
		sb.AppendLine(vehicle.CargoSpaces.Any()
			? vehicle.CargoSpaces.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()}{DisabledCause(actor, x.IsManuallyDisabled, vehicle, VehicleDamageEffectTargetType.CargoSpace, x.Prototype.Id)} projection {x.ProjectionItemId?.ToString("N0", actor) ?? "none".ColourError()}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Installations:");
		sb.AppendLine(vehicle.Installations.Any()
			? vehicle.Installations.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Prototype.Name.ColourName()} [{x.Prototype.MountType.ColourCommand()}]{DisabledCause(actor, x.IsManuallyDisabled, vehicle, VehicleDamageEffectTargetType.InstallationPoint, x.Prototype.Id)} item {(x.InstalledItem is null ? "none".ColourError() : x.InstalledItem.HowSeen(actor))}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Tow Links:");
		sb.AppendLine(vehicle.TowLinks.Any()
			? vehicle.TowLinks.Select(x => DescribeTowLink(actor, x)).ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		var hitchLinks = (actor.Gameworld.VehicleHitchLinks ?? Enumerable.Empty<IVehicleHitchLink>())
		                                  .Where(x => x.SourceVehicleId == vehicle.Id ||
		                                              x.TargetVehicleId == vehicle.Id)
		                                  .OrderBy(x => x.Id)
		                                  .ToList();
		sb.AppendLine("Mixed Hitch Links:");
		sb.AppendLine(hitchLinks.Any()
			? hitchLinks.Select(x => DescribeHitchLink(actor, x)).ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		sb.AppendLine();
		sb.AppendLine("Damage Zones:");
		sb.AppendLine(vehicle.DamageZones.Any()
			? vehicle.DamageZones.Select(x => $"\t#{x.Id.ToString("N0", actor)} {x.Name.ColourName()} {x.Status.DescribeEnum().ColourValue()} damage {x.CurrentDamage.ToString("N2", actor).ColourValue()}/{x.Prototype.MaximumDamage.ToString("N2", actor).ColourValue()} wounds {x.Wounds.Count().ToString("N0", actor).ColourValue()}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
			: "\tNone");
		return sb.ToString();
	}

	private static string DisabledCause(ICharacter actor, bool manualDisabled, IVehicle vehicle,
		VehicleDamageEffectTargetType targetType, long? targetPrototypeId)
	{
		var damage = vehicle.DamageZonesDisabling(targetType, targetPrototypeId).ToList();
		if (!manualDisabled && !damage.Any())
		{
			return string.Empty;
		}

		var causes = new List<string>();
		if (manualDisabled)
		{
			causes.Add("manual");
		}

		if (damage.Any())
		{
			causes.Add($"damage {damage.Select(x => $"{x.Name} {x.Status.DescribeEnum()}").ListToString()}");
		}

		return $" disabled ({causes.ListToString()})".Colour(Telnet.Red);
	}

	private static string DescribeTowLink(ICharacter actor, IVehicleTowLink link)
	{
		var source = link.SourceVehicle;
		var target = link.TargetVehicle;
		var sourcePoint = link.SourceTowPoint;
		var targetPoint = link.TargetTowPoint;
		var hitchItem = link.HitchItem;
		return
			$"\t#{link.Id.ToString("N0", actor)} {source?.Name.ColourName() ?? "missing".ColourError()}:{sourcePoint?.Name.ColourName() ?? "missing".ColourError()} -> {target?.Name.ColourName() ?? "missing".ColourError()}:{targetPoint?.Name.ColourName() ?? "missing".ColourError()} hitch {(hitchItem is null ? "none".ColourError() : hitchItem.HowSeen(actor))}{TowLinkDisabledCause(actor, link)}";
	}

	private static string TowLinkDisabledCause(ICharacter actor, IVehicleTowLink link)
	{
		return TowService.ValidateLink(link, out var reason)
			? string.Empty
			: $" invalid ({reason})".Colour(Telnet.Red);
	}

	private static string DescribeHitchLink(ICharacter actor, IVehicleHitchLink link)
	{
		var hitchItem = link.HitchItem;
		return
			$"\t#{link.Id.ToString("N0", actor)} {DescribeHitchEndpoint(actor, link.SourceType, link.SourceVehicle, link.SourceCharacter, link.SourceTowPoint)} -> {DescribeHitchEndpoint(actor, link.TargetType, link.TargetVehicle, link.TargetCharacter, link.TargetTowPoint)} hitch {(hitchItem is null ? "none".ColourError() : hitchItem.HowSeen(actor))}{HitchLinkDisabledCause(actor, link)}";
	}

	private static string DescribeHitchEndpoint(ICharacter actor, VehicleHitchEndpointType type, IVehicle vehicle,
		ICharacter character, IVehicleTowPointPrototype towPoint)
	{
		return type switch
		{
			VehicleHitchEndpointType.Vehicle =>
				$"{vehicle?.Name.ColourName() ?? "missing".ColourError()}:{towPoint?.Name.ColourName() ?? "missing".ColourError()}",
			VehicleHitchEndpointType.Character => character?.RenderStaffActorReference() ?? "missing".ColourError(),
			_ => "invalid".ColourError()
		};
	}

	private static string HitchLinkDisabledCause(ICharacter actor, IVehicleHitchLink link)
	{
		var perceivable = (IPerceivable)link.SourceVehicle?.ExteriorItem ??
		                  link.SourceCharacter ??
		                  (IPerceivable)link.TargetVehicle?.ExteriorItem ??
		                  link.TargetCharacter;
		var graphLink = perceivable is null
			? null
			: HitchGraphService.LinksInvolving(actor.Gameworld, perceivable)
			                  .FirstOrDefault(x => x.Kind == VehicleHitchGraphLinkKind.PersistentHitch &&
			                                       x.WrappedLink?.Id == link.Id);
		if (graphLink is not null && !HitchGraphService.ValidateLink(graphLink, out var graphReason))
		{
			return $" invalid ({graphReason})".Colour(Telnet.Red);
		}

		if (!link.IsDisabled)
		{
			return string.Empty;
		}

		if (!string.IsNullOrWhiteSpace(link.WhyInvalid))
		{
			return $" invalid ({link.WhyInvalid})".Colour(Telnet.Red);
		}

		return link.IsManuallyDisabled ? " disabled (manual)".Colour(Telnet.Red) : string.Empty;
	}

	private static void VehicleRepair(ICharacter actor, StringStack ss)
	{
		var text = ss.SafeRemainingArgument;
		var damageIndex = text.IndexOf(" damage ", StringComparison.InvariantCultureIgnoreCase);
		if (damageIndex > 0)
		{
			var vehicleText = text[..damageIndex].Trim();
			var zoneText = text[(damageIndex + 8)..].Trim();
			var damageVehicle = actor.Gameworld.Vehicles.GetByIdOrName(vehicleText);
			if (damageVehicle is null)
			{
				actor.OutputHandler.Send("There is no such vehicle.");
				return;
			}

			VehicleRepairDamage(actor, damageVehicle, zoneText);
			return;
		}

		var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(text);
		if (vehicle is null)
		{
			actor.OutputHandler.Send("There is no such vehicle.");
			return;
		}

		if (vehicle.ExteriorItem is null)
		{
			actor.OutputHandler.Send("That vehicle has no exterior item to repair. Use #3vehicle relink <vehicle> <item>#0.".SubstituteANSIColour());
			return;
		}

		var exterior = vehicle.ExteriorItem.GetItemType<IVehicleExterior>();
		if (exterior is null)
		{
			actor.OutputHandler.Send("That vehicle's exterior item does not have the vehicle exterior component.");
			return;
		}

		vehicle.LinkExteriorItem(vehicle.ExteriorItem);
		exterior.LinkVehicle(vehicle);
		vehicle.SynchroniseExteriorItemToLocation();
		actor.Gameworld.SaveManager.Flush();
		actor.OutputHandler.Send($"You repair the vehicle/item projection link for {vehicle.Name.ColourName()}.");
	}

	private static void VehicleRepairDamage(ICharacter actor, IVehicle vehicle, string zoneText)
	{
		if (string.IsNullOrWhiteSpace(zoneText))
		{
			actor.OutputHandler.Send("Which damage zone do you want to repair?");
			return;
		}

		var zones = zoneText.EqualTo("all")
			? vehicle.DamageZones.ToList()
			: vehicle.DamageZones.Where(x =>
				(long.TryParse(zoneText, out var id) && x.Id == id) ||
				x.Name.StartsWith(zoneText, StringComparison.InvariantCultureIgnoreCase)).ToList();

		if (!zones.Any())
		{
			actor.OutputHandler.Send("There is no such damage zone on that vehicle.");
			return;
		}

		foreach (var zone in zones)
		{
			zone.ClearWoundsAndDamage();
		}

		actor.Gameworld.SaveManager.Flush();
		actor.OutputHandler.Send(zoneText.EqualTo("all")
			? $"You repair all damage zones on {vehicle.Name.ColourName()}."
			: $"You repair {zones.Select(x => x.Name.ColourName()).ListToString()} on {vehicle.Name.ColourName()}.");
	}

	private static void VehicleRelink(ICharacter actor, StringStack ss)
	{
		var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(ss.PopSpeech());
		if (vehicle is null)
		{
			actor.OutputHandler.Send("There is no such vehicle.");
			return;
		}

		var itemText = ss.SafeRemainingArgument;
		var item = long.TryParse(itemText, out var itemId)
			? actor.Gameworld.TryGetItem(itemId, true)
			: actor.TargetLocalItem(itemText);
		if (item is null)
		{
			actor.OutputHandler.Send("There is no such item to relink.");
			return;
		}

		var exterior = item.GetItemType<IVehicleExterior>();
		if (exterior is null)
		{
			actor.OutputHandler.Send("That item does not have the vehicle exterior component.");
			return;
		}

		vehicle.LinkExteriorItem(item);
		exterior.LinkVehicle(vehicle);
		vehicle.SynchroniseExteriorItemToLocation();
		actor.Gameworld.SaveManager.Flush();
		actor.OutputHandler.Send($"You relink {vehicle.Name.ColourName()} to {item.HowSeen(actor)}.");
	}

	private const string VehicleProtoHelp = @"The #3vehicleproto#0 command manages vehicle prototypes.

Syntax:
	#3vehicleproto list#0
	#3vehicleproto show <id|name>#0
	#3vehicleproto new <name>#0
	#3vehicleproto edit <id|name>#0
	#3vehicleproto close#0
	#3vehicleproto submit <comment>#0
	#3vehicleproto approve <id> <comment>#0
	#3vehicleproto set <building command>#0
	#3vehicleproto create <id|name>#0";

	[PlayerCommand("VehicleProto", "vehicleproto", "vproto")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("vehicleproto", VehicleProtoHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void VehicleProto(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "":
			case "list":
				VehicleProtoList(actor);
				return;
			case "show":
				VehicleProtoShow(actor, ss);
				return;
			case "new":
				VehicleProtoNew(actor, ss);
				return;
			case "edit":
				VehicleProtoEdit(actor, ss);
				return;
			case "close":
				actor.SetEditingItem<IVehiclePrototype>(null);
				actor.OutputHandler.Send("You stop editing vehicle prototypes.");
				return;
			case "submit":
				VehicleProtoSubmit(actor, ss);
				return;
			case "approve":
				VehicleProtoApprove(actor, ss);
				return;
			case "set":
				VehicleProtoSet(actor, ss);
				return;
			case "create":
				VehicleProtoCreate(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(VehicleProtoHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleProtoList(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Vehicle Prototypes:");
		foreach (var proto in actor.Gameworld.VehiclePrototypes.OrderBy(x => x.Id).ThenByDescending(x => x.RevisionNumber))
		{
			sb.AppendLine($"\t#{proto.Id.ToString("N0", actor)}r{proto.RevisionNumber.ToString("N0", actor)} {proto.Name.ColourName()} [{proto.Scale.DescribeEnum().ColourValue()}] {proto.Status.DescribeColour()}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void VehicleProtoShow(ICharacter actor, StringStack ss)
	{
		var proto = actor.Gameworld.VehiclePrototypes.GetByIdOrName(ss.SafeRemainingArgument);
		actor.OutputHandler.Send(proto?.Show(actor) ?? "There is no such vehicle prototype.");
	}

	private static void VehicleProtoNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give the new vehicle prototype?");
			return;
		}

		var proto = new VehiclePrototype(actor.Gameworld, actor.Account, ss.SafeRemainingArgument);
		actor.Gameworld.Add(proto);
		actor.SetEditingItem<IVehiclePrototype>(proto);
		actor.OutputHandler.Send($"You create and begin editing {proto.EditHeader()}.");
	}

	private static void VehicleProtoEdit(ICharacter actor, StringStack ss)
	{
		var proto = actor.Gameworld.VehiclePrototypes.GetByIdOrName(ss.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such vehicle prototype.");
			return;
		}

		if (proto.Status == RevisionStatus.Current)
		{
			proto = (IVehiclePrototype)proto.CreateNewRevision(actor);
			actor.Gameworld.Add(proto);
		}

		actor.SetEditingItem(proto);
		actor.OutputHandler.Send($"You are now editing {proto.EditHeader()}.");
	}

	private static void VehicleProtoSubmit(ICharacter actor, StringStack ss)
	{
		var proto = actor.EditingItem<IVehiclePrototype>();
		if (proto is null)
		{
			actor.OutputHandler.Send("You are not editing a vehicle prototype.");
			return;
		}

		if (!proto.CanSubmit())
		{
			actor.OutputHandler.Send(proto.WhyCannotSubmit());
			return;
		}

		proto.ChangeStatus(RevisionStatus.PendingRevision, ss.SafeRemainingArgument, actor.Account);
		actor.SetEditingItem<IVehiclePrototype>(null);
		actor.OutputHandler.Send($"You submit {proto.EditHeader()} for review.");
	}

	private static void VehicleProtoApprove(ICharacter actor, StringStack ss)
	{
		var proto = actor.Gameworld.VehiclePrototypes.GetByIdOrName(ss.PopSpeech());
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such vehicle prototype.");
			return;
		}

		if (!proto.CanSubmit())
		{
			actor.OutputHandler.Send(proto.WhyCannotSubmit());
			return;
		}

		foreach (var old in actor.Gameworld.VehiclePrototypes.GetAll(proto.Id).Where(x => x.Status == RevisionStatus.Current))
		{
			old.ChangeStatus(RevisionStatus.Revised, "Revised by newer vehicle prototype.", actor.Account);
		}

		proto.ChangeStatus(RevisionStatus.Current, ss.SafeRemainingArgument, actor.Account);
		actor.OutputHandler.Send($"You approve {proto.EditHeader()}.");
	}

	private static void VehicleProtoSet(ICharacter actor, StringStack ss)
	{
		var proto = actor.EditingItem<IVehiclePrototype>();
		if (proto is null)
		{
			actor.OutputHandler.Send("You are not editing a vehicle prototype.");
			return;
		}

		proto.BuildingCommand(actor, ss);
	}

	private static void VehicleProtoCreate(ICharacter actor, StringStack ss)
	{
		var proto = actor.Gameworld.VehiclePrototypes.GetByIdOrName(ss.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such vehicle prototype.");
			return;
		}

		if (!proto.CanCreateVehicle(out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		var vehicle = VehicleFactory.CreateVehicle(proto, actor.Location, actor.RoomLayer, actor);
		actor.OutputHandler.Send($"You create {vehicle.ExteriorItem.HowSeen(actor)} as vehicle {vehicle.Name.ColourName()} (#{vehicle.Id.ToString("N0", actor)}).");
	}
}
