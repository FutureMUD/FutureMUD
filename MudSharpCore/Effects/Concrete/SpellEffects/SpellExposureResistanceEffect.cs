#nullable enable
using MudSharp.Body;
using MudSharp.Form.Material;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>Ordinary spell/substance protection, independent of wetness and physical transmission.</summary>
public sealed class SpellExposureResistanceEffect : SimpleSpellStatusEffectBase, IExposureResistance
{
	public static void InitialiseEffectType() => RegisterFactory("SpellExposureResistance", (xml, owner) => new SpellExposureResistanceEffect(xml, owner));
	protected override string SpecificEffectType => "SpellExposureResistance";
	public double Multiplier { get; set; }
	public ExposureRoute Routes { get; }
	public string Category { get; }
	public long PartId { get; }

	public SpellExposureResistanceEffect(IPerceivable owner, IMagicSpellEffectParent parent, double multiplier,
		ExposureRoute routes, string category, long partId) : base(owner, parent)
	{
		Multiplier = multiplier; Routes = routes; Category = category; PartId = partId;
	}
	private SpellExposureResistanceEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var xml = root.Element("Effect")!;
		Multiplier = (double?)xml.Element("Multiplier") ?? 1.0;
		Routes = (ExposureRoute)((int?)xml.Element("Routes") ?? 0);
		Category = (string?)xml.Element("Category") ?? "*";
		PartId = (long?)xml.Element("Part") ?? 0;
	}
	protected override XElement SaveDefinition() => SimpleSaveDefinition(new XElement("Multiplier", Multiplier),
		new XElement("Routes", (int)Routes), new XElement("Category", Category), new XElement("Part", PartId));
	public double ExposureDamageMultiplier(ExposureDamageContext context, IBodypart? part) =>
		Applies() && (ParentEffect is not SubstanceExposureEffect { SourceBodyId: > 0 } substance || substance.SourceBodyId == context.TargetBodyId) &&
		Routes.HasFlag(context.Route) && (PartId == 0 || part?.Id == PartId) &&
		(Category == "*" || Category.Equals(context.Category, StringComparison.OrdinalIgnoreCase)) && ExposureArithmetic.Valid(Multiplier)
			? Math.Clamp(Multiplier, 0.0, 100.0) : 1.0;
	public override string Describe(IPerceiver voyeur) => $"Exposure protection: {Routes.DescribeEnum()}, category {Category}, " +
		$"{(PartId == 0 ? "whole target" : $"part #{PartId}")}, damage multiplier {Multiplier.ToString("N3", voyeur)}.";
}
