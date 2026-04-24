using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Planes;

namespace MudSharp.Effects.Concrete;

public class DrugInducedPlanarStateEffect : Effect, IPlanarOverlayEffect
{
	public DrugInducedPlanarStateEffect(IBody owner, PlanarPresenceDefinition definition) : base(owner)
	{
		BodyOwner = owner;
		PlanarPresenceDefinition = definition;
	}

	public IBody BodyOwner { get; }
	public PlanarPresenceDefinition PlanarPresenceDefinition { get; private set; }
	public int PlanarPriority => 50;
	public bool OverridesBasePlanarPresence => true;

	public void UpdateDefinition(PlanarPresenceDefinition definition)
	{
		PlanarPresenceDefinition = definition;
		if (!definition.PropagatesInventory && BodyOwner is MudSharp.Body.Implementations.Body concrete)
		{
			concrete.EjectInventoryForPlanarTransition();
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Drug-induced planar state: {PlanarPresenceDefinition.Describe(Gameworld)}.";
	}

	public override void InitialEffect()
	{
		if (!PlanarPresenceDefinition.PropagatesInventory && BodyOwner is MudSharp.Body.Implementations.Body concrete)
		{
			concrete.EjectInventoryForPlanarTransition();
		}
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return new DrugInducedPlanarStateEffect(BodyOwner, PlanarPresenceDefinition);
	}

	protected override string SpecificEffectType => "DrugInducedPlanarState";
}
