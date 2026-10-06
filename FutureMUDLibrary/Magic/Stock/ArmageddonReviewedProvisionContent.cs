#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Magic;

/// <summary>Reviewed pure provision contributions shared by native builders and the owned installer.</summary>
public static class ArmageddonReviewedProvisionContent
{
	public const string SustainMealKey = "arm.spell.sustain_meal";
	public const string SustainMealName = "Sustain Meal";
	public const string SustainMealLifetimeSeconds = "1350*grade";
	public const string DrawWineKey = "arm.spell.draw_wine";
	public const string DrawWineName = "Draw Wine";
	public const string DrawWineLitres = "0.5*grade";
	public const double MinimumEnergy = 7;
	public const string DrawWineEligibilitySource = "return lowercase(@caster.location.terrain.name) != \"fire plane\" and lowercase(@caster.location.terrain.name) != \"silt\"";
	public static ArmageddonUtilitySpellContent SustainMeal(IReadOnlyList<long> prototypes) => new(SustainMealKey, SustainMealName,
		"Create one edible food item per grade in the caster's room, drawing each independently from the first matching authored food profile. The initial fallback has the three selected approved foods; edit effect foodprofile orders and boolean(character) predicates to map Silt, Plains Ox, Halfling, Templar, Water Elementalist, tribe 24, Defiler and default in that source precedence. Historical object IDs and choice rules are known; native identity mappings and those objects' nutrition are unavailable and must be authored. Each output expires independently after 1350*grade real seconds, converted from source level*30*60 EVENT units at nominal 0.75 seconds per unit. Native deadlines omit historical batching and load delay. Eating consumes actual bites and nutrition; custody cannot reset expiry and consumed food is never recreated. Edit pools, count and lifetime normally. Potion/scroll satiation and device routes are separate. Sorcerer opening 30, cap 90, source minimum 7; add acquisition separately.",
		SustainMealLifetimeSeconds, MinimumEnergy, "$0 summon|summons food into the room.", (r,c,f) => SustainMealDefinition(r,c,prototypes.ToArray()));
	public static ArmageddonUtilitySpellContent DrawWine(long wine, long? plane = null) => new(DrawWineKey, DrawWineName,
		"Draw the first matching authored wine recipe into an accessible open native drink container, clamped to capacity. Source adds five units per grade, mapped to 0.5 litres per grade with an authored 0.1-litre unit; a selected Water plane doubles it. Edit effect recipe orders and boolean(character) predicates to map tribe 24 Horta, tribe 14 Gloth, Templar Ocotillo, tribe 53 spice brandy, tribe 9 Badu, tribe 45 Jik, tribe 62 spiced Ginka and default wine in that source precedence. Native guild/tribe identities and liquid profiles must be authored. Initial fallback is the selected wine. Fire Plane and Silt terrain refuse; incompatible nonempty contents refuse before payment in the approved safe adaptation of historical slime conversion. Builders may explicitly allow other compatible liquids. Ordinary consumption and persistence apply with no magical expiry; ground, direct intoxication and device routes are separate. Edit recipes, litres, compatibility, bonus plane and terrain prog normally. Sorcerer opening 30, cap 90, source minimum 7; add acquisition separately.",
		"0", MinimumEnergy, "$0 draw|draws wine into $1.", (r,c,f) => DrawWineDefinition(r,c,f,wine,plane), DrawWineEligibilitySource, ProgVariableTypes.Item);
	private static XElement SustainMealDefinition(long resource, long cost, params long[] prototypes) =>
		ArmageddonUtilitySpellContent.Definition(SustainMealKey, "room", resource, cost, 0, 30, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"),
				new XElement("ItemPrototypeId", prototypes[0]), new XElement("ItemSkinId", 0), new XElement("Quantity", 1),
				new XElement("LoadString", ""), new XElement("Lifecycle", new XAttribute("version", 1),
					new XAttribute("mode", "TemporaryCleanup"), new XElement("Family", "sustain-meal"),
					new XElement("Seconds", SustainMealLifetimeSeconds), new XElement("Count", "grade"),
					new XElement("FoodProfiles", new XAttribute("version", 1), new XElement("Profile", new XAttribute("order", 32),
						new XAttribute("predicate", 0), prototypes.Select(id => new XElement("Prototype", id)))))));
	private static XElement DrawWineDefinition(long resource, long cost, long filter, long wine, long? plane) =>
		ArmageddonUtilitySpellContent.Definition(DrawWineKey, "item", resource, cost, filter, 30, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "createliquid"), new XElement("LiquidId", wine),
				new XElement("AmountFormula", DrawWineLitres), new XElement("ContainerFill", new XAttribute("version", 1),
					new XElement("Litres", DrawWineLitres), plane is { } id ? new XElement("BonusPlane", new XAttribute("multiplier", 2), id) : null,
					new XElement("Recipes", new XAttribute("version", 1), new XElement("Recipe", new XAttribute("order", 32),
						new XAttribute("predicate", 0), new XAttribute("liquid", wine))))));
}
