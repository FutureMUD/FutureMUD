using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Planes;

public class PlanarPresence
{
	public PlanarPresence(PlanarPresenceDefinition definition)
	{
		Definition = definition;
	}

	public PlanarPresenceDefinition Definition { get; }

	public IReadOnlyCollection<long> PresencePlaneIds => Definition.PresencePlaneIds;
	public IReadOnlyCollection<long> VisibleToPlaneIds => Definition.VisibleToPlaneIds;
	public IReadOnlyCollection<long> PerceivesPlaneIds => Definition.PerceivesPlaneIds;
	public bool SuspendsPhysicalContact => Definition.SuspendsPhysicalContact;
	public bool PropagatesInventory => Definition.PropagatesInventory;
	public bool CanCrossClosedDoors => Definition.CanCrossClosedDoors;
	public bool CanCrossMagicalBarriers => Definition.CanCrossMagicalBarriers;
	public bool VisibleToEtherealPerception => Definition.VisibleToEtherealPerception;
	public bool PlayerCanManifest => Definition.PlayerCanManifest;
	public bool PlayerCanDissipate => Definition.PlayerCanDissipate;

	public IReadOnlyCollection<long> InteractionPlaneIds(PlanarInteractionKind kind)
	{
		return Definition.InteractionPlaneIds(kind);
	}

	public bool SharesPlaneWith(PlanarPresence other)
	{
		return PresencePlaneIds.Intersect(other.PresencePlaneIds).Any();
	}

	public bool CanPerceive(PlanarPresence target)
	{
		return PresencePlaneIds.Intersect(target.VisibleToPlaneIds).Any() ||
		       PerceivesPlaneIds.Intersect(target.VisibleToPlaneIds).Any();
	}

	public bool CanInteract(PlanarPresence target, PlanarInteractionKind kind)
	{
		if (kind is PlanarInteractionKind.Observe or PlanarInteractionKind.Hear or PlanarInteractionKind.Speak)
		{
			return CanPerceive(target);
		}

		return InteractionPlaneIds(kind).Intersect(target.PresencePlaneIds).Any() &&
		       target.InteractionPlaneIds(kind).Intersect(PresencePlaneIds).Any();
	}
}
