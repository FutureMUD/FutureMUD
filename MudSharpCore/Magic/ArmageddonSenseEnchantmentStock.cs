#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
namespace MudSharp.Magic;

public static class ArmageddonSenseEnchantmentStock
{
	public const string Key="arm.spell.sense_enchantment";
	public const string Name="Sense Enchantment";
	public const string LifetimeSeconds="3000*grade";
	public const double MinimumEnergy=7;
	internal const string EligibilitySource="return lowercase(@caster.location.terrain.name) != \"silt\"";
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource) =>
		ArmageddonUtilityStock.Create(world,school,trait,resource,Name,
			"Reveal perceptible native magical effects while detection lasts. Source detect_magick refuses Silt and lasts five game hours per grade. Native policy refreshes one exclusive detection parent for 3000 real seconds per grade instead of accumulating the source's 48-hour stack. Edit the support prog for your terrain names. Sorcerer opening 60, cap 90; add acquisition separately.",
			LifetimeSeconds,MinimumEnergy,"$0 open|opens $0's senses to enchantment.",Definition,EligibilitySource);
	internal static XElement Definition(long resource,long cost,long filter)=>ArmageddonUtilityStock.Definition(Key,"character",resource,cost,filter,60,MinimumEnergy,new XElement("Effect",new XAttribute("type","detectmagick")));
}
