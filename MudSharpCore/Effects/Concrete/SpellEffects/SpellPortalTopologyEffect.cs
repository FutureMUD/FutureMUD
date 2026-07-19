#nullable enable

using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPortalTopologyEffect : MagicSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPortalTopology", (effect, owner) => new SpellPortalTopologyEffect(effect, owner));
	}

	public SpellPortalTopologyEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		IEnumerable<long> createdEndpointIds, IEnumerable<long> createdLinkIds, bool permanent)
		: base(owner, parent, null)
	{
		CreatedEndpointIds = createdEndpointIds.ToList();
		CreatedLinkIds = createdLinkIds.ToList();
		Permanent = permanent;
	}

	private SpellPortalTopologyEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		Permanent = bool.Parse(trueRoot?.Element("Permanent")?.Value ?? "false");
		CreatedEndpointIds = trueRoot?.Element("Endpoints")?.Elements("Endpoint")
			.Select(x => long.Parse(x.Value))
			.ToList() ?? [];
		CreatedLinkIds = trueRoot?.Element("Links")?.Elements("Link")
			.Select(x => long.Parse(x.Value))
			.ToList() ?? [];
	}

	public IReadOnlyList<long> CreatedEndpointIds { get; }
	public IReadOnlyList<long> CreatedLinkIds { get; }
	public bool Permanent { get; }

	public override void RemovalEffect()
	{
		if (!Permanent)
		{
			new MagicPortalTopologyService().DeleteSpellCreatedTopology(Gameworld, CreatedEndpointIds, CreatedLinkIds);
		}

		base.RemovalEffect();
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("Permanent", Permanent),
			new XElement("Endpoints", CreatedEndpointIds.Select(x => new XElement("Endpoint", x))),
			new XElement("Links", CreatedLinkIds.Select(x => new XElement("Link", x)))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return Permanent
			? "Maintaining a permanent portal topology creation marker."
			: $"Maintaining spell-owned portal topology ({CreatedEndpointIds.Count.ToString("N0", voyeur)} endpoints, {CreatedLinkIds.Count.ToString("N0", voyeur)} links).";
	}

	protected override string SpecificEffectType => "SpellPortalTopology";
}
