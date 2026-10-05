#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.Planes;
namespace MudSharp.Magic;

public static class ArmageddonDrawWaterStock
{
	public const string Key="arm.spell.draw_water";
	public const string Name="Draw Water";
	public const double MinimumEnergy=7;
	public const string Litres="0.5*grade";
	internal const string EligibilitySource="return lowercase(@caster.location.terrain.name) != \"fire plane\" and lowercase(@caster.location.terrain.name) != \"silt\"";
	public static MagicSpell Create(IFuturemud world,IMagicSchool school,ITraitDefinition trait,IMagicResource resource,ILiquid water,IPlane? bonusPlane=null)
	{
		if(world.Liquids.Get(water.Id)!=water || bonusPlane is not null && world.Planes.Get(bonusPlane.Id)!=bonusPlane)
			throw new InvalidOperationException("Select existing clean water and an optional water bonus plane.");
		return ArmageddonUtilityStock.Create(world,school,trait,resource,Name,
			"Draw clean water into an accessible open native drink container, clamped to its remaining capacity. Source five units per grade are mapped to half a litre per grade; a selected Water plane doubles that amount. Fire Plane and Silt terrain refuse. Native policy refuses incompatible contents before payment instead of converting them to source slime; there is no ground puddle or character-thirst route. Created water has no magical expiry and uses ordinary liquid consumption and persistence. Edit liquid, litres, compatible liquids, bonus plane and the terrain prog normally. Sorcerer opening 30, cap 90; add acquisition separately.",
			"0",MinimumEnergy,"$0 draw|draws clear water into $1.",(r,c,f)=>Definition(r,c,f,water.Id,bonusPlane?.Id),EligibilitySource,ProgVariableTypes.Item);
	}
	internal static XElement Definition(long resource,long cost,long filter,long water,long? plane)=>ArmageddonUtilityStock.Definition(Key,"item",resource,cost,filter,30,MinimumEnergy,
		new XElement("Effect",new XAttribute("type","createliquid"),new XElement("LiquidId",water),new XElement("AmountFormula",Litres),
			new XElement("ContainerFill",new XAttribute("version",1),new XElement("Litres",Litres),plane is {} id?new XElement("BonusPlane",new XAttribute("multiplier",2),id):null)));
}
