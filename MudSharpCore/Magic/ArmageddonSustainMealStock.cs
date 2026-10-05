#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;
namespace MudSharp.Magic;

public static class ArmageddonSustainMealStock
{
	public const string Key = "arm.spell.sustain_meal";
	public const string Name = "Sustain Meal";
	public const string LifetimeSeconds = "1350*grade";
	public const double MinimumEnergy = 7;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, IReadOnlyList<IGameItemProto> defaultPool)
	{
		if (world.SpellOwnedItems is null || defaultPool.Count is < 1 or > 32 || defaultPool.Select(x => x.Id).Distinct().Count() != defaultPool.Count ||
			defaultPool.Any(food => NativeItemCreationEligibility.Error(food, world) is not null || food.GetItemType<FoodGameItemComponentProto>() is null))
			throw new InvalidOperationException("Select an approved plain native food prototype and enable spell-owned item lifecycle services.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, Name,
			"Create one edible food item per grade in the caster's room, drawing each independently from the first matching authored food profile. The initial fallback has the three selected approved foods; edit effect foodprofile orders and boolean(character) predicates to map Silt, Plains Ox, Halfling, Templar, Water Elementalist, tribe 24, Defiler and default in that source precedence. Historical object IDs and choice rules are known; native identity mappings and those objects' nutrition are unavailable and must be authored. Each output expires independently after 1350*grade real seconds, converted from source level*30*60 EVENT units at nominal 0.75 seconds per unit. Native deadlines omit historical batching and load delay. Eating consumes actual bites and nutrition; custody cannot reset expiry and consumed food is never recreated. Edit pools, count and lifetime normally. Potion/scroll satiation and device routes are separate. Sorcerer opening 30, cap 90, source minimum 7; add acquisition separately.",
			LifetimeSeconds, MinimumEnergy, "$0 summon|summons food into the room.",
			(r, c, f) => Definition(r, c, defaultPool.Select(x => x.Id).ToArray()));
	}
	internal static XElement Definition(long resource, long cost, params long[] prototypes) =>
		ArmageddonUtilityStock.Definition(Key, "room", resource, cost, 0, 30, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", "base"),
				new XElement("ItemPrototypeId", prototypes[0]), new XElement("ItemSkinId", 0), new XElement("Quantity", 1),
				new XElement("LoadString", ""), new XElement("Lifecycle", new XAttribute("version", 1),
					new XAttribute("mode", "TemporaryCleanup"), new XElement("Family", "sustain-meal"),
					new XElement("Seconds", LifetimeSeconds), new XElement("Count", "grade"),
					new XElement("FoodProfiles", new XAttribute("version", 1), new XElement("Profile", new XAttribute("order", 32),
						new XAttribute("predicate", 0), prototypes.Select(id => new XElement("Prototype", id)))))));
}
