#nullable enable

using MudSharp.Character;

namespace MudSharp.GameItems.Inventory.Plans;

/// <summary>Requires material presence without moving, splitting or consuming the selected item.</summary>
public sealed class InventoryPlanActionRetain : InventoryPlanActionConsume
{
	public InventoryPlanActionRetain(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		DesiredState = DesiredItemState.Retained;
	}

	public override XElement SaveToXml()
	{
		var result = base.SaveToXml();
		result.SetAttributeValue("state", "retained");
		return result;
	}

	public override string Describe(ICharacter voyeur) => "Retain " + base.Describe(voyeur)[8..];
}
