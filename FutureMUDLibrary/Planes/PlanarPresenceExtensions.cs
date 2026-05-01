using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Planes;

public static class PlanarPresenceExtensions
{
	public static PlanarPresence GetPlanarPresence(this IPerceivable perceivable)
	{
		var baseDefinition = BaseDefinitionFor(perceivable);
		var overlays = OverlayEffectsFor(perceivable)
			.Select(x => (Definition: x.PlanarPresenceDefinition, x.PlanarPriority, x.OverridesBasePlanarPresence))
			.Concat(OverlayMeritsFor(perceivable)
				.Select(x => (Definition: x.PlanarPresenceDefinition, x.PlanarPriority, x.OverridesBasePlanarPresence)));
		var definition = overlays
			.OrderBy(x => x.PlanarPriority)
			.Aggregate(baseDefinition, (current, overlay) =>
				overlay.OverridesBasePlanarPresence ? overlay.Definition : current.Merge(overlay.Definition));
		return new PlanarPresence(definition);
	}

	public static bool CanPerceivePlanar(this IPerceiver voyeur, IPerceivable target,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (flags.HasFlag(PerceiveIgnoreFlags.IgnorePlanes))
		{
			return true;
		}

		if (voyeur is null || target is null)
		{
			return false;
		}

		if (target is ILocation)
		{
			return true;
		}

		var voyeurPresence = ((IPerceivable)voyeur).GetPlanarPresence();
		var targetPresence = target.GetPlanarPresence();
		if (voyeurPresence.CanPerceive(targetPresence))
		{
			return true;
		}

		if (!targetPresence.VisibleToEtherealPerception)
		{
			return false;
		}

		var perceptions = voyeur.GetPerception(voyeur.NaturalPerceptionTypes);
		return (perceptions & (PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal)) != PerceptionTypes.None;
	}

	public static bool CanInteractPlanar(this IPerceivable actor, IPerceivable target, PlanarInteractionKind kind)
	{
		return actor.GetPlanarPresence().CanInteract(target.GetPlanarPresence(), kind);
	}

	public static bool CanInteractPlanar(this IPerceivable actor, IPerceivable target, PlanarInteractionKind kind,
		out string whyNot)
	{
		if (actor.CanInteractPlanar(target, kind))
		{
			whyNot = string.Empty;
			return true;
		}

		whyNot = $"{target.HowSeen(actor as IPerceiver)} is not physically present to you on a plane you can affect.";
		return false;
	}

	public static bool SharesPlaneWith(this IPerceivable actor, IPerceivable target)
	{
		return actor.GetPlanarPresence().SharesPlaneWith(target.GetPlanarPresence());
	}

	public static IPlane CurrentPlane(this IPerceivable perceivable)
	{
		var presencePlaneIds = perceivable.GetPlanarPresence().PresencePlaneIds;
		var defaultPlane = perceivable.Gameworld.DefaultPlane;
		if (defaultPlane is not null && presencePlaneIds.Contains(defaultPlane.Id))
		{
			return defaultPlane;
		}

		return perceivable.Gameworld.Planes
		                  .Where(x => presencePlaneIds.Contains(x.Id))
		                  .OrderBy(x => x.DisplayOrder)
		                  .ThenBy(x => x.Id)
		                  .FirstOrDefault() ?? defaultPlane;
	}

	public static IPlane RemoteObservationPlane(this IPerceiver voyeur, IPerceivable target,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (flags.HasFlag(PerceiveIgnoreFlags.IgnorePlanes) ||
		    flags.HasFlag(PerceiveIgnoreFlags.IgnoreNamesSetting) ||
		    voyeur is null || target is null || voyeur.IsSelf(target))
		{
			return null;
		}

		var voyeurPresence = ((IPerceivable)voyeur).GetPlanarPresence();
		var targetPresence = target.GetPlanarPresence();
		var currentPlane = ((IPerceivable)voyeur).CurrentPlane();
		if (currentPlane is not null && targetPresence.VisibleToPlaneIds.Contains(currentPlane.Id))
		{
			return null;
		}

		var observableRemotePlanes = targetPresence.PresencePlaneIds
		                                           .Where(x => x != currentPlane?.Id)
		                                           .Where(x => targetPresence.VisibleToPlaneIds.Contains(x))
		                                           .Where(x => voyeurPresence.PresencePlaneIds.Contains(x) ||
		                                                       voyeurPresence.PerceivesPlaneIds.Contains(x))
		                                           .ToHashSet();
		return target.Gameworld.Planes
		             .Where(x => observableRemotePlanes.Contains(x.Id) &&
		                         !string.IsNullOrWhiteSpace(x.RemoteObservationTag))
		             .OrderBy(x => x.DisplayOrder)
		             .ThenBy(x => x.Id)
		             .FirstOrDefault();
	}

	public static string RemoteObservationTagFor(this IPerceiver voyeur, IPerceivable target, bool colour = true,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		var plane = voyeur.RemoteObservationPlane(target, flags);
		if (plane is null)
		{
			return string.Empty;
		}

		string tag;
		try
		{
			tag = string.Format(plane.RemoteObservationTag, plane.Name);
		}
		catch (FormatException)
		{
			tag = $"({plane.Name})";
		}

		tag = tag.SubstituteANSIColour();
		return colour ? tag : tag.StripANSIColour();
	}

	public static string AppendRemoteObservationTag(this string description, IPerceiver voyeur, IPerceivable target,
		bool colour = true, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		var tag = voyeur.RemoteObservationTagFor(target, colour, flags);
		return string.IsNullOrWhiteSpace(tag) ? description : $"{description} {tag}";
	}

	public static bool SuspendsPhysicalContact(this IPerceivable perceivable)
	{
		return perceivable.GetPlanarPresence().SuspendsPhysicalContact;
	}

	public static bool CanCrossClosedDoorsPlanar(this IPerceivable perceivable)
	{
		return perceivable.GetPlanarPresence().CanCrossClosedDoors;
	}

	public static bool CanCrossMagicalBarriersPlanar(this IPerceivable perceivable)
	{
		return perceivable.GetPlanarPresence().CanCrossMagicalBarriers;
	}

	private static PlanarPresenceDefinition BaseDefinitionFor(IPerceivable perceivable)
	{
		return perceivable switch
		{
			ICharacter character when character.Body is IHavePlanarPresence body => body.BasePlanarPresence,
			IHavePlanarPresence have => have.BasePlanarPresence,
			_ => PlanarPresenceDefinition.DefaultMaterial(perceivable.Gameworld)
		};
	}

	private static IEnumerable<IPlanarOverlayEffect> OverlayEffectsFor(IPerceivable perceivable)
	{
		IEnumerable<IPlanarOverlayEffect> self = perceivable.EffectsOfType<IPlanarOverlayEffect>().Where(x => x.Applies());
		return perceivable switch
		{
			ICharacter character => self.Concat(character.Body.EffectsOfType<IPlanarOverlayEffect>().Where(x => x.Applies())),
			IBody body => self.Concat(body.Actor.EffectsOfType<IPlanarOverlayEffect>().Where(x => x.Applies())),
			_ => self
		};
	}

	private static IEnumerable<IPlanarOverlayMerit> OverlayMeritsFor(IPerceivable perceivable)
	{
		return perceivable switch
		{
			ICharacter character => character.Merits.OfType<IPlanarOverlayMerit>().Where(x => x.Applies(character)),
			IBody body => body.Actor.Merits.OfType<IPlanarOverlayMerit>().Where(x => x.Applies(body.Actor)),
			_ => Enumerable.Empty<IPlanarOverlayMerit>()
		};
	}
}
