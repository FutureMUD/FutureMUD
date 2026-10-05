#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Construction;

namespace MudSharp.Magic;

/// <summary>Editable source content; capability admission and acquisition are authored separately.</summary>
public static class ArmageddonSeeTheUnbodiedStock
{
	public const string Key = "arm.spell.see_the_unbodied";
	public const string Name = "See the Unbodied";
	public const string PrerequisiteKey = ArmageddonPierceConcealmentStock.Key;
	public const double PrerequisiteRaw = 80;
	public const int Opening = 30;
	public const double RawCap = 90;
	public const double BranchRaw = 80;
	public const double MinimumEnergy = 7;
	public const string LifetimeSeconds = "1800*grade";
	public const string LifetimeGroup = "armageddon.detect_ethereal";
	public const int UnitSeconds = 600;
	public const int MaximumUnits = 36;
	public const string ComponentReference = "armageddon.see_unbodied.divination";

	// Reuse the recovered minimum-Standing posture mapping and native positionid query.
	// The character trigger supplies the existing editable filter hook; target must be self.
	internal static readonly string EligibilitySource = "if (@target != @caster)\nreturn false\nend if\n" +
		ArmageddonWaterBreathingStock.EligibilitySource;

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, ITerrain silt, ITerrain shadow, IReadOnlyList<ITag> divinationRanks)
	{
		ArgumentNullException.ThrowIfNull(divinationRanks);
		if (silt is null || shadow is null || silt.Id <= 0 || shadow.Id <= 0 ||
			!ReferenceEquals(world.Terrains.Get(silt.Id), silt) || !ReferenceEquals(world.Terrains.Get(shadow.Id), shadow))
			throw new InvalidOperationException("Select existing native Silt and Shadow terrain definitions explicitly.");
		if (divinationRanks.Count != 5 || divinationRanks.Any(x => x is null || x.Id <= 0 || !ReferenceEquals(world.Tags.Get(x.Id), x)) ||
			divinationRanks.Select(x => x.Id).Distinct().Count() != 5 ||
			divinationRanks.Skip(1).Where((tag, index) => !tag.IsA(divinationRanks[index])).Any())
			throw new InvalidOperationException("Select five distinct native Divination rank tags, zero through four, with each higher rank a descendant of the previous rank.");
		var ranks = divinationRanks.Select(x => x.Id).ToArray();
		return ArmageddonUtilityStock.Create(world, school, trait, resource, Name,
			"Open your senses to eligible ethereal entities through native perception. Historical Detect Ethereal is self-only, refuses mapped Silt before mapped Shadow's component exemption, and adds three source hours per grade to remaining whole source hours up to 36 while retaining the strongest source grade. Each source hour is 600 real seconds; exact deadlines and saved remaining time offline are native adaptations. Outside mapped Shadow consume one directly carried Divination quantity unit of rank at least grade minus three, floored at zero, including grades one to three. Native quantity-unit consumption adapts historical whole-object extraction; higher descendants qualify. Edit the source terrain IDs, dedicated component reference, rank hierarchy and accumulated lifetime through the normal spell builders. The editable eligibility prog admits only self and the existing minimum-Standing mapping: standing variants and active movement including swimming, flying and riding; resting postures and combat refuse. Native character-target syntax requires explicit self; historical omitted-target self is not provided by this filter-bearing trigger. No opposed save, duration draw, blindness removal or unrestricted planar sight is supplied. Source setting guild/race component bypasses are not inferred. Pierce Concealment raw80 opens this spell at30, raw cap90, branches80, printed minimum energy7. Capability admission and acquisition remain separate; charged and area delivery are unsupported.",
			LifetimeSeconds, MinimumEnergy, "$0 open|opens $0's senses to the unbodied.",
			(r, c, f) => Definition(r, c, f, silt.Id, shadow.Id, ranks), EligibilitySource);
	}

	internal static XElement Definition(long resource, long cost, long filter, long silt, long shadow, IReadOnlyList<long> ranks)
	{
		var definition = ArmageddonUtilityStock.Definition(Key, "character", resource, cost, filter, Opening, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "detectethereal"),
				SpellEffects.DetectInvisibleEffect.WritePolicy(new(LifetimeGroup, UnitSeconds, MaximumUnits)),
				new XElement("SourceScope", new XAttribute("version", 1), new XAttribute("silt", silt),
					new XAttribute("shadow", shadow), new XAttribute("component", ComponentReference))));
		definition.Element("Plan")!.Add(new XElement("Phase", new XElement("Action", new XAttribute("state", "consumed"),
			new XAttribute("tag", ranks[0]), new XAttribute("secondtag", 0), new XAttribute("quantity", 1),
			new XAttribute("carriedonly", true), new XAttribute("originalreference", ComponentReference),
			new XElement("GradeRank", new XAttribute("offset", -3), ranks.Select((tag, rank) =>
				new XElement("Rank", new XAttribute("minimum", rank), new XAttribute("tag", tag)))))));
		return definition;
	}
}
