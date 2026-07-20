using MudSharp.Accounts;
using MudSharp.Combat.Moves;
using MudSharp.Commands.Trees;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Vehicles;
using DB = MudSharp.Models;

namespace MudSharp.Commands.Modules;

internal partial class VehicleModule : Module<ICharacter>
{
	private VehicleModule() : base("Vehicles")
	{
		IsNecessary = true;
	}

	public static VehicleModule Instance { get; } = new();
	private static readonly IVehicleTowService TowService = new VehicleTowService();
	private static readonly IVehicleHitchService HitchService = new VehicleHitchService();
	private static readonly IVehicleHitchGraphService HitchGraphService = new VehicleHitchGraphService();
	private static readonly IVehicleOperationalReadinessService OperationalReadinessService = new VehicleOperationalReadinessService(HitchGraphService);
	private static readonly IVehiclePropulsionService PropulsionService = new VehiclePropulsionService();
	private static readonly IVehicleFleetOperationsService FleetOperationsService = new VehicleFleetOperationsService(OperationalReadinessService, HitchGraphService);

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

		if (actor.Combat is not null)
		{
			if (!actor.CanSpendStamina(BoardVehicleCombatMove.BoardingStaminaCost))
			{
				actor.OutputHandler.Send("You are too exhausted to board a vehicle in combat.");
				return;
			}

			if (actor.TakeOrQueueCombatAction(
				    SelectedCombatAction.GetEffectBoardVehicle(actor, exterior.Vehicle, slot, access)) &&
			    actor.Gameworld.GetStaticBool("EchoQueuedActions"))
			{
				actor.OutputHandler.Send(
					$"{"[Queued Action]: ".ColourBold(Telnet.Yellow)}Boarding {item.HowSeen(actor)}.");
			}

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
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("drive", "Use #3drive <direction>#0 while in a driver slot to move your vehicle through a normal exit. Ordinary movement commands like #3north#0 and #3east#0 do the same thing while you are controlling a vehicle.", AutoHelp.HelpArgOrNoArg)]
	protected static void Drive(ICharacter actor, string input)
	{
		VehicleMovementCommand.TryMoveControlledVehicle(actor, input.RemoveFirstWord(), true);
	}

	private const string VehiclePropulsionHelp = @"Use #3vehiclepropulsion#0 while aboard a vehicle to see its selected and supported propulsion modes.

Use #3vehiclepropulsion <selfpowered|rowed|sail|outboard|none>#0 while controlling a stationary vehicle to select a mode. Movement never silently falls back to another authored mode.";

	[PlayerCommand("VehiclePropulsion", "vehiclepropulsion")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoMovementCommand]
	[HelpInfo("vehiclepropulsion", VehiclePropulsionHelp, AutoHelp.HelpArg)]
	protected static void VehiclePropulsion(ICharacter actor, string input)
	{
		var vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x => x.IsOccupant(actor));
		if (vehicle is null)
		{
			actor.OutputHandler.Send("You are not aboard a vehicle.");
			return;
		}

		if (vehicle.MovementProfile?.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater)
		{
			actor.OutputHandler.Send("That vehicle does not use surface-water propulsion modes.");
			return;
		}

		var modes = vehicle.MovementProfile.PropulsionProfiles.ToList();
		var ss = new StringStack(input.RemoveFirstWord());
		if (!ss.IsFinished)
		{
			if (vehicle.Controller?.SamePhysicalInstance(actor) != true)
			{
				actor.OutputHandler.Send("You must be aboard and in control of the vehicle to change its propulsion mode.");
				return;
			}

			var type = MudSharp.Vehicles.VehiclePrototype.ParseVehiclePropulsionType(ss.SafeRemainingArgument);
			if (type is null)
			{
				actor.OutputHandler.Send("You must specify selfpowered, rowed, sail, outboard or none.");
				return;
			}

			var profile = modes.FirstOrDefault(x => x.PropulsionType == type.Value);
			if (profile is null)
			{
				actor.OutputHandler.Send("That vehicle does not support that propulsion mode.");
				return;
			}

			if (!vehicle.SetActivePropulsionProfile(profile, out var reason))
			{
				actor.OutputHandler.Send(reason);
				return;
			}

			actor.OutputHandler.Send($"You select {profile.PropulsionType.DescribeEnum().ColourName()} propulsion for {vehicle.Name.ColourName()}.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Propulsion for {vehicle.Name.ColourName()}:");
		if (!modes.Any())
		{
			sb.AppendLine("\tLegacy surface-water movement (no explicit propulsion profiles).".Colour(Telnet.Yellow));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		sb.AppendLine($"Selected: {(vehicle.ActivePropulsionProfile?.PropulsionType.DescribeEnum().ColourName() ?? "none".ColourError())}");
		sb.AppendLine($"Supported: {modes.Select(x => $"{x.PropulsionType.DescribeEnum().ColourName()}{(x.IsDefault ? " (default)".Colour(Telnet.Green) : "")}").ListToString()}");
		var readiness = PropulsionService.BuildReadiness(vehicle, actor, null);
		if (readiness.Profile?.PropulsionType == VehiclePropulsionType.Sail)
		{
			sb.AppendLine($"Wind: {readiness.Wind.Describe().ColourName()}");
		}

		if (readiness.Contributors.Any())
		{
			sb.AppendLine($"Contributors: {readiness.Contributors.Select(x => $"{x.Character.HowSeen(actor)}{(x.OarItem is null ? "" : $" with {x.OarItem.HowSeen(actor)}")}").ListToString()}");
		}

		if (readiness.Motors.Any())
		{
			sb.AppendLine("Motors:");
			foreach (var motor in readiness.Motors)
			{
				sb.AppendLine($"\t{motor.Item?.HowSeen(actor) ?? motor.Installation.Prototype.Name}: {(motor.Available ? "ready".Colour(Telnet.Green) : motor.Reason.ColourError())}");
			}
		}

		sb.AppendLine(readiness.CanMove
			? "Status: ready to provide propulsion.".Colour(Telnet.Green)
			: $"Blocker: {readiness.Reason.ColourError()}");
		actor.OutputHandler.Send(sb.ToString());
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

		if (!CanUseVehicleForAction(actor, sourceVehicle, VehicleOperationalAction.Hitch, out var accessReason) ||
		    !CanUseVehicleForAction(actor, targetVehicle, VehicleOperationalAction.Hitch, out accessReason))
		{
			actor.OutputHandler.Send(accessReason);
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

			if (!CanUseVehicleForAction(actor, targetVehicle, VehicleOperationalAction.Hitch, out var accessReason))
			{
				actor.OutputHandler.Send(accessReason);
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

		foreach (var linkedVehicle in links.SelectMany(x => new[] { x.SourceVehicle, x.TargetVehicle }).Where(x => x is not null).Cast<IVehicle>().DistinctBy(x => x.Id))
		{
			if (!CanUseVehicleForAction(actor, linkedVehicle, VehicleOperationalAction.Hitch, out var accessReason))
			{
				actor.OutputHandler.Send(accessReason);
				return;
			}
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
		if (target is IGameItem item && item.GetItemType<IVehicleExterior>()?.Vehicle is { } vehicle &&
		    !CanUseVehicleForAction(actor, vehicle, VehicleOperationalAction.Hitch, out var accessReason))
		{
			actor.OutputHandler.Send(accessReason);
			return true;
		}

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

	private static bool CanUseVehicleForAction(ICharacter actor, IVehicle vehicle, VehicleOperationalAction action, out string reason)
	{
		if (vehicle is null)
		{
			reason = "There is no such vehicle.";
			return false;
		}

		if (OperationalReadinessService.CanPerformAction(vehicle, actor, action, out var result))
		{
			reason = string.Empty;
			return true;
		}

		reason = result.Reason;
		return false;
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
	#3vehicle repair <id|name> hitch <link|all>#0
	#3vehicle access <id|name> list#0
	#3vehicle access <id|name> grant <character> <board|control|service|repair|hitch|all> <1-3>#0
	#3vehicle access <id|name> revoke <character|row id> [tag]#0
	#3vehicle access <id|name> apply <preset> <character>#0
	#3vehicle access <id|name> clone <source vehicle>#0
	#3vehicle access preset list|show <name>#0
	#3vehicle access preset set <name> <board|control|service|repair|hitch|all> <1-3>#0
	#3vehicle access preset remove <name> <tag>#0
	#3vehicle access preset delete <name>#0
	#3vehicle access preset reset <name|all>#0
	#3vehicle audit <here|zone|prototype <id|name>|all> [readiness|access|resources|hitch|damage|recovery|all]#0
	#3vehicle recover <id|name> [projection|install|hitch|all] [fix]#0
	#3vehicle fleet <here|zone|prototype <id|name>|all> access apply <preset> <character>#0
	#3vehicle fleet <scope> access grant <character> <tag> <1-3>#0
	#3vehicle fleet <scope> access revoke <character|row id> [tag]#0
	#3vehicle fleet <scope> access clone <source vehicle>#0
	#3vehicle fleet <scope> recover <projection|install|hitch|all> [fix]#0
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
			case "access":
				VehicleAccess(actor, ss);
				return;
			case "audit":
				VehicleAudit(actor, ss);
				return;
			case "recover":
				VehicleRecover(actor, ss);
				return;
			case "fleet":
				VehicleFleet(actor, ss);
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

	private static void VehicleAccess(ICharacter actor, StringStack ss)
	{
		if (ss.PeekSpeech().EqualTo("preset"))
		{
			ss.PopSpeech();
			VehicleAccessPreset(actor, ss);
			return;
		}

		var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(ss.PopSpeech());
		if (vehicle is null)
		{
			actor.OutputHandler.Send("There is no such vehicle.");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "":
			case "list":
				VehicleAccessList(actor, vehicle);
				return;
			case "grant":
				VehicleAccessGrant(actor, vehicle, ss);
				return;
			case "revoke":
				VehicleAccessRevoke(actor, vehicle, ss);
				return;
			case "apply":
				VehicleAccessApply(actor, vehicle, ss);
				return;
			case "clone":
				VehicleAccessClone(actor, vehicle, ss);
				return;
			default:
				actor.OutputHandler.Send("Use #3vehicle access <vehicle> list|grant|revoke|apply|clone#0 or #3vehicle access preset <action>#0.".SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleAccessList(ICharacter actor, IVehicle vehicle)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Access grants for {vehicle.Name.ColourName()}:");
		if (!vehicle.AccessStates.Any())
		{
			sb.AppendLine("\tNone - default vehicle access is permissive.");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		foreach (var access in vehicle.AccessStates.OrderBy(x => x.Id))
		{
			sb.AppendLine($"\t#{access.Id.ToString("N0", actor)} {access.Character?.RenderStaffActorReference() ?? "missing character".ColourError()} tag {access.AccessTag.ColourCommand()} level {access.AccessLevel.ToString("N0", actor).ColourValue()}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void VehicleAccessGrant(ICharacter actor, IVehicle vehicle, StringStack ss)
	{
		var target = ResolveAccessCharacter(actor, ss.PopSpeech());
		if (target is null)
		{
			actor.OutputHandler.Send("There is no such character.");
			return;
		}

		var tag = ss.PopSpeech().ToLowerInvariant();
		if (!ValidVehicleAccessTag(tag))
		{
			actor.OutputHandler.Send("Access tags must be one of board, control, service, repair, hitch, or all.");
			return;
		}

		if (!int.TryParse(ss.PopSpeech(), out var level) || level < 1 || level > 3)
		{
			actor.OutputHandler.Send("Access level must be 1, 2, or 3.");
			return;
		}

		var row = vehicle.GrantAccess(target, tag, level);
		actor.OutputHandler.Send($"You grant {target.RenderStaffActorReference()} {tag.ColourCommand()} access level {level.ToString("N0", actor).ColourValue()} on {vehicle.Name.ColourName()} (row #{row.Id.ToString("N0", actor)}).");
	}

	private static void VehicleAccessRevoke(ICharacter actor, IVehicle vehicle, StringStack ss)
	{
		var targetText = ss.PopSpeech();
		if (string.IsNullOrWhiteSpace(targetText))
		{
			actor.OutputHandler.Send("Which access row or character do you want to revoke?");
			return;
		}

		if (long.TryParse(targetText, out var rowId))
		{
			actor.OutputHandler.Send(vehicle.RevokeAccess(rowId)
				? $"You revoke vehicle access row #{rowId.ToString("N0", actor)} from {vehicle.Name.ColourName()}."
				: "There is no such access row on that vehicle.");
			return;
		}

		var target = ResolveAccessCharacter(actor, targetText);
		if (target is null)
		{
			actor.OutputHandler.Send("There is no such character or access row.");
			return;
		}

		var tag = ss.SafeRemainingArgument;
		if (!string.IsNullOrWhiteSpace(tag) && !ValidVehicleAccessTag(tag))
		{
			actor.OutputHandler.Send("Access tags must be one of board, control, service, repair, hitch, or all.");
			return;
		}

		var count = vehicle.RevokeAccess(target, tag);
		actor.OutputHandler.Send(count > 0
			? $"You revoke {count.ToString("N0", actor).ColourValue()} access grant{(count == 1 ? "" : "s")} for {target.RenderStaffActorReference()} on {vehicle.Name.ColourName()}."
			: "There were no matching access grants to revoke.");
	}

	private static void VehicleAccessPreset(ICharacter actor, StringStack ss)
	{
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "":
			case "list":
				VehicleAccessPresetList(actor);
				return;
			case "show":
				VehicleAccessPresetShow(actor, ss.SafeRemainingArgument);
				return;
			case "set":
				VehicleAccessPresetSet(actor, ss);
				return;
			case "remove":
			case "deletegrant":
				VehicleAccessPresetRemove(actor, ss);
				return;
			case "delete":
				VehicleAccessPresetDelete(actor, ss.SafeRemainingArgument);
				return;
			case "reset":
				VehicleAccessPresetReset(actor, ss.SafeRemainingArgument);
				return;
			default:
				actor.OutputHandler.Send("Use #3vehicle access preset list|show|set|remove|delete|reset#0.".SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleAccessPresetList(ICharacter actor)
	{
		var presets = FleetOperationsService.AccessPresets(actor.Gameworld).OrderBy(x => x.Name).ToList();
		var sb = new StringBuilder();
		sb.AppendLine("Vehicle access presets:");
		foreach (var preset in presets)
		{
			sb.AppendLine($"\t{preset.Name.ColourName()}: {DescribeAccessPresetGrants(actor, preset)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void VehicleAccessPresetShow(ICharacter actor, string name)
	{
		if (!FleetOperationsService.TryGetAccessPreset(actor.Gameworld, name, out var preset, out var reason) || preset is null)
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		actor.OutputHandler.Send($"Vehicle access preset {preset.Name.ColourName()}: {DescribeAccessPresetGrants(actor, preset)}");
	}

	private static void VehicleAccessPresetSet(ICharacter actor, StringStack ss)
	{
		var name = ss.PopSpeech().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(name))
		{
			actor.OutputHandler.Send("Which preset do you want to change?");
			return;
		}

		var tag = ss.PopSpeech().ToLowerInvariant();
		if (!ValidVehicleAccessTag(tag))
		{
			actor.OutputHandler.Send("Access tags must be one of board, control, service, repair, hitch, or all.");
			return;
		}

		if (!int.TryParse(ss.PopSpeech(), out var level) || level < 1 || level > 3)
		{
			actor.OutputHandler.Send("Access level must be 1, 2, or 3.");
			return;
		}

		FleetOperationsService.TryGetAccessPreset(actor.Gameworld, name, out var existing, out _);
		var grants = (existing?.Grants ?? [])
		             .Where(x => !x.AccessTag.EqualTo(tag))
		             .Append(new VehicleAccessPresetGrant(tag, level))
		             .OrderBy(x => x.AccessTag)
		             .ToList();
		var preset = FleetOperationsService.SaveAccessPreset(actor.Gameworld, name, grants);
		actor.OutputHandler.Send($"Vehicle access preset {preset.Name.ColourName()} is now {DescribeAccessPresetGrants(actor, preset)}.");
	}

	private static void VehicleAccessPresetRemove(ICharacter actor, StringStack ss)
	{
		var name = ss.PopSpeech().ToLowerInvariant();
		if (!FleetOperationsService.TryGetAccessPreset(actor.Gameworld, name, out var preset, out var reason) || preset is null)
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		var tag = ss.PopSpeech().ToLowerInvariant();
		if (!ValidVehicleAccessTag(tag))
		{
			actor.OutputHandler.Send("Which valid access tag do you want to remove?");
			return;
		}

		var grants = preset.Grants.Where(x => !x.AccessTag.EqualTo(tag)).ToList();
		if (grants.Count == preset.Grants.Count)
		{
			actor.OutputHandler.Send("That preset does not contain such a grant.");
			return;
		}

		preset = FleetOperationsService.SaveAccessPreset(actor.Gameworld, preset.Name, grants);
		actor.OutputHandler.Send($"Vehicle access preset {preset.Name.ColourName()} is now {DescribeAccessPresetGrants(actor, preset)}.");
	}

	private static void VehicleAccessPresetDelete(ICharacter actor, string name)
	{
		if (!FleetOperationsService.DeleteAccessPreset(actor.Gameworld, name, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		actor.OutputHandler.Send($"You delete the vehicle access preset {name.ColourName()}.");
	}

	private static void VehicleAccessPresetReset(ICharacter actor, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			actor.OutputHandler.Send("Which preset do you want to reset?");
			return;
		}

		if (name.EqualTo("all"))
		{
			var presets = FleetOperationsService.ResetAllAccessPresets(actor.Gameworld);
			actor.OutputHandler.Send($"You reset {presets.Count.ToString("N0", actor).ColourValue()} vehicle access presets to defaults.");
			return;
		}

		var preset = FleetOperationsService.ResetAccessPreset(actor.Gameworld, name);
		actor.OutputHandler.Send($"Vehicle access preset {preset.Name.ColourName()} is now {DescribeAccessPresetGrants(actor, preset)}.");
	}

	private static string DescribeAccessPresetGrants(ICharacter actor, VehicleAccessPreset preset)
	{
		return preset.Grants.Any()
			? preset.Grants.OrderBy(x => x.AccessTag).Select(x => $"{x.AccessTag.ColourCommand()}:{x.AccessLevel.ToString("N0", actor).ColourValue()}").ListToString()
			: "no grants".Colour(Telnet.Yellow);
	}

	private static void VehicleAccessApply(ICharacter actor, IVehicle vehicle, StringStack ss)
	{
		var presetName = ss.PopSpeech();
		if (!FleetOperationsService.TryGetAccessPreset(actor.Gameworld, presetName, out var preset, out var reason) || preset is null)
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		var target = ResolveAccessCharacter(actor, ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("There is no such character.");
			return;
		}

		var result = FleetOperationsService.ApplyAccessPreset(vehicle, target, preset);
		actor.OutputHandler.Send($"You apply {preset.Name.ColourName()} access to {target.RenderStaffActorReference()} on {vehicle.Name.ColourName()}, setting {result.Grants.Count.ToString("N0", actor).ColourValue()} grant{(result.Grants.Count == 1 ? "" : "s")}.");
	}

	private static void VehicleAccessClone(ICharacter actor, IVehicle vehicle, StringStack ss)
	{
		var source = actor.Gameworld.Vehicles.GetByIdOrName(ss.SafeRemainingArgument);
		if (source is null)
		{
			actor.OutputHandler.Send("There is no such source vehicle.");
			return;
		}

		var count = FleetOperationsService.CloneAccess(vehicle, source);
		actor.OutputHandler.Send($"You clone {count.ToString("N0", actor).ColourValue()} access grant{(count == 1 ? "" : "s")} from {source.Name.ColourName()} to {vehicle.Name.ColourName()}.");
	}

	private static void VehicleAudit(ICharacter actor, StringStack ss)
	{
		if (!ResolveVehicleScope(actor, ss, out var vehicles, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		if (!TryParseFleetAuditMode(ss.PopSpeech(), out var mode))
		{
			actor.OutputHandler.Send("Audit mode must be readiness, access, resources, hitch, damage, recovery, or all.");
			return;
		}

		DescribeFleetAudit(actor, FleetOperationsService.Audit(vehicles, actor, mode), mode);
	}

	private static void VehicleRecover(ICharacter actor, StringStack ss)
	{
		var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(ss.PopSpeech());
		if (vehicle is null)
		{
			actor.OutputHandler.Send("There is no such vehicle.");
			return;
		}

		var modeText = ss.PopSpeech();
		var apply = false;
		if (modeText.EqualTo("fix"))
		{
			apply = true;
			modeText = string.Empty;
		}

		if (!TryParseRecoveryMode(modeText, out var mode))
		{
			actor.OutputHandler.Send("Recovery mode must be projection, install, hitch, or all.");
			return;
		}

		apply |= ss.PopSpeech().EqualTo("fix");
		DescribeRecoveryResults(actor, [FleetOperationsService.Recover(vehicle, mode, apply)], apply);
	}

	private static void VehicleFleet(ICharacter actor, StringStack ss)
	{
		if (!ResolveVehicleScope(actor, ss, out var vehicles, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		var vehicleList = vehicles.DistinctBy(x => x.Id).ToList();
		if (!vehicleList.Any())
		{
			actor.OutputHandler.Send("That scope does not contain any vehicles.");
			return;
		}

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "audit":
				if (!TryParseFleetAuditMode(ss.PopSpeech(), out var mode))
				{
					actor.OutputHandler.Send("Audit mode must be readiness, access, resources, hitch, damage, recovery, or all.");
					return;
				}

				DescribeFleetAudit(actor, FleetOperationsService.Audit(vehicleList, actor, mode), mode);
				return;
			case "recover":
				VehicleFleetRecover(actor, vehicleList, ss);
				return;
			case "access":
				VehicleFleetAccess(actor, vehicleList, ss);
				return;
			default:
				actor.OutputHandler.Send("Use #3vehicle fleet <scope> access|recover|audit#0.".SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleFleetAccess(ICharacter actor, IReadOnlyList<IVehicle> vehicles, StringStack ss)
	{
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "apply":
				VehicleFleetAccessApply(actor, vehicles, ss);
				return;
			case "grant":
				VehicleFleetAccessGrant(actor, vehicles, ss);
				return;
			case "revoke":
				VehicleFleetAccessRevoke(actor, vehicles, ss);
				return;
			case "clone":
				VehicleFleetAccessClone(actor, vehicles, ss);
				return;
			default:
				actor.OutputHandler.Send("Use #3vehicle fleet <scope> access apply|grant|revoke|clone#0.".SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleFleetAccessApply(ICharacter actor, IReadOnlyList<IVehicle> vehicles, StringStack ss)
	{
		var presetName = ss.PopSpeech();
		if (!FleetOperationsService.TryGetAccessPreset(actor.Gameworld, presetName, out var preset, out var reason) || preset is null)
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		var target = ResolveAccessCharacter(actor, ss.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("There is no such character.");
			return;
		}

		var count = vehicles.Sum(vehicle => FleetOperationsService.ApplyAccessPreset(vehicle, target, preset).Grants.Count);
		actor.OutputHandler.Send($"You apply {preset.Name.ColourName()} access to {target.RenderStaffActorReference()} on {vehicles.Count.ToString("N0", actor).ColourValue()} vehicle{(vehicles.Count == 1 ? "" : "s")}, setting {count.ToString("N0", actor).ColourValue()} grant{(count == 1 ? "" : "s")}.");
	}

	private static void VehicleFleetAccessGrant(ICharacter actor, IReadOnlyList<IVehicle> vehicles, StringStack ss)
	{
		var target = ResolveAccessCharacter(actor, ss.PopSpeech());
		if (target is null)
		{
			actor.OutputHandler.Send("There is no such character.");
			return;
		}

		var tag = ss.PopSpeech().ToLowerInvariant();
		if (!ValidVehicleAccessTag(tag))
		{
			actor.OutputHandler.Send("Access tags must be one of board, control, service, repair, hitch, or all.");
			return;
		}

		if (!int.TryParse(ss.PopSpeech(), out var level) || level < 1 || level > 3)
		{
			actor.OutputHandler.Send("Access level must be 1, 2, or 3.");
			return;
		}

		foreach (var vehicle in vehicles)
		{
			vehicle.GrantAccess(target, tag, level);
		}

		actor.OutputHandler.Send($"You grant {target.RenderStaffActorReference()} {tag.ColourCommand()} access level {level.ToString("N0", actor).ColourValue()} on {vehicles.Count.ToString("N0", actor).ColourValue()} vehicle{(vehicles.Count == 1 ? "" : "s")}.");
	}

	private static void VehicleFleetAccessRevoke(ICharacter actor, IReadOnlyList<IVehicle> vehicles, StringStack ss)
	{
		var targetText = ss.PopSpeech();
		if (string.IsNullOrWhiteSpace(targetText))
		{
			actor.OutputHandler.Send("Which access row or character do you want to revoke?");
			return;
		}

		var count = 0;
		if (long.TryParse(targetText, out var rowId))
		{
			count = vehicles.Count(vehicle => vehicle.RevokeAccess(rowId));
			actor.OutputHandler.Send(count > 0
				? $"You revoke access row #{rowId.ToString("N0", actor)} from {count.ToString("N0", actor).ColourValue()} vehicle{(count == 1 ? "" : "s")} in the scope."
				: "There was no matching access row in that vehicle scope.");
			return;
		}

		var target = ResolveAccessCharacter(actor, targetText);
		if (target is null)
		{
			actor.OutputHandler.Send("There is no such character or access row.");
			return;
		}

		var tag = ss.SafeRemainingArgument;
		if (!string.IsNullOrWhiteSpace(tag) && !ValidVehicleAccessTag(tag))
		{
			actor.OutputHandler.Send("Access tags must be one of board, control, service, repair, hitch, or all.");
			return;
		}

		count = vehicles.Sum(vehicle => vehicle.RevokeAccess(target, tag));
		actor.OutputHandler.Send(count > 0
			? $"You revoke {count.ToString("N0", actor).ColourValue()} access grant{(count == 1 ? "" : "s")} for {target.RenderStaffActorReference()} across {vehicles.Count.ToString("N0", actor).ColourValue()} vehicle{(vehicles.Count == 1 ? "" : "s")}."
			: "There were no matching access grants to revoke.");
	}

	private static void VehicleFleetAccessClone(ICharacter actor, IReadOnlyList<IVehicle> vehicles, StringStack ss)
	{
		var source = actor.Gameworld.Vehicles.GetByIdOrName(ss.SafeRemainingArgument);
		if (source is null)
		{
			actor.OutputHandler.Send("There is no such source vehicle.");
			return;
		}

		var targets = vehicles.Where(x => x.Id != source.Id).ToList();
		var count = targets.Sum(vehicle => FleetOperationsService.CloneAccess(vehicle, source));
		actor.OutputHandler.Send($"You clone {count.ToString("N0", actor).ColourValue()} access grant{(count == 1 ? "" : "s")} from {source.Name.ColourName()} to {targets.Count.ToString("N0", actor).ColourValue()} vehicle{(targets.Count == 1 ? "" : "s")}.");
	}

	private static void VehicleFleetRecover(ICharacter actor, IReadOnlyList<IVehicle> vehicles, StringStack ss)
	{
		var modeText = ss.PopSpeech();
		var apply = false;
		if (modeText.EqualTo("fix"))
		{
			apply = true;
			modeText = string.Empty;
		}

		if (!TryParseRecoveryMode(modeText, out var mode))
		{
			actor.OutputHandler.Send("Recovery mode must be projection, install, hitch, or all.");
			return;
		}

		apply |= ss.PopSpeech().EqualTo("fix");
		DescribeRecoveryResults(actor, vehicles.Select(x => FleetOperationsService.Recover(x, mode, apply)).ToList(), apply);
	}

	private static bool ResolveVehicleScope(ICharacter actor, StringStack ss, out IReadOnlyList<IVehicle> vehicles,
		out string reason)
	{
		vehicles = [];
		reason = string.Empty;
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "here":
				vehicles = actor.Gameworld.Vehicles.Where(x => x.Location == actor.Location).ToList();
				return true;
			case "zone":
				vehicles = actor.Gameworld.Vehicles.Where(x => x.Location?.Zone == actor.Location?.Zone).ToList();
				return true;
			case "prototype":
			case "proto":
				var proto = actor.Gameworld.VehiclePrototypes.GetByIdOrName(ss.PopSpeech());
				if (proto is null)
				{
					reason = "There is no such vehicle prototype.";
					return false;
				}

				vehicles = actor.Gameworld.Vehicles.Where(x => x.Prototype.Id == proto.Id).ToList();
				return true;
			case "all":
				vehicles = actor.Gameworld.Vehicles.ToList();
				return true;
			default:
				reason = "Vehicle scope must be here, zone, prototype <id|name>, or all.";
				return false;
		}
	}

	private static bool TryParseFleetAuditMode(string text, out VehicleFleetAuditMode mode)
	{
		mode = VehicleFleetAuditMode.All;
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}

		return Enum.TryParse(text, true, out mode) && Enum.IsDefined(mode);
	}

	private static bool TryParseRecoveryMode(string text, out VehicleRecoveryMode mode)
	{
		mode = VehicleRecoveryMode.All;
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}

		return Enum.TryParse(text, true, out mode) && Enum.IsDefined(mode);
	}

	private static void DescribeFleetAudit(ICharacter actor, VehicleFleetAuditResult audit, VehicleFleetAuditMode mode)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Vehicle {mode.DescribeEnum().ColourName()} audit for {audit.Vehicles.Count.ToString("N0", actor).ColourValue()} vehicle{(audit.Vehicles.Count == 1 ? "" : "s")}:" );
		if (!audit.Findings.Any())
		{
			sb.AppendLine("\tNo findings.".Colour(Telnet.Green));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		foreach (var vehicleGroup in audit.Findings.GroupBy(x => x.Vehicle).OrderBy(x => x.Key.Id))
		{
			sb.AppendLine($"\t#{vehicleGroup.Key.Id.ToString("N0", actor)} {vehicleGroup.Key.Name.ColourName()}:");
			foreach (var finding in vehicleGroup.OrderByDescending(x => x.Severity).ThenBy(x => x.Subsystem))
			{
				sb.AppendLine($"\t\t{finding.Severity.DescribeEnum().ColourValue()} {finding.Subsystem.DescribeEnum().ColourName()}: {finding.Reason}{(string.IsNullOrWhiteSpace(finding.Hint) ? string.Empty : $" Hint: {finding.Hint.ColourCommand()}")}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void DescribeRecoveryResults(ICharacter actor, IReadOnlyList<VehicleRecoveryResult> results, bool applied)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Vehicle recovery {(applied ? "fix" : "audit").ColourName()} for {results.Count.ToString("N0", actor).ColourValue()} vehicle{(results.Count == 1 ? "" : "s")}:" );
		if (results.All(x => !x.Findings.Any()))
		{
			sb.AppendLine("\tNo recovery findings.".Colour(Telnet.Green));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		foreach (var result in results.Where(x => x.Findings.Any()).OrderBy(x => x.Vehicle.Id))
		{
			sb.AppendLine($"\t#{result.Vehicle.Id.ToString("N0", actor)} {result.Vehicle.Name.ColourName()}:");
			foreach (var finding in result.Findings.OrderByDescending(x => x.Severity).ThenBy(x => x.Subsystem))
			{
				sb.AppendLine($"\t\t{finding.Severity.DescribeEnum().ColourValue()} {finding.Subsystem.DescribeEnum().ColourName()} {finding.Action.DescribeEnum().ColourCommand()}: {finding.Reason}{(string.IsNullOrWhiteSpace(finding.Hint) ? string.Empty : $" Hint: {finding.Hint.ColourCommand()}")}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}
	private static ICharacter ResolveAccessCharacter(ICharacter actor, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		if (long.TryParse(text, out var id))
		{
			return actor.Gameworld.TryGetCharacter(id, true);
		}

		return actor.TargetActor(text) ?? actor.Gameworld.Characters.GetByIdOrName(text);
	}

	private static bool ValidVehicleAccessTag(string tag)
	{
		return tag.EqualToAny("board", "control", "service", "repair", "hitch", "all");
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
		sb.AppendLine();
		AppendOperationalReadiness(actor, vehicle, sb);
		return sb.ToString();
	}

	private static void AppendOperationalReadiness(ICharacter actor, IVehicle vehicle, StringBuilder sb)
	{
		var movementProfile = vehicle.MovementProfile;
		var report = OperationalReadinessService.BuildReport(vehicle, actor, null, movementProfile);
		sb.AppendLine("Operational Readiness:");
		sb.AppendLine(vehicle.AccessStates.Any()
			? $"\tAccess: {vehicle.AccessStates.Count().ToString("N0", actor).ColourValue()} explicit grant{(vehicle.AccessStates.Count() == 1 ? "" : "s")}; unlisted characters are blocked."
			: "\tAccess: default permissive (no explicit grants).".Colour(Telnet.Green));

		if (movementProfile is not null)
		{
			sb.AppendLine($"\tMovement Profile: {movementProfile.Name.ColourName()} (#{movementProfile.Id.ToString("N0", actor)})");
			sb.AppendLine($"\tEnvironment: {movementProfile.MovementEnvironment.DescribeEnum(true).ColourName()}");
			if (movementProfile.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater)
			{
				sb.AppendLine($"\tOccupant Water Exposure: {(movementProfile.ExposesOccupantsToWater ? "Exposed".Colour(Telnet.Yellow) : "Protected".Colour(Telnet.Green))}");
				var modes = movementProfile.PropulsionProfiles.ToList();
				sb.AppendLine($"\tActive Propulsion: {(vehicle.ActivePropulsionProfile?.PropulsionType.DescribeEnum().ColourName() ?? "legacy implicit movement".Colour(Telnet.Yellow))}");
				sb.AppendLine($"\tSupported Propulsion: {(modes.Any() ? modes.Select(x => x.PropulsionType.DescribeEnum().ColourName()).ListToString() : "none authored".Colour(Telnet.Yellow))}");
				var rowerSlots = vehicle.Prototype.OccupantSlots.Where(x => x.ContributesToPropulsion).ToList();
				sb.AppendLine($"\tPropulsion Slots: {(rowerSlots.Any() ? rowerSlots.Select(x => x.Name.ColourName()).ListToString() : "none")}");
				foreach (var mode in modes)
				{
					sb.AppendLine($"\t\t#{mode.Id.ToString("N0", actor)} {mode.PropulsionType.DescribeEnum().ColourName()} time {TimeSpan.FromMilliseconds(mode.BaseMoveTimeMilliseconds).Describe(actor).ColourValue()} speed {mode.SpeedMultiplierExpression.ColourCommand()}{(mode.PropulsionType is VehiclePropulsionType.SelfPowered or VehiclePropulsionType.Rowed ? $" stamina {mode.StaminaCostExpression.ColourCommand()} trait {(mode.PropulsionTrait?.Name.ColourName() ?? "none".ColourError())}" : "")}");
				}

				var propulsion = PropulsionService.BuildReadiness(vehicle, vehicle.Controller ?? actor, null);
				if (propulsion.Profile?.PropulsionType == VehiclePropulsionType.Sail)
				{
					sb.AppendLine($"\tWind: {propulsion.Wind.Describe().ColourName()}");
				}

				if (propulsion.Contributors.Any())
				{
					sb.AppendLine($"\tContributors: {propulsion.Contributors.Select(x => $"{x.Character.HowSeen(actor)}{(x.OarItem is null ? "" : $" with {x.OarItem.HowSeen(actor)}")}").ListToString()}");
				}

				foreach (var motor in propulsion.Motors)
				{
					sb.AppendLine($"\tMotor {(motor.Item?.HowSeen(actor) ?? motor.Installation.Prototype.Name)}: {(motor.Available ? "ready".Colour(Telnet.Green) : motor.Reason.Colour(Telnet.Yellow))}");
				}

				if (!propulsion.CanMove)
				{
					sb.AppendLine($"\tPropulsion Blocker: {propulsion.Reason.ColourError()}");
				}
			}
		}

		foreach (var module in report.Modules.Where(x => !x.IsFunctionalForMovement))
		{
			sb.AppendLine($"\tModule {module.Installation.Prototype.Name.ColourName()}: {module.Reason.Colour(Telnet.Yellow)}");
		}

		AppendResourceCandidates(actor, sb, "Fuel", report.FuelCandidates);
		AppendResourceCandidates(actor, sb, "Power", report.PowerCandidates);

		if (!report.Issues.Any())
		{
			sb.AppendLine("\tNo readiness warnings or blockers detected.".Colour(Telnet.Green));
			return;
		}

		foreach (var issue in report.Issues.OrderByDescending(x => x.Severity).ThenBy(x => x.Subsystem))
		{
			sb.AppendLine($"\t{issue.Severity.DescribeEnum().ColourValue()} {issue.Subsystem.DescribeEnum().ColourName()}: {issue.Reason}{(string.IsNullOrWhiteSpace(issue.RepairHint) ? string.Empty : $" Hint: {issue.RepairHint.ColourCommand()}")}");
		}
	}

	private static void AppendResourceCandidates(ICharacter actor, StringBuilder sb, string label,
		IReadOnlyList<VehicleResourceCandidate> candidates)
	{
		if (!candidates.Any())
		{
			return;
		}

		sb.AppendLine($"\t{label} Candidates:");
		foreach (var candidate in candidates)
		{
			sb.AppendLine($"\t\t{(candidate.Item is null ? candidate.Installation.Prototype.Name.ColourName() : candidate.Item.HowSeen(actor))}: {(candidate.Available ? "ready".Colour(Telnet.Green) : candidate.Reason.Colour(Telnet.Yellow))}");
		}
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
		var hitchIndex = text.IndexOf(" hitch ", StringComparison.InvariantCultureIgnoreCase);
		if (hitchIndex > 0)
		{
			var vehicleText = text[..hitchIndex].Trim();
			var linkText = text[(hitchIndex + 7)..].Trim();
			var hitchVehicle = actor.Gameworld.Vehicles.GetByIdOrName(vehicleText);
			if (hitchVehicle is null)
			{
				actor.OutputHandler.Send("There is no such vehicle.");
				return;
			}

			VehicleRepairHitch(actor, hitchVehicle, linkText);
			return;
		}

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

	private static void VehicleRepairHitch(ICharacter actor, IVehicle vehicle, string linkText)
	{
		if (vehicle.ExteriorItem is null)
		{
			actor.OutputHandler.Send("That vehicle has no exterior item, so its hitch graph cannot be inspected.");
			return;
		}

		var links = HitchGraphService.LinksInvolving(actor.Gameworld, vehicle.ExteriorItem)
		                             .Where(x => x.Kind is VehicleHitchGraphLinkKind.LegacyVehicleTow or VehicleHitchGraphLinkKind.PersistentHitch)
		                             .ToList();
		if (!links.Any())
		{
			actor.OutputHandler.Send("That vehicle has no persistent hitch or tow links.");
			return;
		}

		IEnumerable<VehicleHitchGraphLink> selected;
		if (linkText.EqualTo("all"))
		{
			selected = links;
		}
		else if (long.TryParse(linkText, out var linkId))
		{
			selected = links.Where(x => x.WrappedLink?.Id == linkId).ToList();
		}
		else
		{
			actor.OutputHandler.Send("Specify a hitch link id or #3all#0.".SubstituteANSIColour());
			return;
		}

		var selectedList = selected.ToList();
		if (!selectedList.Any())
		{
			actor.OutputHandler.Send("There is no matching persistent hitch or tow link on that vehicle.");
			return;
		}

		var repaired = new List<VehicleHitchGraphLink>();
		var failed = new List<string>();
		foreach (var link in selectedList)
		{
			if (OperationalReadinessService.RepairHitchLink(link, out var reason))
			{
				repaired.Add(link);
				continue;
			}

			failed.Add($"#{link.WrappedLink?.Id.ToString("N0", actor) ?? "0"}: {reason}");
		}

		var sb = new StringBuilder();
		if (repaired.Any())
		{
			sb.AppendLine($"You repair {repaired.Count.ToString("N0", actor).ColourValue()} persistent hitch/tow link{(repaired.Count == 1 ? "" : "s")} for {vehicle.Name.ColourName()}.");
		}

		if (failed.Any())
		{
			sb.AppendLine("Some links could not be restored:");
			foreach (var failure in failed)
			{
				sb.AppendLine($"\t{failure}");
			}
		}

		actor.Gameworld.SaveManager.Flush();
		actor.OutputHandler.Send(sb.ToString());
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
