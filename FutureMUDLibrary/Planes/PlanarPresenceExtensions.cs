using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.GameItems;
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
