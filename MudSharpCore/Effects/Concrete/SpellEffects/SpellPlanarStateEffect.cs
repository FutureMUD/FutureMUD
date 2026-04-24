using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Planes;
using System.Xml.Linq;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPlanarStateEffect : MagicSpellEffectBase, IPlanarOverlayEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPlanarState", (effect, owner) => new SpellPlanarStateEffect(effect, owner));
	}

	public SpellPlanarStateEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		PlanarPresenceDefinition definition, IFutureProg prog = null)
		: base(owner, parent, prog)
	{
		PlanarPresenceDefinition = definition;
	}

	private SpellPlanarStateEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		PlanarPresenceDefinition = PlanarPresenceDefinition.FromXml(root.Element("Effect")?.Element("PlanarData"), Gameworld);
	}

	public PlanarPresenceDefinition PlanarPresenceDefinition { get; }
	public int PlanarPriority => 100;
	public bool OverridesBasePlanarPresence => true;

	protected override string SpecificEffectType => "SpellPlanarState";

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			PlanarPresenceDefinition.SaveToXml());
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Altered planar state: {PlanarPresenceDefinition.Describe(Gameworld)}.";
	}

	public override void InitialEffect()
	{
		if (PlanarPresenceDefinition.PropagatesInventory)
		{
			return;
		}

		if (Owner is ICharacter character && character.Body is ConcreteBody body)
		{
			body.EjectInventoryForPlanarTransition();
			return;
		}

		if (Owner is IBody { } bodyInterface && bodyInterface is ConcreteBody concrete)
		{
			concrete.EjectInventoryForPlanarTransition();
		}
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return new SpellPlanarStateEffect(newItem, ParentEffect, PlanarPresenceDefinition, ApplicabilityProg);
	}
}
