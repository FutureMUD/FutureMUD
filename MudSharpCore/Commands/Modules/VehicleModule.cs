using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
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
	[HelpInfo("disembark", "Use #3disembark#0 to leave the vehicle you are currently aboard.", AutoHelp.HelpArgOrNoArg)]
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
	[NoMovementCommand]
	[NoHideCommand]
	[HelpInfo("drive", "Use #3drive <direction>#0 while in a driver slot to move your vehicle through a normal exit.", AutoHelp.HelpArgOrNoArg)]
	protected static void Drive(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which direction do you want to drive?");
			return;
		}

		var vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x => x.Controller == actor);
		if (vehicle is null)
		{
			actor.OutputHandler.Send("You are not controlling a vehicle.");
			return;
		}

		var exit = vehicle.Location?.GetExit(ss.PopSpeech(), string.Empty, actor);
		if (exit is null)
		{
			actor.OutputHandler.Send("There is no such exit for the vehicle to use.");
			return;
		}

		if (!vehicle.CanMove(actor, exit, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		vehicle.Move(actor, exit);
	}

	[PlayerCommand("Hitch", "hitch")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("hitch", "Use #3hitch <towpoint>@<vehicle> <towpoint>@<target> [with <item>]#0 to link vehicles for towing.", AutoHelp.HelpArgOrNoArg)]
	protected static void Hitch(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tow point do you want to hitch from?");
			return;
		}

		if (!ResolveTowPoint(actor, ss.PopSpeech(), true, out var sourceVehicle, out var sourcePoint))
		{
			return;
		}

		if (!ResolveTowPoint(actor, ss.PopSpeech(), false, out var targetVehicle, out var targetPoint))
		{
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

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ hitch|hitches $1 to $2.", actor, actor,
			sourceVehicle.ExteriorItem, targetVehicle.ExteriorItem)));
	}

	[PlayerCommand("Unhitch", "unhitch")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoHideCommand]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("unhitch", "Use #3unhitch <vehicle>#0 or #3unhitch <towpoint>@<vehicle>#0 to remove tow links.", AutoHelp.HelpArgOrNoArg)]
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
			links = vehicle.TowLinks.Where(x =>
				(x.SourceVehicle == vehicle && x.SourceTowPoint?.Id == towPoint.Id) ||
				(x.TargetVehicle == vehicle && x.TargetTowPoint?.Id == towPoint.Id)).ToList();
		}
		else
		{
			item = actor.TargetLocalItem(text);
			vehicle = item?.GetItemType<IVehicleExterior>()?.Vehicle;
			if (vehicle is null)
			{
				actor.OutputHandler.Send("You do not see any linked vehicle like that here.");
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
			}

			FMDB.Context.SaveChanges();
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ unhitch|unhitches $1.", actor, actor, item)));
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
			? vehicle.Occupancies.Select(x => $"\t{x.Occupant?.HowSeen(actor) ?? "missing".ColourError()} in {x.Slot.Name.ColourName()}{(x.IsController ? " controlling".Colour(Telnet.Green) : "")}").ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n")
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
		if (!link.IsDisabled)
		{
			return string.Empty;
		}

		if (!string.IsNullOrWhiteSpace(link.WhyInvalid))
		{
			return $" invalid ({link.WhyInvalid})".Colour(Telnet.Red);
		}

		var causes = new List<string>();
		if (link.IsManuallyDisabled)
		{
			causes.Add("manual");
		}

		var sourceDamage = link.SourceVehicle?.DamageZonesDisabling(VehicleDamageEffectTargetType.TowPoint, link.SourceTowPoint.Id).ToList();
		if (sourceDamage?.Any() == true)
		{
			causes.Add($"source damage {sourceDamage.Select(x => $"{x.Name} {x.Status.DescribeEnum()}").ListToString()}");
		}

		var targetDamage = link.TargetVehicle?.DamageZonesDisabling(VehicleDamageEffectTargetType.TowPoint, link.TargetTowPoint.Id).ToList();
		if (targetDamage?.Any() == true)
		{
			causes.Add($"target damage {targetDamage.Select(x => $"{x.Name} {x.Status.DescribeEnum()}").ListToString()}");
		}

		return $" disabled ({causes.ListToString()})".Colour(Telnet.Red);
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
