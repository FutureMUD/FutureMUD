#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.Planes;
namespace MudSharp.Magic;

public static class ArmageddonDrawWineStock
{
	public const string Key = "arm.spell.draw_wine";
	public const string Name = "Draw Wine";
	public const double MinimumEnergy = 7;
	public const string Litres = "0.5*grade";
	internal const string EligibilitySource = "return lowercase(@caster.location.terrain.name) != \"fire plane\" and lowercase(@caster.location.terrain.name) != \"silt\"";
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, ILiquid wine, IPlane? bonusPlane = null)
	{
		if (world.Liquids.Get(wine.Id) != wine || bonusPlane is not null && world.Planes.Get(bonusPlane.Id) != bonusPlane)
			throw new InvalidOperationException("Select an existing wine liquid and an optional Water bonus plane.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, Name,
			"Draw the first matching authored wine recipe into an accessible open native drink container, clamped to capacity. Source adds five units per grade, mapped to 0.5 litres per grade with an authored 0.1-litre unit; a selected Water plane doubles it. Edit effect recipe orders and boolean(character) predicates to map tribe 24 Horta, tribe 14 Gloth, Templar Ocotillo, tribe 53 spice brandy, tribe 9 Badu, tribe 45 Jik, tribe 62 spiced Ginka and default wine in that source precedence. Native guild/tribe identities and liquid profiles must be authored. Initial fallback is the selected wine. Fire Plane and Silt terrain refuse; incompatible nonempty contents refuse before payment in the approved safe adaptation of historical slime conversion. Builders may explicitly allow other compatible liquids. Ordinary consumption and persistence apply with no magical expiry; ground, direct intoxication and device routes are separate. Edit recipes, litres, compatibility, bonus plane and terrain prog normally. Sorcerer opening 30, cap 90, source minimum 7; add acquisition separately.",
			"0", MinimumEnergy, "$0 draw|draws wine into $1.",
			(r, c, f) => Definition(r, c, f, wine.Id, bonusPlane?.Id), EligibilitySource, ProgVariableTypes.Item);
	}
	internal static XElement Definition(long resource, long cost, long filter, long wine, long? plane) =>
		ArmageddonUtilityStock.Definition(Key, "item", resource, cost, filter, 30, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "createliquid"), new XElement("LiquidId", wine),
				new XElement("AmountFormula", Litres), new XElement("ContainerFill", new XAttribute("version", 1),
					new XElement("Litres", Litres), plane is { } id ? new XElement("BonusPlane", new XAttribute("multiplier", 2), id) : null,
					new XElement("Recipes", new XAttribute("version", 1), new XElement("Recipe", new XAttribute("order", 32),
						new XAttribute("predicate", 0), new XAttribute("liquid", wine))))));
}
