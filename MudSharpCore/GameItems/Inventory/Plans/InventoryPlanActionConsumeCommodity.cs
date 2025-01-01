using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionConsumeCommodity : InventoryPlanAction
{
	public double Weight { get; set; }
	public ISolid Material { get; set; }

	public InventoryPlanActionConsumeCommodity(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.ConsumeCommodity)
	{
		Weight = int.Parse(root.Attribute("weight").Value);
		Material = Gameworld.Materials.Get(long.Parse(root.Attribute("material").Value));
	}

	public InventoryPlanActionConsumeCommodity(IFuturemud gameworld, double quantity, ISolid solid, long primaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector)
		: base(gameworld, DesiredItemState.ConsumeCommodity, primaryTag, 0, primaryselector, secondaryselector)
	{
		Weight = quantity;
		Material = solid;
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "consumecommodity"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("weight", Weight),
			new XAttribute("material", Material.Id),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? "")
		);
	}
	#endregion

	public override string Describe(ICharacter voyeur)
	{
		return
			$"Consume {Gameworld.UnitManager.Describe(Weight, UnitType.Mass, voyeur).ColourValue()} of {Material.Name.Colour(Material.ResidueColour)} commodity{(DesiredTag is not null ? $" tagged as {DesiredTag.Name}" : "")}";
	}

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		return null;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		IGameItem item = null;

		// Already held items next
		item =
			executor.Body.HeldItems
			        .SelectNotNull(x => x.GetItemType<ICommodity>())
			        .FirstOrDefault(x => x.Material == Material && (DesiredTag is null && x.Tag is null) || (x.Tag?.IsA(DesiredTag) == true) && x.Weight >= Weight)
			        ?.Parent;
		if (item != null)
		{
			return item;
		}

		// Commodities can't be wielded

		// Commodities can't be worn

		// Commodities can't be attached to things

		// Commodities can't be sheathed

		// In containers in inventory
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .SelectNotNull(x => x.GetItemType<ICommodity>())
			        .FirstOrDefault(x => 
				        x.Material == Material &&
						(DesiredTag is null && x.Tag is null) || (x.Tag?.IsA(DesiredTag) == true) &&
				        x.Weight >= Weight)
			        ?.Parent;
		if (item != null)
		{
			return item;
		}

		// In location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer)
			        .SelectNotNull(x => x.GetItemType<ICommodity>())
			        .FirstOrDefault(x => x.Material == Material && (DesiredTag is null && x.Tag is null) || (x.Tag?.IsA(DesiredTag) == true) && x.Weight >= Weight)
			        ?.Parent;
		if (item != null)
		{
			return item;
		}


		// In containers in location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .SelectNotNull(x => x.GetItemType<ICommodity>())
			        .FirstOrDefault(x => x.Material == Material && (DesiredTag is null && x.Tag is null) || (x.Tag?.IsA(DesiredTag) == true) && x.Weight >= Weight)
			        ?.Parent;
		return item;
	}
}