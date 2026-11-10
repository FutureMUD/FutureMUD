using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.GameItems;
using MudSharp.Planes;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp.Effects.Concrete;

public class PlanarStateEffect : Effect, IPlanarOverlayEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("PlanarState", (effect, owner) => new PlanarStateEffect(effect, owner));
	}

	public PlanarStateEffect(IPerceivable owner, PlanarPresenceDefinition definition, int priority = 0,
		bool overridesBase = true, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		PlanarPresenceDefinition = definition;
		PlanarPriority = priority;
		OverridesBasePlanarPresence = overridesBase;
	}

	private PlanarStateEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var definition = root.Element("Definition");
		PlanarPresenceDefinition = PlanarPresenceDefinition.FromXml(definition?.Element("PlanarData"), Gameworld);
		PlanarPriority = int.Parse(definition?.Attribute("priority")?.Value ?? "0");
		OverridesBasePlanarPresence = bool.Parse(definition?.Attribute("overridesBase")?.Value ?? "true");
	}

	public PlanarPresenceDefinition PlanarPresenceDefinition { get; }
	public int PlanarPriority { get; }
	public bool OverridesBasePlanarPresence { get; }
	public override bool SavingEffect => true;
	public override bool CanBeStoppedByPlayer => PlanarPresenceDefinition.PlayerCanManifest || PlanarPresenceDefinition.PlayerCanDissipate;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Planar state: {PlanarPresenceDefinition.Describe(Gameworld)}.";
	}

	protected override string SpecificEffectType => "PlanarState";

	protected override XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XAttribute("priority", PlanarPriority),
			new XAttribute("overridesBase", OverridesBasePlanarPresence),
			PlanarPresenceDefinition.SaveToXml());
	}

	public override void InitialEffect()
	{
		switch (Owner)
		{
			case ICharacter character when character.Body is ConcreteBody body:
				body.EjectInventoryForPlanarTransition(Owner.GetPlanarPresence().PresencePlaneIds,
					PlanarPresenceDefinition.RetainPlaneSpecificInventory, !PlanarPresenceDefinition.PropagatesInventory);
				break;
			case ConcreteBody body:
				body.EjectInventoryForPlanarTransition(Owner.GetPlanarPresence().PresencePlaneIds,
					PlanarPresenceDefinition.RetainPlaneSpecificInventory, !PlanarPresenceDefinition.PropagatesInventory);
				break;
			case IBody body when body is ConcreteBody concrete:
				concrete.EjectInventoryForPlanarTransition(Owner.GetPlanarPresence().PresencePlaneIds,
					PlanarPresenceDefinition.RetainPlaneSpecificInventory, !PlanarPresenceDefinition.PropagatesInventory);
				break;
		}
	}

	public override void ExpireEffect()
	{
		var before = PlanarVisibilityEchoHelper.CaptureVisibleObservers(Owner);
		ClearRetainedItemVisibilityAfterRemoval();
		Owner.RemoveEffect(this);
		PlanarVisibilityEchoHelper.EchoVisibilityChanges(Owner, before);
	}

	public override void RemovalEffect() => ClearRetainedItemVisibilityAfterRemoval();

	private void ClearRetainedItemVisibilityAfterRemoval()
	{
		var remainingPlanes = Owner.GetPlanarPresence(this).PresencePlaneIds;
		if (Owner is ICharacter character && character.Body is ConcreteBody body)
			body.EjectInventoryForPlanarTransition(remainingPlanes, ejectInventory: false);
		else if (Owner is IBody bodyInterface && bodyInterface is ConcreteBody concrete)
			concrete.EjectInventoryForPlanarTransition(remainingPlanes, ejectInventory: false);
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return new PlanarStateEffect(newItem, PlanarPresenceDefinition, PlanarPriority, OverridesBasePlanarPresence,
			ApplicabilityProg);
	}
}
