#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
namespace MudSharp.Magic;

public static class ArmageddonUnravelEnchantmentStock
{
	public const string Key="arm.spell.unravel_enchantment";
	public const string Name="Unravel Enchantment";
	public const double MinimumEnergy=7;
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource)=>
		ArmageddonUtilityStock.Create(world,school,trait,resource,Name,
			"Contest and remove matching native spell-owned effects from a character. Edit dispel effect keys, caster policy, school and contest bonus normally. This completion-brief adaptation replaces the source's heterogeneous expiration shortening, object extraction and permanent flag removal with the native matching-key contest. It does not reproduce those wider operations. Sorcerer opening 60, cap 90; add acquisition separately.",
			"0",MinimumEnergy,"$0 unravel|unravels the enchantments around $1.",Definition);
	internal static XElement Definition(long resource,long cost,long filter)=>ArmageddonUtilityStock.Definition(Key,"character",resource,cost,filter,60,MinimumEnergy,
		new XElement("Effect",new XAttribute("type","dispelmagic"),new XElement("Mode",0),new XElement("CasterPolicy",1),new XElement("AllowHostile",true),new XElement("EffectKey","any"),new XElement("Contest",true)));
}
