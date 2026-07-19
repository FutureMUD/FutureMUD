#nullable enable


namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPhantomIllusionEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, IIllusionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPhantomIllusion", (effect, owner) => new SpellPhantomIllusionEffect(effect, owner));
	}

	public SpellPhantomIllusionEffect(IPerceivable owner, IMagicSpellEffectParent parent, string text,
		IllusionAudienceScope audienceScope, long casterId, long targetId, long? clanId, IFutureProg? prog = null,
		IFutureProg? viewerProg = null, int priority = 0, string illusionKey = "", ANSIColour? colour = null)
		: base(owner, parent, prog)
	{
		IllusionText = text;
		AudienceScope = audienceScope;
		CasterId = casterId;
		TargetId = targetId;
		ClanId = clanId;
		ViewerProg = viewerProg;
		IllusionPriority = priority;
		IllusionKey = illusionKey;
		IllusionColour = colour;
	}

	private SpellPhantomIllusionEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		IllusionText = trueRoot?.Element("Text")?.Value ?? string.Empty;
		AudienceScope = Enum.TryParse<IllusionAudienceScope>(trueRoot?.Element("AudienceScope")?.Value, true,
			out var scope)
			? scope
			: IllusionAudienceScope.Everyone;
		CasterId = long.Parse(trueRoot?.Element("CasterId")?.Value ?? "0");
		TargetId = long.Parse(trueRoot?.Element("TargetId")?.Value ?? owner.Id.ToString());
		ClanId = long.TryParse(trueRoot?.Element("ClanId")?.Value, out var clanId) && clanId > 0L
			? clanId
			: null;
		ViewerProg = Gameworld.FutureProgs.Get(long.Parse(trueRoot?.Element("ViewerProg")?.Value ?? "0"));
		IllusionPriority = int.Parse(trueRoot?.Element("Priority")?.Value ?? "0");
		IllusionKey = trueRoot?.Element("IllusionKey")?.Value ?? string.Empty;
		var colourText = trueRoot?.Element("Colour")?.Value ?? string.Empty;
		IllusionColour = colourText.EqualTo("none") || string.IsNullOrWhiteSpace(colourText)
			? null
			: Telnet.GetColour(colourText);
	}

	public string IllusionText { get; }
	public IllusionAudienceScope AudienceScope { get; }
	public long CasterId { get; }
	public long TargetId { get; }
	public long? ClanId { get; }
	public IFutureProg? ViewerProg { get; }
	public int IllusionPriority { get; }
	public string IllusionKey { get; }
	public ANSIColour? IllusionColour { get; }
	public bool PlayerSet => false;

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		var text = IllusionText.SubstituteANSIColour().StripANSIColour(!colour);
		return colour && IllusionColour is not null ? text.Colour(IllusionColour) : text;
	}

	public bool DescriptionAdditionApplies(IPerceiver voyeur)
	{
		return IllusionApplies(voyeur) && !string.IsNullOrWhiteSpace(IllusionText);
	}

	public bool IllusionApplies(IPerceiver voyeur)
	{
		return IllusionAudiencePolicy.Applies(Owner, voyeur, AudienceScope, CasterId, TargetId, ClanId, ViewerProg) &&
		       Applies(voyeur);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magical phantom room illusion.";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("Text", new XCData(IllusionText)),
			new XElement("AudienceScope", AudienceScope.ToString()),
			new XElement("CasterId", CasterId),
			new XElement("TargetId", TargetId),
			new XElement("ClanId", ClanId ?? 0L),
			new XElement("ViewerProg", ViewerProg?.Id ?? 0L),
			new XElement("Priority", IllusionPriority),
			new XElement("IllusionKey", new XCData(IllusionKey)),
			new XElement("Colour", IllusionColour?.Name ?? "none")
		);
	}

	protected override string SpecificEffectType => "SpellPhantomIllusion";
}
