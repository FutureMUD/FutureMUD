#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;
namespace MudSharp.Magic;

public static class ArmageddonHoveringLightStock
{
	public const string Key="arm.spell.hovering_light";
	public const string Name="Hovering Light";
	public const string LifetimeSeconds="1800*grade";
	public const double MinimumEnergy=7;
	public static MagicSpell Create(IFuturemud world,IMagicSchool school,ITraitDefinition trait,IMagicResource resource,IGameItemProto light)
	{
		if(world.SpellOwnedItems is null || NativeItemCreationEligibility.Error(light,world) is {} ||
			light.GetItemType<WearableGameItemComponentProto>() is null || light.GetItemType<ProgLightGameItemComponentProto>() is null)
			throw new InvalidOperationException("Select an approved plain native wearable prog-light prototype and enable spell-owned item lifecycle services.");
		return ArmageddonUtilityStock.Create(world,school,trait,resource,Name,
			"Create one temporary native light in the recipient's configured wear profile. Source ball_of_light lasts three game hours per grade, mapped to 1800 real seconds per grade. Select your own approved wearable prog-light; historical guild/tribe light prototypes and luminosities are unavailable. Native admission requires an available wear slot instead of the source's ground fallback. Native custody changes do not reset its immutable cleanup deadline; taking it off does not emulate the source consumes flag. Edit prototype, lifetime and placement normally. Sorcerer opening 30, cap 90; add acquisition separately.",
			LifetimeSeconds,MinimumEnergy,"$0 shape|shapes a hovering light around $1.",(r,c,f)=>Definition(r,c,f,light.Id));
	}
	internal static XElement Definition(long resource,long cost,long filter,long prototype)=>ArmageddonUtilityStock.Definition(Key,"character",resource,cost,filter,30,MinimumEnergy,
		new XElement("Effect",new XAttribute("type","createitem"),new XElement("ItemQuality","base"),new XElement("ItemPrototypeId",prototype),new XElement("ItemSkinId",0),new XElement("Quantity",1),new XElement("LoadString",""),
			new XElement("Lifecycle",new XAttribute("version",1),new XAttribute("mode","TemporaryCleanup"),new XElement("Family","hovering-light"),new XElement("Seconds",LifetimeSeconds),new XElement("Placement","wornlight"))));
}
