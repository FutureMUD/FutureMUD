#nullable enable
using System.Xml.Linq;
using MudSharp.FutureProg;
namespace MudSharp.Magic;

/// <summary>Reviewed partial stock contributions; no database writes or runtime initialization.</summary>
public static class ArmageddonReviewedUtilityContent
{
	public const string SenseEnchantmentKey = "arm.spell.sense_enchantment";
	public const string SenseEnchantmentName = "Sense Enchantment";
	public const string SenseEnchantmentLifetimeSeconds = "3000*grade";
	public const double SenseEnchantmentMinimumEnergy = 7;
	public const string SenseEnchantmentEligibilitySource = "return lowercase(@caster.location.terrain.name) != \"silt\"";
	public static ArmageddonUtilitySpellContent SenseEnchantment() => new(SenseEnchantmentKey, SenseEnchantmentName,
		"Reveal perceptible native magical effects while detection lasts. Source detect_magick refuses Silt and lasts five game hours per grade. Native policy refreshes one exclusive detection parent for 3000 real seconds per grade instead of accumulating the source's 48-hour stack. Edit the support prog for your terrain names. Sorcerer opening 60, cap 90; add acquisition separately.",
		SenseEnchantmentLifetimeSeconds, SenseEnchantmentMinimumEnergy, "$0 open|opens $0's senses to enchantment.", SenseEnchantmentDefinition, SenseEnchantmentEligibilitySource);
	private static XElement SenseEnchantmentDefinition(long resource,long cost,long filter)=>ArmageddonUtilitySpellContent.Definition(SenseEnchantmentKey,"character",resource,cost,filter,60,SenseEnchantmentMinimumEnergy,new XElement("Effect",new XAttribute("type","detectmagick")));

	public const string UnravelEnchantmentKey = "arm.spell.unravel_enchantment";
	public const string UnravelEnchantmentName = "Unravel Enchantment";
	public const double UnravelEnchantmentMinimumEnergy = 7;
	public static ArmageddonUtilitySpellContent UnravelEnchantment() => new(UnravelEnchantmentKey, UnravelEnchantmentName,
		"Contest and remove matching native spell-owned effects from a character. Edit dispel effect keys, caster policy, school and contest bonus normally. This completion-brief adaptation replaces the source's heterogeneous expiration shortening, object extraction and permanent flag removal with the native matching-key contest. It does not reproduce those wider operations. Sorcerer opening 60, cap 90; add acquisition separately.",
		"0", UnravelEnchantmentMinimumEnergy, "$0 unravel|unravels the enchantments around $1.", UnravelEnchantmentDefinition);
	private static XElement UnravelEnchantmentDefinition(long resource,long cost,long filter)=>ArmageddonUtilitySpellContent.Definition(UnravelEnchantmentKey,"character",resource,cost,filter,60,UnravelEnchantmentMinimumEnergy,
		new XElement("Effect",new XAttribute("type","dispelmagic"),new XElement("Mode",0),new XElement("CasterPolicy",1),new XElement("AllowHostile",true),new XElement("EffectKey","any"),new XElement("Contest",true)));

	public const string MendFleshKey = "arm.spell.mend_flesh";
	public const string MendFleshName = "Mend Flesh";
	public const double MendFleshMinimumEnergy = 20;
	public const string MendFleshHealingAmount = "2*grade*grade";
	public static ArmageddonUtilitySpellContent MendFlesh() => new(MendFleshKey, MendFleshName,
		"Mend eligible wounds worst first with a deterministic damage budget of twice grade squared. The source restores grade squared times random one-to-three hit points, rejects undead, and excludes Nilaz except defilers; Fire and Water terrain alter source level. Select and edit a boolean (target, caster) eligibility prog to map those character categories in your world. Native wound budgets use the source mean without terrain level shifts; this is deliberate engine adaptation. Configure this Sorcerer entry opening 30, cap 60, relative grades. Practice can reach grade seven at cap 60; no practice maximum is authored. Add acquisition separately.",
		"0", MendFleshMinimumEnergy, "$0 knit|knits $1's wounded flesh.", MendFleshDefinition);
	private static XElement MendFleshDefinition(long resource,long cost,long filter)=>ArmageddonUtilitySpellContent.Definition(MendFleshKey,"character",resource,cost,filter,30,MendFleshMinimumEnergy,
		new XElement("Effect",new XAttribute("type","heal"),new XElement("HealWorstWoundsFirst",true),new XElement("HealOverflow",true),new XElement("HealingAmount",MendFleshHealingAmount)));

	public const string DrawWaterKey = "arm.spell.draw_water";
	public const string DrawWaterName = "Draw Water";
	public const double DrawWaterMinimumEnergy = 7;
	public const string DrawWaterLitres = "0.5*grade";
	public const string DrawWaterEligibilitySource = "return lowercase(@caster.location.terrain.name) != \"fire plane\" and lowercase(@caster.location.terrain.name) != \"silt\"";
	public static ArmageddonUtilitySpellContent DrawWater(long water, long? plane = null) => new(DrawWaterKey, DrawWaterName,
		"Draw clean water into an accessible open native drink container, clamped to its remaining capacity. Source five units per grade are mapped to half a litre per grade; a selected Water plane doubles that amount. Fire Plane and Silt terrain refuse. Native policy refuses incompatible contents before payment instead of converting them to source slime; there is no ground puddle or character-thirst route. Created water has no magical expiry and uses ordinary liquid consumption and persistence. Edit liquid, litres, compatible liquids, bonus plane and the terrain prog normally. Sorcerer opening 30, cap 90; add acquisition separately.",
		"0", DrawWaterMinimumEnergy, "$0 draw|draws clear water into $1.", (r,c,f) => DrawWaterDefinition(r,c,f,water,plane), DrawWaterEligibilitySource, ProgVariableTypes.Item);
	private static XElement DrawWaterDefinition(long resource,long cost,long filter,long water,long? plane)=>ArmageddonUtilitySpellContent.Definition(DrawWaterKey,"item",resource,cost,filter,30,DrawWaterMinimumEnergy,
		new XElement("Effect",new XAttribute("type","createliquid"),new XElement("LiquidId",water),new XElement("AmountFormula",DrawWaterLitres),
			new XElement("ContainerFill",new XAttribute("version",1),new XElement("Litres",DrawWaterLitres),plane is {} id?new XElement("BonusPlane",new XAttribute("multiplier",2),id):null)));

	public const string HoveringLightKey = "arm.spell.hovering_light";
	public const string HoveringLightName = "Hovering Light";
	public const string HoveringLightLifetimeSeconds = "1800*grade";
	public const double HoveringLightMinimumEnergy = 7;
	public static ArmageddonUtilitySpellContent HoveringLight(long prototype) => new(HoveringLightKey, HoveringLightName,
		"Create one temporary native light in the recipient's configured wear profile. Source ball_of_light lasts three game hours per grade, mapped to 1800 real seconds per grade. Select your own approved wearable prog-light; historical guild/tribe light prototypes and luminosities are unavailable. Native admission requires an available wear slot instead of the source's ground fallback. Native custody changes do not reset its immutable cleanup deadline; taking it off does not emulate the source consumes flag. Edit prototype, lifetime and placement normally. Sorcerer opening 30, cap 90; add acquisition separately.",
		HoveringLightLifetimeSeconds, HoveringLightMinimumEnergy, "$0 shape|shapes a hovering light around $1.", (r,c,f) => HoveringLightDefinition(r,c,f,prototype));
	private static XElement HoveringLightDefinition(long resource,long cost,long filter,long prototype)=>ArmageddonUtilitySpellContent.Definition(HoveringLightKey,"character",resource,cost,filter,30,HoveringLightMinimumEnergy,
		new XElement("Effect",new XAttribute("type","createitem"),new XElement("ItemQuality","base"),new XElement("ItemPrototypeId",prototype),new XElement("ItemSkinId",0),new XElement("Quantity",1),new XElement("LoadString",""),
			new XElement("Lifecycle",new XAttribute("version",1),new XAttribute("mode","TemporaryCleanup"),new XElement("Family","hovering-light"),new XElement("Seconds",HoveringLightLifetimeSeconds),new XElement("Placement","wornlight"))));
}
