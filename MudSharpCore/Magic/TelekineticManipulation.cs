#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework.Units;

namespace MudSharp.Magic;

/// <summary>Prepares component operations without moving items or consuming resources during validation.</summary>
public static class TelekineticManipulation
{
	public static bool IsWithinMassLimit(double baseWeight, double baseWeightToKilograms, double maximumKilograms) =>
		double.IsFinite(baseWeight) && baseWeight >= 0.0 &&
		double.IsFinite(baseWeightToKilograms) && baseWeightToKilograms > 0.0 &&
		double.IsFinite(maximumKilograms) && maximumKilograms >= 0.0 &&
		baseWeight * baseWeightToKilograms <= maximumKilograms;

	public const string Syntax = "<item> get|move|open|close|switch <setting>|select <option>|empty [destination or amount]|pour <destination> [amount]|fill <source> [amount]|put <container>";

	public static bool TryPrepare(ICharacter actor, IGameItem item, string operation, StringStack arguments,
		Func<IGameItem, bool> eligible, out Func<bool> execute, out string error)
	{
		execute = () => false;
		error = "That object cannot be manipulated in that way.";
		if (!eligible(item)) return false;
		bool CanMove(IGameItem value) => value.GetItemType<IDoor>()?.InstalledExit is null &&
			value.CanGet(0) == ItemGetResponse.CanGet && value.Location?.CanGet(value, actor) == true;
		var openable = item.GetItemType<IOpenable>();
		switch (operation)
		{
			case "get":
				if (!arguments.IsFinished || !CanMove(item) || !actor.Body.CanGet(item, 0)) return false;
				execute = () => { actor.Body.Get(item); return true; };
				return true;
			case "move":
				if (!arguments.IsFinished || !CanMove(item)) return false;
				execute = () => { item.SetRoutePosition(actor.RoutePositionMetres); return true; };
				return true;
			case "open":
			case "close":
				if (!arguments.IsFinished || openable is null || (operation == "open" ? !openable.CanOpen(actor.Body) : !openable.CanClose(actor.Body))) return false;
				execute = () => { if (operation == "open") openable.Open(); else openable.Close(); return true; };
				return true;
			case "switch":
				var setting = arguments.SafeRemainingArgument;
				var switches = item.GetItemTypes<ISwitchable>().Where(x => x.SwitchSettings.Any(s => s.EqualTo(setting)) && x.CanSwitch(actor, setting)).ToList();
				if (string.IsNullOrWhiteSpace(setting) || switches.Count == 0) return false;
				execute = () => { var changed = false; foreach (var component in switches) changed |= component.Switch(actor, setting); return changed; };
				return true;
			case "select":
				var option = arguments.SafeRemainingArgument;
				var selectable = item.GetItemTypes<ISelectable>().FirstOrDefault(x => x.CanSelect(actor, option));
				if (string.IsNullOrWhiteSpace(option) || selectable is null) return false;
				execute = () => selectable.Select(actor, option, null, silent: true);
				return true;
			case "put":
				var destination = actor.TargetLocalItem(arguments.PopSpeech());
				var container = destination?.GetItemType<IContainer>();
				if (!arguments.IsFinished || destination is null || destination == item || !eligible(destination) ||
				    container is null || destination.GetItemType<IOpenable>()?.IsOpen == false || !CanMove(item) || !container.CanPut(item)) return false;
				execute = () =>
				{
					item.Location.Extract(item);
					container.Put(actor, item, allowMerge: false);
					item.InvokeInventoryChange(InventoryState.Dropped, InventoryState.InContainer);
					return true;
				};
				return true;
			case "empty" when item.GetItemType<ILiquidContainer>() is null:
				var source = item.GetItemType<IContainer>();
				var hasDestination = !arguments.IsFinished;
				var into = hasDestination ? actor.TargetLocalItem(arguments.PopSpeech()) : null;
				if (hasDestination && into is null) return false;
				if (!arguments.IsFinished || source is null || openable?.IsOpen == false || !CanMove(item) || !source.Contents.Any() ||
				    source.Contents.Any(x => !source.CanTake(actor, x, 0))) return false;
				if (into is not null && (into == item || !eligible(into) || into.GetItemType<IContainer>() is null || into.GetItemType<IOpenable>()?.IsOpen == false)) return false;
				execute = () => { source.Empty(actor, into?.GetItemType<IContainer>()!); return true; };
				return true;
			case "empty":
			case "pour":
			case "fill":
				var other = operation == "empty" ? null : actor.TargetLocalItem(arguments.PopSpeech());
				if (operation != "empty" && (other is null || other == item || !eligible(other))) return false;
				var from = (operation == "fill" ? other : item)!.GetItemType<ILiquidContainer>();
				var to = (operation == "fill" ? item : other)?.GetItemType<ILiquidContainer>();
				if (from is null || !from.IsOpen || from.LiquidMixture?.IsEmpty != false ||
				    operation != "fill" && (!CanMove(item) || !from.CanBeEmptiedWhenInRoom)) return false;
				if (operation != "empty" && (to is null || !to.IsOpen || to.LiquidMixture?.CanMerge(from.LiquidMixture) == false)) return false;
				var amount = from.LiquidVolume;
				if (!arguments.IsFinished)
				{
					amount = actor.Gameworld.UnitManager.GetBaseUnits(arguments.SafeRemainingArgument, UnitType.FluidVolume, out var valid);
					if (!valid || !double.IsFinite(amount) || amount <= 0) { error = "Specify a finite positive liquid volume."; return false; }
				}
				amount = Math.Min(amount, from.LiquidVolume);
				if (to is not null) amount = Math.Min(amount, to.LiquidCapacity - to.LiquidVolume);
				if (!double.IsFinite(amount) || amount <= 0) { error = "There is no liquid available or no capacity in the destination."; return false; }
				execute = () =>
				{
					if (to is not null) to.MergeLiquid(from.RemoveLiquidAmount(amount, actor, "pour"), actor, "pour");
					else
					{
						var mixture = new LiquidMixture(from.LiquidMixture);
						mixture.SetLiquidVolume(amount);
						PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, actor.Location, actor.RoomLayer, actor);
						from.ReduceLiquidQuantity(amount, actor, "empty");
					}
					return true;
				};
				return true;
			default:
				error = $"Use {Syntax}.";
				return false;
		}
	}
}
