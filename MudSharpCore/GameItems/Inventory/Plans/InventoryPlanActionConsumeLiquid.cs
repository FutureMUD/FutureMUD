using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanActionConsumeLiquid : InventoryPlanAction
{
	public InventoryPlanActionConsumeLiquid(XElement root, IFuturemud gameworld)
		: base(root, gameworld, DesiredItemState.ConsumeLiquid)
	{
		LiquidToTake = new LiquidMixture(root.Element("Mix"), gameworld);
		LiquidSelector = mixture => LiquidToTake.Instances.All(x => mixture.CanRemoveLiquid(LiquidToTake));
	}

	public InventoryPlanActionConsumeLiquid(IFuturemud gameworld, long primaryTag, long secondaryTag,
		Func<IGameItem, bool> primaryselector, Func<IGameItem, bool> secondaryselector,
		Func<LiquidMixture, bool> liquidSelector, LiquidMixture liquidToTake)
		: base(gameworld, DesiredItemState.ConsumeLiquid, primaryTag, secondaryTag, primaryselector, secondaryselector)
	{
		LiquidSelector = liquidSelector;
		LiquidToTake = liquidToTake;
	}

	#region Overrides of InventoryPlanAction

	public override XElement SaveToXml()
	{
		return new XElement("Action",
			new XAttribute("state", "consumeliquid"),
			new XAttribute("tag", DesiredTag?.Id ?? 0),
			new XAttribute("secondtag", DesiredSecondaryTag?.Id ?? 0),
			new XAttribute("inplaceoverride", ItemsAlreadyInPlaceOverrideFitnessScore),
			new XAttribute("inplacemultiplier", ItemsAlreadyInPlaceMultiplier),
			new XAttribute("originalreference", OriginalReference?.ToString() ?? ""),
			LiquidToTake.SaveToXml()
		);
	}

	public override string Describe(ICharacter voyeur)
	{
		return
			$"Consume {LiquidToTake.ColouredLiquidDescription} from {DesiredTag?.Name.A_An_RespectPlurals(colour: Telnet.Cyan) ?? "an item"}";
	}

	#endregion

	public LiquidMixture LiquidToTake { get; }

	public Func<LiquidMixture, bool> LiquidSelector { get; }

	public override IGameItem ScoutSecondary(ICharacter executor, IGameItem item)
	{
		return null;
	}

	public override IGameItem ScoutTarget(ICharacter executor)
	{
		IGameItem item = null;

		// Already held items next
		item =
			executor.Body.HeldItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					x.GetItemType<IOpenable>()?.IsOpen != false &&
					LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// Wielded items next
		item =
			executor.Body.WieldedItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					x.GetItemType<IOpenable>()?.IsOpen != false &&
					LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// Worn items next
		item =
			executor.Body.WornItems.FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					x.GetItemType<IOpenable>()?.IsOpen != false &&
					LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// Attached to worn items next
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IBelt>())
			        .Select(
				        x =>
					        x.ConnectedItems.FirstOrDefault(
						         y =>
							         y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							         y.Parent.GetItemType<IOpenable>()?.IsOpen != false &&
							         LiquidSelector.Invoke(y.Parent.GetItemType<ILiquidContainer>()?.LiquidMixture))
					         ?.Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// Sheathed next
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.GetItemType<IOpenable>()?.IsOpen != false &&
					        LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// In containers in inventory
		item =
			executor.Inventory.SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.GetItemType<IOpenable>()?.IsOpen != false &&
					        LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// In location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).FirstOrDefault(
				x =>
					x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					x.GetItemType<IOpenable>()?.IsOpen != false &&
					LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// Attached to room items next
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IBelt>())
			        .Select(
				        x =>
					        x.ConnectedItems.FirstOrDefault(
						         y =>
							         y.Parent.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(y.Parent) ?? true) &&
							         y.Parent.GetItemType<IOpenable>()?.IsOpen != false &&
							         LiquidSelector.Invoke(y.Parent.GetItemType<ILiquidContainer>()?.LiquidMixture))
					         ?.Parent)
			        .FirstOrDefault(x => x != null);
		if (item != null)
		{
			return item;
		}

		// Sheathed in room item next
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<ISheath>())
			        .SelectNotNull(x => x.Content?.Parent)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.GetItemType<IOpenable>()?.IsOpen != false &&
					        LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		if (item != null)
		{
			return item;
		}

		// In containers in location
		item =
			executor.Location.LayerGameItems(executor.RoomLayer).SelectNotNull(x => x.GetItemType<IContainer>())
			        .Where(x => x.Parent.GetItemType<IOpenable>()?.IsOpen ?? true)
			        .SelectMany(x => x.Contents)
			        .FirstOrDefault(
				        x =>
					        x.IsA(DesiredTag) && (PrimaryItemSelector?.Invoke(x) ?? true) &&
					        x.GetItemType<IOpenable>()?.IsOpen != false &&
					        LiquidSelector.Invoke(x.GetItemType<ILiquidContainer>()?.LiquidMixture));
		return item;
	}
}