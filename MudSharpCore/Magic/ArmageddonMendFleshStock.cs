#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;
namespace MudSharp.Magic;

public static class ArmageddonMendFleshStock
{
	public const string Key="arm.spell.mend_flesh";
	public const string Name="Mend Flesh";
	public const double MinimumEnergy=20;
	public const string HealingAmount="2*grade*grade";
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource, IFutureProg eligibility)=>
		ArmageddonUtilityStock.Create(world,school,trait,resource,Name,
			"Mend eligible wounds worst first with a deterministic damage budget of twice grade squared. The source restores grade squared times random one-to-three hit points, rejects undead, and excludes Nilaz except defilers; Fire and Water terrain alter source level. Select and edit a boolean (target, caster) eligibility prog to map those character categories in your world. Native wound budgets use the source mean without terrain level shifts; this is deliberate engine adaptation. Configure this Sorcerer entry opening 30, cap 60, relative grades. Practice can reach grade seven at cap 60; no practice maximum is authored. Add acquisition separately.",
			"0",MinimumEnergy,"$0 knit|knits $1's wounded flesh.",Definition,existingFilter:eligibility);
	internal static XElement Definition(long resource,long cost,long filter)=>ArmageddonUtilityStock.Definition(Key,"character",resource,cost,filter,30,MinimumEnergy,
		new XElement("Effect",new XAttribute("type","heal"),new XElement("HealWorstWoundsFirst",true),new XElement("HealOverflow",true),new XElement("HealingAmount",HealingAmount)));
}
