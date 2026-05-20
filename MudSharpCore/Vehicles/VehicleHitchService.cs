#nullable enable

using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public class VehicleHitchService : IVehicleHitchService
{
	public IReadOnlyList<IVehicleHitchLink> LinksFrom(IFuturemud gameworld, IPerceivable source)
	{
		return (gameworld.VehicleHitchLinks ?? Enumerable.Empty<IVehicleHitchLink>())
		                .Where(x => EndpointMatches(x, true, source))
		                .ToList();
	}

	public IReadOnlyList<IVehicleHitchLink> LinksInvolving(IFuturemud gameworld, IPerceivable perceivable)
	{
		return (gameworld.VehicleHitchLinks ?? Enumerable.Empty<IVehicleHitchLink>())
		                .Where(x => EndpointMatches(x, true, perceivable) ||
		                            EndpointMatches(x, false, perceivable))
		                .ToList();
	}

	public bool CanPersistCharacterHitch(ICharacter source, IPerceivable target, out string reason)
	{
		if (source.IsPlayerCharacter)
		{
			reason = "Player-character hitch endpoints remain transient and are not persisted.";
			return false;
		}

		if (target is ICharacter targetCharacter && targetCharacter.IsPlayerCharacter)
		{
			reason = "Player-character hitch endpoints remain transient and are not persisted.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public IVehicleHitchLink? CreatePersistentCharacterHitch(ICharacter actor, ICharacter source, IPerceivable target,
		IVehicle? targetVehicle, IVehicleTowPointPrototype? targetTowPoint, IGameItem? hitchItem, out string reason)
	{
		if (!CanPersistCharacterHitch(source, target, out reason))
		{
			return null;
		}

		var targetCharacter = target as ICharacter;
		if (targetVehicle is null && targetCharacter is null)
		{
			reason = "Only character and vehicle hitch targets can be persisted.";
			return null;
		}

		DB.VehicleHitchLink dbitem;
		using (new FMDB())
		{
			dbitem = new DB.VehicleHitchLink
			{
				SourceType = (int)VehicleHitchEndpointType.Character,
				SourceCharacterId = source.Id,
				TargetType = targetVehicle is not null
					? (int)VehicleHitchEndpointType.Vehicle
					: (int)VehicleHitchEndpointType.Character,
				TargetVehicleId = targetVehicle?.Id,
				TargetCharacterId = targetCharacter?.Id,
				TargetTowPointProtoId = targetTowPoint?.Id,
				HitchItemId = hitchItem?.Id,
				IsDisabled = false,
				CreatedDateTime = DateTime.UtcNow
			};
			FMDB.Context.VehicleHitchLinks.Add(dbitem);
			FMDB.Context.SaveChanges();
		}

		var link = new VehicleHitchLink(actor.Gameworld, dbitem);
		actor.Gameworld.Add(link);
		reason = string.Empty;
		return link;
	}

	public void DeletePersistentLink(IFuturemud gameworld, long linkId)
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleHitchLinks.Find(linkId);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleHitchLinks.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		var link = gameworld.VehicleHitchLinks?.Get(linkId);
		if (link is not null)
		{
			gameworld.Destroy(link);
		}
	}

	public void RecoverPersistentLinks(IFuturemud gameworld)
	{
		foreach (var link in (gameworld.VehicleHitchLinks ?? Enumerable.Empty<IVehicleHitchLink>()).ToList())
		{
			if (!CanRecover(link, out _))
			{
				continue;
			}

			ApplyRuntimeProjection(link);
		}
	}

	private static bool EndpointMatches(IVehicleHitchLink link, bool source, IPerceivable perceivable)
	{
		var type = source ? link.SourceType : link.TargetType;
		var vehicleId = source ? link.SourceVehicleId : link.TargetVehicleId;
		var characterId = source ? link.SourceCharacterId : link.TargetCharacterId;

		return type switch
		{
			VehicleHitchEndpointType.Vehicle => perceivable is IGameItem item &&
			                                    item.GetItemType<IVehicleExterior>()?.Vehicle?.Id == vehicleId,
			VehicleHitchEndpointType.Character => perceivable is ICharacter character && character.Id == characterId,
			_ => false
		};
	}

	private static bool CanRecover(IVehicleHitchLink link, out string reason)
	{
		if (!string.IsNullOrWhiteSpace(link.WhyInvalid))
		{
			reason = link.WhyInvalid;
			return false;
		}

		var source = link.SourceCharacter;
		if (source is null)
		{
			reason = "the source character is missing";
			return false;
		}

		if (source.IsPlayerCharacter)
		{
			reason = "player-character source endpoints are transient";
			return false;
		}

		if (source.EffectsOfType<Dragging>().Any())
		{
			reason = "the source character is already dragging something";
			return false;
		}

		var target = TargetPerceivable(link);
		if (target is null)
		{
			reason = "the hitch target is missing";
			return false;
		}

		if (target is ICharacter targetCharacter && targetCharacter.IsPlayerCharacter)
		{
			reason = "player-character target endpoints are transient";
			return false;
		}

		if (target.AffectedBy<Dragging.DragTarget>())
		{
			reason = "the target is already being dragged";
			return false;
		}

		if (link.TargetType == VehicleHitchEndpointType.Vehicle &&
		    link.TargetTowPoint is not null &&
		    TowPointRequiresHitchItem(link.TargetTowPoint) &&
		    link.HitchItem?.GetItemType<IDragAid>() is null)
		{
			reason = "the vehicle tow point requires a usable hitch item";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static void ApplyRuntimeProjection(IVehicleHitchLink link)
	{
		var source = link.SourceCharacter;
		var target = TargetPerceivable(link);
		if (source is null || target is null)
		{
			return;
		}

		var towPoint = link.TargetTowPoint;
		source.AddEffect(new CharacterHitch(source, target, towPoint?.CharacterPullMultiplier ?? 1.0, towPoint?.Id, link.Id));
		source.AddEffect(new Dragging(source, link.HitchItem?.GetItemType<IDragAid>(), target));
	}

	private static IPerceivable? TargetPerceivable(IVehicleHitchLink link)
	{
		return link.TargetType switch
		{
			VehicleHitchEndpointType.Vehicle => link.TargetVehicle?.ExteriorItem,
			VehicleHitchEndpointType.Character => link.TargetCharacter,
			_ => null
		};
	}

	private static bool TowPointRequiresHitchItem(IVehicleTowPointPrototype towPoint)
	{
		return !towPoint.TowType.EqualToAny("hand", "manual", "direct", "none", "pull");
	}
}
